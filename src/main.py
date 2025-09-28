from contextlib import asynccontextmanager

from datamodel.entities import TestEntity
from infrastructure.persistence.database_client import DatabaseClient

from fastapi import FastAPI, HTTPException, Request

from infrastructure.notion_client import NotionClient
from parser.parse_website import parse_recipe
from datamodel.recipe_dtos import ParsedRecipeDto, RecipeRequestDto

import logging
from logging import getLogger

from src.utils.auth_utils import verify_webhook_token

logging.basicConfig(
    level=logging.INFO, format="%(asctime)s %(levelname)s %(name)s: %(message)s"
)
logger = getLogger(__name__)
notion_client: NotionClient | None = None
db_client: DatabaseClient | None = None


@asynccontextmanager
async def lifespan(app: FastAPI):
    global notion_client, db_client
    notion_client = NotionClient()
    db_client = DatabaseClient()
    db_client.database.create_tables([TestEntity])
    yield
    if db_client:
        db_client.disconnect()


app = FastAPI(lifespan=lifespan)

recipe_types = ["Main Dish", "Dessert", "Side Dish", "Breakfast", "Test"]


@app.post("/recipe/parse")
def parse_recipe_endpoint(request: RecipeRequestDto):
    if request.recipe_type not in recipe_types:
        raise HTTPException(status_code=400, detail="Invalid recipe type.")
    try:
        recipe: ParsedRecipeDto = parse_recipe(request.url, request.recipe_type)
        logger.info(f"Recipe parsed: {recipe.name}")
        return recipe
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))


@app.post("/recipe")
def save_recipe_endpoint(recipe: ParsedRecipeDto):
    if not recipe.ingredients or not recipe.instructions:
        raise HTTPException(
            status_code=400, detail="Recipe must have ingredients and instructions."
        )

    if recipe.recipe_type not in recipe_types:
        raise HTTPException(status_code=400, detail="Invalid recipe type.")

    try:
        notion_client.save_recipe(recipe)
        logger.info(f"Recipe saved to Notion: {recipe.name}")
        return {"message": f"Recipe '{recipe.name}' saved to Notion."}
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))


@app.post("/webhook/notion")
async def notion_webhook(request: Request):
    """Receive webhook from Notion when database is updated"""

    token = request.query_params.get("token")
    if not verify_webhook_token(token):
        logger.warning("Invalid webhook token received")
        raise HTTPException(status_code=401, detail="Invalid authentication token")

    try:
        payload = await request.json()
        logger.info(f"Received Notion webhook: {payload}")

        # Extract page data from webhook payload
        if payload.get("type") == "page" and payload.get("action") == "updated":
            page_id = payload["data"]["object"]["id"]

            # Get page properties from Notion
            url = payload["data"]["object"]["properties"].get("URL", {}).get("url")
            recipe_type = (
                payload["data"]["object"]["properties"]
                .get("Tags", {})
                .get("multi_select", [{}])[0]
                .get("name", "Main Dish")
            )

            if url and recipe_type:
                # Parse the recipe
                recipe = parse_recipe(url, recipe_type)

                # Update the same page with parsed data
                notion_client.update_recipe(page_id, recipe)

                logger.info(f"Recipe processed and updated: {recipe.name}")
                return {"status": "success", "recipe_name": recipe.name}

        return {"status": "ignored", "message": "No action needed"}

    except Exception as e:
        logger.error(f"Webhook processing failed: {str(e)}")
        raise HTTPException(status_code=500, detail=str(e))

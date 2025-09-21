from contextlib import asynccontextmanager

from datamodel.entities import TestEntity
from infrastructure.persistence.database_client import DatabaseClient

from fastapi import FastAPI, HTTPException

from infrastructure.notion_client import NotionClient
from parser.parse_website import parse_recipe
from datamodel.recipe_dtos import ParsedRecipeDto, RecipeRequestDto

import logging
from logging import getLogger

logging.basicConfig(level=logging.INFO, format="%(asctime)s %(levelname)s %(name)s: %(message)s")
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
        raise HTTPException(status_code=400, detail="Recipe must have ingredients and instructions.")

    if recipe.recipe_type not in recipe_types:
        raise HTTPException(status_code=400, detail="Invalid recipe type.")

    try:
        notion_client.save_recipe(recipe)
        logger.info(f"Recipe saved to Notion: {recipe.name}")
        return {"message": f"Recipe '{recipe.name}' saved to Notion."}
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))


@app.post("/test")
def test_endpoint():
    try:
        db_client.connect()
        test_entity = TestEntity.create(id=1, name="Test Name")
        fetched_entity = TestEntity.get(TestEntity.id == 1)
        db_client.disconnect()
        return {"id": fetched_entity.id, "name": fetched_entity.name}
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))
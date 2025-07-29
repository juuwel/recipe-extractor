import dotenv
dotenv.load_dotenv(".env")

from fastapi import FastAPI, HTTPException
from pydantic import BaseModel

from infrastructure.notion_client import NotionClient
from parser.parse_website import parse_recipe
from parser_recipe import ParsedRecipe

import logging
from logging import getLogger

logging.basicConfig(level=logging.INFO, format="%(asctime)s %(levelname)s %(name)s: %(message)s")
logger = getLogger(__name__)
app = FastAPI()
notion_client = NotionClient()
recipe_types = ["Main Dish", "Dessert", "Side Dish", "Breakfast"]

class RecipeRequest(BaseModel):
    url: str
    recipe_type: str

@app.post("/parse-recipe")
def parse_recipe_endpoint(request: RecipeRequest):
    if request.recipe_type not in recipe_types:
        raise HTTPException(status_code=400, detail="Invalid recipe type.")
    try:
        recipe: ParsedRecipe = parse_recipe(request.url, request.recipe_type)
        logger.info(f"Recipe parsed: {recipe}")

        # notion_client.save_recipe(recipe)
        logger.info(f"Recipe saved to Notion: {recipe.name}")
        return {
            "message": f"Recipe '{recipe.name}' saved to Notion under type '{request.recipe_type}'.",
            "recipe": recipe.dict() if hasattr(recipe, "dict") else str(recipe)
        }
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))
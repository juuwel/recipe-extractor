import logging
from contextlib import asynccontextmanager
from logging import getLogger

from fastapi import FastAPI, HTTPException, Request
from src.datamodel.recipe_dtos import ParsedRecipeDto, RecipeRequestDto
from src.parser.parse_website import parse_recipe

logging.basicConfig(
    level=logging.INFO, format="%(asctime)s %(levelname)s %(name)s: %(message)s"
)
logger = getLogger(__name__)

@asynccontextmanager
async def lifespan(app: FastAPI):
    yield


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
    
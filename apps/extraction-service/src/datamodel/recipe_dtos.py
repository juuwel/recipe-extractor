from typing import List

from pydantic import BaseModel


class ParsedRecipeDto(BaseModel):
    website: str
    name: str
    ingredients: List[str]
    instructions: List[str]
    recipe_type: str = "Main Dish"


class RecipeRequestDto(BaseModel):
    url: str
    recipe_type: str

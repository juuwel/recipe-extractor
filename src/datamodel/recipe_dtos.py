from pydantic import BaseModel
from typing import List


class ParsedRecipeDto(BaseModel):
    website: str
    name: str
    ingredients: List[str]
    instructions: List[str]
    recipe_type: str = "Main Dish"


class RecipeRequestDto(BaseModel):
    url: str
    recipe_type: str

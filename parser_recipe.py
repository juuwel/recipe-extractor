from pydantic import BaseModel
from typing import List

class ParsedRecipe(BaseModel):
    website: str
    name: str
    ingredients: List[str]
    instructions: List[str]
    recipe_type: str = "Main Dish"
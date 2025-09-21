import requests
from bs4 import BeautifulSoup

from parser.ingredients import extract_ingredients
from parser.steps import extract_steps
from datamodel.recipe_dtos import ParsedRecipeDto

user_agent_header = {
    "User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36"
}

def parse_recipe(website_url: str, recipe_type: str) -> ParsedRecipeDto:
    """
    Parse the website to extract relevant information.

    Args:
        recipe_type: (str): The type of the recipe (e.g., "Main Dish", "Dessert").
        website_url (str): The URL of the website to parse.

    Returns:
        dict: A dictionary containing parsed information.
        :param recipe_type: The type of the recipe (e.g., "Main Dish", "Dessert").
    """
    response = requests.get(website_url, headers=user_agent_header)

    if response.status_code != 200:
        raise ValueError(f"Failed to retrieve the page. Status code: {response.status_code}")

    soup = BeautifulSoup(response.text, "html.parser")
    ingredients = extract_ingredients(soup)

    steps = extract_steps(soup)

    return ParsedRecipeDto(
        website=website_url,
        name=soup.find("h1").get_text(strip=True) if soup.find("h1") else "No title found",
        ingredients=ingredients,
        instructions=steps,
        recipe_type=recipe_type
    )
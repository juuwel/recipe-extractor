import requests
from bs4 import BeautifulSoup

from parser.ingredients import extract_ingredients
from parser.steps import extract_steps
from parser_recipe import ParsedRecipe


def parse_recipe(website_url: str, recipe_type: str) -> ParsedRecipe:
    """
    Parse the website to extract relevant information.

    Args:
        website_url (str): The URL of the website to parse.

    Returns:
        dict: A dictionary containing parsed information.
    """
    response = requests.get(website_url)
    soup = BeautifulSoup(response.text, "html.parser")
    ingredients = extract_ingredients(soup)

    steps = extract_steps(soup)

    return ParsedRecipe(
        website=website_url,
        name=soup.find("h1").get_text(strip=True) if soup.find("h1") else "No title found",
        ingredients=ingredients,
        instructions=steps,
        type=recipe_type
    )
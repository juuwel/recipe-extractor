import requests
from bs4 import BeautifulSoup

from parser.ingredients import extract_ingredients
from parser.steps import extract_steps

inputUrl = "https://tchiboblog.sk/halusky-s-bryndzou/"
response = requests.get(inputUrl)
soup = BeautifulSoup(response.text, "html.parser")
ingredients = extract_ingredients(soup)

print("Ingredients:")
for ing in ingredients:
    print(f"- {ing}")

steps = extract_steps(soup)
print("\nSteps:")
for step in steps:
    print(f"- {step}")
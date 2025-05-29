import dotenv
dotenv.load_dotenv(".env")

from infrastructure.notion_client import NotionClient
from parser.parse_website import parse_recipe
from parser_recipe import ParsedRecipe

notion_client = NotionClient()
recipe_types = ["Main Dish", "Dessert", "Side Dish", "Breakfast"]

input_url = input("Enter recipe URL: ").strip()
while True:
    print("Select recipe type:")
    for idx, t in enumerate(recipe_types, 1):
        print(f"{idx}. {t}")
    try:
        type_choice = int(input("Enter number: "))
        if 1 <= type_choice <= len(recipe_types):
            recipe_type = recipe_types[type_choice - 1]
            break
        else:
            print("Invalid number. Please try again.")
    except ValueError:
        print("Please enter a valid number.")

recipe: ParsedRecipe = parse_recipe(input_url, recipe_type)
print(f"Finished parsing recipe: {recipe.name}")

notion_client.save_recipe(recipe)
print(f"Recipe '{recipe.name}' saved to Notion under type '{recipe_type}'.")
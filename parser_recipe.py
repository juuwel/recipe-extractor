class ParsedRecipe:
    def __init__(self, website: str, name: str, ingredients: list[str], instructions: str, type: str = "Main Dish"):
        self.website = website
        self.name = name
        self.ingredients = ingredients
        self.instructions = instructions
        self.type = type

    def __repr__(self):
        return f"ParsedRecipe(name={self.name}, ingredients={self.ingredients}, instructions={self.instructions})"
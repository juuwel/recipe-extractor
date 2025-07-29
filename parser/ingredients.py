COMMON_INGREDIENT_CLASS_NAMES = [
    "ingredient",
    "ingredients",
    "recipe-ingredient",
    "recipe-ingredients",
    "ingredient-list"
]

COMMON_INGREDIENT_HEADINGS = [
    "ingredients",
    "ingredient list",
    "what you need",
    "ingredients list"
]

def extract_ingredients(soup):
    # 1. schema.org
    ingredients = [tag.get_text(" ", strip=True) for tag in soup.find_all(attrs={"itemprop": "recipeIngredient"})]
    if ingredients:
        return ingredients

    # 2. Common class names
    for class_name in COMMON_INGREDIENT_CLASS_NAMES:
        for tag in soup.find_all(class_=class_name):
            ingredients += [li.get_text(" ", strip=True) for li in tag.find_all("li")]
    if ingredients:
        return ingredients

    # 3. Look for headings like "Ingredients" (typo-tolerant)
    headings = soup.find_all(["h2", "h3", "h4"])
    for heading in headings:
        if heading.get_text(strip=True).lower() in COMMON_INGREDIENT_HEADINGS:
            # Walk up the ancestor chain (up to 3 levels)
            ancestor = heading
            for _ in range(4):
                ancestor = ancestor.parent
                if not ancestor:
                    break
                for lst in ancestor.find_all(["ul", "ol"]):
                    items = [li.get_text(" ", strip=True) for li in lst.find_all("li")]
                    if items:
                        return items

    # 4. Heuristic: find the list with most food-like items, but skip comment sections
    all_lists = soup.find_all(["ul", "ol"])
    best_list = []
    food_keywords = ["g", "ml", "cup", "tbsp", "tsp", "salt", "sugar", "flour", "egg", "oil", "butter"]
    for ul in all_lists:
        # Skip lists inside comments or unrelated sections
        if ul.find_parent(class_=["comment", "comments", "footer", "related"]):
            continue
        # Skip if any li has unwanted class
        if any(li.get("class") and any(c in ["comment", "comments", "footer", "related"] for c in li.get("class")) for
               li in ul.find_all("li")):
            continue
        items = [li.get_text(" ", strip=True) for li in ul.find_all("li")]
        score = sum(any(word in item.lower() for word in food_keywords) for item in items)
        if score > len(best_list):
            best_list = items
    return best_list

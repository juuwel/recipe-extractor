def extract_ingredients(soup):
    # 1. schema.org
    ingredients = [tag.get_text(strip=True) for tag in soup.find_all(attrs={"itemprop": "recipeIngredient"})]
    if ingredients:
        return ingredients

    # 2. Common class names
    class_names = ["ingredient", "ingredients", "recipe-ingredients"]
    for class_name in class_names:
        for tag in soup.find_all(class_=class_name):
            ingredients += [li.get_text(strip=True) for li in tag.find_all("li")]
    if ingredients:
        return ingredients

    # 3. Heuristic: find the list with most food-like items
    all_lists = soup.find_all(["ul", "ol"])
    best_list = []
    food_keywords = ["g", "ml", "cup", "tbsp", "tsp", "salt", "sugar", "flour", "egg", "oil", "butter"]
    for ul in all_lists:
        items = [li.get_text(strip=True) for li in ul.find_all("li")]
        score = sum(any(word in item.lower() for word in food_keywords) for item in items)
        if score > len(best_list):
            best_list = items
    return best_list
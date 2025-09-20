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

def ancestor_search(tag, levels=4) -> list | None:
    ancestor = tag
    for _ in range(levels):
        ancestor = ancestor.parent
        if not ancestor:
            break
        for lst in ancestor.find_all(["ul", "ol"]):
            items = [li.get_text(" ", strip=True) for li in lst.find_all("li")]
            if items:
                return items

    return None


def sibling_search(tag, levels=4) -> list | None:
    # Check next siblings
    for sibling in tag.find_next_siblings():
        if sibling.name in ["ul", "ol"]:
            items = [li.get_text(" ", strip=True) for li in sibling.find_all("li")]
            if items:
                return items
        # Stop if we hit another heading
        if sibling.name in ["h2", "h3", "h4"]:
            break

    return None

def find_by_headings(soup):
    """
    Find elements by headings, looking for lists of ingredients.

    Args:
        soup (BeautifulSoup): The BeautifulSoup object to search in.

    Returns:
        list: List of ingredients found under the specified headings.
    """
    candidate_lists = []
    for heading in soup.find_all(["h2", "h3", "h4"]):
        heading_text = heading.get_text(" ", strip=True).lower()
        if any(h in heading_text for h in COMMON_INGREDIENT_HEADINGS):
            # Check immediate next sibling
            search_result = sibling_search(heading)
            if search_result:
                candidate_lists.append(search_result)
                continue

            # If not found in siblings, search ancestors
            search_result = ancestor_search(heading)
            if search_result:
                candidate_lists.append(search_result)
                continue

            sibling = heading.next_sibling
            while sibling:
                if getattr(sibling, "name", None) in ["ul", "ol"]:
                    items = [li.get_text(" ", strip=True) for li in sibling.find_all("li")]
                    if items:
                        candidate_lists.append(items)
                elif getattr(sibling, "name", None) in ["div", "section"]:
                    for lst in sibling.find_all(["ul", "ol"]):
                        items = [li.get_text(" ", strip=True) for li in lst.find_all("li")]
                        if items:
                            candidate_lists.append(items)
                sibling = sibling.next_sibling

    if len(candidate_lists) == 0:
        return None
    elif len(candidate_lists) == 1:
        return candidate_lists[0]
    else:
        # Score lists for measurement/food keywords and numbers
        def score_list(items):
            keywords = ["g", "ml", "cup", "tbsp", "tsp", "oz", "lb", "teaspoon", "tablespoon"]
            score = 0
            for item in items:
                item_lower = item.lower()
                has_keyword = any(word in item_lower for word in keywords)
                has_digit = any(char.isdigit() for char in item)
                # Reward items that look like ingredients
                if has_keyword and has_digit:
                    score += 3
                elif has_keyword or has_digit:
                    score += 1
                # Penalize very short items (likely not ingredients)
                if len(item) < 10:
                    score -= 1
                # Penalize very long items (likely not ingredients)
                if len(item) > 100:
                    score -= 3
            return score
        return max(candidate_lists, key=score_list)


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
    if ingredients := find_by_headings(soup):
        return ingredients

    # 4. Heuristic: find the list with most food-like items, but skip comment sections
    all_lists = soup.find_all(["ul", "ol"])
    best_list = []
    food_keywords = ["g", "ml", "cup", "tbsp", "tsp", "salt", "sugar", "flour", "egg", "oil", "butter"]
    for ul in all_lists:
        # Skip lists inside comments, nav, header, or unrelated sections
        if ul.find_parent(class_=["comment", "comments", "footer", "related", "nav", "menu", "header"]) or \
           ul.find_parent(["nav", "header"]):
            continue
        # Skip if any li has unwanted class
        if any(li.get("class") and any(c in ["comment", "comments", "footer", "related", "nav", "menu", "header"] for c in li.get("class")) for
               li in ul.find_all("li")):
            continue
        items = [li.get_text(" ", strip=True) for li in ul.find_all("li")]
        score = sum(any(word in item.lower() for word in food_keywords) for item in items)
        if score > len(best_list):
            best_list = items
    return best_list
def extract_steps(soup):
    # 1. schema.org
    steps = []
    for tag in soup.find_all(attrs={"itemprop": "recipeInstructions"}):
        # Some sites use <li>, some use <span>, some use <div>
        if tag.name in ["ol", "ul"]:
            steps += [li.get_text(strip=True) for li in tag.find_all("li")]
        else:
            steps.append(tag.get_text(strip=True))
    if steps:
        return steps

    # 2. Common class names
    class_names = ["instruction", "instructions", "step", "steps", "directions"]
    for class_name in class_names:
        for tag in soup.find_all(class_=class_name):
            steps += [li.get_text(strip=True) for li in tag.find_all("li")]
            # If not a list, just get the text
            if not steps and tag.get_text(strip=True):
                steps.append(tag.get_text(strip=True))
    if steps:
        return steps

    # 3. Heuristic: find the <ol> or <ul> with the most step-like items
    all_lists = soup.find_all(["ol", "ul"])
    best_list = []
    for ol in all_lists:
        items = [li.get_text(strip=True) for li in ol.find_all("li")]
        # Heuristic: steps often start with verbs and are full sentences
        score = sum(1 for item in items if len(item.split()) > 3)
        if score > len(best_list):
            best_list = items
    return best_list
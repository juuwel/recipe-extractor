import pytest

from parser_recipe import ParsedRecipe
from fastapi.testclient import TestClient
from main import app

expected_results = [
    ParsedRecipe(
        website="https://thehappypear.ie/plant-based-and-vegan-recipes/chickpea-curry",
        name="Chickpea Curry",
        ingredients=[
            '½ red onion', '2 tbsp olive oil', '1 clove of garlic', '½ thumb-sized piece of ginger',
            '½ a red chili if you like it spice like Dave, leave the seeds in!', '1 tbsp curry powder',
            '1 tsp cumin powder', '1 tsp ground coriander', '1 tsp ground paprika',
            '1 g x 400g tin chopped tomatoes', '1 g x 400g tin coconut milk',
            '1 g x 400g tin of chickpeas drained and rinsed', '1 tsp salt', '½ tsp ground black pepper',
            'small bunch of coriander', 'zest of ½ lime', 'juice of ½ lime', '1 avocado'],
        instructions=[
            'Heat the oil on a medium heat.', 'Thinly slice the onion and garlic and add to the pan.',
            'Grate the ginger into the pan. No need to remove the skin!',
            'Thinly slice the chilli and add to the pan.', 'Add the spices and cook for 30 seconds.',
            'Add the chopped tomatoes, coconut milk and chickpeas.', 'Add the salt and pepper.',
            'Chop the coriander and add to the pan along with the lime zest.',
            'Add the lime juice, season to taste and serve with avocado and the grain of your choice. Lovely!'
        ]
    ),
    ParsedRecipe(
        website="https://www.noracooks.com/chickpea-curry",
        name="Easy Chickpea Curry",
        ingredients=[
            '▢ 2 tablespoons olive oil', '▢ 1 large onion, sliced or diced', '▢ 3 cloves garlic, minced',
            '▢ 2 tablespoons mild curry paste see Notes for options', '▢ 15 ounce can crushed tomatoes',
            '▢ 13.5 ounce can full fat coconut milk', '▢ (2) 15-ounce cans chickpeas, drained and rinsed',
            '▢ juice from 1/2 lime', '▢ 1 teaspoon sugar', '▢ 1/2 teaspoon salt, plus more to taste',
            '▢ 2 cups baby spinach, sliced into ribbons'],
        instructions=[
            'In a large pan, heat the oil over medium-high heat. Add the onion and sautÃ© for 5 minutes.  Add the garlic and curry paste and stir, cooking for 1-2 minutes.',
            'Pour in the crushed tomatoes, coconut milk and chickpeas. Bring to a boil, then simmer for about 10 minutes, stirring occasionally. Stir in the lime juice, sugar, salt to taste and spinach. Cook for a minute until the spinach has wilted.',
            'Serve with rice and/orEasy Vegan Naan. Sprinkle with chopped cilantro and serve with lime wedges. Enjoy!'
        ]
    ),
    ParsedRecipe(
        website="https://hostthetoast.com/easy-chickpea-curry/",
        name="Easy Chickpea Curry",
        ingredients=[
            '2 tablespoons vegetable oil or coconut oil', '1 medium onion, sliced', '3 cloves garlic, minced',
            '¼ teaspoon crushed red pepper flakes', '1 - 2 tablespoons curry powder', '1 teaspoon cumin',
            '1 (15 ounce) can crushed tomatoes', '1 (13.5 ounce) can coconut milk',
            '2 (15 ounce) cans chickpeas, drained and rinsed', 'Salt and pepper, to taste',
            'Chopped fresh cilantro and lime wedges, for garnish (optional)',
            'Naan bread and rice, to serve (optional)'],
        instructions=[
            'In a large, heavy bottomed pot or high-walled pan, heat the oil over medium-low. Add the sliced onion, garlic, and crushed red pepper to the pot. Cook, stirring occasionally, until the onion is softened and deep golden, about 15 minutes. Add a tablespoon of water at a time if the onions get dry.',
            'Increase the heat to medium. Add the curry powder and cumin and stir until toasted, about 1 minute. Add the crushed tomatoes and gently scrape the bottom of the pan with a wooden or rubber spoon to release the any browned spices or onions stuck to the bottom.',
            'Pour in the coconut milk and add the chickpeas the pot. Stir and reduce to low heat. Let simmer until the sauce is thickened and the chickpeas are slightly softened, about 10 minutes, stirring occasionally. Season with salt and pepper to taste, and adjust other seasonings as necessary.',
            'Garnish with chopped cilantro and serve with lime wedges over basmati rice and/or with naan.'
        ]
    ),
    ParsedRecipe(
        website="https://jessicainthekitchen.com/coconut-chickpea-curry-recipe/",
        name="Coconut Chickpea Curry (Vegan & GF)",
        ingredients=[
            '2 tablespoons coconut oil',
            '1 medium red onion or yellow onion , diced',
            '14 ounces fresh or canned tomatoes , diced (400g)',
            'sea salt & ground black pepper , to taste',
            '16 ounces canned chickpeas , drained & rinsed (454g)',
            '3 garlic cloves , minced',
            '1 ½ tablespoons garam masala – I use this one',
            '1 teaspoon curry powder  I use this one',
            '¼ teaspoon cumin',
            '13.5 ounces canned coconut milk* , (383g)',
            '2 teaspoons coconut flour , OPTIONAL**',
            '1 small lime , juice of'],
        instructions=[
            'In a deep pot over medium-high heat, add the coconut oil.',
            'Add in the onions and tomatoes. Grind some sea salt and ground black pepper over the mixture and stir together. Lower heat to medium and allow to cook down until juices of the tomatoes are naturally released and onions are soft, about 10 minutes.',
            'Add in the chickpeas, garlic,garam masala, curry powder and cumin. Stir to combine.',
            'Add in the coconut milk and stir again. Add in the coconut flour which helps to slightly thicken the curry. Bring the curry to a boil, and then reduce to medium-low so that the mixture continues to simmer for 10 to 12 more minutes.',
            'Taste the curry and season with salt and pepper if you desire. Remove the curry from the heat and squeeze a lime lightly over the top of the curry, stirring to combine. Don’t skip this step!! Allow to cool slightly and then serve. Enjoy!'
        ]
    ),
    ParsedRecipe(
        website="https://thehappypear.ie/plant-based-and-vegan-recipes/chickpea-curry/",
        name="Chickpea Curry",
        ingredients=[
            '½ red onion', '2 tbsp olive oil', '1 clove of garlic', '½ thumb-sized piece of ginger',
            '½ a red chili if you like it spice like Dave, leave the seeds in!', '1 tbsp curry powder',
            '1 tsp cumin powder', '1 tsp ground coriander', '1 tsp ground paprika',
            '1 g x 400g tin chopped tomatoes', '1 g x 400g tin coconut milk',
            '1 g x 400g tin of chickpeas drained and rinsed', '1 tsp salt', '½ tsp ground black pepper',
            'small bunch of coriander', 'zest of ½ lime', 'juice of ½ lime', '1 avocado'],
        instructions=[
            'Heat the oil on a medium heat.', 'Thinly slice the onion and garlic and add to the pan.',
            'Grate the ginger into the pan. No need to remove the skin!',
            'Thinly slice the chilli and add to the pan.', 'Add the spices and cook for 30 seconds.',
            'Add the chopped tomatoes, coconut milk and chickpeas.', 'Add the salt and pepper.',
            'Chop the coriander and add to the pan along with the lime zest.',
            'Add the lime juice, season to taste and serve with avocado and the grain of your choice. Lovely!'
        ]
    )
]

client = TestClient(app)

@pytest.mark.parametrize("expected", expected_results)
def test_extract_ingredients(expected):
    response = client.post("/parse-recipe", json={"url": expected.website, "recipe_type": "Main Dish"})
    assert response.status_code == 200
    data = response.json()
    assert data == expected.__dict__

import os
import requests

from datamodel.recipe_dtos import ParsedRecipeDto

class NotionClient(object):
    page_api_url = "https://api.notion.com/v1/pages/"

    def prepare_headers(self) -> dict:
        return {
            "Authorization": f"Bearer {os.environ['NOTION_API_KEY']}",
            "Content-Type": "application/json",
            "Notion-Version": os.environ["NOTION_VERSION"],
        }

    def save_recipe(self, recipe: ParsedRecipeDto) -> dict:
        headers = self.prepare_headers()

        request_body = {
            "parent": {"database_id": os.environ["NOTION_DATABASE_ID"]},
            "properties": {
                "Name": {
                    "title": [
                        {
                            "text": {
                                "content": recipe.name
                            }
                        }
                    ]
                },
                "Source": {
                    "url": recipe.website
                },
                "Tags": {
                    "multi_select": [
                        {"name": recipe.recipe_type}
                    ]
                },
            },
            "children": [
                            {
                                "type": "heading_2",
                                "heading_2": {
                                    "rich_text": [
                                        {
                                            "type": "text",
                                            "text": {
                                                "content": "Ingredients"
                                            }
                                        }
                                    ]
                                }
                            }
                        ] + [
                            {
                                "type": "bulleted_list_item",
                                "bulleted_list_item": {
                                    "rich_text": [
                                        {
                                            "type": "text",
                                            "text": {
                                                "content": ingredient
                                            }
                                        }
                                    ]
                                }
                            } for ingredient in recipe.ingredients
                        ] + [
                            {
                                "type": "heading_2",
                                "heading_2": {
                                    "rich_text": [
                                        {
                                            "type": "text",
                                            "text": {
                                                "content": "Instructions"
                                            }
                                        }
                                    ]
                                }
                            }
                        ] +
                        [
                            {
                                "type": "numbered_list_item",
                                "numbered_list_item": {
                                    "rich_text": [
                                        {
                                            "type": "text",
                                            "text": {
                                                "content": instruction
                                            }
                                        }
                                    ]
                                }
                            } for instruction in recipe.instructions
                        ]
        }
        response = requests.post(self.page_api_url, headers=headers, json=request_body)

        if response.status_code != 200:
            raise Exception(f"Failed to create entry: {response.text}")

        return response.json()

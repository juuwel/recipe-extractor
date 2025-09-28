import os
import requests

from datamodel.recipe_dtos import ParsedRecipeDto


class NotionClient(object):
    page_api_url = "https://api.notion.com/v1/pages/"
    blocks_api_url = "https://api.notion.com/v1/blocks/"

    def prepare_headers(self) -> dict:
        return {
            "Authorization": f"Bearer {os.environ['NOTION_API_KEY']}",
            "Content-Type": "application/json",
            "Notion-Version": os.environ["NOTION_VERSION"],
        }

    def __prepare_children_blocks(self, recipe: ParsedRecipeDto) -> list:
        return (
            [
                {
                    "type": "heading_2",
                    "heading_2": {
                        "rich_text": [
                            {"type": "text", "text": {"content": "Ingredients"}}
                        ]
                    },
                }
            ]
            + [
                {
                    "type": "bulleted_list_item",
                    "bulleted_list_item": {
                        "rich_text": [{"type": "text", "text": {"content": ingredient}}]
                    },
                }
                for ingredient in recipe.ingredients
            ]
            + [
                {
                    "type": "heading_2",
                    "heading_2": {
                        "rich_text": [
                            {"type": "text", "text": {"content": "Instructions"}}
                        ]
                    },
                }
            ]
            + [
                {
                    "type": "numbered_list_item",
                    "numbered_list_item": {
                        "rich_text": [
                            {"type": "text", "text": {"content": instruction}}
                        ]
                    },
                }
                for instruction in recipe.instructions
            ]
        )

    def save_recipe(self, recipe: ParsedRecipeDto) -> dict:
        headers = self.prepare_headers()

        request_body = {
            "parent": {"data_source_id": os.environ["NOTION_DATABASE_ID"]},
            "properties": {
                "Name": {"title": [{"text": {"content": recipe.name}}]},
                "Source": {"url": recipe.website},
                "Tags": {"multi_select": [{"name": recipe.recipe_type}]},
            },
            "children": self.__prepare_children_blocks(recipe),
        }
        response = requests.post(self.page_api_url, headers=headers, json=request_body)

        if response.status_code != 200:
            raise Exception(f"Failed to create entry: {response.text}")

        return response.json()

    def update_recipe(self, page_id: str, recipe: ParsedRecipeDto) -> dict:
        self.clear_all_blocks(page_id)

        headers = self.prepare_headers()
        url = f"{self.blocks_api_url}{page_id}/children"

        request_body = {"children": self.__prepare_children_blocks(recipe)}
        response = requests.patch(url, headers=headers, json=request_body)

        if response.status_code != 200:
            raise Exception(f"Failed to update entry: {response.text}")

        return response.json()

    def clear_all_blocks(self, block_id: str):
        """Delete all child blocks of a given block (e.g., a page)"""
        headers = self.prepare_headers()
        url = f"{self.blocks_api_url}{block_id}/children?page_size=100"

        # Fetch existing children
        response = requests.get(url, headers=headers)
        if response.status_code != 200:
            raise Exception(f"Failed to fetch children: {response.text}")

        children = response.json().get("results", [])
        for child in children:
            child_id = child["id"]
            del_url = f"{self.blocks_api_url}{child_id}"
            del_response = requests.delete(del_url, headers=headers)
            if del_response.status_code != 200:
                raise Exception(
                    f"Failed to delete block {child_id}: {del_response.text}"
                )

import os

from peewee import SqliteDatabase


class DatabaseClient:
    def __init__(self):
        self.database = self.__get_database(
            connection_string=os.environ["DATABASE_CONNECTION_STRING"]
        )

    def connect(self):
        self.database.connect()

    def disconnect(self):
        self.database.close()

    @staticmethod
    def __get_database(connection_string: str):
        if connection_string.startswith("sqlite://"):
            db_path = connection_string.replace("sqlite://", "")
            return SqliteDatabase(db_path)
        else:
            raise ValueError("Unsupported database type")

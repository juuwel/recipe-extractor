from peewee import Model, IntegerField, CharField

from infrastructure.persistance.database_client import DatabaseClient


class BaseModel(Model):
    class Meta:
        database = DatabaseClient().database

class TestEntity(BaseModel):
    id = IntegerField(primary_key=True)
    name = CharField()

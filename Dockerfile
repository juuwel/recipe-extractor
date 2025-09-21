FROM python:3.11-alpine

COPY . /app
WORKDIR /app
RUN pip install --upgrade pip && pip install .

ENTRYPOINT ["uvicorn", "src.main:app", "--host", "0.0.0.0", "--port", "8000"]
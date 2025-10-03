FROM python:3.13.7-slim

RUN curl -LsSf https://astral.sh/uv/install.sh | sh

COPY . /app
WORKDIR /app

COPY uv.lock pyproject.toml ./

RUN uv sync --frozen --no-dev

COPY src/ ./src/

ENV PYTHONPATH=/app/src

EXPOSE 8000

ENTRYPOINT ["uvicorn", "src.main:app", "--host", "0.0.0.0", "--port", "8000"]
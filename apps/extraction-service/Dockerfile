FROM astral/uv:python3.13-trixie-slim

WORKDIR /app
COPY uv.lock pyproject.toml ./
COPY src/ ./src/

RUN uv sync --frozen --no-dev

ENV PYTHONPATH=/app/src

EXPOSE 8000

ENTRYPOINT ["uv", "run", "uvicorn", "src.main:app", "--host", "0.0.0.0", "--port", "8000"]

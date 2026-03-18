# Microservice API (.NET 8 + PostgreSQL + Docker)

Функционал:
- JWT авторизация
- Refresh tokens
- CRUD для Todo
- Pagination + filters
- Rate limiting
- Swagger
- Unit tests
- Serilog logging

## Run with Docker

```bash
docker compose up --build
```

API: `http://localhost:8080`
Swagger: `http://localhost:8080/swagger`

## Main endpoints

- `POST /api/auth/register`
- `POST /api/auth/login`
- `POST /api/auth/refresh`
- `GET /api/todos?page=1&pageSize=10&isCompleted=true&search=abc`
- `GET /api/todos/{id}`
- `POST /api/todos`
- `PUT /api/todos/{id}`
- `DELETE /api/todos/{id}`

## Local development

1. Запустить PostgreSQL.
2. Обновить `ConnectionStrings:DefaultConnection` в `appsettings.json`.
3. Запустить приложение:

```bash
dotnet run
```

## Tests

```bash
dotnet test
```

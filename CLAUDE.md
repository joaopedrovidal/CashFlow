# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

Solution file is `CashFlow.slnx` (.NET 10).

```bash
dotnet build CashFlow.slnx
dotnet run --project src/CashFlow.Api          # Swagger UI at /swagger in Development
dotnet test CashFlow.slnx
dotnet test tests/Validators.Tests --filter "FullyQualifiedName~RegisterExpenseValidatorTests.Error_Title_Empty"
```

The API requires a local MySQL 8 instance (`cashflowdb`, user/pwd `root`) — the connection string is currently hardcoded in `CashFlowDbContext.OnConfiguring`.

## Architecture

Layered/clean-architecture style; project dependencies flow inward:

- **CashFlow.Api** — controllers, `ExceptionFilter` (global MVC filter), `CultureMilddleware` (sets `CurrentCulture`/`CurrentUICulture` from `Accept-Language`, default `pt-BR`).
- **CashFlow.Application** — one folder per use case (`UseCases/<Entity>/<Action>/`) containing the `*UseCase` class and its FluentValidation `*Validator`. Use cases validate first and throw `ErrorOnValidationException` with the error messages.
- **CashFlow.Communication** — request/response DTOs (`Request*Json`, `Response*Json`) and API-facing enums. These are distinct from domain enums; use cases cast between them.
- **CashFlow.Domain** — entities and domain enums.
- **CashFlow.Exception** — `CashFlowException` base class and `ResourceErrorMessages.resx` (localized error strings; message keys are in Portuguese, e.g. `TITULO_OBRIGATORIO`). Validators must use these resource strings so messages follow the request culture.
- **CashFlow.Infraestructure** — EF Core `CashFlowDbContext` using Pomelo MySQL. (Note the project/folder spellings `Infraestructure` and `DataAcess`.)

Error handling: any `CashFlowException` → 400 with `ResponseErrorJson` (validation exceptions carry a list of errors); any other exception → 500 with `ERRO_DESCONHECIDO`.

No DI yet: use cases are `new`-ed up and instantiate `CashFlowDbContext` directly.

## Tests

xUnit + FluentAssertions. `tests/CommonTestUtilities` holds Bogus-based request builders (e.g. `RequestRegisterExpenseJsonBuilder.Build()`) that produce a valid request; tests mutate one field and assert the specific `ResourceErrorMessages` entry.

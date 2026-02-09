# Project context for GitHub Copilot

## Summary
This workspace is an educational project: Personal Finance Tracker (Single Page App + API).

## Project: Personal Finance Tracker
- Type: SPA (Vue.js 3) frontend + .NET 8 REST API
- Database: PostgreSQL
- Containerization: each component runs in its own Docker container

### Pages / Components
- Dashboard (charts)
- Accounts
- Transactions
- Budgets
- Reports
- Settings
- Login / Register

### Routing
- `/` (Dashboard)
- `/accounts`
- `/accounts/:id`
- `/transactions`
- `/budgets`
- `/reports`
- `/settings`
- `/auth` (login/register)

### Layouts
- Main layout: header + sidebar
- Auth layout: centered
- Admin layout: user management

### Backend
- REST CRUD endpoints for users, accounts, transactions, budgets
- CSV import/export for transactions and reports
- Server: .NET 8 Web API

### Why AI-friendly
- Repetitive CRUD patterns and list/detail pages — easy to scaffold
- Forms, validation rules, and chart wiring are predictable and reproducible
- Many components follow similar props/state patterns, enabling helpful AI completions

### Technologies
- API: .NET 8
- Frontend: Vue.js 3
- Database: PostgreSQL
- Containerization: Docker (one container per service/component)

## How Copilot should help
- Prefer generating concise, idiomatic Vue 3 and .NET 8 snippets
- Scaffold CRUD endpoints, API clients, forms, validation, and chart setup
- When suggesting code, include minimal comments and small usage examples/tests

## Do not suggest
- Secrets, credentials, or private keys
- Large unrelated frameworks or unnecessary external services

## Key files & folders (expected)
- README.md — project overview and usage
- src/ — frontend and backend source (separate folders per service)
- docker/ or compose files — container orchestration
- examples/ or demos/ — runnable example flows

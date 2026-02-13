# E2E Tests for Finance Tracker

This directory contains end-to-end tests for the Finance Tracker application using Playwright.

## Prerequisites

- Node.js 18+ installed
- Docker and Docker Compose installed
- All services running via Docker Compose

## Setup

1. Install dependencies:
```bash
npm install
```

2. Install Playwright browsers:
```bash
npx playwright install chromium
```

## Running Tests

### Start the services first:
```bash
cd ../Docker
docker compose up -d
```

### Run tests:
```bash
npm test
```

### Run tests in headed mode (see browser):
```bash
npm run test:headed
```

### Run tests in UI mode (interactive):
```bash
npm run test:ui
```

## Tests Included

- **health.spec.js**: Tests the health check endpoint and verifies PostgreSQL version is displayed on the index page

## Cleaning Up

Stop the Docker services:
```bash
cd ../Docker
docker compose down
```

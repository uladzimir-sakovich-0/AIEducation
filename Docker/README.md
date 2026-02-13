# Docker Containerization Setup

This folder contains Docker configuration for running the complete FinanceTracker application stack.

## Overview

The Finance Tracker application consists of three containerized services:
- **PostgreSQL Database**: Persistent data storage
- **Backend API**: .NET 9.0 ASP.NET Core API
- **Frontend**: Vue.js 3 application served by Nginx

## Prerequisites

- Docker Desktop installed (Windows/macOS) or Docker Engine (Linux)
- Docker Compose (included with Docker Desktop)

## Quick Start

1. Navigate to the Docker folder:
   ```bash
   cd Docker
   ```

2. Start all services:
   ```bash
   docker compose up -d
   ```

3. Check if containers are running:
   ```bash
   docker compose ps
   ```

4. Access the application:
   - **Frontend**: http://localhost:3000
   - **Backend API**: http://localhost:5270
   - **PostgreSQL**: localhost:5432

## Services

### PostgreSQL Database
- **Container**: financetracker-postgres
- **Port**: 5432
- **Image**: Custom build from `Dockerfile`
- **Credentials**:
  - Database: financetracker
  - Username: financetracker
  - Password: financetracker

### Backend API
- **Container**: financetracker-backend
- **Port**: 5270 (mapped to internal port 8080)
- **Image**: Custom build from `Dockerfile.backend`
- **Health Endpoint**: http://localhost:5270/api/Health

### Frontend Application
- **Container**: financetracker-frontend
- **Port**: 3000 (mapped to internal port 80)
- **Image**: Custom build from `Dockerfile.frontend`

## Useful Commands

### View logs for all services
```bash
docker compose logs -f
```

### View logs for a specific service
```bash
docker compose logs -f backend
docker compose logs -f frontend
docker compose logs -f postgres
```

### Stop all containers
```bash
docker compose stop
```

### Stop and remove all containers
```bash
docker compose down
```

### Stop and remove containers with volumes (deletes all data)
```bash
docker compose down -v
```

### Rebuild a specific service
```bash
docker compose up -d --build backend
docker compose up -d --build frontend
```

### Rebuild all services
```bash
docker compose up -d --build
```

### Access PostgreSQL CLI
```bash
docker exec -it financetracker-postgres psql -U financetracker -d financetracker
```

### Check health of backend API
```bash
curl http://localhost:5270/api/Health
```

## Health Check

The backend API includes a comprehensive health check endpoint that verifies database connectivity and returns PostgreSQL version information:

**Endpoint**: `GET /api/Health`

**Response**:
```json
{
  "status": "Healthy",
  "timestamp": "2026-02-13T19:11:04Z",
  "databaseVersion": "PostgreSQL 16.12 on x86_64-pc-linux-musl, compiled by gcc (Alpine 15.2.0) 15.2.0, 64-bit"
}
```

The frontend automatically displays this information on the main page.

## Data Persistence

Database data is persisted in a Docker volume named `postgres_data`. This means your data will survive container restarts. To completely remove all data, use `docker compose down -v`.

## Development Workflow

### Making Backend Changes

1. Make your code changes in the `Backend/` directory
2. Rebuild and restart the backend service:
   ```bash
   docker compose up -d --build backend
   ```

### Making Frontend Changes

1. Make your code changes in the `Frontend/` directory
2. Rebuild and restart the frontend service:
   ```bash
   docker compose up -d --build frontend
   ```

## Troubleshooting

### Container won't start

Check logs for errors:
```bash
docker compose logs backend
docker compose logs frontend
docker compose logs postgres
```

### Database connection issues

Verify the backend can reach PostgreSQL:
```bash
docker exec -it financetracker-backend ping postgres
```

Check PostgreSQL health:
```bash
docker exec -it financetracker-postgres pg_isready -U financetracker
```

### Port conflicts

If ports 3000, 5270, or 5432 are already in use, you can modify the port mappings in `docker-compose.yml`.

## Security Notes

⚠️ **Important**: The default credentials are for development only. For production:
- Use strong passwords
- Don't commit `.env` files with real credentials
- Consider using Docker secrets or environment-specific configurations
- Configure proper SSL/TLS certificates
- Review and harden CORS settings

## Testing

### Backend Unit Tests
```bash
cd Backend/tests/FinanceTracker.UnitTests
dotnet test
```

### E2E Tests with Playwright
```bash
# Start services first
cd Docker
docker compose up -d

# Run tests
cd ../e2e-tests
npm install
npx playwright install chromium
npm test
```

## Production Deployment

For production deployment, consider:
1. Using secrets management for sensitive data (passwords, connection strings)
2. Configuring proper backup strategies for the PostgreSQL volume
3. Setting up reverse proxy (e.g., Traefik, Nginx) for SSL/TLS
4. Using environment-specific configuration files
5. Implementing monitoring and logging solutions
6. Scaling services based on load requirements

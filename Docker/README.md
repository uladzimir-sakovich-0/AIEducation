# Docker PostgreSQL Setup

This folder contains Docker configuration for running PostgreSQL database for the FinanceTracker application.

## Prerequisites

- Docker Desktop installed on Windows
- Docker Compose (included with Docker Desktop)

## Quick Start

1. Navigate to the Docker folder:
   ```powershell
   cd Docker
   ```

2. Start the PostgreSQL container:
   ```powershell
   docker-compose up -d
   ```

3. Check if the container is running:
   ```powershell
   docker-compose ps
   ```

4. View logs:
   ```powershell
   docker-compose logs -f postgres
   ```

## Database Connection

- **Host**: localhost
- **Port**: 5432
- **Database**: financetracker
- **Username**: financetracker
- **Password**: financetracker

### Connection String for .NET
```
Host=localhost;Port=5432;Database=financetracker;Username=financetracker;Password=financetracker
```

## Useful Commands

### Stop the container
```powershell
docker-compose stop
```

### Stop and remove the container
```powershell
docker-compose down
```

### Stop and remove container with volumes (deletes all data)
```powershell
docker-compose down -v
```

### Rebuild the container
```powershell
docker-compose up -d --build
```

### Access PostgreSQL CLI
```powershell
docker exec -it financetracker-postgres psql -U financetracker -d financetracker
```

## Data Persistence

Database data is persisted in a Docker volume named `postgres_data`. This means your data will survive container restarts. To completely remove all data, use `docker-compose down -v`.

## Customization

You can customize the database configuration by:
1. Copying `.env.example` to `.env`
2. Modifying the values in `.env`
3. Updating `docker-compose.yml` to use the `.env` file

## Security Notes

⚠️ **Important**: The default credentials are for development only. For production:
- Use strong passwords
- Don't commit `.env` files with real credentials
- Consider using Docker secrets or environment-specific configurations

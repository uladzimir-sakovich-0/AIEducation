# PostgreSQL Docker Setup Documentation

**Date Created**: February 13, 2026  
**Purpose**: Document the PostgreSQL database Docker containerization for FinanceTracker application

## Overview

This document describes the setup of PostgreSQL database in a Docker container for the FinanceTracker application. The database runs in an isolated, portable environment that can be easily shared across development teams.

## Project Structure Created

```
AIEducation/
├── Docker/
│   ├── Dockerfile
│   ├── docker-compose.yml
│   ├── .env.example
│   └── README.md
├── Backend/
├── Frontend/
└── Docs/
```

## Files Created

### 1. Dockerfile (`Docker/Dockerfile`)

PostgreSQL 16 Alpine-based image configuration:

```dockerfile
# PostgreSQL Database Dockerfile
FROM postgres:16-alpine

# Set environment variables with defaults (can be overridden in docker-compose)
ENV POSTGRES_DB=financetracker
ENV POSTGRES_USER=financetracker
ENV POSTGRES_PASSWORD=financetracker

# Copy initialization scripts if needed
# COPY ./init-scripts/ /docker-entrypoint-initdb.d/

# Expose PostgreSQL port
EXPOSE 5432
```

**Key Features:**
- Uses Alpine Linux for minimal image size
- PostgreSQL 16 for latest features and security
- Configurable environment variables
- Support for initialization scripts

### 2. Docker Compose File (`Docker/docker-compose.yml`)

Container orchestration configuration:

```yaml
version: '3.8'

services:
  postgres:
    build:
      context: .
      dockerfile: Dockerfile
    container_name: financetracker-postgres
    environment:
      POSTGRES_DB: financetracker
      POSTGRES_USER: financetracker
      POSTGRES_PASSWORD: financetracker
      POSTGRES_HOST_AUTH_METHOD: trust
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data
    networks:
      - financetracker-network
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U financetracker -d financetracker"]
      interval: 10s
      timeout: 5s
      retries: 5
    restart: unless-stopped

volumes:
  postgres_data:
    driver: local

networks:
  financetracker-network:
    driver: bridge
```

**Key Features:**
- Named container for easy reference
- Port mapping (5432:5432)
- Persistent data storage with Docker volumes
- Health check monitoring
- Auto-restart on failure
- Isolated network for security

### 3. Environment Variables Template (`Docker/.env.example`)

```env
# PostgreSQL Configuration
POSTGRES_DB=financetracker
POSTGRES_USER=financetracker
POSTGRES_PASSWORD=financetracker

# Connection string for .NET application
# ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=financetracker;Username=financetracker;Password=financetracker
```

### 4. Documentation (`Docker/README.md`)

Comprehensive usage guide with:
- Prerequisites
- Quick start instructions
- Connection details
- Useful Docker commands
- Data persistence information
- Security notes

## Setup Steps Performed

### 1. Created Docker Folder Structure
```powershell
cd c:\_Projects\_Godel\AIEducation\Docker
```

### 2. Built and Started Container
```powershell
docker-compose up -d
```

**Build Output:**
- Downloaded PostgreSQL 16 Alpine image (105.17MB)
- Created custom image with configuration
- Created network: `docker_financetracker-network`
- Created volume: `docker_postgres_data`
- Started container: `financetracker-postgres`

### 3. Verified Container Status
```powershell
docker-compose ps
```

**Result:**
```
NAME                      STATUS
financetracker-postgres   Up (healthy)   0.0.0.0:5432->5432/tcp
```

### 4. Tested Database Connection
```powershell
docker exec -it financetracker-postgres psql -U financetracker -d financetracker -c "SELECT version();"
```

**Result:**
```
PostgreSQL 16.12 on x86_64-pc-linux-musl, compiled by gcc (Alpine 15.2.0) 15.2.0, 64-bit
```

### 5. Verified Database Creation
```powershell
docker exec -it financetracker-postgres psql -U financetracker -d financetracker -c "\l"
```

**Result:** Database `financetracker` successfully created with UTF8 encoding.

### 6. Health Check Verification
```powershell
docker inspect financetracker-postgres --format='{{.State.Health.Status}}'
```

**Result:** `healthy`

## Database Connection Details

### For Development

**Host**: localhost  
**Port**: 5432  
**Database**: financetracker  
**Username**: financetracker  
**Password**: financetracker  

### Connection Strings

**For .NET Core (Entity Framework Core):**
```
Host=localhost;Port=5432;Database=financetracker;Username=financetracker;Password=financetracker
```

**Add to `appsettings.json` or `appsettings.Development.json`:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=financetracker;Username=financetracker;Password=financetracker"
  }
}
```

## Common Operations

### Start the Database
```powershell
cd Docker
docker-compose up -d
```

### Stop the Database
```powershell
docker-compose stop
```

### Stop and Remove Container (preserves data)
```powershell
docker-compose down
```

### Stop and Remove Container with Data
```powershell
docker-compose down -v
```

### View Logs
```powershell
docker-compose logs -f postgres
```

### Access PostgreSQL CLI
```powershell
docker exec -it financetracker-postgres psql -U financetracker -d financetracker
```

### Rebuild Container
```powershell
docker-compose up -d --build
```

### Check Container Health
```powershell
docker-compose ps
docker inspect financetracker-postgres --format='{{.State.Health.Status}}'
```

## Data Persistence

Database data is stored in a Docker volume named `docker_postgres_data`. This ensures:
- Data persists across container restarts
- Data survives container recreation
- Data is only deleted with `docker-compose down -v`

**Volume Location**: Managed by Docker
**View Volume Details**:
```powershell
docker volume inspect docker_postgres_data
```

## Network Configuration

**Network Name**: `docker_financetracker-network`  
**Network Type**: Bridge  
**Purpose**: Isolates database traffic and allows future services to communicate

This network can be used to connect other services (like the backend API) to the database.

## Test Results Summary

✅ **Container Status**: Running and healthy  
✅ **PostgreSQL Version**: 16.12 (latest stable)  
✅ **Database Created**: financetracker  
✅ **Connection Test**: Successful  
✅ **Health Check**: Passing  
✅ **Port Mapping**: 5432:5432 (accessible from host)  
✅ **Data Persistence**: Volume created and mounted  
✅ **Network**: Isolated network created  

## Security Considerations

### Development Settings (Current)
- Password: Simple (for development only)
- Authentication: Trust mode enabled (for ease of development)
- Network: Bridge network (isolated from host)

### Production Settings (Recommended)
- [ ] Use strong, randomly generated passwords
- [ ] Remove `POSTGRES_HOST_AUTH_METHOD=trust`
- [ ] Use environment variables from secure storage (Azure Key Vault, AWS Secrets Manager)
- [ ] Enable SSL/TLS connections
- [ ] Implement network policies
- [ ] Regular backups
- [ ] Enable PostgreSQL logging and monitoring
- [ ] Limit user privileges (principle of least privilege)

## Integration with FinanceTracker Backend

### Required NuGet Packages

Add to `FinanceTracker.Infrastructure.csproj`:
```xml
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="9.0.0" />
```

### Update appsettings.Development.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=financetracker;Username=financetracker;Password=financetracker"
  }
}
```

### Update Program.cs

```csharp
builder.Services.AddDbContext<FinanceTrackerDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
```

### Apply Migrations

```powershell
cd Backend/src/FinanceTracker.API
dotnet ef migrations add InitialCreate --project ../FinanceTracker.Infrastructure
dotnet ef database update
```

## Troubleshooting

### Container Won't Start
```powershell
# Check logs
docker-compose logs postgres

# Check if port 5432 is already in use
netstat -ano | findstr :5432

# Remove and recreate
docker-compose down -v
docker-compose up -d --build
```

### Can't Connect to Database
```powershell
# Verify container is running
docker-compose ps

# Check health status
docker inspect financetracker-postgres --format='{{.State.Health.Status}}'

# Test from within container
docker exec -it financetracker-postgres psql -U financetracker -d financetracker -c "SELECT 1;"
```

### Data Loss After Restart
- Ensure you're not using `docker-compose down -v` (which deletes volumes)
- Check volume exists: `docker volume ls | findstr postgres`

## Future Enhancements

- [ ] Add database initialization scripts for schema creation
- [ ] Configure automated backups
- [ ] Add pgAdmin container for GUI management
- [ ] Set up database replication for high availability
- [ ] Implement monitoring with Prometheus/Grafana
- [ ] Add migration scripts in initialization
- [ ] Configure connection pooling
- [ ] Set up automated testing with test database

## Additional Resources

- [PostgreSQL Official Documentation](https://www.postgresql.org/docs/)
- [Docker PostgreSQL Image](https://hub.docker.com/_/postgres)
- [Npgsql EF Core Provider](https://www.npgsql.org/efcore/)
- [Docker Compose Documentation](https://docs.docker.com/compose/)

## Changelog

**2026-02-13** - Initial Setup
- Created Docker folder structure
- Implemented PostgreSQL 16 Alpine container
- Created docker-compose configuration
- Added health checks and volume persistence
- Tested and verified database connectivity
- Documented setup process

---

**Maintained by**: Development Team  
**Last Updated**: February 13, 2026  
**Status**: ✅ Active and Tested

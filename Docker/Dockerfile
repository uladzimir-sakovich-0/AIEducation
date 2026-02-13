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

# The postgres image has a default entrypoint and cmd
# that starts the PostgreSQL server

# Finance Tracker

A full-stack finance tracking application with .NET backend and Vue.js 3 frontend.

## Project Structure

- `Backend/` - .NET 9 API with PostgreSQL database
- `Frontend/` - Vue.js 3 application with Vite

## Getting Started

### Backend

1. Navigate to the backend directory:
```bash
cd Backend/src/FinanceTracker.API
```

2. Configure the database connection in `appsettings.json`

3. Run the API:
```bash
dotnet run
```

The backend will be available at http://localhost:5270

### Frontend

1. Navigate to the frontend directory:
```bash
cd Frontend
```

2. Install dependencies:
```bash
npm install
```

3. Start the development server:
```bash
npm run dev
```

The frontend will be available at http://localhost:3000

## Features

- Health check endpoint with real-time status monitoring
- CORS-enabled backend for frontend integration
- Clean, responsive UI with Vue.js 3
- Vite-powered development with hot module replacement
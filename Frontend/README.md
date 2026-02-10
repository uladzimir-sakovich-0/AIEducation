# Finance Tracker Frontend

Vue.js 3 frontend application for the Finance Tracker.

## Prerequisites

- Node.js 18+ 
- npm or yarn

## Getting Started

1. Install dependencies:
```bash
npm install
```

2. Configure the API endpoint (optional):

Create a `.env` file in the Frontend directory to customize the backend API URL:
```
VITE_API_BASE_URL=http://localhost:5270
```

The default value is `http://localhost:5270` if not specified.

3. Make sure the backend is running on the configured URL

4. Start the development server:
```bash
npm run dev
```

The application will be available at http://localhost:3000

## Available Scripts

- `npm run dev` - Start development server
- `npm run build` - Build for production
- `npm run preview` - Preview production build

## Environment Variables

- `VITE_API_BASE_URL` - Backend API base URL (default: http://localhost:5270)

## Features

- Health check endpoint integration
- Automatic health status check on load
- Clean, responsive UI
- Real-time status display

# API Services

This directory contains the frontend API service layer that handles all backend communication.

## Structure

- **apiService.js** - Base API service with common functionality
  - Configurable base URL via `VITE_API_BASE_URL` environment variable
  - Authentication token handling from session storage
  - Error response parsing
  - HTTP method wrappers (GET, POST, PUT, DELETE)

- **categoryService.js** - Categories-specific API operations
  - `getAll()` - Fetch all categories
  - `create(data)` - Create a new category
  - `update(data)` - Update an existing category
  - `delete(id)` - Delete a category

- **index.js** - Central export point for all services

## Usage

### In Vue Components

```javascript
import categoryService from '@/services/categoryService'

export default {
  methods: {
    async loadCategories() {
      try {
        this.categories = await categoryService.getAll()
      } catch (err) {
        this.error = err.message
      }
    }
  }
}
```

### Authentication

The API service automatically includes the authentication token from session storage in all requests:

```javascript
// Token is automatically retrieved from sessionStorage.getItem('authToken')
// and added as: Authorization: Bearer <token>
```

## Adding New Services

To add a new service for another entity (e.g., accounts, transactions):

1. Create a new file: `{entity}Service.js`
2. Import the base API service
3. Implement entity-specific methods using the base service
4. Export the service instance
5. Add export to `index.js`

Example:

```javascript
// accountService.js
import apiService from './apiService'

class AccountService {
  async getAll() {
    return apiService.get('/api/Accounts')
  }
  
  async create(accountData) {
    return apiService.post('/api/Accounts', accountData)
  }
  
  // ... other methods
}

export default new AccountService()
```

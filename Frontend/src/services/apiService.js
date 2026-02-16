/**
 * Base API Service
 * Provides common functionality for all API services including:
 * - Base URL configuration
 * - Authentication token handling
 * - Error response parsing
 */

class ApiService {
  constructor() {
    this.baseUrl = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5270'
  }

  /**
   * Get authentication token from session storage
   * @returns {string|null} The auth token or null if not found
   */
  getAuthToken() {
    return sessionStorage.getItem('authToken')
  }

  /**
   * Get default headers for API requests
   * Includes Content-Type and Authorization if token exists
   * @returns {Object} Headers object
   */
  getHeaders() {
    const headers = {
      'Content-Type': 'application/json'
    }

    const token = this.getAuthToken()
    if (token) {
      headers['Authorization'] = `Bearer ${token}`
    }

    return headers
  }

  /**
   * Parse error response from API
   * Extracts detail or title from JSON error responses
   * @param {Response} response - The fetch response object
   * @returns {Promise<string>} Error message
   */
  async parseErrorResponse(response) {
    let errorMessage = `HTTP error! status: ${response.status}`
    try {
      const errorData = await response.json()
      if (errorData.detail) {
        errorMessage = errorData.detail
      } else if (errorData.title) {
        errorMessage = errorData.title
      }
    } catch {
      // If parsing fails, use the default error message
    }
    return errorMessage
  }

  /**
   * Make a GET request
   * @param {string} endpoint - API endpoint path
   * @returns {Promise<any>} Response data
   */
  async get(endpoint) {
    const response = await fetch(`${this.baseUrl}${endpoint}`, {
      method: 'GET',
      headers: this.getHeaders()
    })

    if (!response.ok) {
      const errorMessage = await this.parseErrorResponse(response)
      throw new Error(errorMessage)
    }

    return response.json()
  }

  /**
   * Make a POST request
   * @param {string} endpoint - API endpoint path
   * @param {Object} data - Request body data
   * @returns {Promise<any>} Response data
   */
  async post(endpoint, data) {
    const response = await fetch(`${this.baseUrl}${endpoint}`, {
      method: 'POST',
      headers: this.getHeaders(),
      body: JSON.stringify(data)
    })

    if (!response.ok) {
      const errorMessage = await this.parseErrorResponse(response)
      throw new Error(errorMessage)
    }

    return response.json()
  }

  /**
   * Make a PUT request
   * @param {string} endpoint - API endpoint path
   * @param {Object} data - Request body data
   * @returns {Promise<void>}
   */
  async put(endpoint, data) {
    const response = await fetch(`${this.baseUrl}${endpoint}`, {
      method: 'PUT',
      headers: this.getHeaders(),
      body: JSON.stringify(data)
    })

    if (!response.ok) {
      const errorMessage = await this.parseErrorResponse(response)
      throw new Error(errorMessage)
    }

    // PUT returns 200 OK with no content
    if (response.status !== 204) {
      return response.json().catch(() => null)
    }
  }

  /**
   * Make a DELETE request
   * @param {string} endpoint - API endpoint path
   * @returns {Promise<void>}
   */
  async delete(endpoint) {
    const response = await fetch(`${this.baseUrl}${endpoint}`, {
      method: 'DELETE',
      headers: this.getHeaders()
    })

    if (!response.ok) {
      const errorMessage = await this.parseErrorResponse(response)
      throw new Error(errorMessage)
    }

    // DELETE returns 200 OK with no content
    if (response.status !== 204) {
      return response.json().catch(() => null)
    }
  }
}

export default new ApiService()

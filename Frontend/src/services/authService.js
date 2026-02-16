/**
 * Authentication Service
 * Handles user authentication and token management
 */

import apiService from './apiService.js'

class AuthService {
  /**
   * Login user with email and password
   * @param {string} email - User email
   * @param {string} password - User password
   * @returns {Promise<Object>} Login response with token
   */
  async login(email, password) {
    const response = await apiService.post('/api/auth/login', {
      email,
      password
    })
    
    // Save token to session storage
    if (response.token) {
      sessionStorage.setItem('authToken', response.token)
      sessionStorage.setItem('userEmail', response.email)
      if (response.expiresAt) {
        sessionStorage.setItem('tokenExpiresAt', response.expiresAt)
      }
    }
    
    return response
  }

  /**
   * Logout user and clear session storage
   */
  logout() {
    sessionStorage.removeItem('authToken')
    sessionStorage.removeItem('userEmail')
    sessionStorage.removeItem('tokenExpiresAt')
  }

  /**
   * Check if user is authenticated
   * @returns {boolean} True if user has a valid token
   */
  isAuthenticated() {
    const token = sessionStorage.getItem('authToken')
    return !!token
  }

  /**
   * Get current user email
   * @returns {string|null} User email or null if not authenticated
   */
  getUserEmail() {
    return sessionStorage.getItem('userEmail')
  }
}

export default new AuthService()

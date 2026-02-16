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
   * @throws {Error} If login fails or response is invalid
   */
  async login(email, password) {
    // Clear any existing session before attempting login
    this.logout()
    
    try {
      const response = await apiService.post('/api/auth/login', {
        email,
        password
      })
      
      // Validate response has required fields
      if (!response || !response.token) {
        throw new Error('Invalid response from server: missing token')
      }
      
      // Save token to session storage only after successful authentication
      sessionStorage.setItem('authToken', response.token)
      sessionStorage.setItem('userEmail', response.email || email)
      if (response.expiresAt) {
        sessionStorage.setItem('tokenExpiresAt', response.expiresAt)
      }
      
      return response
    } catch (error) {
      // Ensure session is cleared on any error
      this.logout()
      throw error
    }
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
    if (!token) {
      return false
    }
    
    // Check if token has expired
    const expiresAt = sessionStorage.getItem('tokenExpiresAt')
    if (expiresAt) {
      const expirationDate = new Date(expiresAt)
      const now = new Date()
      if (now >= expirationDate) {
        // Token expired, clear session
        this.logout()
        return false
      }
    }
    
    return true
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

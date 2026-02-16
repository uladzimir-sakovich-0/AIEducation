/**
 * Accounts API Service
 * Handles all API calls related to accounts
 */

import apiService from './apiService'

class AccountService {
  /**
   * Get all accounts
   * @returns {Promise<Array>} List of accounts
   */
  async getAll() {
    return apiService.get('/api/Accounts')
  }

  /**
   * Create a new account
   * @param {Object} accountData - Account data
   * @param {string} accountData.name - Account name
   * @param {string} accountData.accountType - Account type (Cash or Bank)
   * @param {number} accountData.balance - Account balance
   * @returns {Promise<string>} Created account ID (GUID)
   */
  async create(accountData) {
    return apiService.post('/api/Accounts', accountData)
  }

  /**
   * Update an existing account
   * Note: Backend expects ID in request body, not in URL
   * @param {Object} accountData - Account data
   * @param {string} accountData.id - Account ID (GUID)
   * @param {string} accountData.name - Account name
   * @param {string} accountData.accountType - Account type (Cash or Bank)
   * @param {number} accountData.balance - Account balance
   * @returns {Promise<void>}
   */
  async update(accountData) {
    return apiService.put('/api/Accounts', accountData)
  }

  /**
   * Delete an account
   * @param {string} id - Account ID (GUID)
   * @returns {Promise<void>}
   */
  async delete(id) {
    return apiService.delete(`/api/Accounts/${id}`)
  }
}

export default new AccountService()

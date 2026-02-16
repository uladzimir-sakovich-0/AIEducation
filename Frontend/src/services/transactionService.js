/**
 * Transactions API Service
 * Handles all API calls related to transactions
 */

import apiService from './apiService'

class TransactionService {
  /**
   * Get all transactions
   * @returns {Promise<Array>} List of transactions
   */
  async getAll() {
    return apiService.get('/api/Transactions')
  }

  /**
   * Create a new transaction
   * @param {Object} transactionData - Transaction data
   * @param {string} transactionData.accountId - Account ID (GUID)
   * @param {string} transactionData.categoryId - Category ID (GUID)
   * @param {number} transactionData.amount - Transaction amount (positive for income, negative for expense)
   * @param {string} transactionData.timestamp - ISO timestamp
   * @param {string} transactionData.notes - Optional notes
   * @returns {Promise<string>} Created transaction ID (GUID)
   */
  async create(transactionData) {
    return apiService.post('/api/Transactions', transactionData)
  }

  /**
   * Update an existing transaction
   * Note: Backend expects ID in request body, not in URL
   * @param {Object} transactionData - Transaction data
   * @param {string} transactionData.id - Transaction ID (GUID)
   * @param {string} transactionData.accountId - Account ID (GUID)
   * @param {string} transactionData.categoryId - Category ID (GUID)
   * @param {number} transactionData.amount - Transaction amount (positive for income, negative for expense)
   * @param {string} transactionData.timestamp - ISO timestamp
   * @param {string} transactionData.notes - Optional notes
   * @returns {Promise<void>}
   */
  async update(transactionData) {
    return apiService.put('/api/Transactions', transactionData)
  }

  /**
   * Delete a transaction
   * @param {string} id - Transaction ID (GUID)
   * @returns {Promise<void>}
   */
  async delete(id) {
    return apiService.delete(`/api/Transactions/${id}`)
  }
}

export default new TransactionService()

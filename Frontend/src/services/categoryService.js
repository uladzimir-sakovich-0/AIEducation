/**
 * Categories API Service
 * Handles all API calls related to categories
 */

import apiService from './apiService'

class CategoryService {
  /**
   * Get all categories
   * @returns {Promise<Array>} List of categories
   */
  async getAll() {
    return apiService.get('/api/Categories')
  }

  /**
   * Create a new category
   * @param {Object} categoryData - Category data
   * @param {string} categoryData.name - Category name
   * @returns {Promise<string>} Created category ID (GUID)
   */
  async create(categoryData) {
    return apiService.post('/api/Categories', categoryData)
  }

  /**
   * Update an existing category
   * Note: Backend expects ID in request body, not in URL
   * @param {Object} categoryData - Category data
   * @param {string} categoryData.id - Category ID (GUID)
   * @param {string} categoryData.name - Category name
   * @returns {Promise<void>}
   */
  async update(categoryData) {
    return apiService.put('/api/Categories', categoryData)
  }

  /**
   * Delete a category
   * @param {string} id - Category ID (GUID)
   * @returns {Promise<void>}
   */
  async delete(id) {
    return apiService.delete(`/api/Categories/${id}`)
  }
}

export default new CategoryService()

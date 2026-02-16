<template>
  <MainLayout>
    <div class="d-flex justify-space-between align-center mb-6">
      <div>
        <h1 class="text-h4 mb-1">Categories</h1>
        <p class="text-body-2 text-medium-emphasis">Organize your transactions</p>
      </div>
      <v-btn color="primary" prepend-icon="mdi-plus" @click="dialog = true">
        New
      </v-btn>
    </div>

    <v-alert
      v-if="error"
      type="error"
      variant="tonal"
      class="mb-4"
      closable
      @click:close="error = null"
    >
      {{ error }}
    </v-alert>

    <v-card class="elevation-0">
      <v-card-text class="pa-0">
        <v-table class="custom-table">
          <thead>
            <tr>
              <th>Name</th>
              <th class="text-right">Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="loading">
              <td colspan="2" class="text-center py-8">
                <v-progress-circular indeterminate color="primary"></v-progress-circular>
              </td>
            </tr>
            <tr v-else-if="categories.length === 0">
              <td colspan="2" class="text-center py-8">
                <p class="text-medium-emphasis">No categories yet. Click "New" to create one.</p>
              </td>
            </tr>
            <tr v-else v-for="category in categories" :key="category.id">
              <td>{{ category.name }}</td>
              <td class="text-right">
                <v-btn icon variant="text" size="small" @click="editCategory(category)">
                  <v-icon size="20">mdi-pencil</v-icon>
                </v-btn>
                <v-btn icon variant="text" size="small" color="error" @click="deleteCategory(category)">
                  <v-icon size="20">mdi-delete</v-icon>
                </v-btn>
              </td>
            </tr>
          </tbody>
        </v-table>
      </v-card-text>
    </v-card>

    <!-- Add/Edit Category Dialog -->
    <v-dialog v-model="dialog" max-width="500px">
      <v-card>
        <v-card-title class="text-h5 pa-6">
          {{ editMode ? 'Edit Category' : 'New Category' }}
        </v-card-title>
        <v-card-text class="px-6 pb-2">
          <v-form ref="form">
            <v-text-field
              v-model="formData.name"
              label="Category Name"
              variant="outlined"
              density="comfortable"
              required
            ></v-text-field>
          </v-form>
        </v-card-text>
        <v-card-actions class="px-6 pb-6">
          <v-spacer></v-spacer>
          <v-btn color="grey" variant="text" @click="closeDialog">
            Cancel
          </v-btn>
          <v-btn color="primary" variant="flat" @click="saveCategory">
            Save
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </MainLayout>
</template>

<script>
import MainLayout from '../components/MainLayout.vue'
import categoryService from '../services/categoryService'

export default {
  name: 'Categories',
  components: {
    MainLayout
  },
  data() {
    return {
      dialog: false,
      editMode: false,
      loading: false,
      error: null,
      categories: [],
      formData: {
        name: ''
      },
      editingId: null
    }
  },
  mounted() {
    this.loadCategories()
  },
  methods: {
    async loadCategories() {
      this.loading = true
      this.error = null

      try {
        this.categories = await categoryService.getAll()
      } catch (err) {
        this.error = err.message || 'Failed to load categories'
        console.error('Error loading categories:', err)
      } finally {
        this.loading = false
      }
    },
    editCategory(category) {
      this.editMode = true
      this.editingId = category.id
      this.formData = { ...category }
      this.dialog = true
    },
    async deleteCategory(category) {
      if (!confirm('Are you sure you want to delete this category?')) {
        return
      }

      try {
        await categoryService.delete(category.id)
        
        // Remove from local array after successful deletion
        this.categories = this.categories.filter(c => c.id !== category.id)
      } catch (err) {
        this.error = err.message || 'Failed to delete category'
        console.error('Error deleting category:', err)
      }
    },
    async saveCategory() {
      try {
        if (this.editMode) {
          // Update existing category
          await categoryService.update({
            id: this.editingId,
            name: this.formData.name
          })

          // Update local array
          const index = this.categories.findIndex(c => c.id === this.editingId)
          if (index !== -1) {
            this.categories[index] = { ...this.formData, id: this.editingId }
          }
        } else {
          // Create new category
          const categoryId = await categoryService.create({
            name: this.formData.name
          })

          // Add to local array with the ID from the server
          this.categories.push({
            id: categoryId,
            name: this.formData.name
          })
        }

        this.closeDialog()
      } catch (err) {
        this.error = err.message || 'Failed to save category'
        console.error('Error saving category:', err)
      }
    },
    closeDialog() {
      this.dialog = false
      this.editMode = false
      this.editingId = null
      this.formData = {
        name: ''
      }
    }
  }
}
</script>

<style scoped>
.custom-table {
  background: transparent !important;
}

.custom-table thead tr th {
  font-weight: 600 !important;
  font-size: 0.875rem !important;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  padding: 16px !important;
  border-bottom: 1px solid rgba(255, 255, 255, 0.12) !important;
}

.custom-table tbody tr {
  border-bottom: 1px solid rgba(255, 255, 255, 0.05) !important;
}

.custom-table tbody tr:hover {
  background-color: rgba(255, 255, 255, 0.02) !important;
}

.custom-table tbody tr td {
  padding: 10px 16px !important;
  font-size: 0.9375rem;
  height: 48px;
}
</style>

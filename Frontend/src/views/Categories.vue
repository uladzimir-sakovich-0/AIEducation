<template>
  <MainLayout>
    <v-row>
      <v-col cols="12">
        <h1 class="text-h4 mb-4">Categories</h1>
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="12">
        <v-btn color="primary" prepend-icon="mdi-plus" @click="dialog = true">
          Add Category
        </v-btn>
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="12">
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

        <v-card>
          <v-card-text>
            <v-data-table
              :headers="headers"
              :items="categories"
              :items-per-page="10"
              :loading="loading"
            >
              <template v-slot:item.actions="{ item }">
                <v-btn icon size="small" @click="editCategory(item)">
                  <v-icon>mdi-pencil</v-icon>
                </v-btn>
                <v-btn icon size="small" color="error" @click="deleteCategory(item)">
                  <v-icon>mdi-delete</v-icon>
                </v-btn>
              </template>
              <template v-slot:no-data>
                <div class="text-center pa-4">
                  <p>No categories yet. Click "Add Category" to create one.</p>
                </div>
              </template>
            </v-data-table>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <!-- Add/Edit Category Dialog -->
    <v-dialog v-model="dialog" max-width="500px">
      <v-card>
        <v-card-title>
          <span class="text-h5">{{ editMode ? 'Edit Category' : 'Add Category' }}</span>
        </v-card-title>
        <v-card-text>
          <v-form ref="form">
            <v-text-field
              v-model="formData.name"
              label="Category Name"
              variant="outlined"
              required
            ></v-text-field>
          </v-form>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="grey" variant="text" @click="closeDialog">
            Cancel
          </v-btn>
          <v-btn color="primary" variant="text" @click="saveCategory">
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
      headers: [
        { title: 'Name', key: 'name' },
        { title: 'Actions', key: 'actions', sortable: false }
      ],
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

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
        <v-card>
          <v-card-text>
            <v-data-table
              :headers="headers"
              :items="categories"
              :items-per-page="10"
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

export default {
  name: 'Categories',
  components: {
    MainLayout
  },
  data() {
    return {
      dialog: false,
      editMode: false,
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
  methods: {
    editCategory(category) {
      this.editMode = true
      this.editingId = category.id
      this.formData = { ...category }
      this.dialog = true
    },
    deleteCategory(category) {
      if (confirm('Are you sure you want to delete this category?')) {
        this.categories = this.categories.filter(c => c.id !== category.id)
      }
    },
    saveCategory() {
      if (this.editMode) {
        const index = this.categories.findIndex(c => c.id === this.editingId)
        if (index !== -1) {
          this.categories[index] = { ...this.formData, id: this.editingId }
        }
      } else {
        this.categories.push({
          ...this.formData,
          id: Date.now()
        })
      }
      this.closeDialog()
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

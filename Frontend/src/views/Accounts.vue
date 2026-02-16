<template>
  <MainLayout>
    <v-row>
      <v-col cols="12">
        <h1 class="text-h4 mb-4">Accounts</h1>
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="12">
        <v-btn color="primary" prepend-icon="mdi-plus" @click="dialog = true">
          Add Account
        </v-btn>
      </v-col>
    </v-row>
    <v-row v-if="error">
      <v-col cols="12">
        <v-alert type="error" dismissible @click:close="error = null">
          {{ error }}
        </v-alert>
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="12">
        <v-card>
          <v-card-text>
            <v-data-table
              :headers="headers"
              :items="accounts"
              :items-per-page="10"
              :loading="loading"
            >
              <template v-slot:item.balance="{ item }">
                ${{ item.balance.toFixed(2) }}
              </template>
              <template v-slot:item.createdAt="{ item }">
                {{ formatDate(item.createdAt) }}
              </template>
              <template v-slot:item.actions="{ item }">
                <v-btn icon size="small" @click="editAccount(item)">
                  <v-icon>mdi-pencil</v-icon>
                </v-btn>
                <v-btn icon size="small" color="error" @click="deleteAccount(item)">
                  <v-icon>mdi-delete</v-icon>
                </v-btn>
              </template>
              <template v-slot:no-data>
                <div class="text-center pa-4">
                  <p>No accounts yet. Click "Add Account" to create one.</p>
                </div>
              </template>
            </v-data-table>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <!-- Add/Edit Account Dialog -->
    <v-dialog v-model="dialog" max-width="500px">
      <v-card>
        <v-card-title>
          <span class="text-h5">{{ editMode ? 'Edit Account' : 'Add Account' }}</span>
        </v-card-title>
        <v-card-text>
          <v-form ref="form">
            <v-text-field
              v-model="formData.name"
              label="Account Name"
              variant="outlined"
              required
            ></v-text-field>
            <v-select
              v-model="formData.accountType"
              :items="accountTypes"
              label="Account Type"
              variant="outlined"
              required
            ></v-select>
            <v-text-field
              v-model.number="formData.balance"
              label="Initial Balance"
              type="number"
              step="0.01"
              variant="outlined"
              required
            ></v-text-field>
          </v-form>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="grey" variant="text" @click="closeDialog" :disabled="loading">
            Cancel
          </v-btn>
          <v-btn color="primary" variant="text" @click="saveAccount" :loading="loading">
            Save
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </MainLayout>
</template>

<script>
import MainLayout from '../components/MainLayout.vue'
import accountService from '../services/accountService'

export default {
  name: 'Accounts',
  components: {
    MainLayout
  },
  data() {
    return {
      dialog: false,
      editMode: false,
      headers: [
        { title: 'Name', key: 'name' },
        { title: 'Type', key: 'accountType' },
        { title: 'Balance', key: 'balance' },
        { title: 'Created', key: 'createdAt' },
        { title: 'Actions', key: 'actions', sortable: false }
      ],
      accounts: [],
      accountTypes: ['Cash', 'Bank'],
      formData: {
        name: '',
        accountType: '',
        balance: 0
      },
      editingId: null,
      loading: false,
      error: null
    }
  },
  async mounted() {
    await this.loadAccounts()
  },
  methods: {
    async loadAccounts() {
      try {
        this.loading = true
        this.error = null
        this.accounts = await accountService.getAll()
      } catch (err) {
        this.error = err.message || 'Failed to load accounts'
        console.error('Error loading accounts:', err)
      } finally {
        this.loading = false
      }
    },
    editAccount(account) {
      this.editMode = true
      this.editingId = account.id
      this.formData = { 
        name: account.name,
        accountType: account.accountType,
        balance: account.balance
      }
      this.dialog = true
    },
    async deleteAccount(account) {
      if (confirm('Are you sure you want to delete this account?')) {
        try {
          await accountService.delete(account.id)
          await this.loadAccounts()
        } catch (err) {
          this.error = err.message || 'Failed to delete account'
          console.error('Error deleting account:', err)
        }
      }
    },
    async saveAccount() {
      try {
        this.loading = true
        this.error = null
        
        if (this.editMode) {
          await accountService.update({
            id: this.editingId,
            ...this.formData
          })
        } else {
          await accountService.create(this.formData)
        }
        
        await this.loadAccounts()
        this.closeDialog()
      } catch (err) {
        this.error = err.message || 'Failed to save account'
        console.error('Error saving account:', err)
      } finally {
        this.loading = false
      }
    },
    closeDialog() {
      this.dialog = false
      this.editMode = false
      this.editingId = null
      this.formData = {
        name: '',
        accountType: '',
        balance: 0
      }
    },
    formatDate(date) {
      return new Date(date).toLocaleDateString()
    }
  }
}
</script>

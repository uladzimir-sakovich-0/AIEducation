<template>
  <MainLayout>
    <div class="d-flex justify-space-between align-center mb-6">
      <div>
        <h1 class="text-h4 mb-1">Accounts</h1>
        <p class="text-body-2 text-medium-emphasis">Manage your financial accounts</p>
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

    <!-- Total Balance Card -->
    <v-card class="elevation-0 mb-6">
      <v-card-text class="pa-6">
        <div class="text-body-2 text-medium-emphasis mb-2">Total Balance</div>
        <div class="text-h3 font-weight-medium" :class="getBalanceClass(totalBalance)">{{ formatCurrency(totalBalance) }}</div>
      </v-card-text>
    </v-card>

    <!-- All Accounts Section -->
    <div class="text-h6 mb-4">All Accounts</div>
    
    <v-card class="elevation-0">
      <v-card-text class="pa-0">
        <v-table class="custom-table">
          <thead>
            <tr>
              <th>Account Name</th>
              <th>Type</th>
              <th>Balance</th>
              <th>Last Updated</th>
              <th class="text-right">Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="loading">
              <td colspan="5" class="text-center py-8">
                <v-progress-circular indeterminate color="primary"></v-progress-circular>
              </td>
            </tr>
            <tr v-else-if="accounts.length === 0">
              <td colspan="5" class="text-center py-8">
                <p class="text-medium-emphasis">No accounts yet. Click "New" to create one.</p>
              </td>
            </tr>
            <tr v-else v-for="account in accounts" :key="account.id">
              <td>{{ account.name }}</td>
              <td>{{ account.accountType }}</td>
              <td :class="getBalanceClass(account.balance)">
                {{ formatCurrency(account.balance) }}
              </td>
              <td>{{ formatDate(account.updatedAt || account.createdAt) }}</td>
              <td class="text-right">
                <v-btn icon variant="text" size="small" @click="editAccount(account)">
                  <v-icon size="20">mdi-pencil</v-icon>
                </v-btn>
                <v-btn icon variant="text" size="small" color="error" @click="deleteAccount(account)">
                  <v-icon size="20">mdi-delete</v-icon>
                </v-btn>
              </td>
            </tr>
          </tbody>
        </v-table>
      </v-card-text>
    </v-card>

    <!-- Add/Edit Account Dialog -->
    <v-dialog v-model="dialog" max-width="500px">
      <v-card>
        <v-card-title class="text-h5 pa-6">
          {{ editMode ? 'Edit Account' : 'New Account' }}
        </v-card-title>
        <v-card-text class="px-6 pb-2">
          <v-form ref="form">
            <v-text-field
              v-model="formData.name"
              label="Account Name"
              variant="outlined"
              density="comfortable"
              required
              class="mb-2"
            ></v-text-field>
            <v-select
              v-model="formData.accountType"
              :items="accountTypes"
              label="Account Type"
              variant="outlined"
              density="comfortable"
              required
              class="mb-2"
            ></v-select>
            <v-text-field
              v-model.number="formData.balance"
              label="Balance"
              type="number"
              step="0.01"
              variant="outlined"
              density="comfortable"
              required
              class="mb-2"
            ></v-text-field>
          </v-form>
        </v-card-text>
        <v-card-actions class="px-6 pb-6">
          <v-spacer></v-spacer>
          <v-btn color="grey" variant="text" @click="closeDialog" :disabled="loading">
            Cancel
          </v-btn>
          <v-btn color="primary" variant="flat" @click="saveAccount" :loading="loading">
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
      accounts: [],
      accountTypes: ['Checking', 'Savings', 'Credit', 'Investment'],
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
  computed: {
    totalBalance() {
      return this.accounts.reduce((sum, account) => sum + (account.balance || 0), 0)
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
      if (!date) return '-'
      return new Date(date).toLocaleDateString()
    },
    formatCurrency(amount) {
      return new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD'
      }).format(amount || 0)
    },
    getBalanceClass(balance) {
      if (balance > 0) return 'text-success'
      if (balance < 0) return 'text-error'
      return ''
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

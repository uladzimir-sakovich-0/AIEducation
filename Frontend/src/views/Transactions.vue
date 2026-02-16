<template>
  <MainLayout>
    <div class="d-flex justify-space-between align-center mb-6">
      <div>
        <h1 class="text-h4 mb-1">Transactions</h1>
        <p class="text-body-2 text-medium-emphasis">View and manage your transaction history</p>
      </div>
      <v-btn color="primary" prepend-icon="mdi-plus" @click="dialog = true">
        New
      </v-btn>
    </div>

    <!-- Recent Transactions Section -->
    <div class="text-h6 mb-4">Recent Transactions</div>
    
    <v-card class="elevation-0">
      <v-card-text class="pa-0">
        <v-table class="custom-table">
          <thead>
            <tr>
              <th>Date</th>
              <th>Category</th>
              <th>Notes</th>
              <th>Account</th>
              <th>Amount</th>
              <th class="text-right">Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="transactions.length === 0">
              <td colspan="6" class="text-center py-8">
                <p class="text-medium-emphasis">No transactions yet. Click "New" to create one.</p>
              </td>
            </tr>
            <tr v-else v-for="transaction in transactions" :key="transaction.id">
              <td>{{ formatDate(transaction.timestamp) }}</td>
              <td>{{ getCategoryName(transaction.categoryId) }}</td>
              <td>{{ transaction.notes || '-' }}</td>
              <td>{{ getAccountName(transaction.accountId) }}</td>
              <td :class="transaction.amount < 0 ? 'text-error' : 'text-success'">
                {{ formatAmount(transaction.amount) }}
              </td>
              <td class="text-right">
                <v-btn icon variant="text" size="small" @click="editTransaction(transaction)">
                  <v-icon size="20">mdi-pencil</v-icon>
                </v-btn>
                <v-btn icon variant="text" size="small" color="error" @click="deleteTransaction(transaction)">
                  <v-icon size="20">mdi-delete</v-icon>
                </v-btn>
              </td>
            </tr>
          </tbody>
        </v-table>
      </v-card-text>
    </v-card>

    <!-- Add/Edit Transaction Dialog -->
    <v-dialog v-model="dialog" max-width="500px">
      <v-card>
        <v-card-title class="text-h5 pa-6">
          {{ editMode ? 'Edit Transaction' : 'New Transaction' }}
        </v-card-title>
        <v-card-text class="px-6 pb-2">
          <v-form ref="form">
            <v-select
              v-model="formData.accountId"
              :items="sortedAccounts"
              item-title="name"
              item-value="id"
              label="Account"
              variant="outlined"
              density="comfortable"
              required
              class="mb-2"
            ></v-select>
            <v-select
              v-model="formData.categoryId"
              :items="sortedCategories"
              item-title="name"
              item-value="id"
              label="Category"
              variant="outlined"
              density="comfortable"
              required
              class="mb-2"
            ></v-select>
            <v-radio-group
              v-model="operationType"
              inline
              class="mb-2"
            >
              <v-radio
                label="Expense"
                value="expense"
                color="error"
              ></v-radio>
              <v-radio
                label="Income"
                value="income"
                color="success"
              ></v-radio>
            </v-radio-group>
            <v-text-field
              v-model.number="amountValue"
              label="Amount"
              type="number"
              step="0.01"
              variant="outlined"
              density="comfortable"
              required
              class="mb-2"
              :color="operationType === 'income' ? 'success' : 'error'"
              :class="operationType === 'income' ? 'income-input' : 'expense-input'"
            >
              <template v-slot:prepend-inner>
                <span :class="operationType === 'income' ? 'text-success' : 'text-error'">
                  {{ operationType === 'income' ? '+' : '-' }}
                </span>
              </template>
            </v-text-field>
            <v-textarea
              v-model="formData.notes"
              label="Notes"
              variant="outlined"
              density="comfortable"
              rows="3"
            ></v-textarea>
          </v-form>
        </v-card-text>
        <v-card-actions class="px-6 pb-6">
          <v-spacer></v-spacer>
          <v-btn color="grey" variant="text" @click="closeDialog">
            Cancel
          </v-btn>
          <v-btn color="primary" variant="flat" @click="saveTransaction">
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
import categoryService from '../services/categoryService'
import transactionService from '../services/transactionService'

export default {
  name: 'Transactions',
  components: {
    MainLayout
  },
  data() {
    return {
      dialog: false,
      editMode: false,
      transactions: [],
      accounts: [],
      categories: [],
      formData: {
        accountId: null,
        categoryId: null,
        notes: ''
      },
      operationType: 'expense',
      amountValue: 0,
      editingId: null
    }
  },
  computed: {
    sortedAccounts() {
      return [...this.accounts].sort((a, b) => a.name.localeCompare(b.name))
    },
    sortedCategories() {
      return [...this.categories].sort((a, b) => a.name.localeCompare(b.name))
    }
  },
  mounted() {
    this.loadAccounts()
    this.loadCategories()
    this.loadTransactions()
  },
  methods: {
    async loadAccounts() {
      try {
        this.accounts = await accountService.getAll()
      } catch (err) {
        console.error('Error loading accounts:', err)
      }
    },
    async loadCategories() {
      try {
        this.categories = await categoryService.getAll()
      } catch (err) {
        console.error('Error loading categories:', err)
      }
    },
    async loadTransactions() {
      try {
        this.transactions = await transactionService.getAll()
        // Sort by timestamp descending (newest first)
        this.transactions.sort((a, b) => new Date(b.timestamp) - new Date(a.timestamp))
      } catch (err) {
        console.error('Error loading transactions:', err)
      }
    },
    editTransaction(transaction) {
      this.editMode = true
      this.editingId = transaction.id
      this.formData = { 
        accountId: transaction.accountId,
        categoryId: transaction.categoryId,
        notes: transaction.notes 
      }
      // Set operation type based on amount
      this.operationType = transaction.amount >= 0 ? 'income' : 'expense'
      this.amountValue = Math.abs(transaction.amount)
      this.dialog = true
    },
    async deleteTransaction(transaction) {
      if (confirm('Are you sure you want to delete this transaction?')) {
        try {
          await transactionService.delete(transaction.id)
          await this.loadTransactions()
        } catch (err) {
          console.error('Error deleting transaction:', err)
          alert('Failed to delete transaction. Please try again.')
        }
      }
    },
    async saveTransaction() {
      // Calculate the actual amount based on operation type
      const actualAmount = this.operationType === 'income' ? this.amountValue : -this.amountValue
      
      const transactionData = {
        accountId: this.formData.accountId,
        categoryId: this.formData.categoryId,
        amount: actualAmount,
        timestamp: new Date().toISOString(),
        notes: this.formData.notes || ''
      }
      
      try {
        if (this.editMode) {
          transactionData.id = this.editingId
          await transactionService.update(transactionData)
        } else {
          await transactionService.create(transactionData)
        }
        await this.loadTransactions()
        this.closeDialog()
      } catch (err) {
        console.error('Error saving transaction:', err)
        alert('Failed to save transaction. Please try again.')
      }
    },
    closeDialog() {
      this.dialog = false
      this.editMode = false
      this.editingId = null
      this.formData = {
        accountId: null,
        categoryId: null,
        notes: ''
      }
      this.operationType = 'expense'
      this.amountValue = 0
    },
    formatDate(date) {
      if (!date) return '-'
      const d = new Date(date)
      const year = d.getFullYear()
      const month = String(d.getMonth() + 1).padStart(2, '0')
      const day = String(d.getDate()).padStart(2, '0')
      return `${year}-${month}-${day}`
    },
    formatAmount(amount) {
      const absAmount = Math.abs(amount || 0)
      const formatted = new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD'
      }).format(absAmount)
      return amount < 0 ? `-${formatted}` : `+${formatted}`
    },
    getAccountName(accountId) {
      const account = this.accounts.find(a => a.id === accountId)
      return account ? account.name : '-'
    },
    getCategoryName(categoryId) {
      const category = this.categories.find(c => c.id === categoryId)
      return category ? category.name : '-'
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

/* Income input - green border */
.income-input :deep(.v-field--variant-outlined .v-field__outline__start),
.income-input :deep(.v-field--variant-outlined .v-field__outline__notch),
.income-input :deep(.v-field--variant-outlined .v-field__outline__end) {
  border-color: rgb(var(--v-theme-success)) !important;
}

.income-input :deep(.v-field--variant-outlined.v-field--focused .v-field__outline__start),
.income-input :deep(.v-field--variant-outlined.v-field--focused .v-field__outline__notch),
.income-input :deep(.v-field--variant-outlined.v-field--focused .v-field__outline__end) {
  border-color: rgb(var(--v-theme-success)) !important;
}

/* Expense input - red border */
.expense-input :deep(.v-field--variant-outlined .v-field__outline__start),
.expense-input :deep(.v-field--variant-outlined .v-field__outline__notch),
.expense-input :deep(.v-field--variant-outlined .v-field__outline__end) {
  border-color: rgb(var(--v-theme-error)) !important;
}

.expense-input :deep(.v-field--variant-outlined.v-field--focused .v-field__outline__start),
.expense-input :deep(.v-field--variant-outlined.v-field--focused .v-field__outline__notch),
.expense-input :deep(.v-field--variant-outlined.v-field--focused .v-field__outline__end) {
  border-color: rgb(var(--v-theme-error)) !important;
}
</style>

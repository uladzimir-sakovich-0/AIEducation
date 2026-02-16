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
              <td>{{ transaction.category }}</td>
              <td>{{ transaction.notes || '-' }}</td>
              <td>{{ transaction.account }}</td>
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
            <v-text-field
              v-model="formData.account"
              label="Account"
              variant="outlined"
              density="comfortable"
              required
              class="mb-2"
            ></v-text-field>
            <v-text-field
              v-model="formData.category"
              label="Category"
              variant="outlined"
              density="comfortable"
              required
              class="mb-2"
            ></v-text-field>
            <v-text-field
              v-model.number="formData.amount"
              label="Amount"
              type="number"
              step="0.01"
              variant="outlined"
              density="comfortable"
              hint="Use negative values for expenses, positive for income"
              persistent-hint
              required
              class="mb-2"
            ></v-text-field>
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
      formData: {
        account: '',
        amount: 0,
        category: '',
        notes: ''
      },
      editingId: null
    }
  },
  methods: {
    editTransaction(transaction) {
      this.editMode = true
      this.editingId = transaction.id
      this.formData = { ...transaction }
      this.dialog = true
    },
    deleteTransaction(transaction) {
      if (confirm('Are you sure you want to delete this transaction?')) {
        this.transactions = this.transactions.filter(t => t.id !== transaction.id)
      }
    },
    saveTransaction() {
      if (this.editMode) {
        const index = this.transactions.findIndex(t => t.id === this.editingId)
        if (index !== -1) {
          const originalTransaction = this.transactions[index]
          this.transactions[index] = { 
            ...this.formData, 
            id: this.editingId,
            timestamp: originalTransaction.timestamp
          }
        }
      } else {
        this.transactions.push({
          ...this.formData,
          id: Date.now(),
          timestamp: new Date().toISOString()
        })
      }
      this.closeDialog()
    },
    closeDialog() {
      this.dialog = false
      this.editMode = false
      this.editingId = null
      this.formData = {
        account: '',
        amount: 0,
        category: '',
        notes: ''
      }
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

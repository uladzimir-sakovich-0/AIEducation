<template>
  <MainLayout>
    <v-row>
      <v-col cols="12">
        <h1 class="text-h4 mb-4">Transactions</h1>
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="12">
        <v-btn color="primary" prepend-icon="mdi-plus" @click="dialog = true">
          Add Transaction
        </v-btn>
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="12">
        <v-card>
          <v-card-text>
            <v-data-table
              :headers="headers"
              :items="transactions"
              :items-per-page="10"
            >
              <template v-slot:item.amount="{ item }">
                <span :class="item.amount < 0 ? 'text-error' : 'text-success'">
                  ${{ Math.abs(item.amount).toFixed(2) }}
                </span>
              </template>
              <template v-slot:item.timestamp="{ item }">
                {{ formatDate(item.timestamp) }}
              </template>
              <template v-slot:item.actions="{ item }">
                <v-btn icon size="small" @click="editTransaction(item)">
                  <v-icon>mdi-pencil</v-icon>
                </v-btn>
                <v-btn icon size="small" color="error" @click="deleteTransaction(item)">
                  <v-icon>mdi-delete</v-icon>
                </v-btn>
              </template>
              <template v-slot:no-data>
                <div class="text-center pa-4">
                  <p>No transactions yet. Click "Add Transaction" to create one.</p>
                </div>
              </template>
            </v-data-table>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <!-- Add/Edit Transaction Dialog -->
    <v-dialog v-model="dialog" max-width="500px">
      <v-card>
        <v-card-title>
          <span class="text-h5">{{ editMode ? 'Edit Transaction' : 'Add Transaction' }}</span>
        </v-card-title>
        <v-card-text>
          <v-form ref="form">
            <v-text-field
              v-model="formData.account"
              label="Account"
              variant="outlined"
              required
            ></v-text-field>
            <v-text-field
              v-model.number="formData.amount"
              label="Amount"
              type="number"
              step="0.01"
              variant="outlined"
              hint="Use negative values for expenses"
              persistent-hint
              required
            ></v-text-field>
            <v-text-field
              v-model="formData.category"
              label="Category"
              variant="outlined"
              required
            ></v-text-field>
            <v-textarea
              v-model="formData.notes"
              label="Notes"
              variant="outlined"
              rows="3"
            ></v-textarea>
          </v-form>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="grey" variant="text" @click="closeDialog">
            Cancel
          </v-btn>
          <v-btn color="primary" variant="text" @click="saveTransaction">
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
      headers: [
        { title: 'Date', key: 'timestamp' },
        { title: 'Account', key: 'account' },
        { title: 'Amount', key: 'amount' },
        { title: 'Category', key: 'category' },
        { title: 'Notes', key: 'notes' },
        { title: 'Actions', key: 'actions', sortable: false }
      ],
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
      return new Date(date).toLocaleDateString()
    }
  }
}
</script>

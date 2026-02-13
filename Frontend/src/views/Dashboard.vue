<template>
  <MainLayout>
    <v-row>
      <v-col cols="12">
        <h1 class="text-h4 mb-4">Dashboard</h1>
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="12" md="4">
        <v-card>
          <v-card-title>Total Balance</v-card-title>
          <v-card-text>
            <div class="text-h4">$0.00</div>
            <div class="text-caption">Across all accounts</div>
          </v-card-text>
        </v-card>
      </v-col>
      <v-col cols="12" md="4">
        <v-card>
          <v-card-title>Accounts</v-card-title>
          <v-card-text>
            <div class="text-h4">0</div>
            <div class="text-caption">Active accounts</div>
          </v-card-text>
        </v-card>
      </v-col>
      <v-col cols="12" md="4">
        <v-card>
          <v-card-title>Transactions</v-card-title>
          <v-card-text>
            <div class="text-h4">0</div>
            <div class="text-caption">This month</div>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="12">
        <v-card>
          <v-card-title>Recent Transactions</v-card-title>
          <v-card-text>
            <v-list>
              <v-list-item>
                <v-list-item-title>No transactions yet</v-list-item-title>
                <v-list-item-subtitle>Start by creating an account and adding transactions</v-list-item-subtitle>
              </v-list-item>
            </v-list>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </MainLayout>
</template>

<script>
import MainLayout from '../components/MainLayout.vue'

export default {
  name: 'Dashboard',
  components: {
    MainLayout
  }
}
</script>

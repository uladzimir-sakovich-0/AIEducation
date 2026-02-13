<template>
  <MainLayout>
    <v-row>
      <v-col cols="12">
        <h1 class="text-h4 mb-4">System Health</h1>
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="12" md="8">
        <v-card>
          <v-card-title>Health Status</v-card-title>
          <v-card-text>
            <v-btn
              color="primary"
              @click="checkHealth"
              :loading="loading"
              prepend-icon="mdi-refresh"
            >
              Check Health
            </v-btn>

            <v-alert
              v-if="healthData"
              type="success"
              variant="tonal"
              class="mt-4"
            >
              <v-alert-title>✓ Service is Healthy</v-alert-title>
              <div class="mt-2">
                <p><strong>Status:</strong> {{ healthData.status }}</p>
                <p><strong>Timestamp:</strong> {{ formatTimestamp(healthData.timestamp) }}</p>
                <p v-if="healthData.databaseVersion">
                  <strong>Database Version:</strong> {{ healthData.databaseVersion }}
                </p>
              </div>
            </v-alert>

            <v-alert
              v-if="error"
              type="error"
              variant="tonal"
              class="mt-4"
            >
              <v-alert-title>✗ Health Check Failed</v-alert-title>
              <div class="mt-2">{{ error }}</div>
            </v-alert>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </MainLayout>
</template>

<script>
import MainLayout from '../components/MainLayout.vue'

export default {
  name: 'Health',
  components: {
    MainLayout
  },
  data() {
    return {
      healthData: null,
      error: null,
      loading: false
    }
  },
  mounted() {
    this.checkHealth()
  },
  methods: {
    async checkHealth() {
      this.loading = true
      this.error = null
      this.healthData = null

      try {
        const apiBaseUrl = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5270'
        const response = await fetch(`${apiBaseUrl}/api/Health`)

        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status}`)
        }

        this.healthData = await response.json()
      } catch (err) {
        this.error = err.message || 'Failed to connect to the backend service'
      } finally {
        this.loading = false
      }
    },
    formatTimestamp(timestamp) {
      return new Date(timestamp).toLocaleString()
    }
  }
}
</script>

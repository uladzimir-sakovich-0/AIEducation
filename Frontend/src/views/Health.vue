<template>
  <MainLayout>
    <div class="d-flex justify-space-between align-center mb-6">
      <div>
        <h1 class="text-h4 mb-1">Healthcheck</h1>
        <p class="text-body-2 text-medium-emphasis">Monitor system health and status</p>
      </div>
      <v-btn
        color="primary"
        prepend-icon="mdi-refresh"
        @click="checkHealth"
        :loading="loading"
      >
        Refresh
      </v-btn>
    </div>

    <v-card class="elevation-0">
      <v-card-text class="pa-6">
        <v-alert
          v-if="healthData"
          type="success"
          variant="tonal"
          class="mb-4"
        >
          <v-alert-title class="mb-2">✓ Service is Healthy</v-alert-title>
          <div class="health-details">
            <div class="health-item">
              <span class="text-medium-emphasis">Status:</span>
              <span class="font-weight-medium">{{ healthData.status }}</span>
            </div>
            <div class="health-item">
              <span class="text-medium-emphasis">Timestamp:</span>
              <span class="font-weight-medium">{{ formatTimestamp(healthData.timestamp) }}</span>
            </div>
            <div v-if="healthData.databaseVersion" class="health-item">
              <span class="text-medium-emphasis">Database Version:</span>
              <span class="font-weight-medium">{{ healthData.databaseVersion }}</span>
            </div>
          </div>
        </v-alert>

        <v-alert
          v-if="error"
          type="error"
          variant="tonal"
        >
          <v-alert-title class="mb-2">✗ Health Check Failed</v-alert-title>
          <div class="text-body-2">{{ error }}</div>
        </v-alert>

        <div v-if="!healthData && !error && !loading" class="text-center py-8 text-medium-emphasis">
          Click "Refresh" to check system health
        </div>
      </v-card-text>
    </v-card>
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

<style scoped>
.health-details {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.health-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
</style>

<template>
  <div class="app">
    <header class="header">
      <h1>Finance Tracker</h1>
    </header>
    
    <main class="main-content">
      <div class="health-check">
        <h2>Health Status</h2>
        <button @click="checkHealth" :disabled="loading" class="check-button">
          {{ loading ? 'Checking...' : 'Check Health' }}
        </button>
        
        <div v-if="healthData" class="health-result success">
          <h3>✓ Service is Healthy</h3>
          <p><strong>Status:</strong> {{ healthData.status }}</p>
          <p><strong>Timestamp:</strong> {{ formatTimestamp(healthData.timestamp) }}</p>
        </div>
        
        <div v-if="error" class="health-result error">
          <h3>✗ Health Check Failed</h3>
          <p>{{ error }}</p>
        </div>
      </div>
    </main>
  </div>
</template>

<script>
export default {
  name: 'App',
  data() {
    return {
      healthData: null,
      error: null,
      loading: false
    }
  },
  mounted() {
    // Automatically check health on mount
    this.checkHealth()
  },
  methods: {
    async checkHealth() {
      this.loading = true
      this.error = null
      this.healthData = null
      
      try {
        const response = await fetch('http://localhost:5270/api/Health')
        
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
.app {
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, sans-serif;
  max-width: 800px;
  margin: 0 auto;
  padding: 20px;
}

.header {
  text-align: center;
  padding: 20px 0;
  border-bottom: 2px solid #42b983;
  margin-bottom: 40px;
}

.header h1 {
  color: #2c3e50;
  margin: 0;
}

.main-content {
  padding: 20px;
}

.health-check {
  text-align: center;
}

.health-check h2 {
  color: #2c3e50;
  margin-bottom: 20px;
}

.check-button {
  background-color: #42b983;
  color: white;
  border: none;
  padding: 12px 24px;
  font-size: 16px;
  border-radius: 4px;
  cursor: pointer;
  transition: background-color 0.3s;
}

.check-button:hover:not(:disabled) {
  background-color: #35a372;
}

.check-button:disabled {
  background-color: #95c9b4;
  cursor: not-allowed;
}

.health-result {
  margin-top: 30px;
  padding: 20px;
  border-radius: 8px;
  text-align: left;
}

.health-result.success {
  background-color: #f0f9f4;
  border: 2px solid #42b983;
}

.health-result.error {
  background-color: #fef0f0;
  border: 2px solid #f56c6c;
}

.health-result h3 {
  margin-top: 0;
  margin-bottom: 15px;
}

.health-result.success h3 {
  color: #42b983;
}

.health-result.error h3 {
  color: #f56c6c;
}

.health-result p {
  margin: 8px 0;
  color: #2c3e50;
}
</style>

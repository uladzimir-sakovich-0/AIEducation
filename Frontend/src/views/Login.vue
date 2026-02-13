<template>
  <AuthLayout>
    <template #title>
      Finance Tracker Login
    </template>
    <template #content>
      <v-form @submit.prevent="handleLogin">
        <v-text-field
          v-model="email"
          label="Email"
          type="email"
          prepend-inner-icon="mdi-email"
          variant="outlined"
          required
        ></v-text-field>
        <v-text-field
          v-model="password"
          label="Password"
          type="password"
          prepend-inner-icon="mdi-lock"
          variant="outlined"
          required
        ></v-text-field>
        <v-alert
          v-if="error"
          type="error"
          variant="tonal"
          class="mb-4"
        >
          {{ error }}
        </v-alert>
        <v-btn
          type="submit"
          color="primary"
          block
          size="large"
          :loading="loading"
        >
          Login
        </v-btn>
      </v-form>
    </template>
    <template #actions>
      <v-btn
        variant="text"
        color="primary"
        @click="$router.push('/register')"
      >
        Don't have an account? Register
      </v-btn>
    </template>
  </AuthLayout>
</template>

<script>
import AuthLayout from '../components/AuthLayout.vue'

export default {
  name: 'Login',
  components: {
    AuthLayout
  },
  data() {
    return {
      email: '',
      password: '',
      error: null,
      loading: false
    }
  },
  methods: {
    async handleLogin() {
      this.loading = true
      this.error = null
      
      try {
        // TODO: Replace with actual API call when auth endpoints are ready
        // For MVP, we'll use simple localStorage authentication
        if (this.email && this.password) {
          localStorage.setItem('isAuthenticated', 'true')
          localStorage.setItem('userEmail', this.email)
          this.$router.push('/dashboard')
        } else {
          this.error = 'Please enter email and password'
        }
      } catch (err) {
        this.error = err.message || 'Login failed'
      } finally {
        this.loading = false
      }
    }
  }
}
</script>

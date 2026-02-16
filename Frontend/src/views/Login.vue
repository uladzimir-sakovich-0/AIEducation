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
import { authService } from '../services'

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
        if (!this.email || !this.password) {
          this.error = 'Please enter email and password'
          this.loading = false
          return
        }

        console.log('Attempting login...')
        const response = await authService.login(this.email, this.password)
        console.log('Login successful', response)
        
        // Only navigate if login was successful and we have a token
        if (response && response.token) {
          this.$router.push('/dashboard')
        } else {
          throw new Error('Login failed: No authentication token received')
        }
      } catch (err) {
        console.error('Login error:', err)
        this.error = err.message || 'Login failed. Please check your credentials.'
        this.loading = false
      }
    }
  }
}
</script>

<template>
  <AuthLayout>
    <template #title>
      Create Account
    </template>
    <template #content>
      <v-form @submit.prevent="handleRegister">
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
        <v-text-field
          v-model="confirmPassword"
          label="Confirm Password"
          type="password"
          prepend-inner-icon="mdi-lock-check"
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
        <v-alert
          v-if="success"
          type="success"
          variant="tonal"
          class="mb-4"
        >
          {{ success }}
        </v-alert>
        <v-btn
          type="submit"
          color="primary"
          block
          size="large"
          :loading="loading"
        >
          Register
        </v-btn>
      </v-form>
    </template>
    <template #actions>
      <v-btn
        variant="text"
        color="primary"
        @click="$router.push('/login')"
      >
        Already have an account? Login
      </v-btn>
    </template>
  </AuthLayout>
</template>

<script>
import AuthLayout from '../components/AuthLayout.vue'

export default {
  name: 'Register',
  components: {
    AuthLayout
  },
  data() {
    return {
      email: '',
      password: '',
      confirmPassword: '',
      error: null,
      success: null,
      loading: false
    }
  },
  methods: {
    async handleRegister() {
      this.loading = true
      this.error = null
      this.success = null
      
      try {
        // Validation
        if (!this.email || !this.password || !this.confirmPassword) {
          this.error = 'All fields are required'
          return
        }
        
        if (this.password !== this.confirmPassword) {
          this.error = 'Passwords do not match'
          return
        }
        
        if (this.password.length < 6) {
          this.error = 'Password must be at least 6 characters'
          return
        }
        
        // TODO: Replace with actual API call when auth endpoints are ready
        // For MVP, we'll simulate registration
        this.success = 'Registration successful! Redirecting to login...'
        setTimeout(() => {
          this.$router.push('/login')
        }, 2000)
      } catch (err) {
        this.error = err.message || 'Registration failed'
      } finally {
        this.loading = false
      }
    }
  }
}
</script>

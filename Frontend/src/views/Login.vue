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
          return
        }

        await authService.login(this.email, this.password)
        this.$router.push('/dashboard')
      } catch (err) {
        this.error = err.message || 'Login failed'
      } finally {
        this.loading = false
      }
    }
  }
}
</script>

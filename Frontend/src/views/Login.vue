<template>
  <v-app>
    <v-main>
      <v-container fluid fill-height>
        <v-row align="center" justify="center">
          <v-col cols="12" sm="8" md="4">
            <v-card class="elevation-12">
              <v-card-title class="text-h5 text-center pa-6">
                Finance Tracker Login
              </v-card-title>
              <v-card-text>
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
              </v-card-text>
              <v-card-actions class="justify-center pa-4">
                <v-btn
                  variant="text"
                  color="primary"
                  @click="$router.push('/register')"
                >
                  Don't have an account? Register
                </v-btn>
              </v-card-actions>
            </v-card>
          </v-col>
        </v-row>
      </v-container>
    </v-main>
  </v-app>
</template>

<script>
export default {
  name: 'Login',
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

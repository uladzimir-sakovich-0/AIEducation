<template>
  <v-app>
    <v-app-bar color="primary" prominent>
      <v-app-bar-nav-icon @click="drawer = !drawer"></v-app-bar-nav-icon>
      <v-toolbar-title>Finance Tracker</v-toolbar-title>
      <v-spacer></v-spacer>
      <span class="mr-4">{{ userEmail }}</span>
      <v-btn icon @click="logout">
        <v-icon>mdi-logout</v-icon>
      </v-btn>
    </v-app-bar>

    <v-navigation-drawer v-model="drawer" temporary>
      <v-list>
        <v-list-item
          prepend-icon="mdi-account"
          :title="userEmail"
          subtitle="Logged in"
        ></v-list-item>
      </v-list>

      <v-divider></v-divider>

      <v-list density="compact" nav>
        <v-list-item
          v-for="item in menuItems"
          :key="item.title"
          :prepend-icon="item.icon"
          :title="item.title"
          :to="item.to"
          :value="item.title"
        ></v-list-item>
      </v-list>
    </v-navigation-drawer>

    <v-main>
      <v-container fluid>
        <slot></slot>
      </v-container>
    </v-main>
  </v-app>
</template>

<script>
import { authService } from '../services'

export default {
  name: 'MainLayout',
  data() {
    return {
      drawer: false,
      menuItems: [
        { title: 'Dashboard', icon: 'mdi-view-dashboard', to: '/dashboard' },
        { title: 'Accounts', icon: 'mdi-bank', to: '/accounts' },
        { title: 'Categories', icon: 'mdi-tag-multiple', to: '/categories' },
        { title: 'Transactions', icon: 'mdi-cash-multiple', to: '/transactions' },
        { title: 'Health Check', icon: 'mdi-heart-pulse', to: '/health' }
      ]
    }
  },
  computed: {
    userEmail() {
      return authService.getUserEmail() || 'User'
    }
  },
  methods: {
    logout() {
      authService.logout()
      this.$router.push('/login')
    }
  }
}
</script>

<template>
  <v-app>
    <!-- Persistent Side Navigation -->
    <v-navigation-drawer permanent width="220" class="side-nav">
      <!-- User Profile Section -->
      <div class="user-profile">
        <div class="d-flex align-center pa-3">
          <v-avatar color="primary" size="32" class="mr-2">
            <span class="text-body-2">{{ userInitials }}</span>
          </v-avatar>
          <div class="flex-grow-1">
            <div class="text-body-2 font-weight-medium">{{ userName }}</div>
            <div class="text-caption text-medium-emphasis">{{ userEmail }}</div>
          </div>
        </div>
      </div>

      <v-divider></v-divider>

      <!-- Navigation Menu -->
      <v-list density="compact" nav class="py-2">
        <v-list-item
          v-for="item in menuItems"
          :key="item.title"
          :to="item.to"
          :value="item.title"
          :active="isActive(item.to)"
          class="nav-item"
          rounded="lg"
        >
          <template v-slot:prepend>
            <v-icon :icon="item.icon"></v-icon>
          </template>
          <v-list-item-title>{{ item.title }}</v-list-item-title>
        </v-list-item>
      </v-list>
    </v-navigation-drawer>

    <!-- Top App Bar -->
    <v-app-bar elevation="0" class="top-bar">
      <v-toolbar-title class="font-weight-semibold ml-4">
        FinanceTracker
      </v-toolbar-title>
      
      <v-spacer></v-spacer>
      
      <div class="d-flex align-center mr-4">
        <v-icon size="small" class="mr-2">mdi-account</v-icon>
        <span class="text-body-2 mr-4">{{ userEmail }}</span>
        <v-btn icon variant="text" size="small" @click="logout">
          <v-icon>mdi-logout</v-icon>
        </v-btn>
      </div>
    </v-app-bar>

    <!-- Main Content -->
    <v-main>
      <v-container fluid class="pa-6">
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
      menuItems: [
        { title: 'Accounts', icon: 'mdi-wallet', to: '/accounts' },
        { title: 'Transactions', icon: 'mdi-swap-horizontal', to: '/transactions' },
        { title: 'Categories', icon: 'mdi-tag-multiple', to: '/categories' },
        { title: 'Healthcheck', icon: 'mdi-heart-pulse', to: '/health' }
      ]
    }
  },
  computed: {
    userEmail() {
      return authService.getUserEmail() || 'User'
    },
    userName() {
      const email = this.userEmail
      return email.split('@')[0].split('.').map(word => 
        word.charAt(0).toUpperCase() + word.slice(1)
      ).join(' ')
    },
    userInitials() {
      const name = this.userName
      const parts = name.split(' ')
      if (parts.length >= 2) {
        return parts[0][0] + parts[1][0]
      }
      return name.substring(0, 2).toUpperCase()
    }
  },
  methods: {
    logout() {
      authService.logout()
      this.$router.push('/login')
    },
    isActive(path) {
      return this.$route.path === path
    }
  }
}
</script>

<style scoped>
.side-nav {
  border-right: 1px solid rgba(255, 255, 255, 0.12) !important;
  z-index: 1001;
}

.user-profile {
  height: 64px;
  border-bottom: 1px solid rgba(255, 255, 255, 0.12);
  display: flex;
  align-items: center;
}

.nav-item {
  margin: 2px 8px !important;
}

.top-bar {
  border-bottom: 1px solid rgba(255, 255, 255, 0.12) !important;
}
</style>

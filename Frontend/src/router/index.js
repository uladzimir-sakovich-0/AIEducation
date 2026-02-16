import { createRouter, createWebHistory } from 'vue-router'
import { authService } from '../services'
import Login from '../views/Login.vue'
import Register from '../views/Register.vue'
import Accounts from '../views/Accounts.vue'
import Categories from '../views/Categories.vue'
import Transactions from '../views/Transactions.vue'
import Health from '../views/Health.vue'

const routes = [
  {
    path: '/',
    redirect: '/transactions'
  },
  {
    path: '/login',
    name: 'Login',
    component: Login
  },
  {
    path: '/register',
    name: 'Register',
    component: Register
  },
  {
    path: '/accounts',
    name: 'Accounts',
    component: Accounts,
    meta: { requiresAuth: true }
  },
  {
    path: '/categories',
    name: 'Categories',
    component: Categories,
    meta: { requiresAuth: true }
  },
  {
    path: '/transactions',
    name: 'Transactions',
    component: Transactions,
    meta: { requiresAuth: true }
  },
  {
    path: '/health',
    name: 'Health',
    component: Health,
    meta: { requiresAuth: true }
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

// Navigation guard for authentication
router.beforeEach((to, from, next) => {
  const isAuthenticated = authService.isAuthenticated()
  
  if (to.meta.requiresAuth && !isAuthenticated) {
    next('/login')
  } else {
    next()
  }
})

export default router

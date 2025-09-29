import { createApp } from 'vue'
import App from './App.vue'
import { createRouter, createWebHistory } from 'vue-router'

// Tailwind CSS
import './assets/tailwind.css'
// Componets on vue
import LandingPage from './components/LandingPage.vue'
import Login from './components/LoginPage.vue'
import RegisterEmployee from './components/registerEmployee.vue'
import PasswordSetup from './components/PasswordSetup.vue'
import DashboardMainEmployer from './components/DashboardMainEmployer.vue'

const routes = [
  { path: '/', component: LandingPage },
  { path: '/login', component: Login },
  { path: '/register', component: RegisterEmployee },
  { path: '/password-setup', component: PasswordSetup },
  { path: '/dashboard-main-employer', component: DashboardMainEmployer }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

createApp(App).use(router).mount('#app')
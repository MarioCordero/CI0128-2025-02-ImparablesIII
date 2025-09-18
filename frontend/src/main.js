import { createApp } from 'vue'
import App from './App.vue'
import { createRouter, createWebHistory } from 'vue-router'

// Tailwind CSS
import './assets/tailwind.css'
// Componets on vue
import LandingPage from './components/LandingPage.vue'
import Login from './components/LoginPage.vue'

const routes = [
  { path: '/', component: LandingPage },
  { path: '/login', component: Login }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

createApp(App).use(router).mount('#app')
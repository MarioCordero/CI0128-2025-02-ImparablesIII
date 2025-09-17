import { createApp } from 'vue'
import App from './App.vue'
import { createRouter, createWebHistory } from 'vue-router'

// Tailwind CSS
import './assets/tailwind.css'
// Componets on vue
import LandingPage from './components/LandingPage.vue'

const routes = [
  { path: '/', component: LandingPage }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

createApp(App).use(router).mount('#app')
import { createApp } from 'vue'
import App from './App.vue'
import { createRouter, createWebHistory } from 'vue-router'

// Montserrat font styles
import './assets/montserratFont.css'

// Neumorphism global styles
import './assets/neumorphismGlobal.css'

// Tailwind CSS
import './assets/tailwind.css'

// Componets on vue
import LandingPage from './components/LandingPage.vue'
import Login from './components/LoginPage.vue'
import RegisterEmployee from './components/registerEmployee.vue'
import PasswordSetup from './components/PasswordSetup.vue'

const routes = [
  { path: '/', component: LandingPage },
  { path: '/login', component: Login },
  { path: '/register', component: RegisterEmployee },
  { path: '/password-setup', component: PasswordSetup }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

createApp(App).use(router).mount('#app')
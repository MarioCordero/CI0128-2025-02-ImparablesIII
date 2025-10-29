import { createApp } from 'vue'
import App from './App.vue'
import { createRouter, createWebHistory } from 'vue-router'

// Montserrat font styles
import './assets/montserratFont.css'

// Neumorphism global styles
import './assets/neumorphismGlobal.css'

// Tailwind CSS
import './assets/tailwind.css'

// Vue Components
import DashboardEmployee from './components/employee/DashboardEmployee.vue'
import LandingPage from './components/LandingPage.vue'
import Login from './components/LoginPage.vue'
import PasswordSetup from './components/PasswordSetup.vue'
import SuperAdminMenu from './components/SuperAdminMenu.vue'
import SignUpEmployer from './components/SignUpEmployer.vue'
import HoursRegistry from './components/HoursRegistry.vue'
import ProfileEmployee from './components/ProfileEmployee.vue'
// EMPLOYER COMPONENTS
import AddBenefit from './components/employer/projectDashboard/AddBenefit.vue'
import RegisterEmployee from './components/employer/projectDashboard/RegisterEmployee.vue'
import DashboardMainEmployer from './components/employer/DashboardMainEmployer.vue'
import DashboardProject from './components/employer/projectDashboard/DashboardProject.vue'
import CreateProject from './components/employer/CreateProject.vue'

// Route definitions
const routes = [
  { path: '/', component: LandingPage },
  { path: '/login', component: Login },
  {
    path: '/register-employee/:employerId/:projectId',
    name: 'RegisterEmployee',
    component: RegisterEmployee
  },
  { path: '/password-setup', component: PasswordSetup },
  { path: '/dashboard-main-employer', component: DashboardMainEmployer },
  { path: '/superadmin', component: SuperAdminMenu, meta: { requiresAuth: true, requiresRole: 'Administrador' } },
  { path: '/create-project', component: CreateProject },
  { path: '/signup-employer', component: SignUpEmployer },
  { path: '/hourregistry', component: HoursRegistry },
  { path: '/dashboard-project/:id', name: 'DashboardProject', component: DashboardProject },
  { path: '/profile-employee/:id?', name: 'ProfileEmployee', component: ProfileEmployee, props: true },
  { path: '/dashboard-employee', name: 'DashboardEmployee', component: DashboardEmployee },
  { path: '/add-benefit/:projectId?', name: 'AddBenefit', component: AddBenefit },
  { path: '/dashboard-employee', name: 'DashboardEmployee', component: DashboardEmployee },
  { path: '/add-benefit', name: 'AddBenefit', component: AddBenefit },
  { path: '/edit-employee/:id', name: 'EditEmployee', component: ProfileEmployee, props: true }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

// Navigation guard to protect routes
router.beforeEach((to, from, next) => {
  // Check if the route requires authentication
  if (to.meta.requiresAuth) {
    // Get user data from localStorage
    const userData = localStorage.getItem('user');
    const token = localStorage.getItem('token');
    
    // Check if user is authenticated
    if (!userData || !token) {
      // Redirect to login page
      next('/login');
      return;
    }
    
    try {
      const user = JSON.parse(userData);
      
      // Check if user has the required role
      if (to.meta.requiresRole && user.tipoUsuario !== to.meta.requiresRole) {
        // Redirect to home page if user doesn't have required role
        next('/');
        return;
      }
      
      // User is authenticated and has required role
      next();
    } catch (error) {
      // Invalid user data, redirect to login
      localStorage.removeItem('user');
      localStorage.removeItem('token');
      next('/login');
    }
  } else {
    // Route doesn't require authentication
    next();
  }
})

createApp(App).use(router).mount('#app');
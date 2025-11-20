import { createApp } from 'vue'
import App from './App.vue'
import { createRouter, createWebHistory } from 'vue-router'
// Montserrat font styles
import './assets/montserratFont.css'
// Neumorphism global styles
import './assets/neumorphism.css'
// Tailwind CSS
import './assets/tailwind.css'
// COMMON COMPONENTS
import LandingPage from './components/LandingPage.vue'
import Login from './components/LoginPage.vue'
import PasswordSetup from './components/PasswordSetup.vue'
import SuperAdminMenu from './components/superAdmin/SuperAdminMenu.vue'
import SignUpEmployer from './components/SignUpEmployer.vue'
import ProfileEmployee from './components/common/ProfileEmployee.vue' // component reused for editing employee
//EMPLOYEE COMPONENTS
import HoursRegistry from './components/employee/HoursRegistry.vue'
import DashboardEmployee from './components/employee/DashboardEmployee.vue'
// EMPLOYER COMPONENTS
import AddBenefit from './components/employer/projectDashboard/AddBenefit.vue'
import EditBenefit from './components/employer/projectDashboard/EditBenefit.vue'
import RegisterEmployee from './components/employer/projectDashboard/RegisterEmployee.vue'
import DashboardMainEmployer from './components/employer/DashboardMainEmployer.vue'
import DashboardProject from './components/employer/projectDashboard/DashboardProject.vue'
import CreateProject from './components/employer/CreateProject.vue'

const routes = [
  { path: '/', component: LandingPage },
  { path: '/login', component: Login },
  { path: '/register-employee/:employerId/:projectId', name: 'RegisterEmployee', component: RegisterEmployee },
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
  { path: '/edit-employee/:id', name: 'EditEmployee', component: ProfileEmployee, props: true },
  { path: '/edit-benefit/:companyId/:name', name: 'EditBenefit', component: EditBenefit, props: true }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach((to, from, next) => {
  if (to.meta.requiresAuth) {
    const userData = localStorage.getItem('user');
    const token = localStorage.getItem('token');
    if (!userData || !token) {
      next('/login');
      return;
    }
    try {
      const user = JSON.parse(userData);
      if (to.meta.requiresRole && user.tipoUsuario !== to.meta.requiresRole) {
        next('/');
        return;
      }
      next();
    } catch (error) {
      localStorage.removeItem('user');
      localStorage.removeItem('token');
      next('/login');
    }
  } else {
    next();
  }
})

createApp(App).use(router).mount('#app');
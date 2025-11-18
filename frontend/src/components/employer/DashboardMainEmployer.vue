<template>
  <div class="page">
    <MainEmployerHeader :companies="companies" />

    <div class="body">
      
      <!-- Greeting -->
      <section class="p-0">
        <!-- TODO: Agregar la inicial -->
        <h1 class="text-2xl font-bold mb-1">¡Hola, {{ user?.nombre || 'Usuario' }}!</h1>
        <p class="text-gray-600 text-[19px] font-medium">Administra los beneficios de tu organización</p>
      </section>
    
      <!-- Stats & Notifications -->
      <section class="grid md:grid-cols-2 gap-6 mb-6">
        <StatsCard v-if="!loading && !error" :userId="user?.idPersona"/>
        <NotificationsCard/>
      </section>
    
      <!-- Seccion Mis Empresas -->
      <section class="space-y-[33px]">
        <div class="space-y-[18px]">
          <div class="flex justify-between items-center mb-0">
            <h2 class="text-xl font-bold">Mis Empresas</h2>
            <button @click="navigateToCreateProject" class="neumorphism-button-normal-blue">
              Agregar Empresa
            </button>
          </div>
          <div class="w-full h-[10px] mt-2 rounded neumorphism-on-small-item"></div>
        </div>
        
        <!-- Loading State -->
        <div v-if="loading" class="flex justify-center items-center py-8">
          <div class="text-gray-600">Cargando empresas...</div>
        </div>
        
        <!-- Error State -->
        <div v-if="error" class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4 neumorphism-card">
          {{ error }}
        </div>
        
        <ProjectList v-if="!loading && !error" :userId="user?.idPersona" />
      </section>
    </div>
  </div>
</template>

<script>
import MainEmployerHeader from '../common/MainEmployerHeader.vue'
import ProjectList from './ProjectList.vue'
import apiConfig from '../../config/api.js'
import StatsCard from './StatsCard.vue'
import NotificationsCard from './projectDashboard/NotificationsCard.vue'

export default {
  name: 'DashboardMainEmployer',

  components: {
    MainEmployerHeader,
    ProjectList,
    StatsCard,
    NotificationsCard
  },
  data() {
    return {
      companies: [],
      loading: false,
      error: null,
      user: null
    }
  },

  methods: {
    navigateToCreateProject() {
      this.$router.push('/create-project')
    },
    async fetchCompanies() {
      this.loading = true
      this.error = null
      try {
        const response = await fetch(apiConfig.endpoints.project)
        if (!response.ok) throw new Error('No se pudo cargar las empresas')
        this.companies = await response.json()
      } catch (err) {
        this.error = err.message || 'Error al cargar las empresas'
      } finally {
        this.loading = false
      }
    }
  },
  created() {
    const userRaw = localStorage.getItem('user')
    if (userRaw) {
      try {
        this.user = JSON.parse(userRaw)
      } catch {
        this.user = null
      }
    }
  },

  mounted() {
    this.fetchCompanies()
    localStorage.setItem('selectedProject', 'null')
  }
}
</script>
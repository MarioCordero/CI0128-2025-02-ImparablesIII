<template>
  <header class="neumorfismo-tarjeta grid grid-cols-3 items-center gap-[120px] px-20 min-h-[95px] max-h-[95px]">
    <!-- Logo & Title -->
    <div class="flex items-center">
      <button
        @click="navigateToHomeLogged"
        class="focus:outline-none flex items-center"
        aria-label="Ir a inicio"
      >
        <img src="../../assets/PlaniFy.png" alt="PlaniFy Logo" class="h-10 w-full mr-4" />
      </button>
      <div>
        <p class="text-2xl font-bold mb-0 whitespace-nowrap">Panel de Empleador</p>
        <p class="text-gray-600 text-base mb-0 whitespace-nowrap">Gestión de Beneficios Corporativos</p>
      </div>
    </div>

    <!-- Project Selector -->
    <div class="flex flex-col justify-center items-center">
      <select
        class="neumorfismo-input min-w-[300px] min-h-[40px] px-3 py-2 text-gray-700 cursor-pointer!"
        v-model="selectedProjectId"
        @change="onProjectChange"
      >
        <option disabled value="">Seleccionar proyecto</option>
        <option v-for="project in companies" :key="project.id" :value="project.id">
          {{ project.nombre }}
        </option>
      </select>
    </div>

    <!-- Actions -->
    <div class="flex justify-end items-center gap-4">
      <button
        @click="logout"
        class="neumorfismo-boton-azul px-6 py-2 rounded-xl text-base font-semibold text-white!"
      >
        Cerrar Sesión
      </button>
    </div>
  </header>
</template>

<script>
import '../../assets/neumorphismGlobal.css'
import "../../assets/Neumorfismo.css";
import apiConfig from '../../config/api.js'

export default {
  name: 'MainEmployerHeader',
  data() {
    return {
      companies: [],
      selectedProjectId: '',
    }
  },
  async mounted() {
    await this.fetchCompanies()
    this.detectCurrentProject()
  },
  methods: {
    async fetchCompanies() {
      try {
        const response = await fetch(apiConfig.endpoints.project)
        if (response.ok) {
          this.companies = await response.json()
        }
      } catch (error) {
        this.companies = []
      }
    },
    navigateToHomeLogged() {
      localStorage.removeItem('selectedProject')
      this.selectedProjectId = ''
      this.$router.push('/dashboard-main-employer')
    },
    logout() {
      localStorage.removeItem('user')
      localStorage.removeItem('token')
      localStorage.removeItem('selectedProject')
      this.$router.push('/login')
    },
    onProjectChange() {
      if (this.selectedProjectId) {
        const selectedProject = this.companies.find(
          company => company.id == this.selectedProjectId
        )
        localStorage.setItem('selectedProject', JSON.stringify(selectedProject))
        this.$emit('project-changed', selectedProject) // <-- Add this line
        this.$router.push({
          name: 'DashboardProject',
          params: { id: this.selectedProjectId }
        })
      }
    },
    detectCurrentProject() {
      const selectedProject = JSON.parse(localStorage.getItem('selectedProject'))
      if (selectedProject) {
        this.selectedProjectId = selectedProject.id
      } else {
        this.selectedProjectId = ''
      }
    }
  }
}
</script>
<template>
  <header class="grid grid-cols-3 items-center gap-[120px] mb-0 rounded-lg bg-[#E9F7FF] px-20 min-h-[95px] max-h-[95px] shadow-[8px_8px_16px_#d1e3ee,-8px_-8px_16px_#ffffff] neumorphism-card">
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
        class="bg-[#E9F7FF] neumorphism-input shadow-[8px_8px_16px_#d1e3ee,-8px_-8px_16px_#ffffff] min-w-[300px] min-h-[40px] rounded px-3 py-2 text-gray-700"
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
        class="neumorphism-dark px-6 py-2 rounded-xl text-white text-base font-semibold hover:bg-blue-700 transition"
      >
        Cerrar Sesión
      </button>
    </div>
  </header>
</template>

<script>
import '../../assets/neumorphismGlobal.css'

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
        const response = await fetch('http://localhost:5011/api/Project')
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
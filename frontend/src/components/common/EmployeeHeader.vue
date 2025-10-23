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
        <p class="text-2xl font-bold mb-0 whitespace-nowrap">Panel de Empleado</p>
        <p class="text-gray-600 text-base mb-0 whitespace-nowrap">Gestión de Beneficios Corporativos</p>
      </div>
    </div>

    <div></div>

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
export default {
  name: 'EmployeeHeader',
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
    navigateToHomeLogged() {
      localStorage.removeItem('selectedProject')
      this.selectedProjectId = ''
      this.$router.push('/dashboard-employee')
    },
    logout() {
      localStorage.removeItem('user')
      localStorage.removeItem('token')
      this.$router.push('/login')
    },
  }
}
</script>
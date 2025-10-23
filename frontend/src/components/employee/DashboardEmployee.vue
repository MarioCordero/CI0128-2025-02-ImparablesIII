<template>
  <div class="min-h-screen bg-[#E9F7FF] p-0">
    <div class="mx-auto my-10 max-w-2xl">
      <section>
        <h1 class="text-2xl font-bold mb-1">¡Hola, {{ user?.nombre || 'Usuario' }}!</h1>
        <p class="text-gray-600 text-lg font-medium">Tu empresa</p>
      </section>

      <div v-if="loading" class="text-gray-600 py-8">Cargando empresa...</div>
      <div v-if="error" class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
        {{ error }}
      </div>

      <div v-if="company" class="neumorphism-card p-6 rounded-lg bg-white mt-6">
        <h2 class="text-xl font-bold mb-2">{{ company.Name }}</h2>
        <div class="text-gray-600 mb-2">{{ company.ActiveEmployees }} empleados</div>
        <div class="font-bold text-green-600">Rentabilidad: {{ company.CurrentProfitability }}%</div>
      </div>
      <div v-else-if="!loading && !error" class="text-gray-500 py-8">No se encontró empresa asociada.</div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'DashboardEmployee',
  data() {
    return {
      company: null,
      loading: false,
      error: null,
      user: null
    }
  },
  methods: {
    async fetchCompany() {
      this.loading = true
      this.error = null
      try {
        // Suponiendo que el endpoint devuelve la empresa del empleado
        const response = await fetch(`http://localhost:5011/api/Project/employee/${this.user.idPersona}`)
        if (!response.ok) throw new Error('No se pudo cargar la empresa')
        const companies = await response.json()
        this.company = Array.isArray(companies) ? companies[0] : companies
      } catch (err) {
        this.error = err.message || 'Error al cargar la empresa'
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
    if (this.user?.idPersona) {
      this.fetchCompany()
    }
  }
}
</script>
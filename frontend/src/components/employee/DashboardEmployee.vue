<template>
  <div class="min-h-screen bg-[#E9F7FF] p-0">
    <EmployeeHeader />
    <DashboardEmployeeSubHeader
      :selectedSection="selectedSection"
      @section-change="selectedSection = $event"
    />
    <div class="mx-auto my-10 max-w-2xl">      
      <div v-if="error" class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
        {{ error }}
      </div>

      <!-- Section placeholders -->
      <div v-if="selectedSection === 'dashboard'">
        <h1 class="text-2xl font-bold mb-1">¡Hola, {{ user?.nombre || 'Usuario' }}!</h1>
        <p class="text-gray-600 text-lg font-medium">Bienvenido a PlaniFy ¿Por qué no estás trabajando?</p>
        <p class="text-gray-600 mb-2">Aquí puedes ver un resumen de tu perfil.</p>
        <div v-if="user" class="bg-white rounded shadow p-4 mt-4">
          <p><span class="font-semibold">Nombre:</span> {{ user.nombre }} {{ user.segundoNombre }} {{ user.apellidos }}</p>
          <p><span class="font-semibold">Correo:</span> {{ user.correo }}</p>
          <p><span class="font-semibold">Departamento:</span> {{ user.departamento }}</p>
          <p><span class="font-semibold">Puesto:</span> {{ user.puesto }}</p>
          <p><span class="font-semibold">Tipo de Usuario:</span> {{ user.tipoUsuario }}</p>
        </div>
      </div>
      <div v-else-if="selectedSection === 'benefits'">
        <h2 class="text-xl font-bold mb-2">Beneficios</h2>
        <p class="text-gray-600 mb-2">Aquí puedes seleccionar y ver tus beneficios.</p>
      </div>
      <div v-else-if="selectedSection === 'hours'">
        <h2 class="text-xl font-bold mb-2">Registro de Horas</h2>
        <p class="text-gray-600 mb-2">Aquí puedes registrar tus horas trabajadas.</p>
      </div>

      <div v-if="loading" class="text-gray-600 py-8">Cargando empresa...</div>
    </div>
  </div>
</template>

<script>
import EmployeeHeader from '../common/EmployeeHeader.vue'
import DashboardEmployeeSubHeader from './DashboardEmployeeSubHeader.vue'

export default {
  name: 'DashboardEmployee',
  components: {
    EmployeeHeader,
    DashboardEmployeeSubHeader
  },
  data() {
    return {
      company: null,
      loading: false,
      error: null,
      user: null,
      selectedSection: 'dashboard', // <-- Add this line
      API_BASE_URL: 'http://localhost:5011/api'
    }
  },
  methods: {

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
    
  }
}
</script>
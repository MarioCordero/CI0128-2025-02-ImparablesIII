<template>
  <div class="page">
    <EmployeeHeader :user="user"/>
    <DashboardEmployeeSubHeader
      :selectedSection="selectedSection"
      @section-change="selectedSection = $event"
    />
    <div class="body mt-12">      
      <!-- Success Message -->
      <div v-if="successMessage" class="bg-green-100 border border-green-400 text-green-700 px-4 py-3 rounded-2xl mb-4 neumorphism-card">
        <div class="flex items-center">
          <svg class="w-5 h-5 mr-2" fill="currentColor" viewBox="0 0 20 20">
            <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"/>
          </svg>
          <span>{{ successMessage }}</span>
        </div>
      </div>

      <!-- Error Message -->
      <div v-if="error" class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded-2xl mb-4 neumorphism-card">
        <div class="flex items-center">
          <svg class="w-5 h-5 mr-2" fill="currentColor" viewBox="0 0 20 20">
            <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd"/>
          </svg>
          <span>{{ error }}</span>
        </div>
      </div>

      <!-- Dashboard Section -->
      <div v-if="selectedSection === 'dashboard'">
        <div class="space-y-[18px]">
          <h1 class="text-4xl font-bold text-gray-800">¡Hola, {{ user?.nombre || 'Usuario' }}!</h1>
            <p class="text-gray-600 text-lg font-medium">Bienvenido a PlaniFy ¿Por qué no estás trabajando?</p>
            <p class="text-gray-600">Aquí puedes ver un resumen de tu perfil.</p>
          <div class="w-full h-[10px] mt-2 rounded neumorphism-on-small-item"></div>
        </div>

        <div v-if="user" class="mt-[41px]">
          <div class="neumorphism-card">
            <p><span class="font-semibold">Nombre:</span> {{ user.nombre }} {{ user.segundoNombre }} {{ user.apellidos }}</p>
            <p><span class="font-semibold">Correo:</span> {{ user.correo }}</p>
            <p><span class="font-semibold">Departamento:</span> {{ user.departamento }}</p>
            <p><span class="font-semibold">Puesto:</span> {{ user.puesto }}</p>
            <p><span class="font-semibold">Tipo de Usuario:</span> {{ user.tipoUsuario }}</p>
          </div>
        </div>
      </div>

      <!-- Benefits Selection Section -->
      <div v-else-if="selectedSection === 'benefits'">
        <div class="space-y-[18px]">
          <h1 class="text-4xl font-bold text-gray-800">Selección de Beneficios</h1>
          <p class="text-gray-600">Elige tus beneficios según tus necesidades.</p>
          <div class="w-full h-[10px] mt-2 rounded neumorphism-on-small-item"></div>
        </div>

        <div class="mt-[41px]">
          <BenefitsSelectionView
            :employee-id="user?.idPersona"
            @success="handleBenefitUpdate"
            @error="handleError"
          />
        </div>
      </div>

      <!-- Hours Registry Section -->
      <div v-else-if="selectedSection === 'hours'">
        <div class="space-y-[18px]">
          <h1 class="text-4xl font-bold text-gray-800">Registro de Horas</h1>
          <p class="text-gray-600">Aquí puedes registrar tus horas trabajadas.</p>
          <div class="w-full h-[10px] mt-2 rounded neumorphism-on-small-item"></div>
        </div>

        <div class="mt-[41px]">
          <HoursRegistry
            :employee-id="user?.idPersona"
            @error="handleError"
          />
        </div>
      </div>

      <!-- Payroll Reports Section -->
      <div v-else-if="selectedSection === 'reports'">
        <div class="space-y-[18px]">
          <h1 class="text-4xl font-bold text-gray-800">Mis Reportes de Planilla</h1>
          <p class="text-gray-600">Visualiza y descarga tus recibos de pago.</p>
          <div class="w-full h-[10px] mt-2 rounded neumorphism-on-small-item"></div>
        </div>

        <div class="mt-[41px]">
          <PayrollReports
            :employee-id="user?.idPersona"
            user-type="employee"
            @error="handleError"
          />
        </div>
      </div>

      <div v-if="loading" class="text-gray-600 py-8">Cargando empresa...</div>
    </div>
  </div>
</template>

<script>
import EmployeeHeader from '../common/EmployeeHeader.vue'
import DashboardEmployeeSubHeader from './DashboardEmployeeSubHeader.vue'
import BenefitsSelectionView from './BenefitsSelectionView.vue'
import PayrollReports from '../common/PayrollReports.vue'
import HoursRegistry from './HoursRegistry.vue'

export default {
  name: 'DashboardEmployee',
  components: {
    EmployeeHeader,
    DashboardEmployeeSubHeader,
    BenefitsSelectionView,
    PayrollReports,
    HoursRegistry
  },
  data() {
    return {
      company: null,
      loading: false,
      error: null,
      successMessage: null,
      user: null,
      selectedSection: 'dashboard',
    }
  },
  methods: {
    handleBenefitUpdate(message) {
      this.successMessage = message
      this.error = null
      setTimeout(() => {
        this.successMessage = null
      }, 3000)
    },
    handleError(message) {
      this.error = message
      this.successMessage = null
      setTimeout(() => {
        this.error = null
      }, 5000)
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
    
  }
}
</script>
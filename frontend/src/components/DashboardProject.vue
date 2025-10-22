<template>
  <div class="min-h-screen bg-[#E9F7FF]">
    <MainEmployerHeader :companies="companies" />
    <DashboardProjectSubHeader
      :selected-section="selectedSection"
      @section-change="selectedSection = $event"
    />
    <div class="neumorphism-card w-full max-w-5xl p-10 my-20 rounded-[32px] shadow-lg">
      <div class="flex flex-col md:flex-row items-center justify-between mb-8">
        <!-- <button class="neumorphism-dark px-6 py-3 rounded-lg text-white hover:bg-blue-700 transition" @click="goBack">
          Volver
        </button> -->
      </div>
      <div v-if="loading" class="flex justify-center items-center py-12">
        <div class="animate-spin rounded-full h-12 w-12 neumorphism-dark"></div>
      </div>
      <div v-else-if="error" class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded-2xl mb-6 neumorphism-card">
        <span>{{ error }}</span>
      </div>

      <div v-if="selectedSection === 'dashboard'">
        <h1 class="text-4xl font-bold text-gray-800 mb-4 md:mb-0">Dashboard de Empresa</h1>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-8 mb-8">
          <div class="neumorphism-card p-6 rounded-2xl">
            <h2 class="text-xl font-semibold mb-2">Información de la Empresa</h2>
            <p class="text-gray-700"><span class="font-bold">Nombre:</span> {{ project.nombre }}</p>
            <p class="text-gray-700"><span class="font-bold">Cédula Jurídica:</span> {{ project.cedulaJuridica }}</p>
            <p class="text-gray-700"><span class="font-bold">Período de Pago:</span> {{ project.periodoPago }}</p>
            <p class="text-gray-700"><span class="font-bold">Email:</span> {{ project.email }}</p>
            <p class="text-gray-700"><span class="font-bold">Teléfono:</span> {{ project.telefono }}</p>
            <p class="text-gray-700"><span class="font-bold">Dirección:</span> {{ project.direccion || 'N/A' }}</p>
          </div>
          <div class="neumorphism-card p-6 rounded-2xl">
            <h2 class="text-xl font-semibold mb-2">Beneficios Corporativos</h2>
            <div v-if="!project.beneficios || project.beneficios.length === 0" class="text-gray-500">
              No hay beneficios registrados para esta empresa.
            </div>
            <ul v-else class="list-disc pl-5">
              <li v-for="benefit in project.beneficios" :key="benefit.id" class="mb-2">
                <span class="font-bold">{{ benefit.nombre }}</span> - {{ benefit.tipo }} ({{ benefit.tipoCalculo }})
              </li>
            </ul>
          </div>
        </div>
      </div>
      <div v-else-if="selectedSection === 'benefits'">
        <h1 class="text-4xl font-bold text-gray-800 mb-4 md:mb-0">Gestión de Beneficios</h1>
        <p>Aquí va la gestión de beneficios.</p>
        <button class="neumorphism-dark px-6 py-3 rounded-lg text-white hover:bg-blue-700 transition" @click="addBenefit">
          Agreagar beneficio
        </button>
      </div>
      <div v-else-if="selectedSection === 'employees'">
        <h1 class="text-4xl font-bold text-gray-800 mb-4 md:mb-0">Gestión de Empleados</h1>
        <button class="neumorphism-dark px-6 py-3 rounded-lg text-white hover:bg-blue-700 transition" @click="addEmployee">
          Agregar empleado
        </button>
        <p>Aquí va la gestión de empleados.</p>
      </div>
      <div v-else-if="selectedSection === 'info'">
        <h1 class="text-4xl font-bold text-gray-800 mb-4 md:mb-0">Información de la Empresa</h1>
        <p>Aquí va la información de la empresa.</p>
      </div>
      <div v-else-if="selectedSection === 'reports'">
        <h1 class="text-4xl font-bold text-gray-800 mb-4 md:mb-0">Reportes de Planilla</h1>
        <p>Aquí van los reportes de planilla.</p>
      </div>
    </div>
  </div>
</template>

<script>
import MainEmployerHeader from './common/MainEmployerHeader.vue'
import DashboardProjectSubHeader from './projectDashboard/DashboardProjectSubHeader.vue'

export default {
  // 1. Nombre del componente
  name: 'ProjectDashboard',

  // 2. Componentes hijos locales
  components: {
    MainEmployerHeader,
    DashboardProjectSubHeader
  },

  // 3. Directivas locales
  directives: {},

  // 4. Props recibidas del padre
  props: {},

  // 5. Estado reactivo del componente
  data() {
    return {
      project: {},
      loading: true,
      error: null,
      companies: [],
      selectedSection: 'dashboard' // default section
    }
  },

  // 6. Propiedades derivadas
  computed: {},

  // 7. Observadores de cambios
  watch: {},

  // 8. Métodos y lógica ejecutable
  methods: {
    goBack() {
      this.$router.push('/dashboard-main-employer')
    },

    addBenefit() {
      this.$router.push('/dashboard-main-employer/benefits/new') // TODO
    },

    addEmployee() {
      this.$router.push({
        name: 'RegisterEmployee',
        params: {
          employerId: this.project.id,
          projectId: this.project.id
        }
      })
    },

    async fetchCompanies() {
      try {
        const response = await fetch('http://localhost:5011/api/Project')
        if (!response.ok) throw new Error('No se pudo cargar las empresas')
        this.companies = await response.json()
      } catch (err) {
        // manejo
      }
    },

    async fetchProject() {
      try {
        this.loading = true
        this.error = null
        const id = this.$route.params.id
        const response = await fetch(`http://localhost:5011/api/Project/${id}`)
        if (!response.ok) throw new Error('No se pudo cargar el proyecto')
        const data = await response.json()
        this.project = data
      } catch (err) {
        this.error = err.message || 'Error al cargar el proyecto'
      } finally {
        this.loading = false
      }
    }
  },

  // 9. Ciclo de vida
  beforeCreate() {},
  created() {},
  beforeMount() {},
  mounted() {
    this.fetchCompanies()
    this.fetchProject()
  },
  beforeUpdate() {},
  updated() {},
  beforeUnmount() {},
  unmounted() {},

  // 10. Opciones de inyección
  provide() {
    return {}
  },
  inject: [],

  // 11. Eventos emitidos
  emits: [],

  // 12. Reutilización de lógica
  mixins: [],
  extends: null,

  // 13. Filtros
  filters: {}
}
</script>
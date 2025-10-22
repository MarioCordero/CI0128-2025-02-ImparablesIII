<template>
  <div class="min-h-screen bg-[#E9F7FF] p-0">
    <MainEmployerHeader :companies="companies" :current-project="selectedProject" />

    <div class="mx-[171px] my-[41px] space-y-[41px]">
      <!-- Success Message -->
      <div v-if="successMessage" class="bg-green-100 border border-green-400 text-green-700 px-4 py-3 rounded-2xl mb-6 neumorphism-card">
        <div class="flex items-center">
          <svg class="w-5 h-5 mr-2" fill="currentColor" viewBox="0 0 20 20">
            <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"/>
          </svg>
          <span>{{ successMessage }}</span>
        </div>
      </div>

      <!-- Error Message -->
      <div v-if="errorMessage" class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded-2xl mb-6 neumorphism-card">
        <div class="flex items-center">
          <svg class="w-5 h-5 mr-2" fill="currentColor" viewBox="0 0 20 20">
            <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd"/>
          </svg>
          <span>{{ errorMessage }}</span>
        </div>
      </div>

      <!-- Form -->
      <div class="neumorphism-card bg-[#E9F7FF] p-8 my-20! rounded-2xl">
        <h1 class="text-5xl font-black mb-8! mt-2 text-black tracking-wide text-center py-2 px-4">
          Agregar Beneficio
        </h1>
      

        <form @submit.prevent="handleSubmit" class="space-y-6">
          <!-- Información del Beneficio -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <!-- Nombre del Beneficio -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">
                Nombre del Beneficio *
              </label>
              <input
                v-model="form.Nombre"
                type="text"
                required
                maxlength="20"
                :class="['neumorphism-input w-full', errors.Nombre ? 'ring-2 ring-red-500' : '']"
                placeholder="Ej: Bono de Navidad"
              />
              <span v-if="errors.Nombre" class="text-red-500 text-sm mt-1">{{ errors.Nombre }}</span>
            </div>

            <!-- Tipo de Cálculo -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">
                Tipo de Cálculo *
              </label>
              <select
                v-model="form.TipoCalculo"
                required
                :class="['neumorphism-input w-full', errors.TipoCalculo ? 'ring-2 ring-red-500' : '']"
              >
                <option value="">Seleccione el tipo de cálculo</option>
                <option value="Porcentaje">Porcentaje</option>
                <option value="Monto Fijo">Monto Fijo</option>
                <option value="API">API</option>
              </select>
              <span v-if="errors.TipoCalculo" class="text-red-500 text-sm mt-1">{{ errors.TipoCalculo }}</span>
            </div>
          </div>

          <!-- Tipo de Beneficio -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">
                Tipo de Beneficio *
              </label>
              <select
                v-model="form.Tipo"
                required
                :class="['neumorphism-input w-full', errors.Tipo ? 'ring-2 ring-red-500' : '']"
              >
                <option value="">Seleccione el tipo de beneficio</option>
                <option value="Bonificación">Bonificación</option>
                <option value="Descuento">Descuento</option>
                <option value="Prestación">Prestación</option>
              </select>
              <span v-if="errors.Tipo" class="text-red-500 text-sm mt-1">{{ errors.Tipo }}</span>
            </div>

            <!-- Empresa -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">
                Empresa *
              </label>
              <select
                v-model="form.IdEmpresa"
                required
                :disabled="isProjectSelected"
                :class="[
                  'neumorphism-input w-full', 
                  errors.IdEmpresa ? 'ring-2 ring-red-500' : '',
                  isProjectSelected ? 'bg-gray-100 cursor-not-allowed' : ''
                ]"
                @change="clearErrors"
              >
                <option value="">Seleccione la empresa</option>
                <option v-for="company in companies" :key="company.id" :value="company.id">
                  {{ company.nombre }}
                </option>
              </select>
              <span v-if="errors.IdEmpresa" class="text-red-500 text-sm mt-1">{{ errors.IdEmpresa }}</span>
            </div>
          </div>

          <!-- Botones -->
          <div class="flex flex-col sm:flex-row gap-4 pt-6">
            <button
              type="submit"
              :disabled="isSubmitting"
              class="flex-1 neumorphism-dark px-6 py-3 rounded-lg text-white hover:bg-blue-700 transition disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <span v-if="isSubmitting" class="flex items-center justify-center">
                <svg class="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                  <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                  <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                Agregando...
              </span>
              <span v-else>Agregar Beneficio</span>
            </button>

            <button
              type="button"
              @click="goBack"
              class="flex-1 neumorphism-light px-6 py-3 rounded-lg text-gray-700 hover:bg-gray-100 transition"
            >
              Cancelar
            </button>
          </div>
        </form>
      </div>

      <!-- Lista de Beneficios Existentes -->
      <div v-if="beneficios.length > 0" class="neumorphism-card bg-[#E9F7FF] p-8 rounded-2xl">
        <h2 class="text-2xl font-bold mb-6 text-gray-800">Beneficios Existentes</h2>
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          <div
            v-for="beneficio in beneficios"
            :key="`${beneficio.companyId}-${beneficio.name}`"
            class="neumorphism-card p-4 rounded-lg hover:shadow-lg transition"
          >
            <h3 class="font-semibold text-lg text-gray-800">{{ beneficio.name }}</h3>
            <p class="text-sm text-gray-600 mt-1">
              <span class="font-medium">Tipo:</span> {{ beneficio.type }}
            </p>
            <p class="text-sm text-gray-600">
              <span class="font-medium">Cálculo:</span> {{ beneficio.calculationType }}
            </p>
            <p class="text-sm text-gray-600">
              <span class="font-medium">Empresa:</span> {{ beneficio.companyName }}
            </p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import MainEmployerHeader from './common/MainEmployerHeader.vue'

export default {
  name: 'AddBenefit',
  components: {
    MainEmployerHeader
  },
  props: {
    projectId: {
      type: String,
      default: null
    }
  },
  data() {
    return {
      form: {
        Nombre: '',
        TipoCalculo: '',
        Tipo: '',
        IdEmpresa: ''
      },
      errors: {},
      isSubmitting: false,
      successMessage: '',
      errorMessage: '',
      companies: [],
      beneficios: [],
      selectedProject: null
    }
  },
  computed: {
    isProjectSelected() {
      return this.selectedProject !== null
    },
    projectDisplayName() {
      return this.selectedProject ? this.selectedProject.nombre : 'No seleccionado'
    }
  },
  mounted() {
    this.fetchCompanies()
    this.fetchBeneficios()
    this.initializeProject()
  },
  methods: {
    clearErrors() {
      this.errors = {}
      this.errorMessage = ''
    },
    validateForm() {
      this.clearErrors()
      let isValid = true

      // Validate Nombre
      if (!this.form.Nombre.trim()) {
        this.errors.Nombre = 'El nombre del beneficio es obligatorio'
        isValid = false
      } else if (this.form.Nombre.trim().length > 20) {
        this.errors.Nombre = 'El nombre no puede exceder 20 caracteres'
        isValid = false
      } else if (!/^[a-zA-ZÀ-ÿ\u00f1\u00d1\s]+$/.test(this.form.Nombre.trim())) {
        this.errors.Nombre = 'El nombre solo puede contener letras y espacios'
        isValid = false
      }

      // Validate TipoCalculo
      if (!this.form.TipoCalculo) {
        this.errors.TipoCalculo = 'El tipo de cálculo es obligatorio'
        isValid = false
      }

      // Validate Tipo
      if (!this.form.Tipo) {
        this.errors.Tipo = 'El tipo de beneficio es obligatorio'
        isValid = false
      }

      // Validate IdEmpresa
      if (!this.form.IdEmpresa) {
        this.errors.IdEmpresa = 'Debe seleccionar una empresa'
        isValid = false
      }

      return isValid
    },
    async handleSubmit() {
      if (!this.validateForm()) {
        return
      }

      this.isSubmitting = true
      this.clearErrors()

      try {
        const response = await fetch('http://localhost:5011/api/Benefit', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({
            CompanyId: parseInt(this.form.IdEmpresa),
            Name: this.form.Nombre.trim(),
            CalculationType: this.form.TipoCalculo,
            Type: this.form.Tipo
          })
        })

        const data = await response.json()

        if (response.ok) {
          this.successMessage = 'Beneficio agregado exitosamente'
          this.form = {
            Nombre: '',
            TipoCalculo: '',
            Tipo: '',
            IdEmpresa: this.isProjectSelected ? this.selectedProject.id : ''
          }
          // Refresh the benefits list
          if (this.isProjectSelected) {
            await this.fetchBeneficiosByProject(this.selectedProject.id)
          } else {
            await this.fetchBeneficios()
          }
          
          // Auto-redirect after 2 seconds if coming from a project
          if (this.isProjectSelected) {
            setTimeout(() => {
              this.$router.push(`/dashboard-project/${this.selectedProject.id}`)
            }, 2000)
          }
        } else {
          this.errorMessage = data.message || 'Error al agregar el beneficio'
        }
      } catch (error) {
        console.error('Error:', error)
        this.errorMessage = 'Error de conexión. Por favor, intente nuevamente.'
      } finally {
        this.isSubmitting = false
      }
    },
    async fetchCompanies() {
      try {
        const response = await fetch('http://localhost:5011/api/Project')
        if (!response.ok) throw new Error('No se pudo cargar las empresas')
        this.companies = await response.json()
      } catch (err) {
        this.errorMessage = 'Error al cargar las empresas'
      }
    },
    async fetchBeneficios() {
      try {
        const response = await fetch('http://localhost:5011/api/Benefit')
        if (!response.ok) throw new Error('No se pudo cargar los beneficios')
        this.beneficios = await response.json()
      } catch (err) {
        this.errorMessage = 'Error al cargar los beneficios'
      }
    },
    goBack() {
      if (this.isProjectSelected) {
        // Return to the specific project dashboard
        this.$router.push(`/dashboard-project/${this.selectedProject.id}`)
      } else {
        // Return to main employer dashboard
        this.$router.push('/dashboard-main-employer')
      }
    },
    async initializeProject() {
      // Get projectId from route params or props
      const projectId = this.$route.params.projectId || this.projectId
      
      if (projectId) {
        try {
          // Fetch the specific project details
          const response = await fetch(`http://localhost:5011/api/Project/${projectId}`)
          if (response.ok) {
            this.selectedProject = await response.json()
            this.form.IdEmpresa = this.selectedProject.id
            // Fetch benefits for this specific project
            await this.fetchBeneficiosByProject(projectId)
          }
        } catch (err) {
          this.errorMessage = 'Error al cargar el proyecto'
        }
      }
    },
    async fetchBeneficiosByProject(projectId) {
      try {
        const response = await fetch(`http://localhost:5011/api/Benefit/company/${projectId}`)
        if (response.ok) {
          this.beneficios = await response.json()
        }
      } catch (err) {
        this.errorMessage = 'Error al cargar los beneficios del proyecto'
      }
    }
  }
}
</script>


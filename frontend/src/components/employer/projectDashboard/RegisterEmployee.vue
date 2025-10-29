<template>
  <div class="min-h-screen bg-[#E9F7FF]">
    <MainEmployerHeader :companies="companies" :current-project="project" />
    <DashboardProjectSubHeader />
    <div class="bg-[#E9F7FF] rounded-[40px] shadow-[8px_8px_16px_#d1e3ee,-8px_-8px_16px_#ffffff] p-12 w-full max-w-4xl">
      <!-- Step Navigation -->
      <div class="flex justify-center items-center mb-8">
        <div class="flex items-center space-x-8">
          <!-- Steps (Personal, Address, Employment) -->

          <div v-for="(step, idx) in stepTitles" :key="idx" class="flex flex-col items-center">
            <div
              :class="[
                'w-12 h-12 rounded-full flex items-center justify-center shadow-[4px_4px_8px_#d1e3ee,-4px_-4px_8px_#ffffff] transition-all cursor-pointer',
                currentTab === idx ? 'bg-[#87ceeb]' : 'bg-gray-300'
              ]"
              @click="goToTab(idx)"
            >
              <svg v-if="idx === 0" class="w-6 h-6 text-white" fill="currentColor" viewBox="0 0 24 24">
                <path d="M12 12c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm0 2c-2.67 0-8 1.34-8 4v2h16v-2c0-2.66-5.33-4-8-4z"/>
                <circle cx="12" cy="8" r="2"/>
              </svg>
              <svg v-else-if="idx === 1" class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z"/>
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z"/>
              </svg>
              <svg v-else class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path d="M8 7H4C2.89543 7 2 7.89543 2 9V19C2 20.1046 2.89543 21 4 21H20C21.1046 21 22 20.1046 22 19V9C22 7.89543 21.1046 7 20 7H16M8 7V3.6C8 3.26863 8.26863 3 8.6 3H15.4C15.7314 3 16 3.26863 16 3.6V7M8 7H16" stroke="white" stroke-width="2"/>
              </svg>
            </div>
            <div
              :class="[
                'w-8 h-1 rounded-full mt-2 shadow-[2px_2px_4px_#d1e3ee,-2px_-2px_4px_#ffffff]',
                currentTab === idx ? 'bg-[#87ceeb]' : 'bg-gray-300'
              ]"
            ></div>
          </div>
        </div>
      </div>

      <h2 class="text-2xl font-semibold text-gray-700 text-center mb-8 shadow-[2px_2px_4px_#d1e3ee,-2px_-2px_4px_#ffffff] rounded-[12px] bg-[#E9F7FF] py-2 px-4">{{ stepTitles[currentTab] }}</h2>
      
      <!-- Project Selection Info -->
      <div v-if="isProjectSelected" class="bg-blue-50 border border-blue-200 text-blue-800 px-4 py-3 rounded-2xl mb-6 shadow-[2px_2px_4px_#d1e3ee,-2px_-2px_4px_#ffffff]">
        <div class="flex items-center">
          <svg class="w-5 h-5 mr-2" fill="currentColor" viewBox="0 0 20 20">
            <path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z" clip-rule="evenodd"/>
          </svg>
          <span><strong>Proyecto seleccionado:</strong> {{ projectDisplayName }}</span>
        </div>
      </div>

      <form @submit.prevent="handleSubmit" class="space-y-6">
        <!-- Tab 1: Personal Information -->
        <div v-if="currentTab === 0" class="space-y-6">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <!-- Left Column -->
            <div class="space-y-6">
              <div class="relative">
                <div class="space-x-2 ml-2"><p>Primer Nombre</p></div>
                <input v-model="formData.primerNombre" type="text" placeholder="Primer Nombre" class="input-style" required />
                <p v-if="errors.primerNombre" class="text-red-500 text-sm mt-1 ml-2">{{ errors.primerNombre }}</p>
              </div>
              <div class="relative">
                <div class="space-x-2 ml-2"><p>Apellidos</p></div>
                <input v-model="formData.primerApellido" type="text" placeholder="Apellidos" class="input-style" required />
                <p v-if="errors.primerApellido" class="text-red-500 text-sm mt-1 ml-2">{{ errors.primerApellido }}</p>
              </div>
              <div class="relative">
                <div class="space-x-2 ml-2"><p>Teléfono</p></div>
                <input v-model="formData.telefono" type="tel" placeholder="#### ####" @input="formatTelefono" class="input-style" required />
                <p v-if="errors.telefono" class="text-red-500 text-sm mt-1 ml-2">{{ errors.telefono }}</p>
              </div>
              <div class="relative">
                <div class="space-x-2 ml-2"><p>Correo Electrónico</p></div>
                <input v-model="formData.correo" type="email" placeholder="Correo Electrónico" class="input-style" required />
                <p v-if="errors.correo" class="text-red-500 text-sm mt-1 ml-2">{{ errors.correo }}</p>
              </div>
            </div>
            <!-- Right Column -->
            <div class="space-y-6">
              <div class="relative">
                <div class="space-x-2 ml-2"><p>Segundo Nombre</p></div>
                <input v-model="formData.segundoNombre" type="text" placeholder="Segundo Nombre" class="input-style" />
              </div>
              <div class="relative">
                <div class="space-x-2 ml-2"><p>Fecha de Nacimiento</p></div>
                <input v-model="formData.fechaNacimiento" type="date" placeholder="Fecha de nacimiento" class="input-style" required />
                <p v-if="errors.fechaNacimiento" class="text-red-500 text-sm mt-1 ml-2">{{ errors.fechaNacimiento }}</p>
              </div>
              <div class="relative">
                <div class="space-x-2 ml-2"><p>Cédula</p></div>
                <input v-model="formData.cedula" type="text" placeholder="Cédula (#-####-####)" @input="formatCedula" class="input-style" required />
                <p v-if="errors.cedula" class="text-red-500 text-sm mt-1 ml-2">{{ errors.cedula }}</p>
              </div>
            </div>
          </div>
        </div>
        <!-- Tab 2: Address Information -->
        <div v-if="currentTab === 1" class="space-y-6">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div class="space-y-6">
              <div class="relative">
                <div class="space-x-2 ml-2"><p>Provincia</p></div>
                <select v-model="formData.provincia" class="input-style appearance-none cursor-pointer" required>
                  <option value="" disabled selected>Provincia</option>
                  <option v-for="provincia in provinciasCostaRica" :key="provincia" :value="provincia">{{ provincia }}</option>
                </select>
                <div class="absolute inset-y-0 right-0 flex items-center pr-6 pointer-events-none">
                  <svg class="w-5 h-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"/>
                  </svg>
                </div>
                <p v-if="errors.provincia" class="text-red-500 text-sm mt-1 ml-2">{{ errors.provincia }}</p>
              </div>
              <div class="relative">
                <div class="space-x-2 ml-2"><p>Cantón</p></div>
                <input v-model="formData.canton" type="text" placeholder="Cantón" class="input-style" required />
                <p v-if="errors.canton" class="text-red-500 text-sm mt-1 ml-2">{{ errors.canton }}</p>
              </div>
            </div>
            <div class="space-y-6">
              <div class="relative">
                <div class="space-x-2 ml-2"><p>Distrito</p></div>
                <input v-model="formData.distrito" type="text" placeholder="Distrito" class="input-style" required />
                <p v-if="errors.distrito" class="text-red-500 text-sm mt-1 ml-2">{{ errors.distrito }}</p>
              </div>
              <div class="relative">
                <div class="space-x-2 ml-2"><p>Dirección Particular</p></div>
                <input v-model="formData.direccionParticular" type="text" placeholder="Dirección Particular" class="input-style" required />
                <p v-if="errors.direccionParticular" class="text-red-500 text-sm mt-1 ml-2">{{ errors.direccionParticular }}</p>
              </div>
            </div>
          </div>
        </div>
        <!-- Tab 3: Employment Information -->
        <div v-if="currentTab === 2" class="space-y-6">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div class="space-y-6">
              <div class="relative">
                <div class="space-x-2 ml-2"><p>Departamento</p></div>
                <input v-model="formData.departamento" type="text" placeholder="Departamento" class="input-style" required />
                <p v-if="errors.departamento" class="text-red-500 text-sm mt-1 ml-2">{{ errors.departamento }}</p>
              </div>
              <div class="relative">
                <div class="space-x-2 ml-2"><p>Tipo de Contrato</p></div>
                <select v-model="formData.tipoContrato" class="input-style appearance-none cursor-pointer" required>
                  <option value="" disabled selected>Tipo de Contrato</option>
                  <option value="Tiempo Completo">Tiempo Completo</option>
                  <option value="Medio Tiempo">Medio Tiempo</option>
                  <option value="Servicios Profesionales">Servicios Profesionales</option>
                </select>
                <div class="absolute inset-y-0 right-0 flex items-center pr-6 pointer-events-none">
                  <svg class="w-5 h-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"/>
                  </svg>
                </div>
                <p v-if="errors.tipoContrato" class="text-red-500 text-sm mt-1 ml-2">{{ errors.tipoContrato }}</p>
              </div>
              <div class="relative">
                <div class="space-x-2 ml-2"><p>Puesto</p></div>
                <input v-model="formData.puesto" type="text" placeholder="Puesto" class="input-style" required />
                <p v-if="errors.puesto" class="text-red-500 text-sm mt-1 ml-2">{{ errors.puesto }}</p>
              </div>
            </div>
            <div class="space-y-6">
              <div class="relative">
                <div class="space-x-2 ml-2"><p>Salario</p></div>
                <input v-model="formattedSalario" type="text" placeholder="₡0.000" @input="formatSalario" class="input-style" required />
                <p v-if="errors.salario" class="text-red-500 text-sm mt-1 ml-2">{{ errors.salario }}</p>
              </div>
              <div class="relative">
                <div class="space-x-2 ml-2"><p>Número de Cuenta IBAN</p></div>
                <input v-model="formData.numeroCuentaIban" type="text" placeholder="Número de Cuenta IBAN" class="input-style" required />
                <p v-if="errors.numeroCuentaIban" class="text-red-500 text-sm mt-1 ml-2">{{ errors.numeroCuentaIban }}</p>
              </div>
            </div>
          </div>
        </div>
        <!-- Navigation Buttons -->
        <div class="flex justify-between mt-8">
          <button v-if="currentTab > 0" type="button" @click="goToTab(currentTab - 1)" class="custom-button px-4 py-2 flex items-center space-x-2 font-medium text-gray-700">
            <svg class="w-5 h-5" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" d="M15 19l-7-7 7-7"/>
            </svg>
            <span>Anterior</span>
          </button>
          <div class="ml-auto">
            <button v-if="currentTab < 2" type="button" @click="goToTab(currentTab + 1)" class="custom-button px-4 py-2 flex items-center space-x-2 font-medium text-gray-700">
              <span>Siguiente</span>
              <svg class="w-5 h-5" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" d="M9 5l7 7-7 7"/>
              </svg>
            </button>
            <button v-else type="submit" class="custom-button px-4 py-2 flex items-center space-x-2 font-medium text-gray-700">
              <span>Registrar</span>
              <svg class="w-5 h-5" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" d="M5 13l4 4L19 7"/>
              </svg>
            </button>
          </div>
        </div>
      </form>
    </div>
  </div>
</template>

<script>
import MainEmployerHeader from '../../common/MainEmployerHeader.vue'
import DashboardProjectSubHeader from './DashboardProjectSubHeader.vue'

export default {
  name: 'RegisterEmployee',
  components: {
    MainEmployerHeader,
    DashboardProjectSubHeader
  },
  data() {
    return {
      currentTab: 0,
      formattedSalario: '',
      errors: {},
      project: {},
      companies: [],
      provinciasCostaRica: [
        'San José',
        'Alajuela',
        'Cartago',
        'Heredia',
        'Guanacaste',
        'Puntarenas',
        'Limón'
      ],
      stepTitles: [
        'Información Personal',
        'Información de Dirección',
        'Información Laboral'
      ],
      formData: {
        primerNombre: '',
        segundoNombre: '',
        primerApellido: '',
        fechaNacimiento: '',
        cedula: '',
        telefono: '',
        correo: '',
        provincia: '',
        canton: '',
        distrito: '',
        direccionParticular: '',
        departamento: '',
        tipoContrato: '',
        puesto: '',
        salario: '',
        numeroCuentaIban: ''
      }
    }
  },
  mounted() {
    this.initializeProject()
    this.fetchCompanies()
  },
  computed: {
    employerId() {
      return this.$route.params.employerId
    },
    projectId() {
      return this.$route.params.projectId
    },
    isProjectSelected() {
      return this.project && this.project.id
    },
    projectDisplayName() {
      return this.project ? this.project.nombre : 'No seleccionado'
    }
  },

  methods: {
    async initializeProject() {
      if (this.projectId) {
        try {
          const response = await fetch(`http://localhost:5011/api/Project/${this.projectId}`)
          if (!response.ok) throw new Error('No se pudo cargar el proyecto')
          this.project = await response.json()
        } catch (err) {
          console.error('Error fetching project:', err)
        }
      }
    },
    async fetchCompanies() {
      try {
        const response = await fetch('http://localhost:5011/api/Project')
        if (!response.ok) throw new Error('No se pudo cargar las empresas')
        this.companies = await response.json()
      } catch (err) {
        console.error('Error fetching companies:', err)
      }
    },
    goToTab(tabIndex) {
      if (tabIndex > this.currentTab && !this.validateCurrentTab()) return
      if (tabIndex >= 0 && tabIndex <= 2) {
        this.currentTab = tabIndex
        this.clearErrors()
      }
    },
    formatCedula(event) {
      let value = event.target.value.replace(/\D/g, '')
      if (value.length > 1) {
        if (value.length <= 5) {
          value = value.slice(0, 1) + '-' + value.slice(1)
        } else {
          value = value.slice(0, 1) + '-' + value.slice(1, 5) + '-' + value.slice(5, 9)
        }
      }
      this.formData.cedula = value
    },
    formatTelefono(event) {
      let value = event.target.value.replace(/\D/g, '')
      if (value.length > 4) {
        value = value.slice(0, 4) + ' ' + value.slice(4, 8)
      }
      this.formData.telefono = value
    },
    formatSalario(event) {
      let value = event.target.value.replace(/[^\d]/g, '')
      if (value === '') {
        this.formattedSalario = ''
        this.formData.salario = ''
        return
      }
      const number = parseInt(value)
      this.formData.salario = number
      this.formattedSalario = '₡' + number.toLocaleString('es-CR')
    },

    validateCurrentTab() {
      this.clearErrors()
      let isValid = true
      switch (this.currentTab) {
        case 0: isValid = this.validatePersonalInfo(); break
        case 1: isValid = this.validateAddressInfo(); break
        case 2: isValid = this.validateEmploymentInfo(); break
      }
      return isValid
    },
    validatePersonalInfo() {
      let isValid = true
      if (!this.formData.primerNombre || this.formData.primerNombre.trim().length === 0) {
        this.errors.primerNombre = 'El primer nombre es requerido'
        isValid = false
      } else if (this.formData.primerNombre.length > 20) {
        this.errors.primerNombre = 'El primer nombre no puede exceder 20 caracteres'
        isValid = false
      }
      if (!this.formData.primerApellido || this.formData.primerApellido.trim().length === 0) {
        this.errors.primerApellido = 'El primer apellido es requerido'
        isValid = false
      } else if (this.formData.primerApellido.length > 20) {
        this.errors.primerApellido = 'El primer apellido no puede exceder 20 caracteres'
        isValid = false
      }
      if (!this.formData.fechaNacimiento) {
        this.errors.fechaNacimiento = 'La fecha de nacimiento es requerida'
        isValid = false
      } else {
        const today = new Date()
        const birthDate = new Date(this.formData.fechaNacimiento)
        const age = today.getFullYear() - birthDate.getFullYear()
        const monthDiff = today.getMonth() - birthDate.getMonth()
        const dayDiff = today.getDate() - birthDate.getDate()
        if (age < 18 || (age === 18 && monthDiff < 0) || (age === 18 && monthDiff === 0 && dayDiff < 0)) {
          this.errors.fechaNacimiento = 'El empleado debe tener al menos 18 años'
          isValid = false
        } else if (age > 99 || (age === 99 && monthDiff < 0) || (age === 99 && monthDiff === 0 && dayDiff < 0)) {
          this.errors.fechaNacimiento = 'El empleado debe tener menos de 99 años'
          isValid = false
        }
      }
      if (!this.formData.cedula || this.formData.cedula.trim().length === 0) {
        this.errors.cedula = 'La cédula es requerida'
        isValid = false
      } else {
        const cedulaDigits = this.formData.cedula.replace(/-/g, '')
        if (cedulaDigits.length !== 9) {
          this.errors.cedula = 'La cédula debe tener 9 dígitos'
          isValid = false
        }
      }
      if (!this.formData.telefono || this.formData.telefono.trim().length === 0) {
        this.errors.telefono = 'El teléfono es requerido'
        isValid = false
      } else {
        const phoneDigits = this.formData.telefono.replace(/\s/g, '')
        if (phoneDigits.length !== 8) {
          this.errors.telefono = 'El teléfono debe tener 8 dígitos (#### ####)'
          isValid = false
        }
      }
      if (!this.formData.correo || this.formData.correo.trim().length === 0) {
        this.errors.correo = 'El correo electrónico es requerido'
        isValid = false
      } else if (this.formData.correo.length > 50) {
        this.errors.correo = 'El correo no puede exceder 50 caracteres'
        isValid = false
      } else {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
        if (!emailRegex.test(this.formData.correo)) {
          this.errors.correo = 'Formato de correo electrónico inválido'
          isValid = false
        }
      }
      return isValid
    },

    validateAddressInfo() {
      let isValid = true
      if (!this.formData.provincia || this.formData.provincia.trim().length === 0) {
        this.errors.provincia = 'La provincia es requerida'
        isValid = false
      } else if (this.formData.provincia.length > 12) {
        this.errors.provincia = 'La provincia no puede exceder 12 caracteres'
        isValid = false
      }
      if (!this.formData.canton || this.formData.canton.trim().length === 0) {
        this.errors.canton = 'El cantón es requerido'
        isValid = false
      } else if (this.formData.canton.length > 30) {
        this.errors.canton = 'El cantón no puede exceder 30 caracteres'
        isValid = false
      }
      if (!this.formData.distrito || this.formData.distrito.trim().length === 0) {
        this.errors.distrito = 'El distrito es requerido'
        isValid = false
      } else if (this.formData.distrito.length > 30) {
        this.errors.distrito = 'El distrito no puede exceder 30 caracteres'
        isValid = false
      }
      if (!this.formData.direccionParticular || this.formData.direccionParticular.trim().length === 0) {
        this.errors.direccionParticular = 'La dirección particular es requerida'
        isValid = false
      } else if (this.formData.direccionParticular.length > 150) {
        this.errors.direccionParticular = 'La dirección no puede exceder 150 caracteres'
        isValid = false
      }
      return isValid
    },
    validateEmploymentInfo() {
      let isValid = true
      if (!this.formData.departamento || this.formData.departamento.trim().length === 0) {
        this.errors.departamento = 'El departamento es requerido'
        isValid = false
      } else if (this.formData.departamento.length > 20) {
        this.errors.departamento = 'El departamento no puede exceder 20 caracteres'
        isValid = false
      }
      if (!this.formData.tipoContrato || this.formData.tipoContrato.trim().length === 0) {
        this.errors.tipoContrato = 'El tipo de contrato es requerido'
        isValid = false
      } else if (this.formData.tipoContrato.length > 20) {
        this.errors.tipoContrato = 'El tipo de contrato no puede exceder 20 caracteres'
        isValid = false
      }
      if (!this.formData.puesto || this.formData.puesto.trim().length === 0) {
        this.errors.puesto = 'El puesto es requerido'
        isValid = false
      } else if (this.formData.puesto.length > 20) {
        this.errors.puesto = 'El puesto no puede exceder 20 caracteres'
        isValid = false
      }
      if (!this.formData.salario || this.formData.salario === '') {
        this.errors.salario = 'El salario es requerido'
        isValid = false
      } else if (this.formData.salario < 0) {
        this.errors.salario = 'El salario debe ser un valor positivo'
        isValid = false
      }
      if (!this.formData.numeroCuentaIban || this.formData.numeroCuentaIban.trim().length === 0) {
        this.errors.numeroCuentaIban = 'El número de cuenta IBAN es requerido'
        isValid = false
      } else if (this.formData.numeroCuentaIban.length > 30) {
        this.errors.numeroCuentaIban = 'El número de cuenta no puede exceder 30 caracteres'
        isValid = false
      }
      return isValid
    },
    clearErrors() {
      Object.keys(this.errors).forEach(key => { this.errors[key] = '' })
    },
    validateForm() {
      return this.validatePersonalInfo() && this.validateAddressInfo() && this.validateEmploymentInfo()
    },
    resetForm() {
      Object.assign(this.formData, {
        primerNombre: '',
        segundoNombre: '',
        primerApellido: '',
        fechaNacimiento: '',
        cedula: '',
        telefono: '',
        correo: '',
        provincia: '',
        canton: '',
        distrito: '',
        direccionParticular: '',
        departamento: '',
        tipoContrato: '',
        puesto: '',
        salario: '',
        numeroCuentaIban: ''
      })
      this.formattedSalario = ''
      this.currentTab = 0
      this.clearErrors()
    },
    async handleSubmit() {
      if (!this.validateForm()) return
      const employeeData = {
        ...this.formData,
        cedula: this.formData.cedula.replace(/-/g, ''),
        telefono: this.formData.telefono.replace(/\s/g, ''),
        fechaNacimiento: new Date(this.formData.fechaNacimiento).toISOString(),
        numeroCuentaIban: this.formData.numeroCuentaIban,
        employerId: this.employerId,
        projectId: this.project.id
      }
      try {
        const response = await fetch('http://localhost:5011/api/RegisterEmployee', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(employeeData)
        })
        const result = await response.json()
        if (response.ok) {
          alert('Empleado registrado exitosamente!')
          this.resetForm()
          // Return to the project dashboard if coming from a project
          if (this.isProjectSelected) {
            this.$router.push(`/dashboard-project/${this.project.id}`)
          } else {
            this.$router.push('/employer-menu')
          }
        } else {
          alert(`Error: ${result.message || 'Error al registrar el empleado'}`)
        }
      } catch (error) {
        console.error('Error:', error)
        alert('Error de conexión. Intente nuevamente.')
      }
    }
  }
}
</script>

<style scoped>
.input-style {
  @apply w-full bg-[#E9F7FF] rounded-[12px] px-6 py-4 text-gray-700 placeholder-gray-400 border-0 shadow-[inset_2px_2px_4px_#d1e3ee,inset_-2px_-2px_4px_#ffffff] focus:outline-none focus:ring-2 focus:ring-[#87ceeb] focus:shadow-lg transition-all;
}
.custom-button {
  @apply bg-[#E9F7FF] rounded-2xl px-6 py-3 shadow-lg hover:shadow-xl transition-all;
}
</style>
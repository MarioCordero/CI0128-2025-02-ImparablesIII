<template>
  <div class="page">
    <MainEmployerHeader :companies="companies" />
    <DashboardProjectSubHeader
      :selected-section="selectedSection"
      @section-change="selectedSection = $event"
    />

    <div class="body mt-12">
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
      <div class="neumorphism-card-modal w-[50%] mx-auto">
        <h1 class="text-3xl font-black text-black text-center">
          Registro de proyecto
        </h1>
        <div class="w-full h-[5px] mt-2 mb-6 rounded neumorphism-on-small-item"></div>
        <form @submit.prevent="handleSubmit" class="space-y-6">
          <!-- Información Básica -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <!-- Nombre de la Empresa -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">
                Nombre de la Empresa *
              </label>
              <input
                v-model="form.Nombre"
                type="text"
                required
                maxlength="20"
                :class="['neumorphism-input', errors.Nombre ? 'ring-2 ring-red-500' : '']"
                placeholder="Ingrese el nombre de la empresa"
              />
              <span v-if="errors.Nombre" class="text-red-500 text-sm mt-1">{{ errors.Nombre }}</span>
            </div>

            <!-- Cédula Jurídica -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">
                Cédula Jurídica *
              </label>
              <input
                v-model="form.CedulaJuridica"
                type="number"
                required
                min="100000000"
                max="999999999"
                :class="['neumorphism-input no-spinner', errors.CedulaJuridica ? 'ring-2 ring-red-500' : '']"
                placeholder="Ingrese la cédula jurídica (9 dígitos)"
              />
              <span v-if="errors.CedulaJuridica" class="text-red-500 text-sm mt-1">{{ errors.CedulaJuridica }}</span>
            </div>

            <!-- Email -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">
                Correo Electrónico *
              </label>
              <input
                v-model="form.Email"
                type="email"
                required
                maxlength="50"
                :class="['neumorphism-input', errors.Email ? 'ring-2 ring-red-500' : '']"
                placeholder="empresa@ejemplo.com"
              />
              <span v-if="errors.Email" class="text-red-500 text-sm mt-1">{{ errors.Email }}</span>
            </div>

            <!-- Teléfono -->
            <div>
              <label class="block mb-1 font-medium text-gray-700">Número de teléfono*</label>
              <input
                v-model="formattedTelefono"
                type="tel"
                required
                @input="formatTelefono"
                :class="['neumorphism-input', errors.Telefono ? 'ring-2 ring-red-500' : '']"
                placeholder="####-####"
                maxlength="9"
              />
              <span v-if="errors.Telefono" class="text-red-500 text-sm mt-1">{{ errors.Telefono }}</span>
            </div>

            <!-- Período de Pago -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">
                Período de Pago *
              </label>
              <select
                v-model="form.PeriodoPago"
                required
                :class="['neumorphism-input neumorphism-input-select', errors.PeriodoPago ? 'ring-2 ring-red-500' : '']"
              >
                <option value="">Seleccione período de pago</option>
                <option value="Mensual">Mensual</option>
                <option value="Quincenal">Quincenal</option>
              </select>
              <span v-if="errors.PeriodoPago" class="text-red-500 text-sm mt-1">{{ errors.PeriodoPago }}</span>
            </div>

            <!-- Máximo de Beneficios -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">
                Máximo de Beneficios *
              </label>
              <input
                v-model="form.MaximoBeneficios"
                type="number"
                required
                :class="['neumorphism-input', errors.MaximoBeneficios ? 'ring-2 ring-red-500' : '']"
                placeholder="Ingrese el máximo de beneficios"
              />
              <span v-if="errors.MaximoBeneficios" class="text-red-500 text-sm mt-1">{{ errors.MaximoBeneficios }}</span>
            </div>
          </div>

          <!-- Dirección -->
          <div class="mt-8">
            <h3 class="text-3xl font-black text-black text-center">Dirección</h3>
            <div class="w-full h-[5px] mt-2 mb-6 rounded neumorphism-on-small-item"></div>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <!-- Provincia -->
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">
                  Provincia *
                </label>
                <select
                  v-model="form.Provincia"
                  required
                  :class="['neumorphism-input neumorphism-input-select', errors.Provincia ? 'ring-2 ring-red-500' : '']"
                >
                  <option value="">Seleccione provincia</option>
                  <option value="San José">San José</option>
                  <option value="Alajuela">Alajuela</option>
                  <option value="Cartago">Cartago</option>
                  <option value="Heredia">Heredia</option>
                  <option value="Guanacaste">Guanacaste</option>
                  <option value="Puntarenas">Puntarenas</option>
                  <option value="Limón">Limón</option>
                </select>
                <span v-if="errors.Provincia" class="text-red-500 text-sm mt-1">{{ errors.Provincia }}</span>
              </div>

              <!-- Cantón -->
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">
                  Cantón
                </label>
                <input
                  v-model="form.Canton"
                  type="text"
                  maxlength="30"
                  :class="['neumorphism-input', errors.Canton ? 'ring-2 ring-red-500' : '']"
                  placeholder="Ingrese el cantón"
                />
                <span v-if="errors.Canton" class="text-red-500 text-sm mt-1">{{ errors.Canton }}</span>
              </div>

              <!-- Distrito -->
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">
                  Distrito
                </label>
                <input
                  v-model="form.Distrito"
                  type="text"
                  maxlength="30"
                  :class="['neumorphism-input', errors.Distrito ? 'ring-2 ring-red-500' : '']"
                  placeholder="Ingrese el distrito"
                />
                <span v-if="errors.Distrito" class="text-red-500 text-sm mt-1">{{ errors.Distrito }}</span>
              </div>

              <!-- Dirección Particular -->
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">
                  Dirección Particular
                </label>
                <input
                  v-model="form.DireccionParticular"
                  type="text"
                  maxlength="150"
                  :class="['neumorphism-input', errors.DireccionParticular ? 'ring-2 ring-red-500' : '']"
                  placeholder="Dirección específica (opcional)"
                />
                <span v-if="errors.DireccionParticular" class="text-red-500 text-sm mt-1">{{ errors.DireccionParticular }}</span>
              </div>
            </div>
          </div>

          <!-- Botones -->
          <div class="flex justify-end gap-6 pt-6">
            <button
              type="button"
              @click="handleCancel"
              class="neumorphism-button-normal-light"
            >
              <span>Cancelar</span>
            </button>
            
            <button
              type="submit"
              :disabled="loading"
              class="neumorphism-button-normal-blue"
            >
              <span>{{ loading ? 'Registrando...' : 'Registrar Empresa' }}</span>
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script>
import axios from 'axios'
import MainEmployerHeader from '../common/MainEmployerHeader.vue'
import DashboardProjectSubHeader from './projectDashboard/DashboardProjectSubHeader.vue'
import apiConfig from '../../config/api.js'

export default {
  name: 'CompanyRegistration',
  components: {
    MainEmployerHeader,
    DashboardProjectSubHeader
  },
  data() {
    return {
      companies: [],
      loading: false,
      errorMessage: '',
      successMessage: '',
      formattedTelefono: '',
      selectedSection: 'dashboard',
      form: {
        Nombre: '',
        CedulaJuridica: '',
        Email: '',
        Telefono: '',
        PeriodoPago: '',
        Provincia: '',
        Canton: '',
        Distrito: '',
        DireccionParticular: '',
        MaximoBeneficios: ''
      },
      errors: {}
    }
  },
  methods: {
    formatTelefono(event) {
      let value = event.target.value.replace(/\D/g, '')
      if (value.length > 4) {
        value = value.slice(0, 4) + '-' + value.slice(4, 8)
      }
      this.formattedTelefono = value
      this.form.Telefono = value.replace('-', '')
    },
    validateForm() {
      this.errors = {}
      let isValid = true
      if (!this.form.Nombre || this.form.Nombre.trim().length === 0) {
        this.errors.Nombre = 'El nombre es requerido'
        isValid = false
      } else if (this.form.Nombre.length > 20) {
        this.errors.Nombre = 'El nombre no puede exceder 20 caracteres'
        isValid = false
      }
      if (!this.form.CedulaJuridica || this.form.CedulaJuridica.toString().length !== 9) {
        this.errors.CedulaJuridica = 'La cédula jurídica debe tener 9 dígitos'
        isValid = false
      }
      if (!this.form.Email || this.form.Email.trim().length === 0) {
        this.errors.Email = 'El correo electrónico es requerido'
        isValid = false
      } else if (this.form.Email.length > 50) {
        this.errors.Email = 'El correo no puede exceder 50 caracteres'
        isValid = false
      } else {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
        if (!emailRegex.test(this.form.Email)) {
          this.errors.Email = 'Formato de correo electrónico inválido'
          isValid = false
        }
      }
      if (this.form.Telefono && this.form.Telefono.toString().length !== 8) {
        this.errors.Telefono = 'El teléfono debe tener 8 dígitos'
        isValid = false
      }
      if (!this.form.PeriodoPago) {
        this.errors.PeriodoPago = 'El período de pago es requerido'
        isValid = false
      }
      if (!this.form.Provincia) {
        this.errors.Provincia = 'La provincia es requerida'
        isValid = false
      }
      if (this.form.Canton && this.form.Canton.length > 30) {
        this.errors.Canton = 'El cantón no puede exceder 30 caracteres'
        isValid = false
      }
      if (this.form.Distrito && this.form.Distrito.length > 30) {
        this.errors.Distrito = 'El distrito no puede exceder 30 caracteres'
        isValid = false
      }
      if (this.form.DireccionParticular && this.form.DireccionParticular.length > 150) {
        this.errors.DireccionParticular = 'La dirección no puede exceder 150 caracteres'
        isValid = false
      }
      if (!this.form.MaximoBeneficios || this.form.MaximoBeneficios < 1) {
        this.errors.MaximoBeneficios = 'El máximo de beneficios debe ser mayor a 0'
        isValid = false
      }
      return isValid
    },
    async handleSubmit() {
      this.loading = true
      this.errorMessage = ''
      this.successMessage = ''
      if (!this.validateForm()) {
        this.errorMessage = 'Por favor complete todos los campos obligatorios correctamente.'
        this.loading = false
        return
      }
      try {
        const dataToSend = {
          Nombre: this.form.Nombre.trim(),
          CedulaJuridica: parseInt(this.form.CedulaJuridica),
          Email: this.form.Email.trim(),
          PeriodoPago: this.form.PeriodoPago,
          Provincia: this.form.Provincia,
          Telefono: this.form.Telefono ? parseInt(this.form.Telefono) : null,
          Canton: this.form.Canton || null,
          Distrito: this.form.Distrito || null,
          DireccionParticular: this.form.DireccionParticular || null,
          MaximoBeneficios: parseInt(this.form.MaximoBeneficios),
          employerId: parseInt(localStorage.getItem('employerId'))
        }
        await axios.post(apiConfig.endpoints.project, dataToSend, {
          headers: { 'Content-Type': 'application/json' },
          timeout: 10000
        })
        this.successMessage = `¡Empresa "${this.form.Nombre}" registrada exitosamente! Redirigiendo al dashboard...`
        window.scrollTo({ top: 0, behavior: 'smooth' })
        setTimeout(() => {
          this.$router.push('/dashboard-main-employer')
        }, 2000)
      } catch (error) {
        window.scrollTo({ top: 0, behavior: 'smooth' })
        if (error.response?.status === 409) {
          this.errorMessage = error.response.data.message || 'Ya existe una empresa con estos datos'
        } else if (error.response?.status === 400) {
          if (error.response.data.errors) {
            const errs = Object.values(error.response.data.errors).flat()
            this.errorMessage = errs.join(', ')
          } else {
            this.errorMessage = error.response.data.message || 'Datos inválidos'
          }
        } else if (error.response?.status === 500) {
          this.errorMessage = 'Error interno del servidor. Por favor, intente más tarde.'
        } else if (error.code === 'ECONNABORTED') {
          this.errorMessage = 'Tiempo de espera agotado. Verifique su conexión.'
        } else if (error.message) {
          this.errorMessage = error.message
        } else {
          this.errorMessage = 'Error al registrar la empresa. Verifique su conexión.'
        }
      } finally {
        this.loading = false
      }
    },
    handleCancel() {
      if (this.hasFormData() && !confirm('¿Está seguro que desea cancelar? Los datos no guardados se perderán.')) {
        return
      }
      this.resetForm()
      this.$router.push({ path: '/dashboard-main-employer', query: { section: 'dashboard' } })
    },
    resetForm() {
      this.form = {
        Nombre: '',
        CedulaJuridica: '',
        Email: '',
        Telefono: '',
        PeriodoPago: '',
        Provincia: '',
        Canton: '',
        Distrito: '',
        DireccionParticular: '',
        MaximoBeneficios: ''
      }
      this.errorMessage = ''
      this.successMessage = ''
      this.errors = {}
    },
    hasFormData() {
      return Object.values(this.form).some(value =>
        value !== null && value !== undefined && value !== ''
      )
    }
  },
  provide() {
    return {}
  },
  inject: [],
  emits: [],
  mixins: [],
  extends: null,
  filters: {}
}
</script>
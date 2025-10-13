<template>
  <div class="min-h-screen bg-[#E9F7FF] p-0">
    <MainEmployerHeader :companies="companies" />

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
          Registro de proyecto
        </h1>
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
                :class="['neumorphism-input w-full', errors.Nombre ? 'ring-2 ring-red-500' : '']"
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
                :class="['neumorphism-input w-full', errors.CedulaJuridica ? 'ring-2 ring-red-500' : '']"
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
                :class="['neumorphism-input w-full', errors.Email ? 'ring-2 ring-red-500' : '']"
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
                :class="['neumorphism-input w-full', errors.Telefono ? 'ring-2 ring-red-500' : '']"
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
                :class="['neumorphism-input w-full', errors.PeriodoPago ? 'ring-2 ring-red-500' : '']"
              >
                <option value="">Seleccione período de pago</option>
                <option value="Mensual">Mensual</option>
                <option value="Quincenal">Quincenal</option>
              </select>
              <span v-if="errors.PeriodoPago" class="text-red-500 text-sm mt-1">{{ errors.PeriodoPago }}</span>
            </div>
          </div>

          <!-- Dirección -->
          <div class="mt-8">
            <h3 class="text-lg font-semibold text-gray-700 mb-4 neumorphism-card rounded-[12px] bg-[#E9F7FF] py-2 px-4">Dirección</h3>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <!-- Provincia -->
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">
                  Provincia *
                </label>
                <select
                  v-model="form.Provincia"
                  required
                  :class="['neumorphism-input w-full', errors.Provincia ? 'ring-2 ring-red-500' : '']"
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
                  :class="['neumorphism-input w-full', errors.Canton ? 'ring-2 ring-red-500' : '']"
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
                  :class="['neumorphism-input w-full', errors.Distrito ? 'ring-2 ring-red-500' : '']"
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
                  :class="['neumorphism-input w-full', errors.DireccionParticular ? 'ring-2 ring-red-500' : '']"
                  placeholder="Dirección específica (opcional)"
                />
                <span v-if="errors.DireccionParticular" class="text-red-500 text-sm mt-1">{{ errors.DireccionParticular }}</span>
              </div>
            </div>
          </div>

          <!-- Botones -->
          <div class="flex justify-end space-x-4 pt-6">
            <button
              type="button"
              @click="handleCancel"
              class="neumorphism-light px-6 py-3 rounded-lg text-gray-700 hover:bg-gray-100 transition flex items-center space-x-2"
            >
              <svg class="w-4 h-4" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12"/>
              </svg>
              <span>Cancelar</span>
            </button>
            
            <button
              type="submit"
              :disabled="loading"
              class="neumorphism-dark px-6 py-3 rounded-lg text-white hover:bg-blue-600 disabled:opacity-50 transition flex items-center space-x-2"
            >
              <svg v-if="loading" class="animate-spin w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m-15.357-2A8.001 8.001 0 0019.419 15m0 0H15"/>
              </svg>
              <svg v-else class="w-4 h-4" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" d="M5 13l4 4L19 7"/>
              </svg>
              <span>{{ loading ? 'Registrando...' : 'Registrar Empresa' }}</span>
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import axios from 'axios'
import MainEmployerHeader from './common/MainEmployerHeader.vue'
import '../assets/neumorphismGlobal.css'

const router = useRouter()
const companies = ref([])

const loading = ref(false)
const errorMessage = ref('')
const successMessage = ref('')
const formattedTelefono = ref('')

function formatTelefono(event) {
  let value = event.target.value.replace(/\D/g, '') // Remove non-digits
  if (value.length > 4) {
    value = value.slice(0, 4) + '-' + value.slice(4, 8)
  }
  formattedTelefono.value = value
  form.value.Telefono = value.replace('-', '') // Store only digits in form
}

const form = ref({
  Nombre: '',
  CedulaJuridica: '',
  Email: '',
  Telefono: '',
  PeriodoPago: '',
  Provincia: '',
  Canton: '',
  Distrito: '',
  DireccionParticular: ''
})
const errors = ref({})

function validateForm() {
  errors.value = {}
  let isValid = true

  if (!form.value.Nombre || form.value.Nombre.trim().length === 0) {
    errors.value.Nombre = 'El nombre es requerido'
    isValid = false
  } else if (form.value.Nombre.length > 20) {
    errors.value.Nombre = 'El nombre no puede exceder 20 caracteres'
    isValid = false
  }

  if (!form.value.CedulaJuridica || form.value.CedulaJuridica.toString().length !== 9) {
    errors.value.CedulaJuridica = 'La cédula jurídica debe tener 9 dígitos'
    isValid = false
  }

  if (!form.value.Email || form.value.Email.trim().length === 0) {
    errors.value.Email = 'El correo electrónico es requerido'
    isValid = false
  } else if (form.value.Email.length > 50) {
    errors.value.Email = 'El correo no puede exceder 50 caracteres'
    isValid = false
  } else {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
    if (!emailRegex.test(form.value.Email)) {
      errors.value.Email = 'Formato de correo electrónico inválido'
      isValid = false
    }
  }

  if (form.value.Telefono && form.value.Telefono.toString().length !== 8) {
    errors.value.Telefono = 'El teléfono debe tener 8 dígitos'
    isValid = false
  }

  if (!form.value.PeriodoPago) {
    errors.value.PeriodoPago = 'El período de pago es requerido'
    isValid = false
  }

  if (!form.value.Provincia) {
    errors.value.Provincia = 'La provincia es requerida'
    isValid = false
  }

  if (form.value.Canton && form.value.Canton.length > 30) {
    errors.value.Canton = 'El cantón no puede exceder 30 caracteres'
    isValid = false
  }

  if (form.value.Distrito && form.value.Distrito.length > 30) {
    errors.value.Distrito = 'El distrito no puede exceder 30 caracteres'
    isValid = false
  }

  if (form.value.DireccionParticular && form.value.DireccionParticular.length > 150) {
    errors.value.DireccionParticular = 'La dirección no puede exceder 150 caracteres'
    isValid = false
  }

  return isValid
}

async function handleSubmit() {
  loading.value = true
  errorMessage.value = ''
  successMessage.value = ''

  if (!validateForm()) {
    errorMessage.value = 'Por favor complete todos los campos obligatorios correctamente.'
    loading.value = false
    return
  }

  try {
    const dataToSend = {
      Nombre: form.value.Nombre.trim(),
      CedulaJuridica: parseInt(form.value.CedulaJuridica),
      Email: form.value.Email.trim(),
      PeriodoPago: form.value.PeriodoPago,
      Provincia: form.value.Provincia,
      Telefono: form.value.Telefono ? parseInt(form.value.Telefono) : null,
      Canton: form.value.Canton || null,
      Distrito: form.value.Distrito || null,
      DireccionParticular: form.value.DireccionParticular || null
    }

    await axios.post('http://localhost:5011/api/Project', dataToSend, {
      headers: { 'Content-Type': 'application/json' },
      timeout: 10000
    })

    successMessage.value = `¡Empresa "${form.value.Nombre}" registrada exitosamente! Redirigiendo al dashboard...`
    window.scrollTo({ top: 0, behavior: 'smooth' })
    setTimeout(() => {
      router.push('/dashboard-main-employer')
    }, 2000)
  } catch (error) {
    window.scrollTo({ top: 0, behavior: 'smooth' })
    if (error.response?.status === 409) {
      errorMessage.value = error.response.data.message || 'Ya existe una empresa con estos datos'
    } else if (error.response?.status === 400) {
      if (error.response.data.errors) {
        const errs = Object.values(error.response.data.errors).flat()
        errorMessage.value = errs.join(', ')
      } else {
        errorMessage.value = error.response.data.message || 'Datos inválidos'
      }
    } else if (error.response?.status === 500) {
      errorMessage.value = 'Error interno del servidor. Por favor, intente más tarde.'
    } else if (error.code === 'ECONNABORTED') {
      errorMessage.value = 'Tiempo de espera agotado. Verifique su conexión.'
    } else if (error.message) {
      errorMessage.value = error.message
    } else {
      errorMessage.value = 'Error al registrar la empresa. Verifique su conexión.'
    }
  } finally {
    loading.value = false
  }
}

function handleCancel() {
  if (hasFormData() && !confirm('¿Está seguro que desea cancelar? Los datos no guardados se perderán.')) {
    return
  }
  resetForm()
  router.push('/dashboard-main-employer')
}

function resetForm() {
  form.value = {
    Nombre: '',
    CedulaJuridica: '',
    Email: '',
    Telefono: '',
    PeriodoPago: '',
    Provincia: '',
    Canton: '',
    Distrito: '',
    DireccionParticular: ''
  }
  errorMessage.value = ''
  successMessage.value = ''
  errors.value = {}
}

function hasFormData() {
  return Object.values(form.value).some(value =>
    value !== null && value !== undefined && value !== ''
  )
}
</script>
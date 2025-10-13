<template>
  <div class="min-h-screen flex items-center justify-center bg-[#E9F7FF] pt-[95px]">
    <HeaderLandingPage />
    <div class="my-20 neumorphism-card bg-[#eaf4fa] rounded-[40px] p-10 w-full max-w-4xl flex flex-col items-center">
      <h1 class="text-5xl font-black mb-8 mt-2 text-black tracking-wide text-center py-2 px-4">
        Registro de Empleador
      </h1>

      <form @submit.prevent="submitForm" class="w-full space-y-6">
        <!-- Personal Information Section -->
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div>
            <label class="block mb-1 font-medium text-gray-700">Nombre*</label>
            <input
              v-model="form.nombre"
              required
              :class="['neumorphism-input w-full', errors.nombre ? 'ring-2 ring-red-500' : '']"
              placeholder="Ingresa tu nombre"
            />
            <span v-if="errors.nombre" class="text-red-500 text-sm mt-1">{{ errors.nombre }}</span>
          </div>
          <div>
            <label class="block mb-1 font-medium text-gray-700">Primer Apellido*</label>
            <input
              v-model="form.primerApellido"
              required
              :class="['neumorphism-input w-full', errors.primerApellido ? 'ring-2 ring-red-500' : '']"
              placeholder="Ingresa tu primer apellido"
            />
            <span v-if="errors.primerApellido" class="text-red-500 text-sm mt-1">{{ errors.primerApellido }}</span>
          </div>
          <div>
            <label class="block mb-1 font-medium text-gray-700">Segundo Apellido (opcional)</label>
            <input
              v-model="form.segundoApellido"
              :class="['neumorphism-input w-full', errors.segundoApellido ? 'ring-2 ring-red-500' : '']"
              placeholder="Ingresa tu segundo apellido"
            />
            <span v-if="errors.segundoApellido" class="text-red-500 text-sm mt-1">{{ errors.segundoApellido }}</span>
          </div>
          <div>
            <label class="block mb-1 font-medium text-gray-700">Cédula*</label>
            <input
              v-model="form.cedula"
              required
              @input="formatCedula"
              :class="['neumorphism-input w-full', errors.cedula ? 'ring-2 ring-red-500' : '']"
              placeholder="Cédula (#-####-####)"
            />
            <span v-if="errors.cedula" class="text-red-500 text-sm mt-1">{{ errors.cedula }}</span>
          </div>
          <div>
            <label class="block mb-1 font-medium text-gray-700">Correo electrónico*</label>
            <input
              v-model="form.email"
              type="email"
              required
              :class="['neumorphism-input w-full', errors.email ? 'ring-2 ring-red-500' : '']"
              placeholder="ejemplo@email.com"
            />
            <span v-if="errors.email" class="text-red-500 text-sm mt-1">{{ errors.email }}</span>
          </div>
          <div>
            <label class="block mb-1 font-medium text-gray-700">Número de teléfono*</label>
            <input
              v-model="formattedTelefono"
              type="tel"
              required
              @input="formatTelefono"
              :class="['neumorphism-input w-full', errors.telefono ? 'ring-2 ring-red-500' : '']"
              placeholder="#### ####"
            />
            <span v-if="errors.telefono" class="text-red-500 text-sm mt-1">{{ errors.telefono }}</span>
          </div>
          <div>
            <label class="block mb-1 font-medium text-gray-700">Fecha de nacimiento*</label>
            <input
              type="date"
              v-model="form.fechaNacimiento"
              required
              :class="['neumorphism-input w-full', errors.fechaNacimiento ? 'ring-2 ring-red-500' : '']"
            />
            <span v-if="errors.fechaNacimiento" class="text-red-500 text-sm mt-1">{{ errors.fechaNacimiento }}</span>
          </div>
        </div>

        <!-- Address Information Section -->
        <div class="mt-8">
          <h3 class="text-xl font-semibold text-gray-700 mb-4 neumorphism-card rounded-[12px] bg-[#E9F7FF] py-2 px-4">Información de Dirección</h3>
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div class="relative">
              <label class="block mb-1 font-medium text-gray-700">Provincia*</label>
              <select
                v-model="form.provincia"
                required
                :class="['neumorphism-input w-full appearance-none cursor-pointer', errors.provincia ? 'ring-2 ring-red-500' : '']"
              >
                <option value="" disabled selected>Provincia</option>
                <option v-for="provincia in provinciasCostaRica" :key="provincia" :value="provincia">
                  {{ provincia }}
                </option>
              </select>
              <div class="absolute inset-y-0 right-0 flex items-center pr-6 pointer-events-none">
                <svg class="w-5 h-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"/>
                </svg>
              </div>
              <span v-if="errors.provincia" class="text-red-500 text-sm mt-1">{{ errors.provincia }}</span>
            </div>
            <div>
              <label class="block mb-1 font-medium text-gray-700">Cantón*</label>
              <input
                v-model="form.canton"
                required
                :class="['neumorphism-input w-full', errors.canton ? 'ring-2 ring-red-500' : '']"
                placeholder="Cantón"
              />
              <span v-if="errors.canton" class="text-red-500 text-sm mt-1">{{ errors.canton }}</span>
            </div>
            <div>
              <label class="block mb-1 font-medium text-gray-700">Distrito*</label>
              <input
                v-model="form.distrito"
                required
                :class="['neumorphism-input w-full', errors.distrito ? 'ring-2 ring-red-500' : '']"
                placeholder="Distrito"
              />
              <span v-if="errors.distrito" class="text-red-500 text-sm mt-1">{{ errors.distrito }}</span>
            </div>
            <div>
              <label class="block mb-1 font-medium text-gray-700">Dirección Particular*</label>
              <input
                v-model="form.direccionParticular"
                required
                :class="['neumorphism-input w-full', errors.direccionParticular ? 'ring-2 ring-red-500' : '']"
                placeholder="Dirección Particular"
              />
              <span v-if="errors.direccionParticular" class="text-red-500 text-sm mt-1">{{ errors.direccionParticular }}</span>
            </div>
          </div>
        </div>

        <!-- Password Section -->
        <div class="mt-8">
          <h3 class="text-xl font-semibold text-gray-700 mb-4 neumorphism-card rounded-[12px] bg-[#E9F7FF] py-2 px-4">Información de Acceso</h3>
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div>
              <label class="block mb-1 font-medium text-gray-700">Contraseña*</label>
              <div class="relative">
                <input
                  :type="showPassword ? 'text' : 'password'"
                  v-model="form.password"
                  required
                  :class="['neumorphism-input w-full pr-12', errors.password ? 'ring-2 ring-red-500' : '']"
                  placeholder="********"
                />
                <button
                  type="button"
                  @click="showPassword = !showPassword"
                  class="absolute inset-y-0 right-0 flex items-center pr-4 text-gray-500 text-sm"
                >
                  {{ showPassword ? 'Ocultar' : 'Ver' }}
                </button>
              </div>
              <span v-if="errors.password" class="text-red-500 text-sm mt-1">{{ errors.password }}</span>
              <ul class="text-xs text-gray-600 mt-1 ml-2 list-disc list-inside">
                <li>Mínimo 8 caracteres</li>
                <li>Máximo 16 caracteres</li>
              </ul>
            </div>
            <div>
              <label class="block mb-1 font-medium text-gray-700">Confirmar Contraseña*</label>
              <input
                :type="showPassword ? 'text' : 'password'"
                v-model="form.confirmPassword"
                required
                :class="['neumorphism-input w-full', errors.confirmPassword ? 'ring-2 ring-red-500' : '']"
                placeholder="Repite tu contraseña"
              />
              <span v-if="errors.confirmPassword" class="text-red-500 text-sm mt-1">{{ errors.confirmPassword }}</span>
            </div>
          </div>
        </div>

        <!-- Botón registro -->
        <div class="flex justify-center mt-8">
          <button
            type="submit"
            class="custom-button text-xl text-gray-700"
          >
            Registrarse
          </button>
        </div>
      </form>

      <!-- Verificación de correo -->
      <div v-if="showVerification" class="mt-10 text-center md:col-span-2">
        <h3 class="text-xl font-semibold mb-2 neumorphism-card rounded-[12px] bg-[#E9F7FF] py-2 px-4">Verifica tu correo electrónico</h3>
        <p class="text-gray-700 mb-4">Ingresa el código de 6 dígitos enviado a tu correo:</p>
        <input
          v-model="verificationCode"
          maxlength="6"
          class="neumorphism-input rounded-full px-4 py-2 outline-none text-gray-700 text-center tracking-widest"
        />
        <button
          @click="verifyCode"
          class="mt-4 neumorphism-dark text-white rounded-full px-6 py-2 hover:bg-[#1e293b] transition-all"
        >
          Verificar
        </button>
        <span v-if="verificationError" class="block text-red-500 text-sm mt-2">{{ verificationError }}</span>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import axios from 'axios'
import HeaderLandingPage from './common/HeaderLandingPage.vue'

const form = ref({
  nombre: '',
  primerApellido: '',
  segundoApellido: '',
  cedula: '',
  email: '',
  telefono: '',
  fechaNacimiento: '',
  password: '',
  confirmPassword: '',
  provincia: '',
  canton: '',
  distrito: '',
  direccionParticular: '',
})

const formattedTelefono = ref('')
const errors = ref({})
const showPassword = ref(false)
const showVerification = ref(false)
const verificationCode = ref('')
const verificationError = ref('')
const provinciasCostaRica = [
  'San José',
  'Alajuela',
  'Cartago',
  'Heredia',
  'Guanacaste',
  'Puntarenas',
  'Limón'
]

function formatCedula(event) {
  let value = event.target.value.replace(/\D/g, '')
  if (value.length > 1) {
    if (value.length <= 5) {
      value = value.slice(0, 1) + '-' + value.slice(1)
    } else {
      value = value.slice(0, 1) + '-' + value.slice(1, 5) + '-' + value.slice(5, 9)
    }
  }
  form.value.cedula = value
}

function formatTelefono(event) {
  let value = event.target.value.replace(/\D/g, '')
  if (value.length > 4) {
    value = value.slice(0, 4) + ' ' + value.slice(4, 8)
  }
  formattedTelefono.value = value
  form.value.telefono = value.replace(/\s/g, '')
}

async function submitForm() {
  if (validateForm()) {
    try {
      const employerData = {
        ...form.value,
        cedula: form.value.cedula.replace(/-/g, ''),
        telefono: parseInt(form.value.telefono),
        fechaNacimiento: new Date(form.value.fechaNacimiento).toISOString(),
      }

      const response = await axios.post(
        'http://localhost:5011/api/SignUpEmployer',
        employerData,
        { headers: { 'Content-Type': 'application/json' } }
      )

      if (response.data.message) {
        alert(response.data.message)
        window.location.href = '/login'
      } else {
        showVerification.value = true
      }
    } catch (error) {
      if (error.response && error.response.data && error.response.data.message) {
        alert(`Error: ${error.response.data.message}`)
      } else {
        alert('Error en el registro. Por favor, intenta de nuevo.')
      }
    }
  }
}

function validateForm() {
  errors.value = {}
  let isValid = true

  // Personal Information Validation
  if (!form.value.nombre || form.value.nombre.trim().length === 0) {
    errors.value.nombre = 'El nombre es requerido'
    isValid = false
  } else if (form.value.nombre.length > 20) {
    errors.value.nombre = 'El nombre no puede exceder 20 caracteres'
    isValid = false
  }

  if (!form.value.primerApellido || form.value.primerApellido.trim().length === 0) {
    errors.value.primerApellido = 'El primer apellido es requerido'
    isValid = false
  } else if (form.value.primerApellido.length > 20) {
    errors.value.primerApellido = 'El primer apellido no puede exceder 20 caracteres'
    isValid = false
  }

  if (form.value.segundoApellido && form.value.segundoApellido.length > 20) {
    errors.value.segundoApellido = 'El segundo apellido no puede exceder 20 caracteres'
    isValid = false
  }

  if (!form.value.cedula || form.value.cedula.trim().length === 0) {
    errors.value.cedula = 'La cédula es requerida'
    isValid = false
  } else {
    const cedulaDigits = form.value.cedula.replace(/-/g, '')
    if (cedulaDigits.length !== 9) {
      errors.value.cedula = 'La cédula debe tener 9 dígitos'
      isValid = false
    }
  }

  if (!form.value.email || form.value.email.trim().length === 0) {
    errors.value.email = 'El correo electrónico es requerido'
    isValid = false
  } else if (form.value.email.length > 50) {
    errors.value.email = 'El correo no puede exceder 50 caracteres'
    isValid = false
  } else {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
    if (!emailRegex.test(form.value.email)) {
      errors.value.email = 'Formato de correo electrónico inválido'
      isValid = false
    }
  }

  if (!form.value.telefono || form.value.telefono.trim().length === 0) {
    errors.value.telefono = 'El teléfono es requerido'
    isValid = false
  } else {
    const phoneDigits = form.value.telefono.replace(/\s/g, '')
    if (phoneDigits.length !== 8) {
      errors.value.telefono = 'El teléfono debe tener 8 dígitos (#### ####)'
      isValid = false
    }
  }

  if (!form.value.fechaNacimiento) {
    errors.value.fechaNacimiento = 'La fecha de nacimiento es requerida'
    isValid = false
  } else {
    const today = new Date()
    const birthDate = new Date(form.value.fechaNacimiento)
    const age = today.getFullYear() - birthDate.getFullYear()
    const monthDiff = today.getMonth() - birthDate.getMonth()
    const dayDiff = today.getDate() - birthDate.getDate()

    if (age < 18 || (age === 18 && monthDiff < 0) || (age === 18 && monthDiff === 0 && dayDiff < 0)) {
      errors.value.fechaNacimiento = 'Debe tener al menos 18 años'
      isValid = false
    } else if (age > 99 || (age === 99 && monthDiff < 0) || (age === 99 && monthDiff === 0 && dayDiff < 0)) {
      errors.value.fechaNacimiento = 'Debe tener menos de 99 años'
      isValid = false
    }
  }

  // Address Validation
  if (!form.value.provincia || form.value.provincia.trim().length === 0) {
    errors.value.provincia = 'La provincia es requerida'
    isValid = false
  } else if (form.value.provincia.length > 12) {
    errors.value.provincia = 'La provincia no puede exceder 12 caracteres'
    isValid = false
  }

  if (!form.value.canton || form.value.canton.trim().length === 0) {
    errors.value.canton = 'El cantón es requerido'
    isValid = false
  } else if (form.value.canton.length > 30) {
    errors.value.canton = 'El cantón no puede exceder 30 caracteres'
    isValid = false
  }

  if (!form.value.distrito || form.value.distrito.trim().length === 0) {
    errors.value.distrito = 'El distrito es requerido'
    isValid = false
  } else if (form.value.distrito.length > 30) {
    errors.value.distrito = 'El distrito no puede exceder 30 caracteres'
    isValid = false
  }

  if (!form.value.direccionParticular || form.value.direccionParticular.trim().length === 0) {
    errors.value.direccionParticular = 'La dirección particular es requerida'
    isValid = false
  } else if (form.value.direccionParticular.length > 150) {
    errors.value.direccionParticular = 'La dirección no puede exceder 150 caracteres'
    isValid = false
  }

  // Password Validation
  if (!form.value.password || form.value.password.trim().length === 0) {
    errors.value.password = 'La contraseña es requerida'
    isValid = false
  } else if (form.value.password.length < 8) {
    errors.value.password = 'La contraseña debe tener al menos 8 caracteres'
    isValid = false
  } else if (form.value.password.length > 16) {
    errors.value.password = 'La contraseña no puede exceder 16 caracteres'
    isValid = false
  }

  if (!form.value.confirmPassword || form.value.confirmPassword.trim().length === 0) {
    errors.value.confirmPassword = 'La confirmación de contraseña es requerida'
    isValid = false
  } else if (form.value.password !== form.value.confirmPassword) {
    errors.value.confirmPassword = 'Las contraseñas no coinciden'
    isValid = false
  }

  return isValid
}

function verifyCode() {
  if (/^\d{6}$/.test(verificationCode.value)) {
    window.location.href = '/login'
  } else {
    verificationError.value = 'El código debe ser de 6 dígitos.'
  }
}
</script>
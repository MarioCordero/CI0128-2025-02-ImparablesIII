<template>
  <div class="page flex flex-col items-center justify-center font-montserrat">
    <HeaderLandingPage />

    <div class="body">
      <div class="neumorphism-card-modal w-full flex flex-col items-center">
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
                :class="['neumorphism-input', errors.nombre ? 'ring-2 ring-red-500' : '']"
                placeholder="Ingresa tu nombre"
              />
              <span v-if="errors.nombre" class="text-red-500 text-sm mt-1">{{ errors.nombre }}</span>
            </div>
            <div>
              <label class="block mb-1 font-medium text-gray-700">Primer Apellido*</label>
              <input
                v-model="form.primerApellido"
                required
                :class="['neumorphism-input', errors.primerApellido ? 'ring-2 ring-red-500' : '']"
                placeholder="Ingresa tu primer apellido"
              />
              <span v-if="errors.primerApellido" class="text-red-500 text-sm mt-1">{{ errors.primerApellido }}</span>
            </div>
            <div>
              <label class="block mb-1 font-medium text-gray-700">Segundo Apellido (opcional)</label>
              <input
                v-model="form.segundoApellido"
                :class="['neumorphism-input', errors.segundoApellido ? 'ring-2 ring-red-500' : '']"
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
                :class="['neumorphism-input', errors.cedula ? 'ring-2 ring-red-500' : '']"
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
                :class="['neumorphism-input', errors.email ? 'ring-2 ring-red-500' : '']"
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
                :class="['neumorphism-input', errors.telefono ? 'ring-2 ring-red-500' : '']"
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
                :class="['neumorphism-input', errors.fechaNacimiento ? 'ring-2 ring-red-500' : '']"
              />
              <span v-if="errors.fechaNacimiento" class="text-red-500 text-sm mt-1">{{ errors.fechaNacimiento }}</span>
            </div>
          </div>
  
          <!-- Address Information Section -->
          <div class="mt-8">
            <h3 class="text-xl font-semibold text-gray-700 mb-4 py-0 px-0">Información de Dirección</h3>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <label class="block mb-1 font-medium text-gray-700">Provincia*</label>
                <div class="relative">
                  <select
                    v-model="form.provincia"
                    required
                    :class="['neumorphism-input neumorphism-input-select', errors.provincia ? 'ring-2 ring-red-500' : '']"
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
              </div>
              <div>
                <label class="block mb-1 font-medium text-gray-700">Cantón*</label>
                <input
                  v-model="form.canton"
                  required
                  :class="['neumorphism-input', errors.canton ? 'ring-2 ring-red-500' : '']"
                  placeholder="Cantón"
                />
                <span v-if="errors.canton" class="text-red-500 text-sm mt-1">{{ errors.canton }}</span>
              </div>
              <div>
                <label class="block mb-1 font-medium text-gray-700">Distrito*</label>
                <input
                  v-model="form.distrito"
                  required
                  :class="['neumorphism-input', errors.distrito ? 'ring-2 ring-red-500' : '']"
                  placeholder="Distrito"
                />
                <span v-if="errors.distrito" class="text-red-500 text-sm mt-1">{{ errors.distrito }}</span>
              </div>
              <div>
                <label class="block mb-1 font-medium text-gray-700">Dirección Particular*</label>
                <input
                  v-model="form.direccionParticular"
                  required
                  :class="['neumorphism-input', errors.direccionParticular ? 'ring-2 ring-red-500' : '']"
                  placeholder="Dirección Particular"
                />
                <span v-if="errors.direccionParticular" class="text-red-500 text-sm mt-1">{{ errors.direccionParticular }}</span>
              </div>
            </div>
          </div>
  
          <!-- Password Section -->
          <div class="mt-8">
            <h3 class="text-xl font-semibold text-gray-700 mb-4 py-0 px-0">Información de Acceso</h3>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <label class="block mb-1 font-medium text-gray-700">Contraseña*</label>
                <div class="relative">
                  <input
                    :type="showPassword ? 'text' : 'password'"
                    v-model="form.password"
                    required
                    :class="['neumorphism-input pr-12', errors.password ? 'ring-2 ring-red-500' : '']"
                    placeholder="Ingresa una contraseña"
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
                  :class="['neumorphism-input', errors.confirmPassword ? 'ring-2 ring-red-500' : '']"
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
              class="neumorphism-button-xl-light !font-medium w-full"
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

    
  </div>
</template>

<script>
import axios from 'axios'
import HeaderLandingPage from './common/HeaderLandingPage.vue'
import apiConfig from '../config/api.js'

export default {
  name: 'SignUpEmployer',
  components: {
    HeaderLandingPage
  },

  // 5. Estado reactivo del componente
  data() {
    return {
      form: {
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
      },
      formattedTelefono: '',
      errors: {},
      showPassword: false,
      showVerification: false,
      verificationCode: '',
      verificationError: '',
      provinciasCostaRica: [
        'San José',
        'Alajuela',
        'Cartago',
        'Heredia',
        'Guanacaste',
        'Puntarenas',
        'Limón'
      ]
    }
  },

  // 8. Métodos y lógica ejecutable
  methods: {
    formatCedula(event) {
      let value = event.target.value.replace(/\D/g, '')
      if (value.length > 1) {
        if (value.length <= 5) {
          value = value.slice(0, 1) + '-' + value.slice(1)
        } else {
          value = value.slice(0, 1) + '-' + value.slice(1, 5) + '-' + value.slice(5, 9)
        }
      }
      this.form.cedula = value
    },
    formatTelefono(event) {
      let value = event.target.value.replace(/\D/g, '')
      if (value.length > 4) {
        value = value.slice(0, 4) + ' ' + value.slice(4, 8)
      }
      this.formattedTelefono = value
      this.form.telefono = value.replace(/\s/g, '')
    },
    async submitForm() {
      if (this.validateForm()) {
        try {
          console.log("form verificado");
          const employerData = {
            ...this.form,
            cedula: this.form.cedula.replace(/-/g, ''),
            telefono: parseInt(this.form.telefono),
            fechaNacimiento: new Date(this.form.fechaNacimiento).toISOString(),
          }

          console.log("enviando al endpoint");
          const response = await axios.post(
            apiConfig.endpoints.signUpEmployer,
            employerData,
            { headers: { 'Content-Type': 'application/json' } }
          )
          console.log("se envio al endpoint");

          if (response.data.message) {
            console.log("el endpoint respondio " + response.data.message);
            alert(response.data.message)
            window.location.href = '/login'
          } else {
            console.log("el endpoint dice que mostremos la verificacion");
            this.showVerification = true
          }
        } catch (error) {
          if (error.response && error.response.data && error.response.data.message) {
            alert(`Error: ${error.response.data.message}`)
          } else {
            alert('Error en el registro. Por favor, intenta de nuevo.')
          }
        }
      }
    },

    validateForm() {
      this.errors = {}
      let isValid = true
      if (!this.form.nombre || this.form.nombre.trim().length === 0) {
        this.errors.nombre = 'El nombre es requerido'
        isValid = false
      } else if (this.form.nombre.length > 20) {
        this.errors.nombre = 'El nombre no puede exceder 20 caracteres'
        isValid = false
      }
      if (!this.form.primerApellido || this.form.primerApellido.trim().length === 0) {
        this.errors.primerApellido = 'El primer apellido es requerido'
        isValid = false
      } else if (this.form.primerApellido.length > 20) {
        this.errors.primerApellido = 'El primer apellido no puede exceder 20 caracteres'
        isValid = false
      }
      if (this.form.segundoApellido && this.form.segundoApellido.length > 20) {
        this.errors.segundoApellido = 'El segundo apellido no puede exceder 20 caracteres'
        isValid = false
      }
      if (!this.form.cedula || this.form.cedula.trim().length === 0) {
        this.errors.cedula = 'La cédula es requerida'
        isValid = false
      } else {
        const cedulaDigits = this.form.cedula.replace(/-/g, '')
        if (cedulaDigits.length !== 9) {
          this.errors.cedula = 'La cédula debe tener 9 dígitos'
          isValid = false
        }
      }
      if (!this.form.email || this.form.email.trim().length === 0) {
        this.errors.email = 'El correo electrónico es requerido'
        isValid = false
      } else if (this.form.email.length > 50) {
        this.errors.email = 'El correo no puede exceder 50 caracteres'
        isValid = false
      } else {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
        if (!emailRegex.test(this.form.email)) {
          this.errors.email = 'Formato de correo electrónico inválido'
          isValid = false
        }
      }
      if (!this.form.telefono || this.form.telefono.trim().length === 0) {
        this.errors.telefono = 'El teléfono es requerido'
        isValid = false
      } else {
        const phoneDigits = this.form.telefono.replace(/\s/g, '')
        if (phoneDigits.length !== 8) {
          this.errors.telefono = 'El teléfono debe tener 8 dígitos (#### ####)'
          isValid = false
        }
      }
      if (!this.form.fechaNacimiento) {
        this.errors.fechaNacimiento = 'La fecha de nacimiento es requerida'
        isValid = false
      } else {
        const today = new Date()
        const birthDate = new Date(this.form.fechaNacimiento)
        const age = today.getFullYear() - birthDate.getFullYear()
        const monthDiff = today.getMonth() - birthDate.getMonth()
        const dayDiff = today.getDate() - birthDate.getDate()

        if (age < 18 || (age === 18 && monthDiff < 0) || (age === 18 && monthDiff === 0 && dayDiff < 0)) {
          this.errors.fechaNacimiento = 'Debe tener al menos 18 años'
          isValid = false
        } else if (age > 99 || (age === 99 && monthDiff < 0) || (age === 99 && monthDiff === 0 && dayDiff < 0)) {
          this.errors.fechaNacimiento = 'Debe tener menos de 99 años'
          isValid = false
        }
      }
      if (!this.form.provincia || this.form.provincia.trim().length === 0) {
        this.errors.provincia = 'La provincia es requerida'
        isValid = false
      } else if (this.form.provincia.length > 12) {
        this.errors.provincia = 'La provincia no puede exceder 12 caracteres'
        isValid = false
      }
      if (!this.form.canton || this.form.canton.trim().length === 0) {
        this.errors.canton = 'El cantón es requerido'
        isValid = false
      } else if (this.form.canton.length > 30) {
        this.errors.canton = 'El cantón no puede exceder 30 caracteres'
        isValid = false
      }
      if (!this.form.distrito || this.form.distrito.trim().length === 0) {
        this.errors.distrito = 'El distrito es requerido'
        isValid = false
      } else if (this.form.distrito.length > 30) {
        this.errors.distrito = 'El distrito no puede exceder 30 caracteres'
        isValid = false
      }
      if (!this.form.direccionParticular || this.form.direccionParticular.trim().length === 0) {
        this.errors.direccionParticular = 'La dirección particular es requerida'
        isValid = false
      } else if (this.form.direccionParticular.length > 150) {
        this.errors.direccionParticular = 'La dirección no puede exceder 150 caracteres'
        isValid = false
      }
      if (!this.form.password || this.form.password.trim().length === 0) {
        this.errors.password = 'La contraseña es requerida'
        isValid = false
      } else if (this.form.password.length < 8) {
        this.errors.password = 'La contraseña debe tener al menos 8 caracteres'
        isValid = false
      } else if (this.form.password.length > 16) {
        this.errors.password = 'La contraseña no puede exceder 16 caracteres'
        isValid = false
      }
      if (!this.form.confirmPassword || this.form.confirmPassword.trim().length === 0) {
        this.errors.confirmPassword = 'La confirmación de contraseña es requerida'
        isValid = false
      } else if (this.form.password !== this.form.confirmPassword) {
        this.errors.confirmPassword = 'Las contraseñas no coinciden'
        isValid = false
      }
      return isValid
    },
    verifyCode() {
      if (/^\d{6}$/.test(this.verificationCode)) {
        window.location.href = '/login'
      } else {
        this.verificationError = 'El código debe ser de 6 dígitos.'
      }
    }
  }
}
</script>
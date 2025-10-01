<template>
  <div class="min-h-screen flex items-center justify-center bg-[#dbeafe]">
    <div class="bg-[#eaf4fa] rounded-[40px] shadow-2xl p-10 w-full max-w-4xl flex flex-col items-center">
      <h1 class="text-5xl font-black mb-8 mt-2 text-black tracking-wide text-center">
        Registro de Empleador
      </h1>

      <form @submit.prevent="submitForm" class="w-full space-y-6">
        <!-- Personal Information Section -->
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <!-- Nombre -->
          <div>
            <label class="block mb-1 font-medium text-gray-700">Nombre*</label>
            <input
              v-model="form.nombre"
              required
              :class="[
                'w-full bg-white rounded-2xl px-6 py-4 text-gray-700 placeholder-gray-400 border-0 shadow-inner focus:outline-none focus:ring-2 focus:ring-[#87ceeb] focus:shadow-lg transition-all',
                errors.nombre ? 'ring-2 ring-red-500' : ''
              ]"
              placeholder="Ingresa tu nombre"
            />
            <span v-if="errors.nombre" class="text-red-500 text-sm mt-1">{{ errors.nombre }}</span>
          </div>

          <!-- Primer Apellido -->
          <div>
            <label class="block mb-1 font-medium text-gray-700">Primer Apellido*</label>
            <input
              v-model="form.primerApellido"
              required
              :class="[
                'w-full bg-white rounded-2xl px-6 py-4 text-gray-700 placeholder-gray-400 border-0 shadow-inner focus:outline-none focus:ring-2 focus:ring-[#87ceeb] focus:shadow-lg transition-all',
                errors.primerApellido ? 'ring-2 ring-red-500' : ''
              ]"
              placeholder="Ingresa tu primer apellido"
            />
            <span v-if="errors.primerApellido" class="text-red-500 text-sm mt-1">{{ errors.primerApellido }}</span>
          </div>

          <!-- Segundo Apellido -->
          <div>
            <label class="block mb-1 font-medium text-gray-700">Segundo Apellido (opcional)</label>
            <input
              v-model="form.segundoApellido"
              class="w-full bg-white rounded-2xl px-6 py-4 text-gray-700 placeholder-gray-400 border-0 shadow-inner focus:outline-none focus:ring-2 focus:ring-[#87ceeb] focus:shadow-lg transition-all"
              placeholder="Ingresa tu segundo apellido"
            />
            <span v-if="errors.segundoApellido" class="text-red-500 text-sm mt-1">{{ errors.segundoApellido }}</span>
          </div>

          <!-- Cédula -->
          <div>
            <label class="block mb-1 font-medium text-gray-700">Cédula*</label>
            <input
              v-model="form.cedula"
              required
              @input="formatCedula"
              :class="[
                'w-full bg-white rounded-2xl px-6 py-4 text-gray-700 placeholder-gray-400 border-0 shadow-inner focus:outline-none focus:ring-2 focus:ring-[#87ceeb] focus:shadow-lg transition-all',
                errors.cedula ? 'ring-2 ring-red-500' : ''
              ]"
              placeholder="Cédula (#-####-####)"
            />
            <span v-if="errors.cedula" class="text-red-500 text-sm mt-1">{{ errors.cedula }}</span>
          </div>

          <!-- Correo -->
          <div>
            <label class="block mb-1 font-medium text-gray-700">Correo electrónico*</label>
            <input
              v-model="form.email"
              type="email"
              required
              :class="[
                'w-full bg-white rounded-2xl px-6 py-4 text-gray-700 placeholder-gray-400 border-0 shadow-inner focus:outline-none focus:ring-2 focus:ring-[#87ceeb] focus:shadow-lg transition-all',
                errors.email ? 'ring-2 ring-red-500' : ''
              ]"
              placeholder="ejemplo@email.com"
            />
            <span v-if="errors.email" class="text-red-500 text-sm mt-1">{{ errors.email }}</span>
          </div>

          <!-- Teléfono -->
          <div>
            <label class="block mb-1 font-medium text-gray-700">Número de teléfono*</label>
            <input
              v-model="formattedTelefono"
              type="tel"
              required
              @input="formatTelefono"
              :class="[
                'w-full bg-white rounded-2xl px-6 py-4 text-gray-700 placeholder-gray-400 border-0 shadow-inner focus:outline-none focus:ring-2 focus:ring-[#87ceeb] focus:shadow-lg transition-all',
                errors.telefono ? 'ring-2 ring-red-500' : ''
              ]"
              placeholder="#### ####"
            />
            <span v-if="errors.telefono" class="text-red-500 text-sm mt-1">{{ errors.telefono }}</span>
          </div>

          <!-- Fecha de nacimiento -->
          <div>
            <label class="block mb-1 font-medium text-gray-700">Fecha de nacimiento*</label>
            <input
              type="date"
              v-model="form.fechaNacimiento"
              required
              :class="[
                'w-full bg-white rounded-2xl px-6 py-4 text-gray-700 border-0 shadow-inner focus:outline-none focus:ring-2 focus:ring-[#87ceeb] focus:shadow-lg transition-all',
                errors.fechaNacimiento ? 'ring-2 ring-red-500' : ''
              ]"
            />
            <span v-if="errors.fechaNacimiento" class="text-red-500 text-sm mt-1">{{ errors.fechaNacimiento }}</span>
          </div>
        </div>

        <!-- Address Information Section -->
        <div class="mt-8">
          <h3 class="text-xl font-semibold text-gray-700 mb-4">Información de Dirección</h3>
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <!-- Provincia -->
            <div class="relative">
              <label class="block mb-1 font-medium text-gray-700">Provincia*</label>
              <select
                v-model="form.provincia"
                required
                :class="[
                  'w-full bg-white rounded-2xl px-6 py-4 text-gray-700 border-0 shadow-inner focus:outline-none focus:ring-2 focus:ring-[#87ceeb] focus:shadow-lg transition-all appearance-none cursor-pointer',
                  errors.provincia ? 'ring-2 ring-red-500' : ''
                ]"
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

            <!-- Cantón -->
            <div>
              <label class="block mb-1 font-medium text-gray-700">Cantón*</label>
              <input
                v-model="form.canton"
                required
                :class="[
                  'w-full bg-white rounded-2xl px-6 py-4 text-gray-700 placeholder-gray-400 border-0 shadow-inner focus:outline-none focus:ring-2 focus:ring-[#87ceeb] focus:shadow-lg transition-all',
                  errors.canton ? 'ring-2 ring-red-500' : ''
                ]"
                placeholder="Cantón"
              />
              <span v-if="errors.canton" class="text-red-500 text-sm mt-1">{{ errors.canton }}</span>
            </div>

            <!-- Distrito -->
            <div>
              <label class="block mb-1 font-medium text-gray-700">Distrito*</label>
              <input
                v-model="form.distrito"
                required
                :class="[
                  'w-full bg-white rounded-2xl px-6 py-4 text-gray-700 placeholder-gray-400 border-0 shadow-inner focus:outline-none focus:ring-2 focus:ring-[#87ceeb] focus:shadow-lg transition-all',
                  errors.distrito ? 'ring-2 ring-red-500' : ''
                ]"
                placeholder="Distrito"
              />
              <span v-if="errors.distrito" class="text-red-500 text-sm mt-1">{{ errors.distrito }}</span>
            </div>

            <!-- Dirección Particular -->
            <div>
              <label class="block mb-1 font-medium text-gray-700">Dirección Particular*</label>
              <input
                v-model="form.direccionParticular"
                required
                :class="[
                  'w-full bg-white rounded-2xl px-6 py-4 text-gray-700 placeholder-gray-400 border-0 shadow-inner focus:outline-none focus:ring-2 focus:ring-[#87ceeb] focus:shadow-lg transition-all',
                  errors.direccionParticular ? 'ring-2 ring-red-500' : ''
                ]"
                placeholder="Dirección Particular"
              />
              <span v-if="errors.direccionParticular" class="text-red-500 text-sm mt-1">{{ errors.direccionParticular }}</span>
            </div>
          </div>
        </div>

        <!-- Password Section -->
        <div class="mt-8">
          <h3 class="text-xl font-semibold text-gray-700 mb-4">Información de Acceso</h3>
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <!-- Password -->
            <div>
              <label class="block mb-1 font-medium text-gray-700">Contraseña*</label>
              <div class="relative">
                <input
                  :type="showPassword ? 'text' : 'password'"
                  v-model="form.password"
                  required
                  :class="[
                    'w-full bg-white rounded-2xl px-6 py-4 text-gray-700 placeholder-gray-400 border-0 shadow-inner focus:outline-none focus:ring-2 focus:ring-[#87ceeb] focus:shadow-lg transition-all pr-12',
                    errors.password ? 'ring-2 ring-red-500' : ''
                  ]"
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

            <!-- Confirm Password -->
            <div>
              <label class="block mb-1 font-medium text-gray-700">Confirmar Contraseña*</label>
              <input
                :type="showPassword ? 'text' : 'password'"
                v-model="form.confirmPassword"
                required
                :class="[
                  'w-full bg-white rounded-2xl px-6 py-4 text-gray-700 placeholder-gray-400 border-0 shadow-inner focus:outline-none focus:ring-2 focus:ring-[#87ceeb] focus:shadow-lg transition-all',
                  errors.confirmPassword ? 'ring-2 ring-red-500' : ''
                ]"
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
            class="bg-[#87ceeb] text-white text-xl font-medium rounded-2xl py-4 px-12 shadow-lg hover:shadow-xl transition-all"
          >
            Registrarse
          </button>
        </div>
      </form>

      <!-- Verificación de correo -->
      <div v-if="showVerification" class="mt-10 text-center md:col-span-2">
        <h3 class="text-xl font-semibold mb-2">Verifica tu correo electrónico</h3>
        <p class="text-gray-700 mb-4">Ingresa el código de 6 dígitos enviado a tu correo:</p>
        <input
          v-model="verificationCode"
          maxlength="6"
          class="bg-white rounded-full shadow-inner px-4 py-2 outline-none text-gray-700 text-center tracking-widest"
        />
        <button
          @click="verifyCode"
          class="mt-4 bg-[#2d384b] text-white rounded-full px-6 py-2 shadow-lg hover:bg-[#1e293b] transition-all"
        >
          Verificar
        </button>
        <span v-if="verificationError" class="block text-red-500 text-sm mt-2">{{ verificationError }}</span>
      </div>
    </div>
  </div>
</template>



<script>
import axios from 'axios';

export default {
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
        // Address fields
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
    };
  },
  methods: {
    formatCedula(event) {
      let value = event.target.value.replace(/\D/g, '');
      if (value.length > 1) {
        if (value.length <= 5) {
          value = value.slice(0, 1) + '-' + value.slice(1);
        } else {
          value = value.slice(0, 1) + '-' + value.slice(1, 5) + '-' + value.slice(5, 9);
        }
      }
      this.form.cedula = value;
    },
    formatTelefono(event) {
      let value = event.target.value.replace(/\D/g, '');
      if (value.length > 4) {
        value = value.slice(0, 4) + ' ' + value.slice(4, 8);
      }
      this.formattedTelefono = value;
      this.form.telefono = value.replace(/\s/g, ''); // Store as integer
    },
    async submitForm() {
      if (this.validateForm()) {
        try {
          // Prepare data for API
          const employerData = {
            ...this.form,
            cedula: this.form.cedula.replace(/-/g, ''), // Remove dashes from cedula
            telefono: parseInt(this.form.telefono), // Convert to integer
            fechaNacimiento: new Date(this.form.fechaNacimiento).toISOString(),
          };

          console.log('Sending data to API:', employerData);

          const response = await axios.post(
            'http://localhost:5011/api/SignUpEmployer',
            employerData,
            {
              headers: {
                'Content-Type': 'application/json'
              }
            }
          );
          
          console.log('Registro exitoso:', response.data);
          
          if (response.data.message) {
            alert(response.data.message);
            this.$router.push('/login');
          } else {
            this.showVerification = true;
          }
        } catch (error) {
          console.error('Error:', error);
          if (error.response && error.response.data && error.response.data.message) {
            alert(`Error: ${error.response.data.message}`);
          } else {
            alert('Error en el registro. Por favor, intenta de nuevo.');
          }
        }
      }
    },
    validateForm() {
      this.errors = {};
      let isValid = true;

      // Personal Information Validation
      if (!this.form.nombre || this.form.nombre.trim().length === 0) {
        this.errors.nombre = 'El nombre es requerido';
        isValid = false;
      } else if (this.form.nombre.length > 20) {
        this.errors.nombre = 'El nombre no puede exceder 20 caracteres';
        isValid = false;
      }

      if (!this.form.primerApellido || this.form.primerApellido.trim().length === 0) {
        this.errors.primerApellido = 'El primer apellido es requerido';
        isValid = false;
      } else if (this.form.primerApellido.length > 20) {
        this.errors.primerApellido = 'El primer apellido no puede exceder 20 caracteres';
        isValid = false;
      }

      if (this.form.segundoApellido && this.form.segundoApellido.length > 20) {
        this.errors.segundoApellido = 'El segundo apellido no puede exceder 20 caracteres';
        isValid = false;
      }

      if (!this.form.cedula || this.form.cedula.trim().length === 0) {
        this.errors.cedula = 'La cédula es requerida';
        isValid = false;
      } else {
        const cedulaDigits = this.form.cedula.replace(/-/g, '');
        if (cedulaDigits.length !== 9) {
          this.errors.cedula = 'La cédula debe tener 9 dígitos';
          isValid = false;
        }
      }

      if (!this.form.email || this.form.email.trim().length === 0) {
        this.errors.email = 'El correo electrónico es requerido';
        isValid = false;
      } else if (this.form.email.length > 50) {
        this.errors.email = 'El correo no puede exceder 50 caracteres';
        isValid = false;
      } else {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailRegex.test(this.form.email)) {
          this.errors.email = 'Formato de correo electrónico inválido';
          isValid = false;
        }
      }

      if (!this.form.telefono || this.form.telefono.trim().length === 0) {
        this.errors.telefono = 'El teléfono es requerido';
        isValid = false;
      } else {
        const phoneDigits = this.form.telefono.replace(/\s/g, '');
        if (phoneDigits.length !== 8) {
          this.errors.telefono = 'El teléfono debe tener 8 dígitos (#### ####)';
          isValid = false;
        }
      }

      if (!this.form.fechaNacimiento) {
        this.errors.fechaNacimiento = 'La fecha de nacimiento es requerida';
        isValid = false;
      } else {
        const today = new Date();
        const birthDate = new Date(this.form.fechaNacimiento);
        const age = today.getFullYear() - birthDate.getFullYear();
        const monthDiff = today.getMonth() - birthDate.getMonth();
        const dayDiff = today.getDate() - birthDate.getDate();
        
        if (age < 18 || (age === 18 && monthDiff < 0) || (age === 18 && monthDiff === 0 && dayDiff < 0)) {
          this.errors.fechaNacimiento = 'Debe tener al menos 18 años';
          isValid = false;
        } else if (age > 99 || (age === 99 && monthDiff < 0) || (age === 99 && monthDiff === 0 && dayDiff < 0)) {
          this.errors.fechaNacimiento = 'Debe tener menos de 99 años';
          isValid = false;
        }
      }

      // Address Validation
      if (!this.form.provincia || this.form.provincia.trim().length === 0) {
        this.errors.provincia = 'La provincia es requerida';
        isValid = false;
      } else if (this.form.provincia.length > 12) {
        this.errors.provincia = 'La provincia no puede exceder 12 caracteres';
        isValid = false;
      }

      if (!this.form.canton || this.form.canton.trim().length === 0) {
        this.errors.canton = 'El cantón es requerido';
        isValid = false;
      } else if (this.form.canton.length > 30) {
        this.errors.canton = 'El cantón no puede exceder 30 caracteres';
        isValid = false;
      }

      if (!this.form.distrito || this.form.distrito.trim().length === 0) {
        this.errors.distrito = 'El distrito es requerido';
        isValid = false;
      } else if (this.form.distrito.length > 30) {
        this.errors.distrito = 'El distrito no puede exceder 30 caracteres';
        isValid = false;
      }

      if (!this.form.direccionParticular || this.form.direccionParticular.trim().length === 0) {
        this.errors.direccionParticular = 'La dirección particular es requerida';
        isValid = false;
      } else if (this.form.direccionParticular.length > 150) {
        this.errors.direccionParticular = 'La dirección no puede exceder 150 caracteres';
        isValid = false;
      }

      // Password Validation
      if (!this.form.password || this.form.password.trim().length === 0) {
        this.errors.password = 'La contraseña es requerida';
        isValid = false;
      } else if (this.form.password.length < 8) {
        this.errors.password = 'La contraseña debe tener al menos 8 caracteres';
        isValid = false;
      } else if (this.form.password.length > 16) {
        this.errors.password = 'La contraseña no puede exceder 16 caracteres';
        isValid = false;
      }

      if (!this.form.confirmPassword || this.form.confirmPassword.trim().length === 0) {
        this.errors.confirmPassword = 'La confirmación de contraseña es requerida';
        isValid = false;
      } else if (this.form.password !== this.form.confirmPassword) {
        this.errors.confirmPassword = 'Las contraseñas no coinciden';
        isValid = false;
      }

      return isValid;
    },
    verifyCode() {
      if (/^\d{6}$/.test(this.verificationCode)) {
        // TODO: Send code to backend for verification
        // On success, redirect to login
        this.$router.push('/login');
      } else {
        this.verificationError = 'El código debe ser de 6 dígitos.';
      }
    },
  },
};
</script>

<style scoped>
/* Custom neumorphic shadow effects */
.shadow-inner {
  box-shadow: inset 2px 2px 4px rgba(0, 0, 0, 0.1), inset -2px -2px 4px rgba(255, 255, 255, 0.8);
}

.shadow-lg {
  box-shadow: 8px 8px 16px rgba(0, 0, 0, 0.1), -8px -8px 16px rgba(255, 255, 255, 0.8);
}

.shadow-xl {
  box-shadow: 12px 12px 24px rgba(0, 0, 0, 0.15), -12px -12px 24px rgba(255, 255, 255, 0.8);
}

/* Custom focus styles for neumorphic inputs */
input:focus, select:focus {
  box-shadow: inset 2px 2px 4px rgba(0, 0, 0, 0.1), inset -2px -2px 4px rgba(255, 255, 255, 0.8), 0 0 0 2px #87ceeb;
}

/* Custom dropdown arrow positioning */
.relative {
  position: relative;
}

.absolute {
  position: absolute;
}

.inset-y-0 {
  top: 0;
  bottom: 0;
}

.right-0 {
  right: 0;
}

.flex {
  display: flex;
}

.items-center {
  align-items: center;
}

.pr-6 {
  padding-right: 1.5rem;
}

.pointer-events-none {
  pointer-events: none;
}
</style>
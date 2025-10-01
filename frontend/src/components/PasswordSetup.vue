<template>
  <div class="min-h-screen flex items-center justify-center bg-[#E9F7FF] py-12 px-4 sm:px-6 lg:px-8">
    <div class="w-full max-w-xl space-y-8 bg-[#E9F7FF] rounded-[32px] shadow-[8px_8px_16px_#d1e3ee,-8px_-8px_16px_#ffffff] p-10">
      <!-- Header -->
      <div class="text-center">
        <div class="mx-auto h-16 w-16 bg-indigo-600 rounded-full flex items-center justify-center shadow-[4px_4px_8px_#d1e3ee,-4px_-4px_8px_#ffffff]">
          <svg class="h-8 w-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z"></path>
          </svg>
        </div>
        <h2 class="mt-6 text-3xl font-extrabold text-gray-900 py-2 px-4">
          Configurar Contraseña
        </h2>
        <p class="mt-2 text-sm text-gray-600">
          Establece tu contraseña para acceder a la plataforma Imparables
        </p>
      </div>

      <!-- Form -->
      <form class="mt-8 space-y-6" @submit.prevent="handleSubmit">
        <div class="space-y-4">
          <!-- Password Field -->
          <div>
            <label for="password" class="block text-sm font-medium text-gray-700">
              Nueva Contraseña
            </label>
            <div class="mt-1 relative">
              <input
                id="password"
                v-model="formData.password"
                :type="showPassword ? 'text' : 'password'"
                required
                class="appearance-none rounded-[12px] block w-full px-4 py-3 border-0 placeholder-gray-500 text-gray-900 bg-[#E9F7FF] shadow-[inset_2px_2px_4px_#d1e3ee,inset_-2px_-2px_4px_#ffffff] focus:outline-none focus:ring-2 focus:ring-indigo-400 focus:shadow-lg transition-all"
                placeholder="Ingresa tu contraseña"
                :class="{ 'border-red-500': errors.password }"
              />
              <button
                type="button"
                @click="showPassword = !showPassword"
                class="absolute inset-y-0 right-0 pr-3 flex items-center"
              >
                <svg v-if="showPassword" class="h-5 w-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13.875 18.825A10.05 10.05 0 0112 19c-4.478 0-8.268-2.943-9.543-7a9.97 9.97 0 011.563-3.029m5.858.908a3 3 0 114.243 4.243M9.878 9.878l4.242 4.242M9.878 9.878L3 3m6.878 6.878L21 21"></path>
                </svg>
                <svg v-else class="h-5 w-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"></path>
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z"></path>
                </svg>
              </button>
            </div>
            <p v-if="errors.password" class="mt-1 text-sm text-red-600">{{ errors.password }}</p>
            <!-- Password Requirements -->
            <div class="mt-2 text-xs text-gray-600">
              <p class="font-medium mb-1">La contraseña debe contener:</p>
              <ul class="space-y-1">
                <li :class="passwordRequirements.uppercase ? 'text-green-600' : 'text-gray-500'">
                  ✓ Al menos una letra mayúscula
                </li>
                <li :class="passwordRequirements.lowercase ? 'text-green-600' : 'text-gray-500'">
                  ✓ Al menos una letra minúscula
                </li>
                <li :class="passwordRequirements.number ? 'text-green-600' : 'text-gray-500'">
                  ✓ Al menos un número
                </li>
                <li :class="passwordRequirements.special ? 'text-green-600' : 'text-gray-500'">
                  ✓ Al menos un carácter especial (#@$_&()*:;!?)
                </li>
                <li :class="passwordRequirements.length ? 'text-green-600' : 'text-gray-500'">
                  ✓ Entre 8 y 16 caracteres
                </li>
              </ul>
            </div>
          </div>

          <!-- Confirm Password Field -->
          <div>
            <label for="confirmPassword" class="block text-sm font-medium text-gray-700">
              Confirmar Contraseña
            </label>
            <div class="mt-1 relative">
              <input
                id="confirmPassword"
                v-model="formData.confirmPassword"
                :type="showConfirmPassword ? 'text' : 'password'"
                required
                class="appearance-none rounded-[12px] block w-full px-4 py-3 border-0 placeholder-gray-500 text-gray-900 bg-[#E9F7FF] shadow-[inset_2px_2px_4px_#d1e3ee,inset_-2px_-2px_4px_#ffffff] focus:outline-none focus:ring-2 focus:ring-indigo-400 focus:shadow-lg transition-all"
                placeholder="Confirma tu contraseña"
                :class="{ 'border-red-500': errors.confirmPassword }"
              />
              <button
                type="button"
                @click="showConfirmPassword = !showConfirmPassword"
                class="absolute inset-y-0 right-0 pr-3 flex items-center"
              >
                <svg v-if="showConfirmPassword" class="h-5 w-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13.875 18.825A10.05 10.05 0 0112 19c-4.478 0-8.268-2.943-9.543-7a9.97 9.97 0 011.563-3.029m5.858.908a3 3 0 114.243 4.243M9.878 9.878l4.242 4.242M9.878 9.878L3 3m6.878 6.878L21 21"></path>
                </svg>
                <svg v-else class="h-5 w-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"></path>
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z"></path>
                </svg>
              </button>
            </div>
            <p v-if="errors.confirmPassword" class="mt-1 text-sm text-red-600">{{ errors.confirmPassword }}</p>
          </div>
        </div>

        <!-- Submit Button -->
        <div>
          <button
            type="submit"
            :disabled="isLoading"
            class="group relative w-full flex justify-center py-3 px-4 rounded-[12px] font-medium text-white bg-indigo-600 shadow-[4px_4px_8px_#d1e3ee,-4px_-4px_8px_#ffffff] hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50 disabled:cursor-not-allowed transition-all"
          >
            <span v-if="isLoading" class="absolute left-0 inset-y-0 flex items-center pl-3">
              <svg class="animate-spin h-5 w-5 text-white" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
            </span>
            {{ isLoading ? 'Configurando...' : 'Configurar Contraseña' }}
          </button>
        </div>

        <!-- Error Message -->
        <div v-if="errorMessage" class="rounded-[12px] bg-red-50 p-4 shadow-[2px_2px_4px_#d1e3ee,-2px_-2px_4px_#ffffff]">
          <div class="flex">
            <div class="flex-shrink-0">
              <svg class="h-5 w-5 text-red-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
              </svg>
            </div>
            <div class="ml-3">
              <p class="text-sm text-red-800">{{ errorMessage }}</p>
            </div>
          </div>
        </div>

        <!-- Success Message -->
        <div v-if="successMessage" class="rounded-[12px] bg-green-50 p-4 shadow-[2px_2px_4px_#d1e3ee,-2px_-2px_4px_#ffffff]">
          <div class="flex">
            <div class="flex-shrink-0">
              <svg class="h-5 w-5 text-green-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"></path>
              </svg>
            </div>
            <div class="ml-3">
              <p class="text-sm text-green-800">{{ successMessage }}</p>
            </div>
          </div>
        </div>
      </form>

      <!-- Footer -->
      <div class="text-center mt-6">
        <p class="text-xs text-gray-500 shadow-[2px_2px_4px_#d1e3ee,-2px_-2px_4px_#ffffff] rounded-[12px] bg-[#E9F7FF] py-2 px-4">
          ¿Problemas con el enlace? Contacta al administrador del sistema
        </p>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'PasswordSetup',
  data() {
    return {
      formData: {
        password: '',
        confirmPassword: ''
      },
      errors: {},
      isLoading: false,
      errorMessage: '',
      successMessage: '',
      showPassword: false,
      showConfirmPassword: false,
      token: ''
    }
  },
  computed: {
    passwordRequirements() {
      const password = this.formData.password || '';
      return {
        uppercase: /[A-Z]/.test(password),
        lowercase: /[a-z]/.test(password),
        number: /[0-9]/.test(password),
        special: /[#@$_&()*:;!?]/.test(password),
        length: password.length >= 8 && password.length <= 16
      };
    }
  },
  mounted() {
    // Get token from URL parameters
    const urlParams = new URLSearchParams(window.location.search);
    this.token = urlParams.get('token');
    
    if (!this.token) {
      this.errorMessage = 'Token de configuración no válido o faltante.';
    }
  },
  methods: {
    validateForm() {
      this.errors = {};
      let isValid = true;

      // Validate password
      if (!this.formData.password) {
        this.errors.password = 'La contraseña es requerida';
        isValid = false;
      } else {
        const requirements = this.passwordRequirements;
        
        if (!requirements.length) {
          this.errors.password = 'La contraseña debe tener entre 8 y 16 caracteres';
          isValid = false;
        } else if (!requirements.uppercase) {
          this.errors.password = 'La contraseña debe contener al menos una letra mayúscula';
          isValid = false;
        } else if (!requirements.lowercase) {
          this.errors.password = 'La contraseña debe contener al menos una letra minúscula';
          isValid = false;
        } else if (!requirements.number) {
          this.errors.password = 'La contraseña debe contener al menos un número';
          isValid = false;
        } else if (!requirements.special) {
          this.errors.password = 'La contraseña debe contener al menos un carácter especial (#@$_&()*:;!?)';
          isValid = false;
        }
      }

      // Validate confirm password
      if (!this.formData.confirmPassword) {
        this.errors.confirmPassword = 'La confirmación de contraseña es requerida';
        isValid = false;
      } else if (this.formData.password !== this.formData.confirmPassword) {
        this.errors.confirmPassword = 'Las contraseñas no coinciden';
        isValid = false;
      }

      return isValid;
    },
    async handleSubmit() {
      if (!this.validateForm()) {
        return;
      }

      if (!this.token) {
        this.errorMessage = 'Token de configuración no válido.';
        return;
      }

      this.isLoading = true;
      this.errorMessage = '';
      this.successMessage = '';

      try {
        const response = await fetch('http://localhost:5011/api/PasswordSetup/setup', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({
            token: this.token,
            password: this.formData.password,
            confirmPassword: this.formData.confirmPassword
          })
        });

        const result = await response.json();

        if (response.ok && result.success) {
          this.successMessage = 'Contraseña configurada exitosamente. Ya puedes iniciar sesión en la plataforma.';
          this.formData.password = '';
          this.formData.confirmPassword = '';
          
          // Redirect to login after 3 seconds
          setTimeout(() => {
            this.$router.push('/login');
          }, 3000);
        } else {
          this.errorMessage = result.message || 'Error al configurar la contraseña';
        }
      } catch (error) {
        console.error('Error:', error);
        this.errorMessage = 'Error de conexión. Intente nuevamente.';
      } finally {
        this.isLoading = false;
      }
    }
  }
}
</script>

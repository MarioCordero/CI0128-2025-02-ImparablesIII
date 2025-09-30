<!-- filepath: /home/mario/Desktop/UCR/CI0128-2025-02-ImparablesIII/frontend/src/components/CreateProject.vue -->
<template>
  <div class="min-h-screen bg-gray-100 py-8 px-4">
    <div class="max-w-4xl mx-auto">
      <div class="neumorphism-card p-8">
        <!-- Header -->
        <div class="mb-8">
          <h1 class="text-3xl font-bold text-gray-800 mb-2">Registro de Empresa</h1>
          <p class="text-gray-600">Complete los datos de la empresa. Los campos marcados con * son obligatorios</p>
        </div>

        <!-- Loading/Error/Success Messages -->
        <div v-if="loading" class="mb-6 p-4 bg-blue-50 border-l-4 border-blue-400 rounded-r-lg">
          <div class="flex items-center">
            <div class="animate-spin rounded-full h-5 w-5 border-b-2 border-blue-600 mr-3"></div>
            <span class="text-blue-700 font-medium">Registrando empresa...</span>
          </div>
        </div>
        
        <div v-if="errorMessage" class="mb-6 p-4 bg-red-50 border-l-4 border-red-400 rounded-r-lg">
          <div class="flex items-center">
            <svg class="w-5 h-5 text-red-400 mr-3" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd"/>
            </svg>
            <span class="text-red-700 font-medium">{{ errorMessage }}</span>
          </div>
        </div>

        <div v-if="successMessage" class="mb-6 p-4 bg-green-50 border-l-4 border-green-400 rounded-r-lg">
          <div class="flex items-center">
            <svg class="w-5 h-5 text-green-400 mr-3" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"/>
            </svg>
            <span class="text-green-700 font-medium">{{ successMessage }}</span>
          </div>
        </div>

        <form @submit.prevent="handleSubmit" class="space-y-8">
          <!-- Información Básica -->
          <div>
            <h2 class="text-xl font-semibold text-gray-800 mb-4">Información Básica</h2>
            <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">
                  Nombre de la Empresa *
                </label>
                <input
                  v-model="form.Nombre"
                  type="text"
                  placeholder="Ingrese el nombre de la empresa"
                  class="neumorphism-input w-full text-gray-700 placeholder-gray-400"
                  maxlength="20"
                  required
                />
                <p class="text-xs text-gray-500 mt-1">Máximo 20 caracteres</p>
              </div>
              
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">
                  Cédula Jurídica *
                </label>
                <input
                  v-model.number="form.CedulaJuridica"
                  type="number"
                  placeholder="123456789"
                  class="neumorphism-input w-full text-gray-700 placeholder-gray-400"
                  min="100000000"
                  max="999999999"
                  required
                />
                <p class="text-xs text-gray-500 mt-1">Debe tener exactamente 9 dígitos</p>
              </div>
            </div>
          </div>

          <!-- Información de Contacto -->
          <div>
            <h2 class="text-xl font-semibold text-gray-800 mb-4">Información de Contacto</h2>
            <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">
                  Correo Electrónico *
                </label>
                <input
                  v-model="form.Email"
                  type="email"
                  placeholder="empresa@ejemplo.com"
                  class="neumorphism-input w-full text-gray-700 placeholder-gray-400"
                  maxlength="50"
                  required
                />
              </div>
              
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">
                  Teléfono
                </label>
                <input
                  v-model.number="form.Telefono"
                  type="number"
                  placeholder="22001234"
                  class="neumorphism-input w-full text-gray-700 placeholder-gray-400"
                  min="10000000"
                  max="99999999"
                />
                <p class="text-xs text-gray-500 mt-1">8 dígitos (opcional)</p>
              </div>
            </div>
          </div>

          <!-- Configuración de Pagos -->
          <div>
            <h2 class="text-xl font-semibold text-gray-800 mb-4">Configuración de Pagos</h2>
            <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">
                  Período de Pago *
                </label>
                <select
                  v-model="form.PeriodoPago"
                  class="neumorphism-input w-full text-gray-700"
                  required
                >
                  <option value="">Seleccione el período</option>
                  <option value="Mensual">Mensual</option>
                  <option value="Quincenal">Quincenal</option>
                </select>
              </div>
              
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">
                  Provincia *
                </label>
                <select
                  v-model="form.Provincia"
                  class="neumorphism-input w-full text-gray-700"
                  required
                >
                  <option value="">Seleccione una provincia</option>
                  <option value="San José">San José</option>
                  <option value="Alajuela">Alajuela</option>
                  <option value="Cartago">Cartago</option>
                  <option value="Heredia">Heredia</option>
                  <option value="Guanacaste">Guanacaste</option>
                  <option value="Puntarenas">Puntarenas</option>
                  <option value="Limón">Limón</option>
                </select>
              </div>
            </div>
          </div>

          <!-- Dirección Detallada -->
          <div>
            <h2 class="text-xl font-semibold text-gray-800 mb-4">Dirección</h2>
            <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-6">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">
                  Cantón
                </label>
                <input
                  v-model="form.Canton"
                  type="text"
                  placeholder="Nombre del cantón"
                  class="neumorphism-input w-full text-gray-700 placeholder-gray-400"
                  maxlength="30"
                />
              </div>
              
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">
                  Distrito
                </label>
                <input
                  v-model="form.Distrito"
                  type="text"
                  placeholder="Nombre del distrito"
                  class="neumorphism-input w-full text-gray-700 placeholder-gray-400"
                  maxlength="30"
                />
              </div>
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">
                Dirección Particular
              </label>
              <textarea
                v-model="form.DireccionParticular"
                placeholder="Dirección específica de la empresa"
                rows="4"
                class="neumorphism-input w-full text-gray-700 placeholder-gray-400 resize-none"
                maxlength="150"
              ></textarea>
              <p class="text-xs text-gray-500 mt-1 text-right">
                {{ form.DireccionParticular ? form.DireccionParticular.length : 0 }}/150 caracteres
              </p>
            </div>
          </div>

          <!-- Buttons -->
          <div class="flex flex-col sm:flex-row justify-between items-center gap-4 pt-6 border-t border-gray-200">
            <button
              type="button"
              @click="handleCancel"
              class="neumorphism-red px-8 py-3 font-medium rounded-lg w-full sm:w-auto transition-all duration-200"
              :disabled="loading"
            >
              <span class="flex items-center justify-center gap-2">
                <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                  <path fill-rule="evenodd" d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z" clip-rule="evenodd"/>
                </svg>
                Cancelar
              </span>
            </button>
            
            <button
              type="submit"
              class="neumorphism-dark px-8 py-3 font-medium rounded-lg w-full sm:w-auto transition-all duration-200"
              :disabled="loading"
            >
              <span v-if="!loading" class="flex items-center justify-center gap-2">
                <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                  <path d="M13 6a3 3 0 11-6 0 3 3 0 016 0zM18 8a2 2 0 11-4 0 2 2 0 014 0zM14 15a4 4 0 00-8 0v3h8v-3z"/>
                </svg>
                Crear Empresa
              </span>
              <span v-else class="flex items-center justify-center gap-2">
                <div class="animate-spin rounded-full h-4 w-4 border-b-2 border-white"></div>
                Creando...
              </span>
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script>
import axios from 'axios'

export default {
  name: 'CreateProject',
  data() {
    return {
      loading: false,
      errorMessage: '',
      successMessage: '',
      form: {
        Nombre: '',
        CedulaJuridica: null,
        Email: '',
        Telefono: null,
        PeriodoPago: '',
        Provincia: '',
        Canton: '',
        Distrito: '',
        DireccionParticular: ''
      }
    }
  },
  methods: {
    async handleSubmit() {
      this.loading = true;
      this.errorMessage = '';
      this.successMessage = '';

      try {
        // Validar campos requeridos
        if (!this.validateForm()) {
          throw new Error('Por favor complete todos los campos obligatorios');
        }

        // Preparar datos para enviar
        const dataToSend = {
          Nombre: this.form.Nombre.trim(),
          CedulaJuridica: parseInt(this.form.CedulaJuridica),
          Email: this.form.Email.trim(),
          PeriodoPago: this.form.PeriodoPago,
          Provincia: this.form.Provincia,
          Telefono: this.form.Telefono ? parseInt(this.form.Telefono) : null,
          Canton: this.form.Canton || null,
          Distrito: this.form.Distrito || null,
          DireccionParticular: this.form.DireccionParticular || null
        };

        console.log('Enviando datos:', dataToSend);

        const response = await axios.post('http://localhost:5000/api/Project', dataToSend, {
          headers: {
            'Content-Type': 'application/json'
          },
          timeout: 10000 // 10 segundos timeout
        });
        
        this.successMessage = `¡Empresa "${response.data.nombre}" registrada exitosamente!`;
        
        // Scroll to top para mostrar mensaje
        window.scrollTo({ top: 0, behavior: 'smooth' });
        
        // Limpiar formulario después de 3 segundos
        setTimeout(() => {
          this.resetForm();
          // Opcional: redirigir a lista de empresas
          // this.$router.push('/empresas');
        }, 3000);

      } catch (error) {
        console.error('Error creating project:', error);
        
        // Scroll to top para mostrar error
        window.scrollTo({ top: 0, behavior: 'smooth' });
        
        if (error.response?.status === 409) {
          // Conflicto - empresa ya existe
          this.errorMessage = error.response.data.message || 'Ya existe una empresa con estos datos';
        } else if (error.response?.status === 400) {
          // Bad request - error de validación
          if (error.response.data.errors) {
            const errors = Object.values(error.response.data.errors).flat();
            this.errorMessage = errors.join(', ');
          } else {
            this.errorMessage = error.response.data.message || 'Datos inválidos';
          }
        } else if (error.response?.status === 500) {
          this.errorMessage = 'Error interno del servidor. Por favor, intente más tarde.';
        } else if (error.code === 'ECONNABORTED') {
          this.errorMessage = 'Tiempo de espera agotado. Verifique su conexión.';
        } else if (error.message) {
          this.errorMessage = error.message;
        } else {
          this.errorMessage = 'Error al registrar la empresa. Verifique su conexión.';
        }
      } finally {
        this.loading = false;
      }
    },

    validateForm() {
      return this.form.Nombre && 
             this.form.CedulaJuridica && 
             this.form.Email && 
             this.form.PeriodoPago && 
             this.form.Provincia;
    },

    handleCancel() {
      if (this.hasFormData() && !confirm('¿Está seguro que desea cancelar? Los datos no guardados se perderán.')) {
        return;
      }
      this.resetForm();
      this.$router.go(-1);
    },

    resetForm() {
      this.form = {
        Nombre: '',
        CedulaJuridica: null,
        Email: '',
        Telefono: null,
        PeriodoPago: '',
        Provincia: '',
        Canton: '',
        Distrito: '',
        DireccionParticular: ''
      };
      this.errorMessage = '';
      this.successMessage = '';
    },

    hasFormData() {
      return Object.values(this.form).some(value => 
        value !== null && value !== undefined && value !== ''
      );
    }
  }
}
</script>

<style scoped>
/* Custom select arrow for neumorphism style */
select.neumorphism-input {
  background-image: url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' fill='none' viewBox='0 0 20 20'%3e%3cpath stroke='%236b7280' stroke-linecap='round' stroke-linejoin='round' stroke-width='1.5' d='m6 8 4 4 4-4'/%3e%3c/svg%3e");
  background-position: right 12px center;
  background-repeat: no-repeat;
  background-size: 16px 12px;
  padding-right: 40px;
  appearance: none;
}

/* Disabled state for form elements */
.neumorphism-input:disabled,
.neumorphism-dark:disabled,
.neumorphism-red:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

/* Responsive adjustments */
@media (max-width: 640px) {
  .neumorphism-card {
    margin: 16px;
    padding: 24px;
  }
}

/* Animation classes */
.animate-spin {
  animation: spin 1s linear infinite;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}
</style>
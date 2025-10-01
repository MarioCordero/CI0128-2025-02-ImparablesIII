<template>
  <div class="min-h-screen bg-[#E9F7FF] p-0">
    <!-- Header -->
    <header class="grid grid-cols-3 items-center gap-[247px] mb-0 rounded-lg bg-[#E9F7FF] px-[89px] min-h-[95px] max-h-[95px] shadow-[8px_8px_16px_#d1e3ee,-8px_-8px_16px_#ffffff]">
      <!-- Columna 1: Título -->
      <div class="flex items-center">
        <div class="w-[317px] h-auto gap-0">
          <p class="text-[24px] font-bold mb-0 whitespace-nowrap">Registrar Empresa</p>
          <p class="text-gray-600 text-[18px] mb-0 whitespace-nowrap">Agregar nueva empresa al sistema</p>
        </div>
      </div>
      
      <!-- Columna 2: Vacía -->
      <div class="flex justify-center">
      </div>
      
      <!-- Columna 3: Botón volver -->
      <div class="flex justify-end items-center gap-4">
        <button 
          @click="handleCancel"
          class="px-4 py-2 bg-[#E9F7FF] rounded-3 shadow-[4px_4px_8px_#d1e3ee,-4px_-4px_8px_#ffffff] hover:bg-blue-100 transition flex items-center space-x-2"
        >
          <svg class="w-4 h-4" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" d="M10 19l-7-7m0 0l7-7m-7 7h18"/>
          </svg>
          <span>Volver</span>
        </button>
      </div>
    </header>

    <!-- Content -->
    <div class="mx-[171px] my-[41px] space-y-[41px]">
      <!-- Success Message -->
      <div v-if="successMessage" class="bg-green-100 border border-green-400 text-green-700 px-4 py-3 rounded-2xl mb-6">
        <div class="flex items-center">
          <svg class="w-5 h-5 mr-2" fill="currentColor" viewBox="0 0 20 20">
            <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"/>
          </svg>
          <span>{{ successMessage }}</span>
        </div>
      </div>

      <!-- Error Message -->
      <div v-if="errorMessage" class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded-2xl mb-6">
        <div class="flex items-center">
          <svg class="w-5 h-5 mr-2" fill="currentColor" viewBox="0 0 20 20">
            <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd"/>
          </svg>
          <span>{{ errorMessage }}</span>
        </div>
      </div>

      <!-- Form -->
      <div class="bg-[#E9F7FF] shadow-[8px_8px_16px_#d1e3ee,-8px_-8px_16px_#ffffff] p-8 rounded-2xl">
        <form @submit.prevent="handleSubmit" class="space-y-6">
          <!-- Información Básica -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <!-- Nombre de la Empresa -->
            <div>
              <label for="nombre" class="block text-sm font-medium text-gray-700 mb-2">
                Nombre de la Empresa *
              </label>
              <input
                id="nombre"
                v-model="form.Nombre"
                type="text"
                required
                maxlength="20"
                class="w-full px-4 py-3 bg-[#E9F7FF] shadow-[inset_4px_4px_8px_#d1e3ee,inset_-4px_-4px_8px_#ffffff] rounded-lg border-none focus:outline-none focus:ring-2 focus:ring-blue-300"
                placeholder="Ingrese el nombre de la empresa"
              />
            </div>

            <!-- Cédula Jurídica -->
            <div>
              <label for="cedulaJuridica" class="block text-sm font-medium text-gray-700 mb-2">
                Cédula Jurídica *
              </label>
              <input
                id="cedulaJuridica"
                v-model="form.CedulaJuridica"
                type="number"
                required
                min="100000000"
                max="999999999"
                class="w-full px-4 py-3 bg-[#E9F7FF] shadow-[inset_4px_4px_8px_#d1e3ee,inset_-4px_-4px_8px_#ffffff] rounded-lg border-none focus:outline-none focus:ring-2 focus:ring-blue-300"
                placeholder="Ingrese la cédula jurídica (9 dígitos)"
              />
            </div>

            <!-- Email -->
            <div>
              <label for="email" class="block text-sm font-medium text-gray-700 mb-2">
                Correo Electrónico *
              </label>
              <input
                id="email"
                v-model="form.Email"
                type="email"
                required
                maxlength="50"
                class="w-full px-4 py-3 bg-[#E9F7FF] shadow-[inset_4px_4px_8px_#d1e3ee,inset_-4px_-4px_8px_#ffffff] rounded-lg border-none focus:outline-none focus:ring-2 focus:ring-blue-300"
                placeholder="empresa@ejemplo.com"
              />
            </div>

            <!-- Teléfono -->
            <div>
              <label for="telefono" class="block text-sm font-medium text-gray-700 mb-2">
                Teléfono
              </label>
              <input
                id="telefono"
                v-model="form.Telefono"
                type="number"
                min="10000000"
                max="99999999"
                class="w-full px-4 py-3 bg-[#E9F7FF] shadow-[inset_4px_4px_8px_#d1e3ee,inset_-4px_-4px_8px_#ffffff] rounded-lg border-none focus:outline-none focus:ring-2 focus:ring-blue-300"
                placeholder="Ingrese el teléfono (8 dígitos)"
              />
            </div>

            <!-- Período de Pago -->
            <div>
              <label for="periodoPago" class="block text-sm font-medium text-gray-700 mb-2">
                Período de Pago *
              </label>
              <select
                id="periodoPago"
                v-model="form.PeriodoPago"
                required
                class="w-full px-4 py-3 bg-[#E9F7FF] shadow-[inset_4px_4px_8px_#d1e3ee,inset_-4px_-4px_8px_#ffffff] rounded-lg border-none focus:outline-none focus:ring-2 focus:ring-blue-300"
              >
                <option value="">Seleccione período de pago</option>
                <option value="Mensual">Mensual</option>
                <option value="Quincenal">Quincenal</option>
              </select>
            </div>
          </div>

          <!-- Dirección -->
          <div class="mt-8">
            <h3 class="text-lg font-semibold text-gray-700 mb-4">Dirección</h3>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <!-- Provincia -->
              <div>
                <label for="provincia" class="block text-sm font-medium text-gray-700 mb-2">
                  Provincia *
                </label>
                <select
                  id="provincia"
                  v-model="form.Provincia"
                  required
                  class="w-full px-4 py-3 bg-[#E9F7FF] shadow-[inset_4px_4px_8px_#d1e3ee,inset_-4px_-4px_8px_#ffffff] rounded-lg border-none focus:outline-none focus:ring-2 focus:ring-blue-300"
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
              </div>

              <!-- Cantón -->
              <div>
                <label for="canton" class="block text-sm font-medium text-gray-700 mb-2">
                  Cantón
                </label>
                <input
                  id="canton"
                  v-model="form.Canton"
                  type="text"
                  maxlength="30"
                  class="w-full px-4 py-3 bg-[#E9F7FF] shadow-[inset_4px_4px_8px_#d1e3ee,inset_-4px_-4px_8px_#ffffff] rounded-lg border-none focus:outline-none focus:ring-2 focus:ring-blue-300"
                  placeholder="Ingrese el cantón"
                />
              </div>

              <!-- Distrito -->
              <div>
                <label for="distrito" class="block text-sm font-medium text-gray-700 mb-2">
                  Distrito
                </label>
                <input
                  id="distrito"
                  v-model="form.Distrito"
                  type="text"
                  maxlength="30"
                  class="w-full px-4 py-3 bg-[#E9F7FF] shadow-[inset_4px_4px_8px_#d1e3ee,inset_-4px_-4px_8px_#ffffff] rounded-lg border-none focus:outline-none focus:ring-2 focus:ring-blue-300"
                  placeholder="Ingrese el distrito"
                />
              </div>

              <!-- Dirección Particular -->
              <div>
                <label for="direccionParticular" class="block text-sm font-medium text-gray-700 mb-2">
                  Dirección Particular
                </label>
                <input
                  id="direccionParticular"
                  v-model="form.DireccionParticular"
                  type="text"
                  maxlength="150"
                  class="w-full px-4 py-3 bg-[#E9F7FF] shadow-[inset_4px_4px_8px_#d1e3ee,inset_-4px_-4px_8px_#ffffff] rounded-lg border-none focus:outline-none focus:ring-2 focus:ring-blue-300"
                  placeholder="Dirección específica (opcional)"
                />
              </div>
            </div>
          </div>

          <!-- Botones -->
          <div class="flex justify-end space-x-4 pt-6">
            <button
              type="button"
              @click="handleCancel"
              class="px-6 py-3 bg-[#E9F7FF] shadow-[4px_4px_8px_#d1e3ee,-4px_-4px_8px_#ffffff] rounded-lg text-gray-700 hover:bg-gray-100 transition flex items-center space-x-2"
            >
              <svg class="w-4 h-4" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12"/>
              </svg>
              <span>Cancelar</span>
            </button>
            
            <button
              type="submit"
              :disabled="loading"
              class="px-6 py-3 bg-blue-500 shadow-[4px_4px_8px_#d1e3ee,-4px_-4px_8px_#ffffff] rounded-lg text-white hover:bg-blue-600 disabled:opacity-50 transition flex items-center space-x-2"
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

        await axios.post('http://localhost:5011/api/Project', dataToSend, {
          headers: {
            'Content-Type': 'application/json'
          },
          timeout: 10000
        });
        
        this.successMessage = `¡Empresa "${this.form.Nombre}" registrada exitosamente! Redirigiendo al dashboard...`;
        
        // Scroll to top para mostrar mensaje
        window.scrollTo({ top: 0, behavior: 'smooth' });
        
        // Redirigir al dashboard después de 2 segundos
        setTimeout(() => {
          this.$router.push('/dashboard-main-employer');
        }, 2000);

      } catch (error) {
        console.error('Error creating project:', error);
        
        // Scroll to top para mostrar error
        window.scrollTo({ top: 0, behavior: 'smooth' });
        
        if (error.response?.status === 409) {
          this.errorMessage = error.response.data.message || 'Ya existe una empresa con estos datos';
        } else if (error.response?.status === 400) {
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
      this.$router.push('/dashboard-main-employer');
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
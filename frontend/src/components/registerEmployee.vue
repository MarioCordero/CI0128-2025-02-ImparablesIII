<template>
  <div class="min-h-screen flex items-center justify-center bg-[#dbeafe] p-4">
    <div class="bg-[#eaf4fa] rounded-[40px] shadow-2xl p-12 w-full max-w-4xl">
      <!-- Step Navigation -->
      <div class="flex justify-center items-center mb-8">
        <div class="flex items-center space-x-8">
          <!-- Personal Info Step -->
          <div class="flex flex-col items-center">
            <div 
              :class="[
                'w-12 h-12 rounded-full flex items-center justify-center shadow-lg transition-all cursor-pointer',
                currentTab === 0 ? 'bg-[#87ceeb]' : 'bg-gray-300'
              ]"
              @click="goToTab(0)"
            >
              <svg class="w-6 h-6 text-white" fill="currentColor" viewBox="0 0 24 24">
                <path d="M12 12c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm0 2c-2.67 0-8 1.34-8 4v2h16v-2c0-2.66-5.33-4-8-4z"/>
                <circle cx="12" cy="8" r="2"/>
              </svg>
            </div>
            <div 
              :class="[
                'w-8 h-1 rounded-full mt-2',
                currentTab === 0 ? 'bg-[#87ceeb]' : 'bg-gray-300'
              ]"
            ></div>
          </div>

          <!-- Address Step -->
          <div class="flex flex-col items-center">
            <div 
              :class="[
                'w-12 h-12 rounded-full flex items-center justify-center shadow-lg transition-all cursor-pointer',
                currentTab === 1 ? 'bg-[#87ceeb]' : 'bg-gray-300'
              ]"
              @click="goToTab(1)"
            >
              <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z"/>
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z"/>
              </svg>
            </div>
            <div 
              :class="[
                'w-8 h-1 rounded-full mt-2',
                currentTab === 1 ? 'bg-[#87ceeb]' : 'bg-gray-300'
              ]"
            ></div>
          </div>

          <!-- Employment Step -->
          <div class="flex flex-col items-center">
            <div 
              :class="[
                'w-12 h-12 rounded-full flex items-center justify-center shadow-lg transition-all cursor-pointer',
                currentTab === 2 ? 'bg-[#87ceeb]' : 'bg-gray-300'
              ]"
              @click="goToTab(2)"
            >
            <svg width="24" height="24" stroke-width="2" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
            <path d="M8 7H4C2.89543 7 2 7.89543 2 9V19C2 20.1046 2.89543 21 4 21H20C21.1046 21 22 20.1046 22 19V9C22 7.89543 21.1046 7 20 7H16M8 7V3.6C8 3.26863 8.26863 3 8.6 3H15.4C15.7314 3 16 3.26863 16 3.6V7M8 7H16" stroke="white" stroke-width="2"/>
            </svg>
            </div>
            <div 
              :class="[
                'w-8 h-1 rounded-full mt-2',
                currentTab === 2 ? 'bg-[#87ceeb]' : 'bg-gray-300'
              ]"
            ></div>
          </div>
        </div>
      </div>

      <!-- Title -->
      <h2 class="text-2xl font-semibold text-gray-700 text-center mb-8">{{ getCurrentTabTitle() }}</h2>

      <!-- Form -->
      <form @submit.prevent="handleSubmit" class="space-y-6">
        <!-- Tab 1: Personal Information -->
        <div v-if="currentTab === 0" class="space-y-6">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <!-- Left Column -->
            <div class="space-y-6">
              <!-- First Name Field -->
              <div class="relative">
                <div class="space-x-2 ml-2">
                  <p>Primer Nombre</p>
                </div>
                <input
                  v-model="formData.primerNombre"
                  type="text"
                  placeholder="Primer Nombre"
                  class="w-full bg-white rounded-2xl px-6 py-4 text-gray-700 placeholder-gray-400 border-0 shadow-inner focus:outline-none focus:ring-2 focus:ring-[#87ceeb] focus:shadow-lg transition-all"
                  required
                />
              </div>

              
              <div class="relative">
                <div class="space-x-2 ml-2">
                  <p>Apellidos</p>
                </div>
                <input
                  v-model="formData.primerApellido"
                  type="text"
                  placeholder="Apellidos"
                  class="w-full bg-white rounded-2xl px-6 py-4 text-gray-700 placeholder-gray-400 border-0 shadow-inner focus:outline-none focus:ring-2 focus:ring-[#87ceeb] focus:shadow-lg transition-all"
                  required
                />
              </div>

              <!-- Phone Field -->
              <div class="relative">
                <div class="space-x-2 ml-2">
                  <p>Teléfono</p>
                </div>
                <input
                  v-model="formData.telefono"
                  type="tel"
                  placeholder="Teléfono"
                  class="w-full bg-white rounded-2xl px-6 py-4 text-gray-700 placeholder-gray-400 border-0 shadow-inner focus:outline-none focus:ring-2 focus:ring-[#87ceeb] focus:shadow-lg transition-all"
                  required
                />
              </div>

              <!-- Email Field -->
              <div class="relative">
                <div class="space-x-2 ml-2">
                  <p>Correo Electrónico</p>
                </div>
                <input
                  v-model="formData.correo"
                  type="email"
                  placeholder="Correo Electrónico"
                  class="w-full bg-white rounded-2xl px-6 py-4 text-gray-700 placeholder-gray-400 border-0 shadow-inner focus:outline-none focus:ring-2 focus:ring-[#87ceeb] focus:shadow-lg transition-all"
                  required
                />
              </div>
            </div>

            <!-- Right Column -->
            <div class="space-y-6">
              <div class="relative">
                <div class="space-x-2 ml-2">
                  <p>Segundo Nombre</p>
                </div>
                <input
                  v-model="formData.segundoNombre"
                  type="text"
                  placeholder="Segundo Nombre"
                  class="w-full bg-white rounded-2xl px-6 py-4 text-gray-700 placeholder-gray-400 border-0 shadow-inner focus:outline-none focus:ring-2 focus:ring-[#87ceeb] focus:shadow-lg transition-all"
                />
              </div>

              <!-- Date of Birth Field -->
              <div class="relative">
                <div class="space-x-2 ml-2">
                  <p>Fecha de Nacimiento</p>
                </div>
                <input
                  v-model="formData.fechaNacimiento"
                  type="date"
                  placeholder="Fecha de nacimiento"
                  class="w-full bg-white rounded-2xl px-6 py-4 text-gray-700 placeholder-gray-400 border-0 shadow-inner focus:outline-none focus:ring-2 focus:ring-[#87ceeb] focus:shadow-lg transition-all"
                  required
                />
              </div>

              <!-- ID Card Field -->
              <div class="relative">
                <div class="space-x-2 ml-2">
                  <p>Cédula</p>
                </div>
                <input
                  v-model="formData.cedula"
                  type="text"
                  placeholder="Cédula (#-####-####)"
                  class="w-full bg-white rounded-2xl px-6 py-4 text-gray-700 placeholder-gray-400 border-0 shadow-inner focus:outline-none focus:ring-2 focus:ring-[#87ceeb] focus:shadow-lg transition-all"
                  @input="formatCedula"
                  required
                />
              </div>

            </div>
          </div>
        </div>

        <!-- Tab 2: Address Information -->
        <div v-if="currentTab === 1" class="space-y-6">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <!-- Left Column -->
            <div class="space-y-6">
              <!-- Province Field -->
              <div class="relative">
                <div class="space-x-2 ml-2">
                  <p>Provincia</p>
                </div>
                <select
                  v-model="formData.provincia"
                  class="w-full bg-white rounded-2xl px-6 py-4 text-gray-700 border-0 shadow-inner focus:outline-none focus:ring-2 focus:ring-[#87ceeb] focus:shadow-lg transition-all appearance-none cursor-pointer"
                  required
                >
                  <option value="" disabled selected>Provincia</option>
                  <option v-for="provincia in provinciasCostaRica" :key="provincia" :value="provincia">
                    {{ provincia }}
                  </option>
                </select>
                <!-- Custom dropdown arrow -->
                <div class="absolute inset-y-0 right-0 flex items-center pr-6 pointer-events-none">
                  <svg class="w-5 h-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"/>
                  </svg>
                </div>
              </div>

              <!-- Canton Field -->
              <div class="relative">
                <div class="space-x-2 ml-2">
                  <p>Cantón</p>
                </div>
                <input
                  v-model="formData.canton"
                  type="text"
                  placeholder="Cantón"
                  class="w-full bg-white rounded-2xl px-6 py-4 text-gray-700 placeholder-gray-400 border-0 shadow-inner focus:outline-none focus:ring-2 focus:ring-[#87ceeb] focus:shadow-lg transition-all"
                  required
                />
              </div>
            </div>

            <!-- Right Column -->
            <div class="space-y-6">
              <!-- District Field -->
              <div class="relative">
                <div class="space-x-2 ml-2">
                  <p>Distrito</p>
                </div>
                <input
                  v-model="formData.distrito"
                  type="text"
                  placeholder="Distrito"
                  class="w-full bg-white rounded-2xl px-6 py-4 text-gray-700 placeholder-gray-400 border-0 shadow-inner focus:outline-none focus:ring-2 focus:ring-[#87ceeb] focus:shadow-lg transition-all"
                  required
                />
              </div>

              <!-- Address Field -->
              <div class="relative">
                <div class="space-x-2 ml-2">
                  <p>Dirección Particular</p>
                </div>
                <input
                  v-model="formData.direccionParticular"
                  type="text"
                  placeholder="Dirección Particular"
                  class="w-full bg-white rounded-2xl px-6 py-4 text-gray-700 placeholder-gray-400 border-0 shadow-inner focus:outline-none focus:ring-2 focus:ring-[#87ceeb] focus:shadow-lg transition-all"
                  required
                />
              </div>
            </div>
          </div>
        </div>

        <!-- Tab 3: Employment Information -->
        <div v-if="currentTab === 2" class="space-y-6">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <!-- Left Column -->
            <div class="space-y-6">
              <!-- Department Field -->
              <div class="relative">
                <div class="space-x-2 ml-2">
                  <p>Departamento</p>
                </div>
                <input
                  v-model="formData.departamento"
                  type="text"
                  placeholder="Departamento"
                  class="w-full bg-white rounded-2xl px-6 py-4 text-gray-700 placeholder-gray-400 border-0 shadow-inner focus:outline-none focus:ring-2 focus:ring-[#87ceeb] focus:shadow-lg transition-all"
                  required
                />
              </div>

              <!-- Contract Type Field -->
              <div class="relative">
                <div class="space-x-2 ml-2">
                  <p>Tipo de Contrato</p>
                </div>
                  <select
                  v-model="formData.tipoContrato"
                  class="w-full bg-white rounded-2xl px-6 py-4 text-gray-700 border-0 shadow-inner focus:outline-none focus:ring-2 focus:ring-[#87ceeb] focus:shadow-lg transition-all appearance-none cursor-pointer"
                  required
                >
                  <option value="" disabled selected>Tipo de Contrato</option>
                  <option value="Tiempo Completo">Tiempo Completo</option>
                  <option value="Medio Tiempo">Medio Tiempo</option>
                  <option value="Servicios Profesionales">Servicios Profesionales</option>
                </select>
                <!-- Custom dropdown arrow -->
                <div class="absolute inset-y-0 right-0 flex items-center pr-6 pointer-events-none">
                  <svg class="w-5 h-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"/>
                  </svg>
                </div>
              </div>

              <!-- Position Field -->
              <div class="relative">
                <div class="space-x-2 ml-2">
                  <p>Puesto</p>
                </div>
                <input
                  v-model="formData.puesto"
                  type="text"
                  placeholder="Puesto"
                  class="w-full bg-white rounded-2xl px-6 py-4 text-gray-700 placeholder-gray-400 border-0 shadow-inner focus:outline-none focus:ring-2 focus:ring-[#87ceeb] focus:shadow-lg transition-all"
                  required
                />
              </div>
            </div>

            <!-- Right Column -->
            <div class="space-y-6">
              <!-- Salary Field -->
              <div class="relative">
                <div class="space-x-2 ml-2">
                  <p>Salario</p>
                </div>
                <input
                  v-model="formattedSalario"
                  type="text"
                  placeholder="₡0.000"
                  @input="formatSalario"
                  class="w-full bg-white rounded-2xl px-6 py-4 text-gray-700 placeholder-gray-400 border-0 shadow-inner focus:outline-none focus:ring-2 focus:ring-[#87ceeb] focus:shadow-lg transition-all"
                  required
                />
              </div>

              <!-- IBAN Account Number Field -->
              <div class="relative">
                <div class="space-x-2 ml-2">
                  <p>Número de Cuenta IBAN</p>
                </div>
                <input
                  v-model="formData.numeroCuentaIban"
                  type="text"
                  placeholder="Número de Cuenta IBAN"
                  class="w-full bg-white rounded-2xl px-6 py-4 text-gray-700 placeholder-gray-400 border-0 shadow-inner focus:outline-none focus:ring-2 focus:ring-[#87ceeb] focus:shadow-lg transition-all"
                  required
                />
              </div>
            </div>
          </div>
        </div>

        <!-- Navigation Buttons -->
        <div class="flex justify-between mt-8">
          <!-- Previous Button -->
          <button
            v-if="currentTab > 0"
            type="button"
            @click="goToTab(currentTab - 1)"
            class="bg-[#eaf4fa] rounded-2xl px-6 py-3 shadow-lg hover:shadow-xl transition-all flex items-center space-x-2 text-gray-700 font-medium"
          >
            <svg class="w-5 h-5" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" d="M15 19l-7-7 7-7"/>
            </svg>
            <span>Anterior</span>
          </button>

          <!-- Next/Submit Button -->
          <div class="ml-auto">
            <button
              v-if="currentTab < 2"
              type="button"
              @click="goToTab(currentTab + 1)"
              class="bg-[#eaf4fa] rounded-2xl px-6 py-3 shadow-lg hover:shadow-xl transition-all flex items-center space-x-2 text-gray-700 font-medium"
            >
              <span>Siguiente</span>
              <svg class="w-5 h-5" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" d="M9 5l7 7-7 7"/>
              </svg>
            </button>
            <button
              v-else
              type="submit"
              class="bg-[#87ceeb] rounded-2xl px-6 py-3 shadow-lg hover:shadow-xl transition-all flex items-center space-x-2 text-white font-medium"
            >
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
export default {
  name: "RegisterEmployee",
  data() {
    return {
      currentTab: 0,
      formattedSalario: '',
      formData: {
        // Personal Information
        primerNombre: '',
        segundoNombre: '',
        primerApellido: '',
        fechaNacimiento: '',
        cedula: '',
        telefono: '',
        correo: '',
        
        // Address Information
        provincia: '',
        canton: '',
        distrito: '',
        direccionParticular: '',
        
        // Employment Information
        departamento: '',
        tipoContrato: '',
        puesto: '',
        salario: '',
        numeroCuentaIban: ''
      },
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
    goToTab(tabIndex) {
      if (tabIndex >= 0 && tabIndex <= 2) {
        this.currentTab = tabIndex
      }
    },
    getCurrentTabTitle() {
      const titles = [
        'Información Personal',
        'Información de Dirección',
        'Información Laboral'
      ]
      return titles[this.currentTab]
    },
    formatCedula(event) {
      let value = event.target.value.replace(/\D/g, '') // Remove non-digits
      if (value.length > 1) {
        if (value.length <= 5) {
          value = value.slice(0, 1) + '-' + value.slice(1)
        } else {
          value = value.slice(0, 1) + '-' + value.slice(1, 5) + '-' + value.slice(5, 9)
        }
      }
      this.formData.cedula = value
    },
    formatSalario(event) {
      let value = event.target.value.replace(/[^\d]/g, '') // Remove all non-digits
      
      if (value === '') {
        this.formattedSalario = ''
        this.formData.salario = ''
        return
      }
      
      // Convert to number and format with colones
      const number = parseInt(value)
      this.formData.salario = number
      
      // Format with dots as thousands separators
      this.formattedSalario = '₡' + number.toLocaleString('es-CR')
    },
    async handleSubmit() {
      try {
        // Validate required fields
        if (!this.validateForm()) {
          return;
        }

        // Prepare data for API
        const employeeData = {
          ...this.formData,
          cedula: this.formData.cedula.replace(/-/g, ''), // Remove dashes from cedula
          fechaNacimiento: new Date(this.formData.fechaNacimiento).toISOString(),
          numeroCuentaIban: this.formData.numeroCuentaIban
        };

        console.log('Sending data to API:', employeeData);

        const response = await fetch('http://localhost:5011/api/RegisterEmployee', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify(employeeData)
        });

        const result = await response.json();

        if (response.ok) {
          alert('Empleado registrado exitosamente!');
          this.resetForm();
        } else {
          alert(`Error: ${result.message || 'Error al registrar el empleado'}`);
        }
      } catch (error) {
        console.error('Error:', error);
        alert('Error de conexión. Intente nuevamente.');
      }
    },
    validateForm() {
      // Basic validation
      if (!this.formData.primerNombre || !this.formData.primerApellido || 
          !this.formData.fechaNacimiento || !this.formData.cedula || 
          !this.formData.telefono || !this.formData.correo || 
          !this.formData.provincia || !this.formData.canton || 
          !this.formData.distrito || !this.formData.direccionParticular || 
          !this.formData.departamento || !this.formData.tipoContrato || 
          !this.formData.puesto || !this.formData.salario || 
          !this.formData.numeroCuentaIban) {
        alert('Por favor complete todos los campos requeridos.');
        return false;
      }
      return true;
    },
    resetForm() {
      this.formData = {
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
      };
      this.formattedSalario = '';
      this.currentTab = 0;
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
</style>

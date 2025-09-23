<template>
  <div class="min-h-screen flex items-center justify-center bg-[#dbeafe] p-4">
    <div class="bg-[#eaf4fa] rounded-[40px] shadow-2xl p-12 w-full max-w-4xl">
      <!-- Step Navigation -->
      <div class="flex justify-center items-center mb-8">
        <div class="flex items-center space-x-8">
          <!-- Personal Info Step (Active) -->
          <div class="flex flex-col items-center">
            <div class="w-12 h-12 bg-[#87ceeb] rounded-full flex items-center justify-center shadow-lg">
              <svg class="w-6 h-6 text-white" fill="currentColor" viewBox="0 0 24 24">
                <path d="M12 12c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm0 2c-2.67 0-8 1.34-8 4v2h16v-2c0-2.66-5.33-4-8-4z"/>
                <circle cx="12" cy="8" r="2"/>
              </svg>
            </div>
            <div class="w-8 h-1 bg-[#87ceeb] rounded-full mt-2"></div>
          </div>
        </div>
      </div>

      <!-- Title -->
      <h2 class="text-2xl font-semibold text-gray-700 text-center mb-8">Registro de Empleado</h2>

      <!-- Form -->
      <form @submit.prevent="handleSubmit" class="space-y-6">
        <!-- Two Column Grid -->
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <!-- Left Column -->
          <div class="space-y-6">
            <!-- Name Field -->
            <div class="relative">
              <input
                v-model="formData.nombre"
                type="text"
                placeholder="Nombre"
                class="w-full bg-[#eaf4fa] rounded-2xl px-6 py-4 text-gray-700 placeholder-gray-400 border-0 shadow-inner focus:outline-none focus:ring-2 focus:ring-[#87ceeb] focus:shadow-lg transition-all"
                required
              />
            </div>

            <!-- First Last Name Field -->
            <div class="relative">
              <input
                v-model="formData.primerApellido"
                type="text"
                placeholder="Primer Apellido"
                class="w-full bg-[#eaf4fa] rounded-2xl px-6 py-4 text-gray-700 placeholder-gray-400 border-0 shadow-inner focus:outline-none focus:ring-2 focus:ring-[#87ceeb] focus:shadow-lg transition-all"
                required
              />
            </div>

            <!-- Second Last Name Field -->
            <div class="relative">
              <input
                v-model="formData.segundoApellido"
                type="text"
                placeholder="Segundo Apellido"
                class="w-full bg-[#eaf4fa] rounded-2xl px-6 py-4 text-gray-700 placeholder-gray-400 border-0 shadow-inner focus:outline-none focus:ring-2 focus:ring-[#87ceeb] focus:shadow-lg transition-all"
              />
            </div>

            <!-- Date of Birth Field -->
            <div class="relative">
              <input
                v-model="formData.fechaNacimiento"
                type="date"
                placeholder="Fecha de nacimiento"
                class="w-full bg-[#eaf4fa] rounded-2xl px-6 py-4 text-gray-700 placeholder-gray-400 border-0 shadow-inner focus:outline-none focus:ring-2 focus:ring-[#87ceeb] focus:shadow-lg transition-all"
                required
              />
            </div>
          </div>

          <!-- Right Column -->
          <div class="space-y-6">
            <!-- ID Card Field -->
            <div class="relative">
              <input
                v-model="formData.cedula"
                type="text"
                placeholder="Cedula"
                class="w-full bg-[#eaf4fa] rounded-2xl px-6 py-4 text-gray-700 placeholder-gray-400 border-0 shadow-inner focus:outline-none focus:ring-2 focus:ring-[#87ceeb] focus:shadow-lg transition-all"
                required
              />
            </div>

            <!-- Salary Type Field -->
            <div class="relative">
              <select
                v-model="formData.tipoSalario"
                class="w-full bg-[#eaf4fa] rounded-2xl px-6 py-4 text-gray-700 border-0 shadow-inner focus:outline-none focus:ring-2 focus:ring-[#87ceeb] focus:shadow-lg transition-all appearance-none cursor-pointer"
                required
              >
                <option value="" disabled selected>Tipo de Salario</option>
                <option value="mensual">Mensual</option>
                <option value="quincenal">Quincenal</option>
                <option value="por_hora">Por Hora</option>
              </select>
              <!-- Custom dropdown arrow -->
              <div class="absolute inset-y-0 right-0 flex items-center pr-6 pointer-events-none">
                <svg class="w-5 h-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"/>
                </svg>
              </div>
            </div>

            <!-- Salary Field -->
            <div class="relative">
              <input
                v-model="formData.salario"
                type="text"
                placeholder="Salario"
                class="w-full bg-[#eaf4fa] rounded-2xl px-6 py-4 text-gray-700 placeholder-gray-400 border-0 shadow-inner focus:outline-none focus:ring-2 focus:ring-[#87ceeb] focus:shadow-lg transition-all"
                required
              />
            </div>
          </div>
        </div>

        <!-- Next Button -->
        <div class="flex justify-end mt-8">
          <button
            type="submit"
            class="bg-[#eaf4fa] rounded-2xl px-6 py-3 shadow-lg hover:shadow-xl transition-all flex items-center space-x-2 text-gray-700 font-medium"
          >
            <span>Siguiente</span>
            <svg class="w-5 h-5" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" d="M9 5l7 7-7 7"/>
            </svg>
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup>
import { reactive } from 'vue'

// Form data
const formData = reactive({
  nombre: '',
  primerApellido: '',
  segundoApellido: '',
  cedula: '',
  fechaNacimiento: '',
  tipoSalario: '',
  salario: '',
})

// Handle form submission
const handleSubmit = () => {
  console.log('Form submitted:', formData)
  // Here you would typically validate the form and proceed to the next step
  // For now, we'll just log the data
}
</script>

<style scoped>
/* Custom neumorphic shadow effects */
.shadow-inner {
  box-shadow: inset 2px 2px 4px rgba(0, 0, 0, 0.1), inset -2px -2px 4px rgba(255, 255, 255, 0.7);
}

.shadow-lg {
  box-shadow: 8px 8px 16px rgba(0, 0, 0, 0.1), -8px -8px 16px rgba(255, 255, 255, 0.7);
}

.shadow-xl {
  box-shadow: 12px 12px 24px rgba(0, 0, 0, 0.15), -12px -12px 24px rgba(255, 255, 255, 0.7);
}

/* Custom focus styles for neumorphic inputs */
input:focus {
  box-shadow: inset 2px 2px 4px rgba(0, 0, 0, 0.1), inset -2px -2px 4px rgba(255, 255, 255, 0.7), 0 0 0 2px #87ceeb;
}
</style>

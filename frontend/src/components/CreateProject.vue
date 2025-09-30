<template>
  <div class="max-w-4xl mx-auto p-6">
    <div class="neumorphism-card p-8">
      <!-- Header -->
      <div class="mb-8">
        <h1 class="text-2xl font-bold text-gray-800 mb-2">Información de la Empresa</h1>
        <p class="text-sm text-gray-600">Los campos marcados con * son obligatorios</p>
      </div>

      <form @submit.prevent="handleSubmit">
        <!-- First Row -->
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              Nombre de la Empresa *
            </label>
            <input
              v-model="form.companyName"
              type="text"
              placeholder="Ingrese nombre"
              class="neumorphism-input w-full text-gray-700 placeholder-gray-400"
              required
            />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              Cédula Jurídica *
            </label>
            <input
              v-model="form.legalId"
              type="text"
              placeholder="Ingrese cédula"
              class="neumorphism-input w-full text-gray-700 placeholder-gray-400"
              required
            />
            <p class="text-xs text-gray-500 mt-1">Formato: x-xxx-xxx (9 dígitos)</p>
          </div>
        </div>

        <!-- Second Row -->
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              Correo Electrónico *
            </label>
            <input
              v-model="form.email"
              type="email"
              placeholder="Ingrese correo"
              class="neumorphism-input w-full text-gray-700 placeholder-gray-400"
              required
            />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              Teléfono de contacto *
            </label>
            <input
              v-model="form.phone"
              type="tel"
              placeholder="Ingrese teléfono"
              class="neumorphism-input w-full text-gray-700 placeholder-gray-400"
              required
            />
          </div>
        </div>

        <!-- Address -->
        <div class="mb-6">
          <label class="block text-sm font-medium text-gray-700 mb-2">
            Dirección Física *
          </label>
          <textarea
            v-model="form.address"
            placeholder="Ingrese dirección"
            rows="3"
            class="neumorphism-input w-full text-gray-700 placeholder-gray-400 resize-none"
            required
          ></textarea>
          <p class="text-xs text-gray-500 mt-1 text-right">Máximo 200 caracteres</p>
        </div>

        <!-- Third Row -->
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-8">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              Máximo de Beneficios por Empleado *
            </label>
            <input
              v-model="form.maxBenefits"
              type="number"
              placeholder="Ingrese máximo"
              class="neumorphism-input w-full text-gray-700 placeholder-gray-400"
              required
            />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              Período de Pago Predeterminado *
            </label>
            <select
              v-model="form.paymentPeriod"
              class="neumorphism-input w-full text-gray-700"
              required
            >
              <option value="" disabled>Seleccionar período</option>
              <option value="weekly">Semanal</option>
              <option value="biweekly">Quincenal</option>
              <option value="monthly">Mensual</option>
            </select>
          </div>
        </div>

        <!-- Buttons -->
        <div class="flex justify-between items-center">
          <button
            type="button"
            @click="handleCancel"
            class="neumorphism-red px-8 py-3 font-medium"
          >
            Cancelar
          </button>
          <button
            type="submit"
            class="neumorphism-dark px-8 py-3 font-medium flex items-center gap-2"
          >
            <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
              <path d="M13 6a3 3 0 11-6 0 3 3 0 016 0zM18 8a2 2 0 11-4 0 2 2 0 014 0zM14 15a4 4 0 00-8 0v3h8v-3z"/>
            </svg>
            Crear Empresa
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script>
export default {
  name: 'CreateProject',
  data() {
    return {
      form: {
        companyName: '',
        legalId: '',
        email: '',
        phone: '',
        address: '',
        maxBenefits: '',
        paymentPeriod: ''
      }
    }
  },
  methods: {
    handleSubmit() {
      // Handle form submission
      console.log('Form submitted:', this.form)
      // Add your form submission logic here
    },
    handleCancel() {
      // Handle cancel action
      this.$router.go(-1) // Go back to previous page
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
</style>
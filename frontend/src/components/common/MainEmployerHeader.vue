<template>
  <header class="grid grid-cols-3 items-center gap-[120px] mb-0 rounded-lg bg-[#E9F7FF] px-20 min-h-[95px] max-h-[95px] shadow-[8px_8px_16px_#d1e3ee,-8px_-8px_16px_#ffffff] neumorphism-card">
    <!-- Logo & Title -->
    <div class="flex items-center">
      <button
        @click="navigateToHomeLogged"
        class="focus:outline-none flex items-center"
        aria-label="Ir a inicio"
      >
        <img src="../../assets/PlaniFy.svg" alt="PlaniFy Logo" class="h-10 w-full mr-4" />
      </button>

      <!-- TODO: ESTO QUE?   -->
      <div>
        <p class="text-2xl font-bold mb-0 whitespace-nowrap">Panel de Empleador</p>
        <p class="text-gray-600 text-base mb-0 whitespace-nowrap">Gestión de Beneficios Corporativos</p>
      </div>
    </div>

    <!-- Company Selector -->
    <div class="flex justify-center">
      <select
        class="bg-[#E9F7FF] neumorphism-input shadow-[8px_8px_16px_#d1e3ee,-8px_-8px_16px_#ffffff] min-w-[300px] min-h-[40px] rounded px-3 py-2 text-gray-700"
        v-model="selectedCompanyId"
      >
        <option disabled value="">Seleccionar empresa</option>
        <option v-for="company in companies" :key="company.id" :value="company.id">
          {{ company.nombre }}
        </option>
      </select>
    </div>

    <!-- Actions -->
    <div class="flex justify-end items-center gap-4">
      <button
        @click="logout"
        class="neumorphism-dark px-6 py-2 rounded-xl text-white text-base font-semibold hover:bg-blue-700 transition"
      >
        Cerrar Sesión
      </button>
    </div>
  </header>
</template>

<script setup>
// eslint-disable-next-line no-undef
const props = defineProps({
  companies: {
    type: Array,
    default: () => []
  }
})

import { ref, watch } from 'vue'
import { useRouter } from 'vue-router'
import '../../assets/neumorphismGlobal.css'

const router = useRouter()
const selectedCompanyId = ref('')

// Watch for changes and navigate
watch(selectedCompanyId, (newId) => {
  if (newId) {
    router.push({
      name: 'DashboardProject',
      params: { id: newId },
      state: { companies: props.companies }
    })
  }
})

function navigateToHomeLogged() {
  router.push('/dashboard-main-employer')
}

function logout() {
  localStorage.removeItem('user')
  localStorage.removeItem('token')
  router.push('/login')
}
</script>
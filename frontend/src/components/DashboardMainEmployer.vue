<template>
  <div class="min-h-screen bg-[#E9F7FF] p-0">
    <MainEmployerHeader :companies="companies" />

    <div class="mx-[171px] my-[41px] space-y-[41px]">
      <!-- Greeting -->
      <section class="p-0">
        <h1 class="text-2xl font-bold mb-1">¡Hola, {{ user?.nombre || 'Usuario' }}!</h1>
        <p class="text-gray-600 text-[19px] font-medium">Administra los beneficios de tu organización</p>
      </section>

      <!-- Stats & Notifications -->
      <section class="grid md:grid-cols-2 gap-6 mb-6">
        <!-- Rentabilidad -->
        <div class="neumorphism-card p-[30px] rounded-[14px] w-[756px] h-[470px] flex flex-col justify-between bg-[#E9F7FF]">
          <div class="w-[696px] h-[410px] space-y-[26px]">
            <!-- Titulo card -->
            <div class="w-[696px] h-[50px]">
              <p class="font-bold text-[20px] m-0">Rentabilidad</p>
              <p class="text-gray-700 text-[16px] m-0">Observa las ganancias de tus proyectos.</p>
            </div>
            <div>
              <!-- Resumen card -->
              <div class="w-[696px] h-[54px] flex items-center justify-between p-0 mb-[14px]">
                <div class="flex flex-col">
                  <p class="font-bold text-[20px] m-0 whitespace-nowrap">Rentabilidad Total</p>
                  <p class="text-gray-700 text-[16px] m-0">{{ companies.length }} empresas</p>
                </div>
                <div class="flex flex-col items-end">
                  <span class="bg-green-500 text-white rounded px-3 py-1 font-bold text-base block w-fit neumorphism-card">
                    {{ companies.length > 0 ? (companies.reduce((sum, c) => sum + c.CurrentProfitability, 0) / companies.length).toFixed(1) : 0 }}%
                  </span>
                  <span class="text-green-600 text-xs block">
                    {{ companies.length > 0 ? getProfitabilityChange(
                      companies.reduce((sum, c) => sum + c.CurrentProfitability, 0) / companies.length,
                      companies.reduce((sum, c) => sum + c.LastMonthProfitability, 0) / companies.length
                    ) : '+0% vs mes ant.' }}
                  </span>
                </div>
              </div>
              <ul class="space-y-4">
                <li v-for="(company, index) in companies" :key="company.Id" class="flex items-center justify-between">
                  <div class="flex items-center gap-3">
                    <div class="w-[52px] h-[52px] flex items-center justify-center neumorphism-card rounded-[10px] font-bold text-lg">
                      {{ index + 1 }}
                    </div>
                    <div>
                      <span class="font-bold">{{ company.Name }}</span>
                      <div class="text-gray-600 text-sm">{{ company.ActiveEmployees }} empleados</div>
                    </div>
                  </div>
                  <div class="flex flex-col items-end">
                    <span :class="`${getProfitabilityColor(company.CurrentProfitability)} text-white rounded px-3 py-1 font-bold text-base neumorphism-card`">
                      {{ company.CurrentProfitability }}%
                    </span>
                    <span :class="`${getProfitabilityTextColor(company.CurrentProfitability)} text-xs mt-1`">
                      {{ getProfitabilityChange(company.CurrentProfitability, company.LastMonthProfitability) }}
                    </span>
                  </div>
                </li>
              </ul>
            </div>
          </div>
        </div>

        <!-- Notificaciones -->
        <div class="neumorphism-card space-y-[26px] rounded-[14px] p-[30px] w-[756px] h-[470px] bg-[#E9F7FF]">
          <!-- Titulo card -->
          <div class="w-[696px] h-[50px]">
            <p class="font-bold text-[20px] m-0">Notificaciones</p>
            <p class="text-gray-700 text-[16px] m-0">Notificaciones recientes o no atendidas</p>
          </div>
          <!-- Placeholder para feature futuro -->
          <div class="w-[696px] h-[334px] flex flex-col items-center justify-center space-y-4 border-2 border-dashed border-gray-300 rounded-lg">
            <div class="text-center">
              <svg class="w-16 h-16 mx-auto text-gray-400 mb-4" fill="none" stroke="currentColor" stroke-width="1" viewBox="0 0 24 24">
                <path d="M15 17h5l-5 5v-5zM8.5 14.5A2.5 2.5 0 0011 17h-3a2.5 2.5 0 01-2.5-2.5zM15 7a3 3 0 11-6 0 3 3 0 016 0z"/>
                <path d="M13.73 21a2 2 0 01-3.46 0"/>
              </svg>
              <h3 class="text-lg font-semibold text-gray-600 mb-2">Centro de Notificaciones</h3>
              <p class="text-gray-500 text-sm max-w-xs mx-auto">
                Sistema de notificaciones en tiempo real para alertas importantes, 
                recordatorios y actualizaciones del sistema.
              </p>
            </div>
            <div class="bg-yellow-100 text-yellow-800 px-3 py-1 rounded-full text-xs font-medium neumorphism-card">
              ⏳ En desarrollo
            </div>
          </div>
        </div>
      </section>

      <!-- Seccion Mis Empresas -->
      <section>
        <div class="flex justify-between items-center mb-0 bg-[#E9F7FF]">
          <h2 class="text-xl font-bold">Mis Empresas</h2>
          <button @click="navigateToCreateProject" class="flex items-center gap-2 neumorphism-dark px-4 py-2 rounded-3 text-white hover:bg-blue-700 transition">
            Agregar Empresa
          </button>
        </div>
        <div class="w-full h-[10px] my-2 rounded neumorphism-card bg-[#E9F7FF]"></div>
        
        <!-- Loading State -->
        <div v-if="loading" class="flex justify-center items-center py-8">
          <div class="text-gray-600">Cargando empresas...</div>
        </div>
        
        <!-- Error State -->
        <div v-if="error" class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4 neumorphism-card">
          {{ error }}
        </div>
        
        <ProjectList v-if="!loading && !error" :userId="user?.idPersona" />
      </section>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
// import axios from 'axios'
import MainEmployerHeader from './common/MainEmployerHeader.vue'
import '../assets/neumorphismGlobal.css'
import ProjectList from './ProjectList.vue'

// Reactive data
const companies = ref([])
const loading = ref(false)
const error = ref(null)
const router = useRouter()

// const API_BASE_URL = 'http://localhost:5011/api'

const user = ref(null)
onMounted(() => {
  const userRaw = localStorage.getItem('user')
  if (userRaw) {
    try {
      user.value = JSON.parse(userRaw)
    } catch {
      user.value = null
    }
  }
  fetchCompanies()
})

const getProfitabilityColor = (profitability) => {
  if (profitability >= 20) return 'bg-green-500'
  if (profitability >= 15) return 'bg-gray-400'
  return 'bg-red-500'
}

const getProfitabilityTextColor = (profitability) => {
  if (profitability >= 20) return 'text-green-600'
  if (profitability >= 15) return 'text-gray-600'
  return 'text-red-600'
}

const getProfitabilityChange = (current, last) => {
  const change = current - last
  if (change > 0) return `+${change}% vs mes ant.`
  if (change < 0) return `${change}% vs mes ant.`
  return '+0% vs mes ant.'
}

function navigateToCreateProject() {
  router.push('/create-project')
}

async function fetchCompanies() {
  loading.value = true
  error.value = null
  try {
    const response = await fetch('http://localhost:5011/api/Project')
    if (!response.ok) throw new Error('No se pudo cargar las empresas')
    companies.value = await response.json()
  } catch (err) {
    error.value = err.message || 'Error al cargar las empresas'
  } finally {
    loading.value = false
  }
}

</script>
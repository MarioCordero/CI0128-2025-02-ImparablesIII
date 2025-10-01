<template>
  <div class="min-h-screen bg-[#E9F7FF] p-0">
    <!-- Header -->
    <header class="grid grid-cols-3 items-center gap-[247px] mb-0 rounded-lg bg-[#E9F7FF] px-[89px] min-h-[95px] max-h-[95px] shadow-[8px_8px_16px_#d1e3ee,-8px_-8px_16px_#ffffff]">
      <!-- Columna 1: Título -->
      <div class="flex items-center">
        <div class="w-[317px] h-auto gap-0">
          <p class="text-[24px] font-bold mb-0 whitespace-nowrap">Panel de Empleador</p>
          <p class="text-gray-600 text-[18px] mb-0 whitespace-nowrap">Gestion de Beneficios Corporativos</p>
        </div>
      </div>
      <!-- Columna 2: Dropdown centrado -->
      <div class="flex justify-center">
            <select class="bg-[#E9F7FF] shadow-[8px_8px_16px_#d1e3ee,-8px_-8px_16px_#ffffff] min-w-[459px] min-h-[40px] rounded-[4px] px-3 py-1">
                <option default>Seleccionar empresa</option>
                <option v-for="company in companies" :key="company.Id" :value="company.Id">
                  {{ company.Name }}
                </option>
            </select>
      </div>
      <!-- Columna 3: Botones alineados a la derecha -->
      <div class="flex justify-end items-center gap-4">
        <button @click="logout" class="px-4 py-2 bg-[#E9F7FF] rounded-3 shadow-[4px_4px_8px_#d1e3ee,-4px_-4px_8px_#ffffff] hover:bg-blue-100 transition">Cerrar Sesión</button>
      </div>
    </header>
    
    <!-- Contenido -->
    <div class="mx-[171px] my-[41px] space-y-[41px]">
        <!-- Seccion saludo -->
        <!-- Greeting -->
        <section class="p-0">
            <h1 class="text-2xl font-bold mb-1">¡Hola, Cristiano Ronaldo!</h1>
            <p class="text-gray-600 text-[19px] font-medium">Administra los beneficios de tu organización</p>
        </section>


    <!-- Seccion Cartas Medianas -->
          <!-- Seccion Cartas Medianas -->
        <!-- Stats & Notifications -->
        <section class="grid md:grid-cols-2 gap-6 mb-6">
            <!-- Rentabilidad -->
            <div class="bg-[#E9F7FF] shadow-[8px_8px_16px_#d1e3ee,-8px_-8px_16px_#ffffff] p-[30px] rounded-[14px] w-[756px] h-[470px] flex flex-col justify-between">
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
                                <span class="bg-green-500 text-white rounded px-3 py-1 font-bold text-base block w-fit shadow-[2px_2px_4px_#d1e3ee,-2px_-2px_4px_#ffffff]">
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
                                    <div class="w-[52px] h-[52px] flex items-center justify-center bg-[#E9F7FF] rounded-[10px] font-bold text-lg shadow-[4px_4px_8px_#d1e3ee,-4px_-4px_8px_#ffffff]">
                                        {{ index + 1 }}
                                    </div>
                                    <div>
                                        <span class="font-bold">{{ company.Name }}</span>
                                        <div class="text-gray-600 text-sm">{{ company.ActiveEmployees }} empleados</div>
                                    </div>
                                </div>
                                <div class="flex flex-col items-end">
                                    <span :class="`${getProfitabilityColor(company.CurrentProfitability)} text-white rounded px-3 py-1 font-bold text-base shadow-[2px_2px_4px_#d1e3ee,-2px_-2px_4px_#ffffff]`">
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
            <div class="bg-[#E9F7FF] shadow-[8px_8px_16px_#d1e3ee,-8px_-8px_16px_#ffffff] space-y-[26px] rounded-[14px] p-[30px] w-[756px] h-[470px]">
            
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
                    <div class="bg-yellow-100 text-yellow-800 px-3 py-1 rounded-full text-xs font-medium">
                        ⏳ En desarrollo
                    </div>
                </div>
            </div>
        </section>

    <!-- Seccion Mis Empresas -->
      <section>
        <div class="flex justify-between items-center mb-0 bg-[#E9F7FF]">
            <h2 class="text-xl font-bold">Mis Empresas</h2>
            <button @click="navigateToCreateProject" class="flex items-center gap-2 bg-[#E9F7FF] shadow-[4px_4px_8px_#d1e3ee,-4px_-4px_8px_#ffffff] px-4 py-2 rounded-3 text-gray-700 hover:bg-blue-100 transition">
                Agregar Empresa
            </button>
        </div>
        <div class="w-full h-[10px] my-2 rounded shadow-[2px_2px_6px_#d1e3ee,-2px_-2px_6px_#ffffff] bg-[#E9F7FF]"></div>
        
        <!-- Loading State -->
        <div v-if="loading" class="flex justify-center items-center py-8">
          <div class="text-gray-600">Cargando empresas...</div>
        </div>
        
        <!-- Error State -->
        <div v-if="error" class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
          {{ error }}
        </div>
        
        <!-- Companies List -->
        <div v-if="!loading" class="space-y-6">
          <div v-for="company in companies" :key="company.Id">
            <div class="flex items-center justify-between mb-2">
              <h3 class="text-lg font-bold">{{ company.name }}</h3>
              <select class="rounded px-3 py-1 shadow-[2px_2px_4px_#d1e3ee,-2px_-2px_4px_#ffffff]">
                <option>Administrar Empresa</option>
                <option>Detalles Empresa</option>
                <option>Gestionar Beneficios</option>
                <option>Gestionar Empleados</option>
                <option>Reportes de Planilla</option>
                <option>Eliminar</option>
              </select>
            </div>
            <div class="grid md:grid-cols-4 gap-4">
              <!-- Cédula Jurídica -->
              <div class="bg-[#E9F7FF] shadow-[8px_8px_16px_#d1e3ee,-8px_-8px_16px_#ffffff] rounded-[14px] w-[365px] h-[190px] flex items-center justify-center transition-all duration-200 hover:scale-110 hover:bg-[linear-gradient(to_right,#FF0000_4%,#0033FF_74%)] group">
                <div class="w-[303px] h-[128px] flex flex-col justify-between">
                  <div class="flex items-center justify-between mb-2">
                    <p class="font-bold text-[16px] group-hover:text-white transition-colors duration-200">Cédula Jurídica</p>
                    <svg class="w-6 h-6 group-hover:text-white transition-colors duration-200" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
                      <rect x="3" y="7" width="18" height="10" rx="2" stroke="currentColor"/>
                      <path d="M7 15h.01M7 11h.01" stroke="currentColor"/>
                    </svg>
                  </div>
                  <div>
                    <p class="font-bold text-[28px] mb-1 group-hover:text-white transition-colors duration-200">{{ company.legalId }}</p>
                    <p class="text-[15px] group-hover:text-white transition-colors duration-200">de la empresa</p>
                  </div>
                </div>
              </div>
              <!-- Período de Pago -->
              <div class="bg-[#E9F7FF] shadow-[8px_8px_16px_#d1e3ee,-8px_-8px_16px_#ffffff] rounded-[14px] w-[365px] h-[190px] flex items-center justify-center transition-all duration-200 hover:scale-110 hover:bg-[linear-gradient(to_right,#FF0000_4%,#0033FF_74%)] group">
                <div class="w-[303px] h-[128px] flex flex-col justify-between">
                  <div class="flex items-center justify-between mb-2">
                    <p class="font-bold text-[16px] group-hover:text-white transition-colors duration-200">Período de Pago</p>
                    <svg class="w-6 h-6 group-hover:text-white transition-colors duration-200" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
                      <rect x="3" y="5" width="18" height="16" rx="2" stroke="currentColor"/>
                      <path d="M16 3v4M8 3v4" stroke="currentColor"/>
                    </svg>
                  </div>
                  <div>
                    <p class="font-bold text-[28px] mb-1 group-hover:text-white transition-colors duration-200">{{ company.payPeriod }}</p>
                    <p class="text-[15px] group-hover:text-white transition-colors duration-200">{{ formatPeriodDescription(company.payPeriod) }}</p>
                  </div>
                </div>
              </div>
              <!-- Empleados Activos -->
              <div class="bg-[#E9F7FF] shadow-[8px_8px_16px_#d1e3ee,-8px_-8px_16px_#ffffff] rounded-[14px] w-[365px] h-[190px] flex items-center justify-center transition-all duration-200 hover:scale-110 hover:bg-[linear-gradient(to_right,#FF0000_4%,#0033FF_74%)] group">
                <div class="w-[303px] h-[128px] flex flex-col justify-between">
                  <div class="flex items-center justify-between mb-2">
                    <p class="font-bold text-[16px] group-hover:text-white transition-colors duration-200">Empleados Activos</p>
                    <svg class="w-6 h-6 group-hover:text-white transition-colors duration-200" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
                      <circle cx="9" cy="7" r="4" stroke="currentColor"/>
                      <path d="M17 11a4 4 0 1 0-8 0" stroke="currentColor"/>
                      <path d="M17 21v-2a4 4 0 0 0-4-4H7a4 4 0 0 0-4 4v2" stroke="currentColor"/>
                    </svg>
                  </div>
                  <div>
                    <p class="font-bold text-[28px] mb-1 group-hover:text-white transition-colors duration-200">{{ company.activeEmployees }}</p>
                    <p class="text-[15px] group-hover:text-white transition-colors duration-200">en esta empresa</p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'

// Reactive data
const companies = ref([])
const loading = ref(false)
const error = ref(null)

// API base URL - adjust according to your backend configuration
const API_BASE_URL = 'http://localhost:5011/api'

// Fetch companies data
const fetchCompanies = async () => {
  loading.value = true
  error.value = null
  
  try {
    // Get the employer ID from localStorage or wherever it's stored
    const employerId = localStorage.getItem('employerId') || '1' // Default to 1 for testing
    
    const response = await axios.get(`${API_BASE_URL}/project/dashboard/${employerId}`)
    companies.value = response.data || []
    console.log(companies.value)
  } catch (err) {
    console.error('Error fetching companies:', err)
    error.value = 'Error al cargar las empresas'
    // Fallback to static data if API fails
    companies.value = [
      {
        Id: 1,
        Name: 'Imparables III',
        LegalId: '3-123-456789',
        ActiveEmployees: 4,
        PayPeriod: 'Opcional',
        MonthlyPayroll: 4000,
        CurrentProfitability: 12,
        LastMonthProfitability: 19
      },
      {
        Id: 2,
        Name: 'Patitos',
        LegalId: '3-456-123456',
        ActiveEmployees: 369,
        PayPeriod: 'Mensual',
        MonthlyPayroll: 191800000,
        CurrentProfitability: 16,
        LastMonthProfitability: 16
      },
      {
        Id: 3,
        Name: "Jack's Store",
        LegalId: '3-969-696969',
        ActiveEmployees: 2,
        PayPeriod: 'Quincenal',
        MonthlyPayroll: 302300000,
        CurrentProfitability: 24,
        LastMonthProfitability: 9
      }
    ]
  } finally {
    loading.value = false
  }
}



// Format period description
const formatPeriodDescription = (period) => {
  switch (period) {
    case 'Mensual':
      return '28 de cada mes'
    case 'Quincenal':
      return '1 y 16 de cada mes'
    case 'Opcional':
      return 'cuando Mario quiera'
    default:
      return 'No especificado'
  }
}

// Get profitability color class
const getProfitabilityColor = (profitability) => {
  if (profitability >= 20) return 'bg-green-500'
  if (profitability >= 15) return 'bg-gray-400'
  return 'bg-red-500'
}

// Get profitability text color
const getProfitabilityTextColor = (profitability) => {
  if (profitability >= 20) return 'text-green-600'
  if (profitability >= 15) return 'text-gray-600'
  return 'text-red-600'
}

// Calculate profitability change
const getProfitabilityChange = (current, last) => {
  const change = current - last
  if (change > 0) return `+${change}% vs mes ant.`
  if (change < 0) return `${change}% vs mes ant.`
  return '+0% vs mes ant.'
}

// Load data on component mount
onMounted(() => {
  fetchCompanies();
})
</script>
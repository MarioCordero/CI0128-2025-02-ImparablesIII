<template>
  <div class="min-h-screen bg-[#E9F7FF]">
    <MainEmployerHeader :companies="companies" :current-project="project" />
    <DashboardProjectSubHeader />
    <div class="neumorphism-card w-full max-w-5xl p-10 my-20 rounded-[32px] shadow-lg">
      <div class="flex flex-col md:flex-row items-center justify-between mb-8">
        <h1 class="text-4xl font-bold text-gray-800 mb-4 md:mb-0">Dashboard de Empresa</h1>
        <button class="neumorphism-dark px-6 py-3 rounded-lg text-white hover:bg-blue-700 transition" @click="goBack">
          Volver
        </button>
        <button class="neumorphism-dark px-6 py-3 rounded-lg text-white hover:bg-blue-700 transition" @click="addBenefit">
          Agregar beneficio
        </button>
        <button class="neumorphism-dark px-6 py-3 rounded-lg text-white hover:bg-blue-700 transition" @click="addEmployee">
          Agregar empleado
        </button>
      </div>
      <div v-if="loading" class="flex justify-center items-center py-12">
        <div class="animate-spin rounded-full h-12 w-12 neumorphism-dark"></div>
      </div>
      <div v-else-if="error" class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded-2xl mb-6 neumorphism-card">
        <span>{{ error }}</span>
      </div>
      <div v-else>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-8 mb-8">
          <div class="neumorphism-card p-6 rounded-2xl">
            <h2 class="text-xl font-semibold mb-2">Información de la Empresa</h2>
            <p class="text-gray-700"><span class="font-bold">Nombre:</span> {{ project.nombre }}</p>
            <p class="text-gray-700"><span class="font-bold">Cédula Jurídica:</span> {{ project.cedulaJuridica }}</p>
            <p class="text-gray-700"><span class="font-bold">Período de Pago:</span> {{ project.periodoPago }}</p>
            <p class="text-gray-700"><span class="font-bold">Email:</span> {{ project.email }}</p>
            <p class="text-gray-700"><span class="font-bold">Teléfono:</span> {{ project.telefono }}</p>
            <p class="text-gray-700"><span class="font-bold">Dirección:</span> {{ project.direccion || 'N/A' }}</p>
          </div>
          <div class="neumorphism-card p-6 rounded-2xl">
            <h2 class="text-xl font-semibold mb-2">Beneficios Corporativos</h2>
            <div v-if="!project.beneficios || project.beneficios.length === 0" class="text-gray-500">
              No hay beneficios registrados para esta empresa.
            </div>
            <ul v-else class="list-disc pl-5">
              <li v-for="benefit in project.beneficios" :key="`${benefit.idEmpresa}-${benefit.nombre}`" class="mb-2">
                <span class="font-bold">{{ benefit.nombre }}</span> - {{ benefit.tipo }} ({{ benefit.tipoCalculo }})
              </li>
            </ul>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import MainEmployerHeader from './common/MainEmployerHeader.vue'
import DashboardProjectSubHeader from './projectDashboard/DashboardProjectSubHeader.vue'

const route = useRoute()
const router = useRouter()
const project = ref({})
const loading = ref(true)
const error = ref(null)
const companies = ref([])

function goBack() {
  router.push('/dashboard-main-employer')
}

function addBenefit() {
  router.push({
    name: 'AddBenefit',
    params: {
      projectId: project.value.id
    }
  })
}

function addEmployee() {
  router.push({
    name: 'RegisterEmployee',
    params: {
      employerId: project.value.id,
      projectId: project.value.id
    }
  })
}

async function fetchCompanies() {
  try {
    const response = await fetch('http://localhost:5011/api/Project')
    if (!response.ok) throw new Error('No se pudo cargar las empresas')
    companies.value = await response.json()
  } catch (err) {
    // Optionally handle error
  }
}

async function fetchProject() {
  try {
    loading.value = true
    error.value = null
    const id = route.params.id
    const response = await fetch(`http://localhost:5011/api/Project/${id}`)
    if (!response.ok) throw new Error('No se pudo cargar el proyecto')
    const data = await response.json()
    project.value = data
    
    // Fetch benefits for this project
    await fetchBenefits(id)
  } catch (err) {
    error.value = err.message || 'Error al cargar el proyecto'
  } finally {
    loading.value = false
  }
}

async function fetchBenefits(projectId) {
  try {
    const response = await fetch(`http://localhost:5011/api/Benefit/company/${projectId}`)
    if (response.ok) {
      const benefits = await response.json()
      project.value.beneficios = benefits
    }
  } catch (err) {
    console.error('Error fetching benefits:', err)
    // Don't show error for benefits as it's not critical
  }
}

onMounted(() => {
  localStorage.setItem('projectId', route.params.id)
  fetchCompanies()
  fetchProject()
})
</script>
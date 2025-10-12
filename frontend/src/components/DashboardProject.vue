<template>
  <div class="min-h-screen bg-[#E9F7FF]">
    <MainEmployerHeader />
    <div class="neumorphism-card w-full max-w-5xl p-10 my-20 rounded-[32px] shadow-lg">
      <div class="flex flex-col md:flex-row items-center justify-between mb-8">
        <h1 class="text-4xl font-bold text-gray-800 mb-4 md:mb-0">Dashboard de Empresa</h1>
        <button class="neumorphism-dark px-6 py-3 rounded-lg text-white hover:bg-blue-700 transition" @click="goBack">
          Volver
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
            <p class="text-gray-700"><span class="font-bold">Nombre:</span> {{ project.Name }}</p>
            <p class="text-gray-700"><span class="font-bold">Cédula Jurídica:</span> {{ project.LegalId }}</p>
            <p class="text-gray-700"><span class="font-bold">Período de Pago:</span> {{ project.PayPeriod }}</p>
            <p class="text-gray-700"><span class="font-bold">Empleados Activos:</span> {{ project.ActiveEmployees }}</p>
          </div>
          <div class="neumorphism-card p-6 rounded-2xl">
            <h2 class="text-xl font-semibold mb-2">Beneficios Corporativos</h2>
            <div v-if="!project.Benefits || project.Benefits.length === 0" class="text-gray-500">
              No hay beneficios registrados para esta empresa.
            </div>
            <ul v-else class="list-disc pl-5">
              <li v-for="benefit in project.Benefits" :key="benefit.Id" class="mb-2">
                <span class="font-bold">{{ benefit.Name }}</span> - {{ benefit.Type }} ({{ benefit.CalculationType }})
              </li>
            </ul>
          </div>
        </div>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
          <div class="neumorphism-card p-6 rounded-2xl">
            <h2 class="text-xl font-semibold mb-2">Resumen de Planilla</h2>
            <p class="text-gray-700"><span class="font-bold">Próxima Planilla:</span> {{ project.NextPayrollDue || 'N/A' }}</p>
            <p class="text-gray-700"><span class="font-bold">Monto:</span> {{ project.NextPayrollAmount || 'N/A' }}</p>
            <p class="text-gray-700"><span class="font-bold">Empleados:</span> {{ project.NextPayrollEmployees || 'N/A' }}</p>
          </div>
          <div class="neumorphism-card p-6 rounded-2xl">
            <h2 class="text-xl font-semibold mb-2">Notificaciones</h2>
            <ul class="list-disc pl-5">
              <li v-if="project.PendingHours">Horas pendientes: {{ project.PendingHours }}</li>
              <li v-if="project.PendingBenefits">Beneficios por aprobar: {{ project.PendingBenefits }}</li>
              <li v-if="project.ContractsExpiring">Contratos por vencer: {{ project.ContractsExpiring }}</li>
              <li v-if="!project.PendingHours && !project.PendingBenefits && !project.ContractsExpiring" class="text-gray-500">
                No hay notificaciones pendientes.
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

const route = useRoute()
const router = useRouter()
const project = ref({})
const loading = ref(true)
const error = ref(null)

function goBack() {
  router.push('/dashboard-main-employer')
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
  } catch (err) {
    error.value = err.message || 'Error al cargar el proyecto'
  } finally {
    loading.value = false
  }
}

onMounted(fetchProject)
</script>

<style scoped>
@import "@/assets/neumorphismGlobal.css";
</style>
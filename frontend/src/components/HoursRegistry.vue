<template>
  <div class="p-8 bg-gray-50 min-h-screen space-y-8">
    <!-- Header -->
    <div class="flex items-center justify-between">
      <div class="flex items-center gap-2">
        <button class="text-gray-600 hover:text-black text-sm">&larr; Volver al Dashboard</button>
        <h1 class="font-semibold text-lg">Agregar Horas Trabajadas</h1>
      </div>
      <p class="text-sm text-gray-500">Registra tus horas diarias</p>
    </div>

    <!-- Summary Cards -->
    <div class="grid grid-cols-3 gap-4">
      <div class="bg-white rounded-2xl shadow p-4">
        <p class="text-sm text-gray-500">Horas Esta Semana</p>
        <h2 class="text-2xl font-bold">{{ summaries.week.weekHours.toFixed(2) }}h</h2>
        <p class="text-xs text-gray-400">Máximo {{ summaries.week.maxWeeklyHours }} horas semanales</p>
      </div>
      <div class="bg-white rounded-2xl shadow p-4">
        <p class="text-sm text-gray-500">Horas Este Mes</p>
        <h2 class="text-2xl font-bold">{{ summaries.month.monthHours.toFixed(2) }}h</h2>
        <p class="text-xs text-gray-400">Total acumulado del mes</p>
      </div>
      <div class="bg-white rounded-2xl shadow p-4">
        <p class="text-sm text-gray-500">Registros Totales</p>
        <h2 class="text-2xl font-bold">{{ summaries.total.totalRecords }}</h2>
        <p class="text-xs text-gray-400">Días registrados</p>
      </div>
    </div>

    <div class="grid grid-cols-2 gap-8">
      <!-- Registrar Horas -->
      <div class="bg-white rounded-2xl shadow p-6 space-y-4">
        <h3 class="font-semibold text-lg">Registrar Horas</h3>

        <div class="space-y-2">
          <p class="text-sm text-gray-600">Selecciona una fecha y registra las horas de entrada y salida</p>
          <div class="flex gap-2">
            <input type="date" v-model="form.date" class="border rounded px-2 py-1 w-full" />
          </div>
        </div>

        <div class="border-t pt-4 space-y-3">
          <p class="font-medium text-sm text-gray-700">Horarios de Entrada y Salida</p>

          <div v-for="(session, index) in form.sessions" :key="index" class="flex gap-2">
            <input type="time" v-model="session.start" class="border rounded px-2 py-1 flex-1" />
            <input type="time" v-model="session.end" class="border rounded px-2 py-1 flex-1" />
            <button @click="removeSession(index)" class="text-red-500 hover:underline">Eliminar</button>
          </div>

          <button @click="addSession" class="flex items-center gap-1 text-sm text-blue-600 hover:underline">
            + Agregar
          </button>
        </div>

        <button
          @click="registerHours"
          class="w-full bg-black text-white rounded py-2 mt-4 hover:bg-gray-800"
        >
          Registrar Horas
        </button>
      </div>

      <!-- Registros Recientes -->
      <div class="bg-white rounded-2xl shadow p-6 space-y-4">
        <h3 class="font-semibold text-lg">Registros Recientes</h3>
        <div v-if="recentRecords.length === 0" class="text-gray-500 text-sm">Sin registros aún.</div>

        <div
          v-for="record in recentRecords"
          :key="record.date"
          class="flex items-start justify-between border-b py-3"
        >
          <div>
            <p class="font-medium text-gray-800">{{ record.weekday }}, {{ formatDate(record.date) }}</p>
            <span class="inline-block bg-blue-100 text-blue-800 text-xs font-medium px-2 py-1 rounded">
              {{ record.hours.toFixed(2) }} horas
            </span>
            <p class="text-xs text-gray-500 mt-1">
              {{ record.sessions[0].start }} - {{ record.sessions[0].end }} ({{ record.hours.toFixed(2) }}h)
            </p>
          </div>
          <button class="text-red-500 text-sm hover:underline">Eliminar</button>
        </div>
      </div>
    </div>

    <!-- Información Importante -->
    <div class="bg-blue-50 border border-blue-100 rounded-2xl p-4 text-sm text-gray-700 space-y-1">
      <p class="font-semibold flex items-center gap-2">
        <span>ℹ️</span> Información Importante
      </p>
      <ul class="list-disc list-inside text-gray-600 space-y-1">
        <li>Puedes registrar múltiples entradas y salidas por día</li>
        <li>La hora de salida debe ser posterior a la hora de entrada</li>
        <li>No se pueden registrar más de 8 horas por día</li>
        <li>Solo se permiten días laborales (lunes a viernes)</li>
        <li>No se pueden modificar horas de períodos ya pagados</li>
        <li>Los horarios no pueden superponerse</li>
      </ul>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'
import apiConfig from '../config/api.js'

const userId = '1212'

const summaries = ref({
  week: { weekHours: 0, maxWeeklyHours: 40 },
  month: { monthHours: 0 },
  total: { totalRecords: 0 }
})

const recentRecords = ref([])

const form = ref({
  date: '',
  sessions: [{ start: '', end: '' }]
})

function addSession() {
  form.value.sessions.push({ start: '', end: '' })
}

function removeSession(index) {
  form.value.sessions.splice(index, 1)
}

async function registerHours() {
  try {
    const payload = {
      date: form.value.date,
      sessions: form.value.sessions
    }
    await axios.post(apiConfig.endpoints.workHours, payload)
    await loadSummaries()
    await loadRecentRecords()
    form.value.sessions = [{ start: '', end: '' }]
  } catch (err) {
    console.error(err.response?.data || err)
  }
}

async function loadSummaries() {
  const [week, month, total] = await Promise.all([
    axios.get(apiConfig.endpoints.workHoursSummary, { params: { userId, scope: 'week' } }),
    axios.get(apiConfig.endpoints.workHoursSummary, { params: { userId, scope: 'month' } }),
    axios.get(apiConfig.endpoints.workHoursSummary, { params: { userId, scope: 'total' } })
  ])
  summaries.value.week = week.data
  summaries.value.month = month.data
  summaries.value.total = total.data
}

async function loadRecentRecords() {
  const res = await axios.get(apiConfig.endpoints.workHoursRecent, {
    params: { userId, limit: 5 }
  })
  recentRecords.value = res.data.recentRecords
}

function formatDate(dateStr) {
  const date = new Date(dateStr)
  return date.toLocaleDateString('es-CR', {
    day: '2-digit',
    month: 'long',
    year: 'numeric'
  })
}

onMounted(() => {
  loadSummaries()
  loadRecentRecords()
})
</script>

<style scoped>
input,
select {
  font-size: 0.875rem;
}
</style>

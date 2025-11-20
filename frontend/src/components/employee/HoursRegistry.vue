<template>
  <div class="body m-0! p-0!">
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

<script>
import { ref, onMounted } from 'vue'

/*
  MODO MOCK:
  - No hace llamadas a la API.
  - Genera datos de resumen y registros recientes ficticios.
  - El botón "Registrar Horas" agrega el registro al arreglo local y recalcula totales.
  - Cambia MOCK_MODE a false si luego quieres volver a integrar con la API.
*/
const MOCK_MODE = true

export default {
  name: 'HoursRegistry',
  setup() {
    // Estado
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

    // Utilidades de mock
    function randomHourPair() {
      // Genera un par (inicio, fin) entre 7:00 y 17:00 con duración 2-4 h
      const startHour = 7 + Math.floor(Math.random() * 6) // 7-12
      const duration = 2 + Math.floor(Math.random() * 3) // 2-4
      const endHour = Math.min(startHour + duration, 18)
      const pad = v => String(v).padStart(2, '0')
      return {
        start: `${pad(startHour)}:00`,
        end: `${pad(endHour)}:00`
      }
    }

    function calcHours(start, end) {
      const [sh, sm] = start.split(':').map(Number)
      const [eh, em] = end.split(':').map(Number)
      return Math.max(0, (eh + em / 60) - (sh + sm / 60))
    }

    function weekdayName(dateStr) {
      const d = new Date(dateStr)
      return d.toLocaleDateString('es-CR', { weekday: 'long' })
    }

    function formatDate(dateStr) {
      const d = new Date(dateStr)
      return d.toLocaleDateString('es-CR', { day: '2-digit', month: 'long', year: 'numeric' })
    }

    // Mock: cargar resúmenes
    function loadSummariesMock() {
      // Simular horas con límites
      summaries.value.week.weekHours = recentRecords.value
        .slice(0, 5)
        .reduce((acc, r) => acc + r.hours, 0)

      summaries.value.month.monthHours = recentRecords.value
        .reduce((acc, r) => acc + r.hours, 0)

      summaries.value.total.totalRecords = recentRecords.value.length
    }

    // Mock: cargar registros recientes iniciales
    function loadRecentRecordsMock() {
      // Genera entre 3 y 6 días de registros
      const today = new Date()
      const count = 4 + Math.floor(Math.random() * 3)

      recentRecords.value = Array.from({ length: count }).map((_, i) => {
        const d = new Date(today)
        d.setDate(d.getDate() - i)
        const iso = d.toISOString().split('T')[0]

        // Generar 1-2 sesiones
        const sessionCount = 1 + Math.floor(Math.random() * 2)
        const sessions = Array.from({ length: sessionCount }).map(() => randomHourPair())
        const totalHours = sessions.reduce((acc, s) => acc + calcHours(s.start, s.end), 0)

        return {
          date: iso,
          weekday: weekdayName(iso),
            hours: totalHours,
          sessions
        }
      }).sort((a, b) => new Date(b.date) - new Date(a.date))

      loadSummariesMock()
    }

    // Acciones formulario
    function addSession() {
      form.value.sessions.push({ start: '', end: '' })
    }

    function removeSession(index) {
      form.value.sessions.splice(index, 1)
      if (!form.value.sessions.length) {
        form.value.sessions.push({ start: '', end: '' })
      }
    }

    function validateForm() {
      if (!form.value.date) return 'Debe seleccionar una fecha.'
      if (!form.value.sessions.every(s => s.start && s.end)) {
        return 'Complete todas las horas de inicio y fin.'
      }
      const total = form.value.sessions.reduce((acc, s) => acc + calcHours(s.start, s.end), 0)
      if (total <= 0) return 'Las horas totales deben ser mayores a 0.'
      if (total > 12) return 'No se pueden registrar más de 12 horas totales en un día (mock).'
      return null
    }

    function registerHours() {
      if (!MOCK_MODE) return
      const error = validateForm()
      if (error) {
        alert(error)
        return
      }

      const totalHours = form.value.sessions.reduce((acc, s) => acc + calcHours(s.start, s.end), 0)

      // Si ya existe un registro para la fecha, reemplazar
      const existingIndex = recentRecords.value.findIndex(r => r.date === form.value.date)
      const record = {
        date: form.value.date,
        weekday: weekdayName(form.value.date),
        hours: totalHours,
        sessions: JSON.parse(JSON.stringify(form.value.sessions))
      }
      if (existingIndex >= 0) {
        recentRecords.value.splice(existingIndex, 1, record)
      } else {
        recentRecords.value.unshift(record)
      }

      // Limpiar formulario
      form.value.sessions = [{ start: '', end: '' }]
      form.value.date = ''

      loadSummariesMock()
    }

    onMounted(() => {
      loadRecentRecordsMock()
    })

    return {
      summaries,
      recentRecords,
      form,
      addSession,
      removeSession,
      registerHours,
      formatDate
    }
  }
}
</script>

<style scoped>
input,
select {
  font-size: 0.875rem;
}
</style>

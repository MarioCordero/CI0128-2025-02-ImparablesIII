<template>
  <div class="body m-0! p-0!">
    <div class="grid grid-cols-4 gap-7 w-full"> 
      <!-- Card con horas trabajadas esta semana -->
      <div class="neumorphism-card-project flex flex-col justify-between">
        <p class="font-bold text-[16px]">Horas esta semana</p>
        <p class="font-bold text-[28px] mb-1">{{ hoursThisWeek }} horas</p>
        <p class="text-[15px]">trabajadas</p>
      </div>
    
      <!-- Card con horas trabajadas este mes -->
      <div class="neumorphism-card-project flex flex-col justify-between">
        <p class="font-bold text-[16px]">Horas este mes</p>
        <p class="font-bold text-[28px] mb-1">{{ hoursThisMonth }} horas</p>
        <p class="text-[15px]">trabajadas</p>
      </div>
    
      <!-- Card con registros totales -->
      <div class="neumorphism-card-project flex flex-col justify-between">
        <p class="font-bold text-[16px]">Registros totales</p>
        <p class="font-bold text-[28px] mb-1">{{ totalEntries }} registros</p>
        <p class="text-[15px]">en total</p>
      </div>
  
      <!-- Card ultimo registro -->
      <div class="neumorphism-card-project flex flex-col justify-between" v-if="lastEnt">
        <p class="font-bold text-[16px]">Último registro</p>
        <p class="font-bold text-[28px] mb-1">{{ lastEnt.quantity }} horas</p>
        <p class="text-[15px]">{{ formatDate(lastEnt.date) }}</p>
      </div>
      <div class="neumorphism-card-project flex flex-col justify-between" v-else>
        <p class="font-bold text-[16px]">Último registro</p>
        <p class="font-bold text-[28px] mb-1">0 horas</p>
        <p class="text-[15px]">No hay registros</p>
      </div>
    </div>

  
    <div class="grid grid-cols-2 gap-9 w-full">
      <!-- Card que registra horas -->
        <div class="neumorphism-card flex flex-col">
          <h2>Registrar Horas</h2>
          <form @submit.prevent="submitHours" class="flex flex-col flex-1 justify-between gap-4 mt-3">
            <div class="flex flex-col gap-1">
              <label for="date" class="text-[20px]!">Fecha:</label>
              <input type="date" id="date" class="neumorphism-input h-15 px-[20px]! py-[10px]! text-[20px]!" v-model="dateToRegister" required />
            </div>
    
            <div class="flex flex-col gap-1">
              <label for="startHour" class="text-[20px]!">Hora de inicio:</label>
              <input type="time" id="startHour" class="neumorphism-input h-15 px-[20px]! py-[10px]! text-[20px]!" v-model="startHourToRegister" min="00:00" max="23:59" required />
            </div>
    
            <div class="flex flex-col gap-1">
              <label for="endHour" class="text-[20px]!">Hora de fin:</label>
              <input type="time" id="endHour" class="neumorphism-input h-15 px-[20px]! py-[10px]! text-[20px]!" v-model="endHourToRegister" min="00:00" max="23:59" required />
            </div>
            <button type="submit" class="neumorphism-button-normal-blue w-full h-15" :disabled="submitting">
              {{ submitting ? 'Registrando...' : 'Registrar' }}
            </button>
          </form>
        </div>
    
      <!-- Card que muestra los ultimos 6 registros -->
        <div class="neumorphism-card">
          <h2>Últimos Registros</h2>
          <div v-if="recentEntries.length" class="flex flex-col gap-3 mt-3">
            <div
              v-for="entry in recentEntries"
              :key="entry.id"
              class="neumorphism-on-small-item register-item"
              :class="getStatusClass(entry.status)"
            >
              <span><span class="font-semibold">Fecha:</span> {{ formatDate(entry.date) }}</span>
              <span><span class="font-semibold">Horas:</span> {{ entry.quantity }}</span>
              <span><span class="font-semibold">Estado:</span> {{ entry.status }}</span>
            </div>
          </div>
          <p v-else class="text-gray-500">Sin registros recientes.</p>
        </div>
    </div>
  
    <!-- Card con informacion importante -->
      <div class="info-card">
        <h2>Información Importante</h2>
        <div class=" rounded-[24px] border-6 border-dashed border-gray-300 p-4 bg-[#e5f0ff]">
          <p><strong>•</strong>  Recuerda que los registros no pueden ser modificados una vez guardados.</p>
          <p><strong>•</strong>  La hora de salida no puede ser anterior a la hora de entrada.</p>
          <p><strong>•</strong>  Puedes registar un maximo de 8 horas por día.</p>
          <p><strong>•</strong>  Solo seran tomadas en cuenta las horas completas. Si haces minutos extra, no serán contabilizados.</p>
          <p><strong>•</strong>  El formato de hora debe ser {HH}:{MM} {a.m. o p.m.}.</p>
        </div>
      </div>

  </div>
</template>

<script>
import apiConfig from '../../config/api.js'

export default {
  name: 'HoursRegistry',
  props: {
    employeeId: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      hoursThisWeek: 0,
      hoursThisMonth: 0,
      totalEntries: 0,
      lastEnt: null,
      recentEntries: [],
      dateToRegister: '',
      startHourToRegister: '',
      endHourToRegister: '',
      submitting: false,
      loading: false
    }
  },
  watch: {
    employeeId: {
      immediate: true,
      handler(value) {
        if (value) {
          this.fetchHoursSummary()
        }
      }
    }
  },
  methods: {
    async fetchHoursSummary() {
      if (!this.employeeId) {
        return
      }
      this.loading = true
      try {
        const response = await fetch(apiConfig.endpoints.hoursSummary(this.employeeId))
        if (!response.ok) {
          throw new Error('No se pudo obtener el resumen de horas.')
        }
        const data = await response.json()
        this.hoursThisWeek = data.weeklyHours ?? 0
        this.hoursThisMonth = data.monthlyHours ?? 0
        this.totalEntries = data.totalEntries ?? 0
        this.recentEntries = data.recentEntries ?? []
        this.lastEnt = data.lastEntry ?? null
      } catch (error) {
        console.error(error)
        this.$emit('error', 'No se pudo cargar el resumen de horas.')
      } finally {
        this.loading = false
      }
    },
    parseTimeToMinutes(value) {
      if (!value) {
        return null
      }
      const [hours, minutes] = value.split(':').map(Number)
      if (Number.isNaN(hours) || Number.isNaN(minutes)) {
        return null
      }
      return hours * 60 + minutes
    },
    formatDate(value) {
      if (!value) {
        return '—'
      }
      const date = new Date(value)
      if (Number.isNaN(date.getTime())) {
        return value
      }
      return date.toLocaleDateString('es-CR', {
        year: 'numeric',
        month: 'short',
        day: 'numeric'
      })
    },
    resetForm() {
      this.dateToRegister = ''
      this.startHourToRegister = ''
      this.endHourToRegister = ''
    },
    getStatusClass(status) {
      if (!status) {
        return 'status-pending'
      }

      const normalized = status.toLowerCase()
      if (normalized === 'aprobado') {
        return 'status-approved'
      }
      if (normalized === 'rechazado') {
        return 'status-rejected'
      }
      return 'status-pending'
    },
    async submitHours() {
      if (!this.employeeId) {
        this.$emit('error', 'No se pudo identificar al empleado.')
        return
      }

      const startMinutes = this.parseTimeToMinutes(this.startHourToRegister)
      const endMinutes = this.parseTimeToMinutes(this.endHourToRegister)

      if (startMinutes === null || endMinutes === null) {
        this.$emit('error', 'Debes ingresar horas válidas.')
        return
      }

      if (endMinutes <= startMinutes) {
        this.$emit('error', 'La hora de fin debe ser posterior a la hora de inicio.')
        return
      }

      const difference = endMinutes - startMinutes
      const quantity = Math.floor(difference / 60)

      if (quantity <= 0) {
        this.$emit('error', 'Debes registrar al menos 1 hora completa.')
        return
      }

      if (quantity > 8) {
        this.$emit('error', 'Solo puedes registrar un máximo de 8 horas por día.')
        return
      }

      const detail = `Horas registradas de ${this.startHourToRegister} a ${this.endHourToRegister}`

      const payload = {
        employeeId: this.employeeId,
        quantity,
        detail,
        date: this.dateToRegister
      }

      this.submitting = true
      try {
        const response = await fetch(apiConfig.endpoints.hours, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json'
          },
          body: JSON.stringify(payload)
        })

        if (!response.ok) {
          let errorMessage = 'Ocurrió un error al registrar las horas.'
          const contentType = response.headers.get('content-type') || ''

          try {
            if (contentType.includes('application/json')) {
              const errorBody = await response.json()
              errorMessage = errorBody?.message || errorMessage
            } else {
              const rawText = await response.text()
              errorMessage = rawText || errorMessage
            }
          } catch (parseError) {
            console.error('Error parsing response', parseError)
          }

          throw new Error(errorMessage)
        }

        this.$emit('error', null)
        this.resetForm()
        await this.fetchHoursSummary()
      } catch (error) {
        console.error(error)
        this.$emit('error', error.message || 'No se pudo registrar las horas.')
      } finally {
        this.submitting = false
      }
    }
  }
}
</script>

<style scoped>
.register-item {
  width: 100%;
  height: 60px;
  align-items: center;
  box-sizing: border-box;
  padding: 10px 20px;
  font-size: 20px;
  display: grid;
  grid-auto-flow: column;
  grid-auto-columns: 1fr;
  transition: all 0.3s ease;
}

.register-item:hover {
  transform: scale(1.03);
  transition: all 0.3s ease;
}

.status-pending {
  background-color: #f6fedb66;
}

.status-approved {
  background-color: #e1fedb;
}

.status-rejected {
  background-color: #fedbdb;
}
</style>

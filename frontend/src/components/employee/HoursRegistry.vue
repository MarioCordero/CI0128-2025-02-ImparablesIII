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
        <div class="neumorphism-card">
          <h2>Registrar Horas</h2>
          <form @submit.prevent="submitHours">
            <label for="date">Fecha:</label>
            <input type="date" id="date" class="neumorphism-input" v-model="dateToRegister" required />
    
            <label for="startHour">Hora de inicio:</label>
            <input type="time" id="startHour" class="neumorphism-input" v-model="startHourToRegister" min="00:00" max="23:59" required />
    
            <label for="endHour">Hora de fin:</label>
            <input type="time" id="endHour" class="neumorphism-input" v-model="endHourToRegister" min="00:00" max="23:59" required />
            
            <button type="submit" class="neumorphism-button-normal-blue" :disabled="submitting">
              {{ submitting ? 'Registrando...' : 'Registrar' }}
            </button>
          </form>
        </div>
    
      <!-- Card que muestra los ultimos 6 registros -->
        <div class="neumorphism-card">
          <h2>Últimos Registros</h2>
          <ul v-if="recentEntries.length">
            <li v-for="entry in recentEntries" :key="entry.id">
              {{ formatDate(entry.date) }} • {{ entry.quantity }} horas · Estado: {{ entry.status }}
            </li>
          </ul>
          <p v-else class="text-gray-500">Sin registros recientes.</p>
        </div>
    </div>
  
  
  
    <!-- Card con informacion importante -->
      <div class="info-card">
        <h2>Información Importante</h2>
        <ul>
          <li>Recuerda que los registros no pueden ser modificados una vez guardados.</li>
          <li>La hora de salida no puede ser anterior a la hora de entrada.</li>
          <li>Puedes registar un maximo de 8 horas por día.</li>
          <li>Solo seran tomadas en cuenta las horas completas.</li>
        </ul>
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
      if (difference % 60 !== 0) {
        this.$emit('error', 'Solo se permiten horas completas.')
        return
      }

      const quantity = difference / 60
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
          const errorBody = await response.text()
          throw new Error(errorBody || 'Ocurrió un error al registrar las horas.')
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

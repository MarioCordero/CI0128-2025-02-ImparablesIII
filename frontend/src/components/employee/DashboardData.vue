<template>
  <div class="body m-0! p-0!">

    <div v-if="loading" class="flex justify-center items-center py-12">
      <div class="inline-block animate-spin rounded-full h-10 w-10 border-b-2 border-blue-500"></div>
    </div>

    <div v-else class="body m-0! p-0!">
      <div class="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-4 gap-6">
        <div class="neumorphism-card-project">
          <p class="text-sm font-medium">Beneficios seleccionados</p>
          <p class="text-3xl font-bold mt-2">
            {{ benefitsSummary?.currentSelections ?? 0 }} / {{ benefitsSummary?.maxSelections ?? 0 }}
          </p>
          <p class="text-sm mt-1">Disponibles: {{ remainingBenefits }}</p>
        </div>

        <div class="neumorphism-card-project">
          <p class="text-sm font-medium">Horas esta semana</p>
          <p class="text-3xl font-bold mt-2">{{ hoursSummary?.weeklyHours ?? 0 }}</p>
          <p class="text-sm mt-1">Este mes: {{ hoursSummary?.monthlyHours ?? 0 }} h</p>
        </div>

        <div class="neumorphism-card-project">
          <p class="text-sm font-medium">Última planilla</p>
          <p class="text-3xl font-bold mt-2">
            {{ lastPayroll ? formatCurrency(lastPayroll.salarioNeto) : '—' }}
          </p>
          <p class="text-sm mt-1">{{ lastPayrollPeriod }}</p>
        </div>

        <div class="neumorphism-card-project">
          <p class="text-sm font-medium">Último registro de horas</p>
          <p class="text-3xl font-bold mt-2">
            {{ recentHoursEntry ? `${recentHoursEntry.quantity} h` : '—' }}
          </p>
          <p class="text-sm mt-1">{{ recentHoursDate }}</p>
        </div>
      </div>

        <div class="quick-actions">
            <p class="text-sm font-medium text-gray-500 mb-3">Accesos rápidos</p>
            <div class="flex flex-wrap gap-3">
                <button class="neumorphism-button-normal-light" @click="$emit('navigate', 'benefits')">
                Editar beneficios
                </button>
                <button class="neumorphism-button-normal-light" @click="$emit('navigate', 'reports')">
                Última planilla
                </button>
                <button class="neumorphism-button-normal-light" @click="$emit('navigate', 'hours')">
                Nueva entrada de horas
                </button>
            </div>
        </div>

      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <div class="neumorphism-card flex flex-col gap-4">
          <div class="flex items-center justify-between">
            <div>
              <h3 class="text-xl font-semibold text-gray-800">Beneficios seleccionados</h3>
              <p class="text-sm text-gray-500">Últimas elecciones guardadas</p>
            </div>
            <button class="neumorphism-button-normal-light" @click="$emit('navigate', 'benefits')">
              Ver todos
            </button>
          </div>

          <div v-if="selectedBenefitsPreview.length" class="flex flex-col gap-3">
            <div
              v-for="benefit in selectedBenefitsPreview"
              :key="benefit.benefitName"
              class="benefit-pill"
            >
              <div>
                <p class="font-semibold text-gray-800">{{ benefit.benefitName }}</p>
                <p class="text-sm text-gray-500 truncate">{{ benefit.benefitDescription || 'Sin descripción' }}</p>
              </div>
              <span class="benefit-tag">{{ benefit.calculationType }}</span>
            </div>
          </div>
          <p v-else class="text-sm text-gray-500">
            Aún no has seleccionado beneficios. ¡Empieza ahora!
          </p>
        </div>

        <div class="neumorphism-card flex flex-col gap-4">
          <h3 class="text-xl font-semibold text-gray-800">Actividad reciente</h3>
          <div class="activity-item">
            <div>
              <p class="text-sm text-gray-500">Planilla</p>
              <p class="font-semibold text-gray-800">{{ lastPayrollPeriod || 'Sin registros' }}</p>
              <p class="text-sm text-gray-500">{{ lastPayroll ? `Pago neto de ${formatCurrency(lastPayroll.salarioNeto)}` : 'Selecciona un período de planilla para iniciar.' }}</p>
            </div>
            <button
              class="neumorphism-button-normal-blue"
              :disabled="!lastPayroll"
              @click="$emit('navigate', 'reports')"
            >
              Ver reportes
            </button>
          </div>

          <div class="activity-item">
            <div>
              <p class="text-sm text-gray-500">Horas</p>
              <p class="font-semibold text-gray-800">{{ recentHoursDate }}</p>
              <p class="text-sm text-gray-500">{{ recentHoursEntry ? `Registraste ${recentHoursEntry.quantity} horas` : 'Aún no registras horas recientes.' }}</p>
            </div>
            <button class="neumorphism-button-normal-blue" @click="$emit('navigate', 'hours')">
              Registrar horas
            </button>
          </div>

        </div>
    </div>

    </div>

    <div v-if="error" class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded">
      <p class="font-medium">{{ error }}</p>
      <button class="mt-3 neumorphism-button-normal-light" @click="loadDashboardData">
        Reintentar
      </button>
    </div>
  </div>
</template>

<script>
import { apiConfig } from '../../config/api.js'
import {
  CONTENT_TYPES,
  STORAGE_KEYS,
  LOCALE
} from '../../config/const.js'

export default {
  name: 'DashboardData',
  props: {
    employeeId: {
      type: Number,
      required: true
    },
    user: {
      type: Object,
      default: null
    }
  },
  data() {
    return {
      loading: true,
      refreshing: false,
      error: null,
      benefitsSummary: null,
      hoursSummary: null,
      lastPayroll: null,
      recentHoursEntry: null,
      selectedBenefitsPreview: []
    }
  },
  computed: {
    displayName() {
      if (this.user?.nombre) {
        return this.user.nombre.split(' ')[0]
      }
      return 'Empleado'
    },
    remainingBenefits() {
      if (!this.benefitsSummary) {
        return 0
      }
      return Math.max(
        (this.benefitsSummary.maxSelections ?? 0) - (this.benefitsSummary.currentSelections ?? 0),
        0
      )
    },
    lastPayrollPeriod() {
      if (!this.lastPayroll) {
        return 'Sin registros recientes'
      }
      return `${this.getMonthName(this.lastPayroll.month)} ${this.lastPayroll.year}`
    },
    recentHoursDate() {
      if (!this.recentHoursEntry) {
        return 'Sin registros recientes'
      }
      return this.formatDate(this.recentHoursEntry.date)
    }
  },
  watch: {
    employeeId: {
      immediate: true,
      handler(value) {
        if (value) {
          this.loadDashboardData()
        }
      }
    }
  },
  methods: {
    async loadDashboardData() {
      if (!this.employeeId) {
        this.error = 'No se pudo identificar al empleado.'
        this.$emit('error', this.error)
        return
      }

      this.error = null
      this.refreshing = !this.loading

      try {
        const [benefits, hours, payroll] = await Promise.all([
          this.fetchBenefitsSummary(),
          this.fetchHoursSummary(),
          this.fetchLatestPayroll()
        ])

        this.benefitsSummary = benefits
        this.selectedBenefitsPreview = benefits?.selectedBenefits?.slice(0, 3) ?? []
        this.hoursSummary = hours
        this.recentHoursEntry = hours?.lastEntry ?? null
        this.lastPayroll = payroll
      } catch (error) {
        console.error('Error loading dashboard data:', error)
        this.error = error.message || 'No se pudo cargar el resumen.'
        this.$emit('error', this.error)
      } finally {
        this.loading = false
        this.refreshing = false
      }
    },
    async fetchBenefitsSummary() {
      const response = await fetch(apiConfig.endpoints.employeeBenefits(this.employeeId))
      if (!response.ok) {
        throw new Error('No se pudieron cargar los beneficios.')
      }
      return response.json()
    },
    async fetchHoursSummary() {
      const response = await fetch(apiConfig.endpoints.hoursSummary(this.employeeId))
      if (!response.ok) {
        throw new Error('No se pudo obtener el resumen de horas.')
      }
      return response.json()
    },
    async fetchLatestPayroll() {
      const authenticatedEmployeeId = this.getAuthenticatedEmployeeId()
      if (!authenticatedEmployeeId) {
        throw new Error('Usuario no autenticado.')
      }

      const baseUrl = apiConfig.endpoints.employeePayrollReports(this.employeeId)
      const url = `${baseUrl}?authenticatedEmployeeId=${authenticatedEmployeeId}`
      const response = await fetch(url, {
        method: 'GET',
        headers: {
          'Content-Type': CONTENT_TYPES.JSON
        }
      })

      if (!response.ok) {
        throw new Error('No se pudieron obtener las planillas.')
      }

      const data = await response.json()
      if (!Array.isArray(data) || data.length === 0) {
        return null
      }

      const sorted = [...data].sort((a, b) => {
        if (a.year !== b.year) {
          return b.year - a.year
        }
        if (a.month !== b.month) {
          return b.month - a.month
        }
        return b.payrollId - a.payrollId
      })

      return sorted[0]
    },
    getAuthenticatedEmployeeId() {
      const userRaw = localStorage.getItem(STORAGE_KEYS.USER)
      if (!userRaw) {
        return null
      }

      try {
        const parsed = JSON.parse(userRaw)
        return parsed?.idPersona ?? null
      } catch (error) {
        console.error('Error parsing user from storage:', error)
        return null
      }
    },
    formatCurrency(value) {
      if (value === null || value === undefined) {
        return '₡0'
      }
      return new Intl.NumberFormat(LOCALE, {
        style: 'currency',
        currency: 'CRC'
      }).format(value)
    },
    formatDate(dateString) {
      if (!dateString) {
        return '—'
      }
      return new Date(dateString).toLocaleDateString(LOCALE)
    },
    getMonthName(monthNumber) {
      const formatter = new Intl.DateTimeFormat('es-CR', { month: 'long' })
      const mockDate = new Date(2020, (monthNumber || 1) - 1, 1)
      return formatter.format(mockDate)
    }
  }
}
</script>

<style scoped>
.dashboard-data {
  width: 100%;
}

.hero-card {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 24px;
  flex-wrap: wrap;
}

.stat-card {
  background: #dbeafe;
  border-radius: 16px;
  padding: 24px;
  box-shadow: inset 3px 3px 6px #bec8d9,
              inset -3px -3px 6px #ffffff;
}

.benefit-pill {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px;
  border-radius: 16px;
  background: #eef4ff;
  box-shadow: 4px 4px 8px #c7d1e2,
              -4px -4px 8px #ffffff;
}

.benefit-tag {
  font-size: 0.75rem;
  padding: 4px 10px;
  border-radius: 9999px;
  background: #dbeafe;
  color: #1f2937;
}

.activity-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 16px;
  padding: 18px;
  border-radius: 16px;
  background: #eef4ff;
  box-shadow: 4px 4px 8px #c7d1e2,
              -4px -4px 8px #ffffff;
}

.quick-actions {
  padding-top: 16px;
}
</style>

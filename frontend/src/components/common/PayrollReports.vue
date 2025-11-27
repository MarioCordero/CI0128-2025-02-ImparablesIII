<template>
  <div class="payroll-reports-container">
    <h1 class="text-2xl font-bold mb-4">{{ title }}</h1>
    <div class="w-full h-[10px] mt-2 rounded neumorphism-on-small-item"></div>
    <p class="mt-4 text-gray-600 mb-6">{{ description }}</p>
    
    <!-- Report Type Selection (Initial View) -->
    <div v-if="!selectedReportType" class="bg-white rounded-lg shadow-md p-6 mb-6">
      <h2 class="text-lg font-semibold mb-4">Seleccione el tipo de reporte</h2>
      <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <button
          v-for="reportType in availableReportTypes"
          :key="reportType.id"
          @click="selectReportType(reportType)"
          class="w-full md:w-auto px-6 py-3 bg-blue-500 text-white rounded-md hover:bg-blue-600 transition-colors text-left"
        >
          {{ reportType.name }}
        </button>
      </div>
    </div>

    <!-- Report Content (After Report Type Selection) -->
    <div v-else>
      <!-- Back Button -->
      <div class="mb-4">
        <button
          @click="goBackToReportTypeSelection"
          class="px-4 py-2 bg-gray-200 text-gray-700 rounded-md hover:bg-gray-300 transition-colors flex items-center"
        >
          ← Volver a selección de reporte
        </button>
      </div>

      <!-- Current Payroll Report Component (Employee only) -->
      <CurrentPayrollReport
        v-if="selectedReportType.id === 'pago-planilla' && userType === 'employee'"
        :employee-id="employeeId"
        @error="handleError"
      />

      <!-- Historical Payroll Report Component (Employee only) -->
      <HistoricalPayrollReport
        v-if="selectedReportType.id === 'historico-pago-planilla' && userType === 'employee'"
        :employee-id="employeeId"
        @error="handleError"
      />

      <!-- Employer Payroll Reports Component -->
      <EmployerPayrollReports
        v-if="userType === 'employer'"
        @error="handleError"
      />
    </div>
  </div>
</template>

<script>
import CurrentPayrollReport from '../employee/CurrentPayrollReport.vue'
import HistoricalPayrollReport from '../employee/HistoricalPayrollReport.vue'
import EmployerPayrollReports from '../employer/projectDashboard/PayrollPayment.vue'

export default {
  name: 'PayrollReports',
  components: {
    CurrentPayrollReport,
    HistoricalPayrollReport,
    EmployerPayrollReports
  },
  props: {
    employeeId: {
      type: Number,
      required: false,
      default: null
    },
    userType: {
      type: String,
      required: false,
      default: 'employee',
      validator: (value) => ['employee', 'employer'].includes(value)
    }
  },
  data() {
    return {
      selectedReportType: null
    }
  },
  computed: {
    title() {
      return this.userType === 'employer' 
        ? 'Reportes de Planilla' 
        : 'Mis Reportes de Planilla'
    },
    description() {
      return this.userType === 'employer'
        ? 'Visualiza y gestiona los reportes de planilla de la empresa'
        : 'Visualiza y descarga tus recibos de pago'
    },
    availableReportTypes() {
      if (this.userType === 'employer') {
        return [
          {
            id: 'company-payroll-reports',
            name: 'REPORTE DE PLANILLA DE EMPRESA'
          }
        ]
      }
      
      return [
        {
          id: 'pago-planilla',
          name: 'REPORTE DE PAGO PLANILLA'
        },
        {
          id: 'historico-pago-planilla',
          name: 'REPORTE HISTÓRICO DE PAGO PLANILLA'
        }
      ]
    }
  },
  methods: {
    selectReportType(reportType) {
      this.selectedReportType = reportType
    },
    goBackToReportTypeSelection() {
      this.selectedReportType = null
    },
    handleError(error) {
      this.$emit('error', error)
    }
  }
}
</script>


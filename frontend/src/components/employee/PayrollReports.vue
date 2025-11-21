<template>
  <div class="payroll-reports-container">
    <h1 class="text-2xl font-bold mb-4">Mis Reportes de Planilla</h1>
    <p class="text-gray-600 mb-6">Visualiza y descarga tus recibos de pago</p>
    
    <!-- Report Type Selection (Initial View) -->
    <div v-if="!selectedReportType" class="bg-white rounded-lg shadow-md p-6 mb-6">
      <h2 class="text-lg font-semibold mb-4">Seleccione el tipo de reporte</h2>
      <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <button
          v-for="reportType in reportTypes"
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

      <!-- Current Payroll Report Component -->
      <CurrentPayrollReport
        v-if="selectedReportType.id === 'pago-planilla'"
        :employee-id="employeeId"
        @error="handleError"
      />

      <!-- Historical Payroll Report Component -->
      <HistoricalPayrollReport
        v-if="selectedReportType.id === 'historico-pago-planilla'"
        :employee-id="employeeId"
        @error="handleError"
      />
    </div>
  </div>
</template>

<script>
import CurrentPayrollReport from './CurrentPayrollReport.vue'
import HistoricalPayrollReport from './HistoricalPayrollReport.vue'

export default {
  name: 'PayrollReports',
  components: {
    CurrentPayrollReport,
    HistoricalPayrollReport
  },
  props: {
    employeeId: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      selectedReportType: null,
      reportTypes: [
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


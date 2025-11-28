<template>
  <teleport to="body">
    <div
      v-if="show"
      class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50"
      @click.self="$emit('close')"
    >
      <div class="bg-[#dbeafe] rounded-lg shadow-xl max-w-2xl w-full mx-4 max-h-[90vh] overflow-y-auto">
        <div class="p-6">
          <!-- Modal Header -->
          <div class="flex justify-between items-center mb-6">
            <h2 class="text-2xl font-bold text-blue-600">REPORTE DE PAGO PLANILLA</h2>
            <div class="flex items-center gap-3">
              <button
                v-if="detailedReport && !loadingDetail"
                @click="$emit('download-pdf')"
                :disabled="downloadingPdf"
                class="neumorphism-button-normal-green flex items-center gap-2"
              >
                <svg v-if="!downloadingPdf" class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 10v6m0 0l-3-3m3 3l3-3m2 8H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"></path>
                </svg>
                <div v-else class="inline-block animate-spin rounded-full h-5 w-5 border-b-2 border-white"></div>
                {{ downloadingPdf ? 'Descargando...' : 'Descargar PDF' }}
              </button>
              <button
                @click="$emit('close')"
                class="neumorphism-button-normal-light rounded-full! p-3! text-2xl font-bold"
              >
                <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
                </svg>
              </button>
            </div>
          </div>
          <!-- Loading State -->
          <div v-if="loadingDetail" class="text-center py-8">
            <div class="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-blue-500"></div>
            <p class="mt-2 text-gray-600">Cargando detalle del reporte...</p>
          </div>
          <!-- Error State -->
          <div v-else-if="detailError" class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
            {{ detailError }}
          </div>
          <!-- Report Content -->
          <div v-else-if="detailedReport" class="space-y-6">
            <!-- Employee and Company Information -->
            <div class="grid grid-cols-2 gap-4 mb-6">
              <div>
                <p class="text-sm text-blue-600 italic mb-1">Nombre de la empresa</p>
                <p v-if="loadingCompanyName" class="text-base font-medium text-gray-400">Cargando...</p>
                <p v-else class="text-base font-medium">{{ companyName || 'No disponible' }}</p>
              </div>
              <div>
                <p class="text-sm text-blue-600 italic mb-1">Nombre completo del empleado</p>
                <p class="text-base font-medium">{{ employeeName || 'No disponible' }}</p>
              </div>
              <div>
                <p class="text-sm text-blue-600 italic mb-1">Fecha de pago</p>
                <p class="text-base font-medium">{{ formatDate(detailedReport.fechaGeneracion) }}</p>
              </div>
              <div>
                <p class="text-sm text-blue-600 italic mb-1">Tipo de contrato</p>
                <p class="text-base font-medium">{{ detailedReport.tipoContrato }}</p>
              </div>
            </div>
            <!-- Financial Breakdown -->
            <div class="space-y-4">
              <!-- Gross Salary -->
              <div class="flex justify-between items-center py-2 border-b">
                <span class="font-bold text-base">Salario Bruto</span>
                <span class="font-bold text-base">₡{{ formatCurrency(detailedReport.salarioBruto) }}</span>
              </div>
              <!-- Obligatory Deductions -->
              <div class="mt-4">
                <h3 class="font-semibold text-base mb-3">Deducciones obligatorias</h3>
                <div class="space-y-2 ml-4">
                  <div
                    v-for="deduction in detailedReport.deduccionesObligatorias"
                    :key="deduction.nombre"
                    class="flex justify-between items-center"
                  >
                    <span class="text-sm">{{ deduction.nombre }}</span>
                    <span class="text-sm">-₡{{ formatCurrency(deduction.monto) }}</span>
                  </div>
                  <div class="flex justify-between items-center pt-2 border-t font-bold">
                    <span>Total deducciones obligatorias</span>
                    <span>-₡{{ formatCurrency(detailedReport.totalDeduccionesObligatorias) }}</span>
                  </div>
                </div>
              </div>
              <!-- Voluntary Deductions -->
              <div class="mt-4">
                <h3 class="font-semibold text-base mb-3">Deducciones voluntarias</h3>
                <div class="space-y-2 ml-4">
                  <div
                    v-for="deduction in detailedReport.deduccionesVoluntarias"
                    :key="deduction.nombre"
                    class="flex justify-between items-center"
                  >
                    <span class="text-sm">{{ deduction.nombre }}</span>
                    <span class="text-sm">-₡{{ formatCurrency(deduction.monto) }}</span>
                  </div>
                  <div class="flex justify-between items-center pt-2 border-t font-bold">
                    <span>Total deducciones voluntarias</span>
                    <span>-₡{{ formatCurrency(detailedReport.totalDeduccionesVoluntarias) }}</span>
                  </div>
                </div>
              </div>
              <!-- Net Pay -->
              <div class="flex justify-between items-center py-2 border-t-2 border-gray-300 mt-4">
                <span class="font-bold text-lg">Pago Neto</span>
                <span class="font-bold text-lg">₡{{ formatCurrency(detailedReport.salarioNeto) }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </teleport>
</template>

<script>
export default {
  name: 'PayrollReportDetailModal',
  props: {
    show: Boolean,
    detailedReport: Object,
    loadingDetail: Boolean,
    detailError: String,
    downloadingPdf: Boolean,
    companyName: String,
    employeeName: String,
    loadingCompanyName: Boolean,
    formatCurrency: Function,
    formatDate: Function
  }
}
</script>
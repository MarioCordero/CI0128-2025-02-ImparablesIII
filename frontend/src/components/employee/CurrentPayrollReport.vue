<template>
  <div class="body p-0! m-0!">
    <!-- Filters Section -->
    <div>
      <h2 class="text-lg font-semibold mb-4">Seleccione el periodo</h2>
      <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">Reporte</label>
          <select
            v-model="selectedReportId"
            class="neumorphism-input neumorphism-input-select"
            @change="applyReportFilter"
          >
            <option :value="null">Últimos 10 reportes</option>
            <option
              v-for="report in allReports"
              :key="report.payrollId"
              :value="report.payrollId"
            >
              {{ formatPeriod(report) }}
            </option>
          </select>
        </div>
        <div class="flex items-end">
          <button
            @click="clearFilters"
            class="neumorphism-button-normal-light"
          >
            Limpiar Filtros
          </button>
        </div>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="text-center py-8">
      <div class="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-blue-500"></div>
      <p class="mt-2 text-gray-600">Cargando reportes...</p>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
      {{ error }}
    </div>

    <!-- Empty State -->
    <div v-else-if="filteredReports.length === 0" class="bg-gray-100 border border-gray-300 rounded-lg p-8 text-center">
      <p class="text-gray-600 text-lg">No se encontraron reportes para los filtros seleccionados</p>
    </div>

    <!-- Reports Table -->
    <div v-else class="neumorphism-card p-0">
      <div class="neumorphism-table-wrapper overflow-x-auto">
        <table class="neumorphism-table min-w-full divide-y divide-gray-200">
          <thead class="bg-gray-50">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Período
            </th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Posición
            </th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Salario Bruto
            </th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Deducciones
            </th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Beneficios
            </th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Salario Neto
            </th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Acciones
            </th>
          </tr>
        </thead>
          <tbody v-if="filteredReports.length > 0" class="bg-white divide-y divide-gray-200">
          <tr v-for="report in filteredReports" :key="report.payrollId" class="hover:bg-gray-50">
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
              {{ formatPeriod(report) }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
              {{ report.puesto }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
              ₡{{ formatCurrency(report.salarioBruto) }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
              ₡{{ formatCurrency(report.deduccionesEmpleado) }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
              ₡{{ formatCurrency(report.totalBeneficios) }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm font-semibold text-gray-900">
              ₡{{ formatCurrency(report.salarioNeto) }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm">
              <button
                @click="viewReportDetail(report)"
                class="neumorphism-button-normal-blue"
              >
                Ver Detalle
              </button>
            </td>
          </tr>
          </tbody>
        </table>
      </div>
    </div>

    <teleport to="body">
      <!-- Modal for Detailed Report -->
      <div
        v-if="showModal"
        class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50"
        @click.self="closeModal"
      >
        <div class="bg-[#dbeafe] rounded-lg shadow-xl max-w-2xl w-full mx-4 max-h-[90vh] overflow-y-auto">
          <div class="p-6">
            <!-- Modal Header -->
            <div class="flex justify-between items-center mb-6">
              <h2 class="text-2xl font-bold text-blue-600">REPORTE DE PAGO PLANILLA</h2>
              <div class="flex items-center gap-3">
                <button
                  v-if="detailedReport && !loadingDetail"
                  @click="downloadPdfReport"
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
                  @click="closeModal"
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

  </div>
</template>

<script>
import { apiConfig } from '../../config/api.js'
import {
  HTTP_STATUS,
  STORAGE_KEYS,
  CONTENT_TYPES,
  LOCALE,
  ERROR_MESSAGES,
  MONTHS
} from '../../config/const.js'

export default {
  name: 'CurrentPayrollReport',
  props: {
    employeeId: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      allReports: [],
      filteredReports: [],
      loading: false,
      error: null,
      selectedReportId: null,
      months: MONTHS,
      showModal: false,
      detailedReport: null,
      loadingDetail: false,
      detailError: null,
      currentPayrollId: null,
      downloadingPdf: false,
      employeeName: '',
      companyName: '',
      loadingCompanyName: false
    }
  },
  computed: {
    lastTenReports() {
      return this.allReports.slice(0, 10)
    }
  },
  mounted() {
    this.loadUserDataFromLocalStorage()
    this.fetchReports()
  },
  methods: {
    async fetchReports() {
      if (!this.validateEmployeeId()) {
        return
      }

      this.setLoadingState(true)

      try {
        const authenticatedEmployeeId = this.getAuthenticatedEmployeeId()
        const url = this.buildApiUrl(authenticatedEmployeeId)
        const response = await this.performApiRequest(url)
        const data = await this.parseResponse(response)
        this.updateReportsData(data)
      } catch (error) {
        this.handleFetchError(error)
      } finally {
        this.setLoadingState(false)
      }
    },
    validateEmployeeId() {
      if (!this.employeeId) {
        this.error = ERROR_MESSAGES.INVALID_EMPLOYEE_ID
        return false
      }
      return true
    },
    getAuthenticatedEmployeeId() {
      const userRaw = localStorage.getItem(STORAGE_KEYS.USER)
      if (!userRaw) {
        throw new Error(ERROR_MESSAGES.UNAUTHENTICATED_USER)
      }

      const user = JSON.parse(userRaw)
      if (!user || !user.idPersona) {
        throw new Error(ERROR_MESSAGES.UNAUTHENTICATED_USER)
      }

      return user.idPersona
    },
    buildQueryParams(authenticatedEmployeeId) {
      const params = new URLSearchParams({
        authenticatedEmployeeId: authenticatedEmployeeId.toString()
      })

      return params
    },
    buildApiUrl(authenticatedEmployeeId) {
      const baseUrl = apiConfig.endpoints.employeePayrollReports(this.employeeId)
      const params = this.buildQueryParams(authenticatedEmployeeId)
      return `${baseUrl}?${params.toString()}`
    },
    async performApiRequest(url) {
      const response = await fetch(url, {
        method: 'GET',
        headers: {
          'Content-Type': CONTENT_TYPES.JSON
        }
      })

      if (!response.ok) {
        await this.handleApiError(response, url)
      }

      return response
    },
    async handleApiError(response, url) {
      if (response.status === HTTP_STATUS.UNAUTHORIZED) {
        throw new Error(ERROR_MESSAGES.UNAUTHORIZED_ACCESS)
      }

      if (response.status === HTTP_STATUS.NOT_FOUND) {
        throw new Error(ERROR_MESSAGES.ENDPOINT_NOT_FOUND(url))
      }

      const errorData = await this.parseErrorResponse(response)
      throw new Error(errorData.message || ERROR_MESSAGES.GENERIC_FETCH_ERROR)
    },
    async parseErrorResponse(response) {
      try {
        return await response.json()
      } catch {
        return {
          message: `Error ${response.status}: ${response.statusText}`
        }
      }
    },
    async parseResponse(response) {
      return await response.json()
    },
    updateReportsData(data) {
      this.allReports = data.sort((a, b) => {
        if (a.year !== b.year) {
          return b.year - a.year
        }
        if (a.month !== b.month) {
          return b.month - a.month
        }
        return b.payrollId - a.payrollId
      })
      this.applyDefaultFilter()
    },
    handleFetchError(error) {
      this.error = error.message || ERROR_MESSAGES.GENERIC_PAYROLL_ERROR
      this.$emit('error', this.error)
    },
    setLoadingState(isLoading) {
      this.loading = isLoading
      if (isLoading) {
        this.error = null
      }
    },
    applyDefaultFilter() {
      this.selectedReportId = null
      this.filteredReports = this.lastTenReports
    },
    applyReportFilter() {
      if (!this.selectedReportId) {
        this.filteredReports = this.lastTenReports
        return
      }

      const selectedReport = this.allReports.find(
        report => report.payrollId === this.selectedReportId
      )

      if (selectedReport) {
        this.filteredReports = [selectedReport]
      } else {
        this.filteredReports = []
      }
    },
    clearFilters() {
      this.selectedReportId = null
      this.applyDefaultFilter()
    },
    formatPeriod(report) {
      const monthName = this.months[report.month - 1]
      return `${monthName} ${report.year}`
    },
    formatCurrency(amount) {
      return new Intl.NumberFormat(LOCALE).format(amount)
    },
    formatDate(dateString) {
      return new Date(dateString).toLocaleDateString(LOCALE)
    },
    async viewReportDetail(report) {
      this.showModal = true
      this.detailedReport = null
      this.detailError = null
      this.loadingDetail = true
      this.currentPayrollId = report.payrollId

      try {
        const authenticatedEmployeeId = this.getAuthenticatedEmployeeId()
        const url = this.buildDetailedReportUrl(report.payrollId, authenticatedEmployeeId)
        const response = await this.performApiRequest(url)
        const data = await this.parseResponse(response)
        this.detailedReport = data
        await this.fetchCompanyName()
      } catch (error) {
        this.detailError = error.message || ERROR_MESSAGES.GENERIC_FETCH_ERROR
      } finally {
        this.loadingDetail = false
      }
    },
    buildDetailedReportUrl(payrollId, authenticatedEmployeeId) {
      const baseUrl = apiConfig.endpoints.employeePayrollReportDetailed(this.employeeId, payrollId)
      return `${baseUrl}?authenticatedEmployeeId=${authenticatedEmployeeId}`
    },
    closeModal() {
      this.showModal = false
      this.detailedReport = null
      this.detailError = null
      this.currentPayrollId = null
      this.downloadingPdf = false
    },
    async downloadPdfReport() {
      if (!this.currentPayrollId || !this.detailedReport) {
        return
      }

      this.downloadingPdf = true

      try {
        const authenticatedEmployeeId = this.getAuthenticatedEmployeeId()
        const url = this.buildPdfDownloadUrl(this.currentPayrollId, authenticatedEmployeeId)
        const response = await fetch(url, {
          method: 'GET',
          headers: {
            'Content-Type': CONTENT_TYPES.JSON
          }
        })

        if (!response.ok) {
          if (response.status === HTTP_STATUS.UNAUTHORIZED) {
            throw new Error(ERROR_MESSAGES.UNAUTHORIZED_ACCESS)
          }
          if (response.status === HTTP_STATUS.NOT_FOUND) {
            throw new Error(ERROR_MESSAGES.ENDPOINT_NOT_FOUND(url))
          }
          const errorData = await this.parseErrorResponse(response)
          throw new Error(errorData.message || ERROR_MESSAGES.GENERIC_FETCH_ERROR)
        }

        const blob = await response.blob()
        const downloadUrl = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = downloadUrl
        link.download = this.generatePdfFileName(this.detailedReport)
        document.body.appendChild(link)
        link.click()
        document.body.removeChild(link)
        window.URL.revokeObjectURL(downloadUrl)
      } catch (error) {
        this.detailError = error.message || 'Error al descargar el PDF'
      } finally {
        this.downloadingPdf = false
      }
    },
    buildPdfDownloadUrl(payrollId, authenticatedEmployeeId) {
      const baseUrl = apiConfig.endpoints.employeePayrollReportDownloadPdf(this.employeeId, payrollId)
      return `${baseUrl}?authenticatedEmployeeId=${authenticatedEmployeeId}`
    },
    generatePdfFileName(report) {
      const date = new Date(report.fechaGeneracion)
      const year = date.getFullYear()
      const month = String(date.getMonth() + 1).padStart(2, '0')
      const day = String(date.getDate()).padStart(2, '0')
      return `Reporte_Planilla_${year}${month}${day}.pdf`
    },
    loadUserDataFromLocalStorage() {
      const userRaw = localStorage.getItem(STORAGE_KEYS.USER)
      if (!userRaw) {
        return
      }

      try {
        const user = JSON.parse(userRaw)
        if (user) {
          const nombre = user.nombre || ''
          const segundoNombre = user.segundoNombre ? `${user.segundoNombre} ` : ''
          const apellidos = user.apellidos || ''
          this.employeeName = `${nombre} ${segundoNombre}${apellidos}`.trim()
        }
      } catch (error) {
        console.error('Error parsing user data from localStorage:', error)
      }
    },
    async fetchCompanyName() {
      const userRaw = localStorage.getItem(STORAGE_KEYS.USER)
      if (!userRaw) {
        return
      }

      try {
        const user = JSON.parse(userRaw)
        if (!user || !user.idEmpresa) {
          return
        }

        this.loadingCompanyName = true
        const response = await fetch(apiConfig.endpoints.projectById(user.idEmpresa), {
          method: 'GET',
          headers: {
            'Content-Type': CONTENT_TYPES.JSON
          }
        })

        if (response.ok) {
          const company = await response.json()
          this.companyName = company.nombre || ''
        }
      } catch (error) {
        console.error('Error fetching company name:', error)
      } finally {
        this.loadingCompanyName = false
      }
    }
  }
}
</script>


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

    <!-- Payroll Report Detail Modal -->
    <PayrollReportDetailModal
      :show="showModal"
      :detailedReport="detailedReport"
      :loadingDetail="loadingDetail"
      :detailError="detailError"
      :downloadingPdf="downloadingPdf"
      :companyName="companyName"
      :employeeName="employeeName"
      :loadingCompanyName="loadingCompanyName"
      :formatCurrency="formatCurrency"
      :formatDate="formatDate"
      @close="closeModal"
      @download-pdf="downloadPdfReport"
    />

  </div>
</template>

<script>
import { apiConfig } from '../../config/api.js'
import PayrollReportDetailModal from './PayrollReportDetailModal.vue'

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
  components: {
    PayrollReportDetailModal
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


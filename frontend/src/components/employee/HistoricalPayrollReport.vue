<template>
  <div class="body p-0! m-0!">

    <!-- Filters Section -->
    <div class="w-full">
      <h3 class="text-lg font-semibold mb-4">Filtros: Por fecha (fecha inicio y fecha final)</h3>
      <div class="flex flex-wrap gap-6 items-end">
        <div class="flex flex-wrap gap-4 items-end flex-grow">
          <div class="min-w-[200px]">
            <label class="block text-sm font-medium text-gray-700 mb-2">Fecha Inicio</label>
            <input
              v-model="filters.startDate"
              type="date"
              class="neumorphism-input"
              @change="applyFilters"
            />
          </div>
          <div class="min-w-[200px]">
            <label class="block text-sm font-medium text-gray-700 mb-2">Fecha Final</label>
            <input
              v-model="filters.endDate"
              type="date"
              class="neumorphism-input"
              @change="applyFilters"
            />
          </div>
          <div class="flex-shrink-0">
            <button
              @click="clearFilters"
              class="neumorphism-button-normal-light w-full whitespace-nowrap"
            >
              Limpiar Filtros
            </button>
          </div>
        </div>

        <div class="flex items-end justify-end flex-shrink-0 ml-auto">
          <button
            v-if="report && report.items && report.items.length > 0 && !loading"
            @click="downloadExcelReport"
            :disabled="downloadingExcel"
            class="neumorphism-button-normal-blue px-4 py-2 flex items-center gap-2"
          >
            <svg v-if="!downloadingExcel" class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 10v6m0 0l-3-3m3 3l3-3m2 8H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"></path>
            </svg>
            <div v-else class="inline-block animate-spin rounded-full h-5 w-5 border-b-2 border-white"></div>
            {{ downloadingExcel ? 'Descargando...' : 'Exportar Excel' }}
          </button>
        </div>
      </div>
    </div>


 




    <!-- Loading State -->
    <div v-if="loading" class="text-center py-8">
      <div class="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-blue-500"></div>
      <p class="mt-2 text-gray-600">Cargando reporte histórico...</p>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
      {{ error }}
    </div>

    <!-- Empty State -->
    <div v-else-if="!report || !report.items || report.items.length === 0" class="bg-gray-100 border border-gray-300 rounded-lg p-8 text-center">
      <p class="text-gray-600 text-lg">No se encontraron registros para los filtros seleccionados</p>
    </div>

    <!-- Report Content -->
    <div v-else class="space-y-6">
      <!-- Employee and Company Information -->
      <div class="neumorphism-card p-4!"> 
        <div class="grid grid-cols-2 gap-4">
          <div>
            <p class="text-sm text-blue-600 italic mb-1">Nombre de la empresa</p>
            <p v-if="loadingCompanyName" class="text-base font-medium text-gray-400">Cargando...</p>
            <p v-else class="text-base font-medium">{{ companyName || 'No disponible' }}</p>
          </div>
          <div>
            <p class="text-sm text-blue-600 italic mb-1">Nombre del empleado</p>
            <p class="text-base font-medium">{{ employeeName || 'No disponible' }}</p>
          </div>
        </div>
      </div>


      <!-- Table -->
      <div class="neumorphism-table-wrapper overflow-x-auto">
        <table class="neumorphism-table min-w-full divide-y divide-gray-200">
          <thead class="bg-blue-50">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Tipo de contrato
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Posición
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Fecha de pago
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Salario Bruto
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Deducciones obligatorias empleado
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Deducciones voluntarias
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Salario neto
              </th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr v-for="item in report.items" :key="item.payrollId" class="hover:bg-gray-50">
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                {{ item.contractType }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                {{ item.position }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                {{ formatDate(item.paymentDate) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                ₡{{ formatCurrency(item.grossSalary) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                ₡{{ formatCurrency(item.mandatoryEmployeeDeductions) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                ₡{{ formatCurrency(item.voluntaryDeductions) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                ₡{{ formatCurrency(item.netSalary) }}
              </td>
            </tr>
            <!-- Total Row -->
            <tr class="bg-gray-50 font-semibold">
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900" colspan="3">
                Total
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                ₡{{ formatCurrency(report.totals.totalGrossSalary) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                ₡{{ formatCurrency(report.totals.totalMandatoryEmployeeDeductions) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                ₡{{ formatCurrency(report.totals.totalVoluntaryDeductions) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                ₡{{ formatCurrency(report.totals.totalNetSalary) }}
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script>
import { apiConfig } from '../../config/api.js'
import {
  HTTP_STATUS,
  STORAGE_KEYS,
  CONTENT_TYPES,
  LOCALE,
  ERROR_MESSAGES
} from '../../config/const.js'

export default {
  name: 'HistoricalPayrollReport',
  props: {
    employeeId: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      report: null,
      loading: false,
      error: null,
      filters: {
        startDate: null,
        endDate: null
      },
      downloadingExcel: false,
      employeeName: '',
      companyName: '',
      loadingCompanyName: false
    }
  },
  mounted() {
    this.loadUserDataFromLocalStorage()
    this.fetchReport()
  },
  methods: {
    async fetchReport() {
      if (!this.validateEmployeeId()) {
        return
      }

      this.setLoadingState(true)

      try {
        const authenticatedEmployeeId = this.getAuthenticatedEmployeeId()
        const url = this.buildApiUrl(authenticatedEmployeeId)
        const response = await this.performApiRequest(url)
        const data = await this.parseResponse(response)
        this.report = data
        await this.fetchCompanyName()
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

      if (this.filters.startDate) {
        params.append('startDate', this.filters.startDate)
      }
      if (this.filters.endDate) {
        params.append('endDate', this.filters.endDate)
      }

      return params
    },
    buildApiUrl(authenticatedEmployeeId) {
      const baseUrl = apiConfig.endpoints.employeeHistoricalPayrollReport(this.employeeId)
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
    applyFilters() {
      this.fetchReport()
    },
    clearFilters() {
      this.filters = {
        startDate: null,
        endDate: null
      }
      this.fetchReport()
    },
    formatCurrency(amount) {
      return new Intl.NumberFormat(LOCALE).format(amount)
    },
    formatDate(dateString) {
      return new Date(dateString).toLocaleDateString(LOCALE)
    },
    async downloadExcelReport() {
      if (!this.report || !this.report.items || this.report.items.length === 0) {
        return
      }

      this.downloadingExcel = true

      try {
        const authenticatedEmployeeId = this.getAuthenticatedEmployeeId()
        const url = this.buildExcelDownloadUrl(authenticatedEmployeeId)
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
        link.download = this.generateExcelFileName()
        document.body.appendChild(link)
        link.click()
        document.body.removeChild(link)
        window.URL.revokeObjectURL(downloadUrl)
      } catch (error) {
        this.error = error.message || 'Error al descargar el Excel'
      } finally {
        this.downloadingExcel = false
      }
    },
    buildExcelDownloadUrl(authenticatedEmployeeId) {
      const baseUrl = apiConfig.endpoints.employeeHistoricalPayrollReportDownloadExcel(this.employeeId)
      const params = new URLSearchParams({
        authenticatedEmployeeId: authenticatedEmployeeId.toString()
      })

      if (this.filters.startDate) {
        params.append('startDate', this.filters.startDate)
      }
      if (this.filters.endDate) {
        params.append('endDate', this.filters.endDate)
      }

      return `${baseUrl}?${params.toString()}`
    },
    generateExcelFileName() {
      const today = new Date()
      const year = today.getFullYear()
      const month = String(today.getMonth() + 1).padStart(2, '0')
      const day = String(today.getDate()).padStart(2, '0')
      return `Reporte_Historico_Planilla_${year}${month}${day}.xlsx`
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


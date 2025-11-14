<template>
  <div class="payroll-reports-container">
    <h1 class="text-2xl font-bold mb-4">Mis Reportes de Planilla</h1>
    <p class="text-gray-600 mb-6">Visualiza y descarga tus recibos de pago históricos</p>

    <!-- Filters Section -->
    <div class="bg-white rounded-lg shadow-md p-6 mb-6">
      <h2 class="text-lg font-semibold mb-4">Filtros</h2>
      <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">Año</label>
          <select
            v-model="filters.year"
            class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            @change="applyFilters"
          >
            <option :value="null">Todos los años</option>
            <option v-for="year in availableYears" :key="year" :value="year">
              {{ year }}
            </option>
          </select>
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">Mes</label>
          <select
            v-model="filters.month"
            class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            @change="applyFilters"
          >
            <option :value="null">Todos los meses</option>
            <option v-for="(month, index) in months" :key="index" :value="index + 1">
              {{ month }}
            </option>
          </select>
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">Posición</label>
          <select
            v-model="filters.puesto"
            class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            @change="applyFilters"
          >
            <option :value="null">Todas las posiciones</option>
            <option v-for="puesto in availablePositions" :key="puesto" :value="puesto">
              {{ puesto }}
            </option>
          </select>
        </div>
        <div class="flex items-end">
          <button
            @click="clearFilters"
            class="w-full px-4 py-2 bg-gray-200 text-gray-700 rounded-md hover:bg-gray-300 transition-colors"
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
    <div v-else class="bg-white rounded-lg shadow-md overflow-hidden">
      <table class="min-w-full divide-y divide-gray-200">
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
        <tbody class="bg-white divide-y divide-gray-200">
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
                @click="downloadReport(report)"
                class="px-4 py-2 bg-blue-500 text-white rounded-md hover:bg-blue-600 transition-colors"
              >
                Ver Detalle
              </button>
            </td>
          </tr>
        </tbody>
      </table>
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
  ERROR_MESSAGES,
  MONTHS,
  DEFAULT_FILTERS
} from '../../config/const.js'

export default {
  name: 'PayrollReports',
  props: {
    employeeId: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      reports: [],
      filteredReports: [],
      loading: false,
      error: null,
      filters: { ...DEFAULT_FILTERS },
      months: MONTHS
    }
  },
  computed: {
    availableYears() {
      const years = new Set()
      this.reports.forEach(report => {
        years.add(report.year)
      })
      return Array.from(years).sort((a, b) => b - a)
    },
    availablePositions() {
      const positions = new Set()
      this.reports.forEach(report => {
        if (report.puesto) {
          positions.add(report.puesto)
        }
      })
      return Array.from(positions).sort()
    }
  },
  methods: {
    async fetchReports() {
      if (!this.validateEmployeeId()) {
        return;
      }

      this.setLoadingState(true);

      try {
        const authenticatedEmployeeId = this.getAuthenticatedEmployeeId();
        const url = this.buildApiUrl(authenticatedEmployeeId);
        const response = await this.performApiRequest(url);
        const data = await this.parseResponse(response);
        this.updateReportsData(data);
      } catch (error) {
        this.handleFetchError(error);
      } finally {
        this.setLoadingState(false);
      }
    },
    validateEmployeeId() {
      if (!this.employeeId) {
        this.error = ERROR_MESSAGES.INVALID_EMPLOYEE_ID;
        return false;
      }
      return true;
    },
    getAuthenticatedEmployeeId() {
      const userRaw = localStorage.getItem(STORAGE_KEYS.USER);
      if (!userRaw) {
        throw new Error(ERROR_MESSAGES.UNAUTHENTICATED_USER);
      }

      const user = JSON.parse(userRaw);
      if (!user || !user.idPersona) {
        throw new Error(ERROR_MESSAGES.UNAUTHENTICATED_USER);
      }

      return user.idPersona;
    },
    buildQueryParams(authenticatedEmployeeId) {
      const params = new URLSearchParams({
        authenticatedEmployeeId: authenticatedEmployeeId.toString()
      });

      if (this.filters.year) {
        params.append('year', this.filters.year.toString());
      }
      if (this.filters.month) {
        params.append('month', this.filters.month.toString());
      }
      if (this.filters.puesto) {
        params.append('puesto', this.filters.puesto);
      }

      return params;
    },
    buildApiUrl(authenticatedEmployeeId) {
      const baseUrl = apiConfig.endpoints.employeePayrollReports(this.employeeId);
      const params = this.buildQueryParams(authenticatedEmployeeId);
      return `${baseUrl}?${params.toString()}`;
    },
    async performApiRequest(url) {
      const response = await fetch(url, {
        method: 'GET',
        headers: {
          'Content-Type': CONTENT_TYPES.JSON
        }
      });

      if (!response.ok) {
        await this.handleApiError(response, url);
      }

      return response;
    },
    async handleApiError(response, url) {
      if (response.status === HTTP_STATUS.UNAUTHORIZED) {
        throw new Error(ERROR_MESSAGES.UNAUTHORIZED_ACCESS);
      }

      if (response.status === HTTP_STATUS.NOT_FOUND) {
        throw new Error(ERROR_MESSAGES.ENDPOINT_NOT_FOUND(url));
      }

      const errorData = await this.parseErrorResponse(response);
      throw new Error(errorData.message || ERROR_MESSAGES.GENERIC_FETCH_ERROR);
    },
    async parseErrorResponse(response) {
      try {
        return await response.json();
      } catch {
        return {
          message: `Error ${response.status}: ${response.statusText}`
        };
      }
    },
    async parseResponse(response) {
      return await response.json();
    },
    updateReportsData(data) {
      this.reports = data;
      this.filteredReports = data;
    },
    handleFetchError(error) {
      this.error = error.message || ERROR_MESSAGES.GENERIC_PAYROLL_ERROR;
      this.$emit('error', this.error);
    },
    setLoadingState(isLoading) {
      this.loading = isLoading;
      if (isLoading) {
        this.error = null;
      }
    },
    applyFilters() {
      this.fetchReports();
    },
    clearFilters() {
      this.filters = { ...DEFAULT_FILTERS };  
      this.fetchReports();
    },
    formatPeriod(report) {
      const monthName = this.months[report.month - 1];
      return `${monthName} ${report.year}`;
    },
    formatCurrency(amount) {
      return new Intl.NumberFormat(LOCALE).format(amount);
    },
    buildReportData(report) {
      return {
        periodo: this.formatPeriod(report),
        empresa: report.nombreEmpresa,
        puesto: report.puesto,
        salarioBruto: report.salarioBruto,
        deduccionesEmpleado: report.deduccionesEmpleado,
        deduccionesEmpresa: report.deduccionesEmpresa,
        totalBeneficios: report.totalBeneficios,
        salarioNeto: report.salarioNeto,
        horas: report.horas,
        fechaGeneracion: this.formatDate(report.fechaGeneracion)
      };
    },
    formatDate(dateString) {
      return new Date(dateString).toLocaleDateString(LOCALE);
    },
    generateReportContent(reportData) {
      return `
RECIBO DE PAGO DE PLANILLA
===========================

Empresa: ${reportData.empresa}
Período: ${reportData.periodo}
Fecha de Generación: ${reportData.fechaGeneracion}
Posición: ${reportData.puesto}
Horas Trabajadas: ${reportData.horas}

DETALLE DE PAGO:
-----------------
Salario Bruto:        ₡${this.formatCurrency(reportData.salarioBruto)}
Deducciones Empleado: ₡${this.formatCurrency(reportData.deduccionesEmpleado)}
Deducciones Empresa:  ₡${this.formatCurrency(reportData.deduccionesEmpresa)}
Total Beneficios:     ₡${this.formatCurrency(reportData.totalBeneficios)}
----------------------------------------
SALARIO NETO:         ₡${this.formatCurrency(reportData.salarioNeto)}

Este documento es un comprobante de pago generado por PlaniFy.
      `.trim();
    },
    generateFileName(report) {
      const monthPadded = report.month.toString().padStart(2, '0');
      return `Recibo_Planilla_${report.year}_${monthPadded}.txt`;
    },
    createDownloadFile(content, report) {
      const blob = new Blob([content], { type: CONTENT_TYPES.TEXT_PLAIN });
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.download = this.generateFileName(report);
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      window.URL.revokeObjectURL(url);
    }
  },
  mounted() {
    this.fetchReports();
  }
}
</script>

<style scoped>
.payroll-reports-container {
  max-width: 100%;
}
</style>


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

      <!-- Employee: Current Payroll Report -->
      <CurrentPayrollReport
        v-if="selectedReportType.id === 'pago-planilla' && userType === 'employee'"
        :employee-id="employeeId"
        @error="handleError"
      />

      <!-- Employee: Historical Payroll Report -->
      <HistoricalPayrollReport
        v-if="selectedReportType.id === 'historico-pago-planilla' && userType === 'employee'"
        :employee-id="employeeId"
        @error="handleError"
      />

      <!-- Employer: Employee Payroll Report -->
      <div v-if="selectedReportType.id === 'employee-payroll-report' && userType === 'employer'">
        <label class="block mb-2 font-semibold">Seleccione una planilla:</label>
        <ul class="mb-4">
          <li v-for="payroll in payrolls" :key="payroll.payrollId" class="mb-2">
            <button
              class="w-full text-left px-4 py-4 rounded-lg border neumorphism-on-small-item hover:bg-blue-100 flex justify-between items-center transition-colors"
              :class="{'bg-blue-200': selectedPayroll && selectedPayroll.payrollId === payroll.payrollId}"
              @click="togglePayroll(payroll)"
            >
              <span>
                <span class="font-semibold">{{ formatDate(payroll.fechaGeneracion) }}</span>
                <span class="text-gray-500 ml-2">|</span>
                <span class="ml-2">Total: ₡{{ payroll.totalNet.toLocaleString() }}</span>
              </span>
              <svg class="w-6 h-6 text-blue-400" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" d="M9 5l7 7-7 7"/>
              </svg>
            </button>

            <div v-if="selectedPayroll && selectedPayroll.payrollId === payroll.payrollId">
              <label class="block mb-2 font-semibold">Empleados en la planilla:</label>
              <table class="min-w-full mb-4 border rounded">
                <thead>
                  <tr class="bg-gray-100">
                    <th class="px-4 py-2 text-left">Nombre</th>
                    <th class="px-4 py-2 text-left">Salario Bruto</th>
                    <th class="px-4 py-2 text-left">Acción</th>
                  </tr>
                </thead>
                <tbody>
                  <tr 
                    v-for="emp in employees" 
                    :key="emp.id"
                    :class="{'bg-blue-100': selectedEmployee === emp.id}"
                  >
                    <td class="px-4 py-2">{{ emp.nombre }}</td>
                    <td class="px-4 py-2">₡{{ emp.salarioBruto ? emp.salarioBruto.toLocaleString() : '' }}</td>
                    <td class="px-4 py-2">
                      <button
                        class="px-3 py-1 bg-blue-500 text-white rounded hover:bg-blue-600"
                        @click="selectedEmployee = emp.id"
                      >
                        Ver Reporte
                      </button>
                    </td>
                  </tr>
                </tbody>
              </table>
              <CurrentPayrollReport
                v-if="selectedEmployee"
                :employee-id="selectedEmployee"
                :payroll-id="selectedPayroll.payrollId"
                @error="handleError"
              />
            </div>
          </li>
        </ul>

        <CurrentPayrollReport
          v-if="selectedEmployee"
          :employee-id="selectedEmployee"
          :payroll-id="selectedPayroll.payrollId"
          @error="handleError"
        />
      </div>

      <!-- Employer: Payroll History Report -->
      <EmployerPayrollHistory
        v-if="selectedReportType.id === 'employer-payroll-history' && userType === 'employer'"
        @error="handleError"
      />
    </div>
  </div>
</template>

<script>
import CurrentPayrollReport from '../employee/CurrentPayrollReport.vue'
import HistoricalPayrollReport from '../employee/HistoricalPayrollReport.vue'
import EmployerPayrollHistory from '../employer/projectDashboard/EmployerPayrollHistory.vue'
import { apiConfig } from '../../config/api.js'

export default {
  name: 'PayrollReports',
  components: {
    CurrentPayrollReport,
    HistoricalPayrollReport,
    EmployerPayrollHistory
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
      selectedReportType: null,
      selectedEmployee: '',
      payrolls: [],
      selectedPayroll: null,
      employees: []
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
            id: 'employee-payroll-report',
            name: 'REPORTE DE PLANILLA POR EMPLEADO'
          },
          {
            id: 'employer-payroll-history',
            name: 'HISTORIAL DE PLANILLAS'
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
      if (reportType.id === 'employee-payroll-report' && this.userType === 'employer') {
        this.fetchPayrolls()
      }
    },
    goBackToReportTypeSelection() {
      this.selectedReportType = null
      this.selectedEmployee = ''
      this.selectedPayroll = null
      this.employees = []
    },
    handleError(error) {
      this.$emit('error', error)
    },
    getCompanyId() {
      try {
        const project = JSON.parse(localStorage.getItem('selectedProject'));
        return project && project.id ? project.id : null;
      } catch {
        return null;
      }
    },
    async togglePayroll(payroll) {
      if (this.selectedPayroll && this.selectedPayroll.payrollId === payroll.payrollId) {
        this.selectedPayroll = null
        this.selectedEmployee = ''
        this.employees = []
      } else {
        this.selectedPayroll = payroll
        this.selectedEmployee = ''
        await this.fetchEmployeesForPayroll(payroll.payrollId)
      }
    },
    async fetchPayrolls() {
      const companyId = this.getCompanyId();
      if (!companyId) {
        this.payrolls = [];
        return;
      }
      try {
        const url = apiConfig.endpoints.payrollHistory(companyId);
        const res = await fetch(url);
        if (!res.ok) {
          throw new Error('Error al obtener planillas');
        }
        this.payrolls = await res.json();
      } catch (err) {
        this.handleError(err);
        this.payrolls = [];
      }
    },
    async selectPayroll(payroll) {
      this.selectedPayroll = payroll
      this.selectedEmployee = ''
      await this.fetchEmployeesForPayroll(payroll.payrollId)
    },
    async fetchEmployeesForPayroll(payrollId) {
      try {
        const url = apiConfig.endpoints.payrollEmployees(payrollId)
        const res = await fetch(url)
        if (!res.ok) throw new Error('Error al obtener empleados')
        const payrollEmployees = await res.json()
        this.employees = payrollEmployees.map(emp => ({
          id: emp.idEmpleado,
          nombre: `${emp.nombre} ${emp.apellidos}`,
          salarioBruto: emp.salarioBruto
        }))
      } catch (err) {
        this.handleError(err)
        this.employees = []
      }
    },
    formatDate(value) {
      if (!value) return '';
      return new Date(value).toLocaleDateString();
    }
  }
}
</script>
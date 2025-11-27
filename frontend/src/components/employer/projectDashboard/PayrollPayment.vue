<template>
  <div class="w-full min-h-screen bg-[#eaf6fd] p-6 flex flex-col items-center justify-center">
    <h1 class="text-3xl font-bold mb-8 text-center w-full">Pago de planilla</h1>

    <div v-if="showSuccess" class="fixed top-4 right-4 bg-green-600 text-white px-4 py-3 rounded shadow-md z-50">
      {{ successMessage }}
    </div>
    <div v-if="error" class="fixed top-4 right-4 bg-red-600 text-white px-4 py-3 rounded shadow-md z-50">
      {{ error }}
    </div>

    <div class="flex gap-6 mb-8 flex-wrap mt-6 w-full max-w-3xl">
      <div class="neumorphism-card p-6 flex-1 flex flex-col items-start min-w-[260px] mb-4">
        <span class="text-xs text-gray-500 mb-1 flex items-center gap-1">
          Nómina Total Mensual
          <span class="text-blue-500 text-base">$</span>
        </span>
        <span class="text-2xl font-bold text-gray-800">₡{{ formatNumber(latestTotalGross) }}</span>
      </div>
      <div class="neumorphism-card p-6 flex-1 flex flex-col items-start min-w-[200px] mb-4">
        <span class="text-xs text-gray-500 mb-1 flex items-center gap-1">
          Empleados en Planilla
          <span class="text-green-500 text-base">👤</span>
        </span>
        <span class="text-2xl font-bold text-gray-800">{{ employeeCount }}</span>
      </div>
    </div>

    <button
      class="custom-button neumorphism-light flex items-center gap-2 px-10 py-6 bg-blue-500 hover:bg-blue-600 text-white text-2xl font-bold rounded shadow transition-colors duration-200 mb-10"
      @click="onPayPayroll"
      :disabled="loading"
    >
      <span>💸</span>
      Pagar planilla
    </button>

    <div class="neumorphism-card p-8 w-full max-w-5xl mt-4">
      <h2 class="text-xl font-semibold mb-2 w-full">Última planilla generada</h2>
      <p class="text-xs text-gray-500 mb-4">Desglose de la última planilla creada</p>
      <div v-if="lastPayroll">
        <table class="min-w-full text-sm text-left">
          <thead>
            <tr class="bg-[#eaf6fd]">
              <th class="py-2 px-4 font-medium">Fecha de pago</th>
              <th class="py-2 px-4 font-medium">Salario Bruto</th>
              <th class="py-2 px-4 font-medium">Deducciones de ley Empleador</th>
              <th class="py-2 px-4 font-medium">Deducciones ley empleados</th>
              <th class="py-2 px-4 font-medium">Beneficios</th>
              <th class="py-2 px-4 font-medium">Salario Neto</th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td class="py-2 px-4">{{ formatDate(lastPayroll.fechaGeneracion) }}</td>
              <td class="py-2 px-4">₡{{ formatNumber(lastPayroll.totalGross) }}</td>
              <td class="py-2 px-4">₡{{ formatNumber(lastPayroll.totalEmployerDeductions) }}</td>
              <td class="py-2 px-4">₡{{ formatNumber(lastPayroll.totalEmployeeDeductions) }}</td>
              <td class="py-2 px-4 text-blue-600 font-semibold">₡{{ formatNumber(lastPayroll.totalBenefits) }}</td>
              <td class="py-2 px-4">₡{{ formatNumber(lastPayroll.totalNet) }}</td>
            </tr>
          </tbody>
        </table>
      </div>
      <div v-else class="text-gray-500 text-center py-8">
        No hay planillas generadas aún.
      </div>
    </div>
  </div>
</template>

<script>
import { apiConfig } from '@/config/api'
export default {
  data() {
    return {
      companyId: null,
      currentUserId: null,
      selectedProject: null,
      history: [],
      latestTotalGross: 0,
      employeeCount: 0,
      hoursPerPayroll: 200,
      loading: false,
      error: null,
      showSuccess: false,
      successMessage: ''
    };
  },
  computed: {
    lastPayroll() {
      return this.history && this.history.length > 0 ? this.history[0] : null;
    }
  },
  methods: {
    setError(message) {
      this.error = message;
      setTimeout(() => {
        this.error = null;
      }, 3000);
    },
    showSuccessPopup(message) {
      this.successMessage = message;
      this.showSuccess = true;
      setTimeout(() => {
        this.showSuccess = false;
        this.successMessage = '';
      }, 3000);
    },
    async fetchJson(url, options = {}) {
      const res = await fetch(url, options);
      if (!res.ok) throw Object.assign(new Error('Request failed'), { status: res.status });
      return res.json();
    },
    loadFromLocalStorage() {
      this.currentUserId = localStorage.getItem('employerId');
      let company = JSON.parse(localStorage.getItem('selectedProject'));
      if (company) {
        this.companyId = company.id;
        this.selectedProject = company;
      }
    },
    derivePeriodInfo() {
      const periodType = (this.selectedProject && this.selectedProject.periodoPago) || 'Mensual';
      const fortnight = periodType.toLowerCase() === 'quincenal' ? (new Date().getDate() <= 15 ? 1 : 2) : null;
      return { periodType, fortnight };
    },
    async onPayPayroll() {
      try {
        this.loading = true;
        this.error = null;
        if (!this.companyId || !this.currentUserId) {
          this.setError('Faltan datos: companyId o userId en localStorage.');
          return;
        }
        const { periodType, fortnight } = this.derivePeriodInfo();
        const response = await fetch(apiConfig.endpoints.payrollGenerate, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({
            companyId: this.companyId,
            responsibleEmployeeId: this.currentUserId,
            hours: this.hoursPerPayroll,
            periodType: periodType,
            fortnight: fortnight
          })
        });
        if (!response.ok) {
          this.setError('Ya existe una planilla generada para este periodo.');
        }
        await this.fetchHistory();
        this.showSuccessPopup('Planilla generada exitosamente.');
      } catch (e) {
        if (e && e.status === 409) {
          this.setError('Ya existe una planilla generada para este periodo.');
        } else {
          this.setError('Error al pagar planilla');
        }
      } finally {
        this.loading = false;
      }
    },
    async fetchHistory() {
      try {
        if (!this.companyId) return;
        this.history = await this.fetchJson(apiConfig.endpoints.payrollHistory(this.companyId));
        // Also fetch latest summary for totalGross
        const sum = await this.fetchJson(apiConfig.endpoints.payrollSummary(this.companyId));
        this.latestTotalGross = sum?.totalGross || 0;
        // Fetch employee count from existing Project endpoint
        const ecJson = await this.fetchJson(apiConfig.endpoints.projectEmployeeCount(this.companyId));
        this.employeeCount = ecJson?.count ?? 0;
      } catch (e) {
        this.history = [];
        this.latestTotalGross = 0;
        this.employeeCount = 0;
      }
    },
    formatNumber(val) {
      if (val === null || val === undefined) return '0';
      const num = Number(val);
      return num.toLocaleString('es-CR');
    },
    formatDate(iso) {
      const d = new Date(iso);
      return d.toLocaleDateString('es-CR', { year: 'numeric', month: 'long' });
    }
  },
  mounted() {
    this.loadFromLocalStorage();
    this.fetchHistory();
  }
};
</script>
<template>
  <div class="w-full min-h-screen bg-[#eaf6fd] p-6">
    <div class="flex justify-between items-center mb-4">
      <h1 class="text-2xl font-bold">Historial de planillas</h1>
      <div class="flex gap-2">
        <!-- Dropdown de períodos -->
        <select v-model="selectedPeriod" class="p-2 border rounded">
          <option value="12">Últimos 12 meses</option>
          <option value="6">Últimos 6 meses</option>
          <option value="3">Últimos 3 meses</option>
        </select>
        <button 
          @click="generateExcel"
          class="bg-green-600 hover:bg-green-700 text-white px-4 py-2 rounded-lg flex items-center gap-2"
        >
          <span>Generar Excel</span>
        </button>
      </div>
    </div>
    
    <!-- Filtros -->
    <div class="neumorphism-card p-4 mb-6">
      <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div>
          <label class="block text-sm font-medium mb-1">Fecha inicio</label>
          <input 
            type="date" 
            v-model="filters.startDate"
            class="w-full p-2 border rounded"
          >
        </div>
        <div>
          <label class="block text-sm font-medium mb-1">Fecha final</label>
          <input 
            type="date" 
            v-model="filters.endDate"
            class="w-full p-2 border rounded"
          >
        </div>
        <div>
          <label class="block text-sm font-medium mb-1">Empresa</label>
          <select v-model="filters.companyId" class="w-full p-2 border rounded">
            <option value="">Todas las empresas</option>
          </select>
        </div>
      </div>
    </div>

    <hr class="mb-6 border-[#c7e0f7]">

    <div class="neumorphism-card p-6">
      <div class="overflow-x-auto">
        <table class="min-w-full text-sm text-left">
          <thead>
            <tr class="bg-[#eaf6fd]">
              <th class="py-2 px-4 font-medium border">Nombre de la empresa</th>
              <th class="py-2 px-4 font-medium border">Frecuencia de pago</th>
              <th class="py-2 px-4 font-medium border">Período de pago</th>
              <th class="py-2 px-4 font-medium border">Fecha de pago</th>
              <th class="py-2 px-4 font-medium border">Salario Bruto</th>
              <th class="py-2 px-4 font-medium border">Cargas sociales empleador</th>
              <th class="py-2 px-4 font-medium border">Deducciones voluntarias</th>
              <th class="py-2 px-4 font-medium border">Costo empleador</th>
              <th class="py-2 px-4 font-medium border">Acciones</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in filteredHistory" :key="item.payrollId" 
                class="border-t border-[#e0e0e0] hover:bg-[#f4fbff]">
              <td class="py-2 px-4 border">{{ companyName }}</td>
              <td class="py-2 px-4 border">{{ getPaymentFrequency(item.fechaGeneracion) }}</td>
              <td class="py-2 px-4 border">{{ getPaymentPeriod(item.fechaGeneracion) }}</td>
              <td class="py-2 px-4 border">{{ formatDate(item.fechaGeneracion) }}</td>
              <td class="py-2 px-4 border">₡{{ formatNumber(item.totalGross) }}</td>
              <td class="py-2 px-4 border">₡{{ formatNumber(item.totalEmployerDeductions) }}</td>
              <td class="py-2 px-4 border">₡{{ formatNumber(item.totalEmployeeDeductions) }}</td>
              <td class="py-2 px-4 border">₡{{ formatNumber(calculateEmployerCost(item)) }}</td>
              <td class="py-2 px-4 border">
                <button 
                  @click="openDetailsModal(item)"
                  class="text-blue-600 hover:text-blue-800 underline text-sm"
                >
                  Ver detalles
                </button>
              </td>
            </tr>
            
            <tr class="bg-gray-100 font-semibold">
              <td class="py-2 px-4 border" colspan="4">Total</td>
              <td class="py-2 px-4 border">₡{{ formatNumber(totals.gross) }}</td>
              <td class="py-2 px-4 border">₡{{ formatNumber(totals.employerDeductions) }}</td>
              <td class="py-2 px-4 border">₡{{ formatNumber(totals.employeeDeductions) }}</td>
              <td class="py-2 px-4 border">₡{{ formatNumber(totals.employerCost) }}</td>
              <td class="py-2 px-4 border"></td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Modal de detalles -->
    <PayrollDetailsModal
      :is-visible="showDetailsModal"
      :payroll-data="selectedPayroll"
      :company-name="companyName"
      :employer-name="employerName"
      @close="closeDetailsModal"
    />
  </div>
</template>

<script>
import { apiConfig } from '@/config/api'
import PayrollDetailsModal from './PayrollDetailsModal.vue'

export default {
  name: 'EmployerPayrollHistoryReport',
  components: {
    PayrollDetailsModal
  },
  data() {
    return {
      companyId: null,
      companyName: '',
      employerName: '',
      history: [],
      filters: {
        startDate: '',
        endDate: '',
        companyId: ''
      },
      selectedPeriod: '12',
      showDetailsModal: false,
      selectedPayroll: null
    }
  },
  computed: {
    filteredHistory() {
      // Filtrar por período seleccionado
      const months = parseInt(this.selectedPeriod);
      const cutoffDate = new Date();
      cutoffDate.setMonth(cutoffDate.getMonth() - months);
      
      return this.history.filter(item => {
        const itemDate = new Date(item.fechaGeneracion);
        return itemDate >= cutoffDate;
      });
    },
    totals() {
      return this.filteredHistory.reduce((acc, item) => {
        acc.gross += item.totalGross;
        acc.employerDeductions += item.totalEmployerDeductions;
        acc.employeeDeductions += item.totalEmployeeDeductions;
        acc.employerCost += this.calculateEmployerCost(item);
        return acc;
      }, { gross: 0, employerDeductions: 0, employeeDeductions: 0, employerCost: 0 });
    }
  },
  methods: {
    async fetchJson(url, options = {}) {
      const res = await fetch(url, options);
      if (!res.ok) throw Object.assign(new Error('Request failed'), { status: res.status });
      return res.json();
    },
    async fetchHistory() {
      try {
        if (!this.companyId) return;
        this.history = await this.fetchJson(apiConfig.endpoints.payrollHistory(this.companyId));
      } catch (e) {
        this.history = [];
      }
    },
    formatNumber(val) {
      if (val === null || val === undefined) return '0';
      const num = Number(val);
      return num.toLocaleString('es-CR');
    },
    formatDate(iso) {
      const date = new Date(iso);
      return date.toLocaleDateString('es-CR');
    },
    calculateEmployerCost(item) {
      return item.totalGross + item.totalEmployerDeductions;
    },
    getPaymentFrequency() {
      return 'Mensual';
    },
    getPaymentPeriod(date) {
      const paymentDate = new Date(date);
      const year = paymentDate.getFullYear();
      const month = paymentDate.getMonth() + 1;
      
      const startDate = `01/${month.toString().padStart(2, '0')}/${year}`;
      const endDate = new Date(year, month, 0).getDate();
      return `Del ${startDate} al ${endDate}/${month.toString().padStart(2, '0')}/${year}`;
    },
    openDetailsModal(payroll) {
      this.selectedPayroll = payroll;
      this.showDetailsModal = true;
    },
    closeDetailsModal() {
      this.showDetailsModal = false;
      this.selectedPayroll = null;
    },
    generateExcel() {
      console.log('Generando Excel...');
    }
  },
  mounted() {
    let company = JSON.parse(localStorage.getItem('selectedProject'));
    if (company) {
      this.companyId = company.id;
      this.companyName = company.name || 'Empresa';
      this.employerName = company.employerName || 'Empleador';
      this.fetchHistory();
    }
  }
}
</script>
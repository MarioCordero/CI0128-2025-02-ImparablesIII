<template>
  <div class="w-full min-h-screen bg-[#eaf6fd] p-6">
    <h1 class="text-2xl font-bold mb-4">Historial de Planillas</h1>
    <hr class="mb-6 border-[#c7e0f7]">

    <div class="neumorphism-card p-6">
      <h2 class="text-base font-semibold mb-1">Detalle de Planilla por Fecha</h2>
      <p class="text-xs text-gray-500 mb-4">Desglose completo de salarios, deducciones y beneficios</p>
      <div class="overflow-x-auto">
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
            <tr v-for="row in history" :key="row.payrollId" class="border-t border-[#e0e0e0] hover:bg-[#f4fbff]">
              <td class="py-2 px-4">{{ formatDate(row.fechaGeneracion) }}</td>
              <td class="py-2 px-4">₡{{ formatNumber(row.totalGross) }}</td>
              <td class="py-2 px-4">₡{{ formatNumber(row.totalEmployerDeductions) }}</td>
              <td class="py-2 px-4">₡{{ formatNumber(row.totalEmployeeDeductions) }}</td>
              <td class="py-2 px-4 text-blue-600 font-semibold">₡{{ formatNumber(row.totalBenefits) }}</td>
              <td class="py-2 px-4">₡{{ formatNumber(row.totalNet) }}</td>
            </tr>
            <tr class="h-8"></tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script>
import { apiConfig } from '@/config/api'
export default {
  name: 'EmployerPayrollHistory',
  data() {
    return {
      companyId: null,
      history: []
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
      const d = new Date(iso);
      return d.toLocaleDateString('es-CR', { year: 'numeric', month: 'long' });
    }
  },
  mounted() {
    let company = JSON.parse(localStorage.getItem('selectedProject'));
    if (company) {
      this.companyId = company.id;
      this.fetchHistory();
    }
  }
}
</script>
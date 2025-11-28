<template>
  <div v-if="isVisible" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
    <div class="bg-white rounded-lg w-11/12 max-w-4xl max-h-[90vh] overflow-y-auto">
      <!-- Header -->
      <div class="bg-[#eaf6fd] p-6 rounded-t-lg">
        <div class="flex justify-between items-start">
          <div>
            <h2 class="text-2xl font-bold">REPORTE 4 – EMPLEADOR (PAGO PLANILLA)</h2>
            <h3 class="text-lg font-semibold mt-2">{{ companyName }}</h3>
            <h4 class="text-md">{{ employerName }}</h4>
            <p class="text-sm text-gray-600 mt-1">{{ formatPeriod(payrollData.fechaGeneracion) }}</p>
          </div>
          <button 
            @click="closeModal"
            class="text-gray-500 hover:text-gray-700 text-2xl"
          >
            ×
          </button>
        </div>
      </div>

      <!-- Content -->
      <div class="p-6">
        <!-- Salarios Section -->
        <div class="mb-6">
          <h4 class="font-semibold mb-3 text-lg">Salarios</h4>
          <table class="w-full text-sm">
            <tbody>
              <tr class="border-b">
                <td class="py-2 pr-4">Salario empleados por horas</td>
                <td class="py-2 text-right">₡{{ formatNumber(salaryBreakdown.hourly) }}</td>
              </tr>
              <tr class="border-b">
                <td class="py-2 pr-4">Salario tiempo completo</td>
                <td class="py-2 text-right">₡{{ formatNumber(salaryBreakdown.fullTime) }}</td>
              </tr>
              <tr class="border-b">
                <td class="py-2 pr-4">Salario servicios profesionales</td>
                <td class="py-2 text-right">₡{{ formatNumber(salaryBreakdown.professional) }}</td>
              </tr>
              <tr class="border-b font-semibold bg-gray-50">
                <td class="py-2 pr-4">Total salarios</td>
                <td class="py-2 text-right">₡{{ formatNumber(salaryBreakdown.total) }}</td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Pagos de Ley Section -->
        <div class="mb-6">
          <h4 class="font-semibold mb-3 text-lg">Pagos de Ley</h4>
          <table class="w-full text-sm">
            <tbody>
              <tr class="border-b" v-for="deduction in employerDeductions" :key="deduction.name">
                <td class="py-2 pr-4">{{ deduction.name }}</td>
                <td class="py-2 text-right">₡{{ formatNumber(deduction.amount) }}</td>
              </tr>
              <tr class="border-b font-semibold bg-gray-50">
                <td class="py-2 pr-4">Total pagos de ley</td>
                <td class="py-2 text-right">₡{{ formatNumber(payrollData.totalEmployerDeductions) }}</td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Beneficios Section -->
        <div class="mb-6">
          <h4 class="font-semibold mb-3 text-lg">Beneficios</h4>
          <table class="w-full text-sm">
            <tbody>
              <tr class="border-b">
                <td class="py-2 pr-4">Seguro Privado</td>
                <td class="py-2 text-right">₡{{ formatNumber(benefitsBreakdown.privateInsurance) }}</td>
              </tr>
              <tr class="border-b">
                <td class="py-2 pr-4">Asociación Solidarista (5.5%)</td>
                <td class="py-2 text-right">₡{{ formatNumber(benefitsBreakdown.solidarityAssociation) }}</td>
              </tr>
              <tr class="border-b font-semibold bg-gray-50">
                <td class="py-2 pr-4">Beneficios totales</td>
                <td class="py-2 text-right">₡{{ formatNumber(benefitsBreakdown.total) }}</td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Total Section -->
        <div class="border-t-2 border-gray-300 pt-4">
          <table class="w-full text-sm">
            <tbody>
              <tr class="font-semibold text-lg">
                <td class="py-2 pr-4">Costo total empleador</td>
                <td class="py-2 text-right">₡{{ formatNumber(totalEmployerCost) }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'PayrollDetailsModal',
  props: {
    isVisible: {
      type: Boolean,
      required: true
    },
    payrollData: {
      type: Object,
      default: () => ({})
    },
    companyName: {
      type: String,
      default: 'Empresa'
    },
    employerName: {
      type: String,
      default: 'Empleador'
    }
  },
  computed: {
    salaryBreakdown() {
      // Estos datos vendrían del backend, por ahora los calculamos del total
      const total = this.payrollData.totalGross || 0;
      return {
        hourly: total * 0.3, // 30% horas
        fullTime: total * 0.4, // 40% tiempo completo
        professional: total * 0.3, // 30% profesionales
        total: total
      };
    },
    employerDeductions() {
      // Deducciones de ley del empleador - datos de ejemplo
      const total = this.payrollData.totalEmployerDeductions || 0;
      return [
        { name: 'SEM (Seguro Enfermedad/Maternidad)', amount: total * 0.3 },
        { name: 'IVM (Invalidez, Vejez y Muerte)', amount: total * 0.2 },
        { name: 'Cuota Patronal Banco Popular (0.25%)', amount: total * 0.05 },
        { name: 'Asignaciones Familiares (5.00%)', amount: total * 0.05 },
        { name: 'IMAS (0.50%)', amount: total * 0.05 },
        { name: 'INA (1.50%)', amount: total * 0.05 },
        { name: 'Aporte Banco Popular (0.25%)', amount: total * 0.05 },
        { name: 'FCL - Fondo de Capitalización Laboral (3.00%)', amount: total * 0.05 },
        { name: 'Fondo de Pensiones Complementarias (0.50%)', amount: total * 0.05 },
        { name: 'INS (1.00%)', amount: total * 0.15 }
      ];
    },
    benefitsBreakdown() {
      const total = this.payrollData.totalBenefits || 0;
      return {
        privateInsurance: total * 0.6,
        solidarityAssociation: total * 0.4,
        total: total
      };
    },
    totalEmployerCost() {
      return (this.payrollData.totalGross || 0) + (this.payrollData.totalEmployerDeductions || 0);
    }
  },
  methods: {
    closeModal() {
      this.$emit('close');
    },
    formatNumber(val) {
      if (val === null || val === undefined) return '0';
      const num = Number(val);
      return num.toLocaleString('es-CR');
    },
    formatPeriod(date) {
      if (!date) return '';
      const d = new Date(date);
      const month = d.toLocaleDateString('es-CR', { month: 'long', year: 'numeric' });
      return `Periodo: ${month}`;
    }
  }
}
</script>
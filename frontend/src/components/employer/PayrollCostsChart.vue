<template>
  <div class="payroll-chart neumorphism-card p-6">
    <h2 class="text-xl font-bold mb-4">Costos de Planilla</h2>
    
    <!-- Gráfico de Costos -->
    <div class="chart-container">
      <div v-for="company in companies" :key="company.id" class="cost-item mb-4">
        <div class="flex justify-between items-center mb-2">
          <span class="text-sm font-medium text-gray-700">{{ company.nombre }}</span>
          <span class="text-sm font-semibold" :class="company.currentPayroll > 0 ? 'text-green-600' : 'text-gray-500'">
            ₡{{ formatNumber(company.currentPayroll) }}
          </span>
        </div>
        
        <!-- Barra de costos -->
        <div class="cost-bar-container">
          <div class="cost-bar-background">
            <div 
              class="cost-bar-fill bg-gradient-to-r from-green-400 to-blue-500" 
              :style="{ width: getCostPercentage(company.currentPayroll) }"
            ></div>
          </div>
        </div>
        
        <!-- Desglose si tiene planilla -->
        <div v-if="company.currentPayroll > 0" class="mt-2 text-xs text-gray-600">
          <div class="flex justify-between">
            <span>Salario bruto:</span>
            <span>₡{{ formatNumber(company.currentPayroll) }}</span>
          </div>
          <div class="flex justify-between">
            <span>Deducciones:</span>
            <span>₡{{ formatNumber(company.deductions) }}</span>
          </div>
          <div class="flex justify-between font-medium">
            <span>Neto a pagar:</span>
            <span>₡{{ formatNumber(company.netSalary) }}</span>
          </div>
        </div>
        
        <div v-else class="text-xs text-gray-500 text-center mt-1">
          Sin planilla procesada
        </div>
      </div>
    </div>
    
    <!-- Resumen -->
    <div class="mt-4 pt-4 border-t border-gray-200">
      <div class="text-sm text-gray-600">
        <div class="flex items-center justify-between">
          <span>Planilla total:</span>
          <span class="font-semibold text-green-600">₡{{ formatNumber(totalPayroll) }}</span>
        </div>
        <div class="flex items-center justify-between mt-1">
          <span>Empresas con costo:</span>
          <span class="font-semibold">{{ companiesWithCosts }}</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'PayrollCostsChart',
  props: {
    userId: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      companies: [],
      loading: false
    }
  },
  computed: {
    totalPayroll() {
      return this.companies.reduce((sum, company) => sum + company.currentPayroll, 0);
    },
    maxPayroll() {
      const payrolls = this.companies.map(company => company.currentPayroll);
      return Math.max(...payrolls, 1);
    },
    companiesWithCosts() {
      return this.companies.filter(company => company.currentPayroll > 0).length;
    }
  },
  methods: {
    async fetchPayrollData() {
      this.loading = true;
      try {
        // Datos basados en tu BD y ResumenPlanilla
        this.companies = [
          { 
            id: 17, 
            nombre: 'Imparables III', 
            currentPayroll: 500000,
            deductions: 182900, // CCSS empleado + empleador
            netSalary: 446650   // Del resumen planilla
          },
          { 
            id: 19, 
            nombre: 'Patitos', 
            currentPayroll: 0,
            deductions: 0,
            netSalary: 0
          },
          { 
            id: 20, 
            nombre: 'Pipasa', 
            currentPayroll: 0,
            deductions: 0,
            netSalary: 0
          }
        ];
      } catch (error) {
        console.error('Error fetching payroll data:', error);
      } finally {
        this.loading = false;
      }
    },
    
    getCostPercentage(payroll) {
      const percentage = (payroll / this.maxPayroll) * 100;
      return `${percentage}%`;
    },
    
    formatNumber(value) {
      return value.toLocaleString('es-CR');
    }
  },
  mounted() {
    this.fetchPayrollData();
  }
}
</script>

<style scoped>
.cost-bar-background {
  width: 100%;
  height: 6px;
  background: #e5e7eb;
  border-radius: 3px;
  overflow: hidden;
}

.cost-bar-fill {
  height: 100%;
  border-radius: 3px;
  transition: width 0.5s ease;
}
</style>
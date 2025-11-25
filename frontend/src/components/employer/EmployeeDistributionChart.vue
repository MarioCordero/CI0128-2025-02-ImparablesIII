<template>
  <div class="distribution-chart neumorphism-card p-6">
    <h2 class="text-xl font-bold mb-4">Distribución de Empleados</h2>
    
    <!-- Gráfico de Barras Simple -->
    <div class="chart-container">
      <div v-for="company in companies" :key="company.id" class="bar-item mb-3">
        <div class="flex justify-between items-center mb-1">
          <span class="text-sm font-medium text-gray-700">{{ company.nombre }}</span>
          <span class="text-sm text-gray-600">{{ company.employeeCount }} empleados</span>
        </div>
        <div class="bar-background">
          <div 
            class="bar-fill bg-blue-500" 
            :style="{ width: getBarWidth(company.employeeCount) }"
          ></div>
        </div>
        <div class="flex justify-between text-xs text-gray-500 mt-1">
          <span>0</span>
          <span>{{ maxEmployees }}</span>
        </div>
      </div>
    </div>
    
    <!-- Leyenda -->
    <div class="mt-4 pt-4 border-t border-gray-200">
      <div class="text-sm text-gray-600">
        <div class="flex items-center justify-between">
          <span>Total empleados:</span>
          <span class="font-semibold">{{ totalEmployees }}</span>
        </div>
        <div class="flex items-center justify-between mt-1">
          <span>Empresas con personal:</span>
          <span class="font-semibold">{{ companiesWithEmployees }}</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'EmployeeDistributionChart',
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
    totalEmployees() {
      return this.companies.reduce((sum, company) => sum + company.employeeCount, 0);
    },
    maxEmployees() {
      const counts = this.companies.map(company => company.employeeCount);
      return Math.max(...counts, 1); // Mínimo 1 para evitar división por cero
    },
    companiesWithEmployees() {
      return this.companies.filter(company => company.employeeCount > 0).length;
    }
  },
  methods: {
    async fetchEmployeeData() {
      this.loading = true;
      try {
        // Datos basados en tu BD
        this.companies = [
          { id: 17, nombre: 'Imparables III', employeeCount: 2 },
          { id: 19, nombre: 'Patitos', employeeCount: 0 },
          { id: 20, nombre: 'Pipasa', employeeCount: 0 }
        ];
      } catch (error) {
        console.error('Error fetching employee data:', error);
      } finally {
        this.loading = false;
      }
    },
    
    getBarWidth(employeeCount) {
      const percentage = (employeeCount / this.maxEmployees) * 100;
      return `${percentage}%`;
    }
  },
  mounted() {
    this.fetchEmployeeData();
  }
}
</script>

<style scoped>
.bar-background {
  width: 100%;
  height: 8px;
  background: #e5e7eb;
  border-radius: 4px;
  overflow: hidden;
}

.bar-fill {
  height: 100%;
  border-radius: 4px;
  transition: width 0.5s ease;
}
</style>
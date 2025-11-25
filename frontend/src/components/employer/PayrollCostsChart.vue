<template>
  <div class="payroll-chart neumorphism-card p-6">
    <h2 class="text-xl font-bold mb-4">Distribución de Costos de Planilla</h2>
    
    <!-- Gráfico de Pastel con SVG -->
    <div class="flex flex-col md:flex-row items-center gap-6">
      <!-- Gráfico de Pastel SVG -->
      <div class="relative w-48 h-48">
        <svg width="192" height="192" viewBox="0 0 42 42" class="donut">
          <!-- Fondo del donut -->
          <circle 
            cx="21" 
            cy="21" 
            r="15.91549430918954" 
            fill="transparent" 
            stroke="#e5e7eb" 
            stroke-width="3"
          />
          
          <!-- Segmentos del pastel -->
          <circle 
            v-for="(segment, index) in pieSegments" 
            :key="index"
            cx="21" 
            cy="21" 
            r="15.91549430918954" 
            fill="transparent"
            :stroke="segment.color"
            stroke-width="3"
            :stroke-dasharray="`${segment.percentage} ${100 - segment.percentage}`"
            stroke-dashoffset="25"
            class="donut-segment"
          />
        </svg>
        
        <!-- Centro del pastel con total -->
        <div class="absolute inset-0 flex items-center justify-center">
          <div class="text-center">
            <div class="text-2xl font-bold text-gray-800">₡{{ formatNumber(totalPayroll) }}</div>
            <div class="text-xs text-gray-600">Total</div>
          </div>
        </div>
      </div>
      
      <!-- Leyenda -->
      <div class="flex-1 min-w-0">
        <div class="space-y-3">
          <div 
            v-for="(company, index) in companiesWithData" 
            :key="company.id"
            class="flex items-center justify-between p-2 rounded hover:bg-gray-50 transition-colors"
          >
            <div class="flex items-center space-x-3">
              <div 
                class="w-4 h-4 rounded-full" 
                :style="{ backgroundColor: getCompanyColor(index) }"
              ></div>
              <div>
                <div class="font-medium text-gray-800">{{ company.nombre }}</div>
                <div class="text-xs text-gray-500">
                  {{ getPercentage(company.currentPayroll) }}% • {{ company.employeeCount }} empleados
                </div>
              </div>
            </div>
            <div class="text-right">
              <div class="font-semibold text-gray-800">₡{{ formatNumber(company.currentPayroll) }}</div>
              <div class="text-xs text-gray-500">planilla</div>
            </div>
          </div>
          
          <!-- Empresas sin planilla -->
          <div 
            v-if="companiesWithoutData.length > 0"
            class="p-2 border border-gray-200 rounded bg-gray-50"
          >
            <div class="text-sm font-medium text-gray-600 mb-1">Sin planilla procesada:</div>
            <div class="text-xs text-gray-500">
              {{ companiesWithoutData.map(c => c.nombre).join(', ') }}
            </div>
          </div>
        </div>
      </div>
    </div>
    
    <!-- Resumen Estadístico -->
    <div class="mt-6 pt-4 border-t border-gray-200">
      <div class="grid grid-cols-2 gap-4 text-sm">
        <div class="text-center">
          <div class="text-2xl font-bold text-blue-600">{{ companiesWithData.length }}</div>
          <div class="text-gray-600">Empresas con planilla</div>
        </div>
        <div class="text-center">
          <div class="text-2xl font-bold text-green-600">{{ totalEmployees }}</div>
          <div class="text-gray-600">Empleados totales</div>
        </div>
      </div>
      
      <!-- Empresa con mayor costo -->
      <div v-if="largestCompany" class="mt-4 p-3 bg-blue-50 rounded-lg">
        <div class="flex items-center justify-between">
          <div>
            <div class="text-sm font-medium text-blue-800">Mayor costo:</div>
            <div class="font-semibold text-blue-900">{{ largestCompany.nombre }}</div>
          </div>
          <div class="text-right">
            <div class="text-lg font-bold text-blue-800">₡{{ formatNumber(largestCompany.currentPayroll) }}</div>
            <div class="text-sm text-blue-600">{{ getPercentage(largestCompany.currentPayroll) }}% del total</div>
          </div>
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
    // Solo empresas con planilla para el gráfico
    companiesWithData() {
      return this.companies.filter(company => company.currentPayroll > 0);
    },
    
    // Empresas sin planilla
    companiesWithoutData() {
      return this.companies.filter(company => company.currentPayroll === 0);
    },
    
    totalPayroll() {
      return this.companies.reduce((sum, company) => sum + company.currentPayroll, 0);
    },
    
    totalEmployees() {
      return this.companies.reduce((sum, company) => sum + company.employeeCount, 0);
    },
    
    // Empresa con mayor costo de planilla
    largestCompany() {
      if (this.companiesWithData.length === 0) return null;
      return this.companiesWithData.reduce((max, company) => 
        company.currentPayroll > max.currentPayroll ? company : max
      );
    },
    
    // Segmentos para el gráfico de pastel
    pieSegments() {
      if (this.companiesWithData.length === 0) return [];
      
      let accumulatedPercentage = 0;
      return this.companiesWithData.map((company, index) => {
        const percentage = this.getPercentage(company.currentPayroll);
        const segment = {
          percentage: percentage,
          color: this.getCompanyColor(index),
          offset: accumulatedPercentage
        };
        accumulatedPercentage += percentage;
        return segment;
      });
    }
  },
  methods: {
    async fetchPayrollData() {
      this.loading = true;
      try {
        // Datos basados en tu BD
        this.companies = [
          { 
            id: 17, 
            nombre: 'Imparables III', 
            currentPayroll: 500000,
            employeeCount: 2,
            deductions: 182900,
            netSalary: 446650
          },
          { 
            id: 19, 
            nombre: 'Patitos', 
            currentPayroll: 0,
            employeeCount: 0,
            deductions: 0,
            netSalary: 0
          },
          { 
            id: 20, 
            nombre: 'Pipasa', 
            currentPayroll: 0,
            employeeCount: 0,
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
    
    getCompanyColor(index) {
      const colors = [
        '#3B82F6', // Azul
        '#10B981', // Verde
        '#8B5CF6', // Violeta
        '#F59E0B', // Amber
        '#EF4444', // Rojo
        '#06B6D4', // Cian
      ];
      return colors[index % colors.length];
    },
    
    getPercentage(value) {
      if (this.totalPayroll === 0) return 0;
      return (value / this.totalPayroll) * 100;
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
.payroll-chart {
  background: white;
  border-radius: 12px;
}

.donut {
  transform: rotate(-90deg);
}

.donut-segment {
  transition: stroke-dasharray 0.3s ease;
}

.donut-segment:hover {
  opacity: 0.8;
}
</style>
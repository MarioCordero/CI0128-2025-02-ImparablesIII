<template>
  <div class="payroll-chart neumorphism-card p-6">
    <h2 class="text-xl font-bold mb-4">Distribución de Costos de Planilla</h2>
    <div class="flex flex-col md:flex-row items-center gap-6">
      <div class="relative w-48 h-48">
        <svg width="192" height="192" viewBox="0 0 42 42" class="donut">
          <circle 
            cx="21" 
            cy="21" 
            r="15.91549430918954" 
            fill="transparent" 
            stroke="#e5e7eb" 
            stroke-width="3"
          />
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
        <div class="absolute inset-0 flex items-center justify-center">
          <div class="text-center">
            <div class="text-2xl font-bold text-gray-800">₡{{ formatNumber(totalPayroll) }}</div>
            <div class="text-xs text-gray-600">Total</div>
          </div>
        </div>
      </div>
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
                  {{ getPercentage(company.payroll.totalGross) }}% • {{ company.payroll.employeeCount }} empleados
                </div>
              </div>
            </div>
            <div class="text-right">
              <div class="font-semibold text-gray-800">₡{{ formatNumber(company.payroll.totalGross) }}</div>
              <div class="text-xs text-gray-500">planilla</div>
            </div>
          </div>
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
      <div v-if="largestCompany" class="mt-4 p-3 bg-blue-50 rounded-lg">
        <div class="flex items-center justify-between">
          <div>
            <div class="text-sm font-medium text-blue-800">Mayor costo:</div>
            <div class="font-semibold text-blue-900">{{ largestCompany.nombre }}</div>
          </div>
          <div class="text-right">
            <div class="text-lg font-bold text-blue-800">₡{{ formatNumber(largestCompany.payroll.totalGross) }}</div>
            <div class="text-sm text-blue-600">{{ getPercentage(largestCompany.payroll.totalGross) }}% del total</div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import apiConfig from '@/config/api';
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
    companiesWithData() {
      return this.companies.filter(company => company.payroll && company.payroll.totalGross > 0);
    },
    companiesWithoutData() {
      return this.companies.filter(company => !company.payroll || company.payroll.totalGross === 0);
    },
    totalPayroll() {
      return this.companies.reduce((sum, company) => sum + (company.payroll?.totalGross || 0), 0);
    },
    totalEmployees() {
      return this.companies.reduce((sum, company) => sum + (company.payroll?.employeeCount || 0), 0);
    },
    largestCompany() {
      if (this.companiesWithData.length === 0) return null;
      return this.companiesWithData.reduce((max, company) => 
        company.payroll.totalGross > max.payroll.totalGross ? company : max
      );
    },
    pieSegments() {
      if (this.companiesWithData.length === 0) return [];
      let accumulatedPercentage = 0;
      return this.companiesWithData.map((company, index) => {
        const percentage = this.getPercentage(company.payroll.totalGross);
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
        const endpoint = apiConfig.endpoints.payrollDistribution(this.userId);
        const response = await fetch(endpoint);
        if (!response.ok) throw new Error('Error al obtener la distribución de planilla');
        this.companies = await response.json();
      } catch (error) {
        console.error('Error fetching payroll data:', error);
        this.companies = [];
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
      return ((value / this.totalPayroll) * 100).toFixed(1);
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
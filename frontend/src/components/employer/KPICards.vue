<template>
  <div class="kpi-cards neumorphism-card p-6">
    <h2 class="text-xl font-bold mb-4">Métricas Clave</h2>
    <div class="space-y-4">
      <!-- Total Empresas -->
      <div class="kpi-item">
        <div class="kpi-icon bg-blue-100 text-blue-600">🏢</div>
        <div class="kpi-content">
          <div class="kpi-value">{{ metrics.totalCompanies }}</div>
          <div class="kpi-label">Empresas</div>
        </div>
      </div>
      <!-- Empleados Activos -->
      <div class="kpi-item">
        <div class="kpi-icon bg-green-100 text-green-600">👥</div>
        <div class="kpi-content">
          <div class="kpi-value">{{ metrics.totalActiveEmployees }}</div>
          <div class="kpi-label">Empleados Activos</div>
        </div>
      </div>
      <!-- Planilla Total -->
      <div class="kpi-item">
        <div class="kpi-icon bg-purple-100 text-purple-600">💰</div>
        <div class="kpi-content">
          <div class="kpi-value">₡{{ formatNumber(metrics.totalPayroll) }}</div>
          <div class="kpi-label">Planilla Mensual</div>
        </div>
      </div>
      <!-- Empresas con Planilla -->
      <div class="kpi-item">
        <div class="kpi-icon bg-orange-100 text-orange-600">📊</div>
        <div class="kpi-content">
          <div class="kpi-value">{{ metrics.companiesWithPayroll }}</div>
          <div class="kpi-label">Empresas Activas</div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import apiConfig from '@/config/api'

export default {
  name: 'KPICards',
  props: {
    userId: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      metrics: {
        totalCompanies: 0,
        totalActiveEmployees: 0,
        totalPayroll: 0,
        companiesWithPayroll: 0
      },
      loading: false
    }
  },
  methods: {
    async fetchKPIData() {
      this.loading = true;
      try {
        const employerId = localStorage.getItem('employerId');
        if (!employerId) throw new Error('No employerId found in localStorage');
        const response = await fetch(apiConfig.endpoints.kpi(employerId));
        if (!response.ok) throw new Error('Network response was not ok');
        const data = await response.json();
        this.metrics = {
          totalCompanies: data.totalCompanies,
          totalActiveEmployees: data.totalActiveEmployees,
          totalPayroll: data.totalPayroll,
          companiesWithPayroll: data.companiesWithPayroll
        };
      } catch (error) {
        console.error('Error fetching KPI data:', error);
      } finally {
        this.loading = false;
      }
    },
    formatNumber(value) {
      return value.toLocaleString('es-CR');
    }
  },
  mounted() {
    this.fetchKPIData();
  }
}
</script>

<style scoped>
.kpi-item {
  display: flex;
  align-items: center;
  padding: 12px;
  background: #f8fafc;
  border-radius: 8px;
  transition: background-color 0.2s;
}

.kpi-item:hover {
  background: #f1f5f9;
}

.kpi-icon {
  width: 40px;
  height: 40px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.2rem;
  margin-right: 12px;
}

.kpi-content {
  flex: 1;
}

.kpi-value {
  font-size: 1.5rem;
  font-weight: bold;
  color: #1f2937;
}

.kpi-label {
  font-size: 0.875rem;
  color: #6b7280;
}
</style>
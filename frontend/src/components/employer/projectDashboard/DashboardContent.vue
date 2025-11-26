<template>
  <div class="body">
    <div class="space-y-[18px]">
      <h1 class="text-4xl font-bold text-gray-800">Dashboard de Empresa</h1>
      <div class="w-full h-[10px] mt-2 rounded neumorphism-on-small-item"></div>
    </div>

    <!-- Loading State -->
    <div v-if="loadingDashboard" class="flex justify-center items-center py-12">
      <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded-2xl mb-6">
      <span>{{ error }}</span>
    </div>

    <!-- Dashboard Content -->
    <div v-else>
      <!-- Métricas Clave -->
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        <!-- Total Empleados -->
        <div class="neumorphism-card text-center">
          <div class="text-3xl font-bold text-blue-600 mb-2">{{ dashboardData.totalEmployees || 0 }}</div>
          <div class="text-gray-600 text-sm">Empleados Activos</div>
          <div class="text-green-500 text-xs mt-1">{{ dashboardData.employeesTrend || '+0% vs mes anterior' }}</div>
        </div>

        <!-- Planilla del Mes -->
        <div class="neumorphism-card text-center">
          <div class="text-3xl font-bold text-green-600 mb-2">₡{{ (dashboardData.currentPayroll || 0).toLocaleString() }}</div>
          <div class="text-gray-600 text-sm">Planilla Actual</div>
          <div class="text-red-500 text-xs mt-1">{{ dashboardData.payrollTrend || '+0% vs mes anterior' }}</div>
        </div>

        <!-- Departamentos -->
        <div class="neumorphism-card text-center">
          <div class="text-3xl font-bold text-purple-600 mb-2">{{ dashboardData.activeDepartments || 0 }}</div>
          <div class="text-gray-600 text-sm">Departamentos</div>
          <div class="text-gray-400 text-xs mt-1">{{ dashboardData.departmentsTrend || 'Sin cambios' }}</div>
        </div>

        <!-- Tareas Pendientes -->
        <div class="neumorphism-card text-center">
          <div class="text-3xl font-bold text-orange-600 mb-2">{{ dashboardData.pendingTasks || 0 }}</div>
          <div class="text-gray-600 text-sm">Tareas Pendientes</div>
          <div class="text-blue-500 text-xs mt-1">{{ dashboardData.tasksTrend || '0 nuevas hoy' }}</div>
        </div>
      </div>

      <!-- Información de la Empresa y Acciones Rápidas -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-8 mb-8">
        <!-- Información de la Empresa -->
        <div class="neumorphism-card p-6 rounded-2xl">
          <h2 class="text-xl font-semibold mb-4">Información de la Empresa</h2>
          <div class="space-y-2">
            <p class="text-gray-700"><span class="font-bold">Nombre:</span> {{ project.nombre || 'N/A' }}</p>
            <p class="text-gray-700"><span class="font-bold">Cédula Jurídica:</span> {{ project.cedulaJuridica || 'N/A' }}</p>
            <p class="text-gray-700"><span class="font-bold">Período de Pago:</span> {{ project.periodoPago || 'N/A' }}</p>
            <p class="text-gray-700"><span class="font-bold">Email:</span> {{ project.email || 'N/A' }}</p>
            <p class="text-gray-700"><span class="font-bold">Teléfono:</span> {{ project.telefono || 'N/A' }}</p>
            <p class="text-gray-700">
              <span class="font-bold">Dirección:</span> 
              <span v-if="loadingDirection">Cargando...</span>
              <span v-else>{{ projectDirection || 'N/A' }}</span>
            </p>
          </div>
        </div>

        <!-- Acciones Rápidas -->
        <div class="neumorphism-card p-6 rounded-2xl">
          <h2 class="text-xl font-semibold mb-4">Acciones Rápidas</h2>
          <div class="grid grid-cols-2 gap-4">
            <button 
              @click="$emit('section-change', 'employees')" 
              class="neumorphism-button-normal-light p-4! rounded-xl! text-center w-full h-full"
            >
              <div class="text-2xl mb-2">👥</div>
              <div class="text-sm font-medium">Gestionar Empleados</div>
            </button>

            <button 
              @click="$emit('section-change', 'reports')" 
              class="neumorphism-button-normal-light p-4! rounded-xl! text-center w-full h-full"
            >
              <div class="text-2xl mb-2">📊</div>
              <div class="text-sm font-medium">Ver Reportes</div>
            </button>

            <button 
              @click="$emit('section-change', 'benefits')" 
              class="neumorphism-button-normal-light p-4! rounded-xl! text-center w-full h-full"
            >
              <div class="text-2xl mb-2">🎁</div>
              <div class="text-sm font-medium">Gestionar Beneficios</div>
            </button>

            <button 
              @click="$emit('section-change', 'info')" 
              class="neumorphism-button-normal-light p-4! rounded-xl! text-center w-full h-full"
            >
              <div class="text-2xl mb-2">⚙️</div>
              <div class="text-sm font-medium">Configurar Empresa</div>
            </button>
          </div>
        </div>
      </div>

      <!-- Gráficos -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-8 mb-8">
        <!-- Distribución por Departamentos -->
        <div class="neumorphism-card p-6 rounded-2xl">
          <h3 class="text-lg font-semibold mb-4">Empleados por Departamento</h3>
          <div v-if="departmentStats.length > 0" class="flex flex-col items-center">
            <!-- Gráfico de Pastel CSS -->
            <div class="pie-chart mb-4" :style="pieChartStyle">
              <div 
                v-for="(segment, index) in pieSegments" 
                :key="index"
                class="pie-segment"
                :style="segment.style"
              ></div>
            </div>
            <!-- Leyenda -->
            <div class="flex flex-wrap justify-center gap-3">
              <div 
                v-for="(dept, index) in departmentStats" 
                :key="dept.name" 
                class="flex items-center gap-1 text-sm"
              >
                <div 
                  class="w-3 h-3 rounded-full" 
                  :style="{ backgroundColor: departmentColors[index] }"
                ></div>
                <span>{{ dept.name }} ({{ dept.count }})</span>
              </div>
            </div>
          </div>
          <div v-else class="h-64 flex items-center justify-center border-2 border-dashed border-gray-300 rounded-lg">
            <div class="text-center">
              <div class="text-4xl mb-2">📊</div>
              <div class="text-gray-500">Sin datos de departamentos</div>
            </div>
          </div>
        </div>

        <!-- Planilla -->
        <div class="neumorphism-card p-6 rounded-2xl">
          <h3 class="text-lg font-semibold mb-4">Resumen de Planilla</h3>
          <div class="space-y-4">
            <div class="flex justify-between items-center">
              <span class="text-gray-700">Salario Bruto Total:</span>
              <span class="font-bold text-green-600">₡{{ (dashboardData.currentPayroll || 0).toLocaleString() }}</span>
            </div>
            <div class="flex justify-between items-center">
              <span class="text-gray-700">Promedio por Empleado:</span>
              <span class="font-bold text-blue-600">
                ₡{{ dashboardData.totalEmployees > 0 ? Math.round((dashboardData.currentPayroll || 0) / dashboardData.totalEmployees).toLocaleString() : '0' }}
              </span>
            </div>
            <div class="flex justify-between items-center">
              <span class="text-gray-700">Total Empleados:</span>
              <span class="font-bold text-purple-600">{{ dashboardData.totalEmployees || 0 }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { apiConfig } from '../../../config/api.js'

export default {
  name: 'DashboardContent',
  props: {
    project: {
      type: Object,
      default: () => ({})
    }
  },
  emits: ['section-change'],
  data() {
    return {
      dashboardData: {
        totalEmployees: 0,
        currentPayroll: 0,
        activeDepartments: 0,
        pendingTasks: 0,
        employeesTrend: '+0% vs mes anterior',
        payrollTrend: '+0% vs mes anterior',
        departmentsTrend: 'Sin cambios',
        tasksTrend: '0 nuevas hoy'
      },
      departmentStats: [],
      projectDirection: null,
      loadingDirection: false,
      loadingDashboard: false,
      error: null,
      departmentColors: [
        '#3B82F6', // blue-500
        '#10B981', // emerald-500  
        '#F59E0B', // amber-500
        '#EF4444', // red-500
        '#8B5CF6', // violet-500
        '#06B6D4', // cyan-500
        '#84CC16', // lime-500
        '#F97316'  // orange-500
      ]
    }
  },
  computed: {
    pieSegments() {
      if (this.departmentStats.length === 0) return []
      
      const total = this.departmentStats.reduce((sum, dept) => sum + dept.count, 0)
      let currentAngle = 0
      
      return this.departmentStats.map((dept, index) => {
        const angle = (dept.count / total) * 360
        
        const segment = {
          style: {
            '--start-angle': `${currentAngle}deg`,
            '--angle': `${angle}deg`,
            '--color': this.departmentColors[index % this.departmentColors.length],
            transform: `rotate(${currentAngle}deg)`,
            background: `conic-gradient(from 0deg, ${this.departmentColors[index % this.departmentColors.length]} ${angle}deg, transparent ${angle}deg)`
          }
        }
        
        currentAngle += angle
        return segment
      })
    },
    
    pieChartStyle() {
      if (this.departmentStats.length === 0) return {}
      
      const total = this.departmentStats.reduce((sum, dept) => sum + dept.count, 0)
      const gradientParts = []
      let currentPercentage = 0
      
      this.departmentStats.forEach((dept, index) => {
        const percentage = (dept.count / total) * 100
        const color = this.departmentColors[index % this.departmentColors.length]
        
        gradientParts.push(`${color} ${currentPercentage}% ${currentPercentage + percentage}%`)
        currentPercentage += percentage
      })
      
      return {
        background: `conic-gradient(${gradientParts.join(', ')})`
      }
    }
  },
  watch: {
    project: {
      handler(newProject) {
        if (newProject && newProject.id) {
          this.fetchDashboardData()
        }
        if (newProject && newProject.idDireccion) {
          this.fetchDirection()
        }
      },
      deep: true,
      immediate: true
    }
  },
  methods: {
    async fetchDashboardData() {
      if (!this.project || !this.project.id) return
      try {
        this.loadingDashboard = true
        this.error = null
        const response = await fetch(apiConfig.endpoints.dashboardMetrics(this.project.id))
        if (!response.ok) {
          throw new Error('No se pudieron cargar las métricas del dashboard')
        }
        const data = await response.json()
        this.dashboardData = { ...this.dashboardData, ...data }
        await this.fetchEmployeeStats()
      } catch (err) {
        this.error = err.message || 'Error al cargar el dashboard'        
        // Fallback
        this.dashboardData = {
          totalEmployees: this.project.activeEmployees || 0,
          currentPayroll: this.project.monthlyPayroll || 0,
          activeDepartments: 1,
          pendingTasks: 0,
        }
        await this.fetchEmployeeStats()
      } finally {
        this.loadingDashboard = false
      }
    },

    async fetchEmployeeStats() {
      try {
        const response = await fetch(apiConfig.endpoints.projectEmployees(this.project.id))
        if (response.ok) {
          const data = await response.json()
          const employees = data.employees || []
          const activeEmployees = employees.filter(emp => emp.estado === 'Activo')
          const departmentGroups = activeEmployees.reduce((acc, emp) => {
            const dept = emp.departamento || 'Sin Departamento'
            acc[dept] = (acc[dept] || 0) + 1
            return acc
          }, {})
          this.departmentStats = Object.entries(departmentGroups)
            .map(([name, count]) => ({ name, count }))
            .sort((a, b) => b.count - a.count)
        }
      } catch (err) {
        this.departmentStats = []
      }
    },

    async fetchDirection() {
      if (!this.project.idDireccion) {
        this.projectDirection = null
        return
      }
      try {
        this.loadingDirection = true
        const response = await fetch(apiConfig.endpoints.projectDirection(this.project.idDireccion)) 
        if (!response.ok) {
          throw new Error(`Error ${response.status}: No se pudo cargar la dirección`)
        }
        const direction = await response.json()
        this.projectDirection = this.formatAddress(direction)
        console.log('Direction loaded for project:', this.project.id, direction)
      } catch (err) {
        this.projectDirection = 'Error al cargar dirección'
      } finally {
        this.loadingDirection = false
      }
    },

    formatAddress(direction) {
      if (!direction) return null
      const parts = []
      if (direction.provincia) parts.push(direction.provincia)
      if (direction.canton) parts.push(direction.canton)  
      if (direction.distrito) parts.push(direction.distrito)
      if (direction.direccionParticular) parts.push(direction.direccionParticular)
      return parts.length > 0 ? parts.join(', ') : null
    }
  }
}
</script>

<style scoped>
.pie-chart {
  width: 200px;
  height: 200px;
  border-radius: 50%;
  position: relative;
  overflow: hidden;
}
</style>
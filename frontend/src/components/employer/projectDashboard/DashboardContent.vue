<template>
  <div class="body">
    <div class="space-y-[18px]">
      <h1 class="text-4xl font-bold text-gray-800">Dashboard de Empresa</h1>
      <div class="w-full h-[10px] mt-2 rounded neumorphism-on-small-item"></div>
    </div>

    <!-- Métricas Clave -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
      <!-- Total Empleados -->
      <div class="neumorphism-card text-center">
        <div class="text-3xl font-bold text-blue-600 mb-2">{{ dashboardData.totalEmployees || 0 }}</div>
        <div class="text-gray-600 text-sm">Empleados Activos</div>
      </div>

      <!-- Planilla del Mes -->
      <div class="neumorphism-card text-center">
        <div class="text-3xl font-bold text-green-600 mb-2">₡{{ (dashboardData.currentPayroll || 0).toLocaleString() }}</div>
        <div class="text-gray-600 text-sm">Planilla Actual</div>
      </div>

      <!-- Departamentos -->
      <div class="neumorphism-card text-center">
        <div class="text-3xl font-bold text-purple-600 mb-2">{{ dashboardData.activeDepartments || 0 }}</div>
        <div class="text-gray-600 text-sm">Departamentos</div>
      </div>

      <!-- Tareas Pendientes -->
      <div class="neumorphism-card text-center">
        <div class="text-3xl font-bold text-orange-600 mb-2">{{ dashboardData.notifications || 0 }}</div>
        <div class="text-gray-600 text-sm">Notificaciones</div>
      </div>
    </div>

    <!-- Información de la Empresa y Acciones Rápidas -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-8">
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

      <div class="neumorphism-card p-6 rounded-2xl">
        <h3 class="text-lg font-semibold mb-4">Empleados por Departamento</h3>
        <div class="h-64 flex items-center justify-center border-2 border-dashed border-gray-300 rounded-lg">
          <div class="text-center">
            <div class="text-4xl mb-2">📊</div>
            <div class="text-gray-500">Gráfico de Donut</div>
            <div class="text-xs text-gray-400 mt-1">Próximamente</div>
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
      dashboardData: {},
      projectDirection: null,
      loadingDirection: false
    }
  },
  watch: {
    project: {
      handler(newProject) {
        if (newProject && newProject.idDireccion) {
          this.fetchDirection()
        }
      },
      deep: true,
      immediate: true
    }
  },
  methods: {
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
        
        // Format the complete address
        this.projectDirection = this.formatAddress(direction)
        
        console.log('Direction loaded for project:', this.project.id, direction)
      } catch (err) {
        console.error('Error loading direction:', err)
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
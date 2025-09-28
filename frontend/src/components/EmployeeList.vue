<template>
  <div class="min-h-screen bg-[#dbeafe] py-8">
    <div class="max-w-7xl mx-auto px-4">
      <div class="bg-[#eaf4fa] rounded-[40px] shadow-2xl p-10">
        <!-- Header -->
        <div class="text-center mb-8">
          <h1 class="text-5xl font-black mb-2 text-black tracking-wide">Lista de Empleados</h1>
          <p class="text-gray-600 text-lg">Gestiona y consulta la información de tus empleados</p>
        </div>

        <!-- Search and Filter Controls -->
        <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
          <!-- Search Input -->
          <div>
            <label class="block mb-1 font-medium text-gray-700">Buscar empleado</label>
            <input
              v-model="searchTerm"
              @input="searchEmployees"
              type="text"
              placeholder="Buscar por nombre o email..."
              class="w-full bg-white rounded-full shadow-inner px-4 py-2 outline-none text-gray-700 placeholder-gray-400"
            />
          </div>

          <!-- Status Filter -->
          <div>
            <label class="block mb-1 font-medium text-gray-700">Filtrar por estado</label>
            <select
              v-model="statusFilter"
              @change="filterEmployees"
              class="w-full bg-white rounded-full shadow-inner px-4 py-2 outline-none text-gray-700"
            >
              <option value="">Todos los estados</option>
              <option value="Activa">Activa</option>
              <option value="Pendiente">Pendiente</option>
              <option value="Desactivada">Desactivada</option>
            </select>
          </div>

          <!-- Sort Options -->
          <div>
            <label class="block mb-1 font-medium text-gray-700">Ordenar por</label>
            <select
              v-model="sortBy"
              @change="sortEmployees"
              class="w-full bg-white rounded-full shadow-inner px-4 py-2 outline-none text-gray-700"
            >
              <option value="nombre">Nombre (A-Z)</option>
              <option value="email">Email</option>
              <option value="puesto">Puesto</option>
              <option value="salario">Salario</option>
              <option value="estado">Estado</option>
            </select>
          </div>
        </div>

        <!-- Loading State -->
        <div v-if="loading" class="text-center py-12">
          <div class="inline-block animate-spin rounded-full h-12 w-12 border-b-4 border-[#2d384b]"></div>
          <p class="mt-4 text-gray-600 text-lg">Cargando empleados...</p>
        </div>

        <!-- Error State -->
        <div v-else-if="error" class="bg-red-100 border-2 border-red-300 rounded-[20px] p-6 mb-6">
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <svg class="h-6 w-6 text-red-500" viewBox="0 0 20 20" fill="currentColor">
                <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd" />
              </svg>
            </div>
            <div class="ml-3">
              <h3 class="text-lg font-semibold text-red-800">Error al cargar empleados</h3>
              <p class="mt-1 text-red-700">{{ error }}</p>
            </div>
          </div>
        </div>

        <!-- Employee List -->
        <div v-else>
          <!-- Results Summary -->
          <div class="text-center mb-6">
            <p class="text-gray-600 text-lg font-medium">
              {{ employeeData.message || `Mostrando ${employeeData.totalCount || 0} empleados` }}
            </p>
          </div>

          <!-- No Employees State -->
          <div v-if="!employeeData.hasEmployees" class="text-center py-16">
            <svg class="mx-auto h-16 w-16 text-gray-400 mb-4" stroke="currentColor" fill="none" viewBox="0 0 48 48">
              <path d="M34 40h10v-4a6 6 0 00-10.712-3.714M34 40H14m20 0v-4a9.971 9.971 0 00-.712-3.714M14 40H4v-4a6 6 0 0110.713-3.714M14 40v-4c0-1.313.253-2.566.713-3.714m0 0A10.003 10.003 0 0124 26c4.21 0 7.813 2.602 9.288 6.286M30 14a6 6 0 11-12 0 6 6 0 0112 0zm12 6a4 4 0 11-8 0 4 4 0 018 0zm-28 0a4 4 0 11-8 0 4 4 0 018 0z" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
            </svg>
            <h3 class="text-2xl font-semibold text-gray-700 mb-2">No hay empleados registrados</h3>
            <p class="text-gray-500 text-lg">Comienza agregando algunos empleados a tu empresa.</p>
          </div>

          <!-- Employee Cards Grid -->
          <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            <div 
              v-for="employee in employeeData.employees" 
              :key="employee.id" 
              class="bg-white rounded-[20px] shadow-lg p-6 hover:shadow-xl transition-all duration-300"
            >
              <!-- Employee Avatar and Name -->
              <div class="flex items-center mb-4">
                <div class="flex-shrink-0 h-12 w-12 bg-[#2d384b] rounded-full flex items-center justify-center shadow-inner">
                  <span class="text-lg font-bold text-white">
                    {{ getInitials(employee.nombreCompleto) }}
                  </span>
                </div>
                <div class="ml-4">
                  <h3 class="text-lg font-bold text-gray-800">{{ employee.nombreCompleto }}</h3>
                  <span :class="getStatusBadgeClass(employee.estado)" class="inline-flex px-3 py-1 text-xs font-semibold rounded-full">
                    {{ employee.estado }}
                  </span>
                </div>
              </div>

              <!-- Employee Details -->
              <div class="space-y-3">
                <div class="flex items-center">
                  <svg class="h-4 w-4 text-gray-500 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 8l7.89 7.89a2 2 0 002.83 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" />
                  </svg>
                  <span class="text-sm text-gray-600">{{ employee.email }}</span>
                </div>

                <div class="flex items-center">
                  <svg class="h-4 w-4 text-gray-500 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z" />
                  </svg>
                  <span class="text-sm text-gray-600">{{ employee.celular }}</span>
                </div>

                <div class="flex items-center">
                  <svg class="h-4 w-4 text-gray-500 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 13.255A23.931 23.931 0 0112 15c-3.183 0-6.22-.62-9-1.745M16 6V4a2 2 0 00-2-2h-4a2 2 0 00-2-2v2m8 0V6a2 2 0 012 2v6a2 2 0 01-2 2H8a2 2 0 01-2-2V8a2 2 0 012-2h8z" />
                  </svg>
                  <span class="text-sm text-gray-600">{{ employee.puesto }}</span>
                </div>

                <div class="flex items-center justify-between pt-2 border-t border-gray-200">
                  <div class="flex items-center">
                    <svg class="h-4 w-4 text-green-500 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                    </svg>
                    <span class="text-sm font-semibold text-gray-700">₡{{ formatCurrency(employee.salario) }}</span>
                  </div>
                  
                  <div class="text-xs text-gray-500">
                    {{ formatDate(employee.fechaContratacion) }}
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import axios from 'axios'

export default {
  name: 'EmployeeList',
  data() {
    return {
      employeeData: {
        employees: [],
        totalCount: 0,
        message: '',
        hasEmployees: false
      },
      searchTerm: '',
      statusFilter: '',
      sortBy: 'nombre',
      loading: false,
      error: null,
      employerId: 1 // TODO: Get from authentication/route params
    }
  },
  async mounted() {
    await this.loadEmployees()
  },
  methods: {
    async loadEmployees() {
      this.loading = true
      this.error = null
      
      try {
        // Call the PlaniFy database endpoint instead of the in-memory one
        const url = `http://localhost:5011/api/Employee/planify/all`
        
        console.log('Loading employees from PlaniFy database:', url)
        
        const response = await axios.get(url)
        
        // Transform the response to match the expected format
        const transformedData = {
          employees: response.data.empleados.map(emp => ({
            id: emp.idPersona,
            nombreCompleto: emp.persona.nombreCompleto,
            email: emp.persona.correo,
            celular: emp.persona.telefono ? emp.persona.telefono.toString() : 'N/A',
            puesto: emp.puesto,
            salario: emp.salario,
            estado: 'Activa', // Default since PlaniFy doesn't have status field
            fechaContratacion: emp.fechaContratacion
          })),
          totalCount: response.data.totalCount,
          message: response.data.message,
          hasEmployees: response.data.totalCount > 0
        }
        
        this.employeeData = transformedData
        
        console.log('Employees loaded from PlaniFy:', this.employeeData)
      } catch (error) {
        console.error('Error loading employees:', error)
        
        if (error.response) {
          // Server responded with error status
          this.error = error.response.data?.message || `Error del servidor: ${error.response.status}`
        } else if (error.request) {
          // Request was made but no response
          this.error = 'No se pudo conectar con el servidor. Verifica que el backend esté funcionando en http://localhost:5011'
        } else {
          // Something else happened
          this.error = 'Error al cargar la lista de empleados'
        }
        
        this.employeeData = { employees: [], totalCount: 0, message: '', hasEmployees: false }
      } finally {
        this.loading = false
      }
    },
    
    async searchEmployees() {
      // Debounce search
      clearTimeout(this.searchTimeout)
      this.searchTimeout = setTimeout(() => {
        this.loadEmployees()
      }, 300)
    },
    
    async filterEmployees() {
      await this.loadEmployees()
    },
    
    async sortEmployees() {
      await this.loadEmployees()
    },
    
    getInitials(name) {
      return name
        .split(' ')
        .map(n => n[0])
        .join('')
        .toUpperCase()
        .slice(0, 2)
    },
    
    formatCurrency(amount) {
      return new Intl.NumberFormat('es-CR').format(amount)
    },
    
    formatDate(dateString) {
      const date = new Date(dateString)
      return date.toLocaleDateString('es-CR', {
        year: 'numeric',
        month: 'short',
        day: 'numeric'
      })
    },
    
    getStatusBadgeClass(status) {
      const baseClasses = 'inline-flex px-2 py-1 text-xs font-semibold rounded-full'
      
      switch (status) {
        case 'Activa':
          return `${baseClasses} bg-green-100 text-green-800`
        case 'Pendiente':
          return `${baseClasses} bg-yellow-100 text-yellow-800`
        case 'Desactivada':
          return `${baseClasses} bg-red-100 text-red-800`
        default:
          return `${baseClasses} bg-gray-100 text-gray-800`
      }
    }
  }
}
</script>

<style scoped>
/* Style similar to SignUpEmployer component */
.employee-list-container {
  font-family: 'Inter', 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}

/* Enhance the card hover effect */
.bg-white:hover {
  transform: translateY(-2px);
}

/* Custom scrollbar for better visual consistency */
::-webkit-scrollbar {
  width: 8px;
}

::-webkit-scrollbar-track {
  background: #f1f1f1;
  border-radius: 10px;
}

::-webkit-scrollbar-thumb {
  background: #2d384b;
  border-radius: 10px;
}

::-webkit-scrollbar-thumb:hover {
  background: #1e293b;
}
</style>

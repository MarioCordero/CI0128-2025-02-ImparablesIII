<template>
  <div class="min-h-screen bg-[#dbeafe] p-4">
    <div class="max-w-7xl mx-auto">
      <!-- Header -->
      <div class="bg-[#eaf4fa] rounded-[40px] shadow-2xl p-8 mb-8">
        <div class="flex items-center justify-between">
          <div class="flex items-center space-x-4">
            <span class="text-6xl">👥</span>
            <div>
              <h1 class="text-4xl font-black text-black tracking-wide">PlaniFy</h1>
              <h2 class="text-xl font-medium text-gray-700">Panel de Super Administrador</h2>
              <div v-if="user" class="text-sm text-gray-600 mt-1">
                Bienvenido, {{ user.Nombre }} {{ user.Apellidos }}
              </div>
            </div>
          </div>
          <div class="flex items-center space-x-3">
            <button
              @click="goBack"
              class="bg-[#87ceeb] text-white px-6 py-3 rounded-full shadow-lg hover:shadow-xl transition-all flex items-center space-x-2"
            >
              <svg class="w-5 h-5" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" d="M10 19l-7-7m0 0l7-7m-7 7h18"/>
              </svg>
              <span>Volver</span>
            </button>
            <button
              @click="logout"
              class="bg-red-500 text-white px-6 py-3 rounded-full shadow-lg hover:shadow-xl transition-all flex items-center space-x-2"
            >
              <svg class="w-5 h-5" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1"/>
              </svg>
              <span>Cerrar Sesión</span>
            </button>
          </div>
        </div>
      </div>

      <!-- Loading State -->
      <div v-if="loading" class="flex justify-center items-center py-12">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-[#87ceeb]"></div>
      </div>

      <!-- Error State -->
      <div v-else-if="error" class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded-2xl mb-6">
        <div class="flex items-center">
          <svg class="w-5 h-5 mr-2" fill="currentColor" viewBox="0 0 20 20">
            <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd"/>
          </svg>
          <span>{{ error }}</span>
        </div>
      </div>

      <!-- Businesses List -->
      <div v-else class="space-y-6">
        <div class="bg-[#eaf4fa] rounded-[40px] shadow-2xl p-6">
          <h3 class="text-2xl font-semibold text-gray-700 mb-6 text-center">
            Empresas Registradas ({{ empresas.length }})
          </h3>
          
          <div v-if="empresas.length === 0" class="text-center py-8">
            <svg class="w-16 h-16 mx-auto text-gray-400 mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4"/>
            </svg>
            <p class="text-gray-500 text-lg">No hay empresas registradas</p>
          </div>

          <div v-else class="grid gap-6">
            <div
              v-for="empresa in empresas"
              :key="empresa.id"
              class="bg-white rounded-2xl shadow-lg hover:shadow-xl transition-all duration-300 overflow-hidden"
            >
              <!-- Business Header -->
              <div class="p-6 border-b border-gray-100">
                <div class="flex items-start justify-between">
                  <div class="flex-1">
                    <h4 class="text-xl font-bold text-gray-800 mb-2">{{ empresa.nombre }}</h4>
                    <div class="grid grid-cols-1 md:grid-cols-2 gap-4 text-sm text-gray-600">
                      <div class="flex items-center space-x-2">
                        <svg class="w-4 h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"/>
                        </svg>
                        <span><strong>Cédula Jurídica:</strong> {{ empresa.cedulaJuridica }}</span>
                      </div>
                      <div class="flex items-center space-x-2">
                        <svg class="w-4 h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 8l7.89 4.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"/>
                        </svg>
                        <span><strong>Email:</strong> {{ empresa.email }}</span>
                      </div>
                      <div class="flex items-center space-x-2">
                        <svg class="w-4 h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3a2 2 0 012-2h4a2 2 0 012 2v4m-6 0h6m-6 0a2 2 0 00-2 2v6a2 2 0 002 2h6a2 2 0 002-2V9a2 2 0 00-2-2m-6 0V7"/>
                        </svg>
                        <span><strong>Período de Pago:</strong> {{ empresa.periodoPago }}</span>
                      </div>
                      <div v-if="empresa.telefono" class="flex items-center space-x-2">
                        <svg class="w-4 h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z"/>
                        </svg>
                        <span><strong>Teléfono:</strong> {{ empresa.telefono }}</span>
                      </div>
                    </div>
                    
                    <!-- Address Information -->
                    <div v-if="empresa.direccion" class="mt-4 p-4 bg-gray-50 rounded-xl">
                      <h5 class="font-semibold text-gray-700 mb-2 flex items-center">
                        <svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z"/>
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z"/>
                        </svg>
                        Dirección
                      </h5>
                      <p class="text-sm text-gray-600">
                        {{ empresa.direccion.direccionParticular }}, {{ empresa.direccion.distrito }}, 
                        {{ empresa.direccion.canton }}, {{ empresa.direccion.provincia }}
                      </p>
                    </div>
                  </div>
                  
                  <!-- Toggle Benefits Button -->
                  <button
                    @click="toggleBenefits(empresa.id)"
                    class="bg-[#87ceeb] text-white px-4 py-2 rounded-full shadow-lg hover:shadow-xl transition-all flex items-center space-x-2"
                  >
                    <span>{{ expandedEmpresas.has(empresa.id) ? 'Ocultar' : 'Ver' }} Beneficios</span>
                    <svg 
                      class="w-4 h-4 transition-transform duration-200"
                      :class="{ 'rotate-180': expandedEmpresas.has(empresa.id) }"
                      fill="none" 
                      stroke="currentColor" 
                      viewBox="0 0 24 24"
                    >
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"/>
                    </svg>
                  </button>
                </div>
              </div>

              <!-- Benefits Section -->
              <div v-if="expandedEmpresas.has(empresa.id)" class="p-6 bg-gray-50">
                <h5 class="font-semibold text-gray-700 mb-4 flex items-center">
                  <svg class="w-5 h-5 mr-2 text-green-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"/>
                  </svg>
                  Beneficios ({{ empresa.beneficios?.length || 0 }})
                </h5>
                
                <div v-if="!empresa.beneficios || empresa.beneficios.length === 0" class="text-center py-6">
                  <svg class="w-12 h-12 mx-auto text-gray-400 mb-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"/>
                  </svg>
                  <p class="text-gray-500">No hay beneficios registrados para esta empresa</p>
                </div>
                
                <div v-else class="grid gap-3">
                  <div
                    v-for="beneficio in empresa.beneficios"
                    :key="`${beneficio.idEmpresa}-${beneficio.nombre}`"
                    class="bg-white rounded-xl p-4 shadow-sm hover:shadow-md transition-all"
                  >
                    <div class="flex items-center justify-between">
                      <div class="flex-1">
                        <h6 class="font-semibold text-gray-800">{{ beneficio.nombre }}</h6>
                        <div class="flex items-center space-x-4 mt-2 text-sm text-gray-600">
                          <span class="flex items-center">
                            <svg class="w-4 h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 7h.01M7 3h5c.512 0 1.024.195 1.414.586l7 7a2 2 0 010 2.828l-7 7a2 2 0 01-2.828 0l-7-7A1.994 1.994 0 013 12V7a4 4 0 014-4z"/>
                            </svg>
                            <strong>Tipo:</strong> {{ beneficio.tipo }}
                          </span>
                          <span class="flex items-center">
                            <svg class="w-4 h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 7h6m0 10v-3m-3 3h.01M9 17h.01M9 14h.01M12 14h.01M15 11h.01M12 11h.01M9 11h.01M7 21h10a2 2 0 002-2V5a2 2 0 00-2-2H7a2 2 0 00-2 2v14a2 2 0 002 2z"/>
                            </svg>
                            <strong>Cálculo:</strong> {{ beneficio.tipoCalculo }}
                          </span>
                        </div>
                      </div>
                    </div>
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
export default {
  name: 'SuperAdminMenu',
  data() {
    return {
      empresas: [],
      loading: true,
      error: null,
      expandedEmpresas: new Set(),
      user: null
    }
  },
  mounted() {
    // Check authentication first
    if (this.checkAuthentication()) {
      this.fetchEmpresas();
    }
  },
  methods: {
    checkAuthentication() {
      const userData = localStorage.getItem('user');
      const token = localStorage.getItem('token');

      console.log('User data:', userData);
      console.log('Token:', token);
      
      if (!userData || !token) {
        this.$router.push('/login');
        return false;
      }
      
      try {
        this.user = JSON.parse(userData);
        
        // Double-check user role
        if (this.user.tipoUsuario !== 'Administrador') {
          this.$router.push('/');
          return false;
        }
        
        return true;
      } catch (error) {
        localStorage.removeItem('user');
        localStorage.removeItem('token');
        this.$router.push('/login');
        return false;
      }
    },
    goBack() {
      this.$router.push('/');
    },
    logout() {
      localStorage.removeItem('user');
      localStorage.removeItem('token');
      this.$router.push('/login');
    },
    toggleBenefits(empresaId) {
      if (this.expandedEmpresas.has(empresaId)) {
        this.expandedEmpresas.delete(empresaId);
      } else {
        this.expandedEmpresas.add(empresaId);
      }
    },
    async fetchEmpresas() {
      try {
        this.loading = true;
        this.error = null;
        
        const response = await fetch('http://localhost:5011/api/Project');
        
        if (!response.ok) {
          throw new Error(`Error ${response.status}: ${response.statusText}`);
        }
        
        const data = await response.json(); 
        this.empresas = data;
      } catch (err) {
        console.error('Error fetching empresas:', err);
        this.error = 'Error al cargar las empresas. Por favor, intente nuevamente.';
      } finally {
        this.loading = false;
      }
    }
  },
  
}
</script>

<style scoped>
/* Custom neumorphic shadow effects */
.shadow-inner {
  box-shadow: inset 2px 2px 4px rgba(0, 0, 0, 0.1), inset -2px -2px 4px rgba(255, 255, 255, 0.8);
}

.shadow-lg {
  box-shadow: 8px 8px 16px rgba(0, 0, 0, 0.1), -8px -8px 16px rgba(255, 255, 255, 0.8);
}

.shadow-xl {
  box-shadow: 12px 12px 24px rgba(0, 0, 0, 0.15), -12px -12px 24px rgba(255, 255, 255, 0.8);
}

/* Smooth transitions */
.transition-all {
  transition: all 0.3s ease;
}

/* Custom scrollbar for benefits section */
.bg-gray-50::-webkit-scrollbar {
  width: 6px;
}

.bg-gray-50::-webkit-scrollbar-track {
  background: #f1f1f1;
  border-radius: 3px;
}

.bg-gray-50::-webkit-scrollbar-thumb {
  background: #87ceeb;
  border-radius: 3px;
}

.bg-gray-50::-webkit-scrollbar-thumb:hover {
  background: #5fb3d3;
}
</style>

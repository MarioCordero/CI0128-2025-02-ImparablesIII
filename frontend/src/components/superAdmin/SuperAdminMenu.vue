<template>
  <div class="page">
    <SuperAdminHeader />

    <div class="body mt-12">
      <!-- Loading State -->
      <div v-if="loading" class="flex justify-center items-center py-12">
        <div class="animate-spin rounded-full h-12 w-12 border-4 border-blue-500 border-t-transparent"></div>
      </div>
  
      <!-- Error State -->
      <div v-else-if="error" class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded-2xl mb-6 neumorphism-card">
        <div class="flex items-center">
          <svg class="w-5 h-5 mr-2" fill="currentColor" viewBox="0 0 20 20">
            <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd"/>
          </svg>
          <span>{{ error }}</span>
        </div>
      </div>
  
      <!-- Businesses List -->
      <div v-else class="neumorphism-card-modal h-[75vh]! w-full! flex flex-col gap-3">
        
        <div>
          <h2 class="text-2xl font-semibold text-gray-700">
            Empresas Registradas ({{ empresas.length }})
          </h2>
        </div>
        
        
        <div v-if="empresas.length === 0" class="text-center py-8">
          <svg class="w-16 h-16 mx-auto text-gray-400 mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4"/>
          </svg>
          <p class="text-gray-500 text-lg">No hay empresas registradas</p>
        </div>

        <!-- Contenedor con scroll -->
        <div v-else class="scrollable-card h-[60vh]! p-6">
          <div
            v-for="empresa in empresas"
            :key="empresa.id"
            class="neumorphism-card mb-[10px]! ml-[10px]!"
          >
            <!-- Business Header -->
            <div class="flex items-start justify-between mb-4">
              <div class="flex-1">
                <h4 class="text-xl font-bold text-gray-800 mb-3">{{ empresa.nombre }}</h4>
                <div class="grid grid-cols-1 md:grid-cols-2 gap-3 text-sm text-gray-600">
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
                class="neumorphism-button-normal-blue flex items-center space-x-2 ml-4"
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

            <!-- Benefits Section -->
            <div v-if="expandedEmpresas.has(empresa.id)" class="mt-4 pt-4 border-t border-gray-200">
              <h5 class="font-semibold text-gray-700 mb-3 flex items-center">
                <svg class="w-5 h-5 mr-2 text-green-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"/>
                </svg>
                Beneficios ({{ empresa.beneficios?.length || 0 }})
              </h5>
              
              <div v-if="!empresa.beneficios || empresa.beneficios.length === 0" class="text-center py-4">
                <svg class="w-12 h-12 mx-auto text-gray-400 mb-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"/>
                </svg>
                <p class="text-gray-500">No hay beneficios registrados para esta empresa</p>
              </div>
              
              <div v-else class="space-y-2">
                <div
                  v-for="beneficio in empresa.beneficios"
                  :key="`${beneficio.idEmpresa}-${beneficio.nombre}`"
                  class="bg-white rounded-xl p-4 border border-gray-200"
                >
                  <h6 class="font-semibold text-gray-800 mb-2">{{ beneficio.nombre }}</h6>
                  <div class="flex items-center space-x-4 text-sm text-gray-600">
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
</template>

<script>
import SuperAdminHeader from '../common/SuperAdminHeader.vue'
import apiConfig from '../../config/api.js'

export default {
  name: 'SuperAdminMenu',
  components: {
    SuperAdminHeader
  },
  data() {
    return {
      empresas: [],
      loading: true,
      error: null,
      expandedEmpresas: new Set(),
      user: null
    }
  },
  methods: {
    checkAuthentication() {
      const userData = localStorage.getItem('user');
      const token = localStorage.getItem('token');
      if (!userData || !token) {
        this.$router.push('/login');
        return false;
      }
      try {
        this.user = JSON.parse(userData);
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
      console.log(this.empresas);
    },
    async fetchEmpresas() {
      try {
        this.loading = true;
        this.error = null;
        const response = await fetch(apiConfig.endpoints.project);
        if (!response.ok) {
          throw new Error(`Error ${response.status}: ${response.statusText}`);
        }
        const data = await response.json(); 
        this.empresas = data;
      } catch (err) {
        this.error = 'Error al cargar las empresas. Por favor, intente nuevamente.';
      } finally {
        this.loading = false;
      }
    }
  },
  mounted() {
    if (this.checkAuthentication()) {
      this.fetchEmpresas();
    }
  }
}
</script>
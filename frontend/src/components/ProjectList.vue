<template>
  <div>
    <!-- Loading State -->
    <div v-if="loading" class="flex justify-center items-center py-12">
      <div class="animate-spin rounded-full h-12 w-12 neumorphism-dark"></div>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded-2xl mb-6 neumorphism-card">
      <span>{{ error }}</span>
    </div>

    <!-- Companies List -->
    <div v-else class="space-y-6">
      <div v-for="company in companies" :key="company.id">
        <div class="flex items-center justify-between mb-2">
          <h3 class="text-lg font-bold text-black">{{ company.name }}</h3>
          <button
            class="neumorphism-dark px-4 py-2 rounded text-white hover:bg-blue-700 transition"
            @click="goToDashboard(company.id)"
          >
            Ir a dashboard de empresa
          </button>
        </div>

        <div class="grid md:grid-cols-4 gap-4">
          <!-- Cédula Jurídica -->
          <div class="neumorphism-card rounded-[14px] w-[365px] h-[190px] flex items-center justify-center transition-all duration-200 hover:scale-110 group">
            <div class="w-[303px] h-[128px] flex flex-col justify-between">
              <div class="flex items-center justify-between mb-2">
                <p class="font-bold text-[16px] group-hover:text-white transition-colors duration-200">Cédula Jurídica</p>
                <svg class="w-6 h-6 group-hover:text-white transition-colors duration-200" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
                  <rect x="3" y="7" width="18" height="10" rx="2" stroke="currentColor"/>
                  <path d="M7 15h.01M7 11h.01" stroke="currentColor"/>
                </svg>
              </div>
              <div>
                <p class="font-bold text-[28px] mb-1 group-hover:text-white transition-colors duration-200">{{ company.legalId }}</p>
                <p class="text-[15px] group-hover:text-white transition-colors duration-200">de la empresa</p>
              </div>
            </div>
          </div>
          <!-- Período de Pago -->
          <div class="neumorphism-card rounded-[14px] w-[365px] h-[190px] flex items-center justify-center transition-all duration-200 hover:scale-110 group">
            <div class="w-[303px] h-[128px] flex flex-col justify-between">
              <div class="flex items-center justify-between mb-2">
                <p class="font-bold text-[16px] group-hover:text-white transition-colors duration-200">Período de Pago</p>
                <svg class="w-6 h-6 group-hover:text-white transition-colors duration-200" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
                  <rect x="3" y="5" width="18" height="16" rx="2" stroke="currentColor"/>
                  <path d="M16 3v4M8 3v4" stroke="currentColor"/>
                </svg>
              </div>
              <div>
                <p class="font-bold text-[28px] mb-1 group-hover:text-white transition-colors duration-200">{{ company.payPeriod }}</p>
                <p class="text-[15px] group-hover:text-white transition-colors duration-200">{{ formatPeriodDescription(company.payPeriod) }}</p>
              </div>
            </div>
          </div>
          <!-- Empleados Activos -->
          <div class="neumorphism-card rounded-[14px] w-[365px] h-[190px] flex items-center justify-center transition-all duration-200 hover:scale-110 group">
            <div class="w-[303px] h-[128px] flex flex-col justify-between">
              <div class="flex items-center justify-between mb-2">
                <p class="font-bold text-[16px] group-hover:text-white transition-colors duration-200">Empleados Activos</p>
                <svg class="w-6 h-6 group-hover:text-white transition-colors duration-200" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
                  <circle cx="9" cy="7" r="4" stroke="currentColor"/>
                  <path d="M17 11a4 4 0 1 0-8 0" stroke="currentColor"/>
                  <path d="M17 21v-2a4 4 0 0 0-4-4H7a4 4 0 0 0-4 4v2" stroke="currentColor"/>
                </svg>
              </div>
              <div>
                <p class="font-bold text-[28px] mb-1 group-hover:text-white transition-colors duration-200">{{ company.activeEmployees }}</p>
                <p class="text-[15px] group-hover:text-white transition-colors duration-200">en esta empresa</p>
              </div>
            </div>
          </div>
          <!-- Rentabilidad Actual -->
          <div class="neumorphism-card rounded-[14px] w-[365px] h-[190px] flex items-center justify-center transition-all duration-200 hover:scale-110 group">
            <div class="w-[303px] h-[128px] flex flex-col justify-between">
              <div class="flex items-center justify-between mb-2">
                <p class="font-bold text-[16px] group-hover:text-white transition-colors duration-200">Rentabilidad Actual</p>
                <svg class="w-6 h-6 group-hover:text-white transition-colors duration-200" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
                  <path d="M4 17l6-6 4 4 6-6" stroke="currentColor"/>
                </svg>
              </div>
              <div>
                <p class="font-bold text-[28px] mb-1 group-hover:text-white transition-colors duration-200">{{ company.currentProfitability }}%</p>
                <p class="text-[15px] group-hover:text-white transition-colors duration-200">del mes actual</p>
              </div>
            </div>
          </div>
        </div>
        <!-- Notifications -->
        <div v-if="company.notifications && company.notifications.length" class="mt-4">
          <h4 class="font-semibold text-blue-700 mb-2">Notificaciones</h4>
          <ul class="list-disc pl-5">
            <li v-for="(note, idx) in company.notifications" :key="idx" class="text-gray-700">
              {{ note }}
            </li>
          </ul>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  // 1. Nombre del componente
  name: 'ProjectList',

  // 2. Componentes hijos locales
  components: {},

  // 3. Directivas locales
  directives: {},

  // 4. Props recibidas del padre
  props: {
    userId: {
      type: [String, Number],
      required: true
    }
  },

  // 5. Estado reactivo del componente
  data() {
    return {
      companies: [],
      loading: true,
      error: null
    }
  },

  // 6. Propiedades derivadas
  computed: {},

  // 7. Observadores de cambios
  watch: {
    userId: {
      immediate: true,
      handler(newId) {
        if (newId) this.fetchCompaniesByUser(newId);
      }
    }
  },

  // 8. Métodos y lógica ejecutable
  methods: {
    async fetchCompaniesByUser(userId) {
      try {
        this.loading = true;
        this.error = null;
        const response = await fetch(`http://localhost:5011/api/Project/dashboard/${userId}`);
        if (!response.ok) throw new Error('No se pudieron cargar las empresas');
        const data = await response.json();
        this.companies = data;
      } catch (err) {
        this.error = err.message || 'Error al cargar las empresas';
      } finally {
        this.loading = false;
      }
    },

    formatPeriodDescription(period) {
      switch (period) {
        case 'Mensual': return 'Pago cada mes';
        case 'Quincenal': return 'Pago cada quincena';
        case 'Semanal': return 'Pago cada semana';
        default: return '';
      }
    },

    goToDashboard(companyId) {
      if (companyId) {
        this.$router.push({ name: 'DashboardProject', params: { id: companyId } });
      } else {
        // TODO: SHOW BETTER ERROR MESSAGE
        alert('No se encontró el ID del proyecto.');
      }
    }
  },

  // 9. Ciclo de vida
  beforeCreate() {},
  created() {},
  beforeMount() {},
  mounted() {},
  beforeUpdate() {},
  updated() {},
  beforeUnmount() {},
  unmounted() {},

  // 10. Opciones de inyección
  provide() {
    return {}
  },
  inject: [],

  // 11. Eventos emitidos
  emits: [],

  // 12. Reutilización de lógica
  mixins: [],
  extends: null,

  // 13. Filtros
  filters: {}
}
</script>
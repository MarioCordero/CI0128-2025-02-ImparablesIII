<template>
  <div class="page">
    <MainEmployerHeader @project-changed="onProjectChanged"/>
    
    <DashboardProjectSubHeader
      :selected-section="selectedSection"
      @section-change="selectedSection = $event"
    />

    <div class="mt-12">
      <div v-if="loading" class="flex justify-center items-center py-12">
        <div class="animate-spin rounded-full h-12 w-12 neumorphism-dark"></div>
      </div>
      
      <div v-else-if="error" class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded-2xl mb-6 neumorphism-card">
        <span>{{ error }}</span>
      </div>

      <!-- Dashboard Section -->
      <div v-if="selectedSection === 'dashboard'">
        <DashboardContent :project="project" @section-change="selectedSection = $event" />
      </div>

      <!-- Benefits Section -->
      <div v-else-if="selectedSection === 'benefits'" class="body">
        <div class="space-y-[18px]">
          <div class="flex justify-between items-center mb-0">
            <h1 class="text-4xl font-bold text-gray-800 mb-4">Gestión de Beneficios</h1>
            <button @click="addBenefit" class="neumorphism-button-normal-light">
              Agregar beneficio
            </button>
          </div>
          <div class="w-full h-[10px] mt-2 rounded neumorphism-on-small-item"></div>
        </div>

 
        <h2 class="text-xl font-semibold mb-4">Beneficios Corporativos</h2>
        <div v-if="!benefits || benefits.length === 0" class="text-gray-500 text-center py-8">
          No hay beneficios registrados para esta empresa.
        </div>

        <!-- Success Message -->
        <div v-if="successMessage" class="bg-green-100 border border-green-400 text-green-700 px-4 py-3 rounded-2xl mb-6 neumorphism-card">
          <div class="flex items-center">
            <svg class="w-5 h-5 mr-2" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"/>
            </svg>
            <span>{{ successMessage }}</span>
          </div>
        </div>

        <!-- Error Message -->
        <div v-if="errorMessage" class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded-2xl mb-6 neumorphism-card">
          <div class="flex items-center">
            <svg class="w-5 h-5 mr-2" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd"/>
            </svg>
            <span>{{ errorMessage }}</span>
          </div>
        </div>

        <!-- Benefit cards-->
        <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-10">
          <div 
            v-for="benefit in benefits" 
            :key="`${benefit.companyId}-${benefit.name}`" 
            class="neumorphism-card h-full space-y-[24px]"
          >
            <!-- Header card -->
            <div class="mb-4 w-full"> 
              <!-- Name and menu -->
              <div class="w-full flex justify-between items-top">
                <div>
                  <h3 class="text-lg font-bold text-gray-800 truncate">{{ benefit.name }}</h3>
                  <div v-if="benefit.descripcion" class="pr-4">
                    <p class="text-gray-800 text-[16px] mt-1 benefit-description">{{ benefit.descripcion }}</p>
                  </div>
                </div>

                <DropdownMenu @action="handleBenefitMenu($event, benefit)" />
              </div>
            </div>

            <!-- Content card -->
            <div class="text-[18px] grid grid-cols-1 gap-auto h-[180px]">
              <div class="flex justify-between">
                <span class="text-gray-600 font-medium">Tipo:</span>
                <span class="text-gray-800">{{ benefit.type }}</span>
              </div>
              <div class="flex justify-between">
                <span class="text-gray-600 font-medium">Cálculo:</span>
                <span :class="['benefit-type-chip', getBenefitStyleClass(benefit.calculationType)]">{{ benefit.calculationType }}</span>
              </div>
              <div v-if="benefit.value" class="flex justify-between">
                <span class="text-gray-600 font-medium">Valor:</span>
                <span class="text-gray-800">₡{{ benefit.value.toLocaleString() }}</span>
              </div>
              <div v-if="benefit.percentage" class="flex justify-between">
                <span class="text-gray-600 font-medium">Porcentaje:</span>
                <span class="text-gray-800">{{ benefit.percentage }}%</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Employees Section -->
      <div v-else-if="selectedSection === 'employees'" class="body">
        <div class="space-y-[18px]">

          <div class="flex justify-between items-center mb-0">
            <h1 class="text-4xl font-bold text-gray-800 mb-4">Gestión de Empleados</h1>
            
            <button @click="addEmployee" class="neumorphism-button-normal-light">
              Agregar empleado
            </button>
          </div>

          <div class="w-full h-[10px] mt-2 rounded neumorphism-on-small-item"></div>
        </div>

        <div class="grid grid-cols-[1fr_3fr] gap-[81px]">
          <EmployeesFilter/>
          <EmployeesSection :project-id="project.id" />
        </div>
      </div>

      <!-- Information Section -->
      <div v-else-if="selectedSection === 'info'" class="body">
        <div class="space-y-[18px]">
          <h1 class="text-4xl font-bold text-gray-800">Información de la Empresa</h1>
          <div class="w-full h-[10px] mt-2 rounded neumorphism-on-small-item"></div>
        </div>

        <div>
          <EditProjectInfo/>
        </div>
      </div>

      <!-- Reports Section -->
      <div v-else-if="selectedSection === 'reports'" class="body">        
        <div class="space-y-[18px]">
          <h1 class="text-4xl font-bold text-gray-800">Reportes de Planilla</h1>
          <div class="w-full h-[10px] mt-2 rounded neumorphism-on-small-item"></div>
        </div>

        <PayrollReports user-type="employer" />
      </div>
    </div>
  </div>

  <div v-if="showDeleteModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 px-4">
    <div class="neumorphism-card-modal p-6! shadow-none! max-w-lg">
      <h3 class="text-2xl font-semibold text-gray-800 mb-3">Confirmar eliminación</h3>
      <p class="text-gray-700 mb-4">
        ¿Estás seguro de que deseas eliminar el beneficio
        <span class="font-bold">"{{ benefitPendingDeletion?.name }}"</span>?
        Si ya fue utilizado en planillas anteriores se mantendrá oculto para los empleados, pero no podrá volver a seleccionarse.
      </p>
      <p class="text-sm text-gray-500 mb-4">
        Esta acción no puede deshacerse.
      </p>

      <div v-if="deleteModalError" class="bg-red-100 border border-red-300 text-red-700 px-4 py-2 rounded-lg mb-4">
        {{ deleteModalError }}
      </div>

      <div class="flex justify-end gap-3">
        <button
          class="neumorphism-button-normal-light"
          :disabled="deleteModalLoading"
          @click="closeDeleteModal"
        >
          Cancelar
        </button>
        <button
          class="neumorphism-button-normal-red"
          :disabled="deleteModalLoading"
          @click="confirmBenefitDeletion"
        >
          {{ deleteModalLoading ? 'Eliminando...' : 'Eliminar beneficio' }}
        </button>
      </div>
    </div>
  </div>
</template>

<script>
import MainEmployerHeader from '../../common/MainEmployerHeader.vue'
import DashboardProjectSubHeader from './DashboardProjectSubHeader.vue'
// import PayrollReports from '../../common/PayrollReports.vue'
import PayrollReports from '../projectDashboard/PayrollReports.vue'
import EmployeesSection from './EmployeesSection.vue'
import EmployeesFilter from './EmployeesFilter.vue'
import EditProjectInfo from './EditProjectInfo.vue'
import { apiConfig } from '../../../config/api.js'
import DashboardContent from './DashboardContent.vue'
import DropdownMenu from './DropdownMenu.vue'

export default {
  name: 'ProjectDashboard',

  components: {
    MainEmployerHeader,
    DashboardProjectSubHeader,
    PayrollReports,
    DashboardContent,
    EmployeesSection,
    EmployeesFilter,
    EditProjectInfo,
    DropdownMenu
  },

  data() {
    return {
      project: {},
      loading: true,
      error: null,
      companies: [],
      benefits: [],
      selectedSection: 'dashboard',
      dashboardData: {},
      showDeleteModal: false,
      benefitPendingDeletion: null,
      deleteModalError: null,
      deleteModalLoading: false,
      successMessage: '',
      errorMessage: '',
      successMessageTimeoutId: null,
      errorMessageTimeoutId: null
    }

  },

  watch: {
    '$route.query.section': {
      handler(newSection) {
        if (newSection) {
          this.selectedSection = newSection;
          this.$router.replace({ 
            path: this.$route.path,
            query: {} 
          });
        }
      },
      immediate: true
    }
  },
  
  methods: {
    goBack() {
      this.$router.push('/dashboard-main-employer');
    },

    addBenefit() {
      this.$router.push('/add-benefit');
    },

    addEmployee() {
      this.$router.push({
        name: 'RegisterEmployee',
        params: {
          employerId: this.project.id,
          projectId: this.project.id
        }
      })
    },
    async fetchBenefits() {
      try {
        if (!this.project || !this.project.id) {
          this.benefits = [];
          return;
        }
        const response = await fetch(apiConfig.endpoints.benefitByCompany(this.project.id));
        if (!response.ok) throw new Error('No se pudo cargar los beneficios');
        this.benefits = await response.json();
      } catch (err) {
        this.error = err.message || 'Error al cargar los beneficios';
      }
    },
    onProjectChanged(project) {
      this.project = project;
      this.error = null;
      this.loading = false;
      this.fetchBenefits();
    },

    async fetchProject() {
      try {
        this.loading = true;
        this.error = null;
        const localProject = localStorage.getItem('selectedProject');
        if (localProject) {
          this.project = JSON.parse(localProject);
        } else {
          this.error = 'No hay información de la empresa en localStorage';
        }
      } catch (err) {
        this.error = err.message || 'Error al cargar el proyecto';
      } finally {
        this.loading = false;
      }
    },

    editBenefit(benefit) {
      this.$router.push(`/edit-benefit/${benefit.companyId}/${encodeURIComponent(benefit.name)}`);
    },

    handleBenefitMenu(action, benefit) {
      if (!action || !benefit) {
        return;
      }

      if (action === 'Editar') {
        this.editBenefit(benefit);
        return;
      }

      if (action === 'Eliminar') {
        this.openDeleteModal(benefit);
      }
    },

    openDeleteModal(benefit) {
      this.benefitPendingDeletion = benefit;
      this.deleteModalError = null;
      this.showDeleteModal = true;
    },

    closeDeleteModal(force = false) {
      if (this.deleteModalLoading && !force) {
        return;
      }
      this.showDeleteModal = false;
      this.benefitPendingDeletion = null;
      this.deleteModalError = null;
    },

    async confirmBenefitDeletion() {
      if (!this.benefitPendingDeletion) {
        return;
      }

      this.deleteModalError = null;
      this.deleteModalLoading = true;

      try {
        await this.deleteBenefit(this.benefitPendingDeletion);
        this.closeDeleteModal(true);
      } catch (err) {
        this.deleteModalError = err.message || 'No se pudo eliminar el beneficio.';
      } finally {
        this.deleteModalLoading = false;
      }
    },

    async deleteBenefit(benefit) {
      if (!benefit) {
        return;
      }

      const companyId = benefit.companyId ?? this.project.id;

      try {
        this.error = null;
        this.successMessage = '';
        this.errorMessage = '';
        const response = await fetch(
          apiConfig.endpoints.benefitByCompanyAndName(companyId, benefit.name),
          { method: 'DELETE' }
        );

        if (!response.ok) {
          let errorMessage = 'No se pudo eliminar el beneficio';
          try {
            const errorPayload = await response.json();
            if (errorPayload?.message) {
              errorMessage = errorPayload.message;
            }
          } catch (parseError) {
            // ignore parsing errors
          }
          throw new Error(errorMessage);
        }

        const payload = await response.json();
        this.showTemporaryMessage('success', payload?.message || 'Beneficio eliminado correctamente');

        await this.fetchBenefits();
      } catch (err) {
        const message = err.message || 'Error al eliminar el beneficio';
        this.error = message;
        this.showTemporaryMessage('error', message);
        throw err;
      }
    },

    showTemporaryMessage(type, message) {
      const messageKey = type === 'success' ? 'successMessage' : 'errorMessage';
      const timeoutKey = type === 'success' ? 'successMessageTimeoutId' : 'errorMessageTimeoutId';

      if (this[timeoutKey]) {
        clearTimeout(this[timeoutKey]);
        this[timeoutKey] = null;
      }

      this[messageKey] = message;

      this[timeoutKey] = setTimeout(() => {
        this[messageKey] = '';
        this[timeoutKey] = null;
      }, 4000);
    },

    getBenefitStyleClass(calculationType) {
      if (!calculationType) {
        return '';
      }

      const normalizedType = calculationType.toLowerCase();

      if (normalizedType === 'api') {
        return 'api-type';
      }

      if (normalizedType === 'porcentaje') {
        return 'percentage-type';
      }

      if (normalizedType === 'monto fijo') {
        return 'value-type';
      }

      return '';
    }
  },

  beforeUnmount() {
    if (this.successMessageTimeoutId) {
      clearTimeout(this.successMessageTimeoutId);
      this.successMessageTimeoutId = null;
    }

    if (this.errorMessageTimeoutId) {
      clearTimeout(this.errorMessageTimeoutId);
      this.errorMessageTimeoutId = null;
    }
  },

  async mounted() {
    await this.fetchProject();
    this.fetchBenefits();
  
    if (this.$route.query.section) {
      this.selectedSection = this.$route.query.section;

      this.$nextTick(() => {
        this.$router.replace({ 
          path: this.$route.path,
          query: {} 
        });
      });
    }
  }
}
</script>

<style scoped>
.benefit-type-chip {
  padding: 4px 10px;
  border-radius: 999px;
  max-height: fit-content;
  max-width: fit-content;
  box-shadow: 4px 4px 8px #bebebe,
              -4px -4px 8px #ffffff;
}

.benefit-description {
  max-height: 50px;
  overflow-y: auto;
  overflow-x: hidden;
  line-height: 1.4;
  scrollbar-gutter: stable;
  padding-right: 5px;
}

.benefit-description::-webkit-scrollbar {
  width: 12px;
}

.benefit-description::-webkit-scrollbar-track {
  background: #dbeafe;
  border-radius: 10px;
  box-shadow: inset 2px 2px 4px #bebebe,
              inset -2px -2px 4px #ffffff;
}

.benefit-description::-webkit-scrollbar-thumb {
  background: #ffffff;
  border-radius: 10px;
  box-shadow: inset 0 0 2px 1px rgba(16, 72, 255, 1);
  transition: all 0.3s;
}

.api-type{
  background: #dbeafe;
  color: #7476ff;
}

.percentage-type{
  background: #7476ff;
  color: #ffffff;
}

.value-type{
  background: #74ff90;
  color: #000000;
}
</style>
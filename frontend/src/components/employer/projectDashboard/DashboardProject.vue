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


        <div class="neumorphism-card p-6 rounded-2xl">
          <h2 class="text-xl font-semibold mb-4">Beneficios Corporativos</h2>
          <div v-if="!benefits || benefits.length === 0" class="text-gray-500 text-center py-8">
            No hay beneficios registrados para esta empresa.
          </div>
          <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            <div 
              v-for="benefit in benefits" 
              :key="`${benefit.companyId}-${benefit.name}`" 
              class="neumorphism-card p-4 rounded-xl hover:shadow-lg transition-shadow duration-300"
            >
              <div class="flex flex-col space-y-2">
                <div class="flex justify-between items-start">
                  <h3 class="text-lg font-bold text-gray-800 truncate">{{ benefit.name }}</h3>
                  <!-- Botón de editar -->
                  <button class="neumorphism-button-normal-light p-[7px]! rounded-full!" @click="editBenefit(benefit)">
                    ✏️
                  </button>
                </div>
                <div class="space-y-1 text-sm">
                  <div class="flex justify-between">
                    <span class="text-gray-600 font-medium">Tipo:</span>
                    <span class="text-gray-800">{{ benefit.type }}</span>
                  </div>
                  <div class="flex justify-between">
                    <span class="text-gray-600 font-medium">Cálculo:</span>
                    <span class="text-gray-800">{{ benefit.calculationType }}</span>
                  </div>
                  <div v-if="benefit.value" class="flex justify-between">
                    <span class="text-gray-600 font-medium">Valor:</span>
                    <span class="text-gray-800">₡{{ benefit.value.toLocaleString() }}</span>
                  </div>
                  <div v-if="benefit.percentage" class="flex justify-between">
                    <span class="text-gray-600 font-medium">Porcentaje:</span>
                    <span class="text-gray-800">{{ benefit.percentage }}%</span>
                  </div>
                  <!-- Mostrar descripción si existe -->
                  <div v-if="benefit.descripcion" class="mt-2 pt-2 border-t border-gray-200">
                    <span class="text-gray-600 font-medium">Descripción:</span>
                    <p class="text-gray-800 text-xs mt-1">{{ benefit.descripcion }}</p>
                  </div>
                </div>
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
          <EditProjectInfo/> <!--TODO: Make the component prettier-->
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

export default {
  name: 'ProjectDashboard',
  components: {
    MainEmployerHeader,
    DashboardProjectSubHeader,
    PayrollReports,
    DashboardContent,
    EmployeesSection,
    EmployeesFilter,
    EditProjectInfo
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
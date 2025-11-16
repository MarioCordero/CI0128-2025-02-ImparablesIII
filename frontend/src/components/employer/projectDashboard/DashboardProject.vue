<template>
  <div class="bg-[#dbeafe] min-h-screen flex flex-col page">
    <MainEmployerHeader @project-changed="onProjectChanged"/>
    
    <DashboardProjectSubHeader
      :selected-section="selectedSection"
      @section-change="selectedSection = $event"
    />

    <div class="mx-[171px] my-[41px] space-y-[41px] pb-[41px] body">
      <div v-if="loading" class="flex justify-center items-center py-12">
        <div class="animate-spin rounded-full h-12 w-12 neumorphism-dark"></div>
      </div>
      
      <div v-else-if="error" class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded-2xl mb-6 neumorphism-card">
        <span>{{ error }}</span>
      </div>

      <!-- Dashboard Section -->
<div v-if="selectedSection === 'dashboard'">
        <div>
          <h1 class="text-4xl font-bold text-gray-800">Dashboard de Empresa</h1>
        </div>
        
        <!-- Métricas Clave -->
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
          <!-- Total Empleados -->
          <div class="neumorphism-card p-6 rounded-2xl text-center">
            <div class="text-3xl font-bold text-blue-600 mb-2">{{ dashboardData.totalEmployees || 25 }}</div>
            <div class="text-gray-600 text-sm">Empleados Activos</div>
            <div class="text-green-500 text-xs mt-1">+5% vs mes anterior</div>
          </div>

          <!-- Planilla del Mes -->
          <div class="neumorphism-card p-6 rounded-2xl text-center">
            <div class="text-3xl font-bold text-green-600 mb-2">₡{{ (dashboardData.currentPayroll || 2500000).toLocaleString() }}</div>
            <div class="text-gray-600 text-sm">Planilla Actual</div>
            <div class="text-red-500 text-xs mt-1">+3% vs mes anterior</div>
          </div>

          <!-- Departamentos -->
          <div class="neumorphism-card p-6 rounded-2xl text-center">
            <div class="text-3xl font-bold text-purple-600 mb-2">{{ dashboardData.activeDepartments || 5 }}</div>
            <div class="text-gray-600 text-sm">Departamentos</div>
            <div class="text-gray-400 text-xs mt-1">Sin cambios</div>
          </div>

          <!-- Tareas Pendientes -->
          <div class="neumorphism-card p-6 rounded-2xl text-center">
            <div class="text-3xl font-bold text-orange-600 mb-2">{{ dashboardData.pendingTasks || 3 }}</div>
            <div class="text-gray-600 text-sm">Tareas Pendientes</div>
            <div class="text-blue-500 text-xs mt-1">2 nuevas hoy</div>
          </div>
        </div>

        <!-- Información de la Empresa y Acciones Rápidas -->
        <div class="grid grid-cols-1 lg:grid-cols-2 gap-8 mb-8">
          <!-- Información de la Empresa -->
          <div class="neumorphism-card p-6 rounded-2xl">
            <h2 class="text-xl font-semibold mb-4">Información de la Empresa</h2>
            <div class="space-y-2">
              <p class="text-gray-700"><span class="font-bold">Nombre:</span> {{ project.nombre }}</p>
              <p class="text-gray-700"><span class="font-bold">Cédula Jurídica:</span> {{ project.cedulaJuridica }}</p>
              <p class="text-gray-700"><span class="font-bold">Período de Pago:</span> {{ project.periodoPago }}</p>
              <p class="text-gray-700"><span class="font-bold">Email:</span> {{ project.email }}</p>
              <p class="text-gray-700"><span class="font-bold">Teléfono:</span> {{ project.telefono }}</p>
              <p class="text-gray-700"><span class="font-bold">Dirección:</span> {{ project.direccion || 'N/A' }}</p>
            </div>
          </div>

          <!-- Acciones Rápidas -->
          <div class="neumorphism-card p-6 rounded-2xl">
            <h2 class="text-xl font-semibold mb-4">Acciones Rápidas</h2>
            <div class="grid grid-cols-2 gap-4">
              <button 
                @click="selectedSection = 'employees'" 
                class="neumorphism-button p-4 rounded-xl text-center hover:bg-blue-50 transition"
              >
                <div class="text-2xl mb-2">👥</div>
                <div class="text-sm font-medium">Gestionar Empleados</div>
              </button>

              <button 
                @click="selectedSection = 'reports'" 
                class="neumorphism-button p-4 rounded-xl text-center hover:bg-blue-50 transition"
              >
                <div class="text-2xl mb-2">📊</div>
                <div class="text-sm font-medium">Ver Reportes</div>
              </button>

              <button 
                @click="selectedSection = 'benefits'" 
                class="neumorphism-button p-4 rounded-xl text-center hover:bg-blue-50 transition"
              >
                <div class="text-2xl mb-2">🎁</div>
                <div class="text-sm font-medium">Gestionar Beneficios</div>
              </button>

              <button 
                @click="selectedSection = 'info'" 
                class="neumorphism-button p-4 rounded-xl text-center hover:bg-blue-50 transition"
              >
                <div class="text-2xl mb-2">⚙️</div>
                <div class="text-sm font-medium">Configurar Empresa</div>
              </button>
            </div>
          </div>
        </div>

        <!-- Gráficos Placeholder -->
        <div class="grid grid-cols-1 lg:grid-cols-2 gap-8 mb-8">
          <!-- Distribución por Departamentos -->
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

          <!-- Evolución de Planilla -->
          <div class="neumorphism-card p-6 rounded-2xl">
            <h3 class="text-lg font-semibold mb-4">Evolución de Planilla (6 meses)</h3>
            <div class="h-64 flex items-center justify-center border-2 border-dashed border-gray-300 rounded-lg">
              <div class="text-center">
                <div class="text-4xl mb-2">📈</div>
                <div class="text-gray-500">Gráfico de Líneas</div>
                <div class="text-xs text-gray-400 mt-1">Próximamente</div>
              </div>
            </div>
          </div>
        </div>

        <!-- Actividades Recientes -->
        <div class="neumorphism-card p-6 rounded-2xl">
          <h3 class="text-lg font-semibold mb-4">Actividades Recientes</h3>
          <div class="space-y-3">
            <div v-for="activity in dashboardData.recentActivities || mockRecentActivities" 
                 :key="activity.id" 
                 class="flex items-center justify-between p-3 bg-gray-50 rounded-lg"
            >
              <div class="flex items-center">
                <div class="text-lg mr-3">{{ activity.icon }}</div>
                <div>
                  <div class="font-medium">{{ activity.title }}</div>
                  <div class="text-sm text-gray-600">{{ activity.description }}</div>
                </div>
              </div>
              <div class="text-xs text-gray-500">{{ activity.time }}</div>
            </div>
          </div>
        </div>

        <!-- Próximos Vencimientos -->
        <div class="neumorphism-card p-6 rounded-2xl">
          <h3 class="text-lg font-semibold mb-4">Próximos Vencimientos</h3>
          <div class="space-y-3">
            <div v-for="reminder in dashboardData.upcomingReminders || mockUpcomingReminders" 
                 :key="reminder.id" 
                 class="flex items-center justify-between p-3 border-l-4 rounded-lg"
                 :class="{
                   'border-red-500 bg-red-50': reminder.priority === 'high',
                   'border-yellow-500 bg-yellow-50': reminder.priority === 'medium',
                   'border-blue-500 bg-blue-50': reminder.priority === 'low'
                 }"
            >
              <div>
                <div class="font-medium">{{ reminder.title }}</div>
                <div class="text-sm text-gray-600">{{ reminder.description }}</div>
              </div>
              <div class="text-sm font-medium" 
                   :class="{
                     'text-red-600': reminder.priority === 'high',
                     'text-yellow-600': reminder.priority === 'medium',
                     'text-blue-600': reminder.priority === 'low'
                   }"
              >
                {{ reminder.dueDate }}
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Benefits Section -->
      <div v-else-if="selectedSection === 'benefits'">
        <div>
          <h1 class="text-4xl font-bold text-gray-800 mb-4">Gestión de Beneficios</h1>
        </div>
        <div>
          <button class="neumorphism-dark px-6 py-3 rounded-lg text-white hover:bg-blue-700 transition" @click="addBenefit">
            Agregar beneficio
          </button>
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
                    <button class="neumorfismo-boton p-[7px] rounded-full!" @click="editBenefit(benefit)">
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
      <div v-else-if="selectedSection === 'employees'">
        <div>
          <h1 class="text-4xl font-bold text-gray-800">Gestión de Empleados</h1>
        </div>
        <div>
          <button class="neumorphism-dark px-6 py-3 rounded-lg text-white hover:bg-blue-700 transition" @click="addEmployee">
            Agregar empleado
          </button>
        </div>
        <div class="grid grid-cols-[1fr_3fr] gap-[81px]">
          <EmployeesFilter/>
          <EmployeesSection :project-id="project.id" />
        </div>
      </div>

      <!-- Information Section -->
      <div v-else-if="selectedSection === 'info'">
        <div>
          <h1 class="text-4xl font-bold text-gray-800 mb-4">Información de la Empresa</h1>
        </div>
        <div>
          <EditProjectInfo/>
        </div>
      </div>

      <!-- Reports Section -->
      <div v-else-if="selectedSection === 'reports'">
        <div>
          <h1 class="text-4xl font-bold text-gray-800 mb-4">Reportes de Planilla</h1>
        </div>
        <PayrollReports />
      </div>
    </div>
  </div>
</template>

<script>
import MainEmployerHeader from '../../common/MainEmployerHeader.vue'
import DashboardProjectSubHeader from './DashboardProjectSubHeader.vue'
import PayrollReports from './PayrollReports.vue'
import EmployeesSection from './EmployeesSection.vue'
import EmployeesFilter from './EmployeesFilter.vue'
import EditProjectInfo from './EditProjectInfo.vue'
import { apiConfig } from '../../../config/api.js'

export default {
  name: 'ProjectDashboard',
  components: {
    MainEmployerHeader,
    DashboardProjectSubHeader,
    PayrollReports,
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
      dashboardData: {}, // Para datos del backend cuando esté disponible
      mockRecentActivities: [
        {
          id: 1,
          icon: '👤',
          title: 'Nuevo empleado registrado',
          description: 'Juan Pérez se agregó al sistema',
          time: 'Hace 2 horas'
        },
        {
          id: 2,
          icon: '💰',
          title: 'Planilla procesada',
          description: 'Planilla de noviembre completada',
          time: 'Ayer'
        },
        {
          id: 3,
          icon: '🎁',
          title: 'Beneficio actualizado',
          description: 'Seguro médico modificado',
          time: 'Hace 3 días'
        }
      ],
      mockUpcomingReminders: [
        {
          id: 1,
          title: 'Aguinaldo 2024',
          description: 'Cálculo y pago de aguinaldo',
          dueDate: '15 Dic',
          priority: 'high'
        },
        {
          id: 2,
          title: 'Reporte mensual CCSS',
          description: 'Envío de planilla a CCSS',
          dueDate: '30 Nov',
          priority: 'medium'
        },
        {
          id: 3,
          title: 'Revisión de vacaciones',
          description: 'Actualizar días disponibles',
          dueDate: '5 Dic',
          priority: 'low'
        }
      ]
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

    async fetchCompanies() {
      try {
        const response = await fetch(apiConfig.endpoints.project);
        if (!response.ok) throw new Error('No se pudo cargar las empresas');
        this.companies = await response.json();
      } catch (err) {
        console.error('Error fetching companies:', err);
      }
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

    async fetchDashboardData() {
      try {
        if (!this.project || !this.project.id) return;
        
        // Cuando el backend esté listo, descomenta esto:
        // const response = await fetch(apiConfig.endpoints.employerDashboard(this.project.id));
        // if (!response.ok) throw new Error('No se pudo cargar el dashboard');
        // this.dashboardData = await response.json();
        
        // Por ahora usa datos mock
        console.log('Dashboard data loaded for company:', this.project.id);
      } catch (err) {
        console.error('Error loading dashboard:', err);
      }
    },

    onProjectChanged(project) {
      this.project = project;
      this.error = null;
      this.loading = false;
      this.fetchBenefits();
      this.fetchDashboardData(); // Agregar esta línea
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
    this.fetchCompanies();
    await this.fetchProject();
    this.fetchBenefits();
  }
}
</script>
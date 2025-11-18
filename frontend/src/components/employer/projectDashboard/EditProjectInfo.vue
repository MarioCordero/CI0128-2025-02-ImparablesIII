<template>
  <div class="neumorphism-card">
    <div v-if="isLoading" class="text-center py-8 text-gray-500">
      Cargando información de la empresa...
    </div>
    
    <div v-else-if="errorMessage" class="text-center py-8 text-red-500">
      {{ errorMessage }}
    </div>
    
    <div v-else-if="project">
      <!-- Header Section -->
      <div class="flex justify-between items-center mb-6">
        <h1 class="text-4xl font-bold text-gray-800">{{ project.nombre }}</h1>
        <button
          class="neumorphism-button-normal-blue"
          @click="showEditModal = true"
        >
          Editar Empresa
        </button>
      </div>

      <!-- Main Content Grid -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-8">
        
        <!-- Left Column -->
        <div class="neumorphism-on p-6 space-y-4">
          <h3 class="text-xl font-bold text-gray-800 mb-4 border-b border-gray-200 pb-2">
            Información General
          </h3>
          
          <div class="neumorfismo-sobre-suave">
            <div class="font-bold text-gray-700 mb-1">ID Empresa</div>
            <div class="text-gray-600 text-lg">{{ project.id }}</div>
          </div>

          <div class="neumorfismo-sobre-suave">
            <div class="font-bold text-gray-700 mb-1">Cédula Jurídica</div>
            <div class="text-gray-600 text-lg">{{ project.cedulaJuridica }}</div>
          </div>

          <div class="neumorfismo-sobre-suave">
            <div class="font-bold text-gray-700 mb-1">Correo Electrónico</div>
            <div class="text-gray-600 text-lg">{{ project.email }}</div>
          </div>

          <div class="neumorfismo-sobre-suave">
            <div class="font-bold text-gray-700 mb-1">Teléfono</div>
            <div class="text-gray-600 text-lg">{{ project.telefono }}</div>
          </div>
        </div>

        <!-- Right Column -->
        <div class="neumorphism-on p-6 space-y-4">
          <h3 class="text-xl font-bold text-gray-800 mb-4 border-b border-gray-200 pb-2">
            Configuración Empresarial
          </h3>

          <div class="neumorfismo-sobre-suave">
            <div class="font-bold text-gray-700 mb-1">Período de Pago</div>
            <div class="text-gray-600 text-lg">{{ project.periodoPago }}</div>
          </div>

          <div class="neumorfismo-sobre-suave">
            <div class="font-bold text-gray-700 mb-1">Máximo de Beneficios Elegibles</div>
            <div class="text-gray-600 text-lg">{{ project.maximoBeneficios }}</div>
          </div>

          <div class="neumorfismo-sobre-suave">
            <div class="font-bold text-gray-700 mb-1">ID Dirección</div>
            <div class="text-gray-600 text-lg">{{ project.idDireccion }}</div>
          </div>
        </div>
      </div>

      <!-- Address Section (if exists) -->
      <div v-if="project.direccion" class="mt-8">
        <div class="neumorphism-on p-6">
          <h3 class="text-xl font-bold text-gray-800 mb-4 border-b border-gray-200 pb-2">
            📍 Dirección Completa
          </h3>
          
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            <div class="neumorfismo-sobre-suave">
              <div class="font-bold text-gray-700 mb-1">Provincia</div>
              <div class="text-gray-600">{{ project.direccion.provincia }}</div>
            </div>

            <div class="neumorfismo-sobre-suave">
              <div class="font-bold text-gray-700 mb-1">Cantón</div>
              <div class="text-gray-600">{{ project.direccion.canton }}</div>
            </div>

            <div class="neumorfismo-sobre-suave">
              <div class="font-bold text-gray-700 mb-1">Distrito</div>
              <div class="text-gray-600">{{ project.direccion.distrito }}</div>
            </div>
          </div>

          <div class="mt-4 neumorfismo-sobre-suave">
            <div class="font-bold text-gray-700 mb-1">Dirección Particular</div>
            <div class="text-gray-600">{{ project.direccion.direccionParticular }}</div>
          </div>
        </div>
      </div>

      <!-- Action Buttons -->
      <div class="mt-8 flex justify-center gap-4">
        <button 
          class="neumorfismo-boton-rojo px-6 py-3"
          @click="showDeleteModal = true"
        >
          🗑️ Eliminar Empresa
        </button>
      </div>

      <!-- Delete Warning Modal -->
      <WarningModal
        :is-visible="showDeleteModal"
        title="Eliminar Empresa"
        :message="`¿Estás seguro de que deseas eliminar la empresa '${project?.nombre}'? Esta acción eliminará PERMANENTEMENTE todos los datos asociados incluyendo: empleados, planillas, beneficios, reportes y configuraciones. Esta acción NO se puede deshacer.`"
        @confirm="confirmDeleteCompany"
        @cancel="cancelDeleteCompany"
        @close="showDeleteModal = false"
      />

      <!-- Edit Modal -->
      <EditCompanyModal
        :visible="showEditModal"
        :company="project"
        @close="showEditModal = false"
        @save="handleSave"
      />
    </div>
  </div>
</template>

<script>
import { apiConfig } from '../../../config/api.js'
import EditCompanyModal from './EditCompanyModal.vue'
import WarningModal from '../../common/WarningModal.vue'


export default {
  name: 'EditProjectInfo',
  components: { 
    EditCompanyModal, 
    WarningModal 
  },
  data() {
    return {
      project: null,
      isLoading: true,
      errorMessage: '',
      showEditModal: false,
      showDeleteModal: false
    }
  },
  mounted() {
    this.fetchProjectInfo()
  },
  methods: {
    async fetchProjectInfo() {
      this.isLoading = true
      this.errorMessage = ''
      try {
        const selectedProject = JSON.parse(localStorage.getItem('selectedProject'))
        const companyId = selectedProject?.id
        if (!companyId) {
          throw new Error('No se encontró el ID de empresa en localStorage')
        }
        const response = await fetch(apiConfig.endpoints.byCompany(companyId))
        if (!response.ok) {
          throw new Error('No se encontró la empresa')
        }
        this.project = await response.json()
      } catch (error) {
        this.errorMessage = 'Error al cargar la información de la empresa'
      } finally {
        this.isLoading = false
      }
    },
    async handleSave(updatedCompany) {
      this.project = updatedCompany
      this.showEditModal = false
      const selectedProject = JSON.parse(localStorage.getItem('selectedProject'))
      if (selectedProject && selectedProject.id === updatedCompany.id) {
        localStorage.setItem('selectedProject', JSON.stringify(updatedCompany))
      }
    },
    async confirmDeleteCompany() {
      try {
        // TODO: CALL API TO DELETE COMPANY
        // const response = await fetch(apiConfig.endpoints.project, {
        //   method: 'DELETE',
        //   headers: {
        //     'Content-Type': 'application/json',
        //   },
        //   body: JSON.stringify({ id: this.project.id })
        // })
        // if (!response.ok) {
        //   throw new Error('Error al eliminar la empresa')
        // }
        // localStorage.removeItem('selectedProject')
        this.$router.push('/dashboard-main-employer')
        // TODO: Show success notification
        
      } catch (error) {
        console.error('Error deleting company:', error)
        this.errorMessage = 'Error al eliminar la empresa. Inténtalo de nuevo.'
      } finally {
        this.showDeleteModal = false
      }
    },
    cancelDeleteCompany() {
      this.showDeleteModal = false
    }
  }
}
</script>
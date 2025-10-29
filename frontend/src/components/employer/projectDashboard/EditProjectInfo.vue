<template>
  <div class="bg-[#eaf2fb] rounded-2xl shadow-md p-8 mx-auto">
    <div v-if="isLoading" class="text-center py-8 text-gray-500">Cargando información de la empresa...</div>
    <div v-else-if="errorMessage" class="text-center py-8 text-red-500">{{ errorMessage }}</div>
    <div v-else-if="project">
      <div class="flex justify-between items-center mb-4">
        <h1 class="text-4xl font-bold text-gray-800 mb-4 md:mb-0">{{ project.nombre }}</h1>
        <button
          class="neumorphism-dark px-4 py-2 rounded-lg text-white hover:bg-blue-700 transition"
          @click="showEditModal = true"
        >
          Editar Empresa
        </button>
      </div>
      <div class="bg-white rounded-xl p-6 grid grid-cols-1 md:grid-cols-2 gap-8">
        <div>
          <h3 class="text-2xl font-semibold text-gray-800 mb-2">
            <span class="ml-2 px-3 py-1 rounded-full bg-green-100 text-green-700 text-sm font-semibold align-middle">Activo</span>
          </h3>
          <div class="mb-4">
            <div class="font-bold text-gray-700 mb-1">ID Empresa</div>
            <div class="text-gray-600">{{ project.id }}</div>
          </div>
          <div class="mb-4">
            <div class="font-bold text-gray-700 mb-1">Cédula jurídica</div>
            <div class="text-gray-600">{{ project.cedulaJuridica }}</div>
          </div>
          <div class="mb-4">
            <div class="font-bold text-gray-700 mb-1">Correo electrónico</div>
            <div class="text-gray-600">{{ project.email }}</div>
          </div>
          <div class="mb-4">
            <div class="font-bold text-gray-700 mb-1">Teléfono</div>
            <div class="text-gray-600">{{ project.telefono }}</div>
          </div>
          <div class="mb-4">
            <div class="font-bold text-gray-700 mb-1">ID Dirección</div>
            <div class="text-gray-600">{{ project.idDireccion }}</div>
          </div>
        </div>
        <div>
          <div class="mb-4">
            <div class="font-bold text-gray-700 mb-1">Periodo de pago</div>
            <div class="text-gray-600">{{ project.periodoPago }}</div>
          </div>
          <div class="mb-4">
            <div class="font-bold text-gray-700 mb-1">Máximo de beneficios elegibles</div>
            <div class="text-gray-600">{{ project.maximoBeneficios }}</div>
          </div>
          <div v-if="project.direccion" class="mb-4">
            <div class="font-bold text-gray-700 mb-1">Dirección</div>
            <div class="text-gray-600">
              <div><span class="font-bold">ID Dirección:</span> {{ project.direccion.id }}</div>
              <div><span class="font-bold">Provincia:</span> {{ project.direccion.provincia }}</div>
              <div><span class="font-bold">Cantón:</span> {{ project.direccion.canton }}</div>
              <div><span class="font-bold">Distrito:</span> {{ project.direccion.distrito }}</div>
              <div><span class="font-bold">Dirección Particular:</span> {{ project.direccion.direccionParticular }}</div>
            </div>
          </div>
        </div>
      </div>
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

export default {
  name: 'EditProjectInfo',
  components: { EditCompanyModal },
  data() {
    return {
      project: null,
      isLoading: true,
      errorMessage: '',
      showEditModal: false
    }
  },
  mounted() {
    this.fetchProjectInfo();
  },
  methods: {
    async fetchProjectInfo() {
      this.isLoading = true;
      this.errorMessage = '';
      try {
        const selectedProject = JSON.parse(localStorage.getItem('selectedProject'));
        const companyId = selectedProject?.id;
        if (!companyId) throw new Error('No se encontró el ID de empresa en localStorage');
        const response = await fetch(apiConfig.endpoints.byCompany(companyId));
        if (!response.ok) throw new Error('No se encontró la empresa');
        this.project = await response.json();
      } catch (error) {
        this.errorMessage = 'Error al cargar la empresa';
      } finally {
        this.isLoading = false;
      }
    },
    async handleSave(updatedCompany) {
      this.project = updatedCompany
      this.showEditModal = false
    } 
  }
}
</script>
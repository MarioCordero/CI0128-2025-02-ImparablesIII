<template>
  <div class="min-h-screen bg-[#E9F7FF] p-0">
    <MainEmployerHeader :companies="companies" :current-project="selectedProject" />

    <div class="mx-[171px] my-[41px] space-y-[41px]">
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

      <!-- Form -->
      <div class="neumorphism-card bg-[#E9F7FF] p-8 my-20! rounded-2xl">
        <h1 class="text-5xl font-black mb-8! mt-2 text-black tracking-wide text-center py-2 px-4">
          Agregar Beneficio
        </h1>
      

        <form @submit.prevent="handleSubmit" class="space-y-6">
          <!-- Información del Beneficio -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <!-- Nombre del Beneficio -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">
                Nombre del Beneficio *
              </label>
              <input
                v-model="form.name"
                type="text"
                maxlength="20"
                :class="['neumorphism-input w-full', errors.name ? 'ring-2 ring-red-500' : '']"
                placeholder="Ej: Bono de Navidad"
                @blur="validateName"
              />
              <span v-if="errors.name" class="text-red-500 text-sm mt-1">{{ errors.name }}</span>
            </div>

            <!-- Tipo de Cálculo -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">
                Tipo de Cálculo *
              </label>
              <select
                v-model="form.calculationType"
                :class="['neumorphism-input w-full', errors.calculationType ? 'ring-2 ring-red-500' : '']"
                @change="validateCalculationType"
              >
                <option value="">Seleccione el tipo de cálculo</option>
                <option value="Porcentaje">Porcentaje</option>
                <option value="Monto Fijo">Monto Fijo</option>
                <option value="API">API</option>
              </select>
              <span v-if="errors.calculationType" class="text-red-500 text-sm mt-1">{{ errors.calculationType }}</span>
            </div>
          </div>

          <!-- Tipo de Beneficio -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">
                Tipo de Beneficio *
              </label>
              <select
                v-model="computedType"
                :disabled="form.calculationType === 'API'"
                :class="[
                  'neumorphism-input w-full', 
                  errors.type ? 'ring-2 ring-red-500' : '',
                  form.calculationType === 'API' ? 'bg-gray-100 cursor-not-allowed' : ''
                ]"
                @change="validateType"
              >
                <option value="">Seleccione el tipo de beneficio</option>
                <option value="Bonificación">Bonificación</option>
                <option value="Descuento">Descuento</option>
                <option value="Ambos">Ambos</option>
              </select>
              <span v-if="errors.type" class="text-red-500 text-sm mt-1">{{ errors.type }}</span>
            </div>

            <!-- Empresa -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">
                Empresa *
              </label>
              <select
                v-model="form.companyId"
                :disabled="isProjectSelected"
                :class="[
                  'neumorphism-input w-full', 
                  errors.companyId ? 'ring-2 ring-red-500' : '',
                  isProjectSelected ? 'bg-gray-100 cursor-not-allowed' : ''
                ]"
                @change="validateCompanyId"
              >
                <option value="">Seleccione la empresa</option>
                <option v-for="company in companies" :key="company.id" :value="company.id">
                  {{ company.nombre }}
                </option>
              </select>
              <span v-if="errors.companyId" class="text-red-500 text-sm mt-1">{{ errors.companyId }}</span>
            </div>
          </div>

          <!-- Conditional Fields based on Calculation Type -->
          <div v-if="form.calculationType === 'Porcentaje'" class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">
                Porcentaje *
              </label>
              <div class="relative">
                <input
                  v-model="form.percentage"
                  type="number"
                  min="0"
                  max="100"
                  :class="['neumorphism-input w-full pr-8 no-spinner', errors.percentage ? 'ring-2 ring-red-500' : '']"
                  placeholder="Ej: 15% "
                  @blur="validatePercentage"
                />
                <span class="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-500 font-medium">%</span>
              </div>
              <span v-if="errors.percentage" class="text-red-500 text-sm mt-1">{{ errors.percentage }}</span>
            </div>
          </div>

          <div v-if="form.calculationType === 'Monto Fijo'" class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">
                Valor *
              </label>
              <div class="relative">
                <input
                  v-model="formattedValue"
                  type="text"
                  :class="['neumorphism-input w-full pr-8', errors.value ? 'ring-2 ring-red-500' : '']"
                  placeholder="Ej: 50,000"
                  @input="handleValueInput"
                  @blur="validateValue"
                />
                <span class="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-500 font-medium">₡</span>
              </div>
              <span v-if="errors.value" class="text-red-500 text-sm mt-1">{{ errors.value }}</span>
            </div>
          </div>

          <div class="flex flex-col sm:flex-row gap-4 pt-6">
            <button
              type="submit"
              :disabled="isSubmitting"
              class="flex-1 neumorphism-dark px-6 py-3 rounded-lg text-white hover:bg-blue-700 transition disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <span v-if="isSubmitting" class="flex items-center justify-center">
                <svg class="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                  <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                  <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                Agregando...
              </span>
              <span v-else>Agregar Beneficio</span>
            </button>

            <button
              type="button"
              @click="goBack"
              class="flex-1 neumorphism-light px-6 py-3 rounded-lg text-gray-700 hover:bg-gray-100 transition"
            >
              Cancelar
            </button>
          </div>
        </form>
      </div>

      <!-- Loading State -->
      <div v-if="isLoadingBenefits" class="neumorphism-card bg-[#E9F7FF] p-8 rounded-2xl">
        <div class="flex items-center justify-center">
          <svg class="animate-spin -ml-1 mr-3 h-8 w-8 text-blue-600" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
            <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
          </svg>
          <span class="text-lg text-gray-600">Cargando beneficios...</span>
        </div>
      </div>

      <!-- Benefits List -->
      <div v-else-if="benefits.length > 0" class="neumorphism-card bg-[#E9F7FF] p-8 rounded-2xl">
        <h2 class="text-2xl font-bold mb-6 text-gray-800">Beneficios Existentes</h2>
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          <div
            v-for="benefit in benefits"
            :key="`${benefit.companyId}-${benefit.name}`"
            class="neumorphism-card p-4 rounded-lg hover:shadow-lg transition"
          >
            <h3 class="font-semibold text-lg text-gray-800">{{ benefit.name }}</h3>
            <p class="text-sm text-gray-600 mt-1">
              <span class="font-medium">Tipo:</span> {{ benefit.type }}
            </p>
            <p class="text-sm text-gray-600">
              <span class="font-medium">Cálculo:</span> {{ benefit.calculationType }}
            </p>
            <p v-if="benefit.calculationType === 'Porcentaje' && benefit.percentage" class="text-sm text-gray-600">
              <span class="font-medium">Porcentaje:</span> {{ benefit.percentage }}%
            </p>
            <p v-if="benefit.calculationType === 'Monto Fijo' && benefit.value" class="text-sm text-gray-600">
              <span class="font-medium">Valor:</span> ₡{{ benefit.value.toLocaleString() }}
            </p>
            <p class="text-sm text-gray-600">
              <span class="font-medium">Empresa:</span> {{ benefit.companyName }}
            </p>
          </div>
        </div>
      </div>

      <!-- No Benefits Message -->
      <div v-else-if="!isLoadingBenefits" class="neumorphism-card bg-[#E9F7FF] p-8 rounded-2xl">
        <div class="text-center text-gray-600">
          <svg class="mx-auto h-12 w-12 text-gray-400 mb-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
          </svg>
          <h3 class="text-lg font-medium text-gray-900 mb-2">No hay beneficios registrados</h3>
          <p class="text-gray-500">Los beneficios que agregues aparecerán aquí.</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import MainEmployerHeader from '../../common/MainEmployerHeader.vue'
import { apiConfig } from '../../../config/api.js'

export default {
  name: 'AddBenefit',
  components: {
    MainEmployerHeader
  },
  data() {
    return {
      form: {
        name: '',
        calculationType: '',
        type: '',
        companyId: '',
        value: '',
        percentage: ''
      },
      errors: {},
      isSubmitting: false,
      successMessage: '',
      errorMessage: '',
      companies: [],
      benefits: [],
      selectedProject: null,
      isLoadingBenefits: false
    }
  },
  computed: {
    isProjectSelected() {
      return this.selectedProject !== null;
    },
    projectDisplayName() {
      return this.selectedProject ? this.selectedProject.nombre : 'No seleccionado';
    },
    computedType: {
      get() {
        return this.form.calculationType === 'API' ? 'Ambos' : this.form.type;
      },
      set(value) {
        if (this.form.calculationType !== 'API') {
          this.form.type = value;
        }
      }
    },
    formattedValue: {
      get() {
        if (!this.form.value || this.form.value === '') return '';
        return this.formatNumber(this.form.value);
      }
    }
  },
  watch: {
    'form.calculationType'(newValue) {
      // Clear conditional fields when calculation type changes
      this.form.value = '';
      this.form.percentage = '';
      
      // Auto-set type to 'Ambos' when API is selected
      if (newValue === 'API') {
        this.form.type = 'Ambos';
      } else if (this.form.type === 'Ambos' && newValue !== 'API') {
        // Clear type if it was set to 'Ambos' and user changes away from API
        this.form.type = '';
      }
    }
  },
  mounted() {
    this.initializeProject();
    this.fetchCompanies();
  },
  methods: {
    formatNumber(value) {
      const numericValue = value.toString().replace(/[^0-9]/g, '');
      if (!numericValue) return '';
      
      return parseInt(numericValue).toLocaleString('es-CR');
    },
    handleValueInput(event) {
      const inputValue = event.target.value;
      // Remove any non-numeric characters except commas
      const numericValue = inputValue.replace(/[^0-9]/g, '');
      
      // Update the form value with the numeric value
      this.form.value = numericValue;
      
      // Update the display value with formatting
      this.$nextTick(() => {
        if (numericValue) {
          event.target.value = this.formatNumber(numericValue);
        }
      })
    },
    clearErrors() {
      this.errors = {};
      this.errorMessage = '';
    },
    validateName() {
      if (!this.form.name.trim()) {
        this.errors.name = 'El nombre del beneficio es obligatorio';
      } else if (this.form.name.trim().length > 20) {
        this.errors.name = 'El nombre no puede exceder 20 caracteres';
      } else if (!/^[a-zA-ZÀ-ÿ\u00f1\u00d1\s]+$/.test(this.form.name.trim())) {
        this.errors.name = 'El nombre solo puede contener letras y espacios';
      }
    },
    validateCalculationType() {
      if (!this.form.calculationType) {
        this.errors.calculationType = 'El tipo de cálculo es obligatorio';
      }
    },
    validateType() {
      const typeValue = this.form.calculationType === 'API' ? 'Ambos' : this.form.type;
      if (!typeValue) {
        this.errors.type = 'El tipo de beneficio es obligatorio';
      }
    },
    validateCompanyId() {
      if (!this.form.companyId) {
        this.errors.companyId = 'Debe seleccionar una empresa';
      }
    },
    validatePercentage() {
      if (this.form.calculationType === 'Porcentaje') {
        if (!this.form.percentage || this.form.percentage <= 0 || this.form.percentage > 100) {
          this.errors.percentage = 'El porcentaje debe estar entre 1 y 100';
        }
      }
    },
    validateValue() {
      if (this.form.calculationType === 'Monto Fijo') {
        const numericValue = parseInt(this.form.value) || 0;
        if (!this.form.value || numericValue <= 0) {
          this.errors.value = 'El valor debe ser mayor a 0';
        }
      }
    },
    validateForm() {
      this.clearErrors();
      let isValid = true;

      if (!this.form.name.trim()) {
        this.errors.name = 'El nombre del beneficio es obligatorio';
        isValid = false;
      } else if (this.form.name.trim().length > 20) {
        this.errors.name = 'El nombre no puede exceder 20 caracteres';
        isValid = false;
      } else if (!/^[a-zA-ZÀ-ÿ\u00f1\u00d1\s]+$/.test(this.form.name.trim())) {
        this.errors.name = 'El nombre solo puede contener letras y espacios';
        isValid = false;
      }

      if (!this.form.calculationType) {
        this.errors.calculationType = 'El tipo de cálculo es obligatorio';
        isValid = false;
      }

      const typeValue = this.form.calculationType === 'API' ? 'Ambos' : this.form.type;
      if (!typeValue) {
        this.errors.type = 'El tipo de beneficio es obligatorio';
        isValid = false;
      }

      if (this.form.calculationType === 'Porcentaje') {
        if (!this.form.percentage || this.form.percentage <= 0 || this.form.percentage > 100) {
          this.errors.percentage = 'El porcentaje debe estar entre 1 y 100';
          isValid = false;
        }
      }

      if (this.form.calculationType === 'Monto Fijo') {
        const numericValue = parseInt(this.form.value) || 0;
        if (!this.form.value || numericValue <= 0) {
          this.errors.value = 'El valor debe ser mayor a 0';
          isValid = false;
        }
      }

      if (!this.form.companyId) {
        this.errors.companyId = 'Debe seleccionar una empresa';
        isValid = false;
      }

      return isValid;
    },
    async handleSubmit() {
      if (!this.validateForm()) {
        return;
      }

      this.isSubmitting = true;
      this.clearErrors();

      try {
        const requestData = {
          CompanyId: parseInt(this.form.companyId),
          Name: this.form.name.trim(),
          CalculationType: this.form.calculationType,
          Type: this.form.calculationType === 'API' ? 'Ambos' : this.form.type,
          Value: this.form.calculationType === 'Monto Fijo' && this.form.value ? parseInt(this.form.value) : null,
          Percentage: this.form.calculationType === 'Porcentaje' && this.form.percentage ? parseInt(this.form.percentage) : null
        };
        
        const response = await fetch(apiConfig.endpoints.benefit, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify(requestData)
        })

        const data = await response.json();

        if (response.ok) {
          this.successMessage = 'Beneficio agregado exitosamente';
          this.form = {
            name: '',
            calculationType: '',
            type: '',
            companyId: this.isProjectSelected ? this.selectedProject.id : '',
            value: '',
            percentage: ''
          }
          // Refresh the benefits list
          await this.fetchBenefitsByProject(this.selectedProject.id);
          
          // Auto-redirect after 2 seconds if coming from a project
          if (this.isProjectSelected) {
            setTimeout(() => {
              this.$router.push(`/dashboard-project/${this.selectedProject.id}`);
            }, 2000);
          }
        } else {
          this.errorMessage = data.message || 'Error al agregar el beneficio';
        }
      } catch (error) {
        this.errorMessage = 'Error de conexión. Por favor, intente nuevamente.';
      } finally {
        this.isSubmitting = false;
      }
    },
    async fetchCompanies() {
      try {
        const response = await fetch(apiConfig.endpoints.project);
        if (!response.ok) throw new Error('No se pudo cargar las empresas');
        this.companies = await response.json()
      } catch (err) {
        this.errorMessage = 'Error al cargar las empresas';
      }
    },
    goBack() {
      if (this.isProjectSelected) {
        this.$router.push(`/dashboard-project/${this.selectedProject.id}`);
      } else {
        this.$router.push('/dashboard-main-employer');
      }
    },
    async initializeProject() {
      const storedProject = localStorage.getItem('selectedProject');
      
      if (storedProject) {
        try {
          this.selectedProject = JSON.parse(storedProject);
          this.form.companyId = this.selectedProject.id;
          // Load benefits for the stored project
          await this.fetchBenefitsByProject(this.selectedProject.id);
        } catch (err) {
          this.errorMessage = 'Error al cargar el proyecto almacenado';
        }
      } else {
        const projectId = this.$route.params.projectId || this.projectId
        
        if (projectId) {
          try {
            const response = await fetch(apiConfig.endpoints.projectById(projectId));
            if (response.ok) {
              this.selectedProject = await response.json();
              this.form.companyId = this.selectedProject.id;
              // Store in localStorage for future use
              localStorage.setItem('selectedProject', JSON.stringify(this.selectedProject));
              // Load benefits for the fetched project
              await this.fetchBenefitsByProject(this.selectedProject.id);
            }
          } catch (err) {
            this.errorMessage = 'Error al cargar el proyecto'
          }
        }
      }
    },
    async fetchBenefitsByProject(projectId) {
      this.isLoadingBenefits = true;
      try {
        const response = await fetch(apiConfig.endpoints.benefitByCompany(projectId));
        
        if (response.ok) {
          this.benefits = await response.json();
        } else {
          const errorData = await response.json().catch(() => ({}));
          this.errorMessage = errorData.message || 'Error al cargar los beneficios del proyecto';
        }
      } catch (err) {
        this.errorMessage = 'Error al cargar los beneficios del proyecto';
      } finally {
        this.isLoadingBenefits = false;
      }
    }
  }
}
</script>


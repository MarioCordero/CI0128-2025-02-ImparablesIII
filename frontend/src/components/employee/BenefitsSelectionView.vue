<template>
  <div class="body m-0! p-0!">
    
    <h2 class="text-xl font-semibold">Beneficios seleccionados</h2>
    
    <div class="">
      <!-- Selection Counter -->

      <div class="flex items-center justify-between mb-3 neumorphism-card">
        <div>
          <p class="text-sm text-gray-600">Beneficios seleccionados</p>
          <p class="text-2xl font-bold text-[#4a5568]">
            {{ benefitsData.currentSelections }} / {{ benefitsData.maxSelections }}
          </p>
        </div>
        <div v-if="canSelectMore" class="text-right">
          <span class="text-[#4a5568] text-sm font-semibold">
            Puedes seleccionar {{ benefitsData.maxSelections - benefitsData.currentSelections }} más
          </span>
        </div>
        <div v-else class="text-right">
          <span class="text-red-600 text-sm font-semibold">
            Has alcanzado el límite máximo
          </span>
        </div>
      </div>




      
      <!-- Selected Benefits List -->
      <div v-if="benefitsData.selectedBenefits.length > 0" class="pt-3">

        <div v-if="!loading && !error" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-10">
          <!-- Benefit Card -->
          <div
            v-for="benefit in benefitsData.selectedBenefits"
            :key="`selected-${benefit.benefitName}`"
            class="neumorphism-card h-full space-y-[24px] border-2! border-[#1048ff]!"
          >
            <!-- Card Header -->
            <div class="mb-2 w-full">
              <div class="w-full flex justify-between items-top">

                <div>
                  <h3 class="text-lg font-bold text-gray-800 truncate">{{ benefit.benefitName }}</h3>
                  <div v-if="benefit.benefitDescription" class="">
                    <p class="text-gray-800 text-[16px] mt-1 benefit-description">{{ benefit.benefitDescription }}</p>
                  </div>
                  <div v-else class="">
                    <p class="text-gray-800 text-[16px] mt-1 benefit-description">No hay descripción disponible para este beneficio.</p>
                  </div>
                </div>
              </div>
            </div>

            <!-- Content card -->
            <div class="text-[18px] grid grid-cols-1 gap-auto h-[216px]">
              <div class="flex justify-between">
                <span class="text-gray-600 font-medium">Tipo:</span>
                <span class="text-gray-800">{{ benefit.benefitType }}</span>
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

              <!-- Employees using benefit -->
              <div class="flex justify-between items-center">
                <span class="text-gray-600 font-medium">Empleados:</span>
                <div class="flex items-center gap-2">
                  <span class="text-gray-800 font-semibold">{{ benefit.employeeCount }}</span>
                  <span
                    class="text-xs text-[#4a5568]"
                    :title="`${benefit.usagePercentage.toFixed(1)}% de empleados usan este beneficio`"
                  >
                    ({{ benefit.usagePercentage.toFixed(1) }}%)
                  </span>
                </div>
              </div>

              <!-- Selection Action Button -->
              <div class="mt-4 w-full">
                <button
                  @click="deselectBenefit(benefit)"
                  class="neumorphism-button-normal-dark w-full text-white!"
                >
                  Remover
                </button>
              </div>
            </div>
          </div>
        </div>










      </div>
      <div v-else class="border-t border-gray-200 pt-3">
        <p class="text-sm text-gray-500 italic">No hay beneficios seleccionados aún</p>
      </div>

    </div>


    <h2 class="text-xl font-semibold">Beneficios disponibles</h2>

    <!-- Search and Filters -->
    <div class="">
      <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
        <!-- Search Input -->
        <div class="md:col-span-2">
          <input
            v-model="searchTerm"
            type="text"
            placeholder="Buscar beneficios..."
            class="neumorphism-input"
            @input="fetchBenefits"
          />
        </div>

        <!-- Filter: Calculation Type -->
        <div>
          <select
            v-model="selectedCalculationType"
            class="neumorphism-input neumorphism-input-select"
            @change="fetchBenefits"
          >
            <option value="">Todos los tipos</option>
            <option value="Monto Fijo">Monto Fijo</option>
            <option value="Porcentaje">Porcentaje</option>
            <option value="API">Variable</option>
          </select>
        </div>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="flex justify-center items-center py-12">
      <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
    </div>

    <!-- Error State -->
    <div v-if="error && !loading" class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
      {{ error }}
    </div>

    <!-- Benefits Grid -->
    <div v-if="!loading && !error" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-10">

      <!-- Benefit Card -->
      <div
        v-for="benefit in filteredBenefits"
        :key="`${benefit.companyId}-${benefit.benefitName}`"
        class="neumorphism-card h-full space-y-[24px]"
      >
        <!-- Card Header -->
        <div class="mb-2 w-full">
          <div class="w-full flex justify-between items-top">

            <div>
              <h3 class="text-lg font-bold text-gray-800 truncate">{{ benefit.benefitName }}</h3>
              <div v-if="benefit.benefitDescription" class="">
                <p class="text-gray-800 text-[16px] mt-1 benefit-description">{{ benefit.benefitDescription }}</p>
              </div>
              <div v-else class="">
                <p class="text-gray-800 text-[16px] mt-1 benefit-description">No hay descripción disponible para este beneficio.</p>
              </div>
            </div>
          </div>
        </div>

        <!-- Content card -->
        <div class="text-[18px] grid grid-cols-1 gap-auto h-[216px]">
          <div class="flex justify-between">
            <span class="text-gray-600 font-medium">Tipo:</span>
            <span class="text-gray-800">{{ benefit.benefitType }}</span>
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

          <!-- Employees using benefit -->
          <div class="flex justify-between items-center">
            <span class="text-gray-600 font-medium">Empleados:</span>
            <div class="flex items-center gap-2">
              <span class="text-gray-800 font-semibold">{{ benefit.employeeCount }}</span>
              <span
                class="text-xs text-[#4a5568]"
                :title="`${benefit.usagePercentage.toFixed(1)}% de empleados usan este beneficio`"
              >
                ({{ benefit.usagePercentage.toFixed(1) }}%)
              </span>
            </div>
          </div>

          <!-- Selection Action Button -->
          <div class="mt-4 w-full">
            <button
              @click="showBenefitSelectionModal(benefit)"
              :disabled="!canSelectMore || benefit.isDeleted"
              class="neumorphism-button-normal-light w-full text-black!"
              :class="(!canSelectMore || benefit.isDeleted) ? 'bg-gray-300 text-gray-600' : 'bg-[#4a5568] text-white hover:bg-[#374151]'"
            >
              <template v-if="benefit.isDeleted">No disponible</template>
              <template v-else>{{ canSelectMore ? 'Agregar' : 'Límite alcanzado' }}</template>
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Empty State -->
    <div v-if="!loading && !error && filteredBenefits.length === 0" class="text-center py-12">
      <p class="text-gray-500 text-lg">No se encontraron beneficios con los filtros seleccionados</p>
    </div>

    <teleport to="body"> 
      <!-- Benefit Selection Modal -->
      <div v-if="showModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
        <div class="neumorphism-card-modal p-6! shadow-none! max-w-lg">
          <h3 class="text-lg font-bold text-gray-800 mb-4">
            Configurar {{ selectedBenefit?.benefitName }}
          </h3>
          
          <!-- Seguro Privado Fields -->
          <div v-if="selectedBenefit?.benefitName === 'Seguro privado'" class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">
                Número de Dependientes *
              </label>
              <input
                v-model="benefitForm.NumDependents"
                type="number"
                min="0"
                class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                placeholder="Ej: 2"
              />
              <p class="text-sm text-gray-500 mt-1">Hermanos o familiares que debe cuidar</p>
            </div>
          </div>
  
          <!-- Pensiones Voluntarias Fields -->
          <div v-if="selectedBenefit?.benefitName === 'Pensiones voluntarias'" class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">
                Tipo de Pensión *
              </label>
              <select
                v-model="benefitForm.PensionType"
                class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              >
                <option value="">Seleccione el tipo de pensión</option>
                <option value="A">Tipo A</option>
                <option value="B">Tipo B</option>
                <option value="C">Tipo C</option>
              </select>
            </div>
          </div>
  
          <!-- Error Message -->
          <div v-if="modalError" class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
            {{ modalError }}
          </div>
  
          <!-- Modal Actions -->
          <div class="flex justify-end mt-6 gap-3">
            <button
              @click="closeModal"
              class="neumorphism-button-normal-light"
            >
              Cancelar
            </button>
            <button
              @click="confirmBenefitSelection"
              :disabled="!isFormValid"
              class="neumorphism-button-normal-blue"
            >
              Confirmar
            </button>
          </div>
        </div>
      </div>
    </teleport>


  </div>
</template>

<script>
import { apiConfig } from '@/config/api.js'

export default {
  name: 'BenefitsSelectionView',
  props: {
    employeeId: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      benefitsData: {
        availableBenefits: [],
        selectedBenefits: [],
        currentSelections: 0,
        maxSelections: 3
      },
      loading: false,
      error: null,
      searchTerm: '',
      selectedCalculationType: '',
      selectedStatus: '',
      showModal: false,
      selectedBenefit: null,
      benefitForm: {
        NumDependents: '',
        PensionType: ''
      },
      modalError: null
    }
  },
  computed: {
    canSelectMore() {
      return this.benefitsData.currentSelections < this.benefitsData.maxSelections
    },
    filteredBenefits() {
      return (this.benefitsData.availableBenefits || []).filter(b => !b.isDeleted)
    },
    isFormValid() {
      if (!this.selectedBenefit) return false
      
      if (this.selectedBenefit.benefitName === 'Seguro privado') {
        return this.benefitForm.NumDependents !== '' && parseInt(this.benefitForm.NumDependents) >= 0
      }
      
      if (this.selectedBenefit.benefitName === 'Pensiones voluntarias') {
        return this.benefitForm.PensionType !== '' && ['A', 'B', 'C'].includes(this.benefitForm.PensionType)
      }
      
      return true
    }
  },
  created() {
    this.fetchBenefits()
  },
  methods: {
    async fetchBenefits() {
      this.loading = true
      this.error = null

      try {
        const params = new URLSearchParams()
        if (this.searchTerm) params.append('searchTerm', this.searchTerm)
        if (this.selectedCalculationType) params.append('calculationType', this.selectedCalculationType)
        if (this.selectedStatus) params.append('status', this.selectedStatus)

        const response = await fetch(`${apiConfig.endpoints.employeeBenefits(this.employeeId)}?${params}`)
        
        if (!response.ok) {
          throw new Error('Error al obtener los beneficios')
        }

        this.benefitsData = await response.json()
      } catch (error) {
        console.error('Error fetching benefits:', error)
        this.error = 'Error al cargar los beneficios. Por favor, intenta de nuevo.'
        this.$emit('error', this.error)
      } finally {
        this.loading = false
      }
    },
    async selectBenefit(benefit) {
      this.loading = true
      this.error = null

      try {
        const response = await fetch(`${apiConfig.endpoints.employeeBenefitsSelect(this.employeeId)}`, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json'
          },
          body: JSON.stringify({
            benefitName: benefit.benefitName
          })
        })

        const result = await response.json()

        if (response.ok && result.success) {
          this.benefitsData.currentSelections = result.currentSelections
          this.benefitsData.maxSelections = result.maxSelections
          // Refresh benefits to update selection status
          await this.fetchBenefits()
          this.$emit('success', result.message || 'Beneficio agregado exitosamente')
        } else {
          this.error = result.message || 'Error al agregar el beneficio'
          this.$emit('error', this.error)
        }
      } catch (error) {
        console.error('Error selecting benefit:', error)
        this.error = 'Error al agregar el beneficio'
        this.$emit('error', this.error)
      } finally {
        this.loading = false
      }
    },

    async deselectBenefit(benefit) {
      this.loading = true
      this.error = null

      try {
        const response = await fetch(`${apiConfig.endpoints.employeeBenefitsDeselect(this.employeeId, benefit.benefitName)}`, {
          method: 'DELETE'
        })

        const result = await response.json()

        if (response.ok && result.success) {
          this.benefitsData.currentSelections = result.currentSelections
          this.benefitsData.maxSelections = result.maxSelections
          await this.fetchBenefits()
          this.$emit('success', result.message || 'Beneficio eliminado exitosamente')
        } else {
          this.error = result.message || 'Error al eliminar el beneficio'
          this.$emit('error', this.error)
        }
      } catch (error) {
        console.error('Error deselecting benefit:', error)
        this.error = 'Error al eliminar el beneficio'
        this.$emit('error', this.error)
      } finally {
        this.loading = false
      }
    },

    getCalculationTypeBadgeClass(type) {
      const classes = {
        'Monto Fijo': 'bg-blue-100 text-blue-800',
        'Porcentaje': 'bg-purple-100 text-purple-800',
        'API': 'bg-orange-100 text-orange-800'
      }
      return classes[type] || 'bg-gray-100 text-gray-800'
    },

    showBenefitSelectionModal(benefit) {
      if (benefit.isDeleted) {
        return
      }
      this.selectedBenefit = benefit
      this.benefitForm = {
        NumDependents: '',
        PensionType: ''
      }
      this.modalError = null
      this.showModal = true
    },

    closeModal() {
      this.showModal = false
      this.selectedBenefit = null
      this.benefitForm = {
        NumDependents: '',
        PensionType: ''
      }
      this.modalError = null
    },

    async confirmBenefitSelection() {
      if (!this.isFormValid) return

      this.loading = true
      this.modalError = null

      try {
        const requestData = {
          benefitName: this.selectedBenefit.benefitName,
          NumDependents: this.selectedBenefit.benefitName === 'Seguro privado' ? parseInt(this.benefitForm.NumDependents) : null,
          PensionType: this.selectedBenefit.benefitName === 'Pensiones voluntarias' ? this.benefitForm.PensionType : null
        }

        const response = await fetch(`${apiConfig.endpoints.employeeBenefitsSelect(this.employeeId)}`, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json'
          },
          body: JSON.stringify(requestData)
        })

        const result = await response.json()

        if (response.ok && result.success) {
          this.benefitsData.currentSelections = result.currentSelections
          this.benefitsData.maxSelections = result.maxSelections
          // Refresh benefits to update selection status
          await this.fetchBenefits()
          this.closeModal()
          this.$emit('success', result.message || 'Beneficio agregado exitosamente')
        } else {
          this.modalError = result.message || 'Error al agregar el beneficio'
        }
      } catch (error) {
        console.error('Error selecting benefit:', error)
        this.modalError = 'Error al agregar el beneficio'
      } finally {
        this.loading = false
      }
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

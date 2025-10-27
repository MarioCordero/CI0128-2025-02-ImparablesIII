<template>
  <div class="benefits-selection-container">
    <!-- Header and Summary -->
    <div class="mb-8">
      <h1 class="text-3xl font-bold text-gray-800 mb-2">Selección de Beneficios</h1>
      <p class="text-gray-600">Personaliza tus beneficios según tus necesidades</p>
      
      <!-- Selection Counter -->
      <div class="mt-4 p-4 bg-white rounded-lg shadow-sm border-2 border-[#4a5568]/30">
        <div class="flex items-center justify-between mb-3">
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
        <div v-if="benefitsData.selectedBenefits.length > 0" class="border-t border-gray-200 pt-3">
          <p class="text-sm font-semibold text-gray-700 mb-2">Beneficios activos:</p>
          <div class="flex flex-wrap gap-2">
            <div
              v-for="benefit in benefitsData.selectedBenefits"
              :key="`selected-${benefit.benefitName}`"
              class="flex items-center gap-2 px-3 py-1.5 bg-green-50 border border-green-200 rounded-lg text-sm"
            >
              <span class="text-green-700 font-medium">{{ benefit.benefitName }}</span>
            </div>
          </div>
        </div>
        <div v-else class="border-t border-gray-200 pt-3">
          <p class="text-sm text-gray-500 italic">No hay beneficios seleccionados aún</p>
        </div>
      </div>
    </div>

    <!-- Search and Filters -->
    <div class="mb-6 bg-white rounded-lg shadow-sm p-4">
      <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
        <!-- Search Input -->
        <div class="md:col-span-2">
          <input
            v-model="searchTerm"
            type="text"
            placeholder="Buscar beneficios..."
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            @input="fetchBenefits"
          />
        </div>

        <!-- Filter: Calculation Type -->
        <div>
          <select
            v-model="selectedCalculationType"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
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
    <div v-if="!loading && !error" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <div
        v-for="benefit in filteredBenefits"
        :key="`${benefit.companyId}-${benefit.benefitName}`"
        class="bg-white rounded-lg shadow-md hover:shadow-lg transition-shadow duration-300 overflow-hidden"
        :class="getBenefitCardClass(benefit)"
      >
        <!-- Card Header -->
        <div class="p-4 bg-[#4a5568] text-white">
          <div class="flex items-center justify-between">
            <h3 class="text-lg font-bold">{{ benefit.benefitName }}</h3>
            <span
              v-if="benefit.isSelected"
              class="px-2 py-1 bg-green-500 rounded-full text-xs font-semibold"
            >
              Seleccionado
            </span>
          </div>
        </div>

        <!-- Card Body -->
        <div class="p-4">
          <!-- Calculation Type Badge -->
          <div class="mb-3">
            <span
              class="px-3 py-1 rounded-full text-xs font-semibold"
              :class="getCalculationTypeBadgeClass(benefit.calculationType)"
            >
              {{ benefit.calculationType }}
            </span>
          </div>

          <!-- Benefit Details -->
          <div class="space-y-2 text-sm mb-4">
            <div class="flex justify-between">
              <span class="text-gray-600 font-medium">Tipo:</span>
              <span class="text-gray-800">{{ benefit.benefitType }}</span>
            </div>
            
            <div v-if="benefit.value" class="flex justify-between">
              <span class="text-gray-600 font-medium">Valor:</span>
              <span class="text-gray-800 font-semibold">₡{{ benefit.value.toLocaleString() }}</span>
            </div>
            
            <div v-if="benefit.percentage" class="flex justify-between">
              <span class="text-gray-600 font-medium">Porcentaje:</span>
              <span class="text-gray-800 font-semibold">{{ benefit.percentage }}%</span>
            </div>
            
            <div v-if="!benefit.value && !benefit.percentage" class="flex justify-between">
              <span class="text-gray-600 font-medium">Valor:</span>
              <span class="text-gray-800 font-semibold">Variable</span>
            </div>

            <!-- Employees using benefit -->
            <div class="flex justify-between items-center pt-2 border-t border-gray-200">
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
          </div>

          <!-- Selection Action Button -->
          <div class="flex items-center justify-between pt-3 border-t border-gray-200">
            
            <div v-if="benefit.isSelected">
              <button
                @click="deselectBenefit(benefit)"
                class="px-4 py-2 bg-red-500 text-white rounded-lg hover:bg-red-600 transition-colors duration-200 text-sm font-medium"
              >
                Remover
              </button>
            </div>
            <div v-else>
              <button
                @click="selectBenefit(benefit)"
                :disabled="!canSelectMore"
                class="px-4 py-2 rounded-lg transition-colors duration-200 text-sm font-medium disabled:cursor-not-allowed disabled:opacity-50"
                :class="canSelectMore ? 'bg-[#4a5568] text-white hover:bg-[#374151]' : 'bg-gray-300 text-gray-600'"
              >
                {{ canSelectMore ? 'Agregar' : 'Límite alcanzado' }}
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Empty State -->
    <div v-if="!loading && !error && filteredBenefits.length === 0" class="text-center py-12">
      <p class="text-gray-500 text-lg">No se encontraron beneficios con los filtros seleccionados</p>
    </div>
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
      selectedStatus: ''
    }
  },
  computed: {
    canSelectMore() {
      return this.benefitsData.currentSelections < this.benefitsData.maxSelections
    },
    filteredBenefits() {
      return this.benefitsData.availableBenefits || []
    },
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
    getBenefitCardClass(benefit) {
      if (benefit.isSelected) {
        return 'ring-2 ring-green-500 border-green-300'
      }
      return ''
    },
    getCalculationTypeBadgeClass(type) {
      const classes = {
        'Monto Fijo': 'bg-blue-100 text-blue-800',
        'Porcentaje': 'bg-purple-100 text-purple-800',
        'API': 'bg-orange-100 text-orange-800'
      }
      return classes[type] || 'bg-gray-100 text-gray-800'
    }
  }
}
</script>


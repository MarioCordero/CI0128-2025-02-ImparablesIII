<template>
  <!-- Modal Overlay -->
  <div 
    v-if="isVisible" 
    class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50"
    @click="handleOverlayClick"
  >
    <!-- Modal Content -->
    <div 
      class="bg-white rounded-2xl p-8 max-w-md w-full mx-4 shadow-2xl transform transition-all duration-300"
      @click.stop
    >
      <!-- Warning Icon -->
      <div class="flex justify-center mb-6">
        <div class="w-16 h-16 bg-red-100 rounded-full flex items-center justify-center">
          <svg class="w-8 h-8 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L3.732 16.5c-.77.833.192 2.5 1.732 2.5z"></path>
          </svg>
        </div>
      </div>

      <!-- Title -->
      <h2 class="text-2xl font-bold text-gray-900 text-center mb-4">
        {{ title }}
      </h2>

      <!-- Message -->
      <div class="text-gray-600 text-center mb-6 leading-relaxed">
        <p class="mb-2">{{ message }}</p>
      </div>

      <!-- Password Input (shown after first confirmation) -->
      <div v-if="firstConfirmed && !secondConfirmed" class="mb-6">
        <label class="block text-sm font-medium text-gray-700 mb-2">
          Confirme su contraseña para continuar:
        </label>
        <input
          v-model="password"
          type="password"
          class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-red-500 focus:border-transparent"
          placeholder="Ingrese su contraseña"
          @keyup.enter="confirmSecond"
        />
        <p v-if="passwordError" class="text-red-600 text-sm mt-2">{{ passwordError }}</p>
      </div>

      <!-- Motivo Baja Input (optional) -->
      <div v-if="firstConfirmed && !secondConfirmed" class="mb-6">
        <label class="block text-sm font-medium text-gray-700 mb-2">
          Motivo de eliminación (opcional):
        </label>
        <textarea
          v-model="motivoBaja"
          rows="3"
          class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-red-500 focus:border-transparent"
          placeholder="Ingrese el motivo de la eliminación del proyecto"
        ></textarea>
      </div>

      <!-- Confirmation Steps -->
      <div class="space-y-4">
        <!-- First Confirmation -->
        <div v-if="!firstConfirmed" class="text-center">
          <p class="text-sm text-gray-500 mb-3">
            Paso 1 de 2: ¿Estás seguro de que deseas continuar?
          </p>
          <div class="flex gap-3 justify-center">
            <button
              @click="confirmFirst"
              class="px-6 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors font-medium"
            >
              Sí, continuar
            </button>
            <button
              @click="cancel"
              class="px-6 py-2 bg-gray-300 text-gray-700 rounded-lg hover:bg-gray-400 transition-colors font-medium"
            >
              Cancelar
            </button>
          </div>
        </div>

        <!-- Second Confirmation -->
        <div v-if="firstConfirmed && !secondConfirmed" class="text-center">
          <p class="text-sm text-gray-500 mb-3">
            Paso 2 de 2: Esta acción no se puede deshacer. ¿Confirmar definitivamente?
          </p>
          <div class="flex gap-3 justify-center">
            <button
              @click="confirmSecond"
              :disabled="!password || isDeleting"
              class="px-6 py-2 bg-red-700 text-white rounded-lg hover:bg-red-800 transition-colors font-bold disabled:bg-gray-400 disabled:cursor-not-allowed"
            >
              {{ isDeleting ? 'Eliminando...' : 'Confirmar Definitivamente' }}
            </button>
            <button
              @click="resetConfirmation"
              :disabled="isDeleting"
              class="px-6 py-2 bg-gray-300 text-gray-700 rounded-lg hover:bg-gray-400 transition-colors font-medium disabled:bg-gray-200"
            >
              Volver Atrás
            </button>
          </div>
        </div>

        <!-- Success State -->
        <div v-if="secondConfirmed && !isDeleting" class="text-center">
          <div class="w-12 h-12 bg-green-100 rounded-full flex items-center justify-center mx-auto mb-3">
            <svg class="w-6 h-6 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7"></path>
            </svg>
          </div>
          <p class="text-green-600 font-medium mb-4">Acción confirmada</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'ProjectDeletionModal',
  props: {
    title: {
      type: String,
      default: 'Eliminar Empresa'
    },
    message: {
      type: String,
      required: true
    },
    isVisible: {
      type: Boolean,
      default: false
    }
  },
  data() {
    return {
      firstConfirmed: false,
      secondConfirmed: false,
      password: '',
      motivoBaja: '',
      passwordError: '',
      isDeleting: false
    }
  },
  emits: ['confirm', 'cancel', 'close'],
  methods: {
    confirmFirst() {
      this.firstConfirmed = true
      this.passwordError = ''
    },
    async confirmSecond() {
      if (!this.password) {
        this.passwordError = 'La contraseña es requerida'
        return
      }
      this.passwordError = ''
      this.isDeleting = true
      this.secondConfirmed = true

      setTimeout(() => {
        this.$emit('confirm', {
          password: this.password,
          motivoBaja: this.motivoBaja || null
        })
        this.closeModal()
      }, 500)
    },
    cancel() {
      this.$emit('cancel')
      this.closeModal()
    },
    resetConfirmation() {
      this.firstConfirmed = false
      this.secondConfirmed = false
      this.password = ''
      this.motivoBaja = ''
      this.passwordError = ''
      this.isDeleting = false
    },
    closeModal() {
      this.resetConfirmation()
      this.$emit('close')
    },
    handleOverlayClick() {
      if (!this.firstConfirmed) {
        this.cancel()
      }
    }
  },
  watch: {
    isVisible(newValue) {
      if (!newValue) {
        this.resetConfirmation()
      }
    }
  }
}
</script>
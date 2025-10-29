<template>
  <div v-if="visible" class="fixed inset-0 bg-black bg-opacity-40 flex items-center justify-center z-50">
    <div class="bg-[#E9F7FF] rounded-2xl p-8 w-full max-w-2xl shadow-lg relative neumorphism-card">
      <button class="absolute top-4 right-4 text-gray-500 hover:text-gray-700" @click="$emit('close')">✕</button>
      <h2 class="text-3xl font-black mb-6 text-black text-center">Editar Empresa</h2>
      <form @submit.prevent="submitEdit" class="space-y-6">
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <!-- Nombre de la Empresa -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Nombre de la Empresa *</label>
            <input
              v-model="form.nombre"
              type="text"
              required
              maxlength="20"
              :class="['neumorphism-input w-full', errors.nombre ? 'ring-2 ring-red-500' : '']"
              placeholder="Ingrese el nombre de la empresa"
            />
            <span v-if="errors.nombre" class="text-red-500 text-sm mt-1">{{ errors.nombre }}</span>
          </div>
          <!-- Cédula Jurídica -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Cédula Jurídica *</label>
            <input
              v-model="form.cedulaJuridica"
              type="number"
              required
              min="100000000"
              max="999999999"
              :class="['neumorphism-input w-full', errors.cedulaJuridica ? 'ring-2 ring-red-500' : '']"
              placeholder="Ingrese la cédula jurídica (9 dígitos)"
            />
            <span v-if="errors.cedulaJuridica" class="text-red-500 text-sm mt-1">{{ errors.cedulaJuridica }}</span>
          </div>
          <!-- Email -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Correo Electrónico *</label>
            <input
              v-model="form.email"
              type="email"
              required
              maxlength="50"
              :class="['neumorphism-input w-full', errors.email ? 'ring-2 ring-red-500' : '']"
              placeholder="empresa@ejemplo.com"
            />
            <span v-if="errors.email" class="text-red-500 text-sm mt-1">{{ errors.email }}</span>
          </div>
          <!-- Teléfono -->
          <div>
            <label class="block mb-1 font-medium text-gray-700">Número de teléfono*</label>
            <input
              v-model="formattedTelefono"
              type="tel"
              required
              @input="formatTelefono"
              :class="['neumorphism-input w-full', errors.telefono ? 'ring-2 ring-red-500' : '']"
              placeholder="####-####"
              maxlength="9"
            />
            <span v-if="errors.telefono" class="text-red-500 text-sm mt-1">{{ errors.telefono }}</span>
          </div>
          <!-- Período de Pago -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Período de Pago *</label>
            <select
              v-model="form.periodoPago"
              required
              :class="['neumorphism-input w-full', errors.periodoPago ? 'ring-2 ring-red-500' : '']"
            >
              <option value="">Seleccione período de pago</option>
              <option value="Mensual">Mensual</option>
              <option value="Quincenal">Quincenal</option>
            </select>
            <span v-if="errors.periodoPago" class="text-red-500 text-sm mt-1">{{ errors.periodoPago }}</span>
          </div>
          <!-- Máximo de Beneficios -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Máximo de Beneficios *</label>
            <input
              v-model="form.maximoBeneficios"
              type="number"
              required
              :class="['neumorphism-input w-full', errors.maximoBeneficios ? 'ring-2 ring-red-500' : '']"
              placeholder="Ingrese el máximo de beneficios"
            />
            <span v-if="errors.maximoBeneficios" class="text-red-500 text-sm mt-1">{{ errors.maximoBeneficios }}</span>
          </div>
        </div>
        <!-- Dirección -->
        <div class="mt-8">
          <h3 class="text-lg font-semibold text-gray-700 mb-4 neumorphism-card rounded-[12px] bg-[#E9F7FF] py-2 px-4">Dirección</h3>
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <!-- Provincia -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Provincia *</label>
              <select
                v-model="form.provincia"
                required
                :class="['neumorphism-input w-full', errors.provincia ? 'ring-2 ring-red-500' : '']"
              >
                <option value="">Seleccione provincia</option>
                <option value="San José">San José</option>
                <option value="Alajuela">Alajuela</option>
                <option value="Cartago">Cartago</option>
                <option value="Heredia">Heredia</option>
                <option value="Guanacaste">Guanacaste</option>
                <option value="Puntarenas">Puntarenas</option>
                <option value="Limón">Limón</option>
              </select>
              <span v-if="errors.provincia" class="text-red-500 text-sm mt-1">{{ errors.provincia }}</span>
            </div>
            <!-- Cantón -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Cantón</label>
              <input
                v-model="form.canton"
                type="text"
                maxlength="30"
                :class="['neumorphism-input w-full', errors.canton ? 'ring-2 ring-red-500' : '']"
                placeholder="Ingrese el cantón"
              />
              <span v-if="errors.canton" class="text-red-500 text-sm mt-1">{{ errors.canton }}</span>
            </div>
            <!-- Distrito -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Distrito</label>
              <input
                v-model="form.distrito"
                type="text"
                maxlength="30"
                :class="['neumorphism-input w-full', errors.distrito ? 'ring-2 ring-red-500' : '']"
                placeholder="Ingrese el distrito"
              />
              <span v-if="errors.distrito" class="text-red-500 text-sm mt-1">{{ errors.distrito }}</span>
            </div>
            <!-- Dirección Particular -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Dirección Particular</label>
              <input
                v-model="form.direccionParticular"
                type="text"
                maxlength="150"
                :class="['neumorphism-input w-full', errors.direccionParticular ? 'ring-2 ring-red-500' : '']"
                placeholder="Dirección específica (opcional)"
              />
              <span v-if="errors.direccionParticular" class="text-red-500 text-sm mt-1">{{ errors.direccionParticular }}</span>
            </div>
          </div>
        </div>
        <!-- Botones -->
        <div class="flex justify-end space-x-4 pt-6">
          <button
            type="button"
            @click="$emit('close')"
            class="neumorphism-light px-6 py-3 rounded-lg text-gray-700 hover:bg-gray-100 transition flex items-center space-x-2"
          >
            <span>Cancelar</span>
          </button>
          <button
            type="submit"
            class="neumorphism-dark px-6 py-3 rounded-lg text-white hover:bg-blue-600 transition flex items-center space-x-2"
          >
            <span>Guardar Cambios</span>
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script>
import apiConfig from '../../../config/api.js'

export default {
  name: 'EditCompanyModal',
  props: {
    visible: Boolean,
    company: Object
  },
  data() {
    return {
      form: {
        nombre: '',
        cedulaJuridica: '',
        email: '',
        telefono: '',
        periodoPago: '',
        maximoBeneficios: '',
        provincia: '',
        canton: '',
        distrito: '',
        direccionParticular: ''
      },
      formattedTelefono: '',
      errors: {}
    }
  },
  watch: {
    company: {
      handler(newVal) {
        if (newVal) {
          this.form = {
            nombre: newVal.nombre || '',
            cedulaJuridica: newVal.cedulaJuridica || '',
            email: newVal.email || '',
            telefono: newVal.telefono || '',
            periodoPago: newVal.periodoPago || '',
            maximoBeneficios: newVal.maximoBeneficios || '',
            provincia: newVal.direccion?.provincia || '',
            canton: newVal.direccion?.canton || '',
            distrito: newVal.direccion?.distrito || '',
            direccionParticular: newVal.direccion?.direccionParticular || ''
          }
          this.formattedTelefono = this.formatTelefonoString(this.form.telefono)
        }
      },
      immediate: true,
      deep: true
    }
  },
  methods: {
    async submitEdit() {
      if (!this.validateForm()) return

      const edited = {
        nombre: this.form.nombre.trim(),
        cedulaJuridica: parseInt(this.form.cedulaJuridica),
        email: this.form.email.trim(),
        telefono: parseInt(this.formattedTelefono.replace('-', '')),
        periodoPago: this.form.periodoPago,
        maximoBeneficios: parseInt(this.form.maximoBeneficios),
        direccion: {
          provincia: this.form.provincia,
          canton: this.form.canton,
          distrito: this.form.distrito,
          direccionParticular: this.form.direccionParticular
        }
      }

try {
  const projectId = this.company?.id
  const url = apiConfig.endpoints.updateProject(projectId)
  const response = await fetch(url, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(edited)
  })
  if (!response.ok) throw new Error('Error al actualizar la empresa')
  let updated = null
  if (response.status !== 204) {
    updated = await response.json()
  }
  this.$emit('save', updated)
} catch (error) {
  alert('No se pudo actualizar la empresa')
}
    },
    formatTelefono(event) {
      let value = event.target.value.replace(/\D/g, '')
      if (value.length > 4) {
        value = value.slice(0, 4) + '-' + value.slice(4, 8)
      }
      this.formattedTelefono = value
      this.form.telefono = value.replace('-', '')
    },
    formatTelefonoString(value) {
      value = value ? value.toString().replace(/\D/g, '') : ''
      if (value.length === 8) {
        return value.slice(0, 4) + '-' + value.slice(4, 8)
      }
      return value
    },
    validateForm() {
      this.errors = {}
      let isValid = true

      if (!this.form.nombre || this.form.nombre.trim().length === 0) {
        this.errors.nombre = 'El nombre es requerido'
        isValid = false
      } else if (this.form.nombre.length > 20) {
        this.errors.nombre = 'El nombre no puede exceder 20 caracteres'
        isValid = false
      }

      if (!this.form.cedulaJuridica || this.form.cedulaJuridica.toString().length !== 9) {
        this.errors.cedulaJuridica = 'La cédula jurídica debe tener 9 dígitos'
        isValid = false
      }

      if (!this.form.email || this.form.email.trim().length === 0) {
        this.errors.email = 'El correo electrónico es requerido'
        isValid = false
      } else if (this.form.email.length > 50) {
        this.errors.email = 'El correo no puede exceder 50 caracteres'
        isValid = false
      } else {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
        if (!emailRegex.test(this.form.email)) {
          this.errors.email = 'Formato de correo electrónico inválido'
          isValid = false
        }
      }

      if (this.form.telefono && this.form.telefono.toString().length !== 8) {
        this.errors.telefono = 'El teléfono debe tener 8 dígitos'
        isValid = false
      }

      if (!this.form.periodoPago) {
        this.errors.periodoPago = 'El período de pago es requerido'
        isValid = false
      }

      if (!this.form.provincia) {
        this.errors.provincia = 'La provincia es requerida'
        isValid = false
      }

      if (this.form.canton && this.form.canton.length > 30) {
        this.errors.canton = 'El cantón no puede exceder 30 caracteres'
        isValid = false
      }

      if (this.form.distrito && this.form.distrito.length > 30) {
        this.errors.distrito = 'El distrito no puede exceder 30 caracteres'
        isValid = false
      }

      if (this.form.direccionParticular && this.form.direccionParticular.length > 150) {
        this.errors.direccionParticular = 'La dirección no puede exceder 150 caracteres'
        isValid = false
      }

      if (!this.form.maximoBeneficios || this.form.maximoBeneficios < 1) {
        this.errors.maximoBeneficios = 'El máximo de beneficios debe ser mayor a 0'
        isValid = false
      }

      return isValid
    }
  }
}
</script>
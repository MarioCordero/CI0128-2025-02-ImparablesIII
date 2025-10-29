<template>
  <div v-if="visible" class="fixed inset-0 bg-black bg-opacity-40 flex items-center justify-center z-50">
    <div class="bg-white rounded-xl p-8 w-full max-w-lg shadow-lg relative">
      <button class="absolute top-4 right-4 text-gray-500 hover:text-gray-700" @click="$emit('close')">✕</button>
      <h2 class="text-2xl font-bold mb-4">Editar Empresa</h2>
      <form @submit.prevent="submitEdit">
        <div class="mb-4">
          <label class="block font-bold mb-1">Nombre</label>
          <input v-model="form.nombre" class="w-full border rounded px-3 py-2" />
        </div>
        <div class="mb-4">
          <label class="block font-bold mb-1">Cédula jurídica</label>
          <input v-model="form.cedulaJuridica" class="w-full border rounded px-3 py-2" />
        </div>
        <div class="mb-4">
          <label class="block font-bold mb-1">Correo electrónico</label>
          <input v-model="form.email" class="w-full border rounded px-3 py-2" />
        </div>
        <div class="mb-4">
          <label class="block font-bold mb-1">Teléfono</label>
          <input v-model="form.telefono" class="w-full border rounded px-3 py-2" />
        </div>
        <div class="mb-4">
          <label class="block font-bold mb-1">Periodo de pago</label>
          <input v-model="form.periodoPago" class="w-full border rounded px-3 py-2" />
        </div>
        <div class="mb-4">
          <label class="block font-bold mb-1">Máximo de beneficios elegibles</label>
          <input v-model="form.maximoBeneficios" class="w-full border rounded px-3 py-2" />
        </div>
        <div class="flex justify-end gap-2">
          <button type="button" class="px-4 py-2 rounded bg-gray-300" @click="$emit('close')">Cancelar</button>
          <button type="submit" class="px-4 py-2 rounded bg-blue-600 text-white">Guardar</button>
        </div>
      </form>
    </div>
  </div>
</template>

<script>
export default {
  name: 'EditCompanyModal',
  props: {
    visible: Boolean,
    company: Object
  },
  data() {
    return {
      form: { ...this.company }
    }
  },
  watch: {
    company: {
      handler(newVal) {
        this.form = { ...newVal }
      },
      deep: true
    }
  },
  methods: {
    submitEdit() {
      this.$emit('save', this.form)
    }
  }
}
</script>
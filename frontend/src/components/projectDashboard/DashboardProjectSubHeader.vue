<template>
  <div class="w-full flex justify-center">
    <div class="flex w-1/2 h-12 items-center justify-between relative bg-[#e9f7ff] rounded-b-md text-xs">
      <button
        class="self-stretch flex-auto px-3 py-3.5 rounded-md"
        :class="selectedSection === 'dashboard' ? 'bg-[#1347e5] text-[#fefefe]' : 'text-gray-700 hover:bg-blue-100'"
        @click="emit('section-change', 'dashboard')"
      >
        Dashboard de Empresa
      </button>
      <button
        class="self-stretch flex-auto px-3 py-3.5 rounded-md"
        :class="selectedSection === 'benefits' ? 'bg-[#1347e5] text-[#fefefe]' : 'text-gray-700 hover:bg-blue-100'"
        @click="emit('section-change', 'benefits')"
      >
        Gestionar Beneficios
      </button>
      <button
        class="self-stretch flex-auto px-3 py-3.5 rounded-md"
        :class="selectedSection === 'employees' ? 'bg-[#1347e5] text-[#fefefe]' : 'text-gray-700 hover:bg-blue-100'"
        @click="emit('section-change', 'employees')"
      >
        Gestionar Empleados
      </button>
      <button
        class="self-stretch flex-auto px-3 py-3.5 rounded-md"
        :class="selectedSection === 'info' ? 'bg-[#1347e5] text-[#fefefe]' : 'text-gray-700 hover:bg-blue-100'"
        @click="emit('section-change', 'info')"
      >
        Informacion de la Empresa
      </button>
      <button
        class="self-stretch flex-auto px-3 py-3.5 rounded-md"
        :class="selectedSection === 'reports' ? 'bg-[#1347e5] text-[#fefefe]' : 'text-gray-700 hover:bg-blue-100'"
        @click="emit('section-change', 'reports')"
      >
        Reportes de planilla
      </button>
    </div>
  </div>
</template>

<script setup>
import { defineProps, defineEmits } from 'vue'
defineProps({
  selectedSection: String
})
const emit = defineEmits(['section-change'])
</script>
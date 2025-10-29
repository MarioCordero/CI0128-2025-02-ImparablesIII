<template>
  <div class="w-full flex justify-center">
    <div class="flex w-1/2 h-12 items-center justify-between relative neumorfismo-subheader text-xs">
      <button
        class="self-stretch flex-auto px-3 py-3.5 neumorfismo-opcion-subheader-primero"
        :class="selectedSection === 'dashboard' ? 'bg-[#373bff]! text-[#fefefe]!' : 'text-gray-700 hover:bg-blue-100'"
        @click="emit('section-change', 'dashboard')"
      >
        Dashboard de Empresa
      </button>
      <button
        class="self-stretch flex-auto px-3 py-3.5 neumorfismo-opcion-subheader"
        :class="selectedSection === 'benefits' ? 'bg-[#373bff]! text-[#fefefe]!' : 'text-gray-700 hover:bg-blue-100'"
        @click="emit('section-change', 'benefits')"
      >
        Gestionar Beneficios
      </button>
      <button
        class="self-stretch flex-auto px-3 py-3.5 neumorfismo-opcion-subheader"
        :class="selectedSection === 'employees' ? 'bg-[#373bff]! text-[#fefefe]!' : 'text-gray-700 hover:bg-blue-100'"
        @click="emit('section-change', 'employees')"
      >
        Gestionar Empleados
      </button>
      <button
        class="self-stretch flex-auto px-3 py-3.5 neumorfismo-opcion-subheader"
        :class="selectedSection === 'info' ? 'bg-[#373bff]! text-[#fefefe]!' : 'text-gray-700 hover:bg-blue-100'"
        @click="emit('section-change', 'info')"
      >
        Informacion de la Empresa
      </button>
      <button
        class="self-stretch flex-auto px-3 py-3.5 neumorfismo-opcion-subheader-ultimo"
        :class="selectedSection === 'reports' ? 'bg-[#373bff]! text-[#fefefe]!' : 'text-gray-700 hover:bg-blue-100'"
        @click="emit('section-change', 'reports')"
      >
        Reportes de planilla
      </button>
    </div>
  </div>
</template>

<script setup>
// import '../../assets/Neumorfismo.css'
import { defineProps, defineEmits } from 'vue'
defineProps({
  selectedSection: String
})
const emit = defineEmits(['section-change'])
</script>
<template>
  <div class="w-full flex justify-center">
    <div class="flex w-1/2 h-12 items-center justify-between relative neumorphism-card text-xs">
      <button
        class="self-stretch flex-auto px-3 py-3.5 neumorphism-light"
        :class="selectedSection === 'dashboard' ? 'bg-[#1347e5] text-[#fefefe]' : 'text-gray-700 hover:bg-blue-100'"
        @click="emit('section-change', 'dashboard')"
      >
        Información General
      </button>
      <button
        class="self-stretch flex-auto px-3 py-3.5 neumorphism-light"
        :class="selectedSection === 'benefits' ? 'bg-[#1347e5] text-[#fefefe]' : 'text-gray-700 hover:bg-blue-100'"
        @click="emit('section-change', 'benefits')"
      >
        Seleccionar Benefico
      </button>
      <button
        class="self-stretch flex-auto px-3 py-3.5 neumorphism-light"
        :class="selectedSection === 'hours' ? 'bg-[#1347e5] text-[#fefefe]' : 'text-gray-700 hover:bg-blue-100'"
        @click="emit('section-change', 'hours')"
      >
        Registro de horas
      </button>
      <button
        class="self-stretch flex-auto px-3 py-3.5 neumorphism-light"
        :class="selectedSection === 'reports' ? 'bg-[#1347e5] text-[#fefefe]' : 'text-gray-700 hover:bg-blue-100'"
        @click="emit('section-change', 'reports')"
      >
        Mis Reportes
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
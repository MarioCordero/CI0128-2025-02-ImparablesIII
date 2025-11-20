<template>
  <div class="neumorphism-subheader text-xs">
    <button
      class="self-stretch flex-auto px-3 py-3.5 neumorphism-subheader-option nso-first"
      :class="selectedSection === 'dashboard' ? 'bg-[#373bff]! text-[#fefefe]!' : 'text-gray-700 hover:bg-blue-100'"
      @click="emit('section-change', 'dashboard')"
    >
      Información General
    </button>
    <button
      class="self-stretch flex-auto px-3 py-3.5 neumorphism-subheader-option"
      :class="selectedSection === 'benefits' ? 'bg-[#373bff]! text-[#fefefe]!' : 'text-gray-700 hover:bg-blue-100'"
      @click="emit('section-change', 'benefits')"
    >
      Seleccionar Beneficio
    </button>
    <button
      class="self-stretch flex-auto px-3 py-3.5 neumorphism-subheader-option"
      :class="selectedSection === 'hours' ? 'bg-[#373bff]! text-[#fefefe]!' : 'text-gray-700 hover:bg-blue-100'"
      @click="emit('section-change', 'hours')"
    >
      Registro de horas
    </button>
    <button
      class="self-stretch flex-auto px-3 py-3.5 neumorphism-subheader-option nso-last"
      :class="selectedSection === 'reports' ? 'bg-[#373bff]! text-[#fefefe]!' : 'text-gray-700 hover:bg-blue-100'"
      @click="emit('section-change', 'reports')"
    >
      Mis Reportes
    </button>
  </div>
</template>

<script setup>
import { defineProps, defineEmits } from 'vue'
defineProps({
  selectedSection: String
})
const emit = defineEmits(['section-change'])
</script>
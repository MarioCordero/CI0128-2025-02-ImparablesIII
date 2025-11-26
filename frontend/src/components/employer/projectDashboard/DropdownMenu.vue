<template>
  <div class="relative inline-block text-left" ref="dropdownRef">
    
    <button 
      @click="toggleMenu"
      :class="props.buttonClasses"
    >
      <span class="text-2xl leading-none">⋮</span>
    </button>

    <transition>
      <div 
        v-if="isOpen"
        class="absolute top-5 left-11 neumorphism-on-small-item p-0! w-[120px] h-fit z-20"
      >
        <button 
        @click="handleAction('Editar')"
        class="text-sm neumorphism-option-light n-option-first"
        >
        Editar
        </button>
        
        <button 
        @click="handleAction('Eliminar')"
        class="text-sm neumorphism-option-light n-option-last"
        >
        Eliminar
        </button>
      </div>
    </transition>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted, defineProps, defineEmits } from 'vue';

const props = defineProps({
  buttonClasses: {
    type: String,
    default: 'neumorphism-button-normal-light w-[40px] h-[40px] rounded-full! flex items-center justify-center'
  }
});

const emit = defineEmits(['action']);

// Estado del dropdown
const isOpen = ref(false);
// Referencia al elemento del DOM para detectar clics afuera
const dropdownRef = ref(null);

// Función para abrir/cerrar
const toggleMenu = () => {
  isOpen.value = !isOpen.value;
};

// Función para manejar las acciones
const handleAction = (action) => {
  emit('action', action);
  isOpen.value = false; // Cerrar menú al hacer clic
};

// Lógica para cerrar el menú si se hace clic afuera (Click Outside)
const closeOnClickOutside = (event) => {
  if (dropdownRef.value && !dropdownRef.value.contains(event.target)) {
    isOpen.value = false;
  }
};

// Hooks del ciclo de vida para añadir/quitar el listener global
onMounted(() => {
  document.addEventListener('click', closeOnClickOutside);
});

onUnmounted(() => {
  document.removeEventListener('click', closeOnClickOutside);
});
</script>
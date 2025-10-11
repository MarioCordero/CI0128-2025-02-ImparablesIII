<template>
  <header class="w-full bg-[#e9f7ff] shadow-[8px_8px_16px_#d1e3ee,-8px_-8px_16px_#ffffff] px-12 py-0">
    <div class="max-w-[1920px] mx-auto flex items-center justify-between h-[95px] gap-[120px]">
      <!-- Logo & Title -->
      <div class="flex items-center w-full">
        <button
          @click="goToHome"
          class="focus:outline-none flex items-center"
          aria-label="Ir a inicio"
        >
          <img src="../../assets/PlaniFy.svg" alt="PlaniFy Logo" class="h-10 w-full mr-4" />
        </button>
      </div>
      <!-- Actions -->
      <div class="flex items-center gap-6 w-1/2 justify-end">
        <button
          @click="() => scrollToSection('about')"
          class="custom-button font-medium text-gray-700"
        >
          Acerca de nosotros
        </button>
        <button
          @click="() => scrollToSection('contact')"
          class="custom-button font-medium text-gray-700"
        >
          Contacto
        </button>
        <button
          @click="goToLogin"
          class="neumorphism-dark font-medium px-6 py-2"
        >
          Iniciar sesión
        </button>
      </div>
    </div>
  </header>
</template>

<script setup>
import { useRouter } from 'vue-router'
import '../../assets/neumorphismGlobal.css'

const router = useRouter()

function goToHome() {
  router.push('/')
}

function goToLogin() {
  router.push('/login')
}

function scrollToSection(sectionId) {
  const el = document.getElementById(sectionId)
  if (el) {
    el.scrollIntoView({ behavior: 'smooth' })
  }
}
</script>
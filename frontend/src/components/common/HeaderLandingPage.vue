<template>
  <header class="neumorphism-header flex items-center justify-between gap-[120px]">
    <!-- Logo & Title -->
    <div class="flex items-center w-full">
      <button
        @click="goToHome"
        class="focus:outline-none flex items-center"
        aria-label="Ir a inicio"
      >
        <img src="../../assets/PlaniFy.png" alt="PlaniFy Logo" class="h-10 w-full mr-4" />
      </button>
    </div>

    <!-- Actions -->
    <div class="flex items-center gap-6 w-1/2 justify-end">
      <button
        @click="() => scrollToSection('about')"
        class="neumorphism-button-normal-light"
      >
        Acerca de nosotros
      </button>
      <button
        @click="() => scrollToSection('contact')"
        class="neumorphism-button-normal-light"
      >
        Contacto
      </button>
      <button
        @click="goToLogin"
        class="neumorphism-button-normal-dark"
      >
        Iniciar sesión
      </button>
    </div>
  </header>
</template>

<script setup>
import { useRouter } from 'vue-router'

const router = useRouter()

function goToHome() {
  router.push('/')
}

function goToLogin() {
  router.push('/login')
}

function scrollToSection(sectionId) {
  const headerOffset = 95;
  const el = document.getElementById(sectionId);

  if (router.currentRoute.value.path !== '/') {
    router.push('/').then(() => {
      setTimeout(() => {
        if (el) smoothScrollToElement(el, headerOffset);
      }, 300);
    });
  } else {
    if (el) smoothScrollToElement(el, headerOffset);
  }
}

function smoothScrollToElement(element, offset = 0) {
  const elementPosition = element.getBoundingClientRect().top + window.scrollY;
  const offsetPosition = elementPosition - offset - (window.innerHeight / 2 - element.offsetHeight / 2);

  window.scrollTo({
    top: offsetPosition,
    behavior: 'smooth'
  });
}
</script>
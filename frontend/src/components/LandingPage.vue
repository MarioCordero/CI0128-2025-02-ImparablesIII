<template>
  <div class="min-h-screen bg-gray-100 flex flex-col">
    <HeaderLandingPage />
    <main class="flex-1 flex items-center justify-center px-6 py-12">
      <div class="max-w-7xl w-full flex flex-col lg:flex-row items-center justify-between gap-12">
        <div class="flex-1 max-w-xl">
          <div class="flex items-center mb-6">
            <span class="text-6xl mr-4">👥</span>
            <h1 class="text-6xl font-bold text-black">PlaniFy</h1>
          </div>
          <p class="text-gray-700 text-lg mb-8 leading-relaxed">
            Lorem ipsum dolor sit amet, consectetur adipiscing elit. Mauris vehicula gravida 
            massa non vulputate. Phasellus efficitur, tellus ac elementum dignissim, purus 
            diam vestibulum elit, id commodo orci sem nec mi. Donec venenatis sit amet 
            nunc id efficitur. In ut risus in tellus ultricies porttitor quis eu est.
          </p>
          
          <div class="flex gap-4">
            <button
              class="neumorphism-dark font-semibold px-8 py-3"
              @click="goToLogin"
            >
              Iniciar sesión
            </button>

            <button class="neumorphism-light font-semibold px-8 py-3"
              @click="goToRegister">
              Crear cuenta
            </button>
          </div>
        </div>

        <!-- Right: Image -->
        <div class="flex-1 max-w-lg">
          <div class="relative">
            <img
              src="../assets/ccss.png"
              alt="UCR Building"
              class="rounded-3xl shadow-2xl w-full h-80 lg:h-96 object-cover"
            />
          </div>
        </div>
      </div>
    </main>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router'
import HeaderLandingPage from './common/HeaderLandingPage.vue'

const router = useRouter()

function goToLogin() {
  router.push('/login')
}

function goToRegister() {
  router.push('/signup-employer')
}
</script>
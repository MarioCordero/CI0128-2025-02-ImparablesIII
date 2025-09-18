<template>
  <div class="min-h-screen bg-blue-50 flex items-center justify-center">
    <div class="max-w-5xl w-full bg-blue-50 rounded-lg shadow-none flex flex-col md:flex-row items-stretch p-6 md:p-12 gap-8">
      <!-- Left: Logo, Title, Description, Buttons -->
      <div class="flex-1 flex flex-col justify-center">
        <div class="flex items-center mb-2">
          <span class="text-5xl md:text-6xl mr-3">👥</span>
          <span class="text-5xl md:text-6xl font-extrabold text-black">PlaniFy</span>
        </div>
        <p class="text-gray-700 mb-8 max-w-md">
          Lorem ipsum dolor sit amet, consectetur adipiscing elit. Mauris vehicula gravida massa non vulputate. Phasellus efficitur, tellus ac elementum dignissim, purus diam vestibulum elit, id commodo orci sem nec mi. Donec venenatis sit amet nunc id efficitur. In ut risus in tellus ultricies porttitor quis eu est.
        </p>
        <div class="flex gap-4">
          <button
            class="bg-gray-900 text-white font-bold px-8 py-3 rounded-full shadow-md hover:bg-gray-800 transition"
            @click="goToLogin"
          >
            Iniciar sesión
          </button>
          <button class="bg-white text-gray-900 font-bold px-8 py-3 rounded-full border-2 border-gray-900 shadow-md hover:bg-gray-100 transition">
            Crear cuenta
          </button>
        </div>
      </div>
      <!-- Right: Image -->
      <div class="flex-1 flex items-center justify-center">
        <img
          src="../assets/ccss.png"
          alt="UCR Building"
          class="rounded-2xl object-cover w-full h-72 md:h-full"
        />
      </div>
    </div>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router'
const router = useRouter()
function goToLogin() {
  router.push('/login')
}
</script>
<template>
  <div class="min-h-screen flex items-center justify-center bg-[#dbeafe]">
    <div class="bg-[#eaf4fa] rounded-[40px] shadow-2xl p-12 w-full max-w-md flex flex-col items-center">
      <h1 class="text-6xl font-black mb-6 mt-2 text-black tracking-wide">PlaniFy</h1>
      <h2 class="text-lg font-medium mb-6 text-black">INICIAR SESIÓN</h2>
      <form class="w-full flex flex-col gap-5">
        <!-- Email Input -->
        <div class="flex items-center bg-white rounded-full shadow-inner px-4 py-2">
          <svg class="w-6 h-6 text-gray-500 mr-2" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" d="M16 12l-4 4m0 0l-4-4m4 4V4"></path>
            <rect x="3" y="5" width="18" height="14" rx="7" />
            <path d="M3 7l9 6 9-6" />
          </svg>
          <input
            type="email"
            placeholder="Correo electronico"
            class="flex-1 bg-transparent outline-none text-gray-700 placeholder-gray-400"
          />
        </div>
        <!-- Password Input -->
        <div class="flex items-center bg-white rounded-full shadow-inner px-4 py-2">
          <svg class="w-6 h-6 text-gray-500 mr-2" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" d="M12 11c1.104 0 2-.896 2-2s-.896-2-2-2-2 .896-2 2 .896 2 2 2z" />
            <path stroke-linecap="round" stroke-linejoin="round" d="M17 11V7a5 5 0 00-10 0v4" />
            <rect x="5" y="11" width="14" height="7" rx="3.5" />
          </svg>
          <input
            :type="showPassword ? 'text' : 'password'"
            placeholder="Contraseña"
            class="flex-1 bg-transparent outline-none text-gray-700 placeholder-gray-400"
          />
          <button type="button" @click="showPassword = !showPassword" tabindex="-1">
            <svg v-if="!showPassword" class="w-6 h-6 text-gray-400 ml-2" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" d="M13.875 18.825A10.05 10.05 0 0112 19c-5 0-9-4-9-7s4-7 9-7c1.657 0 3.216.418 4.563 1.138M19.07 4.93l-14.14 14.14" />
              <path stroke-linecap="round" stroke-linejoin="round" d="M9.88 9.88a3 3 0 104.24 4.24" />
            </svg>
            <svg v-else class="w-6 h-6 text-gray-400 ml-2" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
              <path stroke-linecap="round" stroke-linejoin="round" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-.274.857-.67 1.664-1.175 2.393M15.54 17.44A9.969 9.969 0 0112 19c-5 0-9-4-9-7" />
            </svg>
          </button>
        </div>
        <!-- Login Button -->
        <button
          type="submit"
          class="mt-2 bg-[#2d384b] text-white text-xl font-medium rounded-full py-3 shadow-lg hover:bg-[#1e293b] transition-all"
        >
          Ingresar
        </button>
      </form>
      <div class="mt-8 text-center text-sm text-gray-700">
        ¿No tienes una cuenta?
        <a href="#" class="text-blue-700 font-semibold hover:underline ml-1">Registrarse</a>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
const showPassword = ref(false)
</script>
<template>
  <div class="min-h-screen flex items-center justify-center bg-[#dbeafe] font-montserrat">
    <div class="neumorphism-card bg-[#eaf4fa] p-20 w-1/3 flex flex-col">
       <!-- Logo -->
      <img :src="logo" alt="PlaniFy Logo" class="w-full mb-4" />
      <h2 class="text-lg text-center font-light! mb-4 text-black">INICIAR SESIÓN</h2>
      <!-- Login Form -->
      <form class="w-full flex flex-col gap-5">
        <!-- Email Input -->
        <div class="flex items-center bg-white rounded-full shadow-inner px-4 py-2">
          <svg class="w-6 h-6 text-gray-500 mr-2" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" d="M3 8l7.89 4.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" />
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
            <path stroke-linecap="round" stroke-linejoin="round" d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" />
          </svg>
          
          <input
            :type="showPassword ? 'text' : 'password'"
            placeholder="Contraseña"
            class="flex-1 bg-transparent outline-none text-gray-700 placeholder-gray-400"
          />
          <button type="button" @click="showPassword = !showPassword" tabindex="-1">
            <svg v-if="!showPassword" class="w-6 h-6 text-gray-400 ml-2" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" d="M13.875 18.825A10.05 10.05 0 0112 19c-4.478 0-8.268-2.943-9.543-7a9.97 9.97 0 011.563-3.029m5.858.908a3 3 0 114.243 4.243M9.878 9.878l4.242 4.242M9.878 9.878L8.464 8.464M9.878 9.878l-.637-.637m5.878 5.878l1.414 1.414M15.12 14.12l-.637-.637m-2.458 4.458l-.42-.42M17 17l-5-5M3 3l3.59 3.59m0 0A9.953 9.953 0 0112 5c4.478 0 8.268 2.943 9.543 7a10.025 10.025 0 01-4.132 5.411m0 0L21 21" />
            </svg>
            <svg v-else class="w-6 h-6 text-gray-400 ml-2" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
              <path stroke-linecap="round" stroke-linejoin="round" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
            </svg>
          </button>
        </div>
        <!-- Login Button -->
        <button
          type="submit"
          class="neumorphism-dark text-xl font-medium py-3 mt-2"
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
import logo from '@/assets/PlaniFyLogo.svg'

const showPassword = ref(false)
</script>
<template>
  <div class="min-h-screen flex items-center justify-center bg-[#dbeafe] font-montserrat">
    <div class="neumorphism-card bg-[#eaf4fa] p-20 w-1/3 flex flex-col">
       <!-- Logo -->
      <img :src="logo" alt="PlaniFy Logo" class="w-full mb-4" />
      <h2 class="text-lg text-center font-light mb-4 text-black">INICIAR SESIÓN</h2>
      
      <!-- Error Message -->
      <div v-if="errorMessage" class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4 text-sm">
        {{ errorMessage }}
      </div>

      <!-- Success Message -->
      <div v-if="successMessage" class="bg-green-100 border border-green-400 text-green-700 px-4 py-3 rounded mb-4 text-sm">
        {{ successMessage }}
      </div>

      <!-- Login Form -->
      <form @submit.prevent="handleLogin" class="w-full flex flex-col gap-5">
        <!-- Email Input -->
        <div class="flex items-center bg-white rounded-full shadow-inner px-4 py-2">
          <svg class="w-6 h-6 text-gray-500 mr-2" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" d="M3 8l7.89 4.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" />
          </svg>
          <input
            v-model="email"
            type="email"
            placeholder="Correo electronico"
            class="flex-1 bg-transparent outline-none text-gray-700 placeholder-gray-400"
            required
          />
        </div>
        <!-- Password Input -->
        <div class="flex items-center bg-white rounded-full shadow-inner px-4 py-2">
          <svg class="w-6 h-6 text-gray-500 mr-2" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" />
          </svg>
          
          <input
            v-model="password"
            :type="showPassword ? 'text' : 'password'"
            placeholder="Contraseña"
            class="flex-1 bg-transparent outline-none text-gray-700 placeholder-gray-400"
            required
          />
          <button type="button" @click="showPassword = !showPassword" tabindex="-1">
            <svg v-if="!showPassword" class="w-6 h-6 text-gray-400 ml-2" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" d="M13.875 18.825A10.05 10.05 0 0112 19c-4.478 0-8.268-2.943-9.543-7a9.97 9.97 0 011.563-3.029m5.858.908a3 3 0 114.243 4.243M9.878 9.878l4.242 4.242M9.878 9.878L8.464 8.464M9.878 9.878l-.637-.637m5.878 5.878l1.414 1.414M15.12 14.12l-.637-.637m-2.458 4.458l-.42-.42M17 17l-5-5M3 3l3.59 3.59m0 0A9.953 9.953 0 0112 5c4.478 0 8.268 2.943 9.542 7a10.025 10.025 0 01-4.132 5.411m0 0L21 21" />
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
          :disabled="isLoading"
          class="neumorphism-dark text-xl font-medium py-3 mt-2 disabled:opacity-50 disabled:cursor-not-allowed"
        >
          {{ isLoading ? 'Ingresando...' : 'Ingresar' }}
        </button>
      </form>
      <div class="mt-8 text-center text-sm text-gray-700">
        ¿No tienes una cuenta?
        <router-link to="/register" class="text-blue-700 font-semibold hover:underline ml-1">Registrarse</router-link>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import logo from '@/assets/PlaniFyLogo.svg'

const router = useRouter()
const showPassword = ref(false)
const email = ref('')
const password = ref('')
const isLoading = ref(false)
const errorMessage = ref('')
const successMessage = ref('')

// Get the backend URL from your Program.cs - it should be https://localhost:7xxx
const API_BASE_URL = 'http://localhost:5011/api'

async function handleLogin() {
  // Clear previous messages
  errorMessage.value = ''
  successMessage.value = ''

  // Validation
  if (!email.value || !password.value) {
    errorMessage.value = 'Por favor ingrese correo y contraseña'
    return
  }

  isLoading.value = true

  try {
    console.log('Attempting login with:', { email: email.value }) // Debug log

    const response = await fetch(`${API_BASE_URL}/login`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        correo: email.value,
        contrasena: password.value
      })
    })

    console.log('Response status:', response.status) // Debug log

    const data = await response.json()
    console.log('Response data:', data) // Debug log

    if (response.ok && data.success) {
      successMessage.value = data.message || 'Login exitoso'
      
      // Store user data and token in localStorage
      localStorage.setItem('user', JSON.stringify(data.userData))
      if (data.token) {
        localStorage.setItem('token', data.token)
      }
      
      console.log('Login successful, user data:', data.userData) // Debug log
      
      // Check if user is Administrador and redirect accordingly
      if (data.userData.tipoUsuario === 'Administrador') {
        successMessage.value = 'Login exitoso como Super Administrador'
        // Redirect to superadmin menu after 1.5 seconds
        setTimeout(() => {
          router.push('/superadmin')
        }, 1500)
      } else {
        // Redirect to home page for other user types
        setTimeout(() => {
          router.push('/')
        }, 1500)
      }
    } else {
      errorMessage.value = data.message || 'Error al iniciar sesión'
    }
  } catch (error) {
    console.error('Login error:', error)
    errorMessage.value = 'Error de conexión. Verifique que el servidor esté ejecutándose.'
  } finally {
    isLoading.value = false
  }
}
</script>
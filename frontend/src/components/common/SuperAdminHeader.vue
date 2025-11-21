<template>
  <header class="neumorphism-header grid grid-cols-3 items-center gap-[120px]">
    <!-- Título -->
    <div class="flex items-center">
      <button
        @click="navigateToHome"
        class="focus:outline-none flex items-center"
        aria-label="Ir a inicio"
      >
        <img src="../../assets/PlaniFy.png" alt="PlaniFy Logo" class="h-10 w-full mr-4" />
      </button>

      <!-- TODO: ESTO QUE?   -->
      <div>
        <p class="text-2xl font-bold mb-0 whitespace-nowrap">Panel de Empleador</p>
        <p class="text-gray-600 text-base mb-0 whitespace-nowrap">Gestión de Beneficios Corporativos</p>
      </div>
    </div>

    <!-- Usuario info centrado -->
    <div class="flex justify-center">
      <div v-if="user" class="neumorphism-input flex items-center justify-center">
        <span class="text-gray-700 font-medium">{{ user.nombre }} {{ user.apellidos }}</span>
      </div>
    </div>

    <!-- Botón cerrar sesión -->
    <div class="flex justify-end items-center gap-4">
      <button
        @click="logout"
        class="neumorphism-button-normal-blue"
      >
        Cerrar Sesión
      </button>
    </div>
  </header>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const user = ref(null)

function logout() {
  localStorage.removeItem('user')
  localStorage.removeItem('token')
  router.push('/login')
}

function checkAuthentication() {
  const userData = localStorage.getItem('user')
  const token = localStorage.getItem('token')
  if (!userData || !token) {
    router.push('/login')
    return false
  }
  try {
    const parsedUser = JSON.parse(userData)
    if (parsedUser.tipoUsuario !== 'Administrador') {
      router.push('/')
      return false
    }
    user.value = parsedUser
    return true
  } catch {
    localStorage.removeItem('user')
    localStorage.removeItem('token')
    router.push('/login')
    return false
  }
}

onMounted(() => {
  checkAuthentication()
})
</script>
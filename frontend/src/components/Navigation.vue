<template>
  <nav class="bg-blue-600 shadow-lg">
    <div class="max-w-7xl mx-auto px-4">
      <div class="flex justify-between items-center py-4">
        <!-- Logo/Brand -->
        <div class="flex items-center">
          <router-link to="/" class="text-white text-xl font-bold hover:text-blue-200">
            PlaniFy
          </router-link>
        </div>

        <!-- Navigation Links -->
        <div class="flex space-x-4">
          <router-link 
            to="/" 
            class="text-white hover:text-blue-200 px-3 py-2 rounded-md text-sm font-medium transition duration-300"
            :class="{ 'bg-blue-700': $route.path === '/' }"
          >
            Inicio
          </router-link>
          
          <router-link 
            to="/employees" 
            class="text-white hover:text-blue-200 px-3 py-2 rounded-md text-sm font-medium transition duration-300"
            :class="{ 'bg-blue-700': $route.path === '/employees' }"
          >
            Lista de Empleados
          </router-link>
          
          <router-link 
            to="/login" 
            class="text-white hover:text-blue-200 px-3 py-2 rounded-md text-sm font-medium transition duration-300"
            :class="{ 'bg-blue-700': $route.path === '/login' }"
          >
            Login
          </router-link>
        </div>
      </div>
    </div>
  </nav>
</template>

<script>
export default {
  name: 'NavigationBar'
}
</script>

<style scoped>
/* Additional navigation styles if needed */
</style>

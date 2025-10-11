<template>
  <header class="bg-white shadow-sm px-6 py-4">
    <div class="max-w-7xl mx-auto flex justify-between items-center">
      <div class="flex items-center">
        <span class="text-2xl mr-2">👥</span>
        <span class="text-2xl font-bold text-black">PlaniFy</span>
      </div>
      <nav class="hidden md:flex items-center gap-8">
        <a href="#" class="text-gray-600 hover:text-gray-900">acerca de nosotros</a>
        <a href="#" class="text-gray-600 hover:text-gray-900">contacto</a>
        <button 
          @click="goToLogin"
          class="text-gray-600 hover:text-gray-900"
        >
          Iniciar sesión
        </button>
      </nav>
    </div>
  </header>
</template>

<script>
export default {
  name: 'HeaderLandingPage',
  methods: {
    goToLogin() {
      this.$router.push('/login')
    }
  }
}
</script>
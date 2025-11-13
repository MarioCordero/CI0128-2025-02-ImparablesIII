<template>
  <div class="page flex flex-col">
    <HeaderLandingPage />


    <div class="body">
      <!-- Hero Section -->
      <main class="min-h-[90vh] flex-1 flex items-center justify-center px-6 py-20">
        <div class="max-w-7xl w-full flex flex-col lg:flex-row items-center justify-between gap-20">
          <!-- Left: Title & CTA -->
          <div class="flex-1 max-w-2xl">
            <div class="flex items-center mb-8">
              <img src="../assets/PlaniFyLogo.png" alt="PlaniFy Logo" class="w-1/2 mr-4" />
            </div>
            <p class="text-gray-700 text-2xl mb-10 leading-relaxed font-medium">
              Simplifica la gestión de planillas y beneficios empresariales.<br>
              PlaniFy te ayuda a ahorrar tiempo y mejorar la experiencia de tu equipo.
            </p>
            <div class="flex gap-6 mt-6">
              <button
                class="neumorphism-button-xl-dark"
                @click="goToLogin"
              >
                Iniciar sesión
              </button>
              <button
                class="neumorphism-button-xl-light"
                @click="goToRegister"
              >
                Crear cuenta
              </button>
            </div>
          </div>
          <!-- Right: Image -->
          <div class="flex-1 max-w-xl flex justify-center">
            <img
              src="../assets/ccss.png"
              alt="UCR Building"
              class="w-full h-[28rem] object-cover neumorphism-on"
            />
          </div>
        </div>
      </main>
  
      <!-- About Us Section -->
      <section id="about" class="min-h-[90vh] max-w-6xl mx-auto w-full py-16 px-8">
        <div class="neumorphism-on p-12 mb-12">
          <h2 class="text-4xl font-bold text-gray-800 mb-6 text-center">Acerca de nosotros</h2>
          <p class="text-gray-700 text-2xl text-center leading-relaxed">
            PlaniFy es una plataforma dedicada a la gestión eficiente de planillas y beneficios empresariales.<br>
            Nuestro objetivo es simplificar procesos administrativos y mejorar la experiencia tanto para empleadores como empleados.<br>
            ¡Únete y lleva tu empresa al siguiente nivel!
          </p>
        </div>
      </section>
  
      <!-- Contact Us Section -->
      <section id="contact" class="min-h-[90vh] max-w-6xl mx-auto w-full pb-20 px-8">
        <div class="neumorphism-on p-12 ">
          <h2 class="text-4xl font-bold text-gray-800 mb-6 text-center">Contáctanos</h2>
          <p class="text-gray-700 text-2xl mb-8 text-center">
            ¿Tienes preguntas o necesitas soporte?<br>
            ¡Estamos aquí para ayudarte!
          </p>
          <div class="flex flex-col md:flex-row gap-10 justify-center items-center text-xl">
            <div>
              <span class="font-semibold text-gray-800">Correo:</span>
              <a href="mailto:info@planify.com" class="text-blue-600 hover:underline ml-2">info@planify.com</a>
            </div>
            <div>
              <span class="font-semibold text-gray-800">Teléfono:</span>
              <span class="ml-2 text-gray-700">+506 2222-3333</span>
            </div>
          </div>
        </div>
      </section>
    </div>
  </div>
</template>

<script>
import HeaderLandingPage from './common/HeaderLandingPage.vue'

export default {
  name: 'LandingPage',

  components: {
    HeaderLandingPage
  },

  data() {
    return {}
  },

  methods: {
    goToLogin() {
      this.$router.push('/login')
    },

    goToRegister() {
      this.$router.push('/signup-employer')
    }
  }
}
</script>
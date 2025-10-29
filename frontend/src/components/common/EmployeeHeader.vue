<template>
  <header class="neumorfismo-tarjeta grid grid-cols-3 items-center gap-[120px] px-20 min-h-[95px] max-h-[95px]">
    <!-- Logo & Title -->
    <div class="flex items-center">
      <button
        @click="navigateToHomeLogged"
        class="focus:outline-none flex items-center"
        aria-label="Ir a inicio"
      >
        <img src="../../assets/PlaniFy.png" alt="PlaniFy Logo" class="h-10 w-full mr-4" />
      </button>
      <div>
        <p class="text-2xl font-bold mb-0 whitespace-nowrap">Panel de Empleado</p>
        <p class="text-gray-600 text-base mb-0 whitespace-nowrap">Gestión de Beneficios Corporativos</p>
      </div>
    </div>

    <div></div>

    <!-- Actions -->
    <div class="flex justify-end items-center gap-4">

      <!-- Botón de Perfil -->
      <button
        @click="goToProfile"
        class="neumorfismo-boton px-6 py-2 font-semibold"
      >
        Mi Perfil
      </button>

      <button
        @click="logout"
        class="neumorfismo-boton-azul px-6 py-2 rounded-xl text-base font-semibold text-white!"
      >
        Cerrar Sesión
      </button>
    </div>
  </header>
</template>

<script>
import "../../assets/Neumorfismo.css";

export default {
  name: 'EmployeeHeader',
  props: {
    user: {
      type: Object,
      default: null
    }
  },

  data() {
    return {
      companies: [],
      selectedProjectId: '',
    }
  },
  async mounted() {
  },
  methods: {
    navigateToHomeLogged() {
      localStorage.removeItem('selectedProject')
      this.selectedProjectId = ''
      this.$router.push('/dashboard-employee')
    },
    logout() {
      localStorage.removeItem('user')
      localStorage.removeItem('token')
      this.$router.push('/login')
    },
    goToProfile() {
      if (this.user && this.user.idPersona) {
        this.$router.push({
          name: 'ProfileEmployee',
          params: { id: this.user.idPersona }
        });
      } else {
        console.error('No se pudo obtener el ID del usuario');
        this.$router.push('/profile-employee');
      }
    },
  }
}
</script>
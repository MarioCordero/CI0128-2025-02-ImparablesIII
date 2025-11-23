<template>
  <div class="page">
    <div class="min-h-screen flex items-center justify-center p-5">
      <div class="neumorphism-card-modal">
        <div class="text-center mb-8">
          <img src="@/assets/PlaniFy.png" alt="PlaniFy Logo" class="max-w-xs h-auto mx-auto mb-4" />
          <p class="text-gray-500 text-lg">Verifica tu Email</p>
        </div>

        <!-- Cargando -->
        <div v-if="loading" class="text-center">
          <p class="text-gray-900 text-base font-semibold">Verificando tu email...</p>
        </div>

        <!-- Éxito -->
        <div v-else-if="verified" class="text-center">
          <div class="text-6xl text-green-500 font-bold mb-4">✓</div>
          <p class="text-black text-2xl font-bold mb-2">¡Email Verificado!</p>
          <p class="text-gray-500 text-sm mb-6">Tu cuenta ha sido activada correctamente.</p>
          <button class="neumorphism-button-xl-dark w-full" @click="goToPasswordSetup">
            Continuar con Setup de Contraseña
          </button>
        </div>

        <!-- Error -->
        <div v-else-if="error" class="text-center">
          <div class="text-6xl text-red-500 font-bold mb-4">✕</div>
          <p class="text-black text-2xl font-bold mb-2">Error en Verificación</p>
          <p class="text-gray-500 text-sm mb-6">{{ error }}</p>
          <button class="neumorphism-button-xl-dark w-full" @click="$router.push('/')">
            Volver al Inicio
          </button>
        </div>

        <!-- Pedir Email (si no lo tiene del registro) -->
        <div v-else class="text-center">
          <p class="text-gray-900 text-base font-semibold mb-5">Ingresa tu email para verificar tu cuenta:</p>
          <input 
            v-model="email" 
            type="email" 
            placeholder="tu@email.com"
            class="neumorphism-input w-full mb-4"
          />
          <button class="neumorphism-button-xl-dark w-full" @click="verifyEmail">Verificar</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { apiConfig } from '../config/api';

export default {
  name: 'VerifyEmail',
  data() {
    return {
      email: '',
      token: '',
      loading: false,
      verified: false,
      error: '',
      personaId: null
    };
  },
  mounted() {
    this.token = this.$route.query.token;
    const storedEmail = sessionStorage.getItem('registeredEmail');
    if (storedEmail) {
      this.email = storedEmail;
      this.verifyEmail();
    }
  },
  methods: {
    async verifyEmail() {
      if (!this.email || !this.token) {
        this.error = 'Email y token son requeridos';
        return;
      }

      this.loading = true;
      this.error = '';

      try {
        const response = await fetch(apiConfig.endpoints.verifyEmployerCode, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({
            email: this.email,
            code: this.token
          })
        });

        const data = await response.json();

        if (response.ok && data.success) {
          this.verified = true;
          this.personaId = data.personaId;
          sessionStorage.setItem('personaId', this.personaId);
        } else {
          this.error = data.message || 'Error al verificar el email';
        }
      } catch (err) {
        this.error = 'Error de conexión con el servidor';
        console.error(err);
      } finally {
        this.loading = false;
      }
    },
    goToPasswordSetup() {
      this.$router.push({
        name: 'PasswordSetup',
        params: { personaId: this.personaId }
      });
    }
  }
};
</script>
<template>
  <div class="signup-employer">
    <h2>Registro de Empleador</h2>
    <form @submit.prevent="submitForm">
      <div>
        <label>Nombre*</label>
        <input v-model="form.nombre" :class="{ invalid: errors.nombre }" required />
        <span v-if="errors.nombre">{{ errors.nombre }}</span>
      </div>
      <div>
        <label>Primer Apellido*</label>
        <input v-model="form.primerApellido" :class="{ invalid: errors.primerApellido }" required />
        <span v-if="errors.primerApellido">{{ errors.primerApellido }}</span>
      </div>
      <div>
        <label>Segundo Apellido (opcional)</label>
        <input v-model="form.segundoApellido" :class="{ invalid: errors.segundoApellido }" />
        <span v-if="errors.segundoApellido">{{ errors.segundoApellido }}</span>
      </div>
      <div>
        <label>Cédula*</label>
        <input v-model="form.cedula" :class="{ invalid: errors.cedula }" required />
        <span v-if="errors.cedula">{{ errors.cedula }}</span>
      </div>
      <div>
        <label>Correo electrónico*</label>
        <input v-model="form.email" :class="{ invalid: errors.email }" required />
        <span v-if="errors.email">{{ errors.email }}</span>
      </div>
      <div>
        <label>Número de teléfono*</label>
        <input v-model="form.telefono" :class="{ invalid: errors.telefono }" required />
        <span v-if="errors.telefono">{{ errors.telefono }}</span>
      </div>
      <div>
        <label>Nombre de usuario*</label>
        <input v-model="form.username" :class="{ invalid: errors.username }" required />
        <span v-if="errors.username">{{ errors.username }}</span>
      </div>
      <div>
        <label>Fecha de nacimiento*</label>
        <input type="date" v-model="form.fechaNacimiento" :class="{ invalid: errors.fechaNacimiento }" required />
        <span v-if="errors.fechaNacimiento">{{ errors.fechaNacimiento }}</span>
      </div>
      <div>
        <label>Contraseña*</label>
        <input :type="showPassword ? 'text' : 'password'" v-model="form.password" :class="{ invalid: errors.password }" required />
        <button type="button" @click="showPassword = !showPassword">{{ showPassword ? 'Ocultar' : 'Ver' }}</button>
        <span v-if="errors.password">{{ errors.password }}</span>
        <ul>
          <li>Mínimo 8 y máximo 32 caracteres</li>
          <li>Al menos una mayúscula, una minúscula, un número y un carácter especial (#@$_&()*:;!?)</li>
        </ul>
      </div>
      <div>
        <label>Confirmar Contraseña*</label>
        <input :type="showPassword ? 'text' : 'password'" v-model="form.confirmPassword" :class="{ invalid: errors.confirmPassword }" required />
        <span v-if="errors.confirmPassword">{{ errors.confirmPassword }}</span>
      </div>
      <button type="submit">Registrarse</button>
    </form>
    <div v-if="showVerification">
      <h3>Verifica tu correo electrónico</h3>
      <p>Ingresa el código de 6 dígitos enviado a tu correo:</p>
      <input v-model="verificationCode" maxlength="6" />
      <button @click="verifyCode">Verificar</button>
      <span v-if="verificationError">{{ verificationError }}</span>
    </div>
  </div>
</template>

<script>
export default {
  data() {
    return {
      form: {
        nombre: '',
        primerApellido: '',
        segundoApellido: '',
        cedula: '',
        email: '',
        telefono: '',
        username: '',
        fechaNacimiento: '',
        password: '',
        confirmPassword: '',
      },
      errors: {},
      showPassword: false,
      showVerification: false,
      verificationCode: '',
      verificationError: '',
    };
  },
  methods: {
    validateForm() {
      const nameRegex = /^[A-Za-zÁÉÍÓÚáéíóúÑñ' -]{2,50}$/;
      const cedulaRegex = /^[1-7][0-9]{8}$/;
      const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
      const phoneRegex = /^\+[1-9]\d{1,14}$/;
      const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#@$_&()*:;!?])[A-Za-z\d#@$_&()*:;!?]{8,32}$/;
      this.errors = {};

      if (!nameRegex.test(this.form.nombre)) this.errors.nombre = 'Nombre inválido.';
      if (!nameRegex.test(this.form.primerApellido)) this.errors.primerApellido = 'Primer apellido inválido.';
      if (this.form.segundoApellido && !nameRegex.test(this.form.segundoApellido)) this.errors.segundoApellido = 'Segundo apellido inválido.';
      if (!cedulaRegex.test(this.form.cedula)) this.errors.cedula = 'Cédula inválida.';
      if (!emailRegex.test(this.form.email)) this.errors.email = 'Correo inválido.';
      if (!phoneRegex.test(this.form.telefono)) this.errors.telefono = 'Teléfono inválido.';
      if (!this.form.username) this.errors.username = 'Nombre de usuario requerido.';
      if (!this.form.fechaNacimiento || !this.isValidAge(this.form.fechaNacimiento)) this.errors.fechaNacimiento = 'Debes ser mayor de 18 y menor de 99 años.';
      if (!passwordRegex.test(this.form.password)) this.errors.password = 'Contraseña inválida.';
      if (this.form.password !== this.form.confirmPassword) this.errors.confirmPassword = 'Las contraseñas no coinciden.';

      return Object.keys(this.errors).length === 0;
    },
    isValidAge(dateStr) {
      const birthDate = new Date(dateStr);
      const today = new Date();
      const age = today.getFullYear() - birthDate.getFullYear();
      const m = today.getMonth() - birthDate.getMonth();
      if (m < 0 || (m === 0 && today.getDate() < birthDate.getDate())) {
        return age - 1 >= 18 && age - 1 < 99;
      }
      return age >= 18 && age < 99;
    },
    submitForm() {
      if (this.validateForm()) {
        // TODO: Send registration data to backend
        // On success, show verification code input
        this.showVerification = true;
      }
    },
    verifyCode() {
      if (/^\d{6}$/.test(this.verificationCode)) {
        // TODO: Send code to backend for verification
        // On success, redirect to login
        this.$router.push('/login');
      } else {
        this.verificationError = 'El código debe ser de 6 dígitos.';
      }
    },
  },
};
</script>

<style scoped>
.signup-employer {
  max-width: 500px;
  margin: auto;
}
.invalid {
  border-color: red;
}
span {
  color: red;
  font-size: 0.9em;
}
</style>
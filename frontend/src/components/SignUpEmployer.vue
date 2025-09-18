<template>
  <div class="min-h-screen flex items-center justify-center bg-[#dbeafe]">
    <div class="bg-[#eaf4fa] rounded-[40px] shadow-2xl p-10 w-full max-w-2xl flex flex-col items-center">
      <h1 class="text-5xl font-black mb-6 mt-2 text-black tracking-wide">Registro de Empleador</h1>

      <form @submit.prevent="submitForm" class="w-full flex flex-col gap-6">
        <!-- Nombre -->
        <div>
          <label class="block mb-1 font-medium text-gray-700">Nombre*</label>
          <input
            v-model="form.nombre"
            required
            class="w-full bg-white rounded-full shadow-inner px-4 py-2 outline-none text-gray-700 placeholder-gray-400"
            placeholder="Ingresa tu nombre"
          />
          <span v-if="errors.nombre" class="text-red-500 text-sm mt-1">{{ errors.nombre }}</span>
        </div>

        <!-- Primer Apellido -->
        <div>
          <label class="block mb-1 font-medium text-gray-700">Primer Apellido*</label>
          <input
            v-model="form.primerApellido"
            required
            class="w-full bg-white rounded-full shadow-inner px-4 py-2 outline-none text-gray-700 placeholder-gray-400"
            placeholder="Ingresa tu primer apellido"
          />
          <span v-if="errors.primerApellido" class="text-red-500 text-sm mt-1">{{ errors.primerApellido }}</span>
        </div>

        <!-- Segundo Apellido -->
        <div>
          <label class="block mb-1 font-medium text-gray-700">Segundo Apellido (opcional)</label>
          <input
            v-model="form.segundoApellido"
            class="w-full bg-white rounded-full shadow-inner px-4 py-2 outline-none text-gray-700 placeholder-gray-400"
            placeholder="Ingresa tu segundo apellido"
          />
          <span v-if="errors.segundoApellido" class="text-red-500 text-sm mt-1">{{ errors.segundoApellido }}</span>
        </div>

        <!-- Cédula -->
        <div>
          <label class="block mb-1 font-medium text-gray-700">Cédula*</label>
          <input
            v-model="form.cedula"
            required
            class="w-full bg-white rounded-full shadow-inner px-4 py-2 outline-none text-gray-700 placeholder-gray-400"
            placeholder="Ej: 101230456"
          />
          <span v-if="errors.cedula" class="text-red-500 text-sm mt-1">{{ errors.cedula }}</span>
        </div>

        <!-- Correo -->
        <div>
          <label class="block mb-1 font-medium text-gray-700">Correo electrónico*</label>
          <input
            v-model="form.email"
            type="email"
            required
            class="w-full bg-white rounded-full shadow-inner px-4 py-2 outline-none text-gray-700 placeholder-gray-400"
            placeholder="ejemplo@email.com"
          />
          <span v-if="errors.email" class="text-red-500 text-sm mt-1">{{ errors.email }}</span>
        </div>

        <!-- Teléfono -->
        <div>
          <label class="block mb-1 font-medium text-gray-700">Número de teléfono*</label>
          <input
            v-model="form.telefono"
            required
            class="w-full bg-white rounded-full shadow-inner px-4 py-2 outline-none text-gray-700 placeholder-gray-400"
            placeholder="+506XXXXXXXX"
          />
          <span v-if="errors.telefono" class="text-red-500 text-sm mt-1">{{ errors.telefono }}</span>
        </div>

        <!-- Username -->
        <div>
          <label class="block mb-1 font-medium text-gray-700">Nombre de usuario*</label>
          <input
            v-model="form.username"
            required
            class="w-full bg-white rounded-full shadow-inner px-4 py-2 outline-none text-gray-700 placeholder-gray-400"
            placeholder="Elige un usuario"
          />
          <span v-if="errors.username" class="text-red-500 text-sm mt-1">{{ errors.username }}</span>
        </div>

        <!-- Fecha de nacimiento -->
        <div>
          <label class="block mb-1 font-medium text-gray-700">Fecha de nacimiento*</label>
          <input
            type="date"
            v-model="form.fechaNacimiento"
            required
            class="w-full bg-white rounded-full shadow-inner px-4 py-2 outline-none text-gray-700"
          />
          <span v-if="errors.fechaNacimiento" class="text-red-500 text-sm mt-1">{{ errors.fechaNacimiento }}</span>
        </div>

        <!-- Password -->
        <div>
          <label class="block mb-1 font-medium text-gray-700">Contraseña*</label>
          <div class="flex items-center bg-white rounded-full shadow-inner px-4 py-2">
            <input
              :type="showPassword ? 'text' : 'password'"
              v-model="form.password"
              required
              class="flex-1 bg-transparent outline-none text-gray-700 placeholder-gray-400"
              placeholder="********"
            />
            <button
              type="button"
              @click="showPassword = !showPassword"
              class="text-gray-500 text-sm"
            >
              {{ showPassword ? 'Ocultar' : 'Ver' }}
            </button>
          </div>
          <span v-if="errors.password" class="text-red-500 text-sm mt-1">{{ errors.password }}</span>
          <ul class="text-xs text-gray-600 mt-1 ml-2 list-disc list-inside">
            <li>Mínimo 8 y máximo 32 caracteres</li>
            <li>Al menos una mayúscula, una minúscula, un número y un carácter especial</li>
          </ul>
        </div>

        <!-- Confirm Password -->
        <div>
          <label class="block mb-1 font-medium text-gray-700">Confirmar Contraseña*</label>
          <input
            :type="showPassword ? 'text' : 'password'"
            v-model="form.confirmPassword"
            required
            class="w-full bg-white rounded-full shadow-inner px-4 py-2 outline-none text-gray-700 placeholder-gray-400"
            placeholder="Repite tu contraseña"
          />
          <span v-if="errors.confirmPassword" class="text-red-500 text-sm mt-1">{{ errors.confirmPassword }}</span>
        </div>

        <!-- Botón registro -->
        <button
          type="submit"
          class="mt-4 bg-[#2d384b] text-white text-xl font-medium rounded-full py-3 shadow-lg hover:bg-[#1e293b] transition-all"
        >
          Registrarse
        </button>
      </form>

      <!-- Verificación de correo -->
      <div v-if="showVerification" class="mt-8 text-center">
        <h3 class="text-xl font-semibold mb-2">Verifica tu correo electrónico</h3>
        <p class="text-gray-700 mb-4">Ingresa el código de 6 dígitos enviado a tu correo:</p>
        <input
          v-model="verificationCode"
          maxlength="6"
          class="bg-white rounded-full shadow-inner px-4 py-2 outline-none text-gray-700 text-center tracking-widest"
        />
        <button
          @click="verifyCode"
          class="mt-4 bg-[#2d384b] text-white rounded-full px-6 py-2 shadow-lg hover:bg-[#1e293b] transition-all"
        >
          Verificar
        </button>
        <span v-if="verificationError" class="block text-red-500 text-sm mt-2">{{ verificationError }}</span>
      </div>
    </div>
  </div>
</template>


<script>
import axios from 'axios';

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
    async submitForm() {
      if (this.validateForm()) {
        try {
          const response = await axios.post(
              'https://localhost:7030/api/SignUpEmployer',
            this.form,
            {
              headers: {
                'Content-Type': 'application/json'
              }
            }
          );
          console.log('Registro exitoso:', response.data);
          this.showVerification = true;
        } catch (error) {
          console.error(error);
          alert('Error en el registro. Por favor, intenta de nuevo.');
        }
      }
    },
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
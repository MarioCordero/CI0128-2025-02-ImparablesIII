<template>
    <div class="bg-[#dbeafe] min-h-screen">

        <SuperAdminHeader/>

        <div class="mx-[171px] my-[41px] space-y-[41px] pb-[41px]">
            
            <!-- Primera parte 1x1 -->
            <div class="neumorfismo-tarjeta flex items-center justify-between px-[39px] py-[37px]">
            <div class="flex items-center gap-4">
                <div class="neumorfismo-sobre w-[160px] h-[160px] rounded-full flex items-center justify-center">
                    <!-- hay que meter logo o foto de perfil -->
                </div>
                <div>
                <p class="font-medium text-[44px]">Cristiano Ronaldo Dos Santos Aveiro</p>
                <p class="text-[29px]">Desarrollador Senior</p>
                <p class="text-[20px]">Desarrollo de Software</p>
                </div>
            </div>
            <div class="flex items-center gap-[22px]">
                <button @click="navigateToProfileEmployee" class="neumorfismo-boton flex items-center gap-2 text-gray-800 px-4 py-2 rounded-lg">
                    Guardar Cambios
                </button>
                <button @click="navigateToProfileEmployee" class="neumorfismo-boton flex items-center gap-2 text-gray-800 px-4 py-2 rounded-lg">
                    Descartar Cambios
                </button>
            </div>
            </div> <!-- Fin Segunda Parte 1x2 -->


            <!-- Segunda Parte 1x2 -->
            <div class="grid grid-cols-2 gap-[41px]">

                <!-- Información Personal -->
                <div class="neumorfismo-tarjeta px-[39px] py-[37px]">
                    <p class="text-[32px] font-medium mb-4">Información Personal</p>
                    <div class="space-y-3">
                    <div>
                        <label class="text-[23px] text-gray-600">Nombre Completo</label>
                        <input type="text" class="neumorfismo-input" value="Cristiano Ronaldo Dos Santos Aveiro" readonly />
                    </div>
                    <div>
                        <label class="text-[23px] text-gray-600">Correo Electrónico</label>
                        <input type="text" class="neumorfismo-input" value="goatCR7@bicho.siuuu" readonly />
                    </div>
                    <div>
                        <label class="text-[23px] text-gray-600">Número de Teléfono</label>
                        <input type="text" class="neumorfismo-input" value="7777-7777" readonly />
                    </div>
                    <div>
                        <label class="text-[23px] text-gray-600">Dirección</label>
                        <input type="text" class="neumorfismo-input" value="Santiago Bernabeu, Madrid, España." readonly />
                    </div>
                    <div>
                        <label class="text-[23px] text-gray-600">Cuenta IBAN</label>
                        <input type="text" class="neumorfismo-input" value="CR77 7777 7777 7777 7777 7777" readonly />
                    </div>
                    </div>
                </div>

                <!-- Información Laboral -->
                <div class="neumorfismo-tarjeta px-[39px] py-[37px]">
                    <p class="text-[32px] font-medium mb-4">Información Laboral</p>
                    <div class="space-y-3">
                    <div>
                        <label class="text-[23px] text-gray-600">Empresa</label>
                        <input type="text" class="neumorfismo-input" value="Patitos" readonly />
                    </div>
                    <div>
                        <label class="text-[23px] text-gray-600">Departamento</label>
                        <input type="text" class="neumorfismo-input" value="Desarrollo de Software" readonly />
                    </div>
                    <div>
                        <label class="text-[23px] text-gray-600">Puesto</label>
                        <input type="text" class="neumorfismo-input" value="Desarrollador Senior" readonly />
                    </div>
                    <div>
                        <label class="text-[23px] text-gray-600">Tipo de Contrato</label>
                        <input type="text" class="neumorfismo-input" value="Tiempo Completo" readonly />
                    </div>
                    <div>
                        <label class="text-[23px] text-gray-600">Fecha de Contratación</label>
                        <input type="text" class="neumorfismo-input" value="06/07/2009" readonly />
                    </div>
                    </div>
                </div>

            </div> <!-- Fin Segunda Parte 1x2 -->

            <!-- Tercera Parte 1x1 -->
            <!-- Información de Compensación -->
            <div class="neumorfismo-tarjeta px-[34px] py-[41px]">
                <p class="text-[32px] font-medium mb-4">Información de Compensación</p>
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                    <label class="text-[23px] text-gray-600">Tipo de Salario</label>
                    <input type="text" class="neumorfismo-input" value="Mensual" readonly />
                </div>
                <div>
                    <label class="text-[23px] text-gray-600">Salario</label>
                    <input type="text" class="neumorfismo-input" value="€7.000.000" readonly />
                </div>
                </div>
            </div> <!-- Fin Tercera Parte 1x1 -->

        </div> <!-- Fin de Container -->

    </div> <!-- Fin de Componente -->
</template>

<script>
import SuperAdminHeader from './common/SuperAdminHeader.vue'
import "../assets/Neumorfismo.css";


export default {
  // 1. Nombre del componente
  name: 'EditProfileEmployee',

  // 2. Componentes hijos locales
  components: { SuperAdminHeader },

  // 3. Directivas locales
  directives: {},

  // 4. Props recibidas del padre
  props: {},

  // 5. Estado reactivo del componente
  data() { },

  // 6. Propiedades derivadas
  computed: {},

  // 7. Observadores de cambios
  watch: {},

  // 8. Métodos y lógica ejecutable
  methods: {
    navigateToProfileEmployee() {
      this.$router.push('/profile-employee')
    },
  },

  // 9. Ciclo de vida
  beforeCreate() {},
  created() {},
  beforeMount() {},
  mounted() {},
  beforeUpdate() {},
  updated() {},
  beforeUnmount() {},
  unmounted() {},

  // 10. Opciones de inyección
  provide() {
    return {}
  },
  inject: [],

  // 11. Eventos emitidos
  emits: [],

  // 12. Reutilización de lógica
  mixins: [],
  extends: null,

  // 13. Filtros
  filters: {}
}
</script>

<style scoped>
.input {
  @apply w-full bg-gray-50 border border-gray-300 rounded-lg px-3 py-2 text-gray-800 cursor-not-allowed;
}
</style>
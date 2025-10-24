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
                <p class="font-medium text-[44px]">{{ user?.nombre || 'Empleado' }}</p>
                <p class="text-[29px]">{{ user?.puesto || 'Puesto' }}</p>
                <p class="text-[20px]">{{ user?.departamento || 'Departamento' }}</p>
                </div>
            </div>
            <button @click="navigateToEditInfoEmployee" class="neumorfismo-boton flex items-center gap-2 text-gray-800 px-4 py-2 rounded-lg">
                Editar Perfil
            </button>
            </div> <!-- Fin Segunda Parte 1x2 -->


            <!-- Segunda Parte 1x2 -->
            <div class="grid grid-cols-2 gap-[41px]">

                <!-- Información Personal -->
                <div class="neumorfismo-tarjeta px-[39px] py-[37px]">
                    <p class="text-[32px] font-medium mb-4">Información Personal</p>
                    <div class="space-y-3">
                    <div>
                        <label class="text-[23px] text-gray-600">Nombre Completo</label>
                        <p class="neumorfismo-sobre-suave text-[20px]"> {{ user?.nombre || 'Nombre Completo del Empleado' }} </p>
                    </div>
                    <div>
                        <label class="text-[23px] text-gray-600">Correo Electrónico</label>
                        <p class="neumorfismo-sobre-suave text-[20px]"> {{ user?.correo || 'Correo Electronico del Empleado' }} </p>
                    </div>
                    <div>
                        <label class="text-[23px] text-gray-600">Número de Teléfono</label>
                        <p class="neumorfismo-sobre-suave text-[20px]"> {{ user?.telefono || 'Numero Telefonico del Empleado' }} </p>
                    </div>
                    <div>
                        <label class="text-[23px] text-gray-600">Dirección</label>
                        <p class="neumorfismo-sobre-suave text-[20px]"> {{ user?.direccion || 'Direccion del Empleado' }} </p>
                    </div>
                    <div>
                        <label class="text-[23px] text-gray-600">Cuenta IBAN</label>
                        <p class="neumorfismo-sobre-suave text-[20px]"> {{ user?.iban || 'Cuenta Iban del Empleado' }} </p>
                    </div>
                    </div>
                </div>

                <!-- Información Laboral -->
                <div class="neumorfismo-tarjeta px-[39px] py-[37px]">
                    <p class="text-[32px] font-medium mb-4">Información Laboral</p>
                    <div class="space-y-3">
                    <div>
                        <label class="text-[23px] text-gray-600">Empresa</label>
                        <p class="neumorfismo-sobre-suave text-[20px]"> {{ project?.nombre || 'Nombre de la Empresa' }} </p>
                    </div>
                    <div>
                        <label class="text-[23px] text-gray-600">Departamento</label>
                        <p class="neumorfismo-sobre-suave text-[20px]"> {{ user?.departamento || 'Departamento del Empleado' }} </p>
                    </div>
                    <div>
                        <label class="text-[23px] text-gray-600">Puesto</label>
                        <p class="neumorfismo-sobre-suave text-[20px]"> {{ user?.puesto || 'Puesto del Empleado' }} </p>
                    </div>
                    <div>
                        <label class="text-[23px] text-gray-600">Tipo de Contrato</label>
                        <p class="neumorfismo-sobre-suave text-[20px]"> {{ user?.tipoContrato || 'Tipo de contrato del Empleado' }} </p>
                    </div>
                    <div>
                        <label class="text-[23px] text-gray-600">Fecha de Contratación</label>
                        <p class="neumorfismo-sobre-suave text-[20px]"> {{ user?.fechaContratacion || 'Fecha de Contratacion del Empleado' }} </p>
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
                    <p class="neumorfismo-sobre-suave text-[20px]"> {{ proyect?.periodoPago || 'Periodo de Pago de la Empresa' }} </p>
                </div>
                <div>
                    <label class="text-[23px] text-gray-600">Salario</label>
                    <p class="neumorfismo-sobre-suave text-[20px]"> {{ user?.salario || 'Salario del Empleado' }} </p>
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
  name: 'ProfileEmployee',

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
    navigateToEditInfoEmployee() {
        this.$router.push('/edit-info-employee')
    },

      async loadUserData() {
        try {
        // Opción 1: Si tienes el ID en los parámetros de ruta
        const employeeId = this.$route.params.id;
        
        // Opción 2: Si es el perfil del usuario logueado
        // const employeeId = null; // Para usar el endpoint "me"
        
        let url = '/api/EmployeeProfile/me';
        if (employeeId) {
            url = `/api/EmployeeProfile/${employeeId}`;
        }

        const response = await this.$http.get(url);
        this.user = response.data.user;
        this.project = response.data.project;
        
        } catch (error) {
        console.error('Error loading profile:', error);
        // Manejar error - mostrar datos por defecto
        this.user = {
            nombre: 'Empleado no encontrado',
            puesto: 'Puesto no disponible',
            // ... otros campos por defecto
        };
        }
    }
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
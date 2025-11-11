<template>
    <div class="bg-[#dbeafe] min-h-screen">
        <EmployeeHeader v-if="!isEmployer" :user="user" />
        <MainEmployerHeader v-else :user="user" />
        
        <div class="mx-[171px] my-[41px] space-y-[41px] pb-[41px]">
            
            <!-- Primera parte 1x1 -->
            <div class="neumorfismo-tarjeta flex items-center justify-between px-[39px] py-[37px]">
                <div class="flex items-center gap-4">
                    <div class="neumorfismo-sobre w-[160px] h-[160px] rounded-full flex items-center justify-center">
                        <!-- hay que meter logo o foto de perfil -->
                    </div>
                    <div>
                        <p class="font-medium text-[44px]">{{ fullName }}</p>
                        <p class="text-[29px]">{{ userData.user.puesto }}</p>
                        <p class="text-[20px]">{{ userData.user.departamento }}</p>
                    </div>
                </div>
                <div class="flex gap-2">
                    <button 
                        v-if="!isEditing" 
                        @click="enableEditing" 
                        class="neumorfismo-boton flex items-center gap-2 text-gray-800 px-4 py-2"
                    >
                        Editar Perfil
                    </button>
                    <button 
                        v-if="isEditing" 
                        @click="saveChanges" 
                        :disabled="saving"
                        class="neumorfismo-boton-verde flex items-center gap-2 text-gray-800 px-4 py-2"
                    >
                        <span v-if="saving">Guardando...</span>
                        <span v-else>Guardar Cambios</span>
                    </button>
                    <button 
                        v-if="isEditing" 
                        @click="cancelEditing" 
                        :disabled="saving"
                        class="neumorfismo-boton-rojo flex items-center gap-2 text-gray-800 px-4 py-2"
                    >
                        Cancelar
                    </button>
                </div>
            </div>

            <!-- Mensajes de éxito/error -->
            <div v-if="successMessage" class="bg-green-100 border border-green-400 text-green-700 px-4 py-3 rounded">
                {{ successMessage }}
            </div>
            <div v-if="error" class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded">
                {{ error }}
            </div>

            <!-- Segunda Parte 1x2 -->
            <div class="grid grid-cols-2 gap-[41px]">

                <!-- Información Personal -->
                <div class="neumorfismo-tarjeta px-[39px] py-[37px]">
                    <p class="text-[32px] font-medium mb-4">Información Personal</p>
                    <div class="space-y-[25px]">
                        <!-- Nombre Completo -->
                        <div>
                            <label class="text-[23px] text-gray-600">Nombre Completo</label>
                            <div v-if="!isEditing" class="neumorfismo-sobre-suave text-[20px] p-2">
                                {{ editedUserData.nombre }} {{ editedUserData.segundoNombre }} {{ editedUserData.apellidos }}
                            </div>
                            <div v-else class="grid grid-cols-3 gap-2">
                                <input 
                                    v-model="editedUserData.nombre"
                                    class="neumorfismo-input text-[16px] p-2 rounded"
                                    placeholder="Primer nombre"
                                    :disabled="saving"
                                />
                                <input 
                                    v-model="editedUserData.segundoNombre"
                                    class="neumorfismo-input text-[16px] p-2 rounded"
                                    placeholder="Segundo nombre"
                                    :disabled="saving"
                                />
                                <input 
                                    v-model="editedUserData.apellidos"
                                    class="neumorfismo-input text-[16px] p-2 rounded"
                                    placeholder="Apellidos"
                                    :disabled="saving"
                                />
                            </div>
                        </div>

                        <div>
                            <label class="text-[23px] text-gray-600">Correo Electrónico</label>
                            <p class="neumorfismo-sobre-suave text-[20px]"> {{ userData.user.correo }} </p>
                        </div>

                        <!-- Número de Teléfono -->
                        <div>
                            <label class="text-[23px] text-gray-600">Número de Teléfono</label>
                            <div v-if="!isEditing" class="neumorfismo-sobre-suave text-[20px] p-2">
                                {{ editedUserData.telefono }}
                            </div>
                            <input 
                                v-else
                                v-model.number="editedUserData.telefono"
                                type="number"
                                class="neumorfismo-input text-[16px] p-2 rounded w-full"
                                placeholder="Número de teléfono"
                                :disabled="saving"
                            />
                        </div>

                        <!-- Dirección -->
                        <div>
                            <label class="text-[23px] text-gray-600">Dirección</label>
                            <div v-if="!isEditing" class="neumorfismo-sobre-suave text-[20px] p-2">
                                {{ editedUserData.direccion }}
                            </div>
                            <div v-else class="space-y-2">
                                <select 
                                    v-model="editedUserData.provincia"
                                    class="neumorfismo-input text-[16px] p-2 rounded w-full"
                                    :disabled="saving"
                                >
                                    <option value="">Seleccione provincia</option>
                                    <option value="San José">San José</option>
                                    <option value="Alajuela">Alajuela</option>
                                    <option value="Cartago">Cartago</option>
                                    <option value="Heredia">Heredia</option>
                                    <option value="Guanacaste">Guanacaste</option>
                                    <option value="Puntarenas">Puntarenas</option>
                                    <option value="Limón">Limón</option>
                                </select>
                                <input 
                                    v-model="editedUserData.canton"
                                    class="neumorfismo-input text-[16px] p-2 rounded w-full"
                                    placeholder="Cantón"
                                    :disabled="saving"
                                />
                                <input 
                                    v-model="editedUserData.distrito"
                                    class="neumorfismo-input text-[16px] p-2 rounded w-full"
                                    placeholder="Distrito"
                                    :disabled="saving"
                                />
                                <input 
                                    v-model="editedUserData.direccionParticular"
                                    class="neumorfismo-input text-[16px] p-2 rounded w-full"
                                    placeholder="Dirección particular"
                                    :disabled="saving"
                                />
                            </div>
                        </div>

                        <!-- Cuenta IBAN -->
                        <div>
                            <label class="text-[23px] text-gray-600">Cuenta IBAN</label>
                            <div v-if="!isEditing" class="neumorfismo-sobre-suave text-[20px] p-2">
                                {{ editedUserData.iban }}
                            </div>
                            <input 
                                v-else
                                v-model="editedUserData.iban"
                                class="neumorfismo-input text-[16px] p-2 rounded w-full"
                                placeholder="IBAN"
                                :disabled="saving"
                            />
                        </div>
                    </div>
                </div>

                <!-- Información Laboral -->
                <div class="neumorfismo-tarjeta px-[39px] py-[37px]">
                  <p class="text-[32px] font-medium mb-4">Información Laboral</p>
                  <div class="space-y-[25px]">
                    <div>
                      <label class="text-[23px] text-gray-600">Empresa</label>
                      <p class="neumorfismo-sobre-suave text-[20px]"> {{ userData.project.nombreEmpresa }} </p>
                    </div>
                    
                    <!-- Departamento - editable para empleadores -->
                    <div>
                      <label class="text-[23px] text-gray-600">Departamento</label>
                      <p v-if="!isEditing || !isEmployer" class="neumorfismo-sobre-suave text-[20px] p-2">
                        {{ userData.user.departamento }}
                      </p>
                      <input 
                        v-else
                        v-model="editedUserData.departamento"
                        class="neumorfismo-input text-[16px] p-2 rounded w-full"
                        placeholder="Departamento"
                        :disabled="saving"
                      />
                    </div>
                    
                    <!-- Puesto - editable para empleadores -->
                    <div>
                      <label class="text-[23px] text-gray-600">Puesto</label>
                      <p v-if="!isEditing || !isEmployer" class="neumorfismo-sobre-suave text-[20px] p-2">
                        {{ userData.user.puesto }}
                      </p>
                      <input 
                        v-else
                        v-model="editedUserData.puesto"
                        class="neumorfismo-input text-[16px] p-2 rounded w-full"
                        placeholder="Puesto"
                        :disabled="saving"
                      />
                    </div>
                    
                    <!-- Tipo de Contrato -->
                    <div>
                      <label class="text-[23px] text-gray-600">Tipo de Contrato</label>
                      <p class="neumorfismo-sobre-suave text-[20px] p-2">
                        {{ userData.user.tipoContrato }}
                      </p>
                    </div>
                    
                    <div>
                      <label class="text-[23px] text-gray-600">Fecha de Contratación</label>
                      <p class="neumorfismo-sobre-suave text-[20px] p-2"> {{ formattedDate }} </p>
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
                        <p class="neumorfismo-sobre-suave text-[20px]"> {{ userData.project.periodoPago }} </p>
                    </div>

                    <!-- Salario - editable para empleadores -->
                    <div>
                      <label class="text-[23px] text-gray-600">Salario</label>
                      <p v-if="!isEditing || !isEmployer" class="neumorfismo-sobre-suave text-[20px] p-2">
                        {{ formattedSalary }}
                      </p>
                      <input 
                        v-else
                        v-model.number="editedUserData.salario"
                        type="number"
                        class="neumorfismo-input text-[16px] p-2 rounded w-full"
                        placeholder="Salario"
                        :disabled="saving"
                      />
                    </div>

                </div>
            </div> <!-- Fin Tercera Parte 1x1 -->

        </div> <!-- Fin de Container -->

    </div> <!-- Fin de Componente -->
</template>

<script>
import "../assets/Neumorfismo.css";
import EmployeeHeader from './common/EmployeeHeader.vue'
import MainEmployerHeader from './common/MainEmployerHeader.vue'
import apiConfig from '../config/api.js'   // <-- agregado

export default {
  name: 'EditInfoEmployee',
  components: { 
    EmployeeHeader,
    MainEmployerHeader
  },
  props: {
    id: {
      type: [String, Number],
      required: true
    }
  },
  data() {
    return {
      isEmployer: false,
      userData: {
        user: {
          nombre: '',
          puesto: '',
          departamento: '',
          correo: '',
          telefono: '',
          direccion: '',
          iban: '',
          tipoContrato: '',
          fechaContratacion: '',
          salario: 0
        },
        project: {
          nombreEmpresa: '',
          periodoPago: ''
        }
      },
      editedUserData: {
        nombre: '',
        segundoNombre: '',
        apellidos: '',
        correo: '',
        telefono: 0,
        provincia: '',
        canton: '',
        distrito: '',
        direccionParticular: '',
        direccion: '',
        iban: '',
        departamento: '',
        puesto: '', 
        salario: 0
      },
      originalUserData: {},
      user: null,
      loading: false,
      saving: false,
      error: null,
      successMessage: null,
      isEditing: false
    }
  },

  computed: {
    formattedDate() {
      if (!this.userData.user.fechaContratacion) return '';
      const date = new Date(this.userData.user.fechaContratacion);
      return date.toLocaleDateString('es-ES');
    },

    formattedSalary() {
      if (!this.userData.user.salario) return '';
      return new Intl.NumberFormat('es-CR', {
        style: 'currency',
        currency: 'CRC'
      }).format(this.userData.user.salario);
    },

    fullName() {
      return `${this.editedUserData.nombre} 
        ${this.editedUserData.segundoNombre || ''} 
        ${this.editedUserData.apellidos}`.replace(/\s+/g, ' ').trim();
    }
  },

  methods: {
    enableEditing() {
      this.isEditing = true;
      this.successMessage = null;
      this.error = null;
      
      this.originalUserData = { ...this.editedUserData };
    },

    cancelEditing() {
      this.isEditing = false;
      this.error = null;
      
      this.editedUserData = { ...this.originalUserData };
    },

    async saveChanges() {
      this.saving = true;
      this.error = null;
      this.successMessage = null;

      try {
        if (!this.editedUserData.nombre.trim() || 
            !this.editedUserData.apellidos.trim() ||
            !this.editedUserData.provincia.trim() ||
            !this.editedUserData.canton.trim() ||
            !this.editedUserData.distrito.trim() ||
            !this.editedUserData.direccionParticular.trim()) {
          this.error = 'Todos los campos son requeridos';
          this.saving = false;
          return;
        }

        // Validación de campos laborales si es empleador
        if (this.isEmployer && (
          !this.editedUserData.departamento.trim() ||
          !this.editedUserData.puesto.trim() ||
          this.editedUserData.salario <= 0
        )) {
          this.error = 'Todos los campos laborales son requeridos';
          this.saving = false;
          return;
        }

        const updateData = {
          nombre: this.editedUserData.nombre.trim(),
          segundoNombre: this.editedUserData.segundoNombre.trim(),
          apellidos: this.editedUserData.apellidos.trim(),
          telefono: this.editedUserData.telefono,
          provincia: this.editedUserData.provincia.trim(),
          canton: this.editedUserData.canton.trim(),
          distrito: this.editedUserData.distrito.trim(),
          direccionParticular: this.editedUserData.direccionParticular.trim(),
          iban: this.editedUserData.iban.trim(),

          departamento: this.editedUserData.departamento.trim(),
          puesto: this.editedUserData.puesto.trim(),
          salario: this.editedUserData.salario

        };

        console.log('Enviando datos al backend:', updateData);

        const response = await fetch(apiConfig.endpoints.profileEmployee(this.id), {
          method: 'PUT',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify(updateData)
        });

        const result = await response.json();

        if (response.ok && result.success) {
          this.successMessage = result.message;
          this.isEditing = false;
          
          await this.fetchUserData();
        } else {
          this.error = result.message || 'Error al guardar los cambios';
        }

      } catch (error) {
        console.error('Error al guardar cambios:', error);
        this.error = 'Error de conexión al servidor';
      } finally {
        this.saving = false;
      }
    },

    parseExistingData() {
      const nombreCompleto = this.userData.user.nombre?.split(' ') || [];
      this.editedUserData.nombre = nombreCompleto[0] || '';
      this.editedUserData.segundoNombre = nombreCompleto[1] || '';
      this.editedUserData.apellidos = nombreCompleto.slice(2).join(' ') || '';
      
      this.editedUserData.correo = this.userData.user.correo || '';
      this.editedUserData.telefono = this.userData.user.telefono || 0;
      this.editedUserData.iban = this.userData.user.iban || '';
      this.editedUserData.direccion = this.userData.user.direccion || '';

      this.editedUserData.departamento = this.userData.user.departamento || '';
      this.editedUserData.puesto = this.userData.user.puesto || '';
      this.editedUserData.tipoContrato = this.userData.user.tipoContrato || '';
      this.editedUserData.salario = this.userData.user.salario || 0;
      
      if (this.userData.user.direccion) {
        const direccionParts = this.userData.user.direccion.split(', ');
        if (direccionParts.length >= 4) {
          this.editedUserData.provincia = direccionParts[0] || '';
          this.editedUserData.canton = direccionParts[1] || '';
          this.editedUserData.distrito = direccionParts[2] || '';
          this.editedUserData.direccionParticular = direccionParts[3] || '';
        }
      }
    },

    async fetchUserData() {
      if (!this.id) {
        this.error = 'No se pudo identificar al usuario.';
        return;
      }

      this.loading = true;
      this.error = null;

      try {
        const response = await fetch(apiConfig.endpoints.profileEmployee(this.id));
        
        if (!response.ok) {
          throw new Error('Error al obtener los datos del usuario');
        }
        
        const data = await response.json();
        this.userData = data;
        this.parseExistingData();
      } catch (error) {
        console.error('Error en la petición:', error);
        this.error = error.message || 'Error al cargar los datos del perfil';
      } finally {
        this.loading = false;
      }
    },

    loadUserFromLocalStorage() {
      const userRaw = localStorage.getItem('user');
      if (userRaw) {
        try {
          this.user = JSON.parse(userRaw);
        } catch {
          this.user = null;
        }
      }
    },


    checkUserRole() {
      const userRaw = localStorage.getItem('user');
      if (userRaw) {
        try {
          const user = JSON.parse(userRaw);
          this.isEmployer = user.tipoUsuario === 'Empleador' || user.rol === 'Empleador';
        } catch {
          this.isEmployer = false;
        }
      }
    },


  },

  created() {
    this.loadUserFromLocalStorage();
    this.checkUserRole();
  },

  mounted() {
    this.fetchUserData();
  }
}
</script>
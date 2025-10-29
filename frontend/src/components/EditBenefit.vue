<template>
    <div class="bg-[#dbeafe] min-h-screen">
        <MainEmployerHeader @project-changed="onProjectChanged"/>
        <DashboardProjectSubHeader
        :selected-section="selectedSection"
        @section-change="selectedSection = $event"
        />
        <div class="mx-[171px] my-[41px] space-y-[41px] pb-[41px]">
            <div class="neumorfismo-tarjeta w-[700px] space-y-[23px] p-[24px]">
            
                <!-- Nombre del beneficio -->
                <div class="mb-4">
                <label class="block text-gray-700 mb-1">Nombre del Beneficio</label>
                <input
                    type="text"
                    v-model="beneficio.nombre"
                    class="neumorfismo-sobre-suave w-full p-2"
                />
                <small class="text-gray-400 text-sm">
                    El nombre debe ser único dentro de la empresa
                </small>
                </div>
        
                <div>
                    <!-- Tipo de beneficio -->
                    <div class="mb-4">
                    <label class="block text-gray-700 mb-1">Tipo de Cálculo</label>
                    <p class="neumorfismo-sobre-suave px-4 py-2 w-fit!"> {{ beneficio.tipoCalculo }} </p>
                    </div>
            
                    <!-- Campo dinámico según el tipo de cálculo -->
                    <div v-if="beneficio.tipoCalculo === 'Monto Fijo'" class="mb-4">
                        <label class="block text-gray-700 mb-1">Valor del Beneficio</label>
                        <input
                            type="number"
                            v-model.number="beneficio.valor"
                            class="neumorfismo-sobre-suave w-full p-2"
                            placeholder="Ingrese el monto"
                        />
                    </div>

                    <div v-else-if="beneficio.tipoCalculo === 'Porcentaje'" class="mb-4">
                        <label class="block text-gray-700 mb-1">Porcentaje del Beneficio</label>
                        <input
                            type="number"
                            v-model.number="beneficio.porcentaje"
                            class="neumorfismo-sobre-suave w-full p-2"
                            placeholder="Ingrese el porcentaje"
                            min="0"
                            max="100"
                        />
                    </div>

                    <div v-else-if="beneficio.tipoCalculo === 'API'" class="mb-4">
                        <label class="block text-gray-700 mb-1">Tipo de Beneficio</label>
                        <p class="neumorfismo-sobre-suave px-4 py-2 w-fit!">API - Configuración Externa</p>
                    </div>
                </div>
        
        
                <!-- Descripción -->
                <div class="mb-6">
                <label class="block text-gray-700 mb-1">Descripción</label>
                <textarea
                    v-model="beneficio.descripcion"
                    rows="2"
                    class="neumorfismo-sobre-suave w-full p-2"
                ></textarea>
                </div>
        
                <!-- Botones -->
                <div class="grid grid-cols-[1fr_1fr] gap-3">
                <button class="neumorfismo-boton-azul px-5 py-2" @click="guardarCambios" :disabled="loading">
                    {{ loading ? 'Guardando...' : 'Guardar Cambios' }}
                </button>
                <button class="neumorfismo-boton px-5 py-2" @click="cancelar" :disabled="loading">
                    Cancelar
                </button>
                </div>

                <!-- Mensaje de error -->
                <div v-if="error" class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded">
                    {{ error }}
                </div>
            
            </div>
        </div>
    </div>
</template>

<script>
import "../assets/Neumorfismo.css";
import MainEmployerHeader from './common/MainEmployerHeader.vue'
import DashboardProjectSubHeader from './projectDashboard/DashboardProjectSubHeader.vue'

export default {
  name: "EditBenefit",
  components: {
    MainEmployerHeader,
    DashboardProjectSubHeader,
  },
  props: {
    id: {
      type: [String, Number],
      required: true
    }
  },
  data() {
    return {
      beneficio: {
        id: null,
        nombre: "",
        tipoCalculo: "",
        tipo: "",
        valor: null,
        porcentaje: null,
        descripcion: "",
        idEmpresa: null
      },
      loading: false,
      error: null
    };
  },
  methods: {
    async guardarCambios() {
      try {
        this.loading = true;
        this.error = null;

        // Validaciones
        if (!this.beneficio.nombre.trim()) {
          this.error = "El nombre del beneficio es requerido";
          return;
        }

        // Validaciones específicas por tipo
        if (this.beneficio.tipoCalculo === 'Monto Fijo' && (!this.beneficio.valor || this.beneficio.valor <= 0)) {
          this.error = "El valor del beneficio es requerido y debe ser mayor a 0";
          return;
        }

        if (this.beneficio.tipoCalculo === 'Porcentaje' && (!this.beneficio.porcentaje || this.beneficio.porcentaje <= 0)) {
          this.error = "El porcentaje del beneficio es requerido y debe ser mayor a 0";
          return;
        }

        console.log("Datos a guardar:", this.beneficio);
        
        // Simulación de guardado (reemplazar con llamada real al backend)
        await this.actualizarBeneficio();
        
        alert("Cambios guardados correctamente ✅");
        this.$router.push('/benefits');
        
      } catch (error) {
        console.error("Error al guardar:", error);
        this.error = "Error al guardar los cambios";
      } finally {
        this.loading = false;
      }
    },

    cancelar() {
      this.$router.push('/benefits');
    },

    async cargarBeneficio() {
      try {
        this.loading = true;
        // TODO: Reemplazar con llamada real al backend
        // const response = await fetch(`http://localhost:5011/api/Benefit/${this.id}`);
        // const data = await response.json();
        
        // Simulación de datos (eliminar cuando tengamos el backend)
        await new Promise(resolve => setTimeout(resolve, 500));
        this.beneficio = {
          id: this.id,
          nombre: "Seguro Médico Privado",
          tipoCalculo: "Monto Fijo",
          tipo: "Bonificación",
          valor: 45000,
          porcentaje: null,
          descripcion: "Cobertura médica completa para empleados y familia",
          idEmpresa: 6
        };
        
      } catch (error) {
        console.error("Error al cargar beneficio:", error);
        this.error = "Error al cargar los datos del beneficio";
      } finally {
        this.loading = false;
      }
    },

    async actualizarBeneficio() {
      // TODO: Reemplazar con llamada real al backend
      // const response = await fetch(`http://localhost:5011/api/Benefit/${this.id}`, {
      //   method: 'PUT',
      //   headers: { 'Content-Type': 'application/json' },
      //   body: JSON.stringify(this.beneficio)
      // });
      
      // Simulación de guardado
      await new Promise(resolve => setTimeout(resolve, 1000));
      return { success: true };
    },

    //onProjectChanged(project) {
      // Manejar cambio de proyecto si es necesario
    //}
  },
  mounted() {
    this.cargarBeneficio();
  }
};
</script>
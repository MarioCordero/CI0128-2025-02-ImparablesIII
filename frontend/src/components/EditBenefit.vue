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
                            disabled
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
                            disabled
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

                <!-- Mensaje de éxito -->
                <div v-if="successMessage" class="bg-green-100 border border-green-400 text-green-700 px-4 py-3 rounded">
                    {{ successMessage }}
                </div>
            
            </div>
        </div>
    </div>
</template>

<script>
import "../assets/Neumorfismo.css";
import MainEmployerHeader from './common/MainEmployerHeader.vue'
import DashboardProjectSubHeader from './employer/projectDashboard/DashboardProjectSubHeader.vue'

export default {
  name: "EditBenefit",
  components: {
    MainEmployerHeader,
    DashboardProjectSubHeader,
  },
  props: {
    // Props automáticos desde la ruta con params
    companyId: {
      type: [String, Number],
      required: true
    },
    name: {
      type: String,
      required: true
    }
  },
  data() {
    return {
      selectedSection: 'benefits',
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
      error: null,
      successMessage: null
    };
  },
  methods: {
    async guardarCambios() {
      try {
        this.loading = true;
        this.error = null;
        this.successMessage = null;

        // Validaciones
        if (!this.beneficio.nombre.trim()) {
          this.error = "El nombre del beneficio es requerido";
          return;
        }

        if (this.beneficio.nombre.trim().length > 50) {
          this.error = "El nombre no puede exceder 50 caracteres";
          return;
        }

        // Validar que solo contenga letras y espacios
        const nameRegex = /^[a-zA-ZÀ-ÿ\u00f1\u00d1\s]+$/;
        if (!nameRegex.test(this.beneficio.nombre.trim())) {
          this.error = "El nombre solo puede contener letras y espacios";
          return;
        }

        if (this.beneficio.descripcion && this.beneficio.descripcion.length > 200) {
          this.error = "La descripción no puede exceder 200 caracteres";
          return;
        }

        console.log("Datos a guardar:", this.beneficio);
        
        const result = await this.actualizarBeneficio();
        
        if (result.success) {
          this.successMessage = result.message;
          setTimeout(() => {
            this.$router.go(-1);
          }, 1500);
        } else {
          this.error = result.message;
        }
        
      } catch (error) {
        console.error("Error al guardar:", error);
        this.error = error.message || "Error al guardar los cambios";
      } finally {
        this.loading = false;
      }
    },

    cancelar() {
      this.$router.go(-1);
    },

    async cargarBeneficio() {
      try {
        this.loading = true;
        this.error = null;

        const companyId = this.companyId;
        const benefitName = this.name;

        console.log('Cargando beneficio con:', { companyId, benefitName });

        const response = await fetch(`http://localhost:5011/api/Benefit/company/${companyId}/benefit/${encodeURIComponent(benefitName)}`);
        
        if (!response.ok) {
          if (response.status === 404) {
            throw new Error('Beneficio no encontrado');
          }
          throw new Error('Error al cargar el beneficio');
        }
        
        const data = await response.json();
        
        this.beneficio = {
          id: data.name,
          nombre: data.name,
          tipoCalculo: data.calculationType,
          tipo: data.type,
          valor: data.value,
          porcentaje: data.percentage,
          descripcion: data.descripcion || '',
          idEmpresa: data.companyId
        };
        
      } catch (error) {
        console.error("Error al cargar beneficio:", error);
        this.error = error.message;
      } finally {
        this.loading = false;
      }
    },

    async actualizarBeneficio() {
      const companyId = this.companyId;
      const originalName = this.name;
      
      const updateData = {
        name: this.beneficio.nombre.trim(),
        descripcion: this.beneficio.descripcion.trim()
      };

      const response = await fetch(`http://localhost:5011/api/Benefit/company/${companyId}/benefit/${encodeURIComponent(originalName)}`, {
        method: 'PUT',
        headers: { 
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(updateData)
      });
      
      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || 'Error al actualizar');
      }
      
      return await response.json();
    },

  },
  mounted() {
    this.cargarBeneficio();
  },
  watch: {
    // Si cambian los parámetros de la ruta, recargar el beneficio
    '$route.params': {
      handler() {
        this.cargarBeneficio();
      },
      deep: true
    }
  }
};
</script>
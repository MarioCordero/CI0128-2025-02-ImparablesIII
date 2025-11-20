<template>
  <div class="w-full h-[537px] flex flex-col gap-4 box-border">

    <h2>Lista de Empleados</h2>

    <!-- Loading state -->
    <div v-if="loading" class="text-center py-8">
      <p class="text-gray-600">Cargando empleados...</p>
    </div>

    <!-- Error state -->
    <div v-if="error" class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
      {{ error }}
    </div>

    <!-- Contenedor con scroll -->
    <div v-if="!loading && !error" class="scrollable-card">
      
      <div
        v-for="empleado in empleados"
        :key="empleado.id"
        class="employee-item neumorphism-on-small-item"
      >
        <!-- Columna izquierda -->
        <div class="flex items-center gap-3">
          <div class="avatar neumorphism-on-small-item">{{ empleado.iniciales }}</div>

          <div class="flex flex-col gap-0">
            <p class=" text-[20px] text-black font-medium m-0 p-0">{{ empleado.nombreCompleto }}</p>
            <p class=" text-[18px] text-gray-500 font-normal m-0 p-0">{{ empleado.puesto }}</p>

            <div class="flex flex-row gap-2 text-[16px] text-gray-500 font-normal">
              <span class="mr-[5px]">{{ empleado.departamento }}</span>
              <span class="mr-[5px]">{{ empleado.correo }}</span>
              <span>{{ empleado.telefono || 'Sin teléfono' }}</span>
            </div>
          </div>
        </div>

        <!-- Columna derecha -->
        <div class="flex items-center gap-3">
          <div class="flex flex-col gap-2">
            <p class=" text-[18px] text-gray-500 font-normal m-0 p-0 text-right">
              ₡{{ empleado.salario.toLocaleString() }}
            </p>
  
            <div class="flex flex-row gap-2 text-[16px] font-normal">
              <span class="py-[3px] px-[9px] rounded-[100px] text-white bg-[#7476ff]">
                {{ empleado.tipoContrato }}
              </span>

              <span class="py-[3px] px-[9px] rounded-[100px] text-black bg-[#b9ffbc]">
                {{ empleado.estado }}
              </span>
            </div>
          </div>

          <button class="neumorphism-button-normal-light p-[7px]! rounded-full!" @click="editEmployee(empleado.id)">
            ✏️
          </button>
        </div>
      </div>
    </div>

    <!-- Empty state -->
    <div v-if="!loading && !error && empleados.length === 0" class="text-center py-0 my-0">

      <p class="text-gray-600 text-[50px]">No hay empleados registrados en esta empresa.</p>

    </div>
  </div>
</template>

<script>
  export default {
    name: 'ListaEmpleados',
    props: {
      projectId: {
        type: [String, Number],
        required: true
      }
    },

    data() {
      return {
        empleados: [],
        loading: false,
        error: null
      }
    },

    methods: {

      obtenerIniciales(nombreCompleto) {
        if (!nombreCompleto) return 'NN';
        return nombreCompleto
          .split(' ')
          .map(word => word.charAt(0))
          .join('')
          .toUpperCase()
          .substring(0, 2);
      },

      async fetchEmpleados() {

        if (!this.projectId) {
          this.error = 'No se proporcionó ID de empresa';
          return;
        }

        this.loading = true;
        this.error = null;

        try {

          const response = await fetch(`http://localhost:5011/api/Project/${this.projectId}/employees`);
          
          if (!response.ok) {
            throw new Error('Error al cargar los empleados');
          }
          
          const data = await response.json();
          
          const empleadosArray = data.employees || [];
          
          this.empleados = empleadosArray.map(empleado => ({
            id: empleado.id,
            iniciales: this.obtenerIniciales(empleado.nombreCompleto),
            nombreCompleto: empleado.nombreCompleto,
            puesto: empleado.puesto,
            departamento: empleado.departamento,
            correo: empleado.correo,
            telefono: empleado.telefono,
            salario: empleado.salario,
            tipoContrato: empleado.tipoContrato,
            estado: 'Activo' // Por defecto, aun no tenemos eliminacion ni desactivacion
          }));

        } catch (error) {

          console.error('Error al cargar empleados:', error);
          this.error = error.message || 'Error al cargar los empleados';

        } finally {

          this.loading = false;

        }
      },

      editEmployee(employeeId) {
        this.$router.push(`/edit-employee/${employeeId}`);
      },
  },

  watch: {
    projectId: {
      immediate: true,
      handler(newProjectId) {
        if (newProjectId) {
          this.fetchEmpleados();
        }
      }
    }
  }
}
</script>

<style scoped>
.employee-item {
  width: 100%;
  display: flex;
  justify-content: space-between;
  align-items: center;
  box-sizing: border-box;
  padding: 27px;
  margin-top: 5px;
  margin-bottom: 5px;
  margin-left: 10px;
}

</style>
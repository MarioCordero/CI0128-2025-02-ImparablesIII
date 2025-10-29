<template>
  <div class="lista-empleados">

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
    <div v-if="!loading && !error" class="contenedor-empleados">
      
      <div
        v-for="empleado in empleados"
        :key="empleado.id"
        class="empleado-item neumorfismo-sobre-suave-np"
      >
        <!-- Columna izquierda -->
        <div class="empleado-izquierda">
          
          <div class="avatar neumorfismo-sobre rounded-full">{{ empleado.iniciales }}</div>

          <div class="datos-personales">

            <p class=" text-[20px] text-black font-medium m-0 p-0">{{ empleado.nombreCompleto }}</p>
            <p class=" text-[18px] text-gray-500 font-normal m-0 p-0">{{ empleado.puesto }}</p>

            <div class="contacto text-[16px] text-gray-500 font-normal">
              <span class="mr-[5px]">{{ empleado.departamento }}</span>
              <span class="mr-[5px]">{{ empleado.correo }}</span>
              <span>{{ empleado.telefono || 'Sin teléfono' }}</span>
            </div>

          </div>

        </div>

        <!-- Columna derecha -->
        <div class="empleado-derecha">

          <div class="datos-personales">

            <p class=" text-[18px] text-gray-500 font-normal m-0 p-0 text-right">
              ₡{{ empleado.salario.toLocaleString() }}
            </p>
  
            <div class="contacto text-[16px] font-normal">

              <span class="jornada">{{ empleado.tipoContrato }}</span>
              <span class="estado">{{ empleado.estado }}</span>

            </div>

          </div>

          <button class="neumorfismo-boton p-[7px] rounded-full!">✏️</button>

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
  import "../assets/Neumorfismo.css"

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
      }
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
.lista-empleados {
  width: 100%;
  height: 537px;
  display: flex;
  flex-direction: column;
  gap: 16px;
  box-sizing: border-box;
}

.contenedor-empleados {
  width: 100%;
  height: 100%;
  overflow-y: auto;
  overflow-x: hidden;
  display: flex;
  flex-direction: column;
  gap: 10px;
  scrollbar-gutter: stable;
  padding-right: 40px;
}

.empleado-item {
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

.empleado-izquierda {
  display: flex;
  align-items: center;
  gap: 12px;
}

.avatar {
  width: 64px;
  height: 64px;
  display: flex;
  justify-content: center;
  align-items: center;
}

.datos-personales {
  display: flex;
  flex-direction: column;
  gap: 0px;
}

.contacto {
  display: flex;
  flex-direction: row;
  gap: 8px;
}

.empleado-derecha {
  display: flex;
  align-items: center;
  gap: 12px;
}



.contenedor-empleados::-webkit-scrollbar { /* */
  width: 20px;
}

.contenedor-empleados::-webkit-scrollbar-track { /* Fondo */
  background: #dbeafe;
  border-radius: 10px;
  width: 15px;
  box-shadow: inset 3px 3px 6px #bebebe,
              inset -3px -3px 6px #ffffff;
}

.contenedor-empleados::-webkit-scrollbar-thumb { /* Barra */

  background: #ffffff;
  border-radius: 10px;
  box-shadow: inset 0 0 3px 1px rgba(16, 72, 255, 1);
  transition: all 0.3s;

}

.jornada {
  background: rgba(16, 72, 255, 1);
  color: #ffffff;
  border-radius: 100px;
  padding-top: 3px;
  padding-left: 9px;
  padding-bottom: 3px;
  padding-right: 9px;
}

.estado {
  background: #b9ffbc;
  color: #000000;
  border-radius: 100px;
  padding-top: 3px;
  padding-left: 9px;
  padding-bottom: 3px;
  padding-right: 9px;
}




</style>
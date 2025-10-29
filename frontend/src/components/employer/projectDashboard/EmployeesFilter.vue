<template>
  <div class=" neumorfismo-tarjeta p-[25px] w-full space-y-[23px] bg-gray-200! cursor-not-allowed">
    <div>
        <span class=" text-[30px] line-through mr-[50px]"> Filtros </span>
        <span class=" text-[20px] text-blue-700"> (Proximamente) </span>
    </div>
   

    <!-- Tipo de contrato -->
    <div class="seccion">
      <label class="text-[18px] font-medium m-0 p-0">Tipo de Contrato</label>
      <div class="opciones">
        <label><input type="radio" name="contrato" value="Todos" checked /> Todos</label>
        <label><input type="radio" name="contrato" value="Tiempo Completo" /> Tiempo Completo</label>
        <label><input type="radio" name="contrato" value="Medio Tiempo" /> Medio Tiempo</label>
        <label><input type="radio" name="contrato" value="Contratista" /> Contratista</label>
      </div>
    </div>

    <!-- Departamento -->
    <div class="seccion">
      <label class="text-[18px] font-medium m-0 p-0">Departamento</label>
      <select v-model="departamento" class="neumorfismo-input-select">
        <option value="Todos">Todos los departamentos</option>
        <option value="Infraestructura">Infraestructura</option>
        <option value="Diseno">Diseño</option>
        <option value="Finanzas">Finanzas</option>
      </select>
    </div>

    <!-- Puesto -->
    <div class="seccion">
      <label class="text-[18px] font-medium m-0 p-0">Puesto</label>
      <select v-model="puesto" class="neumorfismo-input-select">
        <option value="Todos">Todos los puestos</option>
        <option value="Ingeniería Civil">Ingeniería Civil</option>
        <option value="Arquitectura">Arquitectura</option>
        <option value="Contador">Contador</option>
      </select>
    </div>

    <!-- Estado -->
    <div class="seccion">
      <label class="text-[18px] font-medium m-0 p-0">Estado</label>
      <select v-model="estado" class="neumorfismo-input-select">
        <option value="Todos">Todos los estados</option>
        <option value="Activo">Activo</option>
        <option value="Inactivo">Inactivo</option>
      </select>
    </div>
  </div>
</template>

<script>
// import "../assets/Neumorfismo.css";

export default {
  name: "EmployeesFilter",
  data() {
    return {
      departamento: "Todos",
      puesto: "Todos",
      estado: "Todos",
    };
  },
};
</script>

<style scoped>
.filtros {
  width: 100%;
  height: 537px;
  padding: 24px;
  display: flex;
  flex-direction: column;
  gap: 20px;
  box-sizing: border-box;
  border-radius: 20px;
  background: #e6efff;
}

.titulo {
  font-size: 20px;
  font-weight: 600;
  color: #1e40af;
  display: flex;
  align-items: center;
  gap: 8px;
  margin: 0;
}

.seccion {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.label {
  font-size: 16px;
  color: #1f2937;
  font-weight: 500;
}

.opciones {
  display: flex;
  flex-direction: column;
  gap: 6px;
  font-size: 15px;
  color: #374151;
}

.select {
  background: #f8fafc;
  border: none;
  border-radius: 12px;
  padding: 8px 10px;
  font-size: 15px;
  color: #374151;
  box-shadow: inset 3px 3px 6px #cfd8e3, inset -3px -3px 6px #ffffff;
  outline: none;
  transition: all 0.2s;
}

.select:focus {
  box-shadow: inset 0 0 6px 1px rgba(16, 72, 255, 0.8);
}


input[type="radio"] {
  appearance: none;
  width: 16px;
  height: 16px;
  background-color: #dbeafe;
  border: 1px solid #d1d1d1;
  border-radius: 4px;
  cursor: pointer;
  transition: all 0.2s ease;
  position: relative;
}

input[type="radio"]:checked {
  background-color: rgba(16, 72, 255, 1);
   /* crea un efecto de “punto blanco” interno */
}

input[type="radio"]:hover {
  background-color: rgba(16, 72, 255, 0.5);
  transform: scale(1.1);
}


</style>
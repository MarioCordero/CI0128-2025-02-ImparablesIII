<template>
  <div class="neumorphism-card p-[25px]! w-full space-y-[23px] bg-gray-200! cursor-not-allowed">
    <div>
        <span class=" text-[30px] line-through mr-[50px]"> Filtros </span>
        <span class=" text-[20px] text-[#7476ff]"> (Proximamente) </span>
    </div>
   

    <!-- Tipo de contrato -->
    <div class="flex flex-col gap-2">
      <label class="text-[18px] font-medium m-0 p-0">Tipo de Contrato</label>
      <div class="flex flex-col gap-[6px] text-[15px] text-[#374151]">
        <label><input type="radio" name="contrato" value="Todos" checked /> Todos</label>
        <label><input type="radio" name="contrato" value="Tiempo Completo" /> Tiempo Completo</label>
        <label><input type="radio" name="contrato" value="Medio Tiempo" /> Medio Tiempo</label>
        <label><input type="radio" name="contrato" value="Contratista" /> Contratista</label>
      </div>
    </div>

    <!-- Departamento -->
    <div class="flex flex-col gap-2">
      <label class="text-[18px] font-medium m-0 p-0">Departamento</label>
      <select v-model="departamento" class="neumorphism-input neumorphism-input-select">
        <option value="Todos">Todos los departamentos</option>
        <option value="Infraestructura">Infraestructura</option>
        <option value="Diseno">Diseño</option>
        <option value="Finanzas">Finanzas</option>
      </select>
    </div>

    <!-- Puesto -->
    <div class="flex flex-col gap-2">
      <label class="text-[18px] font-medium m-0 p-0">Puesto</label>
      <select v-model="puesto" class="neumorphism-input neumorphism-input-select">
        <option value="Todos">Todos los puestos</option>
        <option value="Ingeniería Civil">Ingeniería Civil</option>
        <option value="Arquitectura">Arquitectura</option>
        <option value="Contador">Contador</option>
      </select>
    </div>

    <!-- Estado -->
    <div class="flex flex-col gap-2">
      <label class="text-[18px] font-medium m-0 p-0">Estado</label>
      <select v-model="estado" class="neumorphism-input neumorphism-input-select">
        <option value="Todos">Todos los estados</option>
        <option value="Activo">Activo</option>
        <option value="Inactivo">Inactivo</option>
      </select>
    </div>
  </div>
</template>

<script>
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
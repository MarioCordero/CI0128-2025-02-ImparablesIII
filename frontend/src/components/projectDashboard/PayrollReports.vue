<template>
  <div class="w-full min-h-screen bg-[#eaf6fd] p-6">
    <h1 class="text-2xl font-bold mb-2">Reportes de planilla</h1>
    <hr class="mb-6 border-[#c7e0f7]">

    <button class="custom-button neumorphism-light mb-6 flex items-center gap-2">
      <span>💸</span>
      Pagar planilla
    </button>

    <div class="flex gap-6 mb-8 flex-wrap">
      <div class="neumorphism-card p-6 flex-1 flex flex-col items-start min-w-[260px] mb-4">
        <span class="text-xs text-gray-500 mb-1 flex items-center gap-1">
          Nómina Total Mensual
          <span class="text-blue-500 text-base">$</span>
        </span>
        <span class="text-2xl font-bold text-gray-800">₡2,400,000</span>
      </div>
      <div class="neumorphism-card p-6 flex-1 flex flex-col items-start min-w-[200px] mb-4">
        <span class="text-xs text-gray-500 mb-1 flex items-center gap-1">
          Empleados en Planilla
          <span class="text-green-500 text-base">👤</span>
        </span>
        <span class="text-2xl font-bold text-gray-800">80</span>
      </div>
    </div>

    <div class="neumorphism-card p-6">
      <h2 class="text-base font-semibold mb-1">Detalle de Planilla por Fecha</h2>
      <p class="text-xs text-gray-500 mb-4">Desglose completo de salarios, deducciones y beneficios</p>
      <div class="overflow-x-auto">
        <table class="min-w-full text-sm text-left">
          <thead>
            <tr class="bg-[#eaf6fd]">
              <th class="py-2 px-4 font-medium">Fecha de pago</th>
              <th class="py-2 px-4 font-medium">Salario Bruto</th>
              <th class="py-2 px-4 font-medium">Deducciones de ley Empleador</th>
              <th class="py-2 px-4 font-medium">Deducciones ley empleados</th>
              <th class="py-2 px-4 font-medium">Beneficios</th>
              <th class="py-2 px-4 font-medium">Salario Neto</th>
            </tr>
          </thead>
          <tbody>
            <tr class="border-t border-[#e0e0e0] hover:bg-[#f4fbff]">
              <td class="py-2 px-4">15 octubre</td>
              <td class="py-2 px-4">₡3 500 000</td>
              <td class="py-2 px-4">₡525 000</td>
              <td class="py-2 px-4">₡525 000</td>
              <td class="py-2 px-4 text-blue-600 font-semibold">₡125 000</td>
              <td class="py-2 px-4">₡2 400 000</td>
            </tr>
            <tr class="h-8"></tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  data() {
    return {
      // Aquí puedes agregar datos reactivos si lo necesitas en el futuro
    };
  },
  computed: {
    // Computed properties si las necesitas
  },
  methods: {
    // Métodos para manejar eventos, por ejemplo, pagar planilla
  },
  mounted() {
    // Código a ejecutar cuando el componente se monta
  }
};
</script>
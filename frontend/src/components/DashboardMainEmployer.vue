<template>
  <div class="min-h-screen bg-gray-100 p-6">
    <!-- Header -->
    <header class="grid grid-cols-3 items-center gap-[247px] mb-6 rounded-lg bg-[#E9F7FF] px-[52px] min-h-[95px]">
        <!-- Columna 1: Título -->
        <div class="flex items-center">
            <div class="text-2xl font-bold">Panel de Empleador</div>
        </div>

        <!-- Columna 2: Dropdown centrado -->
        <div class="flex justify-center">
            <select class="border border-gray-300 rounded px-3 py-1 bg-white min-w-[459px]">
            <option>Seleccionar empresa</option>
            </select>
        </div>

        <!-- Columna 3: Botones alineados a la derecha -->
        <div class="flex justify-end items-center gap-4">
            <button class="px-4 py-2 bg-gray-200 rounded hover:bg-gray-300 transition">Configuración</button>
            <button class="px-4 py-2 bg-gray-200 rounded hover:bg-gray-300 transition">Cerrar Sesión</button>
        </div>
    </header>

    <!-- Greeting -->
    <section class="mb-6">
      <h1 class="text-2xl font-bold mb-1">¡Hola, Cristiano Ronaldo!</h1>
      <p class="text-gray-600">Administra los beneficios de tu organización</p>
    </section>

    <!-- Stats & Notifications (opcional) -->
    <div class="grid md:grid-cols-2 gap-6 mb-6">
      <!-- Rentabilidad -->
      <div class="bg-white shadow rounded p-4">
        <h2 class="font-bold mb-2">Rentabilidad Total</h2>
        <p class="text-gray-600 mb-4">Observa las ganancias de tus proyectos.</p>
        <ul class="space-y-2">
          <li class="flex justify-between">
            <span>Jack's Store (2 empleados)</span>
            <span class="text-green-600 font-bold">24%</span>
          </li>
          <li class="flex justify-between">
            <span>Patitos (369 empleados)</span>
            <span class="text-green-600 font-bold">16%</span>
          </li>
          <li class="flex justify-between">
            <span>Imparables III (4 empleados)</span>
            <span class="text-red-600 font-bold">12%</span>
          </li>
        </ul>
        <button class="mt-4 text-blue-600 font-bold">Ver Análisis</button>
      </div>

      <!-- Notificaciones -->
      <div class="bg-white shadow rounded p-4">
        <h2 class="font-bold mb-2">Notificaciones</h2>
        <ul class="space-y-2">
          <li class="flex justify-between items-center bg-yellow-100 p-2 rounded">
            <span>Horas Pendientes</span>
            <span class="bg-yellow-500 text-white rounded-full px-2">5</span>
          </li>
          <li class="flex justify-between items-center bg-blue-100 p-2 rounded">
            <span>Beneficios por Aprobar</span>
            <span class="bg-blue-500 text-white rounded-full px-2">3</span>
          </li>
          <li class="flex justify-between items-center bg-red-100 p-2 rounded">
            <span>Contrato por vencer</span>
            <span class="bg-red-500 text-white rounded-full px-2">1</span>
          </li>
        </ul>
      </div>
    </div>

    <!-- Mis Empresas -->
    <section>
        <div class="flex justify-between items-center mb-4 pb-2 border-b border-gray-300">
            <h2 class="text-xl font-bold">Mis Empresas</h2>
            <button class="bg-blue-500 text-white px-4 py-2 rounded">Agregar Empresa</button>
        </div>

      <div class="space-y-6">
        <!-- Empresa -->
        <div>
          <div class="flex items-center justify-between mb-2">
            <h3 class="text-lg font-bold">Imparables III</h3>
            <select class="border rounded px-3 py-1">
              <option>Administrar Empresa</option>
              <option>Detalles Empresa</option>
              <option>Gestionar Beneficios</option>
              <option>Gestionar Empleados</option>
              <option>Reportes de Planilla</option>
              <option>Eliminar</option>
            </select>
          </div>
          <div class="grid md:grid-cols-4 gap-4">
            <div class="bg-white shadow rounded p-4 text-gray-700 text-sm">
              <span class="font-semibold">Cédula Jurídica:</span>
              <div>3-123-456789</div>
            </div>
            <div class="bg-white shadow rounded p-4 text-gray-700 text-sm">
              <span class="font-semibold">Período de Pago:</span>
              <div>Opcional</div>
            </div>
            <div class="bg-white shadow rounded p-4 text-gray-700 text-sm">
              <span class="font-semibold">Empleados Activos:</span>
              <div>4</div>
            </div>
            <div class="bg-white shadow rounded p-4 text-gray-700 text-sm">
              <span class="font-semibold">Nómina Mensual:</span>
              <div>4000</div>
            </div>
          </div>
        </div>

        <!-- Empresa -->
        <div>
          <div class="flex items-center justify-between mb-2">
            <h3 class="text-lg font-bold">Patitos</h3>
            <select class="border rounded px-3 py-1">
              <option>Administrar Empresa</option>
              <option>Detalles Empresa</option>
              <option>Gestionar Beneficios</option>
              <option>Gestionar Empleados</option>
              <option>Reportes de Planilla</option>
              <option>Eliminar</option>
            </select>
          </div>
          <div class="grid md:grid-cols-4 gap-4">
            <div class="bg-white shadow rounded p-4 text-gray-700 text-sm">
              <span class="font-semibold">Cédula Jurídica:</span>
              <div>3-456-123456</div>
            </div>
            <div class="bg-white shadow rounded p-4 text-gray-700 text-sm">
              <span class="font-semibold">Período de Pago:</span>
              <div>Mensual</div>
            </div>
            <div class="bg-white shadow rounded p-4 text-gray-700 text-sm">
              <span class="font-semibold">Empleados Activos:</span>
              <div>369</div>
            </div>
            <div class="bg-white shadow rounded p-4 text-gray-700 text-sm">
              <span class="font-semibold">Nómina Mensual:</span>
              <div>191.8 M</div>
            </div>
          </div>
        </div>

        <!-- Empresa -->
        <div>
          <div class="flex items-center justify-between mb-2">
            <h3 class="text-lg font-bold">Jack's Store</h3>
            <select class="border rounded px-3 py-1">
              <option>Administrar Empresa</option>
              <option>Detalles Empresa</option>
              <option>Gestionar Beneficios</option>
              <option>Gestionar Empleados</option>
              <option>Reportes de Planilla</option>
              <option>Eliminar</option>
            </select>
          </div>
          <div class="grid md:grid-cols-4 gap-4">
            <div class="bg-white shadow rounded p-4 text-gray-700 text-sm">
              <span class="font-semibold">Cédula Jurídica:</span>
              <div>3-969-696969</div>
            </div>
            <div class="bg-white shadow rounded p-4 text-gray-700 text-sm">
              <span class="font-semibold">Período de Pago:</span>
              <div>Quincenal</div>
            </div>
            <div class="bg-white shadow rounded p-4 text-gray-700 text-sm">
              <span class="font-semibold">Empleados Activos:</span>
              <div>2</div>
            </div>
            <div class="bg-white shadow rounded p-4 text-gray-700 text-sm">
              <span class="font-semibold">Nómina Mensual:</span>
              <div>302.3 M</div>
            </div>
          </div>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup>
</script>
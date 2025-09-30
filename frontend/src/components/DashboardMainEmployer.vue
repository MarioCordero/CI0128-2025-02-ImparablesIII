<template>
  <div class="min-h-screen bg-[#E9F7FF] p-0">
    <!-- Header -->
    <header class="grid grid-cols-3 items-center gap-[247px] mb-0 rounded-lg bg-[#E9F7FF] px-[89px] min-h-[95px] max-h-[95px] shadow-[8px_8px_16px_#d1e3ee,-8px_-8px_16px_#ffffff]">
      <!-- Columna 1: Título -->
      <div class="flex items-center">
        <div class="w-[317px] h-auto gap-0">
          <p class="text-[24px] font-bold mb-0 whitespace-nowrap">Panel de Empleador</p>
          <p class="text-gray-600 text-[18px] mb-0 whitespace-nowrap">Gestion de Beneficios Corporativos</p>
        </div>
      </div>
      <!-- Columna 2: Dropdown centrado -->
      <div class="flex justify-center">
            <select class="bg-[#E9F7FF] shadow-[8px_8px_16px_#d1e3ee,-8px_-8px_16px_#ffffff] min-w-[459px] min-h-[40px] rounded-[4px] px-3 py-1">
                <option default>Seleccionar empresa</option>
                <option>Imparables III</option>
                <option>Patitos</option>
                <option>Jack's Store</option>
            </select>
      </div>
      <!-- Columna 3: Botones alineados a la derecha -->
      <div class="flex justify-end items-center gap-4">
        <button class="px-4 py-2 bg-[#E9F7FF] rounded-3 shadow-[4px_4px_8px_#d1e3ee,-4px_-4px_8px_#ffffff] hover:bg-blue-100 transition">Configuración</button>
        <button class="px-4 py-2 bg-[#E9F7FF] rounded-3 shadow-[4px_4px_8px_#d1e3ee,-4px_-4px_8px_#ffffff] hover:bg-blue-100 transition">Cerrar Sesión</button>
      </div>
    </header>
    
    <!-- Contenido -->
    <div class="mx-[171px] my-[41px] space-y-[41px]">
        <!-- Seccion saludo -->
        <!-- Greeting -->
        <section class="p-0">
            <h1 class="text-2xl font-bold mb-1">¡Hola, Cristiano Ronaldo!</h1>
            <p class="text-gray-600 text-[19px] font-medium">Administra los beneficios de tu organización</p>
        </section>


    <!-- Seccion Cartas Medianas -->

        <!-- Stats & Notifications -->
        <section class="grid md:grid-cols-2 gap-6 mb-6">
            <!-- Rentabilidad -->
            <div class="bg-[#E9F7FF] shadow-[8px_8px_16px_#d1e3ee,-8px_-8px_16px_#ffffff] p-[30px] rounded-[14px] w-[756px] h-[470px] flex flex-col justify-between">
                <div class="w-[696px] h-[410px] space-y-[26px]">

                    <!-- Titulo card -->
                    <div class="w-[696px] h-[50px]">
                        <p class="font-bold text-[20px] m-0">Rentabilidad</p>
                        <p class="text-gray-700 text-[16px] m-0">Observa las ganancias de tus proyectos.</p>
                    </div>
                    
                    
                    <div>
                        <!-- Resumen card -->
                        <div class="w-[696px] h-[54px] flex items-center justify-between p-0 mb-[14px]">
                            <div class="flex flex-col">
                                <p class="font-bold text-[20px] m-0 whitespace-nowrap">Rentabilidad Total</p>
                                <p class="text-gray-700 text-[16px] m-0">3 empresas</p>
                            </div>
                            <div class="flex flex-col items-end">
                                <span class="bg-green-500 text-white rounded px-3 py-1 font-bold text-base block w-fit shadow-[2px_2px_4px_#d1e3ee,-2px_-2px_4px_#ffffff]">
                                    17.3%
                                </span>
                                <span class="text-green-600 text-xs block">
                                    +2.6% vs mes ant.
                                </span>
                            </div>
                        </div>

                        <ul class="space-y-4">
                            <!-- Jack's Store -->
                            <li class="flex items-center justify-between">
                                <div class="flex items-center gap-3">
                                    <div class="w-[52px] h-[52px] flex items-center justify-center bg-[#E9F7FF] rounded-[10px] font-bold text-lg shadow-[4px_4px_8px_#d1e3ee,-4px_-4px_8px_#ffffff]">
                                        1
                                    </div>
                                    <div>
                                        <span class="font-bold">Jack's Store</span>
                                        <div class="text-gray-600 text-sm">2 empleados</div>
                                    </div>
                                </div>
                                <div class="flex flex-col items-end">
                                    <span class="bg-green-500 text-white rounded px-3 py-1 font-bold text-base shadow-[2px_2px_4px_#d1e3ee,-2px_-2px_4px_#ffffff]">
                                        24%
                                    </span>
                                    <span class="text-green-600 text-xs mt-1">
                                        +15% vs mes ant.
                                    </span>
                                </div>
                            </li>
                            <!-- Patitos -->
                            <li class="flex items-center justify-between">
                                <div class="flex items-center gap-3">
                                    <div class="w-[52px] h-[52px] flex items-center justify-center bg-[#E9F7FF] rounded-[10px] font-bold text-lg shadow-[4px_4px_8px_#d1e3ee,-4px_-4px_8px_#ffffff]">
                                        2
                                    </div>
                                    <div>
                                        <span class="font-bold">
                                            Patitos
                                        </span>
                                        <div class="text-gray-600 text-sm">
                                            369 empleados
                                        </div>
                                    </div>
                                </div>
                                <div class="flex flex-col items-end">
                                    <span class="bg-gray-400 text-white rounded px-3 py-1 font-bold text-base shadow-[2px_2px_4px_#d1e3ee,-2px_-2px_4px_#ffffff]">
                                        16%
                                    </span>
                                    <span class="text-gray-600 text-xs mt-1">
                                        +0% vs mes ant.
                                    </span>
                                </div>
                            </li>
                            <!-- Imparables III -->
                            <li class="flex items-center justify-between">
                                <div class="flex items-center gap-3">
                                    <div class="w-[52px] h-[52px] flex items-center justify-center bg-[#E9F7FF] rounded-[10px] font-bold text-lg shadow-[4px_4px_8px_#d1e3ee,-4px_-4px_8px_#ffffff]">
                                        3
                                    </div>
                                    <div>
                                        <span class="font-bold">
                                            Imparables III
                                        </span>
                                        <div class="text-gray-600 text-sm">
                                            4 empleados
                                        </div>
                                    </div>
                                </div>
                                <div class="flex flex-col items-end">
                                    <span class="bg-red-500 text-white rounded px-3 py-1 font-bold text-base shadow-[2px_2px_4px_#d1e3ee,-2px_-2px_4px_#ffffff]">
                                        12%
                                    </span>
                                    <span class="text-red-600 text-xs mt-1">
                                        -7% vs mes ant.
                                    </span>
                                </div>
                            </li>
                        </ul>
                    </div>

                    
                    <div class="flex justify-end mt-6">
                        <button class="flex items-center gap-2 bg-[#E9F7FF] shadow-[4px_4px_8px_#d1e3ee,-4px_-4px_8px_#ffffff] px-4 py-2 rounded-3 text-gray-700 hover:bg-blue-100 transition">
                            Ver Analisis
                        </button>
                    </div>
                    
                </div>
            </div>


            
            <!-- Notificaciones -->
            <div class="bg-[#E9F7FF] shadow-[8px_8px_16px_#d1e3ee,-8px_-8px_16px_#ffffff] space-y-[26px] rounded-[14px] p-[30px] w-[756px] h-[470px]">
            
                <!-- Titulo card -->
                <div class="w-[696px] h-[50px]">
                    <p class="font-bold text-[20px] m-0">Notificaciones</p>
                    <p class="text-gray-700 text-[16px] m-0">Notificaciones recientes o no atendidas</p>
                </div>
            
            <!-- Lista de Notificaciones -->
                <div class="w-[696px] h-auto p-0">
                    <ul class="space-y-4 w-[696px] m-0 pl-0 list-none">
                        <li class="flex justify-between items-center bg-yellow-100 rounded-[14px] shadow-[2px_2px_4px_#d1e3ee,-2px_-2px_4px_#ffffff] px-4 w-auto h-[67px]">
                            <span>Horas Pendientes</span>
                            <span class="bg-yellow-500 text-white rounded-full px-2">5</span>
                        </li>
                        <li class="flex justify-between items-center bg-blue-100 rounded-[14px] shadow-[2px_2px_4px_#d1e3ee,-2px_-2px_4px_#ffffff] px-4 w-auto h-[67px]">
                            <span>Beneficios por Aprobar</span>
                            <span class="bg-blue-500 text-white rounded-full px-2">3</span>
                        </li>
                        <li class="flex justify-between items-center bg-red-100 rounded-[14px] shadow-[2px_2px_4px_#d1e3ee,-2px_-2px_4px_#ffffff] px-4 w-auto h-[67px]">
                            <span>Contrato por vencer</span>
                            <span class="bg-red-500 text-white rounded-full px-2">1</span>
                        </li>
                    </ul>
                </div>
            </div>


        </section>

    <!-- Seccion Mis Empresas -->
      <section>
        <div class="flex justify-between items-center mb-0 bg-[#E9F7FF]">
            <h2 class="text-xl font-bold">Mis Empresas</h2>
            <button class="flex items-center gap-2 bg-[#E9F7FF] shadow-[4px_4px_8px_#d1e3ee,-4px_-4px_8px_#ffffff] px-4 py-2 rounded-3 text-gray-700 hover:bg-blue-100 transition">
                Agregar Empresa
            </button>
        </div>
        <div class="w-full h-[10px] my-2 rounded shadow-[2px_2px_6px_#d1e3ee,-2px_-2px_6px_#ffffff] bg-[#E9F7FF]"></div>
        <div class="space-y-6">
          <!-- Empresa -->
          <div>
            <div class="flex items-center justify-between mb-2">
              <h3 class="text-lg font-bold">Imparables III</h3>
              <select class="rounded px-3 py-1 shadow-[2px_2px_4px_#d1e3ee,-2px_-2px_4px_#ffffff]">
                <option>Administrar Empresa</option>
                <option>Detalles Empresa</option>
                <option>Gestionar Beneficios</option>
                <option>Gestionar Empleados</option>
                <option>Reportes de Planilla</option>
                <option>Eliminar</option>
              </select>
            </div>
            <div class="grid md:grid-cols-4 gap-4">
              <!-- Cédula Jurídica -->
              <div class="bg-[#E9F7FF] shadow-[8px_8px_16px_#d1e3ee,-8px_-8px_16px_#ffffff] rounded-[14px] w-[365px] h-[190px] flex items-center justify-center transition-all duration-200 hover:scale-110 hover:bg-[linear-gradient(to_right,#FF0000_4%,#0033FF_74%)] group">
                <div class="w-[303px] h-[128px] flex flex-col justify-between">
                  <div class="flex items-center justify-between mb-2">
                    <p class="font-bold text-[16px] group-hover:text-white transition-colors duration-200">Cédula Jurídica</p>
                    <svg class="w-6 h-6 group-hover:text-white transition-colors duration-200" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
                      <rect x="3" y="7" width="18" height="10" rx="2" stroke="currentColor"/>
                      <path d="M7 15h.01M7 11h.01" stroke="currentColor"/>
                    </svg>
                  </div>
                  <div>
                    <p class="font-bold text-[28px] mb-1 group-hover:text-white transition-colors duration-200">3-123-456789</p>
                    <p class="text-[15px] group-hover:text-white transition-colors duration-200">de la empresa</p>
                  </div>
                </div>
              </div>
              <!-- Período de Pago -->
              <div class="bg-[#E9F7FF] shadow-[8px_8px_16px_#d1e3ee,-8px_-8px_16px_#ffffff] rounded-[14px] w-[365px] h-[190px] flex items-center justify-center transition-all duration-200 hover:scale-110 hover:bg-[linear-gradient(to_right,#FF0000_4%,#0033FF_74%)] group">
                <div class="w-[303px] h-[128px] flex flex-col justify-between">
                  <div class="flex items-center justify-between mb-2">
                    <p class="font-bold text-[16px] group-hover:text-white transition-colors duration-200">Período de Pago</p>
                    <svg class="w-6 h-6 group-hover:text-white transition-colors duration-200" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
                      <rect x="3" y="5" width="18" height="16" rx="2" stroke="currentColor"/>
                      <path d="M16 3v4M8 3v4" stroke="currentColor"/>
                    </svg>
                  </div>
                  <div>
                    <p class="font-bold text-[28px] mb-1 group-hover:text-white transition-colors duration-200">Opcional</p>
                    <p class="text-[15px] group-hover:text-white transition-colors duration-200">cuando Mario quiera</p>
                  </div>
                </div>
              </div>
              <!-- Empleados Activos -->
              <div class="bg-[#E9F7FF] shadow-[8px_8px_16px_#d1e3ee,-8px_-8px_16px_#ffffff] rounded-[14px] w-[365px] h-[190px] flex items-center justify-center transition-all duration-200 hover:scale-110 hover:bg-[linear-gradient(to_right,#FF0000_4%,#0033FF_74%)] group">
                <div class="w-[303px] h-[128px] flex flex-col justify-between">
                  <div class="flex items-center justify-between mb-2">
                    <p class="font-bold text-[16px] group-hover:text-white transition-colors duration-200">Empleados Activos</p>
                    <svg class="w-6 h-6 group-hover:text-white transition-colors duration-200" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
                      <circle cx="9" cy="7" r="4" stroke="currentColor"/>
                      <path d="M17 11a4 4 0 1 0-8 0" stroke="currentColor"/>
                      <path d="M17 21v-2a4 4 0 0 0-4-4H7a4 4 0 0 0-4 4v2" stroke="currentColor"/>
                    </svg>
                  </div>
                  <div>
                    <p class="font-bold text-[28px] mb-1 group-hover:text-white transition-colors duration-200">4</p>
                    <p class="text-[15px] group-hover:text-white transition-colors duration-200">en esta empresa</p>
                  </div>
                </div>
              </div>
              <!-- Nómina Mensual -->
              <div class="bg-[#E9F7FF] shadow-[8px_8px_16px_#d1e3ee,-8px_-8px_16px_#ffffff] rounded-[14px] w-[365px] h-[190px] flex items-center justify-center transition-all duration-200 hover:scale-110 hover:bg-[linear-gradient(to_right,#FF0000_4%,#0033FF_74%)] group">
                <div class="w-[303px] h-[128px] flex flex-col justify-between">
                  <div class="flex items-center justify-between mb-2">
                    <p class="font-bold text-[16px] group-hover:text-white transition-colors duration-200">Nómina Mensual</p>
                    <svg class="w-6 h-6 group-hover:text-white transition-colors duration-200" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
                      <path d="M3 17l6-6 4 4 8-8" stroke="currentColor"/>
                    </svg>
                  </div>
                  <div>
                    <p class="font-bold text-[28px] mb-1 group-hover:text-white transition-colors duration-200">4000</p>
                    <p class="text-[15px] group-hover:text-white transition-colors duration-200">de colones</p>
                  </div>
                </div>
              </div>
            </div>
          </div>
          <!-- Empresa -->
          <div>
            <div class="flex items-center justify-between mb-2">
              <h3 class="text-lg font-bold">Patitos</h3>
              <select class="rounded px-3 py-1 shadow-[2px_2px_4px_#d1e3ee,-2px_-2px_4px_#ffffff]">
                <option>Administrar Empresa</option>
                <option>Detalles Empresa</option>
                <option>Gestionar Beneficios</option>
                <option>Gestionar Empleados</option>
                <option>Reportes de Planilla</option>
                <option>Eliminar</option>
              </select>
            </div>
            <div class="grid md:grid-cols-4 gap-4">
              <!-- Cédula Jurídica -->
              <div class="bg-[#E9F7FF] shadow-[8px_8px_16px_#d1e3ee,-8px_-8px_16px_#ffffff] rounded-[14px] w-[365px] h-[190px] flex items-center justify-center transition-all duration-200 hover:scale-110 hover:bg-[linear-gradient(to_right,#FF0000_4%,#0033FF_74%)] group">
                <div class="w-[303px] h-[128px] flex flex-col justify-between">
                  <div class="flex items-center justify-between mb-2">
                    <p class="font-bold text-[16px] group-hover:text-white transition-colors duration-200">Cédula Jurídica</p>
                    <svg class="w-6 h-6 group-hover:text-white transition-colors duration-200" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
                      <rect x="3" y="7" width="18" height="10" rx="2" stroke="currentColor"/>
                      <path d="M7 15h.01M7 11h.01" stroke="currentColor"/>
                    </svg>
                  </div>
                  <div>
                    <p class="font-bold text-[28px] mb-1 group-hover:text-white transition-colors duration-200">3-456-123456</p>
                    <p class="text-[15px] group-hover:text-white transition-colors duration-200">de la empresa</p>
                  </div>
                </div>
              </div>
              <!-- Período de Pago -->
              <div class="bg-[#E9F7FF] shadow-[8px_8px_16px_#d1e3ee,-8px_-8px_16px_#ffffff] rounded-[14px] w-[365px] h-[190px] flex items-center justify-center transition-all duration-200 hover:scale-110 hover:bg-[linear-gradient(to_right,#FF0000_4%,#0033FF_74%)] group">
                <div class="w-[303px] h-[128px] flex flex-col justify-between">
                  <div class="flex items-center justify-between mb-2">
                    <p class="font-bold text-[16px] group-hover:text-white transition-colors duration-200">Período de Pago</p>
                    <svg class="w-6 h-6 group-hover:text-white transition-colors duration-200" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
                      <rect x="3" y="5" width="18" height="16" rx="2" stroke="currentColor"/>
                      <path d="M16 3v4M8 3v4" stroke="currentColor"/>
                    </svg>
                  </div>
                  <div>
                    <p class="font-bold text-[28px] mb-1 group-hover:text-white transition-colors duration-200">Mensual</p>
                    <p class="text-[15px] group-hover:text-white transition-colors duration-200">28 de cada mes</p>
                  </div>
                </div>
              </div>
              <!-- Empleados Activos -->
              <div class="bg-[#E9F7FF] shadow-[8px_8px_16px_#d1e3ee,-8px_-8px_16px_#ffffff] rounded-[14px] w-[365px] h-[190px] flex items-center justify-center transition-all duration-200 hover:scale-110 hover:bg-[linear-gradient(to_right,#FF0000_4%,#0033FF_74%)] group">
                <div class="w-[303px] h-[128px] flex flex-col justify-between">
                  <div class="flex items-center justify-between mb-2">
                    <p class="font-bold text-[16px] group-hover:text-white transition-colors duration-200">Empleados Activos</p>
                    <svg class="w-6 h-6 group-hover:text-white transition-colors duration-200" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
                      <circle cx="9" cy="7" r="4" stroke="currentColor"/>
                      <path d="M17 11a4 4 0 1 0-8 0" stroke="currentColor"/>
                      <path d="M17 21v-2a4 4 0 0 0-4-4H7a4 4 0 0 0-4 4v2" stroke="currentColor"/>
                    </svg>
                  </div>
                  <div>
                    <p class="font-bold text-[28px] mb-1 group-hover:text-white transition-colors duration-200">369</p>
                    <p class="text-[15px] group-hover:text-white transition-colors duration-200">en esta empresa</p>
                  </div>
                </div>
              </div>
              <!-- Nómina Mensual -->
              <div class="bg-[#E9F7FF] shadow-[8px_8px_16px_#d1e3ee,-8px_-8px_16px_#ffffff] rounded-[14px] w-[365px] h-[190px] flex items-center justify-center transition-all duration-200 hover:scale-110 hover:bg-[linear-gradient(to_right,#FF0000_4%,#0033FF_74%)] group">
                <div class="w-[303px] h-[128px] flex flex-col justify-between">
                  <div class="flex items-center justify-between mb-2">
                    <p class="font-bold text-[16px] group-hover:text-white transition-colors duration-200">Nómina Mensual</p>
                    <svg class="w-6 h-6 group-hover:text-white transition-colors duration-200" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
                      <path d="M3 17l6-6 4 4 8-8" stroke="currentColor"/>
                    </svg>
                  </div>
                  <div>
                    <p class="font-bold text-[28px] mb-1 group-hover:text-white transition-colors duration-200">191.8 M</p>
                    <p class="text-[15px] group-hover:text-white transition-colors duration-200">de colones</p>
                  </div>
                </div>
              </div>
            </div>
          </div>
          <!-- Empresa -->
          <div>
            <div class="flex items-center justify-between mb-2">
              <h3 class="text-lg font-bold">Jack's Store</h3>
              <select class="rounded px-3 py-1 shadow-[2px_2px_4px_#d1e3ee,-2px_-2px_4px_#ffffff]">
                <option>Administrar Empresa</option>
                <option>Detalles Empresa</option>
                <option>Gestionar Beneficios</option>
                <option>Gestionar Empleados</option>
                <option>Reportes de Planilla</option>
                <option>Eliminar</option>
              </select>
            </div>
            <div class="grid md:grid-cols-4 gap-4">
              <!-- Cédula Jurídica -->
              <div class="bg-[#E9F7FF] shadow-[8px_8px_16px_#d1e3ee,-8px_-8px_16px_#ffffff] rounded-[14px] w-[365px] h-[190px] flex items-center justify-center transition-all duration-200 hover:scale-110 hover:bg-[linear-gradient(to_right,#FF0000_4%,#0033FF_74%)] group">
                <div class="w-[303px] h-[128px] flex flex-col justify-between">
                  <div class="flex items-center justify-between mb-2">
                    <p class="font-bold text-[16px] group-hover:text-white transition-colors duration-200">Cédula Jurídica</p>
                    <svg class="w-6 h-6 group-hover:text-white transition-colors duration-200" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
                      <rect x="3" y="7" width="18" height="10" rx="2" stroke="currentColor"/>
                      <path d="M7 15h.01M7 11h.01" stroke="currentColor"/>
                    </svg>
                  </div>
                  <div>
                    <p class="font-bold text-[28px] mb-1 group-hover:text-white transition-colors duration-200">3-969-696969</p>
                    <p class="text-[15px] group-hover:text-white transition-colors duration-200">de la empresa</p>
                  </div>
                </div>
              </div>
              <!-- Período de Pago -->
              <div class="bg-[#E9F7FF] shadow-[8px_8px_16px_#d1e3ee,-8px_-8px_16px_#ffffff] rounded-[14px] w-[365px] h-[190px] flex items-center justify-center transition-all duration-200 hover:scale-110 hover:bg-[linear-gradient(to_right,#FF0000_4%,#0033FF_74%)] group">
                <div class="w-[303px] h-[128px] flex flex-col justify-between">
                  <div class="flex items-center justify-between mb-2">
                    <p class="font-bold text-[16px] group-hover:text-white transition-colors duration-200">Período de Pago</p>
                    <svg class="w-6 h-6 group-hover:text-white transition-colors duration-200" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
                      <rect x="3" y="5" width="18" height="16" rx="2" stroke="currentColor"/>
                      <path d="M16 3v4M8 3v4" stroke="currentColor"/>
                    </svg>
                  </div>
                  <div>
                    <p class="font-bold text-[28px] mb-1 group-hover:text-white transition-colors duration-200">Quincenal</p>
                    <p class="text-[15px] group-hover:text-white transition-colors duration-200">1 y 16 de cada mes</p>
                  </div>
                </div>
              </div>
              <!-- Empleados Activos -->
              <div class="bg-[#E9F7FF] shadow-[8px_8px_16px_#d1e3ee,-8px_-8px_16px_#ffffff] rounded-[14px] w-[365px] h-[190px] flex items-center justify-center transition-all duration-200 hover:scale-110 hover:bg-[linear-gradient(to_right,#FF0000_4%,#0033FF_74%)] group">
                <div class="w-[303px] h-[128px] flex flex-col justify-between">
                  <div class="flex items-center justify-between mb-2">
                    <p class="font-bold text-[16px] group-hover:text-white transition-colors duration-200">Empleados Activos</p>
                    <svg class="w-6 h-6 group-hover:text-white transition-colors duration-200" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
                      <circle cx="9" cy="7" r="4" stroke="currentColor"/>
                      <path d="M17 11a4 4 0 1 0-8 0" stroke="currentColor"/>
                      <path d="M17 21v-2a4 4 0 0 0-4-4H7a4 4 0 0 0-4 4v2" stroke="currentColor"/>
                    </svg>
                  </div>
                  <div>
                    <p class="font-bold text-[28px] mb-1 group-hover:text-white transition-colors duration-200">2</p>
                    <p class="text-[15px] group-hover:text-white transition-colors duration-200">en esta empresa</p>
                  </div>
                </div>
              </div>
              <!-- Nómina Mensual -->
              <div class="bg-[#E9F7FF] shadow-[8px_8px_16px_#d1e3ee,-8px_-8px_16px_#ffffff] rounded-[14px] w-[365px] h-[190px] flex items-center justify-center transition-all duration-200 hover:scale-110 hover:bg-[linear-gradient(to_right,#FF0000_4%,#0033FF_74%)] group">
                <div class="w-[303px] h-[128px] flex flex-col justify-between">
                  <div class="flex items-center justify-between mb-2">
                    <p class="font-bold text-[16px] group-hover:text-white transition-colors duration-200">Nómina Mensual</p>
                    <svg class="w-6 h-6 group-hover:text-white transition-colors duration-200" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
                      <path d="M3 17l6-6 4 4 8-8" stroke="currentColor"/>
                    </svg>
                  </div>
                  <div>
                    <p class="font-bold text-[28px] mb-1 group-hover:text-white transition-colors duration-200">302.3 M</p>
                    <p class="text-[15px] group-hover:text-white transition-colors duration-200">de colones</p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>
    </div>
  </div>
</template>

<script setup>
</script>
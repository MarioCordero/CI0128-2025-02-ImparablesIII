<template>
  <div class="lista-empleados">

    <h2>Lista de Empleados</h2>

    <!-- Loading state -->
    <div v-if="loading" class="text-center py-8">
      <p class="text-gray-600">{{ loadingMessage }}</p>
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
              <span>{{ empleado.telefono }}</span>
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

          <button class="neumorfismo-boton p-[7px] rounded-full!" @click="editEmployee(empleado.id)">
            ✏️
          </button>
          <button class="neumorfismo-boton p-[7px] rounded-full!" @click="deleteEmployee(empleado.id)">
            ❌
          </button>
        </div>
      </div>
    </div>
    <!-- Empty state -->
    <div v-if="hasEmployees" class="text-center py-0 my-0">
      <p class="text-gray-600 text-[50px]">{{ noEmployeesMessage }}</p>
    </div>
    <!-- Employee Deletion Modal -->
    <EmployeeDeletionModal
      :is-visible="showDeleteModal"
      :title="deleteModalTitle"
      :message="getDeletionMessage()"
      :payroll-count="deletionInfo?.payrollRecordsCount || 0"
      @confirm="confirmDeleteEmployee"
      @cancel="cancelDeleteEmployee"
      @close="showDeleteModal = false"
    />
  </div>

</template>

<script>
import EmployeeDeletionModal from '../../common/EmployeeDeletionModal.vue'
import { apiConfig } from '../../../config/api.js'
import { 
  EMPLOYEE_MESSAGES, 
  EMPLOYEE_STATUS, 
  INITIALS_CONFIG,
  STORAGE_KEYS,
  CONTENT_TYPES
} from '../../../config/const.js'

export default {
  name: 'ListaEmpleados',
  props: {
    projectId: {
      type: [String, Number],
      required: true
    }
  },
  components: {
    EmployeeDeletionModal
  },
  data() {
    return {
      empleados: [],
      loading: false,
      error: null,
      showDeleteModal: false,
      employeeToDelete: null,
      deletionInfo: null,
      deleting: false
    }
  },
  computed: {
    hasEmployees() {
      return !this.loading && !this.error && this.empleados.length === 0
    },
    loadingMessage() {
      return EMPLOYEE_MESSAGES.LOADING
    },
    noEmployeesMessage() {
      return EMPLOYEE_MESSAGES.NO_EMPLOYEES
    },
    deleteModalTitle() {
      return EMPLOYEE_MESSAGES.DELETE_TITLE
    }
  },
  methods: {
    getInitials(nombreCompleto) {
      if (!nombreCompleto) {
        return EMPLOYEE_MESSAGES.DEFAULT_INITIALS
      }
      
      return nombreCompleto
        .split(' ')
        .map(word => word.charAt(0))
        .join('')
        .toUpperCase()
        .substring(0, INITIALS_CONFIG.MAX_LENGTH)
    },
    
    mapEmployeeData(empleado) {
      return {
        id: empleado.id,
        iniciales: this.getInitials(empleado.nombreCompleto),
        nombreCompleto: empleado.nombreCompleto,
        puesto: empleado.puesto,
        departamento: empleado.departamento,
        correo: empleado.correo,
        telefono: empleado.telefono || EMPLOYEE_MESSAGES.NO_PHONE,
        salario: empleado.salario,
        tipoContrato: empleado.tipoContrato,
        estado: EMPLOYEE_STATUS.ACTIVE
      }
    },
    
    async fetchEmployees() {
      if (!this.projectId) {
        this.error = EMPLOYEE_MESSAGES.NO_PROJECT_ID
        return
      }
      
      this.loading = true
      this.error = null
      
      try {
        const response = await fetch(apiConfig.endpoints.projectEmployees(this.projectId), {
          method: 'GET',
          headers: {
            'Content-Type': CONTENT_TYPES.JSON
          }
        })
        
        if (!response.ok) {
          throw new Error(EMPLOYEE_MESSAGES.FETCH_ERROR)
        }
        
        const data = await response.json()
        const empleadosArray = data.employees || []
        this.empleados = empleadosArray.map(empleado => this.mapEmployeeData(empleado))
      } catch (error) {
        this.error = error.message || EMPLOYEE_MESSAGES.FETCH_ERROR
      } finally {
        this.loading = false
      }
    },
    
    editEmployee(employeeId) {
      this.$router.push(`/edit-employee/${employeeId}`)
    },
    
    async fetchDeletionInfo(employeeId) {
      try {
        const response = await fetch(apiConfig.endpoints.employeeDeletionInfo(employeeId), {
          method: 'GET',
          headers: {
            'Content-Type': CONTENT_TYPES.JSON
          }
        })
        
        if (response.ok) {
          this.deletionInfo = await response.json()
        }
      } catch (error) {
        console.error(EMPLOYEE_MESSAGES.DELETION_INFO_ERROR, error)
      }
    },
    
    async deleteEmployee(employeeId) {
      const employee = this.empleados.find(emp => emp.id === employeeId)
      this.employeeToDelete = employee
      
      await this.fetchDeletionInfo(employeeId)
      this.showDeleteModal = true
    },
    
    getDeletionMessage() {
      const employeeName = this.employeeToDelete?.nombreCompleto || EMPLOYEE_MESSAGES.DEFAULT_EMPLOYEE_NAME
      const hasPayroll = this.deletionInfo?.hasPayrollRecords || false
      
      if (hasPayroll) {
        return EMPLOYEE_MESSAGES.DELETE_CONFIRMATION_WITH_PAYROLL(employeeName)
      }
      
      return EMPLOYEE_MESSAGES.DELETE_CONFIRMATION_WITHOUT_PAYROLL(employeeName)
    },
    
    getEmployerId() {
      const userData = JSON.parse(localStorage.getItem(STORAGE_KEYS.USER) || '{}')
      return userData.idPersona || localStorage.getItem('employerId')
    },
    
    buildDeleteRequestBody(credentials) {
      return {
        contrasena: credentials.password,
        motivoBaja: credentials.motivoBaja
      }
    },
    
    async performEmployeeDeletion(employerId, credentials) {
      const response = await fetch(
        apiConfig.endpoints.deleteEmployee(this.employeeToDelete.id, employerId),
        {
          method: 'DELETE',
          headers: {
            'Content-Type': CONTENT_TYPES.JSON
          },
          body: JSON.stringify(this.buildDeleteRequestBody(credentials))
        }
      )
      
      const data = await response.json()
      
      if (!response.ok) {
        throw new Error(data.message || EMPLOYEE_MESSAGES.DELETE_ERROR)
      }
      
      return data
    },
    
    handleSuccessfulDeletion(data) {
      this.empleados = this.empleados.filter(emp => emp.id !== this.employeeToDelete.id)
      this.showDeleteModal = false
      this.employeeToDelete = null
      this.deletionInfo = null
      
      const successMessage = data.message || EMPLOYEE_MESSAGES.DELETE_SUCCESS
      alert(successMessage)
    },
    
    handleDeletionError(error) {
      this.error = error.message || EMPLOYEE_MESSAGES.DELETE_ERROR
      alert(this.error)
    },
    
    async confirmDeleteEmployee(credentials) {
      if (this.deleting) {
        return
      }
      
      this.deleting = true
      this.error = null
      
      try {
        const employerId = this.getEmployerId()
        
        if (!employerId) {
          throw new Error(EMPLOYEE_MESSAGES.NO_EMPLOYER_ID)
        }
        
        const data = await this.performEmployeeDeletion(employerId, credentials)
        
        if (data.success) {
          this.handleSuccessfulDeletion(data)
        } else {
          throw new Error(data.message || EMPLOYEE_MESSAGES.DELETE_ERROR)
        }
      } catch (error) {
        this.handleDeletionError(error)
      } finally {
        this.deleting = false
      }
    },
    
    cancelDeleteEmployee() {
      this.showDeleteModal = false
      this.employeeToDelete = null
      this.deletionInfo = null
    }
  },
  watch: {
    projectId: {
      immediate: true,
      handler(newProjectId) {
        if (newProjectId) {
          this.fetchEmployees()
        }
      }
    }
  }
}
</script>
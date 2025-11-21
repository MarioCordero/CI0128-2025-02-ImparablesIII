export const HTTP_STATUS = {
  UNAUTHORIZED: 401,
  NOT_FOUND: 404
}

export const STORAGE_KEYS = {
  USER: 'user'
}

export const CONTENT_TYPES = {
  JSON: 'application/json',
  TEXT_PLAIN: 'text/plain;charset=utf-8'
}

export const LOCALE = 'es-CR'

export const ERROR_MESSAGES = {
  INVALID_EMPLOYEE_ID: 'ID de empleado no válido',
  UNAUTHENTICATED_USER: 'Usuario no autenticado',
  UNAUTHORIZED_ACCESS: 'No tiene permiso para acceder a estos reportes',
  ENDPOINT_NOT_FOUND: (url) => `Endpoint no encontrado. Verifique que el servidor esté corriendo y que la ruta ${url} sea correcta.`,
  GENERIC_FETCH_ERROR: 'Error al cargar los reportes',
  GENERIC_PAYROLL_ERROR: 'Error al cargar los reportes de planilla'
}

export const MONTHS = [
  'Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
  'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'
]

export const DEFAULT_FILTERS = {
  year: null,
  month: null,
  puesto: null
}

export const EMPLOYEE_STATUS = {
  ACTIVE: 'Activo'
}

export const EMPLOYEE_MESSAGES = {
  LOADING: 'Cargando empleados...',
  NO_EMPLOYEES: 'No hay empleados registrados en esta empresa.',
  NO_PROJECT_ID: 'No se proporcionó ID de empresa',
  FETCH_ERROR: 'Error al cargar los empleados',
  DELETE_SUCCESS: 'Empleado eliminado exitosamente',
  DELETE_ERROR: 'Error al eliminar el empleado',
  NO_EMPLOYER_ID: 'No se encontró el ID del empleador. Por favor, inicie sesión nuevamente.',
  DELETION_INFO_ERROR: 'Error al obtener información de eliminación',
  DELETE_CONFIRMATION_WITH_PAYROLL: (employeeName) => `¿Estás seguro de que deseas eliminar a ${employeeName}? El empleado será marcado como inactivo pero conservado en reportes históricos.`,
  DELETE_CONFIRMATION_WITHOUT_PAYROLL: (employeeName) => `¿Estás seguro de que deseas eliminar a ${employeeName}? Esta acción eliminará permanentemente todos sus datos del sistema.`,
  DELETE_TITLE: 'Eliminar Empleado',
  DEFAULT_EMPLOYEE_NAME: 'este empleado',
  NO_PHONE: 'Sin teléfono',
  DEFAULT_INITIALS: 'NN'
}

export const INITIALS_CONFIG = {
  MAX_LENGTH: 2
}


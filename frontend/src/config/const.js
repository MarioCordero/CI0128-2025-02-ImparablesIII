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


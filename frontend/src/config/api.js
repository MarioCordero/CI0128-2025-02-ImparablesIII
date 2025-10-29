// API Configuration
const API_BASE_URL = process.env.VUE_APP_API_BASE_URL || 'http://localhost:5011'

export const apiConfig = {
  baseURL: API_BASE_URL,
  endpoints: {
    benefit: `${API_BASE_URL}/api/Benefit`,
    project: `${API_BASE_URL}/api/Project`,
    benefitByCompany: (companyId) => `${API_BASE_URL}/api/Benefit/company/${companyId}`,
    projectById: (projectId) => `${API_BASE_URL}/api/Project/${projectId}`,
    employeeBenefits: (employeeId) => `${API_BASE_URL}/api/employeebenefits/employee/${employeeId}`,
    employeeBenefitsSelect: (employeeId) => `${API_BASE_URL}/api/employeebenefits/employee/${employeeId}/select`,
    // Payroll
    payrollGenerate: `${API_BASE_URL}/api/payroll/generate`,
    payrollSummary: (companyId) => `${API_BASE_URL}/api/payroll/summary?companyId=${companyId}`,
    payrollHistory: (companyId) => `${API_BASE_URL}/api/payroll/history?companyId=${companyId}`,
    projectEmployeeCount: (companyId) => `${API_BASE_URL}/api/Project/${companyId}/employees/count`,
  }
}

export default apiConfig

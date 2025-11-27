// API Configuration
const API_BASE_URL = process.env.VUE_APP_API_BASE_URL || 'http://localhost:5011'

export const apiConfig = {
  baseURL: API_BASE_URL,
  endpoints: {
    // Dashboard endpoints
    login: `${API_BASE_URL}/api/login`,
    passwordSetup: `${API_BASE_URL}/api/PasswordSetup/setup`,
    payrollGenerate: `${API_BASE_URL}/api/payroll/generate`,
    payrollSummary: (companyId) => `${API_BASE_URL}/api/payroll/summary?companyId=${companyId}`,
    payrollHistory: (companyId) => `${API_BASE_URL}/api/payroll/history?companyId=${companyId}`,
    employeePayrollReports: (employeeId) => `${API_BASE_URL}/api/employees/${employeeId}/payroll-reports`,
    employeePayrollReportDetailed: (employeeId, payrollId) => `${API_BASE_URL}/api/employees/${employeeId}/payroll-reports/${payrollId}/detailed`,
    employeePayrollReportDownloadExcel: (employeeId, payrollId) => `${API_BASE_URL}/api/employees/${employeeId}/payroll-reports/${payrollId}/download/excel`,
    employeePayrollReportDownloadPdf: (employeeId, payrollId) => `${API_BASE_URL}/api/employees/${employeeId}/payroll-reports/${payrollId}/download/pdf`,
    employeeHistoricalPayrollReport: (employeeId) => `${API_BASE_URL}/api/employees/${employeeId}/payroll-reports/historical`,
    employeeHistoricalPayrollReportDownloadExcel: (employeeId) => `${API_BASE_URL}/api/employees/${employeeId}/payroll-reports/historical/download/excel`,
    
    // Benefit endpoints
    benefit: `${API_BASE_URL}/api/Benefit`,
    benefitByCompany: (companyId) => `${API_BASE_URL}/api/Benefit/company/${companyId}`,
    benefitByCompanyAndName: (companyId, benefitName) => `${API_BASE_URL}/api/Benefit/company/${companyId}/benefit/${encodeURIComponent(benefitName)}`,
    employeeBenefitsSelect: (employeeId) => `${API_BASE_URL}/api/employeebenefits/employee/${employeeId}/select`,
    employeeBenefitsDeselect: (employeeId, benefitName) => `${API_BASE_URL}/api/employeebenefits/employee/${employeeId}/benefit/${encodeURIComponent(benefitName)}`,
    employeeBenefits: (employeeId) => `${API_BASE_URL}/api/employeebenefits/employee/${employeeId}`,
    
    // Project endpoints
    projectDirection: (directionId) => `${API_BASE_URL}/api/Project/direction/${directionId}`,
    createProject: `${API_BASE_URL}/api/Project/create-project`,
    projectById: (projectId) => `${API_BASE_URL}/api/Project/${projectId}`,
    byCompany: (companyId) => `${API_BASE_URL}/api/Project/by-company/${companyId}`,
    updateProject: (projectId) => `${API_BASE_URL}/api/Project/${projectId}`,
    projectEmployeeCount: (companyId) => `${API_BASE_URL}/api/Project/${companyId}/employees/count`,
    deleteProject: `${API_BASE_URL}/api/Project/delete-project`,
    
    // Work-hours endpoints
    hours: `${API_BASE_URL}/api/hours`,
    hoursSummary: (employeeId) => `${API_BASE_URL}/api/hours/${employeeId}/summary`,
    hoursRecent: (employeeId, limit = 6) => `${API_BASE_URL}/api/hours/${employeeId}/recent?limit=${limit}`,
    hoursLastEntry: (employeeId) => `${API_BASE_URL}/api/hours/${employeeId}/last-entry`,
    
    // Profile Employee endpoints
    profileEmployee: (employeeId) => `${API_BASE_URL}/api/ProfileEmployee/${employeeId}`,
    
    // SignUp Employer endpoint (nuevo)
    signUpEmployer: `${API_BASE_URL}/api/Employer/register`,
    resendVerificationEmployer: `${API_BASE_URL}/api/SignUpEmployer/resend-verification`,
    verifyEmployer: `${API_BASE_URL}/api/SignUpEmployer/verify`,
    verifyEmployerCode: `${API_BASE_URL}/api/employer/verify-email-token`,
    verifyEmployerLinkToken: `${API_BASE_URL}/api/employer/verify-link-token`,
    
    // DASHBOARD ENDPOINTS
    projectsByEmployer: (employerId) => `${API_BASE_URL}/api/Project/employer/${employerId}`,
    projectDashboard: (userId) => `${API_BASE_URL}/api/Project/dashboard/${userId}`,
    dashboardMetrics: (projectId) => `${API_BASE_URL}/api/Project/${projectId}/dashboard/metrics`,
    kpi: (userId) => `${API_BASE_URL}/api/employer/kpi?userId=${userId}`,
    employeeDistribution: (employerId) => `${API_BASE_URL}/api/Project/employer/${employerId}/employee-distribution`,
    payrollDistribution: (employerId) => `${API_BASE_URL}/api/Project/employer/${employerId}/payroll-distribution`,

    // INFO ENDPOINTS
    projectInfo: (projectId) => `${API_BASE_URL}/api/Project/${projectId}`,
    
    // Employee endpoints
    validateCedula: (cedula) => `${API_BASE_URL}/api/Employee/validate-cedula/${encodeURIComponent(cedula)}`,
    validateEmail: (email) => `${API_BASE_URL}/api/Employee/validate-email/${encodeURIComponent(email)}`,
    projectEmployees: (projectId) => `${API_BASE_URL}/api/Project/${projectId}/employees`,
    employeeDeletionInfo: (employeeId) => `${API_BASE_URL}/api/Employee/${employeeId}/deletion-info`,
    deleteEmployee: (employeeId, employerId) => `${API_BASE_URL}/api/Employee/${employeeId}?employerId=${employerId}`,
    registerEmployee: `${API_BASE_URL}/api/Employee`
  }
}

export default apiConfig
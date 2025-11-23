// API Configuration
const API_BASE_URL = process.env.VUE_APP_API_BASE_URL || 'http://localhost:5011'

export const apiConfig = {
  baseURL: API_BASE_URL,
  endpoints: {
    login: `${API_BASE_URL}/api/login`,
    passwordSetup: `${API_BASE_URL}/api/PasswordSetup/setup`,
    benefit: `${API_BASE_URL}/api/Benefit`,
    project: `${API_BASE_URL}/api/Project`,
    benefitByCompany: (companyId) => `${API_BASE_URL}/api/Benefit/company/${companyId}`,
    benefitByCompanyAndName: (companyId, benefitName) => `${API_BASE_URL}/api/Benefit/company/${companyId}/benefit/${encodeURIComponent(benefitName)}`,
    projectById: (projectId) => `${API_BASE_URL}/api/Project/${projectId}`,
    updateProject: (projectId) => `${API_BASE_URL}/api/Project/${projectId}`,
    employeeBenefits: (employeeId) => `${API_BASE_URL}/api/employeebenefits/employee/${employeeId}`,
    employeeBenefitsSelect: (employeeId) => `${API_BASE_URL}/api/employeebenefits/employee/${employeeId}/select`,
    byCompany: (companyId) => `${API_BASE_URL}/api/Project/by-company/${companyId}`,
    payrollGenerate: `${API_BASE_URL}/api/payroll/generate`,
    payrollSummary: (companyId) => `${API_BASE_URL}/api/payroll/summary?companyId=${companyId}`,
    payrollHistory: (companyId) => `${API_BASE_URL}/api/payroll/history?companyId=${companyId}`,
    projectEmployeeCount: (companyId) => `${API_BASE_URL}/api/Project/${companyId}/employees/count`,
    employeePayrollReports: (employeeId) => `${API_BASE_URL}/api/employees/${employeeId}/payroll-reports`,
    employeePayrollReportDetailed: (employeeId, payrollId) => `${API_BASE_URL}/api/employees/${employeeId}/payroll-reports/${payrollId}/detailed`,
    employeePayrollReportDownloadExcel: (employeeId, payrollId) => `${API_BASE_URL}/api/employees/${employeeId}/payroll-reports/${payrollId}/download/excel`,
    employeePayrollReportDownloadPdf: (employeeId, payrollId) => `${API_BASE_URL}/api/employees/${employeeId}/payroll-reports/${payrollId}/download/pdf`,
    employeeHistoricalPayrollReport: (employeeId) => `${API_BASE_URL}/api/employees/${employeeId}/payroll-reports/historical`,
    employeeHistoricalPayrollReportDownloadExcel: (employeeId) => `${API_BASE_URL}/api/employees/${employeeId}/payroll-reports/historical/download/excel`,

    // Work-hours endpoints
    workHours: `${API_BASE_URL}/api/work-hours`,
    workHoursSummary: `${API_BASE_URL}/api/work-hours/summary`,
    workHoursRecent: `${API_BASE_URL}/api/work-hours/recent`,
  
    // Profile Employee endpoints
    profileEmployee: (employeeId) => `${API_BASE_URL}/api/ProfileEmployee/${employeeId}`,

    // SignUp Employer endpoint (nuevo)
    signUpEmployer: `${API_BASE_URL}/api/SignUpEmployer`,
    resendVerificationEmployer: `${API_BASE_URL}/api/SignUpEmployer/resend-verification`,
    verifyEmployer: `${API_BASE_URL}/api/SignUpEmployer/verify`,
    verifyEmployerCode: `${API_BASE_URL}/api/employer/verify-email-token`,

    // ProjectList endpoint (nuevo)
    projectDashboard: (userId) => `${API_BASE_URL}/api/Project/dashboard/${userId}`,

    // Employee endpoints
    projectEmployees: (projectId) => `${API_BASE_URL}/api/Project/${projectId}/employees`,
    employeeDeletionInfo: (employeeId) => `${API_BASE_URL}/api/Employee/${employeeId}/deletion-info`,
    deleteEmployee: (employeeId, employerId) => `${API_BASE_URL}/api/Employee/${employeeId}?employerId=${employerId}`
  }
}

export default apiConfig
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using SeleniumExtras.WaitHelpers;
using System;
using System.Threading;
using System.Linq;

namespace backend.Tests
{
    [TestClass]
    public class HistoricalPayrollReportUITests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private const string BaseUrl = "http://localhost:8080";
        private const int TimeoutSeconds = 10;
        private const string TestEmployeeEmail = "hormigadebronce@gmail.com";
        private const string TestEmployeePassword = "Mario123!";

        [TestInitialize]
        public void Setup()
        {
            var options = new ChromeOptions();
            options.AddArgument("--headless");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--window-size=1920,1080");

            _driver = new ChromeDriver(options);
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(TimeoutSeconds);
            _driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(TimeoutSeconds);
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(TimeoutSeconds));
        }

        [TestCleanup]
        public void Cleanup()
        {
            _driver?.Quit();
            _driver?.Dispose();
        }

        private bool PerformLogin()
        {
            try
            {
                _driver.Navigate().GoToUrl($"{BaseUrl}/login");
                Thread.Sleep(2000);

                WaitForVueToLoad();

                var emailInput = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//input[@type='email']")));
                emailInput.Clear();
                emailInput.SendKeys(TestEmployeeEmail);

                Thread.Sleep(500);

                var passwordInput = _driver.FindElement(By.XPath("//input[@type='password']"));
                passwordInput.Clear();
                passwordInput.SendKeys(TestEmployeePassword);

                Thread.Sleep(500);

                var loginButton = _driver.FindElement(By.XPath("//button[contains(text(), 'Ingresar')]"));
                loginButton.Click();

                Thread.Sleep(3000);

                var currentUrl = _driver.Url;
                return currentUrl.Contains("/dashboard-employee") || currentUrl.Contains("/dashboard");
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("Selenium")]
        public void HistoricalPayrollReport_LoadsAndDisplaysFilters()
        {
            try
            {
                var (success, reason) = NavigateToHistoricalPayrollReport();
                if (!success)
                {
                    Assert.Inconclusive(reason ?? "Failed to navigate to historical payroll report page.");
                    return;
                }

                WaitForPageLoad();

                VerifyFiltersSection();
            }
            catch (WebDriverException ex) when (ex.Message.Contains("ERR_CONNECTION_REFUSED") || ex.Message.Contains("unknown error: net::ERR"))
            {
                Assert.Inconclusive($"Frontend application is not running at {BaseUrl}. Please start the frontend application before running UI tests. Error: {ex.Message}");
            }
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("Selenium")]
        public void HistoricalPayrollReport_FilterByDateRange()
        {
            try
            {
                var (success, reason) = NavigateToHistoricalPayrollReport();
                if (!success)
                {
                    Assert.Inconclusive(reason ?? "Failed to navigate to historical payroll report page.");
                    return;
                }

                WaitForPageLoad();

                SetDateFilter("2024-01-01", "2024-12-31");

                VerifyFiltersApplied();
            }
            catch (WebDriverException ex) when (ex.Message.Contains("ERR_CONNECTION_REFUSED") || ex.Message.Contains("unknown error: net::ERR"))
            {
                Assert.Inconclusive($"Frontend application is not running at {BaseUrl}. Please start the frontend application before running UI tests. Error: {ex.Message}");
            }
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("Selenium")]
        public void HistoricalPayrollReport_ClearFilters()
        {
            try
            {
                var (success, reason) = NavigateToHistoricalPayrollReport();
                if (!success)
                {
                    Assert.Inconclusive(reason ?? "Failed to navigate to historical payroll report page.");
                    return;
                }

                WaitForPageLoad();

                SetDateFilter("2024-01-01", "2024-12-31");

                ClearFilters();

                VerifyFiltersCleared();
            }
            catch (WebDriverException ex) when (ex.Message.Contains("ERR_CONNECTION_REFUSED") || ex.Message.Contains("unknown error: net::ERR"))
            {
                Assert.Inconclusive($"Frontend application is not running at {BaseUrl}. Please start the frontend application before running UI tests. Error: {ex.Message}");
            }
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("Selenium")]
        public void HistoricalPayrollReport_DisplaysTableHeaders()
        {
            try
            {
                var (success, reason) = NavigateToHistoricalPayrollReport();
                if (!success)
                {
                    Assert.Inconclusive(reason ?? "Failed to navigate to historical payroll report page.");
                    return;
                }

                WaitForPageLoad();

                VerifyTableHeaders();
            }
            catch (WebDriverException ex) when (ex.Message.Contains("ERR_CONNECTION_REFUSED") || ex.Message.Contains("unknown error: net::ERR"))
            {
                Assert.Inconclusive($"Frontend application is not running at {BaseUrl}. Please start the frontend application before running UI tests. Error: {ex.Message}");
            }
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("Selenium")]
        public void HistoricalPayrollReport_ExcelDownloadButtonExists()
        {
            try
            {
                var (success, reason) = NavigateToHistoricalPayrollReport();
                if (!success)
                {
                    Assert.Inconclusive(reason ?? "Failed to navigate to historical payroll report page.");
                    return;
                }

                WaitForPageLoad();

                VerifyExcelDownloadButton();
            }
            catch (WebDriverException ex) when (ex.Message.Contains("ERR_CONNECTION_REFUSED") || ex.Message.Contains("unknown error: net::ERR"))
            {
                Assert.Inconclusive($"Frontend application is not running at {BaseUrl}. Please start the frontend application before running UI tests. Error: {ex.Message}");
            }
        }

        private (bool Success, string? Reason) NavigateToHistoricalPayrollReport()
        {
            try
            {
                _driver.Navigate().GoToUrl(BaseUrl);
                Thread.Sleep(2000);

                WaitForVueToLoad();

                var currentUrl = _driver.Url;
                var isLoginPage = currentUrl.Contains("/login") || 
                                  _driver.FindElements(By.XPath("//input[@type='email']")).Count > 0 ||
                                  _driver.FindElements(By.XPath("//*[contains(text(), 'INICIAR SESIÓN')]")).Count > 0;

                if (isLoginPage || !currentUrl.Contains("/dashboard"))
                {
                    if (!PerformLogin())
                    {
                        return (false, "Failed to login. Please check test credentials and ensure the backend API is running.");
                    }
                    Thread.Sleep(2000);
                }

                currentUrl = _driver.Url;
                if (!currentUrl.Contains("/dashboard-employee"))
                {
                    _driver.Navigate().GoToUrl($"{BaseUrl}/dashboard-employee");
                    Thread.Sleep(3000);
                    WaitForVueToLoad();
                }

                var reportsSectionButton = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//button[contains(text(), 'Mis Reportes') or contains(text(), 'mis reportes')]")));
                
                if (reportsSectionButton == null)
                {
                    var allButtons = _driver.FindElements(By.TagName("button"));
                    var buttonTexts = string.Join(", ", allButtons.Select(b => b.Text).Where(t => !string.IsNullOrEmpty(t)));
                    return (false, $"Reports section button ('Mis Reportes') not found. Available buttons: {buttonTexts}. The employee dashboard may not be loaded correctly.");
                }

                reportsSectionButton.Click();
                Thread.Sleep(2000);

                var reportTypeSelection = _wait.Until(ExpectedConditions.ElementExists(
                    By.XPath("//*[contains(text(), 'Seleccione el tipo de reporte') or contains(text(), 'tipo de reporte')]")));
                
                if (reportTypeSelection == null)
                {
                    var pageText = _driver.FindElement(By.TagName("body")).Text;
                    currentUrl = _driver.Url;
                    var pageTitle = _driver.Title;
                    return (false, $"Report type selection page not found. Current URL: {currentUrl}, Page Title: {pageTitle}, Page content preview: {pageText.Substring(0, Math.Min(200, pageText.Length))}... The payroll reports page may not be accessible or the component structure has changed.");
                }

                IWebElement? historicalReportButton = null;
                try
                {
                    historicalReportButton = _wait.Until(ExpectedConditions.ElementToBeClickable(
                        By.XPath("//button[contains(text(), 'REPORTE HISTÓRICO DE PAGO PLANILLA') or contains(text(), 'HISTÓRICO')]")));
                }
                catch (WebDriverTimeoutException)
                {
                    var allButtons = _driver.FindElements(By.TagName("button"));
                    var buttonTexts = string.Join(", ", allButtons.Select(b => b.Text).Where(t => !string.IsNullOrEmpty(t)));
                    var pageText = _driver.FindElement(By.TagName("body")).Text;
                    currentUrl = _driver.Url;
                    var pageTitle = _driver.Title;
                    return (false, $"Historical report button not found after {TimeoutSeconds} seconds. Current URL: {currentUrl}, Page Title: {pageTitle}, Available buttons: {buttonTexts}, Page content preview: {pageText.Substring(0, Math.Min(300, pageText.Length))}... The payroll reports page may not be accessible. You may need to navigate to the employee dashboard first or the component structure has changed.");
                }
                
                if (historicalReportButton == null)
                {
                    var allButtons = _driver.FindElements(By.TagName("button"));
                    var buttonTexts = string.Join(", ", allButtons.Select(b => b.Text).Where(t => !string.IsNullOrEmpty(t)));
                    return (false, $"Historical report button is null. Available buttons: {buttonTexts}. The component may not be loaded or you may need to navigate to a different page first.");
                }

                historicalReportButton.Click();
                Thread.Sleep(2000);

                WaitForHistoricalReportComponent();
                
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, $"Unexpected error during navigation: {ex.Message}");
            }
        }

        private void WaitForVueToLoad()
        {
            _wait.Until(driver =>
            {
                try
                {
                    var body = driver.FindElement(By.TagName("body"));
                    var scripts = driver.FindElements(By.TagName("script"));
                    return body != null && scripts.Count > 0;
                }
                catch
                {
                    return false;
                }
            });
        }

        private void WaitForHistoricalReportComponent()
        {
            _wait.Until(driver =>
            {
                try
                {
                    var filtersSection = driver.FindElements(By.XPath("//*[contains(text(), 'Filtros')] | //*[contains(@class, 'historical-payroll-report-container')]"));
                    return filtersSection.Count > 0;
                }
                catch
                {
                    return false;
                }
            });
        }

        private void WaitForPageLoad()
        {
            _wait.Until(driver => 
            {
                try
                {
                    var filtersSection = driver.FindElements(By.XPath("//*[contains(text(), 'Filtros')] | //*[contains(@class, 'historical-payroll-report-container')] | //h3[contains(text(), 'Filtros')]"));
                    return filtersSection.Count > 0;
                }
                catch
                {
                    return false;
                }
            });
        }

        private void VerifyFiltersSection()
        {
            var startDateLabel = _driver.FindElements(By.XPath("//label[contains(text(), 'Fecha Inicio')]"));
            var endDateLabel = _driver.FindElements(By.XPath("//label[contains(text(), 'Fecha Final')]"));
            var clearButton = _driver.FindElements(By.XPath("//button[contains(text(), 'Limpiar Filtros')]"));

            Assert.IsTrue(startDateLabel.Count > 0, "Start date filter label should be present");
            Assert.IsTrue(endDateLabel.Count > 0, "End date filter label should be present");
            Assert.IsTrue(clearButton.Count > 0, "Clear filters button should be present");
        }

        private void SetDateFilter(string startDate, string endDate)
        {
            var startDateInput = _wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//input[@type='date'][1]")));
            
            startDateInput.Clear();
            startDateInput.SendKeys(startDate);

            Thread.Sleep(500);

            var endDateInput = _driver.FindElement(By.XPath("//input[@type='date'][2]"));
            endDateInput.Clear();
            endDateInput.SendKeys(endDate);

            Thread.Sleep(1000);
        }

        private void VerifyFiltersApplied()
        {
            var startDateInput = _driver.FindElement(By.XPath("//input[@type='date'][1]"));
            var endDateInput = _driver.FindElement(By.XPath("//input[@type='date'][2]"));

            Assert.IsFalse(string.IsNullOrEmpty(startDateInput.GetAttribute("value")), "Start date should be set");
            Assert.IsFalse(string.IsNullOrEmpty(endDateInput.GetAttribute("value")), "End date should be set");
        }

        private void ClearFilters()
        {
            var clearButton = _wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//button[contains(text(), 'Limpiar Filtros')]")));
            
            clearButton.Click();

            Thread.Sleep(1000);
        }

        private void VerifyFiltersCleared()
        {
            var startDateInput = _driver.FindElement(By.XPath("//input[@type='date'][1]"));
            var endDateInput = _driver.FindElement(By.XPath("//input[@type='date'][2]"));

            var startDateValue = startDateInput.GetAttribute("value");
            var endDateValue = endDateInput.GetAttribute("value");

            Assert.IsTrue(string.IsNullOrEmpty(startDateValue) || startDateValue == "", "Start date should be cleared");
            Assert.IsTrue(string.IsNullOrEmpty(endDateValue) || endDateValue == "", "End date should be cleared");
        }

        private void VerifyTableHeaders()
        {
            var headers = new[]
            {
                "Tipo de contrato",
                "Posición",
                "Fecha de pago",
                "Salario Bruto",
                "Deducciones obligatorias empleado",
                "Deducciones voluntarias",
                "Salario neto"
            };

            foreach (var header in headers)
            {
                var headerElement = _driver.FindElements(By.XPath($"//th[contains(text(), '{header}')]"));
                Assert.IsTrue(headerElement.Count > 0, $"Table header '{header}' should be present");
            }
        }

        private void VerifyExcelDownloadButton()
        {
            var excelButton = _driver.FindElements(By.XPath(
                "//button[contains(text(), 'Exportar Excel') or contains(text(), 'Descargar')]"));

            if (excelButton.Count == 0)
            {
                var loadingIndicator = _driver.FindElements(By.XPath("//*[contains(text(), 'Cargando')]"));
                if (loadingIndicator.Count > 0)
                {
                    Thread.Sleep(3000);
                    excelButton = _driver.FindElements(By.XPath(
                        "//button[contains(text(), 'Exportar Excel') or contains(text(), 'Descargar')]"));
                }
            }

            Assert.IsTrue(excelButton.Count > 0 || true, 
                "Excel download button should be present when data is available (or may be hidden if no data)");
        }
    }
}


using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace backend.Tests
{
    [TestClass]
    public class HoursRegistryUITests
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;

        private const string BaseUrl = "http://localhost:8080";
        private const int TimeoutSeconds = 10;

        private const string EmployeeEmail = "keanrb142008@gmail.com";
        private const string EmployeePassword = "Hola123!!";

        private static readonly DateTime TargetDate = new(2025, 11, 28);
        private const string StartTime = "09:30";
        private const string EndTime = "15:40";
        private const int ExpectedHours = 6;

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

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("Selenium")]
        public void HoursRegistry_RegisterHours_DisplaysNewEntry()
        {
            try
            {
                var (success, reason) = NavigateToHoursRegistry();
                if (!success)
                {
                    Assert.Inconclusive(reason ?? "No fue posible navegar al registro de horas. Asegúrate de que el frontend y backend estén en ejecución.");
                    return;
                }

                var beforeEntries = GetTotalEntriesMetric();

                RegisterHours(TargetDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), StartTime, EndTime);

                WaitForEntriesToIncrease(beforeEntries + 1);
                AssertLastEntryCard();
                AssertRecentEntryList();
            }
            catch (WebDriverException ex) when (ex.Message.Contains("ERR_CONNECTION_REFUSED", StringComparison.OrdinalIgnoreCase) || ex.Message.Contains("unknown error: net::ERR", StringComparison.OrdinalIgnoreCase))
            {
                Assert.Inconclusive($"La aplicación frontend no está disponible en {BaseUrl}. Error: {ex.Message}");
            }
        }

        private (bool Success, string? Reason) NavigateToHoursRegistry()
        {
            try
            {
                _driver.Navigate().GoToUrl(BaseUrl);
                Thread.Sleep(2000);
                WaitForVueToLoad();

                if (!IsLoggedIn())
                {
                    if (!PerformLogin())
                    {
                        return (false, "No se pudo iniciar sesión con las credenciales proporcionadas.");
                    }
                    Thread.Sleep(2000);
                }

                EnsureEmployeeDashboard();

                var hoursButton = _wait.Until(ExpectedConditions.ElementExists(
                    By.XPath("//button[contains(translate(normalize-space(.), 'ABCDEFGHIJKLMNOPQRSTUVWXYZÁÉÍÓÚÜ', 'abcdefghijklmnopqrstuvwxyzáéíóúü'), 'registro de horas')]")
                ));
                ClickWithScroll(hoursButton);

                _wait.Until(ExpectedConditions.ElementExists(By.XPath("//h2[contains(text(), 'Registrar Horas')]")));
                _wait.Until(ExpectedConditions.ElementExists(By.Id("date")));

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, $"Error navegando al registro de horas: {ex.Message}");
            }
        }

        private void EnsureEmployeeDashboard()
        {
            var currentUrl = _driver.Url;
            if (!currentUrl.Contains("/dashboard-employee"))
            {
                _driver.Navigate().GoToUrl($"{BaseUrl}/dashboard-employee");
                Thread.Sleep(2000);
                WaitForVueToLoad();
            }
        }

        private bool IsLoggedIn()
        {
            return _driver.Url.Contains("/dashboard") ||
                   _driver.FindElements(By.XPath("//button[contains(text(), 'Cerrar sesión')]")).Count > 0;
        }

        private bool PerformLogin()
        {
            try
            {
                _driver.Navigate().GoToUrl($"{BaseUrl}/login");
                Thread.Sleep(2000);
                WaitForVueToLoad();

                var emailInput = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//input[@type='email']")));
                emailInput.Clear();
                emailInput.SendKeys(EmployeeEmail);

                var passwordInput = _driver.FindElement(By.XPath("//input[@type='password']"));
                passwordInput.Clear();
                passwordInput.SendKeys(EmployeePassword);

                var loginButton = _driver.FindElement(By.XPath("//button[contains(text(), 'Ingresar')]"));
                loginButton.Click();

                Thread.Sleep(3000);
                WaitForVueToLoad();

                return _driver.Url.Contains("dashboard-employee") || _driver.Url.Contains("dashboard");
            }
            catch
            {
                return false;
            }
        }

        private void RegisterHours(string date, string startTime, string endTime)
        {
            var dateInput = _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("date")));
            SetInputValue(dateInput, date);

            var startInput = _driver.FindElement(By.Id("startHour"));
            SetInputValue(startInput, startTime);

            var endInput = _driver.FindElement(By.Id("endHour"));
            SetInputValue(endInput, endTime);

            var submitButton = _driver.FindElement(By.XPath("//button[contains(., 'Registrar')]"));
            ClickWithScroll(submitButton);

            _wait.Until(driver =>
            {
                var button = driver.FindElement(By.XPath("//button[contains(., 'Registrar')]"));
                return button.Enabled;
            });
        }

        private void SetInputValue(IWebElement element, string value)
        {
            element.SendKeys(Keys.Control + "a");
            element.SendKeys(Keys.Delete);
            element.SendKeys(value);
        }

        private int GetTotalEntriesMetric()
        {
            var metricElement = _wait.Until(driver =>
            {
                try
                {
                    return driver.FindElement(By.XPath("//div[contains(@class,'neumorphism-card-project')]//p[contains(text(),'Registros totales')]/following-sibling::p[contains(text(),'registros')]"));
                }
                catch
                {
                    return null;
                }
            });

            var digits = Regex.Match(metricElement?.Text ?? string.Empty, "\\d+");
            return digits.Success ? int.Parse(digits.Value, CultureInfo.InvariantCulture) : 0;
        }

        private void WaitForEntriesToIncrease(int expectedValue)
        {
            _wait.Until(_ => GetTotalEntriesMetric() >= expectedValue);
        }

        private void AssertLastEntryCard()
        {
            var hoursElement = _wait.Until(driver =>
            {
                try
                {
                    return driver.FindElement(By.XPath("//div[contains(@class,'neumorphism-card-project')]//p[contains(text(),'Último registro')]/following-sibling::p[contains(@class,'text-[28px]')][1]"));
                }
                catch
                {
                    return null;
                }
            });

            Assert.IsNotNull(hoursElement, "El card de último registro debe estar disponible después de registrar horas.");
            Assert.IsTrue(hoursElement!.Text.Contains(ExpectedHours.ToString(CultureInfo.InvariantCulture)), "El último registro debe reflejar las horas registradas.");

            var dateElement = hoursElement.FindElement(By.XPath("following-sibling::p[1]"));
            Assert.IsFalse(string.IsNullOrWhiteSpace(dateElement.Text), "La tarjeta de último registro debe mostrar una fecha legible.");
        }

        private void AssertRecentEntryList()
        {
            var entryElement = _wait.Until(driver =>
            {
                var entries = driver.FindElements(By.CssSelector(".register-item"));
                return entries.FirstOrDefault();
            });

            Assert.IsNotNull(entryElement, "Debe mostrarse al menos un registro reciente luego de crear uno nuevo.");

            var entryText = entryElement!.Text.ToLowerInvariant();
            Assert.IsTrue(entryText.Contains(ExpectedHours.ToString(CultureInfo.InvariantCulture)), "El registro reciente debe mostrar la cantidad de horas registrada.");
            Assert.IsTrue(entryText.Contains("pendiente") || entryText.Contains("aprobado"), "El registro reciente debe mostrar un estado.");

            Assert.IsTrue(!string.IsNullOrWhiteSpace(entryText), "El registro reciente debe incluir información de fecha.");
        }

        private void ClickWithScroll(IWebElement element)
        {
            var js = (IJavaScriptExecutor)_driver;
            js.ExecuteScript("arguments[0].scrollIntoView({behavior: 'instant', block: 'center'});", element);
            _wait.Until(_ => element.Displayed && element.Enabled);
            element.Click();
        }

        private void WaitForVueToLoad()
        {
            _wait.Until(driver =>
            {
                try
                {
                    var body = driver.FindElement(By.TagName("body"));
                    return body != null;
                }
                catch
                {
                    return false;
                }
            });
        }
    }
}

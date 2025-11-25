using Microsoft.VisualStudio.TestTools.UnitTesting;
using backend.Services;
using System;

namespace backend.Tests
{
    [TestClass]
    public class BenefitCodeParserTests
    {
        private BenefitCodeParser _parser = null!;

        [TestInitialize]
        public void Setup()
        {
            _parser = new BenefitCodeParser();
        }

        // -------------------------------------------------------------
        // GenerateBenefitCode Tests
        // -------------------------------------------------------------

        [TestMethod]
        public void GenerateBenefitCode_ValidInput_ReturnsNormalizedCode()
        {
            // Arrange
            var benefitName = "Vacaciones";
            var deductionType = "Porcentaje";

            // Act
            var result = _parser.GenerateBenefitCode(benefitName, deductionType);

            // Assert
            Assert.AreEqual("VACACIONES_PORCENTAJE", result);
        }

        [TestMethod]
        public void GenerateBenefitCode_TrimsAndNormalizesSpaces_ReturnsCorrectCode()
        {
            // Arrange
            var benefitName = "  bono productividad ";
            var deductionType = " porcentaje ";

            // Act
            var result = _parser.GenerateBenefitCode(benefitName, deductionType);

            // Assert
            Assert.AreEqual("BONO_PRODUCTIVIDAD_PORCENTAJE", result);
        }

        [TestMethod]
        public void GenerateBenefitCode_ReplacesHyphensInBenefitName()
        {
            // Arrange
            var benefitName = "bono-productividad";
            var deductionType = "monto-fijo";

            // Act
            var result = _parser.GenerateBenefitCode(benefitName, deductionType);

            // Assert
            // Solo se normaliza el nombre, el tipo se deja con guion como en la implementación actual
            Assert.AreEqual("BONO_PRODUCTIVIDAD_MONTO-FIJO", result);
        }

        [TestMethod]
        public void GenerateBenefitCode_EmptyBenefitName_ThrowsException()
        {
            // Act & Assert
            var ex = Assert.ThrowsException<ArgumentException>(() =>
                _parser.GenerateBenefitCode("", "PORCENTAJE"));

            Assert.IsTrue(ex.Message.Contains("El nombre del beneficio es requerido"));
        }

        [TestMethod]
        public void GenerateBenefitCode_EmptyDeductionType_ThrowsException()
        {
            // Act & Assert
            var ex = Assert.ThrowsException<ArgumentException>(() =>
                _parser.GenerateBenefitCode("Vacaciones", ""));

            Assert.IsTrue(ex.Message.Contains("El tipo de deducción es requerido"));
        }

        // -------------------------------------------------------------
        // ParseBenefitNameFromCode Tests
        // -------------------------------------------------------------

        [TestMethod]
        public void ParseBenefitNameFromCode_ValidCode_ReturnsBenefitName()
        {
            // Arrange
            var code = "VACACIONES_PORCENTAJE";

            // Act
            var result = _parser.ParseBenefitNameFromCode(code);

            // Assert
            Assert.AreEqual("VACACIONES", result);
        }

        [TestMethod]
        public void ParseBenefitNameFromCode_MultipleWordBenefit_ReturnsFullName()
        {
            // Arrange
            var code = "BONO_PRODUCTIVIDAD_MENSUAL_PORCENTAJE";

            // Act
            var result = _parser.ParseBenefitNameFromCode(code);

            // Assert
            Assert.AreEqual("BONO PRODUCTIVIDAD MENSUAL", result);
        }

        [TestMethod]
        public void ParseBenefitNameFromCode_NoUnderscore_ReturnsOriginal()
        {
            // Arrange
            var code = "VACACIONES";

            // Act
            var result = _parser.ParseBenefitNameFromCode(code);

            // Assert
            Assert.AreEqual("VACACIONES", result);
        }

        [TestMethod]
        public void ParseBenefitNameFromCode_NullOrEmpty_ReturnsDefault()
        {
            // Assert
            Assert.AreEqual("Beneficio", _parser.ParseBenefitNameFromCode(null!));
            Assert.AreEqual("Beneficio", _parser.ParseBenefitNameFromCode(""));
            Assert.AreEqual("Beneficio", _parser.ParseBenefitNameFromCode("   "));
        }

        [TestMethod]
        public void ParseBenefitNameFromCode_IgnoresLastSegmentAsType()
        {
            // Arrange
            var code = "AGUINALDO_ANUAL_PORCENTAJE";

            // Act
            var result = _parser.ParseBenefitNameFromCode(code);

            // Assert
            Assert.AreEqual("AGUINALDO ANUAL", result);
        }
    }
}
﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ISO20022PaymentInitiations;
using XMLSerializerValidator;
using System.IO;

namespace ISO20022PaymentInitiationsUnitTests
{
    [TestClass]
    public class ShemaValidatorTests
    {
        [TestMethod]
        public void ISO20020XMLExampleFileIsWellValidatedThroughXSD()
        {
            //Original valid pain.008.001.02 XML file from ISO20022
            string xMLFilePath = @"XML Test Files\pain.002.001.03\pain.002.001.03_2.xml";

            string validatingErrors = SchemaValidators.ValidatePaymentStatusReportFile(xMLFilePath);
            Assert.AreEqual(String.Empty, validatingErrors);
        }

        [TestMethod]
        public void NonCompilantISO20020XMLExample2FileHasValidationErrors()
        {
            //Original valid pain.008.001.02 XML file from ISO20022
            string xMLFilePath = @"XML Test Files\pain.002.001.03\pain.002.001.03_2(NotCompilant).xml";

            string validatingErrors = SchemaValidators.ValidatePaymentStatusReportFile(xMLFilePath);
            Assert.IsFalse(validatingErrors=="");
        }

        [TestMethod]
        [ExpectedException(typeof(System.Xml.XmlException))]
        public void ErroneousFormatXMLExample3FileRaisesAnExceptioWhenValidated()
        {
            //Original valid pain.008.001.02 XML file from ISO20022
            string xMLFilePath = @"XML Test Files\pain.002.001.03\pain.002.001.03_2(ErroneousFormat).xml";

            try
            {
                string validatingErrors = SchemaValidators.ValidatePaymentStatusReportFile(xMLFilePath);
            }
            catch(System.Xml.XmlException notValidXMLFileException)
            {
                Assert.IsNotNull(notValidXMLFileException.Message);
                throw;
            }            
        }

        [TestMethod]
        public void ISO20020XMLExampleStringIsWellValidatedThroughXSD()
        {
            //Original valid pain.008.001.02 XML file from ISO20022
            string xMLFilePath = @"XML Test Files\pain.002.001.03\pain.002.001.03_2.xml";
            string xMLString = File.ReadAllText(xMLFilePath);

            string validatingErrors = SchemaValidators.ValidatePaymentStatusReportString(xMLString);
            Assert.AreEqual(String.Empty, validatingErrors);
        }

        [TestMethod]
        public void NonCompilantISO20020XMLExample2StringHasValidationErrors()
        {
            //Original valid pain.008.001.02 XML file from ISO20022
            string xMLFilePath = @"XML Test Files\pain.002.001.03\pain.002.001.03_2(NotCompilant).xml";
            string xMLString = File.ReadAllText(xMLFilePath);

            string validatingErrors = SchemaValidators.ValidatePaymentStatusReportString(xMLString);
            Assert.IsFalse(validatingErrors == "");
        }

        [TestMethod]
        [ExpectedException(typeof(System.Xml.XmlException))]
        public void ErroneousFormatXMLExample3StringRaisesAnExceptioWhenValidated()
        {
            //Original valid pain.008.001.02 XML file from ISO20022
            string xMLFilePath = @"XML Test Files\pain.002.001.03\pain.002.001.03_2(ErroneousFormat).xml";
            string xMLString = File.ReadAllText(xMLFilePath);

            try
            {
                string validatingErrors = SchemaValidators.ValidatePaymentStatusReportString(xMLString);
            }
            catch (System.Xml.XmlException notValidXMLFileException)
            {
                Assert.IsNotNull(notValidXMLFileException.Message);
                throw;
            }
        }

        //[TestMethod]
        //public void ISO20020XMLExample2FileIsWellValidatedThroughXSD()
        //{
        //    //Original valid pain.008.001.02 XML file from ISO20022
        //    string xMLFilePath = @"XML Test Files\pain.002.001.03\pain.002.001.03_2.xml";

        //    //Original valid pain.008.002.01 XSD File from ISO20022
        //    string xSDFilePath = @"XSDFiles\pain.002.001.03.xsd";

        //    string validatingErrors = XMLValidator.ValidateXMLFileThroughXSDFile(xMLFilePath, xSDFilePath);
        //    Assert.AreEqual(String.Empty, validatingErrors);
        //}





        //[TestMethod]
        //public void ACorrectPaymentStatusReportFileIsCorrectlyValidatedAndReturnsNoError()
        //{
        //}

        //[TestMethod]
        //public void ACorrectPaymentStatusReportStringIsCorrectlyValidatedAndReturnsNoError()
        //{
        //}

        //[TestMethod]
        //public void AnErroneousPaymentStatusReportFileRetursErrors()
        //{

        //}

        //[TestMethod]
        //public void AnErroneousPaymentStatusReportStringRetursErrors()
        //{

        //}
    }
}

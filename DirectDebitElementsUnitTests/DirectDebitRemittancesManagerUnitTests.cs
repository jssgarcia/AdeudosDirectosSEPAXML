﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Billing;
using DirectDebitElements;
using ReferencesAndTools;
using ExtensionMethods;

namespace DirectDebitElementsUnitTests
{
    [TestClass]
    public class DirectDebitRemittancesManagerUnitTests
    {
        static Dictionary<string, Debtor> debtors;
        static Creditor creditor;
        static CreditorAgent creditorAgent;
        static DirectDebitInitiationContract directDebitInitiationContract;
        static DirectDebitPropietaryCodesGenerator directDebitPropietaryCodesGenerator;
        static DirectDebitTransaction directDebitTransaction1;
        static DirectDebitTransaction directDebitTransaction2;
        static DirectDebitTransaction directDebitTransaction3;
        static DirectDebitAmendmentInformation amendmentInformation1;
        static string paymentInformationID1;
        static string paymentInformationID2;


        //static BankCodes spanishBankCodes;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            debtors = new Dictionary<string, Debtor>()
            {
                {"00001", new Debtor("00001", "Francisco", "Gómez-Caldito", "Viseas")},
                {"00002", new Debtor("00002", "Pedro", "Pérez", "Gómez")}
            };

            creditor = new Creditor("G12345678", "NOMBRE ACREEDOR PRUEBAS");
            creditorAgent = new CreditorAgent(new BankCode("2100", "CaixaBank", "CAIXESBBXXX"));
            directDebitInitiationContract = new DirectDebitInitiationContract(
                new BankAccount(new InternationalAccountBankNumberIBAN("ES5621001111301111111111")),
                creditor.NIF,
                "777",
                creditorAgent);

            directDebitPropietaryCodesGenerator = new DirectDebitPropietaryCodesGenerator(directDebitInitiationContract);

            var directDebitmandateslist = new[]
            {
                new {debtorID = "00001", internalReference = 1234, ccc = "21002222002222222222" },
                new {debtorID = "00002", internalReference = 1235, ccc = "21003333802222222222" }
            };

            foreach (var ddM in directDebitmandateslist)
            {
                DirectDebitMandate directDebitMandate = new DirectDebitMandate(
                    ddM.internalReference,
                    new DateTime(2013, 11, 11),
                    new BankAccount(new ClientAccountCodeCCC(ddM.ccc)),
                    debtors[ddM.debtorID].FullName);
                debtors[ddM.debtorID].AddDirectDebitMandate(directDebitMandate);
            }

            var billsList = new[]
            {
                new {debtorID = "00001", billID= "00001/01", Amount = 79, transactionDescription = "Cuota Social Octubre 2013" },
                new {debtorID = "00002", billID= "00002/01",Amount = 79, transactionDescription="Cuota Social Octubre 2013" },
                new {debtorID = "00002", billID= "00002/02",Amount = 79, transactionDescription="Cuota Social Noviembre 2013"}
            };

            foreach (var bill in billsList)
            {

                Debtor debtor = debtors[bill.debtorID];
                SimplifiedBill bills = new SimplifiedBill(
                    bill.billID,
                    bill.transactionDescription,
                    bill.Amount,
                    DateTime.Today,
                    DateTime.Today.AddMonths(1));
                debtor.AddSimplifiedBill(bills);
            }

            directDebitTransaction1 = new DirectDebitTransaction(
                debtors["00001"].SimplifiedBills.Values.ToList(),
                "PaymentInstruction1-00001",
                directDebitPropietaryCodesGenerator.CalculateMyOldCSB19MandateID(debtors["00001"].DirectDebitmandates[1234].InternalReferenceNumber),
                debtors["00001"].DirectDebitmandates[1234].DirectDebitMandateCreationDate,
                debtors["00001"].DirectDebitmandates[1234].BankAccount,
                debtors["00001"].FullName,
                null);

            directDebitTransaction2 = new DirectDebitTransaction(
                debtors["00002"].SimplifiedBills.Values.ToList(),
                "PaymentInstruction1-00002",
                directDebitPropietaryCodesGenerator.CalculateMyOldCSB19MandateID(debtors["00002"].DirectDebitmandates[1235].InternalReferenceNumber),
                debtors["00002"].DirectDebitmandates[1235].DirectDebitMandateCreationDate,
                debtors["00002"].DirectDebitmandates[1235].BankAccount,
                debtors["00002"].FullName,
                null);

            directDebitTransaction3 = new DirectDebitTransaction(
                debtors["00001"].SimplifiedBills.Values.ToList(),
                "PaymentInstruction2-00001",
                directDebitPropietaryCodesGenerator.CalculateMyOldCSB19MandateID(debtors["00001"].DirectDebitmandates[1234].InternalReferenceNumber),
                debtors["00001"].DirectDebitmandates[1234].DirectDebitMandateCreationDate,
                debtors["00001"].DirectDebitmandates[1234].BankAccount,
                debtors["00001"].FullName,
                null);

            amendmentInformation1 = new DirectDebitAmendmentInformation(
                directDebitPropietaryCodesGenerator.CalculateMyOldCSB19MandateID(1000),
                new BankAccount(new InternationalAccountBankNumberIBAN("ES7621000000650000000001")));

            paymentInformationID1 = "PRE201512010001";
            paymentInformationID2 = "PRE201511150001";
        }

        [TestMethod]
        public void AnEmptyDirectDebitTransactionInCorrectlyGenerated()
        {
            List<SimplifiedBill> emptyBillsList = new List<SimplifiedBill>();
            string transactionID = "PaymentInstruction1-00001";
            DirectDebitMandate directDebitMandate = debtors["00001"].DirectDebitmandates[1234];
            string mandateID = directDebitPropietaryCodesGenerator.CalculateMyOldCSB19MandateID(directDebitMandate.InternalReferenceNumber);

            DirectDebitRemittancesManager directDebitRemittancesManager = new DirectDebitRemittancesManager();
            DirectDebitTransaction emptyDirectDebitTransaction = directDebitRemittancesManager.CreateAnEmptyDirectDebitTransaction(
                transactionID,
                mandateID,
                directDebitMandate,
                amendmentInformation1);

            Assert.AreEqual(transactionID, emptyDirectDebitTransaction.TransactionID);
            Assert.AreEqual(mandateID, emptyDirectDebitTransaction.MandateID);
            Assert.AreEqual(directDebitMandate.DirectDebitMandateCreationDate, emptyDirectDebitTransaction.MandateSigatureDate);
            Assert.AreEqual(directDebitMandate.BankAccount, emptyDirectDebitTransaction.DebtorAccount);
            Assert.AreEqual(directDebitMandate.AccountHolderName, emptyDirectDebitTransaction.AccountHolderName);
            Assert.AreEqual(0, emptyDirectDebitTransaction.NumberOfBills);
            Assert.AreEqual(0, emptyDirectDebitTransaction.Amount);
            CollectionAssert.AreEqual(emptyBillsList, emptyDirectDebitTransaction.BillsInTransaction);
            Assert.AreEqual(amendmentInformation1, emptyDirectDebitTransaction.AmendmentInformation);
        }

        [TestMethod]
        public void AmendmentinformationCanBeNullWhenCreatingAnEmptyDirectDebitTransaction()
        {
            List<SimplifiedBill> emptyBillsList = new List<SimplifiedBill>();
            string transactionID = "PaymentInstruction1-00001";
            DirectDebitMandate directDebitMandate = debtors["00001"].DirectDebitmandates[1234];
            string mandateID = directDebitPropietaryCodesGenerator.CalculateMyOldCSB19MandateID(directDebitMandate.InternalReferenceNumber);

            DirectDebitRemittancesManager directDebitRemittancesManager = new DirectDebitRemittancesManager();
            DirectDebitTransaction emptyDirectDebitTransaction = directDebitRemittancesManager.CreateAnEmptyDirectDebitTransaction(
                transactionID,
                mandateID,
                directDebitMandate,
                null);

            Assert.AreEqual(transactionID, emptyDirectDebitTransaction.TransactionID);
            Assert.AreEqual(mandateID, emptyDirectDebitTransaction.MandateID);
            Assert.AreEqual(directDebitMandate.DirectDebitMandateCreationDate, emptyDirectDebitTransaction.MandateSigatureDate);
            Assert.AreEqual(directDebitMandate.BankAccount, emptyDirectDebitTransaction.DebtorAccount);
            Assert.AreEqual(directDebitMandate.AccountHolderName, emptyDirectDebitTransaction.AccountHolderName);
            Assert.AreEqual(0, emptyDirectDebitTransaction.NumberOfBills);
            Assert.AreEqual(0, emptyDirectDebitTransaction.Amount);
            CollectionAssert.AreEqual(emptyBillsList, emptyDirectDebitTransaction.BillsInTransaction);
            Assert.AreEqual(null, emptyDirectDebitTransaction.AmendmentInformation);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void TransactionIDCantBeNullWhenCreatingAnEmptyDirectDebitTransaction()
        {
            List<SimplifiedBill> emptyBillsList = new List<SimplifiedBill>();
            string transactionID = null;
            DirectDebitMandate directDebitMandate = debtors["00001"].DirectDebitmandates[1234];
            string mandateID = directDebitPropietaryCodesGenerator.CalculateMyOldCSB19MandateID(directDebitMandate.InternalReferenceNumber);

            DirectDebitRemittancesManager directDebitRemittancesManager = new DirectDebitRemittancesManager();
            try
            {
                DirectDebitTransaction emptyDirectDebitTransaction = directDebitRemittancesManager.CreateAnEmptyDirectDebitTransaction(
                transactionID,
                mandateID,
                directDebitMandate,
                null);
            }

            catch (System.ArgumentNullException e)
            {
                Assert.AreEqual("transactionID", e.ParamName);
                Assert.AreEqual("TransactionID can't be null", e.GetMessageWithoutParamName());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void TransactionIDCantBeEmptyWhenCreatingAnEmptyDirectDebitTransaction()
        {
            List<SimplifiedBill> emptyBillsList = new List<SimplifiedBill>();
            string transactionID = "";
            DirectDebitMandate directDebitMandate = debtors["00001"].DirectDebitmandates[1234];
            string mandateID = directDebitPropietaryCodesGenerator.CalculateMyOldCSB19MandateID(directDebitMandate.InternalReferenceNumber);

            DirectDebitRemittancesManager directDebitRemittancesManager = new DirectDebitRemittancesManager();
            try
            {
                DirectDebitTransaction emptyDirectDebitTransaction = directDebitRemittancesManager.CreateAnEmptyDirectDebitTransaction(
                transactionID,
                mandateID,
                directDebitMandate,
                null);
            }

            catch (System.ArgumentNullException e)
            {
                Assert.AreEqual("transactionID", e.ParamName);
                Assert.AreEqual("transactionID can't be empty", e.GetMessageWithoutParamName());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void MandateIDCantBeNullWhenCreatingAnEmptyDirectDebitTransaction()
        {
            List<SimplifiedBill> emptyBillsList = new List<SimplifiedBill>();
            string transactionID = "PaymentInstruction1-00001";
            DirectDebitMandate directDebitMandate = debtors["00001"].DirectDebitmandates[1234];
            string mandateID = null;

            DirectDebitRemittancesManager directDebitRemittancesManager = new DirectDebitRemittancesManager();
            try
            {
                DirectDebitTransaction emptyDirectDebitTransaction = directDebitRemittancesManager.CreateAnEmptyDirectDebitTransaction(
                transactionID,
                mandateID,
                directDebitMandate,
                null);
            }

            catch (System.ArgumentNullException e)
            {
                Assert.AreEqual("MandateID", e.ParamName);
                Assert.AreEqual("MandateID can't be null", e.GetMessageWithoutParamName());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void MandateIDCantBeEmptyWhenCreatingAnEmptyDirectDebitTransaction()
        {
            List<SimplifiedBill> emptyBillsList = new List<SimplifiedBill>();
            string transactionID = "PaymentInstruction1-00001";
            DirectDebitMandate directDebitMandate = debtors["00001"].DirectDebitmandates[1234];
            string mandateID = " ";

            DirectDebitRemittancesManager directDebitRemittancesManager = new DirectDebitRemittancesManager();
            try
            {
                DirectDebitTransaction emptyDirectDebitTransaction = directDebitRemittancesManager.CreateAnEmptyDirectDebitTransaction(
                transactionID,
                mandateID,
                directDebitMandate,
                null);
            }

            catch (System.ArgumentNullException e)
            {
                Assert.AreEqual("MandateID", e.ParamName);
                Assert.AreEqual("MandateID can't be empty", e.GetMessageWithoutParamName());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void DirectDebitMandateCantBeNullWhenCreatingAnEmptyDirectDebitTransaction()
        {
            List<SimplifiedBill> emptyBillsList = new List<SimplifiedBill>();
            string transactionID = "PaymentInstruction1-00001";
            DirectDebitMandate directDebitMandate = null;
            string mandateID = "000007701234";

            DirectDebitRemittancesManager directDebitRemittancesManager = new DirectDebitRemittancesManager();
            try
            {
                DirectDebitTransaction emptyDirectDebitTransaction = directDebitRemittancesManager.CreateAnEmptyDirectDebitTransaction(
                transactionID,
                mandateID,
                directDebitMandate,
                null);
            }

            catch (System.ArgumentNullException e)
            {
                Assert.AreEqual("directDebitMandate", e.ParamName);
                Assert.AreEqual("DirectDebitMandate can't be null", e.GetMessageWithoutParamName());
                throw;
            }
        }

        [TestMethod]
        public void ADirectDebitTransactionInCorrectlyGeneratedFromAGroupOfBills()
        {
            List<SimplifiedBill> billsList = debtors["00001"].SimplifiedBills.Values.ToList();
            string transactionID = "PaymentInstruction1-00001";
            DirectDebitMandate directDebitMandate = debtors["00001"].DirectDebitmandates[1234];
            string mandateID = directDebitPropietaryCodesGenerator.CalculateMyOldCSB19MandateID(directDebitMandate.InternalReferenceNumber);

            DirectDebitRemittancesManager directDebitRemittancesManager = new DirectDebitRemittancesManager();
            DirectDebitTransaction directDebitTransaction = directDebitRemittancesManager.CreateADirectDebitTransaction(
                transactionID,
                mandateID,
                directDebitMandate,
                billsList,
                amendmentInformation1);

            Assert.AreEqual(transactionID, directDebitTransaction.TransactionID);
            Assert.AreEqual(mandateID, directDebitTransaction.MandateID);
            Assert.AreEqual(directDebitMandate.DirectDebitMandateCreationDate, directDebitTransaction.MandateSigatureDate);
            Assert.AreEqual(directDebitMandate.BankAccount, directDebitTransaction.DebtorAccount);
            Assert.AreEqual(directDebitMandate.AccountHolderName, directDebitTransaction.AccountHolderName);
            Assert.AreEqual(1, directDebitTransaction.NumberOfBills);
            Assert.AreEqual(79, directDebitTransaction.Amount);
            CollectionAssert.AreEqual(billsList, directDebitTransaction.BillsInTransaction);
        }

        [TestMethod]
        public void AmendmentinformationCanBeNullWhenCreatingADirectDebitTransactionFromAGroupOfBills()
        {
            List<SimplifiedBill> billsList = debtors["00001"].SimplifiedBills.Values.ToList();
            string transactionID = "PaymentInstruction1-00001";
            DirectDebitMandate directDebitMandate = debtors["00001"].DirectDebitmandates[1234];
            string mandateID = directDebitPropietaryCodesGenerator.CalculateMyOldCSB19MandateID(directDebitMandate.InternalReferenceNumber);

            DirectDebitRemittancesManager directDebitRemittancesManager = new DirectDebitRemittancesManager();
            DirectDebitTransaction directDebitTransaction = directDebitRemittancesManager.CreateADirectDebitTransaction(
                transactionID,
                mandateID,
                directDebitMandate,
                billsList,
                null);

            Assert.AreEqual(transactionID, directDebitTransaction.TransactionID);
            Assert.AreEqual(mandateID, directDebitTransaction.MandateID);
            Assert.AreEqual(directDebitMandate.DirectDebitMandateCreationDate, directDebitTransaction.MandateSigatureDate);
            Assert.AreEqual(directDebitMandate.BankAccount, directDebitTransaction.DebtorAccount);
            Assert.AreEqual(directDebitMandate.AccountHolderName, directDebitTransaction.AccountHolderName);
            Assert.AreEqual(1, directDebitTransaction.NumberOfBills);
            Assert.AreEqual(79, directDebitTransaction.Amount);
            CollectionAssert.AreEqual(billsList, directDebitTransaction.BillsInTransaction);
            Assert.AreEqual(null, directDebitTransaction.AmendmentInformation);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void TransactionIDCantBeNullWhenCreatingADirectDebitTransaction()
        {
            List<SimplifiedBill> billsList = debtors["00001"].SimplifiedBills.Values.ToList();
            string transactionID = null;
            DirectDebitMandate directDebitMandate = debtors["00001"].DirectDebitmandates[1234];
            string mandateID = directDebitPropietaryCodesGenerator.CalculateMyOldCSB19MandateID(directDebitMandate.InternalReferenceNumber);

            DirectDebitRemittancesManager directDebitRemittancesManager = new DirectDebitRemittancesManager();
            try
            {
                DirectDebitTransaction directDebitTransaction = directDebitRemittancesManager.CreateADirectDebitTransaction(
                transactionID,
                mandateID,
                directDebitMandate,
                billsList,
                null);
            }

            catch (System.ArgumentNullException e)
            {
                Assert.AreEqual("transactionID", e.ParamName);
                Assert.AreEqual("TransactionID can't be null", e.GetMessageWithoutParamName());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void TransactionIDCantBeEmptyWhenCreatingADirectDebitTransactionFromAGroupOfBills()
        {
            List<SimplifiedBill> billsList = debtors["00001"].SimplifiedBills.Values.ToList();
            string transactionID = "";
            DirectDebitMandate directDebitMandate = debtors["00001"].DirectDebitmandates[1234];
            string mandateID = directDebitPropietaryCodesGenerator.CalculateMyOldCSB19MandateID(directDebitMandate.InternalReferenceNumber);

            DirectDebitRemittancesManager directDebitRemittancesManager = new DirectDebitRemittancesManager();
            try
            {
                DirectDebitTransaction directDebitTransaction = directDebitRemittancesManager.CreateADirectDebitTransaction(
                transactionID,
                mandateID,
                directDebitMandate,
                billsList,
                null);
            }

            catch (System.ArgumentNullException e)
            {
                Assert.AreEqual("transactionID", e.ParamName);
                Assert.AreEqual("transactionID can't be empty", e.GetMessageWithoutParamName());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void MandateIDCantBeNullWhenCreatingADirectDebitTransactionFromAGroupOfBills()
        {
            List<SimplifiedBill> billsList = debtors["00001"].SimplifiedBills.Values.ToList();
            string transactionID = "PaymentInstruction1-00001";
            DirectDebitMandate directDebitMandate = debtors["00001"].DirectDebitmandates[1234];
            string mandateID = null;

            DirectDebitRemittancesManager directDebitRemittancesManager = new DirectDebitRemittancesManager();
            try
            {
                DirectDebitTransaction directDebitTransaction = directDebitRemittancesManager.CreateADirectDebitTransaction(
                transactionID,
                mandateID,
                directDebitMandate,
                billsList,
                null);
            }

            catch (System.ArgumentNullException e)
            {
                Assert.AreEqual("MandateID", e.ParamName);
                Assert.AreEqual("MandateID can't be null", e.GetMessageWithoutParamName());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void MandateIDCantBeEmptyWhenCreatingADirectDebitTransactionFromAGroupOfBills()
        {
            List<SimplifiedBill> billsList = debtors["00001"].SimplifiedBills.Values.ToList();
            string transactionID = "PaymentInstruction1-00001";
            DirectDebitMandate directDebitMandate = debtors["00001"].DirectDebitmandates[1234];
            string mandateID = " ";

            DirectDebitRemittancesManager directDebitRemittancesManager = new DirectDebitRemittancesManager();
            try
            {
                DirectDebitTransaction directDebitTransaction = directDebitRemittancesManager.CreateADirectDebitTransaction(
                transactionID,
                mandateID,
                directDebitMandate,
                billsList,
                null);
            }

            catch (System.ArgumentNullException e)
            {
                Assert.AreEqual("MandateID", e.ParamName);
                Assert.AreEqual("MandateID can't be empty", e.GetMessageWithoutParamName());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void DirectDebitMandateCantBeNullWhenCreatingADirectDebitTransactionFromAGroupOfBills()
        {
            List<SimplifiedBill> billsList = debtors["00001"].SimplifiedBills.Values.ToList();
            string transactionID = "PaymentInstruction1-00001";
            DirectDebitMandate directDebitMandate = null;
            string mandateID = "000007701234";

            DirectDebitRemittancesManager directDebitRemittancesManager = new DirectDebitRemittancesManager();
            try
            {
                DirectDebitTransaction directDebitTransaction = directDebitRemittancesManager.CreateADirectDebitTransaction(
                transactionID,
                mandateID,
                directDebitMandate,
                billsList,
                null);
            }

            catch (System.ArgumentNullException e)
            {
                Assert.AreEqual("directDebitMandate", e.ParamName);
                Assert.AreEqual("DirectDebitMandate can't be null", e.GetMessageWithoutParamName());
                throw;
            }
        }

        [TestMethod]
        public void ABillCanBeAddedToADirectDebitTransaction()
        {
            DirectDebitRemittancesManager directDebitRemittancesManager = new DirectDebitRemittancesManager();

            List<SimplifiedBill> billsList = new List<SimplifiedBill>() { debtors["00002"].SimplifiedBills.ElementAt(0).Value };
            string transactionID = "PaymentInstruction1-00002";
            DirectDebitMandate directDebitMandate = debtors["00002"].DirectDebitmandates[1235];
            string mandateID = directDebitPropietaryCodesGenerator.CalculateMyOldCSB19MandateID(directDebitMandate.InternalReferenceNumber);
            DirectDebitTransaction directDebitTransaction = directDebitRemittancesManager.CreateADirectDebitTransaction(
                transactionID,
                mandateID,
                directDebitMandate,
                billsList,
                null);
            SimplifiedBill bill = debtors["00002"].SimplifiedBills.ElementAt(1).Value;

            directDebitRemittancesManager.AddBilllToExistingDirectDebitTransaction(directDebitTransaction, bill);

            Assert.AreEqual(2, directDebitTransaction.NumberOfBills);
            Assert.AreEqual(158, directDebitTransaction.Amount);
            CollectionAssert.AreEqual(billsList, directDebitTransaction.BillsInTransaction);
        }

        [TestMethod]
        public void AnEmptyADirectDebitPaymentInstructionInCorrectlyCreated()
        {
            string localInstrument = "COR1";

            DirectDebitRemittancesManager directDebitRemittancesManager = new DirectDebitRemittancesManager();
            DirectDebitPaymentInstruction directDebitPaymentInstruction = directDebitRemittancesManager.CreateAnEmptyDirectDebitPaymentInstruction(
                paymentInformationID1,
                localInstrument);

            Assert.AreEqual("PRE201512010001", directDebitPaymentInstruction.PaymentInformationID);
            Assert.AreEqual("COR1", directDebitPaymentInstruction.LocalInstrument);
            Assert.AreEqual(0, directDebitPaymentInstruction.NumberOfDirectDebitTransactions);
            Assert.AreEqual(0, directDebitPaymentInstruction.TotalAmount);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentOutOfRangeException))]
        public void CantAssignAPaymentInformationIDLongerThan35Characters()
        {
            string paymentInformationID = "0123456789012345678901234567890123456789";

            DirectDebitRemittancesManager directDebitRemittancesManager = new DirectDebitRemittancesManager();
            try
            {
                DirectDebitPaymentInstruction directDebitPaymentInstruction = directDebitRemittancesManager.CreateAnEmptyDirectDebitPaymentInstruction(
                    paymentInformationID,
                    "COR1");
            }

            catch (System.ArgumentOutOfRangeException e)
            {
                Assert.AreEqual("paymentInformationID", e.ParamName);
                Assert.AreEqual("PaymentInformationID lenght can't exceed 35 characters", e.GetMessageWithoutParamName());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void CantAssignAnEmptyPaymentInformationID()
        {
            DirectDebitRemittancesManager directDebitRemittancesManager = new DirectDebitRemittancesManager();
            try
            {             
                DirectDebitPaymentInstruction directDebitPaymentInstruction = directDebitRemittancesManager.CreateAnEmptyDirectDebitPaymentInstruction("", "COR1");
            }

            catch (System.ArgumentException e)
            {
                Assert.AreEqual("paymentInformationID", e.ParamName);
                Assert.AreEqual("PaymentInformationID can't be empty", e.GetMessageWithoutParamName());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void CantAssignANullPaymentInformationID()
        {
            DirectDebitRemittancesManager directDebitRemittancesManager = new DirectDebitRemittancesManager();
            try
            {
                DirectDebitPaymentInstruction directDebitPaymentInstruction = directDebitRemittancesManager.CreateAnEmptyDirectDebitPaymentInstruction(null, "COR1");
            }

            catch (System.ArgumentNullException e)
            {
                Assert.AreEqual("paymentInformationID", e.ParamName);
                Assert.AreEqual("PaymentInformationID can't be null", e.GetMessageWithoutParamName());
                throw;
            }
        }

        [TestMethod]
        public void ADirectDebitTransactionIsCorrectlyAddedToADirectDebitPaymentInstruction()
        {
            string localInstrument = "COR1";
            List<DirectDebitTransaction> directDebitTransactions = new List<DirectDebitTransaction>() { directDebitTransaction1 };

            DirectDebitRemittancesManager directDebitRemittancesManager = new DirectDebitRemittancesManager();
            DirectDebitPaymentInstruction directDebitPaymentInstruction = directDebitRemittancesManager.CreateADirectDebitPaymentInstruction(
                paymentInformationID1,
                localInstrument,
                directDebitTransactions);

            directDebitPaymentInstruction.AddDirectDebitTransaction(directDebitTransaction2);

            Assert.AreEqual(2, directDebitPaymentInstruction.NumberOfDirectDebitTransactions);
            Assert.AreEqual(237, directDebitPaymentInstruction.TotalAmount);
        }

        [TestMethod]
        public void ADirectDebitPaymentInstructionIsCorrectlyCreatedWithoutProvidingNumberOfTransactionsNorControlSum()
        {
            string localInstrument = "COR1";
            List<DirectDebitTransaction> directDebitTransactions = new List<DirectDebitTransaction>() { directDebitTransaction1, directDebitTransaction2 };

            DirectDebitRemittancesManager directDebitRemittancesManager = new DirectDebitRemittancesManager();
            DirectDebitPaymentInstruction directDebitPaymentInstruction = directDebitRemittancesManager.CreateADirectDebitPaymentInstruction(
                paymentInformationID1,
                localInstrument,
                directDebitTransactions);

            Assert.AreEqual("PRE201512010001", directDebitPaymentInstruction.PaymentInformationID);
            Assert.AreEqual("COR1", directDebitPaymentInstruction.LocalInstrument);
            Assert.AreEqual(2, directDebitPaymentInstruction.NumberOfDirectDebitTransactions);
            Assert.AreEqual(237, directDebitPaymentInstruction.TotalAmount);
        }

        [TestMethod]
        public void IfGivenCorrectNumberOfTransactionsAndControlSumTheDirectDebitPaymentInstructionIsCorrectlyCreated()
        {
            string localInstrument = "COR1";
            List<DirectDebitTransaction> directDebitTransactions = new List<DirectDebitTransaction>() { directDebitTransaction1, directDebitTransaction2 };
            int numberOfTransactions = directDebitTransactions.Count;
            decimal controlSum = directDebitTransactions.Select(directDebitTransaction => directDebitTransaction.Amount).Sum();

            DirectDebitRemittancesManager directDebitRemittancesManager = new DirectDebitRemittancesManager();
            DirectDebitPaymentInstruction directDebitPaymentInstruction = directDebitRemittancesManager.CreateADirectDebitPaymentInstruction(
                paymentInformationID1,
                localInstrument,
                directDebitTransactions,
                numberOfTransactions,
                controlSum);

            Assert.AreEqual("PRE201512010001", directDebitPaymentInstruction.PaymentInformationID);
            Assert.AreEqual("COR1", directDebitPaymentInstruction.LocalInstrument);
            Assert.AreEqual(2, directDebitPaymentInstruction.NumberOfDirectDebitTransactions);
            Assert.AreEqual(237, directDebitPaymentInstruction.TotalAmount);
        }

        [TestMethod]
        [ExpectedException(typeof(System.TypeInitializationException))]
        public void IfGivenIncorrectNumberOfTransactionsTheDirectDebitPaymentInstructionThrowsATypeInitializationErrorException()
        {
            string localInstrument = "COR1";
            List<DirectDebitTransaction> directDebitTransactions = new List<DirectDebitTransaction>() { directDebitTransaction1, directDebitTransaction2 };
            int numberOfTransactions = 1;
            decimal controlSum = directDebitTransactions.Select(directDebitTransaction => directDebitTransaction.Amount).Sum();

            DirectDebitRemittancesManager directDebitRemittancesManager = new DirectDebitRemittancesManager();
            try
            {
                DirectDebitPaymentInstruction directDebitPaymentInstruction = directDebitRemittancesManager.CreateADirectDebitPaymentInstruction(
                    paymentInformationID1,
                    localInstrument,
                    directDebitTransactions,
                    numberOfTransactions,
                    controlSum);
            }
            catch (TypeInitializationException typeInitializationException)
            {
                Assert.AreEqual("DirectDebitPaymentInstruction", typeInitializationException.TypeName);

                string expectedErrorMessage = "The Number of Transactions is wrong. It should be 2, but 1 is provided";
                ArgumentException argumentException = (ArgumentException)typeInitializationException.InnerException;
                string paramName = argumentException.ParamName;
                string exceptionMessage = argumentException.GetMessageWithoutParamName();
                Assert.AreEqual("numberOfTransactions", paramName);
                Assert.AreEqual(expectedErrorMessage, exceptionMessage);
                throw typeInitializationException;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(System.TypeInitializationException))]
        public void IfGivenIncorrectControlSumTheDirectDebitPaymentInstructionThrowsATypeInitializationErrorException()
        {
            string localInstrument = "COR1";
            List<DirectDebitTransaction> directDebitTransactions = new List<DirectDebitTransaction>() { directDebitTransaction1, directDebitTransaction2 };
            int numberOfTransactions = directDebitTransactions.Count;
            decimal controlSum = 0;

            DirectDebitRemittancesManager directDebitRemittancesManager = new DirectDebitRemittancesManager();
            try
            {
                DirectDebitPaymentInstruction directDebitPaymentInstruction = directDebitRemittancesManager.CreateADirectDebitPaymentInstruction(
                    paymentInformationID1,
                    localInstrument,
                    directDebitTransactions,
                    numberOfTransactions,
                    controlSum);
            }
            catch (TypeInitializationException typeInitializationException)
            {
                Assert.AreEqual("DirectDebitPaymentInstruction", typeInitializationException.TypeName);

                string expectedErrorMessage = "The Control Sum is wrong. It should be 237, but 0 is provided";
                ArgumentException argumentException = (ArgumentException)typeInitializationException.InnerException;
                string paramName = argumentException.ParamName;
                string exceptionMessage = argumentException.GetMessageWithoutParamName();
                Assert.AreEqual("controlSum", paramName);
                Assert.AreEqual(expectedErrorMessage, exceptionMessage);
                throw typeInitializationException;
            }
        }

        [TestMethod]
        public void AnEmptyDirectDebitRemittanceInstanceIsCorrectlyCreated()
        {
            DateTime creationDate = new DateTime(2013, 11, 30, 7, 15, 0);
            string messageID = "ES26777G12345678" + creationDate.ToString("yyyyMMddHH:mm:ss");
            DateTime requestedCollectionDate = new DateTime(2013, 12, 1);

            DirectDebitRemittancesManager directDebitRemittancesManager = new DirectDebitRemittancesManager();
            DirectDebitRemittance directDebitRemittance = directDebitRemittancesManager.CreateAnEmptyDirectDebitRemittance(
                messageID,
                creationDate,
                requestedCollectionDate,
                directDebitInitiationContract);

            string expectedMessageId = "ES26777G123456782013113007:15:00";
            Assert.AreEqual(expectedMessageId, directDebitRemittance.MessageID);
            Assert.AreEqual(creationDate, directDebitRemittance.CreationDate);
            Assert.AreEqual(requestedCollectionDate, directDebitRemittance.RequestedCollectionDate);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void MessageIDCantBeNullInADirectDebitRemittance()
        {
            DateTime creationDate = new DateTime(2013, 11, 30, 7, 15, 0);
            string messageID = null;
            DateTime requestedCollectionDate = new DateTime(2013, 12, 1);

            DirectDebitRemittancesManager directDebitRemittancesManager = new DirectDebitRemittancesManager();
            try
            {
                DirectDebitRemittance directDebitRemittance = directDebitRemittancesManager.CreateAnEmptyDirectDebitRemittance(
                    messageID,
                    creationDate,
                    requestedCollectionDate,
                    directDebitInitiationContract);
            }

            catch (System.ArgumentNullException e)
            {
                Assert.AreEqual("MessageID", e.ParamName);
                Assert.AreEqual("MessageID can't be null", e.GetMessageWithoutParamName());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void MessageIDCantBeEmptyInADirectDebitRemittance()
        {
            DateTime creationDate = new DateTime(2013, 11, 30, 7, 15, 0);
            string messageID = "";
            DateTime requestedCollectionDate = new DateTime(2013, 12, 1);

            DirectDebitRemittancesManager directDebitRemittancesManager = new DirectDebitRemittancesManager();
            try
            {
                DirectDebitRemittance directDebitRemittance = directDebitRemittancesManager.CreateAnEmptyDirectDebitRemittance(
                    messageID,
                    creationDate,
                    requestedCollectionDate,
                    directDebitInitiationContract);
            }

            catch (System.ArgumentException e)
            {
                Assert.AreEqual("MessageID", e.ParamName);
                Assert.AreEqual("MessageID can't be empty", e.GetMessageWithoutParamName());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentOutOfRangeException))]
        public void MessageIDCantBeLongerThan35CharactersInADirectDebitRemittance()
        {
            DateTime creationDate = new DateTime(2013, 11, 30, 7, 15, 0);
            string messageID = "1234567890123456789012345678901234567890";
            DateTime requestedCollectionDate = new DateTime(2013, 12, 1);

            DirectDebitRemittancesManager directDebitRemittancesManager = new DirectDebitRemittancesManager();
            try
            {
                DirectDebitRemittance directDebitRemittance = directDebitRemittancesManager.CreateAnEmptyDirectDebitRemittance(
                    messageID,
                    creationDate,
                    requestedCollectionDate,
                    directDebitInitiationContract);
            }

            catch (System.ArgumentOutOfRangeException e)
            {
                Assert.AreEqual("MessageID", e.ParamName);
                Assert.AreEqual("MessageID can't be longer than 35 characters", e.GetMessageWithoutParamName());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void DirectDebitInitiationContractCantBeNullInADirectDebitRemittance()
        {
            DateTime creationDate = new DateTime(2013, 11, 30, 7, 15, 0);
            string messageID = "ES26777G123456782013113007:15:00";
            DateTime requestedCollectionDate = new DateTime(2013, 12, 1);

            DirectDebitRemittancesManager directDebitRemittancesManager = new DirectDebitRemittancesManager();
            try
            {
                DirectDebitRemittance directDebitRemittance = directDebitRemittancesManager.CreateAnEmptyDirectDebitRemittance(
                    messageID,
                    creationDate,
                    requestedCollectionDate,
                    null);
            }

            catch (System.ArgumentNullException e)
            {
                Assert.AreEqual("DirectDebitInitiationContract", e.ParamName);
                Assert.AreEqual("DirectDebitInitiationContract can't be null", e.GetMessageWithoutParamName());
                throw;
            }
        }

        [TestMethod]
        public void AnDirectDebitRemittanceIsCorrectlyCreatedWithoutProvidingNumberOfTransactionsNorControlSum()
        {
            DateTime creationDate = new DateTime(2013, 11, 30, 7, 15, 0);
            string messageID = "ES26777G12345678" + creationDate.ToString("yyyyMMddHH:mm:ss");
            DateTime requestedCollectionDate = new DateTime(2013, 12, 1);
            string localInstrument = "COR1";
            List<DirectDebitTransaction> directDebitTransactions = new List<DirectDebitTransaction>() { directDebitTransaction1, directDebitTransaction2 };
            DirectDebitPaymentInstruction directDebitPaymentInstruction = new DirectDebitPaymentInstruction(
                paymentInformationID1, localInstrument, directDebitTransactions);
            List<DirectDebitPaymentInstruction> directDebitPaymentInstructions = new List<DirectDebitPaymentInstruction>() { directDebitPaymentInstruction };

            DirectDebitRemittancesManager directDebitRemittancesManager = new DirectDebitRemittancesManager();
            DirectDebitRemittance directDebitRemittance = directDebitRemittancesManager.CreateADirectDebitRemittance(
                messageID,
                creationDate,
                requestedCollectionDate,
                directDebitInitiationContract,
                directDebitPaymentInstructions);

            string expectedMessageId = "ES26777G123456782013113007:15:00";
            Assert.AreEqual(expectedMessageId, directDebitRemittance.MessageID);
            Assert.AreEqual(creationDate, directDebitRemittance.CreationDate);
            Assert.AreEqual(requestedCollectionDate, directDebitRemittance.RequestedCollectionDate);
            Assert.AreEqual(1, directDebitRemittance.DirectDebitPaymentInstructions.Count);
            Assert.AreEqual(2, directDebitRemittance.DirectDebitPaymentInstructions[0].NumberOfDirectDebitTransactions);
            Assert.AreEqual(237, directDebitRemittance.DirectDebitPaymentInstructions[0].TotalAmount);
        }

        [TestMethod]
        public void IfGivenCorrectNumberOfTransactionsAndControlSumTheDirectDebitRemittanceIsCorrectlyCreated()
        {
            DateTime creationDate = new DateTime(2013, 11, 30, 7, 15, 0);
            string messageID = "ES26777G12345678" + creationDate.ToString("yyyyMMddHH:mm:ss");
            DateTime requestedCollectionDate = new DateTime(2013, 12, 1);
            string localInstrument = "COR1";
            List<DirectDebitTransaction> directDebitTransactions = new List<DirectDebitTransaction>() { directDebitTransaction1, directDebitTransaction2 };
            DirectDebitPaymentInstruction directDebitPaymentInstruction = new DirectDebitPaymentInstruction(
                paymentInformationID1, localInstrument, directDebitTransactions);
            List<DirectDebitPaymentInstruction> directDebitPaymentInstructions = new List<DirectDebitPaymentInstruction>() { directDebitPaymentInstruction };
            int numberOfTransactions = directDebitPaymentInstructions.Select(paymentInstruction => paymentInstruction.NumberOfDirectDebitTransactions).Sum();
            decimal controlSum = directDebitPaymentInstructions.Select(paymentInstruction => paymentInstruction.TotalAmount).Sum();

            DirectDebitRemittancesManager directDebitRemittancesManager = new DirectDebitRemittancesManager();
            DirectDebitRemittance directDebitRemittance = directDebitRemittancesManager.CreateADirectDebitRemittance(
                messageID,
                creationDate,
                requestedCollectionDate,
                directDebitInitiationContract,
                directDebitPaymentInstructions,
                numberOfTransactions,
                controlSum);

            Assert.AreEqual("PRE201512010001", directDebitPaymentInstruction.PaymentInformationID);
            Assert.AreEqual("COR1", directDebitPaymentInstruction.LocalInstrument);
            Assert.AreEqual(2, directDebitPaymentInstruction.NumberOfDirectDebitTransactions);
            Assert.AreEqual(237, directDebitPaymentInstruction.TotalAmount);
        }

        [TestMethod]
        [ExpectedException(typeof(System.TypeInitializationException))]
        public void IfGivenIncorrectNumberOfTransactionsTheDirectDebitRemittanceThrowsATypeInitializationErrorException()
        {
            DateTime creationDate = new DateTime(2013, 11, 30, 7, 15, 0);
            string messageID = "ES26777G12345678" + creationDate.ToString("yyyyMMddHH:mm:ss");
            DateTime requestedCollectionDate = new DateTime(2013, 12, 1);
            string localInstrument = "COR1";
            List<DirectDebitTransaction> directDebitTransactions = new List<DirectDebitTransaction>() { directDebitTransaction1, directDebitTransaction2 };
            DirectDebitPaymentInstruction directDebitPaymentInstruction = new DirectDebitPaymentInstruction(
                paymentInformationID1, localInstrument, directDebitTransactions);
            List<DirectDebitPaymentInstruction> directDebitPaymentInstructions = new List<DirectDebitPaymentInstruction>() { directDebitPaymentInstruction };
            int numberOfTransactions = 1;
            decimal controlSum = directDebitPaymentInstructions.Select(paymentInstruction => paymentInstruction.TotalAmount).Sum();

            DirectDebitRemittancesManager directDebitRemittancesManager = new DirectDebitRemittancesManager();
            try
            {
                DirectDebitRemittance directDebitRemittance = directDebitRemittancesManager.CreateADirectDebitRemittance(
                    messageID,
                    creationDate,
                    requestedCollectionDate,
                    directDebitInitiationContract,
                    directDebitPaymentInstructions,
                    numberOfTransactions,
                    controlSum);
            }
            catch (TypeInitializationException typeInitializationException)
            {
                Assert.AreEqual("DirectDebitRemittance", typeInitializationException.TypeName);

                string expectedErrorMessage = "The Number of Transactions is wrong. It should be 2, but 1 is provided";
                ArgumentException argumentException = (ArgumentException)typeInitializationException.InnerException;
                string paramName = argumentException.ParamName;
                string exceptionMessage = argumentException.GetMessageWithoutParamName();
                Assert.AreEqual("numberOfTransactions", paramName);
                Assert.AreEqual(expectedErrorMessage, exceptionMessage);
                throw typeInitializationException;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(System.TypeInitializationException))]
        public void IfGivenIncorrectControlSumTheDirectDebitRemittanceThrowsATypeInitializationErrorException()
        {
            DateTime creationDate = new DateTime(2013, 11, 30, 7, 15, 0);
            string messageID = "ES26777G12345678" + creationDate.ToString("yyyyMMddHH:mm:ss");
            DateTime requestedCollectionDate = new DateTime(2013, 12, 1);
            string localInstrument = "COR1";
            List<DirectDebitTransaction> directDebitTransactions = new List<DirectDebitTransaction>() { directDebitTransaction1, directDebitTransaction2 };
            DirectDebitPaymentInstruction directDebitPaymentInstruction = new DirectDebitPaymentInstruction(
                paymentInformationID1, localInstrument, directDebitTransactions);
            List<DirectDebitPaymentInstruction> directDebitPaymentInstructions = new List<DirectDebitPaymentInstruction>() { directDebitPaymentInstruction };
            int numberOfTransactions = directDebitPaymentInstructions.Select(paymentInstruction => paymentInstruction.NumberOfDirectDebitTransactions).Sum();
            decimal controlSum = 100;

            DirectDebitRemittancesManager directDebitRemittancesManager = new DirectDebitRemittancesManager();
            try
            {
                DirectDebitRemittance directDebitRemittance = directDebitRemittancesManager.CreateADirectDebitRemittance(
                    messageID,
                    creationDate,
                    requestedCollectionDate,
                    directDebitInitiationContract,
                    directDebitPaymentInstructions,
                    numberOfTransactions,
                    controlSum);
            }
            catch (TypeInitializationException typeInitializationException)
            {
                Assert.AreEqual("DirectDebitRemittance", typeInitializationException.TypeName);

                string expectedErrorMessage = "The Control Sum is wrong. It should be 237, but 100 is provided";
                ArgumentException argumentException = (ArgumentException)typeInitializationException.InnerException;
                string paramName = argumentException.ParamName;
                string exceptionMessage = argumentException.GetMessageWithoutParamName();
                Assert.AreEqual("controlSum", paramName);
                Assert.AreEqual(expectedErrorMessage, exceptionMessage);
                throw typeInitializationException;
            }
        }



        [TestMethod]
        public void ADirectDebitPaymentInstructionIsCorrectlyAddedToADirectDebitRemittance()
        {
            DateTime creationDate = new DateTime(2013, 11, 30, 7, 15, 0);
            string messageID = "ES26777G12345678" + creationDate.ToString("yyyyMMddHH:mm:ss");
            DateTime requestedCollectionDate = new DateTime(2013, 12, 1);          
            DirectDebitRemittance directDebitRemittance = new DirectDebitRemittance(messageID, creationDate, requestedCollectionDate, directDebitInitiationContract);
            string localInstrument = "COR1";
            List<DirectDebitTransaction> directDebitTransactions = new List<DirectDebitTransaction>() { directDebitTransaction1, directDebitTransaction2 };
            DirectDebitPaymentInstruction directDebitPaymentInstruction = new DirectDebitPaymentInstruction(
                paymentInformationID1, localInstrument, directDebitTransactions);

            DirectDebitRemittancesManager directDebitRemittancesManager = new DirectDebitRemittancesManager();
            directDebitRemittancesManager.AddDirectDebitPaymentInstructionToDirectDebitRemittance(
                directDebitRemittance,
                directDebitPaymentInstruction);

            Assert.AreEqual(1, directDebitRemittance.DirectDebitPaymentInstructions.Count);
            Assert.AreEqual(2, directDebitRemittance.DirectDebitPaymentInstructions[0].NumberOfDirectDebitTransactions);
            Assert.AreEqual(237, directDebitRemittance.DirectDebitPaymentInstructions[0].TotalAmount);
        }

        [TestMethod]
        public void ADirectDebitRemittanceCanHaveMoreThanOnePaymentInstruction()
        {
            string localInstrument1 = "COR1";
            List<DirectDebitTransaction> directDebitTransactions1 = new List<DirectDebitTransaction>() { directDebitTransaction1, directDebitTransaction2 };
            DirectDebitPaymentInstruction directDebitPaymentInstruction1 = new DirectDebitPaymentInstruction(
                paymentInformationID1, localInstrument1, directDebitTransactions1);
            DateTime creationDate = new DateTime(2013, 11, 30, 7, 15, 0);
            string messageID = "ES26777G12345678" + creationDate.ToString("yyyyMMddHH:mm:ss");
            DateTime requestedCollectionDate = new DateTime(2013, 12, 1);
            DirectDebitRemittance directDebitRemittance = new DirectDebitRemittance(
                messageID,
                creationDate,
                requestedCollectionDate,
                directDebitInitiationContract,
                new List<DirectDebitPaymentInstruction>() { directDebitPaymentInstruction1 });
            string localInstrument2 = "COR1";
            List<DirectDebitTransaction> directDebitTransactions2 = new List<DirectDebitTransaction>() { directDebitTransaction3 };
            DirectDebitPaymentInstruction directDebitPaymentInstruction2 = new DirectDebitPaymentInstruction(
                paymentInformationID2, localInstrument2, directDebitTransactions2);

            DirectDebitRemittancesManager directDebitRemittancesManager = new DirectDebitRemittancesManager();
            directDebitRemittance.AddDirectDebitPaymentInstruction(directDebitPaymentInstruction2);

            Assert.AreEqual(2, directDebitRemittance.DirectDebitPaymentInstructions.Count);
            Assert.AreEqual(3, directDebitRemittance.NumberOfTransactions);
            Assert.AreEqual(316, directDebitRemittance.ControlSum);
            Assert.AreEqual(2, directDebitRemittance.DirectDebitPaymentInstructions[0].NumberOfDirectDebitTransactions);
            Assert.AreEqual(237, directDebitRemittance.DirectDebitPaymentInstructions[0].TotalAmount);
            Assert.AreEqual(1, directDebitRemittance.DirectDebitPaymentInstructions[1].NumberOfDirectDebitTransactions);
            Assert.AreEqual(79, directDebitRemittance.DirectDebitPaymentInstructions[1].TotalAmount);
        }












        //[TestMethod]
        //public void TheNumberOfTransactionsAndControlSumAreCorrectlyUpdatedInAPaymentInstructionWhenAddingABillToATransaction()
        //{
        //    DirectDebitRemittancesManager directDebitRemittancesManager = new DirectDebitRemittancesManager();

        //    List<SimplifiedBill> billsList = new List<SimplifiedBill>() { debtors["00002"].SimplifiedBills.ElementAt(0).Value };
        //    string transactionID = "PaymentInstruction1-00002";
        //    DirectDebitMandate directDebitMandate = debtors["00002"].DirectDebitmandates[1235];
        //    string mandateID = directDebitPropietaryCodesGenerator.CalculateMyOldCSB19MandateID(directDebitMandate.InternalReferenceNumber);
        //    DirectDebitTransaction directDebitTransaction = directDebitRemittancesManager.CreateADirectDebitTransactionFromAGroupOfBills(
        //        transactionID,
        //        mandateID,
        //        directDebitMandate,
        //        billsList,
        //        null);
        //    SimplifiedBill bill = debtors["00002"].SimplifiedBills.ElementAt(1).Value;

        //    directDebitRemittancesManager.AddBilllToExistingDirectDebitTransaction(directDebitTransaction, bill);

        //    Assert.AreEqual(2, directDebitTransaction.NumberOfBills);
        //    Assert.AreEqual(158, directDebitTransaction.Amount);
        //    CollectionAssert.AreEqual(billsList, directDebitTransaction.BillsInTransaction);
        //}

        //[TestMethod]
        //public void TheNumberOfTransactionsAndControlSumAreCorrectlyUpdatedInARemmitanceWhenAddingABillToATransaction()
        //{
        //    DirectDebitRemittancesManager directDebitRemittancesManager = new DirectDebitRemittancesManager();

        //    List<SimplifiedBill> billsList = new List<SimplifiedBill>() { debtors["00002"].SimplifiedBills.ElementAt(0).Value };
        //    string transactionID = "PaymentInstruction1-00002";
        //    DirectDebitMandate directDebitMandate = debtors["00002"].DirectDebitmandates[1235];
        //    string mandateID = directDebitPropietaryCodesGenerator.CalculateMyOldCSB19MandateID(directDebitMandate.InternalReferenceNumber);
        //    DirectDebitTransaction directDebitTransaction = directDebitRemittancesManager.CreateADirectDebitTransactionFromAGroupOfBills(
        //        transactionID,
        //        mandateID,
        //        directDebitMandate,
        //        billsList,
        //        null);
        //    SimplifiedBill bill = debtors["00002"].SimplifiedBills.ElementAt(1).Value;

        //    directDebitRemittancesManager.AddBilllToExistingDirectDebitTransaction(directDebitTransaction, bill);

        //    Assert.AreEqual(2, directDebitTransaction.NumberOfBills);
        //    Assert.AreEqual(158, directDebitTransaction.Amount);
        //    CollectionAssert.AreEqual(billsList, directDebitTransaction.BillsInTransaction);
        //}
    }
}
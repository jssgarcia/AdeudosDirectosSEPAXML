﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DirectDebitElements;
using Billing;
using ReferencesAndTools;
using ExtensionMethods;

namespace DirectDebitElementsUnitTests
{
    [TestClass]
    public class DirectDebitRemmitanceUnitTest
    {
        static Dictionary<string, Debtor> debtors;
        static Creditor creditor;
        static CreditorAgent creditorAgent;
        static DirectDebitInitiationContract directDebitInitiationContract;
        static DirectDebitPropietaryCodesGenerator directDebitPropietaryCodesGenerator;
        static BankCodes spanishBankCodes;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            debtors = new Dictionary<string, Debtor>()
            {
                {"00001", new Debtor("00001", "Francisco", "Gómez-Caldito", "Viseas")},
                {"00002", new Debtor("00002", "Pedro", "Pérez", "Gómez")}
            };

            creditor = new Creditor("G12345678", "NOMBRE ACREEDOR PRUEBAS");
            creditorAgent = new CreditorAgent(new BankCode("2100", "CaixaBank","CAIXESBBXXX"));
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
                    new DateTime(2013,11,11),
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

            spanishBankCodes = new BankCodes(@"XMLFiles\SpanishBankCodes.xml", BankCodes.BankCodesFileFormat.XML);
        }

        [TestMethod]
        public void ADirectDebittRemmitanceInstanceIsCorrectlyCreated()
        {
            DateTime creationDate = new DateTime(2013, 11, 30, 7, 15, 0);
            string messageID = "ES26777G12345678" + creationDate.ToString("yyyyMMddHH:mm:ss");
            DateTime requestedCollectionDate = new DateTime(2013, 12, 1);

            DirectDebitRemittance directDebitRemmitance = new DirectDebitRemittance(messageID, creationDate, requestedCollectionDate, directDebitInitiationContract);

            string expectedMessageId = "ES26777G123456782013113007:15:00";
            Assert.AreEqual(expectedMessageId, directDebitRemmitance.MessageID);
            Assert.AreEqual(creationDate, directDebitRemmitance.CreationDate);
            Assert.AreEqual(requestedCollectionDate, directDebitRemmitance.RequestedCollectionDate);
        }

        [TestMethod]
        public void AnEmptyDirectDebitTransactionIsCorrectlyCreated()
        {
            Debtor debtor = debtors["00002"];
            DirectDebitMandate directDebitMandate = debtors["00002"].DirectDebitmandates.ElementAt(0).Value;
            string internalUniqueInstructionID = "00001";
            string mandateID = directDebitPropietaryCodesGenerator.CalculateMyOldCSB19MandateID(directDebitMandate.InternalReferenceNumber);
            DateTime mandateSignatureDate = directDebitMandate.DirectDebitMandateCreationDate;
            BankAccount debtorAccount = directDebitMandate.BankAccount;
            string accountHolderName = directDebitMandate.AccountHolderName; 

            DirectDebitTransaction directDebitTransaction = new DirectDebitTransaction(
                new List<SimplifiedBill>(),
                internalUniqueInstructionID,
                mandateID,
                mandateSignatureDate,
                debtorAccount,
                accountHolderName,
                null);

            List<SimplifiedBill> expectedEmptyList = new List<SimplifiedBill>();
            Assert.AreEqual(internalUniqueInstructionID, directDebitTransaction.InternalUniqueInstructionID);
            Assert.AreEqual("000077701235", directDebitTransaction.MandateID);
            Assert.AreEqual(mandateSignatureDate, directDebitTransaction.MandateSigatureDate);
            Assert.AreEqual(debtorAccount, directDebitTransaction.DebtorAccount);
            Assert.AreEqual(accountHolderName, directDebitTransaction.AccountHolderName);
            CollectionAssert.AreEqual(expectedEmptyList, directDebitTransaction.BillsInTransaction);
            Assert.AreEqual(0, directDebitTransaction.NumberOfBills);
            Assert.AreEqual(0, directDebitTransaction.Amount);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void MessageIDCantBeNull()
        {
            DateTime creationDate = new DateTime(2013, 11, 30, 7, 15, 0);
            string messageID = null;
            DateTime requestedCollectionDate = new DateTime(2013, 12, 1);

            try
            {
                DirectDebitRemittance directDebitRemmitance = new DirectDebitRemittance(
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
        public void MessageIDCantBeEmpty()
        {
            DateTime creationDate = new DateTime(2013, 11, 30, 7, 15, 0);
            string messageID = "";
            DateTime requestedCollectionDate = new DateTime(2013, 12, 1);

            try
            {
                DirectDebitRemittance directDebitRemmitance = new DirectDebitRemittance(
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
        public void MessageIDCantBeLongerThan35Characters()
        {
            DateTime creationDate = new DateTime(2013, 11, 30, 7, 15, 0);
            string messageID = "1234567890123456789012345678901234567890";
            DateTime requestedCollectionDate = new DateTime(2013, 12, 1);

            try
            {
                DirectDebitRemittance directDebitRemmitance = new DirectDebitRemittance(
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
        public void DirectDebitInitiationContractCantBeNullInADirectDebitRemmitance()
        {
            DateTime creationDate = new DateTime(2013, 11, 30, 7, 15, 0);
            string messageID = "ES26777G123456782013113007:15:00";
            DateTime requestedCollectionDate = new DateTime(2013, 12, 1);

            try
            {
                DirectDebitRemittance directDebitRemmitance = new DirectDebitRemittance(
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
        public void ADirectDebitTransactionIsCorrectlyCreated()
        {
            Debtor debtor = debtors["00002"];
            DirectDebitMandate directDebitMandate = debtors["00002"].DirectDebitmandates.ElementAt(0).Value;
            string internalUniqueInstructionID = "00001";
            string mandateID = directDebitPropietaryCodesGenerator.CalculateMyOldCSB19MandateID(directDebitMandate.InternalReferenceNumber);
            DateTime mandateSignatureDate = directDebitMandate.DirectDebitMandateCreationDate;
            BankAccount debtorAccount = directDebitMandate.BankAccount;
            string accountHolderName = directDebitMandate.AccountHolderName;
            List<SimplifiedBill> bills = debtor.SimplifiedBills.Values.ToList();

            DirectDebitTransaction directDebitTransaction = new DirectDebitTransaction(
                bills,
                internalUniqueInstructionID,
                mandateID,
                mandateSignatureDate,
                debtorAccount,
                accountHolderName,
                null);

            Assert.AreEqual(internalUniqueInstructionID, directDebitTransaction.InternalUniqueInstructionID);
            Assert.AreEqual("000077701235", directDebitTransaction.MandateID);
            Assert.AreEqual(mandateSignatureDate, directDebitTransaction.MandateSigatureDate);
            Assert.AreEqual(debtorAccount, directDebitTransaction.DebtorAccount);
            Assert.AreEqual(accountHolderName, directDebitTransaction.AccountHolderName);
            Assert.AreEqual(bills, directDebitTransaction.BillsInTransaction);
            Assert.AreEqual(2, directDebitTransaction.NumberOfBills);
            Assert.AreEqual(158, directDebitTransaction.Amount);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void InternalUniqueInstructionIDCantBeNull()
        {
            Debtor debtor = debtors["00002"];
            DirectDebitMandate directDebitMandate = debtors["00002"].DirectDebitmandates.ElementAt(0).Value;
            string internalUniqueInstructionID = null;
            string mandateID = directDebitPropietaryCodesGenerator.CalculateMyOldCSB19MandateID(directDebitMandate.InternalReferenceNumber);
            DateTime mandateSignatureDate = directDebitMandate.DirectDebitMandateCreationDate;
            BankAccount debtorAccount = directDebitMandate.BankAccount;
            string accountHolderName = directDebitMandate.AccountHolderName;
            List<SimplifiedBill> bills = debtor.SimplifiedBills.Values.ToList();

            try
            {
                DirectDebitTransaction directDebitTransaction = new DirectDebitTransaction(
                    bills,
                    internalUniqueInstructionID,
                    mandateID,
                    mandateSignatureDate,
                    debtorAccount,
                    accountHolderName,
                    null);
            }

            catch (System.ArgumentNullException e)
            {
                Assert.AreEqual("InternalUniqueInstructionID", e.ParamName);
                Assert.AreEqual("InternalUniqueInstructionID can't be null", e.GetMessageWithoutParamName());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void InternalUniqueInstructionIDCantBeEmpty()
        {
            Debtor debtor = debtors["00002"];
            DirectDebitMandate directDebitMandate = debtors["00002"].DirectDebitmandates.ElementAt(0).Value;
            string internalUniqueInstructionID = "";
            string mandateID = directDebitPropietaryCodesGenerator.CalculateMyOldCSB19MandateID(directDebitMandate.InternalReferenceNumber);
            DateTime mandateSignatureDate = directDebitMandate.DirectDebitMandateCreationDate;
            BankAccount debtorAccount = directDebitMandate.BankAccount;
            string accountHolderName = directDebitMandate.AccountHolderName;
            List<SimplifiedBill> bills = debtor.SimplifiedBills.Values.ToList();

            try
            {
                DirectDebitTransaction directDebitTransaction = new DirectDebitTransaction(
                    bills,
                    internalUniqueInstructionID,
                    mandateID,
                    mandateSignatureDate,
                    debtorAccount,
                    accountHolderName,
                    null);
            }

            catch (System.ArgumentException e)
            {
                Assert.AreEqual("InternalUniqueInstructionID", e.ParamName);
                Assert.AreEqual("InternalUniqueInstructionID can't be empty", e.GetMessageWithoutParamName());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void InternalUniqueInstructionIDCantBeOnlySpaces()
        {
            Debtor debtor = debtors["00002"];
            DirectDebitMandate directDebitMandate = debtors["00002"].DirectDebitmandates.ElementAt(0).Value;
            string internalUniqueInstructionID = "   ";
            string mandateID = directDebitPropietaryCodesGenerator.CalculateMyOldCSB19MandateID(directDebitMandate.InternalReferenceNumber);
            DateTime mandateSignatureDate = directDebitMandate.DirectDebitMandateCreationDate;
            BankAccount debtorAccount = directDebitMandate.BankAccount;
            string accountHolderName = directDebitMandate.AccountHolderName;
            List<SimplifiedBill> bills = debtor.SimplifiedBills.Values.ToList();

            try
            {
                DirectDebitTransaction directDebitTransaction = new DirectDebitTransaction(
                    bills,
                    internalUniqueInstructionID,
                    mandateID,
                    mandateSignatureDate,
                    debtorAccount,
                    accountHolderName,
                    null);
            }

            catch (System.ArgumentException e)
            {
                Assert.AreEqual("InternalUniqueInstructionID", e.ParamName);
                Assert.AreEqual("InternalUniqueInstructionID can't be empty", e.GetMessageWithoutParamName());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentOutOfRangeException))]
        public void InternalUniqueInstructionIDCantBeLongerThan35Characters()
        {
            Debtor debtor = debtors["00002"];
            DirectDebitMandate directDebitMandate = debtors["00002"].DirectDebitmandates.ElementAt(0).Value;
            string internalUniqueInstructionID = "1234567890123456789012345678901234567890";
            string mandateID = directDebitPropietaryCodesGenerator.CalculateMyOldCSB19MandateID(directDebitMandate.InternalReferenceNumber);
            DateTime mandateSignatureDate = directDebitMandate.DirectDebitMandateCreationDate;
            BankAccount debtorAccount = directDebitMandate.BankAccount;
            string accountHolderName = directDebitMandate.AccountHolderName;
            List<SimplifiedBill> bills = debtor.SimplifiedBills.Values.ToList();

            try
            {
                DirectDebitTransaction directDebitTransaction = new DirectDebitTransaction(
                    bills,
                    internalUniqueInstructionID,
                    mandateID,
                    mandateSignatureDate,
                    debtorAccount,
                    accountHolderName,
                    null);
            }

            catch (System.ArgumentOutOfRangeException e)
            {
                Assert.AreEqual("InternalUniqueInstructionID", e.ParamName);
                Assert.AreEqual("InternalUniqueInstructionID can't be longer than 35 characters", e.GetMessageWithoutParamName());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void MandateIDCantBeNull()
        {
            Debtor debtor = debtors["00002"];
            DirectDebitMandate directDebitMandate = debtors["00002"].DirectDebitmandates.ElementAt(0).Value;
            string internalUniqueInstructionID = "00001";
            string mandateID = null;
            DateTime mandateSignatureDate = directDebitMandate.DirectDebitMandateCreationDate;
            BankAccount debtorAccount = directDebitMandate.BankAccount;
            string accountHolderName = directDebitMandate.AccountHolderName;
            List<SimplifiedBill> bills = debtor.SimplifiedBills.Values.ToList();

            try
            {
                DirectDebitTransaction directDebitTransaction = new DirectDebitTransaction(
                    bills,
                    internalUniqueInstructionID,
                    mandateID,
                    mandateSignatureDate,
                    debtorAccount,
                    accountHolderName,
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
        public void MandateIDCantBeEmpty()
        {
            Debtor debtor = debtors["00002"];
            DirectDebitMandate directDebitMandate = debtors["00002"].DirectDebitmandates.ElementAt(0).Value;
            string internalUniqueInstructionID = "00001";
            string mandateID = "";
            DateTime mandateSignatureDate = directDebitMandate.DirectDebitMandateCreationDate;
            BankAccount debtorAccount = directDebitMandate.BankAccount;
            string accountHolderName = directDebitMandate.AccountHolderName;
            List<SimplifiedBill> bills = debtor.SimplifiedBills.Values.ToList();

            try
            {
                DirectDebitTransaction directDebitTransaction = new DirectDebitTransaction(
                    bills,
                    internalUniqueInstructionID,
                    mandateID,
                    mandateSignatureDate,
                    debtorAccount,
                    accountHolderName,
                    null);
            }

            catch (System.ArgumentException e)
            {
                Assert.AreEqual("MandateID", e.ParamName);
                Assert.AreEqual("MandateID can't be empty", e.GetMessageWithoutParamName());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void MandateIDCantBeOnlySpaces()
        {
            Debtor debtor = debtors["00002"];
            DirectDebitMandate directDebitMandate = debtors["00002"].DirectDebitmandates.ElementAt(0).Value;
            string internalUniqueInstructionID = "00001";
            string mandateID = " ";
            DateTime mandateSignatureDate = directDebitMandate.DirectDebitMandateCreationDate;
            BankAccount debtorAccount = directDebitMandate.BankAccount;
            string accountHolderName = directDebitMandate.AccountHolderName;
            List<SimplifiedBill> bills = debtor.SimplifiedBills.Values.ToList();

            try
            {
                DirectDebitTransaction directDebitTransaction = new DirectDebitTransaction(
                    bills,
                    internalUniqueInstructionID,
                    mandateID,
                    mandateSignatureDate,
                    debtorAccount,
                    accountHolderName,
                    null);
            }

            catch (System.ArgumentException e)
            {
                Assert.AreEqual("MandateID", e.ParamName);
                Assert.AreEqual("MandateID can't be empty", e.GetMessageWithoutParamName());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentOutOfRangeException))]
        public void MandateIDCantBeLongerThan35Characters()
        {
            Debtor debtor = debtors["00002"];
            DirectDebitMandate directDebitMandate = debtors["00002"].DirectDebitmandates.ElementAt(0).Value;
            string internalUniqueInstructionID = "00001";
            string mandateID = "1234567890123456789012345678901234567890";
            DateTime mandateSignatureDate = directDebitMandate.DirectDebitMandateCreationDate;
            BankAccount debtorAccount = directDebitMandate.BankAccount;
            string accountHolderName = directDebitMandate.AccountHolderName;
            List<SimplifiedBill> bills = debtor.SimplifiedBills.Values.ToList();

            try
            {
                DirectDebitTransaction directDebitTransaction = new DirectDebitTransaction(
                    bills,
                    internalUniqueInstructionID,
                    mandateID,
                    mandateSignatureDate,
                    debtorAccount,
                    accountHolderName,
                    null);
            }

            catch (System.ArgumentOutOfRangeException e)
            {
                Assert.AreEqual("MandateID", e.ParamName);
                Assert.AreEqual("MandateID can't be longer than 35 characters", e.GetMessageWithoutParamName());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void DebtorAccountCantBeNullInADirectDebitTrasaction()
        {
            Debtor debtor = debtors["00002"];
            DirectDebitMandate directDebitMandate = debtors["00002"].DirectDebitmandates.ElementAt(0).Value;
            string internalUniqueInstructionID = "00001";
            string mandateID = directDebitPropietaryCodesGenerator.CalculateMyOldCSB19MandateID(directDebitMandate.InternalReferenceNumber);
            DateTime mandateSignatureDate = directDebitMandate.DirectDebitMandateCreationDate;
            BankAccount debtorAccount = null;
            string accountHolderName = directDebitMandate.AccountHolderName;
            List<SimplifiedBill> bills = debtor.SimplifiedBills.Values.ToList();

            try
            {
                DirectDebitTransaction directDebitTransaction = new DirectDebitTransaction(
                    bills,
                    internalUniqueInstructionID,
                    mandateID,
                    mandateSignatureDate,
                    debtorAccount,
                    accountHolderName,
                    null);
            }

            catch (System.ArgumentNullException e)
            {
                Assert.AreEqual("DebtorAccount", e.ParamName);
                Assert.AreEqual("DebtorAccount can't be null", e.GetMessageWithoutParamName());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void DebtorAccountIBANCantBeinvalidInADirectDebitTrasaction()
        {
            Debtor debtor = debtors["00002"];
            DirectDebitMandate directDebitMandate = debtors["00002"].DirectDebitmandates.ElementAt(0).Value;
            string internalUniqueInstructionID = "00001";
            string mandateID = directDebitPropietaryCodesGenerator.CalculateMyOldCSB19MandateID(directDebitMandate.InternalReferenceNumber);
            DateTime mandateSignatureDate = directDebitMandate.DirectDebitMandateCreationDate;
            BankAccount debtorAccount = new BankAccount(new BankAccountFields("1111", "1111", "11", "1111111111"));
            string accountHolderName = directDebitMandate.AccountHolderName;
            List<SimplifiedBill> bills = debtor.SimplifiedBills.Values.ToList();

            try
            {
                DirectDebitTransaction directDebitTransaction = new DirectDebitTransaction(
                    bills,
                    internalUniqueInstructionID,
                    mandateID,
                    mandateSignatureDate,
                    debtorAccount,
                    accountHolderName,
                    null);
            }

            catch (System.ArgumentNullException e)
            {
                Assert.AreEqual("DebtorAccount", e.ParamName);
                Assert.AreEqual("DebtorAccount must be a valid IBAN", e.GetMessageWithoutParamName());
                throw;
            }
        }

        [TestMethod]
        public void WhenAddingAnotherBillToADirectDebitTransactionTheAmmountAndNumberOfBillsAreCorrectlyUpdated()
        {
            Debtor debtor = debtors["00002"];
            DirectDebitMandate directDebitMandate = debtors["00002"].DirectDebitmandates.ElementAt(0).Value;
            int internalDirectDebitReferenceNumber = directDebitMandate.InternalReferenceNumber;
            string internalUniqueInstructionID = "00001";
            string mandateID = directDebitPropietaryCodesGenerator.CalculateMyOldCSB19MandateID(directDebitMandate.InternalReferenceNumber);
            DateTime mandateSignatureDate = directDebitMandate.DirectDebitMandateCreationDate;
            BankAccount debtorAccount = directDebitMandate.BankAccount;
            string accountHolderName = directDebitMandate.AccountHolderName;
            List<SimplifiedBill> bills = new List<SimplifiedBill> { debtor.SimplifiedBills.ElementAt(0).Value };
            DirectDebitTransaction directDebitTransaction = new DirectDebitTransaction(
                bills,
                internalUniqueInstructionID,
                mandateID,
                mandateSignatureDate,
                debtorAccount,
                accountHolderName,
                null);
            Assert.AreEqual((decimal)79, directDebitTransaction.Amount);
            Assert.AreEqual(1, directDebitTransaction.NumberOfBills);
            SimplifiedBill bill = debtor.SimplifiedBills.ElementAt(1).Value;

            directDebitTransaction.AddBill(bill);

            Assert.AreEqual((decimal)158, directDebitTransaction.Amount);
            Assert.AreEqual(2, directDebitTransaction.NumberOfBills);
        }

        [TestMethod]
        public void ADirecDebitTransactionGroupPaymnetIsCorrectlyCreated()
        {
            string paymentInformationID = "PaymentGroup1";
            string localInstrument = "COR1";
            DirectDebitTransactionsGroupPayment dDTxGrpPaymentInfo = new DirectDebitTransactionsGroupPayment(paymentInformationID, localInstrument);
            Assert.AreEqual("PaymentGroup1", dDTxGrpPaymentInfo.PaymentInformationID);
            Assert.AreEqual("COR1", dDTxGrpPaymentInfo.LocalInstrument);
            Assert.AreEqual(0, dDTxGrpPaymentInfo.NumberOfDirectDebitTransactions);
            Assert.AreEqual(0, dDTxGrpPaymentInfo.TotalAmount);
        }

        [TestMethod]
        public void ADirectDebitTransactionIsCorrectlyAddedToGroupPayment()
        {
            string paymentInformationID = "PaymentGroup1";
            string localInstrument = "COR1";
            DirectDebitTransactionsGroupPayment directDebitTransactionsGroupPayment = new DirectDebitTransactionsGroupPayment(paymentInformationID, localInstrument);
            Debtor debtor = debtors["00002"];
            DirectDebitMandate directDebitMandate = debtors["00002"].DirectDebitmandates.ElementAt(0).Value;
            int internalDirectDebitReferenceNumber = directDebitMandate.InternalReferenceNumber;
            string internalUniqueInstructionID = "00001";
            string mandateID = directDebitPropietaryCodesGenerator.CalculateMyOldCSB19MandateID(directDebitMandate.InternalReferenceNumber);
            DateTime mandateSignatureDate = directDebitMandate.DirectDebitMandateCreationDate;
            BankAccount debtorAccount = directDebitMandate.BankAccount;
            string accountHolderName = directDebitMandate.AccountHolderName;
            List<SimplifiedBill> bills = new List<SimplifiedBill> { debtor.SimplifiedBills.ElementAt(0).Value };
            DirectDebitTransaction directDebitTransaction = new DirectDebitTransaction(
                bills,
                internalUniqueInstructionID,
                mandateID,
                mandateSignatureDate,
                debtorAccount,
                accountHolderName,
                null);

            directDebitTransactionsGroupPayment.AddDirectDebitTransaction(directDebitTransaction);

            Assert.AreEqual(1, directDebitTransactionsGroupPayment.NumberOfDirectDebitTransactions);
            Assert.AreEqual((decimal)79, directDebitTransactionsGroupPayment.TotalAmount);
        }

        [TestMethod]
        public void APaymentGroupIsCorrectlyAddedToADirectDebitRemmitance()
        {
            DateTime creationDate = new DateTime(2013, 11, 30, 7, 15, 0);
            string messageID = "ES26777G12345678" + creationDate.ToString("yyyyMMddHH:mm:ss");
            DateTime requestedCollectionDate = new DateTime(2013, 12,1);
            DirectDebitRemittance directDebitRemmitance = new DirectDebitRemittance(messageID, creationDate, requestedCollectionDate, directDebitInitiationContract);
            string paymentInformationID = "PaymentGroup1";
            string localInstrument = "COR1";
            DirectDebitTransactionsGroupPayment directDebitTransactionsGroupPayment = new DirectDebitTransactionsGroupPayment(paymentInformationID, localInstrument);
            directDebitRemmitance.AddDirectDebitTransactionsGroupPayment(directDebitTransactionsGroupPayment);
            Assert.AreEqual(1, directDebitRemmitance.DirectDebitTransactionGroupPaymentCollection.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentOutOfRangeException))]
        public void CantAssignAPaymentInformationIDLongerThan35Characters()
        {
            string paymentInformationID = "0123456789012345678901234567890123456789";
            try
            {
                DirectDebitTransactionsGroupPayment directDebitTransactionsGroupPayment = new DirectDebitTransactionsGroupPayment(paymentInformationID, "COR1");
            }

            catch (System.ArgumentOutOfRangeException e)
            {
                Assert.AreEqual("PaymentInformationID", e.ParamName);
                Assert.AreEqual("PaymentInformationID lenght can't exceed 35 characters", e.GetMessageWithoutParamName());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void CantAssignAnEmptyPaymentInformationID()
        {
            try
            {
                DirectDebitTransactionsGroupPayment directDebitTransactionsGroupPayment = new DirectDebitTransactionsGroupPayment("", "COR1");
            }

            catch (System.ArgumentException e)
            {
                Assert.AreEqual("PaymentInformationID", e.ParamName);
                Assert.AreEqual("PaymentInformationID lenght can't be empty", e.GetMessageWithoutParamName());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void CantAssignANullPaymentInformationID()
        {
            try
            {
                DirectDebitTransactionsGroupPayment directDebitTransactionsGroupPayment = new DirectDebitTransactionsGroupPayment(null, "COR1");
            }

            catch (System.ArgumentNullException e)
            {
                Assert.AreEqual("PaymentInformationID", e.ParamName);
                Assert.AreEqual("PaymentInformationID can't be null", e.GetMessageWithoutParamName());
                throw;
            }
        }

        [TestMethod]
        public void ADirectDebitAmendmentInformationIsCorrectlyCreated()
        {
            string oldMandateID = "112211";
            BankAccount oldBankAccount = new BankAccount(new InternationalAccountBankNumberIBAN("ES5621001111301111111111"));

            DirectDebitAmendmentInformation directDebitAmendmentInformation = new DirectDebitAmendmentInformation(oldMandateID, oldBankAccount);

            Assert.AreEqual(oldMandateID, directDebitAmendmentInformation.OldMandateID);
            Assert.AreEqual(oldBankAccount, directDebitAmendmentInformation.OldBankAccount);
        }

        [TestMethod]
        public void TheMandateIDofAnAmendmentInformationCanBeNull()
        {
            string oldMandateID = null;
            BankAccount oldBankAccount = new BankAccount(new InternationalAccountBankNumberIBAN("ES5621001111301111111111"));

            DirectDebitAmendmentInformation directDebitAmendmentInformation = new DirectDebitAmendmentInformation(oldMandateID, oldBankAccount);

            Assert.AreEqual(null, directDebitAmendmentInformation.OldMandateID);
            Assert.AreEqual(oldBankAccount, directDebitAmendmentInformation.OldBankAccount);
        }

        [TestMethod]
        public void TheBankAcountOfAnAmendmentInformationCanBeNull()
        {
            string oldMandateID = "112211";
            BankAccount oldBankAccount = null;

            DirectDebitAmendmentInformation directDebitAmendmentInformation = new DirectDebitAmendmentInformation(oldMandateID, oldBankAccount);

            Assert.AreEqual(oldMandateID, directDebitAmendmentInformation.OldMandateID);
            Assert.AreEqual(null, directDebitAmendmentInformation.OldBankAccount);
        }

        [TestMethod]
        [ExpectedException(typeof(System.TypeInitializationException))]
        public void TheMandateIDOfAnAmendmentInformationCantBeEmptyOrOnlySpaces()
        {
            string oldMandateID = "   ";
            BankAccount oldBankAccount = null;

            try
            {
                DirectDebitAmendmentInformation directDebitAmendmentInformation = new DirectDebitAmendmentInformation(oldMandateID, oldBankAccount);
            }

            catch (TypeInitializationException typeInitializationException)
            {
                ArgumentException argumentException = (ArgumentException)typeInitializationException.InnerException;
                Assert.AreEqual("mandateID", argumentException.ParamName);
                Assert.AreEqual("The MandateID can't be empty", argumentException.GetMessageWithoutParamName());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(System.TypeInitializationException))]
        public void TheBankAccountOfAndAmendmentInformationCantBeInvalid()
        {
            string oldMandateID = null;
            BankAccount oldBankAccount = new BankAccount(new BankAccountFields("1111", "1111", "11", "1111111111"));

            try
            {
                DirectDebitAmendmentInformation directDebitAmendmentInformation = new DirectDebitAmendmentInformation(oldMandateID, oldBankAccount);
            }

            catch (TypeInitializationException typeInitializationException)
            {
                ArgumentException argumentException = (ArgumentException)typeInitializationException.InnerException;
                Assert.AreEqual("oldBankAccount", argumentException.ParamName);
                Assert.AreEqual("The Bank Account must have a valid IBAN", argumentException.GetMessageWithoutParamName());
                throw;
            }
        }
    }
}

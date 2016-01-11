﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Billing;
using DirectDebitElements;
using ReferencesAndTools;
using ISO20022PaymentInitiations.SchemaSerializableClasses;
using ISO20022PaymentInitiations.SchemaSerializableClasses.DDInitiation;
using ExtensionMethods;

namespace DirectDebitElementsUnitTests
{
    [TestClass]
    public class SEPAElementsGeneratorUnitTests
    {

        static Dictionary<string, Debtor> debtors;
        static Creditor creditor;
        static CreditorAgent creditorAgent;
        static DirectDebitInitiationContract directDebitInitiationContract;
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
            creditorAgent = new CreditorAgent(new BankCode("2100", "CaixaBank", "CAIXESBBXXX"));
            directDebitInitiationContract = new DirectDebitInitiationContract(
                new BankAccount(new InternationalAccountBankNumberIBAN("ES5621001111301111111111")),
                creditor.NIF,
                "777",
                creditorAgent);

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
        }

        [TestMethod]
        public void APartyIdentification32IsCorrectlyGenerated()
        {
            Creditor creditor = new Creditor("G12345678", "NOMBRE ACREEDOR PRUEBAS");
            BankAccount creditorAccount = new BankAccount(new InternationalAccountBankNumberIBAN("ES5621001111301111111111"));
            BankCode bankCode = new BankCode("2100", "CaixaBank", "CAIXESBBXXX");
            CreditorAgent creditorAgent = new CreditorAgent(bankCode);
            DirectDebitInitiationContract directDebitInitiationContract = new DirectDebitInitiationContract(creditorAccount, "G12345678", "011", creditorAgent);

            PartyIdentification32 initiationParty_InitPty = SEPAElementsGenerator.GenerateInitiationParty_InitPty(
                creditor,
                directDebitInitiationContract);

            Assert.AreEqual("NOMBRE ACREEDOR PRUEBAS",initiationParty_InitPty.Nm);
            Assert.AreEqual("ES26011G12345678", ((OrganisationIdentification4)initiationParty_InitPty.Id.Item).Othr[0].Id);

            Assert.IsNull(initiationParty_InitPty.CtctDtls);
            Assert.IsNull(initiationParty_InitPty.CtryOfRes);
            Assert.IsNull(initiationParty_InitPty.PstlAdr);
        }

        [TestMethod]
        public void AGroupHeader39IsCorrectlyGenerated()
        {
            string messageID = "ES26011G123456782015011007:15:00";
            DateTime generationDateTime = DateTime.Now;
            int numberOfTransactions = 2;
            decimal controlSum = 158;

            Creditor creditor = new Creditor("G12345678", "NOMBRE ACREEDOR PRUEBAS");
            BankAccount creditorAccount = new BankAccount(new InternationalAccountBankNumberIBAN("ES5621001111301111111111"));
            BankCode bankCode = new BankCode("2100", "CaixaBank", "CAIXESBBXXX");
            CreditorAgent creditorAgent = new CreditorAgent(bankCode);
            DirectDebitInitiationContract directDebitInitiationContract = new DirectDebitInitiationContract(
                creditorAccount,
                "G12345678",
                "011",
                creditorAgent);
            PartyIdentification32 initiationParty_InitPty = SEPAElementsGenerator.GenerateInitiationParty_InitPty(
                creditor,
                directDebitInitiationContract);

            GroupHeader39 groupHeader_GrpHdr = SEPAElementsGenerator.GenerateGroupHeader_GrpHdr(
                messageID,
                generationDateTime,
                numberOfTransactions,
                controlSum,
                initiationParty_InitPty);

            DateTime truncatedToSecondsGenerationDateTime = DateTime.SpecifyKind(generationDateTime, DateTimeKind.Unspecified).Truncate(TimeSpan.FromSeconds(1));
            Assert.AreEqual("ES26011G123456782015011007:15:00", groupHeader_GrpHdr.MsgId);
            Assert.AreEqual(truncatedToSecondsGenerationDateTime, groupHeader_GrpHdr.CreDtTm);
            Assert.IsTrue(groupHeader_GrpHdr.CtrlSumSpecified);
            Assert.AreEqual(numberOfTransactions.ToString(),groupHeader_GrpHdr.NbOfTxs);
            Assert.AreEqual(controlSum, groupHeader_GrpHdr.CtrlSum);
            Assert.AreEqual(initiationParty_InitPty, groupHeader_GrpHdr.InitgPty);

            CollectionAssert.AreEqual(new Authorisation1Choice[] { null }, groupHeader_GrpHdr.Authstn);
            Assert.IsNull(groupHeader_GrpHdr.FwdgAgt);
        }

        [TestMethod]
        public void ADirectDebitTransactionInformation9IsCorrectlyGenerated()
        {
            BankCode bankCode = new BankCode("2100", "CaixaBank", "CAIXESBBXXX");
            CreditorAgent creditorAgent = new CreditorAgent(bankCode);

            //DirectDebitTransaction directDebitTransaction = new DirectDebitTransaction()


            //DateTime truncatedToSecondsGenerationDateTime = DateTime.SpecifyKind(generationDateTime, DateTimeKind.Unspecified).Truncate(TimeSpan.FromSeconds(1));
            //Assert.AreEqual("ES26011G123456782015011007:15:00", groupHeader_GrpHdr.MsgId);
            //Assert.AreEqual(truncatedToSecondsGenerationDateTime, groupHeader_GrpHdr.CreDtTm);
            //Assert.IsTrue(groupHeader_GrpHdr.CtrlSumSpecified);
            //Assert.AreEqual(numberOfTransactions.ToString(), groupHeader_GrpHdr.NbOfTxs);
            //Assert.AreEqual(controlSum, groupHeader_GrpHdr.CtrlSum);
            //Assert.AreEqual(initiationParty_InitPty, groupHeader_GrpHdr.InitgPty);

            //CollectionAssert.AreEqual(new Authorisation1Choice[] { null }, groupHeader_GrpHdr.Authstn);
            //Assert.IsNull(groupHeader_GrpHdr.FwdgAgt);
        }



    }
}

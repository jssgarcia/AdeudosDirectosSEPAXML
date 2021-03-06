﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:1.9.0.77
//      SpecFlow Generator Version:1.9.0.0
//      Runtime Version:4.0.30319.42000
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace AdeudosDirectosSEPAXMLSpecFlowBDD
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "1.9.0.77")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute()]
    public partial class GeneratingDirectDebitRemittancesFeature
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "GeneratingDirectDebitRemittances.feature"
#line hidden
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.ClassInitializeAttribute()]
        public static void FeatureSetup(Microsoft.VisualStudio.TestTools.UnitTesting.TestContext testContext)
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Generating Direct Debit Remittances", "In order to charge the bills to my debtors\nAs an administrative assistant\nI want " +
                    "to generate direct debit Remittances to bank", ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.ClassCleanupAttribute()]
        public static void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestInitializeAttribute()]
        public virtual void TestInitialize()
        {
            if (((TechTalk.SpecFlow.FeatureContext.Current != null) 
                        && (TechTalk.SpecFlow.FeatureContext.Current.FeatureInfo.Title != "Generating Direct Debit Remittances")))
            {
                AdeudosDirectosSEPAXMLSpecFlowBDD.GeneratingDirectDebitRemittancesFeature.FeatureSetup(null);
            }
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCleanupAttribute()]
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioSetup(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioStart(scenarioInfo);
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        public virtual void FeatureBackground()
        {
#line 6
#line hidden
            TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                        "NIF",
                        "Name",
                        "BIC",
                        "CreditorAgentName",
                        "LocalBankCode",
                        "CreditorBussinesCode",
                        "CreditorAccount"});
            table1.AddRow(new string[] {
                        "G12345678",
                        "NOMBRE ACREEDOR PRUEBAS",
                        "CAIXESBBXXX",
                        "CAIXABANK",
                        "2100",
                        "777",
                        "ES5621001111301111111111"});
#line 8
 testRunner.Given("My Direct Debit Initiation Contract is", ((string)(null)), table1, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table2 = new TechTalk.SpecFlow.Table(new string[] {
                        "DebtorID",
                        "Name",
                        "FirstSurname",
                        "SecondSurname",
                        "Reference",
                        "Account",
                        "BIC"});
            table2.AddRow(new string[] {
                        "00001",
                        "Francisco",
                        "Gomez-Caldito",
                        "Viseas",
                        "1234",
                        "01821111601111111111",
                        "BBVAESMMXXX"});
            table2.AddRow(new string[] {
                        "00002",
                        "Pedro",
                        "Perez",
                        "Gomez",
                        "1235",
                        "21001111301111111111",
                        "CAIXESBBXXX"});
#line 12
 testRunner.Given("These debtors", ((string)(null)), table2, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table3 = new TechTalk.SpecFlow.Table(new string[] {
                        "DebtorID",
                        "TransactionConcept",
                        "Amount"});
            table3.AddRow(new string[] {
                        "00001",
                        "Cuota Mensual Octubre 2013",
                        "79"});
            table3.AddRow(new string[] {
                        "00002",
                        "Cuota Mensual Octubre 2013",
                        "79"});
            table3.AddRow(new string[] {
                        "00002",
                        "Cuota Mensual Noviembre 2013",
                        "79"});
#line 17
 testRunner.Given("These bills", ((string)(null)), table3, "Given ");
#line hidden
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Create a new direct debit Remittance")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Generating Direct Debit Remittances")]
        public virtual void CreateANewDirectDebitRemittance()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Create a new direct debit Remittance", ((string[])(null)));
#line 23
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 24
 testRunner.Given("I have a I have a direct debit initiation contract", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 25
 testRunner.When("I generate a new direct debit Remittance", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 26
 testRunner.Then("An empty direct debit Remittance is created", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Create an empty group of direct debit payments")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Generating Direct Debit Remittances")]
        public virtual void CreateAnEmptyGroupOfDirectDebitPayments()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Create an empty group of direct debit payments", ((string[])(null)));
#line 28
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 29
 testRunner.Given("I will send the payments using \"COR1\" local instrument", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 30
 testRunner.When("I generate an empty group of direct debit payments", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 31
 testRunner.Then("An empty group of direct debit payments using \"COR1\" is generated", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Create a Direct Debit Transaction from a bill as specified by a member direct deb" +
            "it mandate")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Generating Direct Debit Remittances")]
        public virtual void CreateADirectDebitTransactionFromABillAsSpecifiedByAMemberDirectDebitMandate()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Create a Direct Debit Transaction from a bill as specified by a member direct deb" +
                    "it mandate", ((string[])(null)));
#line 33
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 34
 testRunner.Given("I have a debtor", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 35
 testRunner.And("The debtor has a bill", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 36
 testRunner.And("The debtor has a Direct Debit Mandate", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 37
 testRunner.When("I generate Direct Debit Transaction", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 38
 testRunner.Then("The direct debit transaction is correctly created", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Add a second bill to a direct debit transaction")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Generating Direct Debit Remittances")]
        public virtual void AddASecondBillToADirectDebitTransaction()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Add a second bill to a direct debit transaction", ((string[])(null)));
#line 40
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 41
 testRunner.Given("I have a direct debit with 1 bill and amount of 80", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 42
 testRunner.When("I add a new bill with amount of 79", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 43
 testRunner.Then("The direct debit transaction is updated with 2 bills and amount of 159", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Add a direct debit transaction to an empty group of payments")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Generating Direct Debit Remittances")]
        public virtual void AddADirectDebitTransactionToAnEmptyGroupOfPayments()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Add a direct debit transaction to an empty group of payments", ((string[])(null)));
#line 45
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 46
 testRunner.Given("I have a direct debit with 1 bill and amount of 79", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 47
 testRunner.And("I have an empty group of payments", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 48
 testRunner.When("I add the direct debit transaction to the group of payments", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 49
 testRunner.Then("The group of payments is updated with 1 direct debit and total amount of 79", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Add a second direct debit transaction to a group of payments")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Generating Direct Debit Remittances")]
        public virtual void AddASecondDirectDebitTransactionToAGroupOfPayments()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Add a second direct debit transaction to a group of payments", ((string[])(null)));
#line 51
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 52
 testRunner.Given("I have a group of payments with 1 direct debit transaction and amount of 50", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 53
 testRunner.When("I add a new direct debit transaction with amount of 79", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 54
 testRunner.Then("The group of payments is updated with 2 direct debit and total amount of 129", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Add a group payments to a direct debit Remittance")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Generating Direct Debit Remittances")]
        public virtual void AddAGroupPaymentsToADirectDebitRemittance()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Add a group payments to a direct debit Remittance", ((string[])(null)));
#line 56
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 57
 testRunner.Given("I have an empty direct debit Remittance", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 58
 testRunner.And("I have a group of payments with 1 direct debit transaction and amount of 79", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 59
 testRunner.When("I add the group to the direct debit remittance", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 60
 testRunner.Then("The direct debit remittance is updated with 1 direct debit and total amount of 79" +
                    "", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Generating SEPA ISO20022 XML CustomerDirectDebitInitiation Message from a Direct " +
            "Debit Remittance")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Generating Direct Debit Remittances")]
        public virtual void GeneratingSEPAISO20022XMLCustomerDirectDebitInitiationMessageFromADirectDebitRemittance()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Generating SEPA ISO20022 XML CustomerDirectDebitInitiation Message from a Direct " +
                    "Debit Remittance", ((string[])(null)));
#line 62
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 63
 testRunner.Given("I have a prepared Direct Debit Remittance", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 64
 testRunner.When("I generate de SEPA ISO200022 XML CustomerDirectDebitInitiation message", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 65
 testRunner.Then("The message is correctly created", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion

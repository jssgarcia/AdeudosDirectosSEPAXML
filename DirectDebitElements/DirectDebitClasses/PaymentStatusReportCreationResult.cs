﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectDebitElements
{
    //
    //The use of this class has been discarded, but keeped (just in case...)
    //
    //public class PaymentStatusReportCreationResult
    //{
    //    PaymentStatusReport paymentStatusreport;
    //    List<string> errorMessages;

    //    public PaymentStatusReportCreationResult(string messageID, DateTime messageCreationDateTime, DateTime rejectAccountChargeDateTime, int numberOfTransactions, decimal controlSum, List<DirectDebitRemittanceReject> directDebitRemittanceRejects)
    //    {
    //        this.paymentStatusreport = new PaymentStatusReport(
    //            messageID,
    //            messageCreationDateTime,
    //            rejectAccountChargeDateTime,
    //            directDebitRemittanceRejects);
    //        CheckForErrors(numberOfTransactions, controlSum, directDebitRemittanceRejects);
    //    }

    //    private void CheckForErrors(int numberOfTransactions, decimal controlSum, List<DirectDebitRemittanceReject> directDebitRemittanceRejects)
    //    {
    //        int calculatedNumberOfTransactions = directDebitRemittanceRejects.Select(ddRemittanceRejects => ddRemittanceRejects.NumberOfTransactions).Sum();
    //        decimal calculatedControlSum = directDebitRemittanceRejects.Select(ddRemittanceRejects => ddRemittanceRejects.ControlSum).Sum();
    //        errorMessages = new List<string>();
    //        if (numberOfTransactions != calculatedNumberOfTransactions)
    //            errorMessages.Add(AddErroneousNumberOfTransactionsErrorMessage(numberOfTransactions, calculatedNumberOfTransactions));
    //        if (controlSum != calculatedControlSum)
    //            errorMessages.Add(AddErroneousControlSumErrorMessage(controlSum, calculatedControlSum));
    //    }

    //    private string AddErroneousNumberOfTransactionsErrorMessage(int providedNumberOfTransactions, int calculatedNumberOfTransactions)
    //    {
    //        string errorMessage =
    //            "The Number of Transactions is wrong. Provided: " + providedNumberOfTransactions.ToString() + ". Expected: " + calculatedNumberOfTransactions.ToString() + ". Initialized with expected value";
    //        return errorMessage;
    //    }

    //    private string AddErroneousControlSumErrorMessage(decimal providedControlSum, decimal calculatedControlSum)
    //    {
    //        string errorMessage =
    //            "The Control Sum is wrong. Provided: " + providedControlSum.ToString() + ". Expected: " + calculatedControlSum.ToString() + ". Initialized with expected value";
    //        return errorMessage;
    //    }

    //    public PaymentStatusReport PaymentStatusreport
    //    {
    //        get
    //        {
    //            return paymentStatusreport;
    //        }
    //    }

    //    public List<string> ErrorMessages
    //    {
    //        get
    //        {
    //            return errorMessages;
    //        }
    //    }


    //}
}

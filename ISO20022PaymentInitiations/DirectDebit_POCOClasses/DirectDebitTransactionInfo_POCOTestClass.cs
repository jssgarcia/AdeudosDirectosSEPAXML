﻿using System;

namespace ISO20022PaymentInitiations.DirectDebitPOCOClasses
{
    public class DirectDebitTransactionInfo_POCOClass
    {
        string debtorName;
        string txInternalId;
        string[] remittanceInformation;
        double amount;
        string mandateID;
        string iBAN;
        DateTime mandateSignatureDate;
        string previousMandateID;
        string previuosIBAN;

        public DirectDebitTransactionInfo_POCOClass(
            string debtorName,
            string txInternalId,
            string[] remittanceInformation,
            double amount,
            string mandateID,
            string iBAN,
            DateTime mandateSignatureDate,
            string previousMandateID,
            string previuosIBAN)
        {
            this.debtorName = debtorName;
            this.txInternalId = txInternalId;
            this.remittanceInformation=(string[])remittanceInformation.Clone();
            this.amount = amount;
            this.mandateID = mandateID;
            this.iBAN = iBAN;
            this.mandateSignatureDate = mandateSignatureDate;
            this.previousMandateID = previousMandateID;
            this.previuosIBAN = previuosIBAN;
        }

        public string DebtorName
        {
            get { return debtorName; }
        }

        public string TxInternalId
        {
            get { return txInternalId; }
        }

        public string[] RemittanceInformation
        {
            get { return remittanceInformation; }
        }

        public double Amount
        {
            get { return amount; }
        }

        public string MandateID
        {
            get { return mandateID; }
        }

        public string IBAN
        {
            get { return iBAN; }
        }

        public DateTime MandateSignatureDate
        {
            get { return mandateSignatureDate; }
        }

        public string PreviousMandateID
        {
            get { return previousMandateID; }
        }

        public string PreviuosIBAN
        {
            get { return previuosIBAN; }
        }
    }
}

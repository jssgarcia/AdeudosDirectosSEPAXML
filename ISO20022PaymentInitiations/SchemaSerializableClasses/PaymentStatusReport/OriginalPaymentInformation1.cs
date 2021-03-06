﻿namespace ISO20022PaymentInitiations.SchemaSerializableClasses.PaymentStatusReport
{
    /// <comentarios/>
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.002.001.03")]
    public class OriginalPaymentInformation1
    {
        private string orgnlPmtInfIdField;
        private string orgnlNbOfTxsField;
        private decimal orgnlCtrlSumField;
        private bool orgnlCtrlSumFieldSpecified;
        private TransactionGroupStatus3Code pmtInfStsField;
        private bool pmtInfStsFieldSpecified;
        private StatusReasonInformation8[] stsRsnInfField;
        private NumberOfTransactionsPerStatus3[] nbOfTxsPerStsField;
        private PaymentTransactionInformation25[] txInfAndStsField;

        //Parameterless constructor for Serialization purpose
        public OriginalPaymentInformation1() { }

        public OriginalPaymentInformation1(
            string originalPaymentInformationIdentification,
            string originalNumberOfTransactions,
            decimal originalControlSum,
            bool originalControlSumSpecified,
            TransactionGroupStatus3Code paymentInformationStatus,
            bool paymentInformationStatusSpecified,
            StatusReasonInformation8[] statusReasonInformation,
            NumberOfTransactionsPerStatus3[] numberOfTransactionsPerStatus,
            PaymentTransactionInformation25[] transactionInformationAndStatus)
        {
            this.orgnlPmtInfIdField = originalPaymentInformationIdentification;
            this.orgnlNbOfTxsField = originalNumberOfTransactions;
            this.orgnlCtrlSumField = originalControlSum;
            this.orgnlCtrlSumFieldSpecified = originalControlSumSpecified;
            this.pmtInfStsField = paymentInformationStatus;
            this.pmtInfStsFieldSpecified = paymentInformationStatusSpecified;
            this.stsRsnInfField = (StatusReasonInformation8[])statusReasonInformation.Clone();
            this.nbOfTxsPerStsField = (NumberOfTransactionsPerStatus3[])numberOfTransactionsPerStatus.Clone();
            this.txInfAndStsField = (PaymentTransactionInformation25[])transactionInformationAndStatus.Clone();
        }

        /// <comentarios/>
        public string OrgnlPmtInfId
        {
            get
            {
                return this.orgnlPmtInfIdField;
            }
            set
            {
                this.orgnlPmtInfIdField = value;
            }
        }

        /// <comentarios/>
        public string OrgnlNbOfTxs
        {
            get
            {
                return this.orgnlNbOfTxsField;
            }
            set
            {
                this.orgnlNbOfTxsField = value;
            }
        }

        /// <comentarios/>
        public decimal OrgnlCtrlSum
        {
            get
            {
                return this.orgnlCtrlSumField;
            }
            set
            {
                this.orgnlCtrlSumField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool OrgnlCtrlSumSpecified
        {
            get
            {
                return this.orgnlCtrlSumFieldSpecified;
            }
            set
            {
                this.orgnlCtrlSumFieldSpecified = value;
            }
        }

        /// <comentarios/>
        public TransactionGroupStatus3Code PmtInfSts
        {
            get
            {
                return this.pmtInfStsField;
            }
            set
            {
                this.pmtInfStsField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool PmtInfStsSpecified
        {
            get
            {
                return this.pmtInfStsFieldSpecified;
            }
            set
            {
                this.pmtInfStsFieldSpecified = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlElementAttribute("StsRsnInf")]
        public StatusReasonInformation8[] StsRsnInf
        {
            get
            {
                return this.stsRsnInfField;
            }
            set
            {
                this.stsRsnInfField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlElementAttribute("NbOfTxsPerSts")]
        public NumberOfTransactionsPerStatus3[] NbOfTxsPerSts
        {
            get
            {
                return this.nbOfTxsPerStsField;
            }
            set
            {
                this.nbOfTxsPerStsField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlElementAttribute("TxInfAndSts")]
        public PaymentTransactionInformation25[] TxInfAndSts
        {
            get
            {
                return this.txInfAndStsField;
            }
            set
            {
                this.txInfAndStsField = value;
            }
        }
    }
}

﻿using ReferencesAndTools;

namespace DirectDebitElements
{
    public class InternationalAccountBankNumberIBAN
    {
        string iban;

        public InternationalAccountBankNumberIBAN(string iban)
        {
            if (BankAccountNumberChecker.IsValidIBAN(iban))
            {
                this.iban = iban;
            }
            //else
            //{
            //    string exceptionMessage = "The provided IBAN string is invalid";
            //    throw new System.ArgumentException(exceptionMessage, "IBAN");
            //}
        }

        public InternationalAccountBankNumberIBAN(ClientAccountCodeCCC ccc)
        {
            if (BankAccountNumberChecker.IsValidCCC(ccc.CCC))
            {
                this.iban = BankAccountNumberChecker.CalculateSpanishIBAN(ccc.CCC);
            }
            //else
            //{
            //    string exceptionMessage = "The provided CCC string is invalid";
            //    throw new System.ArgumentException(exceptionMessage, "CCC");
            //}
        }

        public string IBAN
        {
            get { return iban; }
        }

        public string FormattedIBAN
        {
            get
            {
                if (iban!=null) return "IBAN " + splitIBAN(iban);
                return null;
            }
        }

        public string CCC
        {
            get
            {
                if (iban != null) return iban.Substring(4);
                return null;
            }
        }

        public string InternationalCode
        {
            get
            {
                if (iban != null) return iban.Substring(0,2);
                return null;
            }
        }

        public string IBANCheck
        {
            get
            {
                if (iban != null) return iban.Substring(2,2);
                return null;
            }
        }

        private string splitIBAN(string iban)
        {
            string splittedIBAN=string.Empty;
            for (int i = 0; i <= 20; i += 4)
            {
                splittedIBAN += iban.Substring(i, 4) + " ";
            }
            return splittedIBAN.TrimEnd();
        }
    }
}

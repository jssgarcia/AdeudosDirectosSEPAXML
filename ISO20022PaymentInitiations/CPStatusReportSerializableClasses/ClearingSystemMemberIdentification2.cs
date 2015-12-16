﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISO20022PaymentInitiations.CPStatusReportSerializableClasses
{
    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.81.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.002.001.03")]
    public partial class ClearingSystemMemberIdentification2
    {

        private ClearingSystemIdentification2Choice clrSysIdField;

        private string mmbIdField;

        /// <comentarios/>
        public ClearingSystemIdentification2Choice ClrSysId
        {
            get
            {
                return this.clrSysIdField;
            }
            set
            {
                this.clrSysIdField = value;
            }
        }

        /// <comentarios/>
        public string MmbId
        {
            get
            {
                return this.mmbIdField;
            }
            set
            {
                this.mmbIdField = value;
            }
        }
    }
}

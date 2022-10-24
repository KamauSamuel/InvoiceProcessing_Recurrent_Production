using MFiles.VAF.Configuration;
using MFiles.VAF.Configuration.JsonAdaptor;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Recurrent.Invoices.Production
{
    [DataContract]
    public class Configuration
    {
        [MFPropertyDef(Required = true)]
        public MFIdentifier PDCostCenter = "PD.Team";
        [MFPropertyDef(Required = true)]
        public MFIdentifier PDCurrency = "PD.Currency";
        [MFPropertyDef(Required = true)]
        public MFIdentifier PDInvoiceAmount = "PD.Invoice Amount";
        [MFPropertyDef(Required = true)]
        public MFIdentifier PDFrequency = "PD.Frequency";
        [MFPropertyDef(Required = true)]
        public MFIdentifier PDNarrative = "PD.Narrative";
        [MFPropertyDef(Required = true)]
        public MFIdentifier PDVendor = "PD.Vendor";
        [MFPropertyDef(Required = true)]
        public MFIdentifier PDExpenseCategory = "PD.Expense Category";
        [MFClass(Required = true)]
        public MFIdentifier CLInvoiceProcessing = "CL.Non-Procured Purchase";
        [MFPropertyDef(Required = true)]
        public MFIdentifier PDNextInvoiceDueDate = "PD.NextInvoiceDueDate";
        [MFPropertyDef(Required = true)]
        public MFIdentifier PDInvoiceDueDate = "PD.Invoice Due Date";
        [MFObjType(Required = true)]
        public MFIdentifier ObjTransactionFile = "O.Transaction File";
        [MFWorkflow(Required = true)]
        public MFIdentifier WFNonProcuredPurchases = "WF.Procured & Non-Procured Purchases";
        [MFState(Required = true)]
        public MFIdentifier WorkflowStateReceiveInvoice = "WFS.Procured & Non-Procured Purchases.6. Recieve Invoice and DN (w/applicable)";
    }
}
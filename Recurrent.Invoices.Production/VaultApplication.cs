using MFiles.VAF;
using MFiles.VAF.Common;
using MFiles.VAF.Configuration;
using MFiles.VAF.Core;
using MFilesAPI;
using System;
using System.Diagnostics;

namespace Recurrent.Invoices.Production
{
    /// <summary>
    /// The entry point for this Vault Application Framework application.
    /// </summary>
    /// <remarks>Examples and further information available on the developer portal: http://developer.m-files.com/. </remarks>
    public class VaultApplication
        : ConfigurableVaultApplicationBase<Configuration>
    {
        [StateAction("WFS.Procured Purchases & Invoice Processing.Create New Transcation File")]
        public void WorkflowStateAction(StateEnvironment env)
        {
            bool expe = env.PropertyValues.SearchForProperty(Configuration.PDExpenseCategory).TypedValue.IsNULL();
            SysUtils.ReportInfoToEventLog($"Expense Category status: {expe} ");
            string Narrative = env.PropertyValues.SearchForProperty(Configuration.PDNarrative).TypedValue.DisplayValue;
            SysUtils.ReportInfoToEventLog($"Narrative value: {Narrative} ");
            var vendor = env.PropertyValues.SearchForProperty(Configuration.PDVendor).TypedValue.GetValueAsLookups();
            Lookups vendorid = ReturnLookup(vendor);
            SysUtils.ReportInfoToEventLog($"Vendor ID value: {vendorid} ");
            int currid = Convert.ToInt32(env.PropertyValues.SearchForProperty(Configuration.PDCurrency).TypedValue.GetValueAsLookup().DisplayID);
            SysUtils.ReportInfoToEventLog($"Currency ID value: {currid} ");
            int freqid = Convert.ToInt32(env.PropertyValues.SearchForProperty(Configuration.PDFrequency).TypedValue.GetValueAsLookup().DisplayID);
            SysUtils.ReportInfoToEventLog($"Frequency ID value: {freqid} ");
            double invamt = Convert.ToDouble(env.PropertyValues.SearchForProperty(Configuration.PDInvoiceAmount).TypedValue.DisplayValue);
            SysUtils.ReportInfoToEventLog($"Invoice amount value: {invamt} ");
            var costcenter = env.PropertyValues.SearchForProperty(Configuration.PDCostCenter).TypedValue.GetValueAsLookups();
            Lookups costcenterid = ReturnLookup(costcenter);
            SysUtils.ReportInfoToEventLog($"Cost Center ID: {costcenterid} ");
            String nextinvduedate = env.PropertyValues.SearchForProperty(Configuration.PDNextInvoiceDueDate).TypedValue.DisplayValue;
            SysUtils.ReportInfoToEventLog($"Next Invoice Due Date is: {nextinvduedate} ");
            
            PropertyValues oPropVals = new PropertyValues();

            PropertyValue classpropertyvalue = new PropertyValue();
            classpropertyvalue.PropertyDef = (int)MFBuiltInPropertyDef.MFBuiltInPropertyDefClass;
            classpropertyvalue.TypedValue.SetValue(MFDataType.MFDatatypeLookup, Configuration.CLInvoiceProcessing.ID);
            oPropVals.Add(-1, classpropertyvalue);

            if (expe == false)
            {
                var ec = env.PropertyValues.SearchForProperty(Configuration.PDExpenseCategory).TypedValue.GetValueAsLookups();
                Lookups expensecatid = ReturnLookup(ec);
                PropertyValue expencecategorypropertyvalue = new PropertyValue();
                expencecategorypropertyvalue.PropertyDef = Configuration.PDExpenseCategory;
                expencecategorypropertyvalue.TypedValue.SetValue(MFDataType.MFDatatypeMultiSelectLookup, expensecatid);
                oPropVals.Add(-1, expencecategorypropertyvalue);
            }

            PropertyValue vendorpropertyvalue = new PropertyValue();
            vendorpropertyvalue.PropertyDef = Configuration.PDVendor;
            vendorpropertyvalue.TypedValue.SetValue(MFDataType.MFDatatypeMultiSelectLookup, vendorid);
            oPropVals.Add(-1, vendorpropertyvalue);

            PropertyValue narrativepropertyvalue = new PropertyValue();
            narrativepropertyvalue.PropertyDef = Configuration.PDNarrative;
            narrativepropertyvalue.TypedValue.SetValue(MFDataType.MFDatatypeMultiLineText, Narrative);
            oPropVals.Add(-1, narrativepropertyvalue);

            PropertyValue currencypropertyvalue = new PropertyValue();
            currencypropertyvalue.PropertyDef = Configuration.PDCurrency;
            currencypropertyvalue.TypedValue.SetValue(MFDataType.MFDatatypeLookup, currid);
            oPropVals.Add(-1, currencypropertyvalue);

            PropertyValue invoiceamtpropertyvalue = new PropertyValue();
            invoiceamtpropertyvalue.PropertyDef = Configuration.PDInvoiceAmount;
            invoiceamtpropertyvalue.TypedValue.SetValue(MFDataType.MFDatatypeFloating, invamt);
            oPropVals.Add(-1, invoiceamtpropertyvalue);

            PropertyValue costcenterpropertyvalue = new PropertyValue();
            costcenterpropertyvalue.PropertyDef = Configuration.PDCostCenter;
            costcenterpropertyvalue.TypedValue.SetValue(MFDataType.MFDatatypeMultiSelectLookup, costcenterid);
            oPropVals.Add(-1, costcenterpropertyvalue);

            PropertyValue frequencypropertyvalue = new PropertyValue();
            frequencypropertyvalue.PropertyDef = Configuration.PDFrequency;
            frequencypropertyvalue.TypedValue.SetValue(MFDataType.MFDatatypeLookup, freqid);
            oPropVals.Add(-1, frequencypropertyvalue);

            PropertyValue invoiceduedatepropertyvalue = new PropertyValue();
            invoiceduedatepropertyvalue.PropertyDef = Configuration.PDInvoiceDueDate;
            invoiceduedatepropertyvalue.TypedValue.SetValue(MFDataType.MFDatatypeDate, nextinvduedate);
            oPropVals.Add(-1, invoiceduedatepropertyvalue);

            PropertyValue propworkflow = new PropertyValue();
            propworkflow.PropertyDef = 38;
            propworkflow.TypedValue.SetValue(MFDataType.MFDatatypeLookup, Configuration.WFNonProcuredPurchases.ID);
            oPropVals.Add(-1, propworkflow);

            PropertyValue propstate = new PropertyValue();
            propstate.PropertyDef = 39;
            propstate.TypedValue.SetValue(MFDataType.MFDatatypeLookup, Configuration.WorkflowStateReceiveInvoice.ID);
            oPropVals.Add(-1, propstate);

            // Define the source files to add (none, in this case).
            var sourceFiles = new MFilesAPI.SourceObjectFiles();

            // What object type is being created (Employee)?
            var objectTypeID = Configuration.ObjTransactionFile.ID; // Employee object type ID.

            //A "single file document" must be both a document and contain exactly one file.
            var isSingleFileDocument =
             objectTypeID == (int)MFBuiltInObjectType.MFBuiltInObjectTypeDocument && sourceFiles.Count == 1;

            var oACL = new MFilesAPI.AccessControlList();
            // Create the object and check it in.
            var objectVersion = env.Vault.ObjectOperations.CreateNewObjectEx(
                objectTypeID,
                oPropVals,
                sourceFiles,
                SFD: isSingleFileDocument,
                CheckIn: true);
        }
        Lookups ReturnLookup(Lookups ollukups)
        {
            Lookups olookups = new Lookups();
            foreach (Lookup olookup in ollukups)
            {
                olookups.Add(-1, olookup);
            }
            return olookups;
        }
    }
}
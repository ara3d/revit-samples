﻿//
// (C) Copyright 2003-2019 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//

using System;
using System.Windows.Forms;

using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.Units.CS
{
    /// <summary>
    /// 
    /// </summary>
    public partial class FormatForm : Form
    {
        //Required designer variable.
        private Autodesk.Revit.DB.ForgeTypeId m_specTypeId;

        /// <summary>
        /// format options. 
        /// </summary>
        public Autodesk.Revit.DB.FormatOptions FormatOptions { get; }

        /// <summary>
        /// Initialize GUI with FormatData 
        /// </summary>
        ///// <param name="dataBuffer">relevant data from revit</param>
        public FormatForm(Autodesk.Revit.DB.ForgeTypeId specTypeId, Autodesk.Revit.DB.FormatOptions formatoptions)
        {
            InitializeComponent();
            m_specTypeId = specTypeId;
            FormatOptions = new Autodesk.Revit.DB.FormatOptions(formatoptions.GetUnitTypeId(), formatoptions.GetSymbolTypeId());
            FormatOptions.UseDefault = formatoptions.UseDefault;
            FormatOptions.Accuracy = formatoptions.Accuracy;
            FormatOptions.SuppressTrailingZeros = formatoptions.SuppressTrailingZeros;
            FormatOptions.SuppressLeadingZeros = formatoptions.SuppressLeadingZeros;
            FormatOptions.UsePlusPrefix = formatoptions.UsePlusPrefix;
            FormatOptions.UseDigitGrouping = formatoptions.UseDigitGrouping;
            FormatOptions.SuppressSpaces = formatoptions.SuppressSpaces;
        }

        private void FormatForm_Load(object sender, EventArgs e)
        {
            try
            {
                var isUseDefault = FormatOptions.UseDefault;
                if (isUseDefault)
                {
                   FormatOptions.UseDefault = false;
                }
                
                FormatForm_Clear();

                // Initialize the combo box and check box. 
                DisplayUnitTypecomboBox.BeginUpdate();
                DisplayUnitcomboBox.BeginUpdate();
                foreach (var unitTypeId in Autodesk.Revit.DB.UnitUtils.GetValidUnits(m_specTypeId))
                {
                   DisplayUnitTypecomboBox.Items.AddRange(new object[] { unitTypeId });
                   DisplayUnitcomboBox.Items.Add(Autodesk.Revit.DB.LabelUtils.GetLabelForUnit(unitTypeId));
                }
                DisplayUnitTypecomboBox.SelectedItem = FormatOptions.GetUnitTypeId();
                DisplayUnitcomboBox.SelectedIndex = DisplayUnitTypecomboBox.SelectedIndex;
                DisplayUnitTypecomboBox.EndUpdate();
                DisplayUnitcomboBox.EndUpdate();

                AccuracytextBox.Text = FormatOptions.Accuracy.ToString("###########0.############");

                SuppressTrailingZeroscheckBox.Checked = FormatOptions.SuppressTrailingZeros;
                SuppressLeadingZeroscheckBox.Checked = FormatOptions.SuppressLeadingZeros;
                UsePlusPrefixcheckBox.Checked = FormatOptions.UsePlusPrefix;
                UseDigitGroupingcheckBox.Checked = FormatOptions.UseDigitGrouping;
                SuppressSpacescheckBox.Checked = FormatOptions.SuppressSpaces;

                FormatOptions.UseDefault = isUseDefault;
                UseDefaultcheckBox.Checked = FormatOptions.UseDefault;

                if (!Autodesk.Revit.DB.Units.IsModifiableSpec(m_specTypeId))
                {
                   Text = "This unit type is unmodifiable";
                   UseDefaultcheckBox.Checked = true;
                   UseDefaultcheckBox.Enabled = false;
                   buttonOK.Enabled = false;
                }
            }
            catch
            {
                throw;
            }
        }

        private void FormatForm_Clear()
        {
            // Clear the combo box and check box. 
            UseDefaultcheckBox.Checked = false;
            
            DisplayUnitTypecomboBox.Items.Clear();
            DisplayUnitcomboBox.Items.Clear();
           
            AccuracytextBox.Text = "";
            
            UnitSymbolTypecomboBox.Items.Clear();
            UnitSymbolcomboBox.Items.Clear();
        
            SuppressTrailingZeroscheckBox.Checked = false;
            SuppressLeadingZeroscheckBox.Checked = false;
            UsePlusPrefixcheckBox.Checked = false;
            UseDigitGroupingcheckBox.Checked = false;
            SuppressSpacescheckBox.Checked = false;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            try
            {
                // Save the properties of FormatOptions. 
                FormatOptions.UseDefault = UseDefaultcheckBox.Checked;

                if (!FormatOptions.UseDefault)
                {
                   DisplayUnitTypecomboBox.SelectedIndex = DisplayUnitcomboBox.SelectedIndex;
                   FormatOptions.SetUnitTypeId((Autodesk.Revit.DB.ForgeTypeId)DisplayUnitTypecomboBox.SelectedItem);

                   FormatOptions.Accuracy = double.Parse(AccuracytextBox.Text);

                   UnitSymbolTypecomboBox.SelectedIndex = UnitSymbolcomboBox.SelectedIndex;
                   FormatOptions.SetSymbolTypeId((Autodesk.Revit.DB.ForgeTypeId)UnitSymbolTypecomboBox.SelectedItem);

                   FormatOptions.SuppressTrailingZeros = SuppressTrailingZeroscheckBox.Checked;
                   FormatOptions.SuppressLeadingZeros = SuppressLeadingZeroscheckBox.Checked;
                   FormatOptions.UsePlusPrefix = UsePlusPrefixcheckBox.Checked;
                   FormatOptions.UseDigitGrouping = UseDigitGroupingcheckBox.Checked;
                   FormatOptions.SuppressSpaces = SuppressSpacescheckBox.Checked;
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Invalid Input", ex.Message, TaskDialogCommonButtons.Ok);
            }
        }

        private void DisplayUnitcomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayUnitTypecomboBox.SelectedIndex = DisplayUnitcomboBox.SelectedIndex;

            UnitSymbolTypecomboBox.Items.Clear();
            UnitSymbolcomboBox.Items.Clear();
            
            UnitSymbolTypecomboBox.BeginUpdate();
            UnitSymbolcomboBox.BeginUpdate();
            foreach (var symbolTypeId in Autodesk.Revit.DB.FormatOptions.GetValidSymbols((Autodesk.Revit.DB.ForgeTypeId)DisplayUnitTypecomboBox.SelectedItem))
            {
                UnitSymbolTypecomboBox.Items.AddRange(new object[] { symbolTypeId });
                if (symbolTypeId.Empty())
                {
                   UnitSymbolcomboBox.Items.Add("");
                }
                else
                {
                   UnitSymbolcomboBox.Items.Add(Autodesk.Revit.DB.LabelUtils.GetLabelForSymbol(symbolTypeId));
                }
            }
            UnitSymbolTypecomboBox.SelectedItem = FormatOptions.GetSymbolTypeId();
            if (UnitSymbolTypecomboBox.SelectedIndex < 0)
            {
               UnitSymbolTypecomboBox.SelectedIndex = 0;
               FormatOptions.SetSymbolTypeId((Autodesk.Revit.DB.ForgeTypeId)UnitSymbolTypecomboBox.SelectedItem);
            }
            UnitSymbolcomboBox.SelectedIndex = UnitSymbolTypecomboBox.SelectedIndex;
            UnitSymbolTypecomboBox.EndUpdate();
            UnitSymbolcomboBox.EndUpdate();
        }

        private void UseDefaultcheckBox_CheckedChanged(object sender, EventArgs e)
        {
           FormatOptions.UseDefault = UseDefaultcheckBox.Checked; 
           if (UseDefaultcheckBox.Checked)
           {
              DisplayUnitcomboBox.Enabled = false;
              AccuracytextBox.Enabled = false;
              UnitSymbolcomboBox.Enabled = false;
              SuppressTrailingZeroscheckBox.Enabled = false;
              SuppressLeadingZeroscheckBox.Enabled = false;
              UsePlusPrefixcheckBox.Enabled = false;
              UseDigitGroupingcheckBox.Enabled = false;
              SuppressSpacescheckBox.Enabled = false;
           }
           else
           {
              DisplayUnitcomboBox.Enabled = true;
              AccuracytextBox.Enabled = true;
              UnitSymbolcomboBox.Enabled = true;
              SuppressTrailingZeroscheckBox.Enabled = true;
              SuppressLeadingZeroscheckBox.Enabled = true;
              UsePlusPrefixcheckBox.Enabled = true;
              UseDigitGroupingcheckBox.Enabled = true;
              SuppressSpacescheckBox.Enabled = true;
           }
        }
    }
}

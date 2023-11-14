//
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

namespace Revit.SDK.Samples.Loads.CS
{
    /// <summary>
    /// mainly deal with the operation on load case page on the form
    /// </summary>
    public partial class LoadsForm
    {
        int m_loadCaseDataGridViewSelectedIndex;
        int m_loadNatureDataGridViewSelectedIndex;
        DataGridViewTextBoxColumn LoadCasesName;
        DataGridViewTextBoxColumn LoadCasesNumber;
        DataGridViewComboBoxColumn LoadCasesNature;
        DataGridViewComboBoxColumn LoadCasesCategory;
        DataGridViewTextBoxColumn LoadNatureName;


        // Methods
        /// <summary>
        /// Initialize the data on this page.
        /// </summary>
        void InitializeLoadCasePage()
        {
            InitializeLoadCasesDataGridView();
            InitializeLoadNaturesDataGridView();


            if (0 == m_dataBuffer.LoadCases.Count)
            {
                duplicateLoadCasesButton.Enabled = false;
            }
            if (0 == m_dataBuffer.LoadNatures.Count)
            {
                addLoadNaturesButton.Enabled = false;
            }
            addLoadNaturesButton.Enabled = false;
        }

        /// <summary>
        /// Initialize the loadCasesDataGridView
        /// </summary>
        private void InitializeLoadCasesDataGridView()
        {
            LoadCasesName = new DataGridViewTextBoxColumn();
            LoadCasesNumber = new DataGridViewTextBoxColumn();
            LoadCasesNature = new DataGridViewComboBoxColumn();
            LoadCasesCategory = new DataGridViewComboBoxColumn();
            loadCasesDataGridView.AutoGenerateColumns = false;
            loadCasesDataGridView.Columns.AddRange(new DataGridViewColumn[] { LoadCasesName, LoadCasesNumber, LoadCasesNature, LoadCasesCategory });
            loadCasesDataGridView.DataSource = m_dataBuffer.LoadCasesMap;

            LoadCasesName.DataPropertyName = "LoadCasesName";
            LoadCasesName.HeaderText = "Name";
            LoadCasesName.Name = "LoadCasesName";
            LoadCasesName.ReadOnly = false;
            LoadCasesName.Width = loadCasesDataGridView.Width / 6;

            LoadCasesNumber.DataPropertyName = "LoadCasesNumber";
            LoadCasesNumber.HeaderText = "Case Number";
            LoadCasesNumber.Name = "LoadCasesNumber";
            LoadCasesNumber.ReadOnly = true;
            LoadCasesNumber.Width = loadCasesDataGridView.Width / 4;

            LoadCasesNature.DataPropertyName = "LoadCasesNatureId";
            LoadCasesNature.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            LoadCasesNature.HeaderText = "Nature";
            LoadCasesNature.Name = "LoadCasesNature";
            LoadCasesNature.Resizable = DataGridViewTriState.True;
            LoadCasesNature.SortMode = DataGridViewColumnSortMode.Automatic;
            LoadCasesNature.Width = loadCasesDataGridView.Width / 4;

            LoadCasesNature.DataSource = m_dataBuffer.LoadNatures;
            LoadCasesNature.DisplayMember = "Name";
            LoadCasesNature.ValueMember = "Id";

            LoadCasesCategory.DataPropertyName = "LoadCasesCategoryId";
            LoadCasesCategory.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            LoadCasesCategory.HeaderText = "Category";
            LoadCasesCategory.Name = "LoadCasesCategory";
            LoadCasesCategory.Resizable = DataGridViewTriState.True;
            LoadCasesCategory.SortMode = DataGridViewColumnSortMode.Automatic;
            LoadCasesCategory.Width = loadCasesDataGridView.Width / 4;

            LoadCasesCategory.DataSource = m_dataBuffer.LoadCaseCategories;
            LoadCasesCategory.DisplayMember = "Name";
            LoadCasesCategory.ValueMember = "Id";
            loadCasesDataGridView.MultiSelect = false;
            loadCasesDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
        }

        /// <summary>
        /// Initialize the loadNaturesDataGridView
        /// </summary>
        private void InitializeLoadNaturesDataGridView()
        {
            LoadNatureName = new DataGridViewTextBoxColumn();
            loadNaturesDataGridView.AutoGenerateColumns = false;
            loadNaturesDataGridView.Columns.AddRange(new DataGridViewColumn[] { LoadNatureName });
            loadNaturesDataGridView.DataSource = m_dataBuffer.LoadNaturesMap;
            LoadNatureName.DataPropertyName = "LoadNaturesName";
            LoadNatureName.HeaderText = "Name";
            LoadNatureName.Name = "LoadNaturesName";
            LoadNatureName.ReadOnly = false;
            LoadNatureName.Width = loadCasesDataGridView.Width - 100;
            loadNaturesDataGridView.MultiSelect = false;
            loadNaturesDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;

        }

        /// <summary>
        /// Respond the loadCasesDataGridView_CellClick event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadCasesDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            Initilize();
            m_loadCaseDataGridViewSelectedIndex = e.RowIndex;
        }

        /// <summary>
        /// Respond the loadNaturesDataGridView_CellClick event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadNaturesDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            Initilize();
            m_loadNatureDataGridViewSelectedIndex = e.RowIndex;
        }

        /// <summary>
        /// Respond the loadCasesDataGridView_ColumnHeaderMouseClick event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadCasesDataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            m_loadCaseDataGridViewSelectedIndex = e.RowIndex;
        }

        /// <summary>
        /// Respond the loadNaturesDataGridView_RowHeaderMouseClick event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadNaturesDataGridView_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            m_loadNatureDataGridViewSelectedIndex = e.RowIndex;
        }

        /// <summary>
        /// Respond the DataGridView cell validating event, 
        /// check the user's input whether it is correct.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadNaturesDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            var objectTemp = loadNaturesDataGridView.CurrentCell.Value;
            var nameTemp = objectTemp as string;

            var changeValue = e.FormattedValue;
            var changeValueTemp = changeValue as string;

            if (nameTemp == changeValueTemp)
            {
                return;
            }

            if (null == changeValueTemp)
            {
                TaskDialog.Show("Revit", "Name can not be null");
                e.Cancel = true;
                return;
            }

            if ("" == changeValueTemp)
            {
                TaskDialog.Show("Revit", "Name can not be null");
                e.Cancel = true;
                return;
            }

            if (!m_dataBuffer.LoadCasesDeal.IsNatureNameUnique(changeValueTemp))
            {
                TaskDialog.Show("Revit", "Name can not be same");
                e.Cancel = true;
                return;
            }
        }

        /// <summary>
        /// Respond the DataGridView cell validating event, 
        /// check the user's input whether it is correct.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadCasesDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex != 0)
            {
                return;
            }

            var cellTemp = loadCasesDataGridView.CurrentCell;
            if (null == cellTemp)
            {
                return;
            }
            var nameTemp = cellTemp.Value as string;
            if (null == nameTemp)
            {
                e.Cancel = false;
                return;
            }

            var changeValue = e.FormattedValue;
            var changeValueTemp = changeValue as string;

            if (nameTemp == changeValueTemp)
            {
                return;
            }

            if (null == changeValueTemp)
            {
                TaskDialog.Show("Revit", "Name can not be null");
                e.Cancel = true;
                return;
            }

            if ("" == changeValueTemp)
            {
                TaskDialog.Show("Revit", "Name can not be null");
                e.Cancel = true;
                return;
            }

            if (!m_dataBuffer.LoadCasesDeal.IsCaseNameUnique(changeValueTemp))
            {
                TaskDialog.Show("Revit", "Name can not be same");
                e.Cancel = true;
                return;
            }
        }

        /// <summary>
        /// When duplicateLoadCasesButton clicked, duplicate a load case. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void duplicateLoadCasesButton_Click(object sender, EventArgs e)
        {
            m_loadCaseDataGridViewSelectedIndex = loadCasesDataGridView.CurrentCell.RowIndex;
            if (!m_dataBuffer.LoadCasesDeal.DuplicateLoadCase(m_loadCaseDataGridViewSelectedIndex))
            {
                TaskDialog.Show("Revit", "Duplicate failed");
                return;
            }
            ReLoad();
        }

        /// <summary>
        /// When addLoadNaturesButton clicked, add a new load nature. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addLoadNaturesButton_Click(object sender, EventArgs e)
        {
            if (!m_dataBuffer.LoadCasesDeal.AddLoadNature(m_loadNatureDataGridViewSelectedIndex))
            {
                TaskDialog.Show("Revit", "Add Nature Failed");
                return;
            }
            ReLoad();
        }

        /// <summary>
        /// Reload the data of the cases and natures
        /// </summary>
        private void ReLoad()
        {
            loadNaturesDataGridView.DataSource = null;
            loadCasesDataGridView.DataSource = null;
            LoadCasesNature.SortMode = DataGridViewColumnSortMode.Automatic;
            loadNaturesDataGridView.DataSource = m_dataBuffer.LoadNaturesMap;
            loadCasesDataGridView.DataSource = m_dataBuffer.LoadCasesMap;
            Refresh();
            return;
        }

        /// <summary>
        /// enable button
        /// </summary>
        private void Initilize()
        {
            if (loadCasesDataGridView.Focused)
            {
                addLoadNaturesButton.Enabled = false;
                duplicateLoadCasesButton.Enabled = true;

            }
            else if (loadNaturesDataGridView.Focused)
            {
                addLoadNaturesButton.Enabled = true;
                duplicateLoadCasesButton.Enabled = false;

            }
            Refresh();
            return;
        }
    }
}
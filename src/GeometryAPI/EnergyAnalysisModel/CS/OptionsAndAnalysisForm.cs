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

namespace Revit.SDK.Samples.EnergyAnalysisModel.CS
{
    public partial class OptionsAndAnalysisForm : Form
    {
        private readonly EnergyAnalysisModel m_model;

        public OptionsAndAnalysisForm(EnergyAnalysisModel analysisModel)
        {
            m_model = analysisModel;
            InitializeComponent();
            InitializeOptionsUI();
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            // get UI input of options
            m_model.SetTier(comboBoxTier.SelectedText);
            m_model.Options.ExportMullions = checkBoxExportMullions.Checked;
            m_model.Options.IncludeShadingSurfaces = checkBoxIncludeShadingSurfaces.Checked;
            m_model.Options.SimplifyCurtainSystems = checkBoxSimplifyCurtainSystems.Checked;

            m_model.Initialize();
            m_model.RefreshAnalysisData(treeViewAnalyticalData);

            // expand all child
            treeViewAnalyticalData.ExpandAll();
            Refresh();
        }

        /// <summary>
        ///     Set default Tier as SecondLevelBoundaries
        /// </summary>
        private void InitializeOptionsUI()
        {
            comboBoxTier.SelectedIndex = 3;
        }
    }
}
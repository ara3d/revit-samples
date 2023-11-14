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
namespace Revit.SDK.Samples.CreateSimpleAreaRein.CS
{
    using System;
    using System.Windows.Forms;


    /// <summary>
    /// simple business process of UI
    /// </summary>
    public partial class CreateSimpleAreaReinForm : Form
    {
        private AreaReinData m_dataBuffer;

        /// <summary>
        /// constructor; initialize member data
        /// </summary>
        /// <param name="dataBuffer"></param>
        public CreateSimpleAreaReinForm(AreaReinData dataBuffer)
        {
            InitializeComponent();

            m_dataBuffer = dataBuffer;
        }

        /// <summary>
        /// bind data to controls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateSimpleAreaReinForm_Load(object sender, EventArgs e)
        {
            areaReinPropertyGrid.SelectedObject = m_dataBuffer;
        }

        /// <summary>
        /// to create
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// cancel the command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}

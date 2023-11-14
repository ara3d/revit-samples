﻿//
// (C) Copyright 2003-2020 by Autodesk, Inc. All rights reserved.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.

//
// AUTODESK PROVIDES THIS PROGRAM 'AS IS' AND WITH ALL ITS FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable. 

using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Revit.SDK.Samples.UpdateExternallyTaggedBRep.CS.Properties;

namespace Revit.SDK.Samples.UpdateExternallyTaggedBRep.CS
{
    /// <summary>
    ///     Implements the Revit add-in interface IExternalApplication.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Application : IExternalApplication
    {
        private static readonly string m_addinAssemblyPath = typeof(Application).Assembly.Location;

        /// <summary>
        ///     Implements the OnShutdown event.
        /// </summary>
        /// <param name="application">
        ///     An object that is passed to the external application
        ///     which contains the controlled application.
        /// </param>
        /// <returns></returns>
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        /// <summary>
        ///     Implements the OnStartup event.
        /// </summary>
        /// <param name="application">
        ///     An object that is passed to the external application
        ///     which contains the controlled application.
        /// </param>
        /// <returns></returns>
        public Result OnStartup(UIControlledApplication application)
        {
            createRibbonButtons(application);
            return Result.Succeeded;
        }

        /// <summary>
        ///     Creates and adds the new ribbon and the new ribbon's buttons to the application.
        /// </summary>
        /// <param name="application">
        ///     An object that is passed to the external application
        ///     which contains the controlled application.
        /// </param>
        private void createRibbonButtons(UIControlledApplication application)
        {
            // Create and add the new ribbon "Create ExternallyTaggedBRep".
            application.CreateRibbonTab("Create ExternallyTaggedBRep");
            var rp = application.CreateRibbonPanel("Create ExternallyTaggedBRep",
                "Create Geometry with persistent tags");

            // Create and add the new ribbon button "Create tagged BRep".
            var pbd1 = new PushButtonData("CreateTaggedBRep", "Create tagged BRep",
                m_addinAssemblyPath,
                "Revit.SDK.Samples.UpdateExternallyTaggedBRep.CS.CreateBRep");
            pbd1.LargeImage = convertFromBitmap(Resources.large_shape);
            pbd1.Image = convertFromBitmap(Resources.small_shape);
            _ = rp.AddItem(pbd1) as PushButton;

            // Create and add the new ribbon button "Update tagged BRep".
            var pbd2 = new PushButtonData("UpdateTaggedBRep", "Update tagged BRep",
                m_addinAssemblyPath,
                "Revit.SDK.Samples.UpdateExternallyTaggedBRep.CS.UpdateBRep");
            pbd2.LargeImage = convertFromBitmap(Resources.large_shape);
            pbd2.Image = convertFromBitmap(Resources.small_shape);
            _ = rp.AddItem(pbd2) as PushButton;
        }

        private BitmapSource convertFromBitmap(Bitmap bitmap)
        {
            return Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }
    }
}
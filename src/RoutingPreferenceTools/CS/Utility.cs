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

using System.Linq;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.RoutingPreferenceTools.CS
{
    internal class Validation
    {
        public static bool ValidateMep(Application application)
        {
            return application.IsPipingEnabled;
        }

        public static void MepWarning()
        {
            TaskDialog.Show("RoutingPreferenceTools", "Revit MEP is required to run this addin.");
        }

        public static bool ValidatePipesDefined(Document document)
        {
            var collector = new FilteredElementCollector(document);
            collector.OfClass(typeof(PipeType));
            if (collector.Count() == 0)
                return false;
            return true;
        }

        public static void PipesDefinedWarning()
        {
            TaskDialog.Show("RoutingPreferenceTools",
                "At least two PipeTypes are required to run this command.  Please define another PipeType.");
        }
    }

    internal class Convert
    {
        public static double ConvertValueDocumentUnits(double decimalFeet, Document document)
        {
            var formatOption = document.GetUnits().GetFormatOptions(SpecTypeId.PipeSize);
            return UnitUtils.ConvertFromInternalUnits(decimalFeet, formatOption.GetUnitTypeId());
        }


        public static double ConvertValueToFeet(double unitValue, Document document)
        {
            var tempVal = ConvertValueDocumentUnits(unitValue, document);
            var ratio = unitValue / tempVal;
            return unitValue * ratio;
        }
    }
}
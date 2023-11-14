﻿//
// (C) Copyright 2003-2021 by Autodesk, Inc.
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
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI.Selection;

namespace ContextualAnalyticalModel
{
   /// <summary>
   /// Implements the Revit add-in interface IExternalCommand
   /// </summary>
   [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
   [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
   public class CreateCustomAreaLoad : IExternalCommand
   {
       public virtual Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
      {
         try
         {
            var document = commandData.Application.ActiveUIDocument.Document;
            var activeDoc = commandData.Application.ActiveUIDocument;

            //select object for adding a line load
            var eRef = activeDoc.Selection.PickObject(ObjectType.Element, "Please select the analytical element");
            ElementId selectedElementId = null;
            if (eRef != null && eRef.ElementId != ElementId.InvalidElementId)
               selectedElementId = eRef.ElementId;

            var start = activeDoc.Selection.PickPoint("start");
            var end = activeDoc.Selection.PickPoint("end");

            //create curveloop which will be assigned to the analytical panel
            var profileloop = new CurveLoop();
            profileloop.Append(Line.CreateBound(
              start, new XYZ(end.X, start.Y, 0)));
            profileloop.Append(Line.CreateBound(
               new XYZ(end.X, start.Y, 0), end));
            profileloop.Append(Line.CreateBound(
               end, new XYZ(start.X, end.Y, 0)));
            profileloop.Append(Line.CreateBound(
               new XYZ(start.X, end.Y, 0), start));

            var loops = new List<CurveLoop>();
            loops.Add(profileloop);

            using (var transaction = new Transaction(document, "Create custom AreaLoad"))
            {
               transaction.Start();

               if (AreaLoad.IsCurveLoopsInsideHostBoundaries(document, selectedElementId, loops))
                   AreaLoad.Create(document, selectedElementId, loops, new XYZ(1,0,0), null);

               transaction.Commit();
            }

         }
         catch (Exception ex)
         {
            message = ex.Message;
            return Result.Failed;
         }

         return Result.Succeeded;
      }
   }
}


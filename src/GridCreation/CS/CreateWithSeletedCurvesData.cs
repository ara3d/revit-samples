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
using System.Collections;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.GridCreation.CS
{
   /// <summary>
   /// The dialog which provides the options of creating grids with selected lines/arcs
   /// </summary>
   public class CreateWithSelectedCurvesData : CreateGridsData
   {
      #region Fields
      // Selected curves in current document
      private CurveArray m_selectedCurves;
      // Whether to delete selected lines/arc after creation
      // Label of first grid
      // Bubble location of grids

      #endregion

      #region Properties
      /// <summary>
      /// Whether to delete selected lines/arc after creation
      /// </summary>
      public bool DeleteSelectedElements { get; set; }

      /// <summary>
      /// Bubble location of grids
      /// </summary>
      public BubbleLocation BubbleLocation { get; set; }

      /// <summary>
      /// Label of first grid
      /// </summary>
      public string FirstLabel { get; set; }

      #endregion

      #region Methods
      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="application">Revit application</param>
      /// <param name="selectedCurves">Array contains geometry curves of selected lines or arcs </param>
      /// <param name="labels">List contains all existing labels in Revit document</param>
      public CreateWithSelectedCurvesData(UIApplication application, CurveArray selectedCurves, ArrayList labels)
         : base(application, labels)
      {
         m_selectedCurves = selectedCurves;
      }

      /// <summary>
      /// Create grids
      /// </summary>
      public void CreateGrids()
      {
         var errorCount = 0;

         var curves = new CurveArray();

         var i = 0;
         foreach (Curve curve in m_selectedCurves)
         {
            try
            {
               var line = curve as Line;
               if (line != null) // Selected curve is a line
               {
                   var lineToCreate = TransformLine(line, BubbleLocation);
                  if (i == 0)
                  {
                      var grid =
                          // Create the first grid
                          CreateLinearGrid(lineToCreate);

                     try
                     {
                        // Set label of first grid
                        grid.Name = FirstLabel;
                     }
                     catch (ArgumentException)
                     {
                        ShowMessage(resManager.GetString("FailedToSetLabel") + FirstLabel + "!",
                                    resManager.GetString("FailureCaptionSetLabel"));
                     }
                  }
                  else
                  {
                     AddCurveForBatchCreation(ref curves, lineToCreate);
                  }
               }
               else // Selected curve is an arc
               {
                  var arc = curve as Arc;
                  if (arc != null)
                  {
                     if (arc.IsBound) // Part of a circle
                     {
                         var arcToCreate = TransformArc(arc, BubbleLocation);

                        if (i == 0)
                        {
                            var grid =
                                // Create arc grid
                                NewGrid(arcToCreate);

                           try
                           {
                              // Set label of first grid
                              grid.Name = FirstLabel;
                           }
                           catch (ArgumentException)
                           {
                              ShowMessage(resManager.GetString("FailedToSetLabel") + FirstLabel + "!",
                                          resManager.GetString("FailureCaptionSetLabel"));
                           }
                        }
                        else
                        {
                           AddCurveForBatchCreation(ref curves, arcToCreate);
                        }
                     }
                     else // Arc is a circle
                     {
                        // In Revit UI user can select a circle to create a grid, but actually two grids 
                        // (One from 0 to 180 degree and the other from 180 degree to 360) will be created. 
                        // In RevitAPI using NewGrid method with a circle as its argument will raise an exception. 
                        // Therefore in this sample we will create two arcs from the upper and lower parts of the 
                        // circle, and then create two grids on the base of the two arcs to accord with UI.
                        Arc upperArc = null;
                        Arc lowerArc = null;

                        TransformCircle(arc, ref upperArc, ref lowerArc, BubbleLocation);
                        // Create grids
                        if (i == 0)
                        {
                            var gridUpper =
                                // Create arc grid
                                NewGrid(upperArc);
                           try
                           {
                              // Set label of first grid
                              gridUpper.Name = FirstLabel;
                           }
                           catch (ArgumentException)
                           {
                              ShowMessage(resManager.GetString("FailedToSetLabel") + FirstLabel + "!",
                                          resManager.GetString("FailureCaptionSetLabel"));
                           }
                           AddCurveForBatchCreation(ref curves, lowerArc);
                        }
                        else
                        {
                           AddCurveForBatchCreation(ref curves, upperArc);
                           AddCurveForBatchCreation(ref curves, lowerArc);
                        }
                     }
                  }
               }
            }
            catch (Exception)
            {
               ++errorCount;
               continue;
            }

            ++i;
         }

         // Create grids with curves
         CreateGrids(curves);

         if (DeleteSelectedElements)
         {
            try
            {
               foreach (Element e in Command.GetSelectedModelLinesAndArcs(m_revitDoc))
               {
                   m_revitDoc.Delete(e.Id);
               }
            }
            catch (Exception)
            {
               ShowMessage(resManager.GetString("FailedToDeletedLinesOrArcs"),
                           resManager.GetString("FailureCaptionDeletedLinesOrArcs"));
            }
         }

         if (errorCount != 0)
         {
            ShowMessage(resManager.GetString("FailedToCreateGrids"),
                        resManager.GetString("FailureCaptionCreateGrids"));
         }
      }
      #endregion
   }
}

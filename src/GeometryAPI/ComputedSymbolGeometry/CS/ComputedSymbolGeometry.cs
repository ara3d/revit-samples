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


using System.Collections.Generic;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;

namespace Revit.SDK.Samples.ComputedSymbolGeometry.CS
{
   class ComputedSymbolGeometry
   {
      /// <summary>
      /// Revit document
      /// </summary>
      private Document RevitDoc;

      /// <summary>
      /// Options for geometry
      /// </summary>
      private Options m_options;

      /// <summary>
      /// Schema Id
      /// </summary>
      private int m_schemaId = -1;

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="doc">Revit Document</param>
      public ComputedSymbolGeometry(Document doc)
      {
         RevitDoc = doc;
         m_options = new Options();
      }

      /// <summary>
      /// Get geometry of family instances and show them in Revit views
      /// </summary>
      public void GetInstanceGeometry()
      {
         var instanceCollector = new FilteredElementCollector(RevitDoc);
         instanceCollector.OfClass(typeof(FamilyInstance));

         // create views by different names
         var View3DId = ElementId.InvalidElementId;
         var elems = new FilteredElementCollector(RevitDoc).OfClass(typeof(ViewFamilyType)).ToElements();
         foreach (var e in elems)
         {
            var v = e as ViewFamilyType;

            if (v != null && v.ViewFamily == ViewFamily.ThreeDimensional)
            {
               View3DId = e.Id;
               break;
            }
         }

         //View instanceView = RevitDoc.Create.NewView3D(new XYZ(1, 1, -1));
         var instanceView = View3D.CreateIsometric(RevitDoc, View3DId);
         var instanceViewOrientation3D = new ViewOrientation3D(new XYZ(-30.8272352809007, -2.44391067967133, 18.1013736367246), new XYZ(0.577350269189626, 0.577350269189626, -0.577350269189626), new XYZ(0.408248290463863, 0.408248290463863, 0.816496580927726));
         instanceView.SetOrientation(instanceViewOrientation3D);
         instanceView.SaveOrientation();
         instanceView.Name = "InstanceGeometry";

         //View originalView = RevitDoc.Create.NewView3D(new XYZ(0, 1, -1));
         var originalView = View3D.CreateIsometric(RevitDoc, View3DId);
         var originalViewOrientation3D = new ViewOrientation3D(new XYZ(-19.0249866627872, -5.09536632799455, 20.7528292850478), new XYZ(0, 0.707106781186547, -0.707106781186547), new XYZ(0, 0.707106781186548, 0.707106781186548));
         originalView.SetOrientation(originalViewOrientation3D);
         originalView.SaveOrientation();
         originalView.Name = "OriginalGeometry";

         //View transView = RevitDoc.Create.NewView3D(new XYZ(-1, 1, -1));
         var transView = View3D.CreateIsometric(RevitDoc, View3DId);
         //ViewOrientation3D transViewOrientation3D = new ViewOrientation3D(new XYZ(-7.22273804467383, -2.44391067967133, 18.1013736367246), new XYZ(-0.577350269189626, 0.577350269189626, -0.577350269189626), new XYZ(-0.408248290463863, 0.408248290463863, 0.816496580927726));
         var transViewOrientation3D = new ViewOrientation3D(new XYZ(-19.0249866627872, -5.09536632799455, 20.7528292850478), new XYZ(0, 0.707106781186547, -0.707106781186547), new XYZ(0, 0.707106781186548, 0.707106781186548));
         transView.SetOrientation(transViewOrientation3D);
         transView.SaveOrientation();
         transView.Name = "TransformedGeometry";

         foreach (FamilyInstance instance in instanceCollector)
         {
            var instanceGeo = instance.get_Geometry(m_options);
            var computedGeo = instance.GetOriginalGeometry(m_options);
            var transformGeo = computedGeo.GetTransformed(instance.GetTransform());

            // show family instance geometry
            //foreach (GeometryObject obj in instanceGeo.Objects)
            var Objects = instanceGeo.GetEnumerator();
            while (Objects.MoveNext())
            {
               var obj = Objects.Current;

               if (obj is Solid)
               {
                  var solid = obj as Solid;
                  PaintSolid(solid, instanceView);
               }
            }

            // show geometry that is original geometry
            //foreach (GeometryObject obj in computedGeo.Objects)
            var Objects1 = computedGeo.GetEnumerator();
            while (Objects1.MoveNext())
            {
               var obj = Objects1.Current;

               if (obj is Solid)
               {
                  var solid = obj as Solid;
                  PaintSolid(solid, originalView);
               }
            }

            // show geometry that was transformed
            //foreach (GeometryObject obj in transformGeo.Objects)
            var Objects2 = transformGeo.GetEnumerator();
            while (Objects2.MoveNext())
            {
               var obj = Objects2.Current;

               if (obj is Solid)
               {
                  var solid = obj as Solid;
                  PaintSolid(solid, transView);
               }
            }
         }
         // remove original instances to view point results.
         RevitDoc.Delete(instanceCollector.ToElementIds());
      }

      /// <summary>
      /// Paint solid by AVF
      /// </summary>
      /// <param name="solid">Solid to be painted</param>
      /// <param name="view">The view that shows solid</param>
      private void PaintSolid(Solid solid, View view)
      {
         var viewName = view.Name;
         var sfm = SpatialFieldManager.GetSpatialFieldManager(view);
         if (sfm == null) sfm = SpatialFieldManager.CreateSpatialFieldManager(view, 1);

         if (m_schemaId != -1)
         {
            var results = sfm.GetRegisteredResults();

            if (!results.Contains(m_schemaId))
            {
               m_schemaId = -1;
            }
         }

         // set up the display style
         if (m_schemaId == -1)
         {
            var resultSchema1 = new AnalysisResultSchema("PaintedSolid " + viewName, "Description");

            var displayStyle = AnalysisDisplayStyle.CreateAnalysisDisplayStyle(RevitDoc, "Real_Color_Surface" + viewName, new AnalysisDisplayColoredSurfaceSettings(), new AnalysisDisplayColorSettings(), new AnalysisDisplayLegendSettings());

            resultSchema1.AnalysisDisplayStyleId = displayStyle.Id;

            m_schemaId = sfm.RegisterResult(resultSchema1);
         }

         // get points of all faces in the solid
         var faces = solid.Faces;
         var trf = Transform.Identity;
         foreach (Face face in faces)
         {
            var idx = sfm.AddSpatialFieldPrimitive(face, trf);
            IList<UV> uvPts = null;
            IList<ValueAtPoint> valList = null;
            ComputeValueAtPointForFace(face, out uvPts, out valList, 1);

            var pnts = new FieldDomainPointsByUV(uvPts);
            var vals = new FieldValues(valList);
            sfm.UpdateSpatialFieldPrimitive(idx, pnts, vals, m_schemaId);
         }
      }

      /// <summary>
      /// Compute values at point for face
      /// </summary>
      /// <param name="face">Give face</param>
      /// <param name="uvPts">UV points</param>
      /// <param name="valList">Values at point</param>
      /// <param name="measurementNo"></param>
      private void ComputeValueAtPointForFace(Face face, out IList<UV> uvPts,
          out IList<ValueAtPoint> valList, int measurementNo)
      {
         var doubleList = new List<double>();
         uvPts = new List<UV>();
         valList = new List<ValueAtPoint>();
         var bb = face.GetBoundingBox();
         for (var u = bb.Min.U; u < bb.Max.U + 0.0000001; u = u + (bb.Max.U - bb.Min.U) / 1)
         {
            for (var v = bb.Min.V; v < bb.Max.V + 0.0000001; v = v + (bb.Max.V - bb.Min.V) / 1)
            {
               var uvPnt = new UV(u, v);
               uvPts.Add(uvPnt);
               var faceXYZ = face.Evaluate(uvPnt);
               // Specify three values for each point
               for (var ii = 1; ii <= measurementNo; ii++)
                  doubleList.Add(faceXYZ.DistanceTo(XYZ.Zero) * ii);
               valList.Add(new ValueAtPoint(doubleList));
               doubleList.Clear();
            }
         }
      }
   }
}
// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Document = Autodesk.Revit.Creation.Document;

namespace Revit.SDK.Samples.ModelLines.CS
{
    /// <summary>
    ///     The main deal class, which takes charge of showing the number of each model line type
    ///     and creating one instance for each type using Revit API
    /// </summary>
    public class ModelLines
    {
        private readonly ModelCurveArray m_arcArray; // Store the ModelArc references
        private readonly Application m_createApp; // Store the create Application reference
        private readonly Document m_createDoc; // Store the create Document reference
        private readonly ModelCurveArray m_ellipseArray; // Store the ModelEllipse references
        private readonly ModelCurveArray m_hermiteArray; // Store the ModelHermiteSpline references

        private readonly List<ModelCurveCounter> m_informationMap; // Store the number of each model line type

        private readonly ModelCurveArray m_lineArray; // Store the ModelLine references

        private readonly ModelCurveArray m_nurbArray; // Store the ModelNurbSpline references

        // Private members
        private readonly UIApplication m_revit; // Store the reference of the application in revit
        private readonly List<SketchPlane> m_sketchArray; // Store the SketchPlane references

        /// <summary>
        ///     The default constructor
        /// </summary>
        /// <param name="revit">The reference of the application in revit</param>
        public ModelLines(UIApplication revit)
        {
            // Store the reference of the application for further use.
            m_revit = revit;
            // Get the create references
            m_createApp = m_revit.Application.Create; // Creation.Application
            m_createDoc = m_revit.ActiveUIDocument.Document.Create; // Creation.Document

            // Construct all the ModelCurveArray instances for model lines
            m_lineArray = new ModelCurveArray();
            m_arcArray = new ModelCurveArray();
            m_ellipseArray = new ModelCurveArray();
            m_hermiteArray = new ModelCurveArray();
            m_nurbArray = new ModelCurveArray();

            // Construct the sketch plane list data
            m_sketchArray = new List<SketchPlane>();

            // Construct the information list data
            m_informationMap = new List<ModelCurveCounter>();
        }

        /// <summary>
        ///     The type-number map, store the number of each model line type
        /// </summary>
        public ReadOnlyCollection<ModelCurveCounter> InformationMap =>
            new ReadOnlyCollection<ModelCurveCounter>(m_informationMap);

        /// <summary>
        ///     Get the id information of all ModelEllipses in revit,
        ///     which displayed this in elementIdComboBox when ellipseRadioButton checked
        /// </summary>
        public ReadOnlyCollection<IdInfo> EllispeIdArray
        {
            get
            {
                // Create a new list
                var idArray = new List<IdInfo>();
                // Add all ModelEllipses' id information into the list
                foreach (ModelCurve ellipse in m_ellipseArray)
                {
                    var info = new IdInfo("ModelEllipse", ellipse.Id);
                    idArray.Add(info);
                }

                // return a read only list
                return new ReadOnlyCollection<IdInfo>(idArray);
            }
        }

        /// <summary>
        ///     Get the id information of all ModelHermiteSpline in revit,
        ///     which displayed this in elementIdComboBox when hermiteSplineRadioButton checked
        /// </summary>
        public ReadOnlyCollection<IdInfo> HermiteSplineIdArray
        {
            get
            {
                // Create a new list
                var idArray = new List<IdInfo>();
                // Add all ModelHermiteSplines' id information into the list
                foreach (ModelCurve hermite in m_hermiteArray)
                {
                    var info = new IdInfo("ModelHermiteSpline", hermite.Id);
                    idArray.Add(info);
                }

                // return a read only list
                return new ReadOnlyCollection<IdInfo>(idArray);
            }
        }

        /// <summary>
        ///     Get the id information of all ModelNurbSpline in revit,
        ///     which displayed this in elementIdComboBox when NurbSplineRadioButton checked
        /// </summary>
        public ReadOnlyCollection<IdInfo> NurbSplineIdArray
        {
            get
            {
                // Create a new list
                var idArray = new List<IdInfo>();
                // Add all ModelNurbSplines' id information into the list
                foreach (ModelCurve nurb in m_nurbArray)
                {
                    var info = new IdInfo("ModelNurbSpline", nurb.Id);
                    idArray.Add(info);
                }

                // return a read only list
                return new ReadOnlyCollection<IdInfo>(idArray);
            }
        }

        /// <summary>
        ///     Allow the user to get all sketch plane in revit
        /// </summary>
        public ReadOnlyCollection<IdInfo> SketchPlaneIdArray
        {
            get
            {
                // Create a new list
                var idArray = new List<IdInfo>();
                // Add all SketchPlane' id information into the list
                foreach (var sketch in m_sketchArray)
                {
                    var info = new IdInfo("SketchPlane", sketch.Id);
                    idArray.Add(info);
                }

                // return a read only list
                return new ReadOnlyCollection<IdInfo>(idArray);
            }
        }

        /// <summary>
        ///     This is the main deal method in this example.
        /// </summary>
        public void Run()
        {
            // Get all sketch plane in revit
            GetSketchPlane();

            // Get all model lines in revit
            GetModelLines();

            // Initialize the InformationMap property for DataGridView display
            InitDisplayInformation();

            // Display the form and allow the user to create one of each model line in revit
            using (var displayForm = new ModelLinesForm(this))
            {
                displayForm.ShowDialog();
            }
        }

        /// <summary>
        ///     Create a new sketch plane which all model lines are placed on.
        /// </summary>
        /// <param name="normal"></param>
        /// <param name="origin"></param>
        public void CreateSketchPlane(XYZ normal, XYZ origin)
        {
            try
            {
                // First create a Geometry.Plane which need in NewSketchPlane() method
                var geometryPlane = Plane.CreateByNormalAndOrigin(normal, origin);
                if (null == geometryPlane) // assert the creation is successful
                    throw new Exception("Create the geometry plane failed.");
                // Then create a sketch plane using the Geometry.Plane
                var plane = SketchPlane.Create(m_revit.ActiveUIDocument.Document, geometryPlane);
                if (null == plane) // assert the creation is successful
                    throw new Exception("Create the sketch plane failed.");

                // Finally add the created plane into the sketch plane array
                m_sketchArray.Add(plane);
            }
            catch (Exception ex)
            {
                throw new Exception("Can not create the sketch plane, message: " + ex.Message);
            }
        }

        /// <summary>
        ///     Create the line(ModelLine)
        /// </summary>
        /// <param name="sketchId">the id of the sketch plane</param>
        /// <param name="startPoint">the start point of the line</param>
        /// <param name="endPoint">the end point of the line</param>
        public void CreateLine(ElementId sketchId, XYZ startPoint, XYZ endPoint)
        {
            try
            {
                // First get the sketch plane by the giving element id.
                var workPlane = GetSketchPlaneById(sketchId);

                // Additional check: start point should not equal end point
                if (startPoint.Equals(endPoint)) throw new ArgumentException("Two points should not be the same.");

                // create geometry line
                var geometryLine = Line.CreateBound(startPoint, endPoint);
                if (null == geometryLine) // assert the creation is successful
                    throw new Exception("Create the geometry line failed.");
                // create the ModelLine
                if (!(m_createDoc.NewModelCurve(geometryLine, workPlane) is ModelLine line)) // assert the creation is successful
                    throw new Exception("Create the ModelLine failed.");
                // Add the created ModelLine into the line array
                m_lineArray.Append(line);

                // Finally refresh information map.
                RefreshInformationMap();
            }
            catch (Exception ex)
            {
                throw new Exception("Can not create the ModelLine, message: " + ex.Message);
            }
        }

        /// <summary>
        ///     Create the arc(ModelArc)
        /// </summary>
        /// <param name="sketchId">the id of the sketch plane</param>
        /// <param name="startPoint">the start point of the arc</param>
        /// <param name="endPoint">the end point of the arc</param>
        /// <param name="thirdPoint">the third point which is on the arc</param>
        public void CreateArc(ElementId sketchId, XYZ startPoint, XYZ endPoint, XYZ thirdPoint)
        {
            try
            {
                // First get the sketch plane by the giving element id.
                var workPlane = GetSketchPlaneById(sketchId);

                // Additional check: the start, end and third point should not be the same
                if (startPoint.Equals(endPoint) || startPoint.Equals(thirdPoint)
                                                || endPoint.Equals(thirdPoint))
                    throw new ArgumentException("Three points should not be the same.");

                // create the geometry arc
                var geometryArc = Arc.Create(startPoint, endPoint, thirdPoint);
                if (null == geometryArc) // assert the creation is successful
                    throw new Exception("Create the geometry arc failed.");
                // create the ModelArc
                if (!(m_createDoc.NewModelCurve(geometryArc, workPlane) is ModelArc arc)) // assert the creation is successful
                    throw new Exception("Create the ModelArc failed.");
                // Add the created ModelArc into the arc array
                m_arcArray.Append(arc);

                // Finally refresh information map.
                RefreshInformationMap();
            }
            catch (Exception ex)
            {
                throw new Exception("Can not create the ModelArc, message: " + ex.Message);
            }
        }

        /// <summary>
        ///     Create other lines, including Ellipse, HermiteSpline and NurbSpline
        /// </summary>
        /// <param name="sketchId">the id of the sketch plane</param>
        /// <param name="elementId">the element id which copy the curve from</param>
        /// <param name="offsetPoint">the offset direction from the copied line</param>
        public void CreateOthers(ElementId sketchId, ElementId elementId, XYZ offsetPoint)
        {
            // First get the sketch plane by the giving element id.
            var workPlane = GetSketchPlaneById(sketchId);

            // Because the geometry of these lines can't be created by API,
            // use an existing geometry to create ModelEllipse, ModelHermiteSpline, ModelNurbSpline
            // and then move a bit to make the user see the creation distinctly

            // This method use NewModelCurveArray() method to create model lines
            var curves = m_createApp.NewCurveArray(); // create a geometry curve array

            // Get the Autodesk.Revit.DB.ElementId which used to get the corresponding element
            if (!(GetElementById(elementId) is ModelCurve selected)) throw new Exception("Don't have the element you select");

            // add the geometry curve of the element
            curves.Append(selected.GeometryCurve); // add the geometry ellipse

            // Create the model line
            var modelCurves = m_createDoc.NewModelCurveArray(curves, workPlane);
            if (null == modelCurves || 1 != modelCurves.Size) // assert the creation is successful
                throw new Exception("Create the ModelCurveArray failed.");

            // Offset the create model lines in order to differentiate the existing model lines
            foreach (ModelCurve m in
                     modelCurves) ElementTransformUtils.MoveElement(m.Document, m.Id, offsetPoint); // move the lines
            // Add the created model lines into corresponding array
            foreach (ModelCurve m in modelCurves)
                switch (m.GetType().Name)
                {
                    case "ModelEllipse": // If the line is Ellipse
                        m_ellipseArray.Append(m); // Add to Ellipse array
                        break;
                    case "ModelHermiteSpline": // If the line is HermiteSpline
                        m_hermiteArray.Append(m); // Add to HermiteSpline array
                        break;
                    case "ModelNurbSpline": // If the line is NurbSpline
                        m_nurbArray.Append(m); // Add to NurbSpline
                        break;
                }

            // Finally refresh information map.
            RefreshInformationMap();
        }

        /// <summary>
        ///     Get all model lines in current document of revit, and store them into the arrays
        /// </summary>
        private void GetModelLines()
        {
            // Search all elements in current document and find all model lines
            // ModelLine is not supported by ElementClassFilter/OfClass, 
            // so use its base type to find all CurveElement and then process the results further to find modelline
            var modelCurves = from elem in new FilteredElementCollector(m_revit.ActiveUIDocument.Document)
                    .OfClass(typeof(CurveElement)).ToElements()
                let modelCurve = elem as ModelCurve
                where modelCurve != null
                select modelCurve;
            foreach (var modelCurve in modelCurves)
            {
                // Get all the ModelLines references
                var typeName = modelCurve.GetType().Name;
                switch (typeName)
                {
                    case "ModelLine": // Get all the ModelLine references
                        m_lineArray.Append(modelCurve);
                        break;
                    case "ModelArc": // Get all the ModelArc references
                        m_arcArray.Append(modelCurve);
                        break;
                    case "ModelEllipse": // Get all the ModelEllipse references
                        m_ellipseArray.Append(modelCurve);
                        break;
                    case "ModelHermiteSpline": // Get all the ModelHermiteSpline references
                        m_hermiteArray.Append(modelCurve);
                        break;
                    case "ModelNurbSpline": // Get all the ModelNurbSpline references
                        m_nurbArray.Append(modelCurve);
                        break;
                }
            }
        }

        /// <summary>
        ///     Get all sketch planes in revit
        /// </summary>
        private void GetSketchPlane()
        {
            // Search all elements in current document and find all sketch planes
            var elements = new FilteredElementCollector(m_revit.ActiveUIDocument.Document).OfClass(typeof(SketchPlane))
                .ToElements();
            foreach (var elem in elements)
            {
                if (elem is SketchPlane sketch)
                    // Add all the sketchPlane into the array
                    m_sketchArray.Add(sketch);
            }
        }

        /// <summary>
        ///     Initiate the information map which will display in information DataGridView
        /// </summary>
        private void InitDisplayInformation()
        {
            // First add the type name into the m_information data map
            m_informationMap.Add(new ModelCurveCounter("ModelArc"));
            m_informationMap.Add(new ModelCurveCounter("ModelLine"));
            m_informationMap.Add(new ModelCurveCounter("ModelEllipse"));
            m_informationMap.Add(new ModelCurveCounter("ModelHermiteSpline"));
            m_informationMap.Add(new ModelCurveCounter("ModelNurbSpline"));

            // Use RefreshInformationMap to refresh the number of each model line type
            RefreshInformationMap();
        }

        /// <summary>
        ///     Refresh the m_informationMap member, include the number of each model line type
        /// </summary>
        public void RefreshInformationMap()
        {
            // Search the model line types in the map, and refresh the number of each type
            foreach (var info in m_informationMap)
                switch (info.TypeName)
                {
                    case "ModelArc": // if the type is ModelAre
                        info.Number = m_arcArray.Size; // refresh the number of arc
                        break;
                    case "ModelLine": // if the type is ModelLine
                        info.Number = m_lineArray.Size; // refresh the number of line
                        break;
                    case "ModelEllipse": // If the type is ModelEllipse
                        info.Number = m_ellipseArray.Size; // refresh the number of ellipse
                        break;
                    case "ModelHermiteSpline": // If the type is ModelHermiteSpline
                        info.Number = m_hermiteArray.Size; // refresh the number of HermiteSpline
                        break;
                    case "ModelNurbSpline": // If the type is ModelNurbSpline
                        info.Number = m_nurbArray.Size; // refresh the number of NurbSpline
                        break;
                }
        }

        /// <summary>
        ///     Use Autodesk.Revit.DB.ElementId to get the corresponding element
        /// </summary>
        /// <param name="id">the element id value</param>
        /// <returns>the corresponding element</returns>
        private Element GetElementById(ElementId id)
        {
            // Get the corresponding element
            return m_revit.ActiveUIDocument.Document.GetElement(id);
        }

        /// <summary>
        ///     Use Autodesk.Revit.DB.ElementId to get the corresponding sketch plane
        /// </summary>
        /// <param name="id">the element id value</param>
        /// <returns>the corresponding sketch plane</returns>
        private SketchPlane GetSketchPlaneById(ElementId id)
        {
            // First get the sketch plane by the giving element id.
            if (!(GetElementById(id) is SketchPlane workPlane)) throw new Exception("Don't have the work plane you select.");
            return workPlane;
        }
    }
}

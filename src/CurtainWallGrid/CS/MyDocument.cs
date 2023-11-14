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

using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.CurtainWallGrid.CS
{
    /// <summary>
    /// maintains all the data used in the sample
    /// </summary>
    public class MyDocument
    {
        #region Fields
        // object which contains reference of Revit Application
        /// <summary>
        /// object which contains reference of Revit Application
        /// </summary>
        public ExternalCommandData CommandData { get; }

        // the active document of Revit
        /// <summary>
        /// the active document of Revit
        /// </summary>
        public UIDocument UIDocument { get; }

        // the active document of Revit
        /// <summary>
        /// the active document of Revit
        /// </summary>
        public Document Document { get; }

        // stores all the Curtain WallTypes in the active Revit document

        // stores all the ViewPlans in the active Revit document

        // stores the wall creation related data and operations

        // stores the curtain wall created

        // indicates whether the curtain wall has been created

        // store the grid information of the created curtain wall

        // store the active grid line operation

        // the length unit type for the active Revit document

        // store the message of the sample
        private KeyValuePair<string/*msgText*/, bool/*is warningOrError*/> m_message;




        /// <summary>
        /// stores all the Curtain WallTypes in the active Revit document 
        /// </summary>
        public List<WallType> WallTypes { get; private set; }

        /// <summary>
        /// stores all the ViewPlans in the active Revit document
        /// </summary>
        public List<View> Views { get; private set; }

        /// <summary>
        /// stores the wall creation related data and operations
        /// </summary>
        public WallGeometry WallGeometry { get; }

        /// <summary>
        /// stores the curtain wall created
        /// </summary>
        public Wall CurtainWall { get; set; }

        /// <summary>
        /// indicates whether the curtain wall has been created
        /// </summary>
        public bool WallCreated { get; set; }

        /// <summary>
        /// store the grid information of the created curtain wall
        /// </summary>
        public GridGeometry GridGeometry { get; }

        /// <summary>
        /// store the active grid line operation
        /// </summary>
        public LineOperation ActiveOperation { get; set; }

        public ForgeTypeId LengthUnit { get; private set; }

        /// <summary>
        /// store the message of the sample
        /// </summary>
        public KeyValuePair<string/*msgText*/, bool/*is warningOrError*/> Message
        {
            get => m_message;
            set
            {
                m_message = value;
                if (null != MessageChanged)
                {
                    MessageChanged();
                }
            }
        }
        #endregion

        #region Delegates and Events
        // occurs only when the message was updated
        public delegate void MessageChangedHandler();
        public event MessageChangedHandler MessageChanged;
        #endregion

        #region Constructors
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="commandData">
        /// object which contains reference of Revit Application
        /// </param>
        public MyDocument(ExternalCommandData commandData)
        {
            if (null != commandData.Application.ActiveUIDocument)
            {
                CommandData = commandData;
                UIDocument = CommandData.Application.ActiveUIDocument;
                Document = UIDocument.Document;
                Views = new List<View>();
                WallTypes = new List<WallType>();
                WallGeometry = new WallGeometry(this);
                WallCreated = false;
                GridGeometry = new GridGeometry(this);

                // get all the wall types and all the view plans
                InitializeData();

                // get the length unit type of the active Revit document
                GetLengthUnitType();

                // initialize the curtain grid operation type
                ActiveOperation = new LineOperation(LineOperationType.Waiting);
            }
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Get current length display unit type
        /// </summary>
        private void GetLengthUnitType()
        {
            var specTypeId = SpecTypeId.Length;
            var projectUnit = Document.GetUnits();
            try
            {
                var formatOption = projectUnit.GetFormatOptions(specTypeId);
                LengthUnit = formatOption.GetUnitTypeId();
            }
            catch (System.Exception /*e*/)
            {
                LengthUnit = UnitTypeId.Feet;
            }
        }

        /// <summary>
        /// get all the wall types for curtain wall and all the view plans from the active document
        /// </summary>
        private void InitializeData()
        {
            // get all the wall types
            var filteredElementCollector = new FilteredElementCollector(Document);
            filteredElementCollector.OfClass(typeof(WallType));
            // just get all the curtain wall type
            WallTypes = filteredElementCollector.Cast<WallType>().Where<WallType>(wallType => wallType.Kind == WallKind.Curtain).ToList<WallType>();

            // sort them alphabetically
            var wallComp = new WallTypeComparer();
            WallTypes.Sort(wallComp);

            // get all the ViewPlans
            Views = SkipTemplateViews(GetElements<View>());

            // sort them alphabetically
            var viewComp = new ViewComparer();
            Views.Sort(viewComp);

            // get one of the mullion types
            var mullTypes = Document.MullionTypes;
            foreach (MullionType type in mullTypes)
            {
                if (null != type)
                {
                    var bip = BuiltInParameter.ALL_MODEL_FAMILY_NAME;
                    var para = type.get_Parameter(bip);
                    if (null != para)
                    {
                        var name = para.AsString().ToLower();
                        if (name.StartsWith("circular mullion"))
                        {
                            GridGeometry.MullionType = type;
                        }
                    }

                }
            }
        }

        protected List<T> GetElements<T>() where T : Element
        {
            var returns = new List<T>();
            var collector = new FilteredElementCollector(Document);
            ICollection<Element> founds = collector.OfClass(typeof(T)).ToElements();
            foreach (var elem in founds)
            {
                returns.Add(elem as T);
            }
            return returns;
        }

        /// <summary>
        /// View elements filtered by new iteration will include template views.
        /// These views are not invalid for test(because they're invisible in project browser) 
        /// Skip template views for regression test
        /// </summary>
        /// <param name="views"></param>
        /// <returns></returns>
        private List<T> SkipTemplateViews<T>(List<T> views) where T : View
        {
            var returns = new List<T>();
            foreach (View curView in views)
            {
                if (null != curView && !curView.IsTemplate)
                    returns.Add(curView as T);
            }
            return returns;
        }
        #endregion
    }
}

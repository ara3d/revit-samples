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
using System.Xml.Linq;
using Autodesk.Revit.DB;

namespace Revit.SDK.Samples.CS.PointCloudEngine
{
    /// <summary>
    /// Utilities used by the sample to process XML entries in file-based point clouds.
    /// </summary>
    public static class XmlUtils
    {
        /// <summary>
        /// Gets an XYZ point from an XML element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The XYZ.</returns>
        public static XYZ GetXYZ(XElement element)
        {
            var x = element.Attribute("X");
            var y = element.Attribute("Y");
            var z = element.Attribute("Z");

            return new XYZ(double.Parse(x.Value), double.Parse(y.Value), double.Parse(z.Value));
        }

        /// <summary>
        /// Gets a boolean value from an XML element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The value.</returns>
        public static bool GetBoolean(XElement element)
        {
            return bool.Parse(element.Attribute("value").Value);
        }

        /// <summary>
        /// Gets a double value from an XML element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The value.</returns>
        public static double GetDouble(XElement element)
        {
            return double.Parse(element.Attribute("value").Value);
        }

        /// <summary>
        /// Gets an integer value from an XML element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The value.</returns>
        public static int GetInteger(XElement element)
        {
            return int.Parse(element.Attribute("value").Value);
        }

        /// <summary>
        /// Gets a color value (in the form needed for inclusion in a CloudPoint) from an XML element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The value.</returns>
        public static int GetColor(XElement element)
        {
            return System.Drawing.ColorTranslator.ToWin32(System.Drawing.ColorTranslator.FromHtml(element.Attribute("value").Value));
        }

        /// <summary>
        /// Gets the XML element representing a point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="name">The name of the XML element.</param>
        /// <returns>The element.</returns>
        public static XElement GetXElement(XYZ point, string name)
        {
            var ret = new XElement(name);
            ret.Add(new XAttribute("X", point.X));
            ret.Add(new XAttribute("Y", point.Y));
            ret.Add(new XAttribute("Z", point.Z));

            return ret;
        }

        /// <summary>
        /// Gets the XML element representing a CloudPoint color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="name">The name.</param>
        /// <returns>The element.</returns>
        public static XElement GetColorXElement(int color, string name)
        {
            var ret = new XElement(name);
            var htmlRep = System.Drawing.ColorTranslator.ToHtml(System.Drawing.ColorTranslator.FromWin32(color));
            ret.Add(new XAttribute("value", htmlRep));

            return ret;
        }

        /// <summary>
        /// Gets the XML element representing an object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="name">The name.</param>
        /// <returns>The element.</returns>
        public static XElement GetXElement(object obj, string name)
        {
            var ret = new XElement(name);
            ret.Add(new XAttribute("value", obj.ToString()));

            return ret;
        }
    }

}
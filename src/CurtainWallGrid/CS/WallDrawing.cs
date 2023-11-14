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
using System.Drawing;

namespace Revit.SDK.Samples.CurtainWallGrid.CS
{
   /// <summary>
   /// maintain the baseline of the curtain wall
   /// </summary>
   public class WallDrawing
   {
      #region Fields
      // zoom the baseline to a suitable length
      public double SCALEFACTOR = 5.0;

      // the boundary of the canvas for the baseline drawing

      // the origin for the baseline (it's the center of the canvas)
      Point m_origin;

      // the referred parent wall geometry
      WallGeometry m_refGeometry;

      // store the document of this sample
      MyDocument m_myDocument;

      // the font used in drawing the baseline of the curtain wall
      Font m_coordinateFont;

      // the baseline of the curtain wall

      #endregion

      #region Properties
      /// <summary>
      /// the boundary of the canvas for the baseline drawing
      /// </summary>
      public Rectangle Boundary { get; set; }

      /// <summary>
      /// the origin for the baseline (it's the center of the canvas)
      /// </summary>
      public Point Origin
      {
         get => m_origin;
         set => m_origin = value;
      }

      /// <summary>
      /// the baseline of the curtain wall
      /// </summary>
      public WallBaseline2D WallLine2D { get; }

      #endregion

      #region Constructors
      /// <summary>
      /// default constructor
      /// </summary>
      /// <param name="wallGeo">
      /// the mapped wall geometry information
      /// </param>
      public WallDrawing(WallGeometry wallGeo)
      {
         m_coordinateFont = new Font("Verdana", 10, FontStyle.Regular);
         WallLine2D = new WallBaseline2D();
         m_myDocument = wallGeo.MyDocument;
         m_refGeometry = wallGeo;
      }
      #endregion

      #region Public methods
      /// <summary>
      /// Add point to baseline of the curtain wall
      /// </summary>
      /// <param name="mousePosition">
      /// the location of the mouse cursor
      /// </param>
      public void AddPoint(Point mousePosition)
      {
         // both end points of the baseline specified, can't add more points
         if (Point.Empty != WallLine2D.StartPoint &&
             Point.Empty != WallLine2D.EndPoint)
         {
            return;
         }

         // start point isn't specified, specify it
         if (Point.Empty == WallLine2D.StartPoint)
         {
            WallLine2D.StartPoint = mousePosition;
            m_refGeometry.StartPointD = ConvertToPointD(mousePosition);
         }
         // start point specified, end point isn't, so specify the end point
         else if (Point.Empty == WallLine2D.EndPoint)
         {
            // don't let the length of the 2 points too small
            if (Math.Abs(WallLine2D.StartPoint.X - mousePosition.X) >= 2 ||
                Math.Abs(WallLine2D.StartPoint.Y - mousePosition.Y) >= 2)
            {
               WallLine2D.EndPoint = mousePosition;
               m_refGeometry.EndPointD = ConvertToPointD(mousePosition);
               WallLine2D.AssistantPoint = Point.Empty;
            }
         }
      }

      /// <summary>
      /// store mouse position when mouse moves (the location will be the candidate end points of the baseline)
      /// </summary>
      /// <param name="mousePosition">
      /// the location of the mouse cursor
      /// </param>
      public void AddMousePosition(Point mousePosition)
      {
         // both endpoints for the baseline have been confirmed, no need to record the mouse location
         if (Point.Empty != WallLine2D.StartPoint &&
             Point.Empty != WallLine2D.EndPoint)
         {
            return;
         }

         // we just start to draw the baseline, no end points are specified
         // or just the start point was specified, so the mouse position will be the "candidate end point"
         WallLine2D.AssistantPoint = mousePosition;
      }

      /// <summary>
      /// draw the baseline for the curtain wall creation in the picture box
      /// in the "Create Curtain Wall" tab page, user needs to draw the baseline for wall creation
      /// </summary>
      /// <param name="graphics">
      /// form graphic
      /// </param>
      /// <param name="pen">
      /// pen used to draw line in pictureBox
      /// </param>
      public void Draw(Graphics graphics, Pen pen)
      {
         // draw the coordinate system origin
         DrawCoordinateOrigin(graphics, pen);

         // draw the baseline
         DrawBaseline(graphics, pen);
      }

      /// <summary>
      /// Clear points in baseline
      /// </summary>
      public void RemovePoints()
      {
         WallLine2D.Clear();
      }
      #endregion

      #region Private methods
      /// <summary>
      /// scale the point and store them in PointD format
      /// </summary>
      /// <param name="srcPoint">
      /// the point to-be-zoomed
      /// </param>
      /// <returns>
      /// the scaled result point
      /// </returns>
      private PointD ConvertToPointD(Point srcPoint)
      {
          double x = srcPoint.X - m_origin.X;
         double y = m_origin.Y - srcPoint.Y;
         x /= SCALEFACTOR;
         y /= SCALEFACTOR;
         return new PointD(x, y);
      }

      /// <summary>
      /// draw the coordinate system origin for the baseline drawing
      /// </summary>
      /// <param name="graphics">
      /// form graphic
      /// </param>
      /// <param name="pen">
      /// pen used to draw line in pictureBox
      /// </param>
      private void DrawCoordinateOrigin(Graphics graphics, Pen pen)
      {
         // draw the coordinate system origin
         graphics.DrawLine(pen, new Point(m_origin.X - 10, m_origin.Y), new Point(m_origin.X + 10, m_origin.Y));
         graphics.DrawLine(pen, new Point(m_origin.X, m_origin.Y - 10), new Point(m_origin.X, m_origin.Y + 10));
         graphics.DrawString("(0,0)", m_coordinateFont, Brushes.Blue, new PointF(m_origin.X + 2, m_origin.Y + 2));
      }

      /// <summary>
      /// draw the baseline / the candidate baseline (start point confirmed, end point didn't)
      /// </summary>
      /// <param name="graphics">
      /// form graphic
      /// </param>
      /// <param name="pen">
      /// pen used to draw line in pictureBox
      /// </param>
      private void DrawBaseline(Graphics graphics, Pen pen)
      {
         if (Point.Empty != WallLine2D.AssistantPoint)
         {
            if (Point.Empty != WallLine2D.StartPoint)
            {
               graphics.DrawLine(pen, WallLine2D.StartPoint, WallLine2D.AssistantPoint);
            }

            // show the real-time coordinate of the mouse position 
            WriteCoordinate(graphics, pen);
         }

         if (Point.Empty != WallLine2D.EndPoint &&
             Point.Empty != WallLine2D.EndPoint)
         {
            graphics.DrawLine(pen, WallLine2D.StartPoint, WallLine2D.EndPoint);
         }
      }

      /// <summary>
      /// write the coordinate for moving mouse
      /// </summary>
      /// <param name="graphics">
      /// form graphic
      /// </param>
      /// <param name="pen">
      /// pen used to draw line in pictureBox
      /// </param>
      private void WriteCoordinate(Graphics graphics, Pen pen)
      {
         var assistPointD = ConvertToPointD(WallLine2D.AssistantPoint);
         var x = Unit.CovertFromAPI(m_myDocument.LengthUnit, assistPointD.X);
         var y = Unit.CovertFromAPI(m_myDocument.LengthUnit, assistPointD.Y);

         var xCoorString = Convert.ToString(Math.Round(x, 1));
         var yCoorString = Convert.ToString(Math.Round(y, 1));

         var unitType = Unit.GetUnitLabel(m_myDocument.LengthUnit);
         var coordinate = "(" + xCoorString + unitType + "," + yCoorString + unitType + ")";
         graphics.DrawString(coordinate, m_coordinateFont, Brushes.Blue,
             new PointF(WallLine2D.AssistantPoint.X + 2, WallLine2D.AssistantPoint.Y + 2));
      }
      #endregion
   }
}
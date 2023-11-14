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

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.DisplacementElementAnimation.CS
{
    /// <summary>
    ///     The command that initializes and starts the model animation.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    internal class DisplacementStructureModelAnimatorCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            new DisplacementStructureModelAnimator(commandData.Application, true).StartAnimation();

            return Result.Succeeded;
        }
    }

    /// <summary>
    ///     The command that initializes and starts the model animation step by step.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    internal class DisplacementStructureModelAnimatorCommandStepByStep : IExternalCommand
    {
        public static DisplacementStructureModelAnimator m_displacementstructuremodelAnimator;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (m_displacementstructuremodelAnimator == null)
            {
                m_displacementstructuremodelAnimator =
                    new DisplacementStructureModelAnimator(commandData.Application, false);
                m_displacementstructuremodelAnimator.StartAnimation();
            }
            else
            {
                m_displacementstructuremodelAnimator.AnimateNextStep();
            }

            return Result.Succeeded;
        }
    }
}
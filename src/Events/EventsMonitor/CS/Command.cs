// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.Events.EventsMonitor.CS
{
    /// <summary>
    ///     This class inherits IExternalCommand interface and used to retrieve the events setting
    ///     dialog to help user changing his choices. None event is register in this class.
    ///     Because all trigger points in this sample come from UI, all events in application level
    ///     must be registered to ControlledApplication. If the trigger point is from API,
    ///     user can register it to Application or Document according to what level it is in.
    ///     But then, the syntax is the same in these three cases.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            // These #if directives within file are used to compile project in different purpose:
            // . Build project with Release mode for regression test,
            // . Build project with Debug mode for manual run
#if !(Debug || DEBUG)
            // playing journal.
            if (ExternalApplication.JnlProcessor.IsReplay)
            {
                ExternalApplication.ApplicationEvents =
 ExternalApplication.JnlProcessor.GetEventsListFromJournalData(commandData.JournalData);

            }

            // running the sample form UI.
            else
            {
#endif

            ExternalApplication.SettingDialog.ShowDialog();
            if (DialogResult.OK == ExternalApplication.SettingDialog.DialogResult)
            {
                // get what user select.
                ExternalApplication.ApplicationEvents = ExternalApplication.SettingDialog.AppSelectionList;
                IDictionary<string, string> journalData = commandData.JournalData;

#if !(Debug || DEBUG)
                    // dump what user select to a file in order to autotesting.
                    ExternalApplication.JnlProcessor.DumpEventListToJournalData(ExternalApplication.ApplicationEvents, ref journalData);
#endif
            }
#if !(Debug || DEBUG)
            }
#endif
            // update the events according to the selection.
            ExternalApplication.AppEventMgr.Update(ExternalApplication.ApplicationEvents);

            // track the selected events by showing the information in the information windows.
            ExternalApplication.InfoWindows.Show();

            return Result.Succeeded;
        }
    }
}

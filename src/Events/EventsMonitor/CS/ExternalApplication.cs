// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.Events.EventsMonitor.CS
{
    /// <summary>
    ///     A class inherits IExternalApplication interface and provide an entry of the sample.
    ///     This class controls other function class and plays the bridge role in this sample.
    ///     It create a custom menu "Track Revit Events" of which the corresponding
    ///     external command is the command in this project.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class ExternalApplication : IExternalApplication
    {
        /// <summary>
        ///     A controlled application used to register the events. Because all trigger points
        ///     in this sample come from UI, all events in application level must be registered
        ///     to ControlledApplication. If the trigger point is from API, user can register it
        ///     to Application or Document according to what level it is in. But then,
        ///     the syntax is the same in these three cases.
        /// </summary>
        private static UIControlledApplication _ctrlApp;

        /// <summary>
        ///     A common object used to write the log file and generate the data table
        ///     for event information.
        /// </summary>
        private static LogManager _logManager;

        /// <summary>
        ///     The window is used to show events' information.
        /// </summary>
        private static EventsInfoWindows _infWindows;

        /// <summary>
        ///     The window is used to choose what event to be subscribed.
        /// </summary>
        private static EventsSettingForm _settingDialog;

        /// <summary>
        ///     This list is used to store what user selected.
        ///     It can be got from the selectionDialog.
        /// </summary>
        private static List<string> _appEventsSelection;

        /// <summary>
        ///     This object is used to manager the events in application level.
        ///     It can be updated according to what user select.
        /// </summary>
        private static EventManager _appEventMgr;

        // These #if directives within file are used to compile project in different purpose:
        // . Build project with Release mode for regression test,
        // . Build project with Debug mode for manual run
#if !(Debug || DEBUG)
        /// <summary>
        /// This object is used to make the sample can be autotest.
        /// It can dump the event list to file or commandData.Data
        /// and also can retrieval the list from file and commandData.Data.
        /// If you just pay attention to how to use events, 
        /// please skip over this class and related sentence.
        /// </summary>
        private static JournalProcessor m_journalProcessor;
#endif

        /// <summary>
        ///     Property to get and set private member variable of InfoWindows
        /// </summary>
        public static EventsInfoWindows InfoWindows
        {
            get => _infWindows ?? (_infWindows = new EventsInfoWindows(EventLogManager));
            set => _infWindows = value;
        }

        /// <summary>
        ///     Property to get private member variable of SeletionDialog
        /// </summary>
        public static EventsSettingForm SettingDialog 
            => _settingDialog ?? (_settingDialog = new EventsSettingForm());

        /// <summary>
        ///     Property to get and set private member variable of log data
        /// </summary>
        public static LogManager EventLogManager 
            => _logManager ?? (_logManager = new LogManager());

        /// <summary>
        ///     Property to get and set private member variable of application events selection.
        /// </summary>
        public static List<string> ApplicationEvents
        {
            get => _appEventsSelection ?? (_appEventsSelection = new List<string>());
            set => _appEventsSelection = value;
        }

        /// <summary>
        ///     Property to get private member variable of application events manager.
        /// </summary>
        public static EventManager AppEventMgr 
            => _appEventMgr ?? (_appEventMgr = new EventManager(_ctrlApp));

#if !(Debug || DEBUG)
        /// <summary>
        /// Property to get private member variable of journal processor.
        /// </summary>
        public static JournalProcessor JnlProcessor
        {
            get
            {
                if (null == m_journalProcessor)
                {
                    m_journalProcessor = new JournalProcessor();
                }
                return m_journalProcessor;
            }
        }
#endif

        /// <summary>
        ///     Implement this method to implement the external application which should be called when
        ///     Revit starts before a file or default template is actually loaded.
        /// </summary>
        /// <param name="application">
        ///     An object that is passed to the external application
        ///     which contains the controlled application.
        /// </param>
        /// <returns>
        ///     Return the status of the external application.
        ///     A result of Succeeded means that the external application successfully started.
        ///     Cancelled can be used to signify that the user cancelled the external operation at
        ///     some point.
        ///     If false is returned then Revit should inform the user that the external application
        ///     failed to load and the release the public reference.
        /// </returns>
        public Result OnStartup(UIControlledApplication application)
        {
            // initialize member variables.
            _ctrlApp = application;
            _logManager = new LogManager();
            _infWindows = new EventsInfoWindows(_logManager);
            _settingDialog = new EventsSettingForm();
            _appEventsSelection = new List<string>();
            _appEventMgr = new EventManager(_ctrlApp);

#if !(Debug || DEBUG)
            m_journalProcessor = new JournalProcessor();
#endif

            try
            {
#if !(Debug || DEBUG)
                // playing journal.
                if (m_journalProcessor.IsReplay)
                {
                    _appEventsSelection = m_journalProcessor.EventsList;
                }
                
                // running the sample form UI.
                else
                {
#endif
                _settingDialog.ShowDialog();
                if (DialogResult.OK == _settingDialog.DialogResult)
                    //get what user select.
                    _appEventsSelection = _settingDialog.AppSelectionList;

#if !(Debug || DEBUG)
                    // dump what user select to a file in order to autotesting.
                    m_journalProcessor.DumpEventsListToFile(_appEventsSelection);
                }
#endif

                // update the events according to the selection.
                _appEventMgr.Update(_appEventsSelection);

                // track the selected events by showing the information in the information windows.
                _infWindows.Show();

                // add menu item in Revit menu bar to provide an approach to 
                // retrieve events setting form. User can change his choices 
                // by calling the setting form again.
                AddCustomPanel(application);
            }
            catch (Exception)
            {
                return Result.Failed;
            }

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            // dispose some resource.
            Dispose();
            return Result.Succeeded;
        }

        /// <summary>
        ///     Dispose some resource.
        /// </summary>
        public static void Dispose()
        {
            if (_infWindows != null)
            {
                _infWindows.Close();
                _infWindows = null;
            }

            if (_settingDialog != null)
            {
                _settingDialog.Close();
                _settingDialog = null;
            }

            _appEventMgr = null;
            _logManager.CloseLogFile();
            _logManager = null;
        }

        /// <summary>
        ///     Add custom menu.
        /// </summary>
        private static void AddCustomPanel(UIControlledApplication application)
        {
            // create a panel named "Events Monitor";
            var panelName = "Events Monitor";
            // create a button on the panel.
            var ribbonPanelPushButtons = application.CreateRibbonPanel(panelName);
            var pushButtonData = new PushButtonData("EventsSetting",
                "Set Events", Assembly.GetExecutingAssembly().Location,
                "Ara3D.RevitSampleBrowser.EventsMonitor.CS.Command");
            var pushButtonCreateWall = ribbonPanelPushButtons.AddItem(pushButtonData) as PushButton;
            pushButtonCreateWall.ToolTip = "Setting Events";
        }
    }
}

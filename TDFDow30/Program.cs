using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TDFDow30
{
    // Assemby reference for logger - done once per application
    //[assembly: log4net.Config.XmlConfigurator(Watch = true)]

    static class Program
    {
        #region Logger instantiation - uses reflection to get module name; needs to be done for each class
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // Need to call this in the main loop to init the logger
                log4net.Config.XmlConfigurator.Configure();

                // Set the unhandled exception mode to force all Windows Forms errors to go through our handler.
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

                var mainForm = new frmMain();
                ((log4net.Repository.Hierarchy.Hierarchy)log4net.LogManager.GetRepository()).Root.AddAppender(mainForm);
                Application.Run(mainForm);

                
                // Register this event to capture and log unhandled exceptions
                AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                {
                    var ex = (Exception)e.ExceptionObject;
                    // Log error
                    log.Debug("Unhandled exception occurred", ex);
                    log.Error("Unhandled exception occurred: " + ex.Message);
                };

            }
            catch (Exception ex)
            {
                // Top-level error dialog if exception bubbles up
                //MessageBox.Show("General error occurred with application. Please re-start to ensure proper operation.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Log the error 
                //log.Debug("General exception occurred at main program level", ex);
                log.Error("General exception occurred at main program level: " + ex.Message);
            }


        }
    }
}

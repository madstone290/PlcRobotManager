using PlcRobotManager.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PlcRobotManager.Ui
{
    internal static class Program
    {
        internal static IRobotManager RobotManager { get; private set; }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += Application_ThreadException;

            var robotManager = Demo.Run(1).GetAwaiter().GetResult();
            RobotManager = robotManager;

            var form = new FormMain();

            form.FormClosed += async (s, e) =>
            {
                await Demo.Stop(robotManager);
            };

            Application.Run(form);
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message);
        }
    }
}

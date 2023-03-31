using PlcRobotManager.Core;
using PlcRobotManager.Core.Infos;
using PlcRobotManager.Ui.Inputs;
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

            List<RobotInfo> robotInfos = new RobotFileReader().Read("RobotConfig/robot_list.json");
            var robotManager = RobotManagerHelper.Run(robotInfos).GetAwaiter().GetResult();
            RobotManager = robotManager;

            var form = new FormMain();

            form.FormClosed += async (s, e) =>
            {
                await RobotManagerHelper.Stop(robotManager);
            };

            Application.Run(form);
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message);
        }
    }
}

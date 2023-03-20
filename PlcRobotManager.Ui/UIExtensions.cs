using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PlcRobotManager.Ui
{
    public static class UIExtentions
    {
        public static void InvokeSafe(this Control control, Action action)
        {
            control.Invoke((MethodInvoker)delegate {
                action.Invoke();
            });
        }
    }
}

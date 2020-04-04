using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ATM
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            mainForm = new Form1();
            Application.Run(mainForm);
            //Application.Run(new Form1());


        }
        static Form1 mainForm;
        public static void sendStatusMessage(string statusMessage)
        {
            mainForm.sendStatusMessage(statusMessage);
        }
    }
}

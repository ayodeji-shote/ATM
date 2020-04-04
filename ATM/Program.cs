using System;
using System.Collections.Generic;
using System.Drawing;
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
        public static void showMainForm()
        {
            mainForm.Show();
        }
        public static void setIcon(Form form)
        {
            form.Icon = Icon.ExtractAssociatedIcon("icon.ico");
        }
        public static void formset(Form pl)
        {
            pl.Width = 500;
            pl.Height = 500;
            pl.MaximumSize = pl.Size;
            pl.MinimumSize = pl.Size;
            pl.BackColor = Color.White;
            setIcon(pl);

        }
    }
}

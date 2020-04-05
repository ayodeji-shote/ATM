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
            mainForm = new BaseForm();
            Application.Run(mainForm);


        }
        //store our main form
        static BaseForm mainForm;
        // get random image for bank
        public static string[] getImages()
        {
            string[] bankNames = new string[8];
            bankNames[0] = "Barclays";
            bankNames[1] = "BOS";
            bankNames[2] = "HSBC";
            bankNames[3] = "Nationwide";
            bankNames[4] = "RBS";
            bankNames[5] = "Santander";
            bankNames[6] = "Standard Chartered";
            bankNames[7] = "TSB";

            Random rand = new Random();
            int randNum = rand.Next(7);

            string[] images = new string[3];
            images[0] = "Card in - " + bankNames[randNum] + ".png";
            images[1] = "Card " + bankNames[randNum] + ".png";
            images[2] = "Screen " + bankNames[randNum] + ".png";
            //"Card in - (bank name).png"
            //"Card (bank name).png"
            //Screen (bank name.png"
            return images;
        }
        //send a status message to the main form
        public static void sendStatusMessage(string statusMessage)
        {
            mainForm.sendStatusMessage(statusMessage);
        }
        // show the mainform
        public static void showMainForm()
        {
            mainForm.Show();
        }
        //set icon for a form to the icon
        public static void setIcon(Form form)
        {
            form.Icon = Icon.ExtractAssociatedIcon("icon.ico");
        }
        // set size colour and icon on form
        public static void formset(Form pl)
        {
            pl.Width = 600;
            pl.Height = 550;
            pl.MaximumSize = pl.Size;
            pl.MinimumSize = pl.Size;
            pl.BackColor = Color.GhostWhite;
            setIcon(pl);

        }
    }
}

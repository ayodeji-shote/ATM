using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ATM
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            startingForm();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            Formset(this);

        }


        public void startingForm()
        {
            Form f = new Form();
            Label mylab = new Label();
            mylab.Text = " ATM simulator ";
            mylab.AutoSize = true;
            mylab.Font = new Font("Calibri", 25);
            mylab.ForeColor = Color.Black;
            mylab.Location = new Point(this.Width/5, 90);
            this.Controls.Add(mylab);
            Button MyB = new Button();
            Button MyB2 = new Button();
            MyB.Text = " Base program ";
            MyB.AutoSize = true;
            MyB.BackColor = Color.LightBlue;
            MyB.Padding = new Padding(6);
            MyB.Font = new Font("Calibri", 18);
            MyB.Location = new Point(150, 180);
            this.Controls.Add(MyB);
            MyB2.Text = " Changed program ";
            MyB2.AutoSize = true;
            MyB2.BackColor = Color.LightBlue;
            MyB2.Padding = new Padding(6);
            MyB2.Font = new Font("Calibri", 18);
            MyB2.Location = new Point(130, 280);
            this.Controls.Add(MyB2);
            MyB.Click += (sender, EventArgs) => { MyB_Click(sender, EventArgs, f); }; // this sets the button click to the event handler.
            MyB2.Click += (sender, EventArgs) => { MyB_Click(sender, EventArgs, f); }; // this sets the button click to the event handler.

        }

        private void MyB_Click(object sender, EventArgs e, Form F)
        {
            Formset(F);
            F.Show();
            this.Hide();
            MultATM(F);
        }

        public void MultATM(Form F)
        {
            Label mylab = new Label();
            mylab.Text = " How many ATM's do you want to simulate ";
            mylab.AutoSize = true;
            mylab.Font = new Font("Calibri", 20);
            mylab.ForeColor = Color.Black;
            mylab.Location = new Point(0, 90);
            F.Controls.Add(mylab);
            Button[,] btn = new Button[3, 3];
            int st = 1;
            for (int y = 0; y < 3; y++) // Loop for each button
            {
                for (int x = 0; x < 3; x++) // Loop for y
                {
                    btn[x, y] = new Button(); // Create button
                    btn[x, y].Text = st++.ToString();
                    btn[x, y].SetBounds((x*50)+170, (y*40)+200, 40, 40); // Set size & position
                    F.Controls.Add(btn[x, y]);
                    int k = int.Parse(btn[x, y].Text);
                    btn[x,y].Click += (sender, EventArgs) => { btn_Click(sender, EventArgs, F,k); };
                }
            }
        }
        private void btn_Click(object sender, EventArgs e, Form F, int p)
        {
            Form[] bs = new Form[p];
            for (int y = 0; y < p; y++)
            {
                bs[y] = new Form();
            }
            for (int y = 0; y < p; y++)
            {
                Formset(bs[y]);
                Button[,] btn = new Button[3, 3];
                int st = 1;
                for (int n = 0; n < 3; n++) // Loop for each button
                {
                    for (int x = 0; x < 3; x++) // Loop for y
                    {
                        btn[x, n] = new Button(); // Create button
                        btn[x, n].Text = st++.ToString();
                        btn[x, n].SetBounds((x * 60) + 140, (n * 50) + 240, 50, 50); // Set size & position
                        bs[y].Controls.Add(btn[x, n]);
                        int k = int.Parse(btn[x, n].Text);
                    }
                }
                ATMMaker(bs,y);
                Optionmaker(bs, y);
            }
            F.Hide();
        }
        public void ATMMaker(Form[] bs,int s)
        {
            Button[] btt = new Button[3];
            btt[0] = new Button();
            btt[1] = new Button();
            btt[2] = new Button();
            btt[0].Text = "00";
            btt[1].Text = "0";
            btt[2].Text = ".";
            btt[0].SetBounds(140, 390, 50, 50); // Set size & position
            btt[1].SetBounds(200, 390, 50, 50);
            btt[2].SetBounds(260, 390, 50, 50);
            bs[s].Controls.Add(btt[0]);
            bs[s].Controls.Add(btt[1]);
            bs[s].Controls.Add(btt[2]);
            bs[s].Show();
        }
        public void Optionmaker(Form[] bs, int s)
        {
            Button[] btt = new Button[4];
            btt[0] = new Button();
            btt[1] = new Button();
            btt[2] = new Button();
            btt[3] = new Button();
            btt[0].Font = new Font("Calibri", 6);
            btt[1].Font = new Font("Calibri", 7);
            btt[2].Font = new Font("Calibri", 7);
            btt[0].Text = "Cancel";
            btt[1].Text = "Clear";
            btt[2].Text = "Enter";
            btt[0].BackColor = Color.Red;
            btt[0].FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btt[0].FlatAppearance.BorderSize = 0;

            btt[1].BackColor = Color.Yellow;
            btt[1].FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btt[1].FlatAppearance.BorderSize = 0;

            btt[2].BackColor = Color.Green;
            btt[2].FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btt[2].FlatAppearance.BorderSize = 0;

            btt[0].SetBounds(330, 243, 70, 40); // Set size & position
            btt[1].SetBounds(330, 292, 70, 40);
            btt[2].SetBounds(330, 344, 70, 40);
            btt[3].SetBounds(330, 396, 70, 40);

            bs[s].Controls.Add(btt[0]);
            bs[s].Controls.Add(btt[1]);
            bs[s].Controls.Add(btt[2]);
            bs[s].Controls.Add(btt[3]);
            bs[s].Show();

            Label mylab = new Label();
            mylab.BackColor = Color.Red;
            mylab.SetBounds(140, 80, 260, 110);
            bs[s].Controls.Add(mylab);

        }
        private void MyB2_Click(object sender, EventArgs e, Form F)
        {
            Formset(F);
            F.Show();
            this.Hide();
        }
        public Form Formset(Form pl)
        {
            pl.Width = 500;
            pl.Height = 500;
            pl.MaximumSize = this.Size;
            pl.MinimumSize = this.Size;
            return pl;
        }


    }
     /*
     *   This is the root of program and the entry point
     * 
     *   Class programm contains an array of account objects and a singel ATM object  
     * 
     */
    class Program1
    {
        private Account[] ac = new Account[3];
        private ATM atm;
       
        /*
         * This function initilises the 3 accounts 
         * and instanciates the ATM class passing a referance to the account information
         * 
         */
        public Program1()
        {
            ac[0] = new Account(300, 1111, 111111);
            ac[1] = new Account(750, 2222, 222222);
            ac[2] = new Account(3000, 3333, 333333);

            atm = new ATM(ac);

        }

        //static void Main(string[] args)
        //{
        //    new Program1();
        //}
    }
    /*
     *   The Account class encapusulates all features of a simple bank account
     */ 
    class Account
    {
        //the attributes for the account
        private int balance;
        private int pin;
        private int accountNum;

        // a constructor that takes initial values for each of the attributes (balance, pin, accountNumber)
        public Account(int balance, int pin, int accountNum)
        {
            this.balance = balance;
            this.pin = pin;
            this.accountNum = accountNum;
        }

        //getter and setter functions for balance
        public int getBalance()
        {
            return balance;
        }
        public void setBalance(int newBalance)
        {
            this.balance = newBalance;
        }

        /*
         *   This funciton allows us to decrement the balance of an account
         *   it perfomes a simple check to ensure the balance is greater tha
         *   the amount being debeted
         *   
         *   reurns:
         *   true if the transactions if possible
         *   false if there are insufficent funds in the account
         */
        public Boolean decrementBalance(int amount)
        {
            if (this.balance > amount)
            {
                balance -= amount;
                return true;
            }
            else
            {
                return false;
            }
        }

        /*
         * This funciton check the account pin against the argument passed to it
         *
         * returns:
         * true if they match
         * false if they do not
         */
        public Boolean checkPin(int pinEntered)
        {
            if (pinEntered == pin)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public int getAccountNum()
        {
            return accountNum;
        }

    }
    /* 
     *      This is out main ATM class that preforms the actions outlined in the assigment hand out
     *      
     *      the constutor contains the main funcitonality.
     */
    class ATM
    {
        //local referance to the array of accounts
        private Account[] ac;

        //this is a referance to the account that is being used
        private Account activeAccount = null;
        
        // the atm constructor takes an array of account objects as a referance
        public ATM(Account[] ac)
        {
            this.ac = ac;
            Console.WriteLine("hello from ATM");

            // an infanite loop to keep the flow of controll going on and on
            while (true)
            {

                //ask for account number and store result in acctiveAccount (null if no match found)
                activeAccount = this.findAccount();

                if (activeAccount != null)
                {
                    //if the account is found check the pin 
                    if (activeAccount.checkPin(this.promptForPin()))
                    {
                        //if the pin is a match give the options to do stuff to the account (take money out, view balance, exit)
                        dispOptions();
                    }
                }
                else
                {   //if the account number entered is not found let the user know!
                    Console.WriteLine("no matching account found.");
                }

                //wipes all text from the console
                Console.Clear();
            }


        }

        /*
         *    this method promts for the input of an account number
         *    the string input is then converted to an int
         *    a for loop is used to check the enterd account number
         *    against those held in the account array
         *    if a match is found a referance to the match is returned
         *    if the for loop completest with no match we return null
         * 
         */
        private Account findAccount()
        {
            Console.WriteLine("enter your account number..");
            
            int input = Convert.ToInt32(Console.ReadLine());

            for (int i = 0; i < this.ac.Length; i++)
            {
                if (ac[i].getAccountNum() == input)
                {
                    return ac[i];
                }
            }

            return null;
        }
        /*
         * 
         *  this jsut promt the use to enter a pin number
         *  
         * returns the string entered converted to an int
         * 
         */
        private int promptForPin()
        {
            Console.WriteLine("enter pin:");
            String str = Console.ReadLine();
            int pinNumEntered = Convert.ToInt32(str);
            return pinNumEntered;
        }

        /*
         * 
         *  give the use the options to do with the accoutn
         *  
         *  promt for input
         *  and defer to appropriate method based on input
         *  
         */
        private void dispOptions()
        {
            Console.WriteLine("1> take out cash");
            Console.WriteLine("2> balance");
            Console.WriteLine("3> exit");

            int input = Convert.ToInt32(Console.ReadLine());

            if (input == 1)
            {
                dispWithdraw();
            }
            else if (input == 2)
            {
                dispBalance();
            }
            else if (input == 3)
            {
                
             
            }
            else
            {

            }

        }

        /*
         * 
         * offer withdrawable amounts
         * 
         * based on input attempt to withraw the corosponding amount of money
         * 
         */
        private void dispWithdraw()
        {
            Console.WriteLine("1> 10");
            Console.WriteLine("2> 50");
            Console.WriteLine("3> 500");

            int input = Convert.ToInt32(Console.ReadLine());

            if (input > 0 && input < 4)
            {

                //opiton one is entered by the user
                if (input == 1)
                {

                    //attempt to decrement account by 10 punds
                    if (activeAccount.decrementBalance(10))
                    {   
                        //if this is possible display new balance and await key press
                        Console.WriteLine("new balance " + activeAccount.getBalance());
                        Console.WriteLine(" (prese enter to continue)");
                        Console.ReadLine();
                    }else{
                        //if this is not possible inform user and await key press
                         Console.WriteLine("insufficent funds");
                         Console.WriteLine(" (prese enter to continue)");
                         Console.ReadLine();
                    }
                }
                else if (input == 2)
                {
                    if (activeAccount.decrementBalance(50))
                    {
                        Console.WriteLine("new balance " + activeAccount.getBalance());
                        Console.WriteLine(" (prese enter to continue)");
                        Console.ReadLine();
                    }
                    else
                    {
                        Console.WriteLine("insufficent funds");
                        Console.WriteLine(" (prese enter to continue)");
                        Console.ReadLine();
                    }
                }
                else if (input == 3)
                {
                    if (activeAccount.decrementBalance(500))
                    {
                        Console.WriteLine("new balance " + activeAccount.getBalance());
                        Console.WriteLine(" (prese enter to continue)");
                        Console.ReadLine();
                    }
                    else
                    {
                        Console.WriteLine("insufficent funds");
                        Console.WriteLine(" (prese enter to continue)");
                        Console.ReadLine();
                    }
                }
            }
        }
        /*
         *  display balance of activeAccount and await keypress
         *  
         */
        private void dispBalance(){
            if (this.activeAccount != null)
            {
                Console.WriteLine(" your current balance is : "+activeAccount.getBalance());
                Console.WriteLine(" (prese enter to continue)");
                Console.ReadLine();
            }
        }
        
    }
}

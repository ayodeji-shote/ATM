using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ATM
{
    public partial class Form1 : Form
    {
        private Account[] ac = new Account[3];
        private ATM1 atm;
        private DataGridView accountsView;

        public Form1()
        {
            ac[0] = new Account(300, 1111, 111111);
            ac[1] = new Account(750, 2222, 222222);
            ac[2] = new Account(3000, 3333, 333333);

            //atm = new ATM1(ac);
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
            mylab.Location = new Point(this.Width / 5, 90);
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
        private DataGridView createAccountsView(int width)
        {

            /** 
             private int balance;
            private int pin;
            private int accountNum;
            public Semaphore semaphore;
            private int atmCount;
             */
            DataGridView accountsDataGridView = new DataGridView();
            accountsDataGridView.Size = new Size(width, 90);
            accountsDataGridView.ColumnCount = 3;
            accountsDataGridView.Columns[0].Name = "Account Number";
            accountsDataGridView.Columns[1].Name = "Balance";
            accountsDataGridView.Columns[2].Name = "Atm Count";
            accountsDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            accountsDataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            accountsDataGridView.MultiSelect = false;
            accountsDataGridView.AllowUserToResizeColumns = false;
            accountsDataGridView.AllowUserToAddRows = false;
            accountsDataGridView.AllowUserToDeleteRows = false;
            accountsDataGridView.AllowUserToResizeRows = false;
            accountsDataGridView.AllowDrop = false;
            accountsDataGridView.ReadOnly = true;
            accountsDataGridView.RowHeadersVisible = false;
            for (int x = 0; x < accountsDataGridView.ColumnCount; x++)
            {
                DataGridViewColumn curCol = accountsDataGridView.Columns[x];
                curCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                // curCol.Width = width / 3;
            }
            for (int x = 0; x < ac.Length; x++)
            {
                string[] curRow = new string[4];
                curRow[0] = Convert.ToString(ac[x].getAccountNum());
                curRow[1] = Convert.ToString(ac[x].getBalance());
                curRow[2] = Convert.ToString(ac[x].getAtmCount());
                accountsDataGridView.Rows.Add(curRow);
            }
            return accountsDataGridView;
        }

        public void MultATM(Form F)
        {
            accountsView = createAccountsView(F.Width);
            F.Controls.Add(accountsView);
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
                    btn[x, y].SetBounds((x * 50) + 170, (y * 40) + 200, 40, 40); // Set size & position
                    F.Controls.Add(btn[x, y]);
                    int k = int.Parse(btn[x, y].Text);
                    btn[x, y].Click += (sender, EventArgs) => { btn_Click(sender, EventArgs, F, k, getAccount()); };
                }
            }
        }
        private int getAccount() // TODO: change this 
        {
            string accNumberAsString = Convert.ToString(accountsView.SelectedRows[0].Cells[0].Value);
            int accNumber = Convert.ToInt32(accNumberAsString);
            for (int x = 0; x < ac.Length; x++)
            {
                if (ac[x].getAccountNum() == accNumber)
                {
                    return x;
                }
            }
            return -1;
        }
        private void btn_Click(object sender, EventArgs e, Form F, int p, int account)
        {
            ATMForm[] bs = new ATMForm[p];

            //Form[] bs = new Form[p];
            for (int y = 0; y < p; y++)
            {
                //break;
                new Thread(new ThreadStart(delegate
                {
                    Application.Run(new ATMForm(accountCheck));
                })).Start();
                //bs[y] = new ATMForm();
                //bs[y].FormClosed += (sender2, EventArgs) => { atmClose(sender2, EventArgs, account); Debug.Print("test"); };

            }
        }
        public bool accountCheck(string[] accNumAndPin)
        {
            for (int x = 0; x < ac.Length; x++)
            {
                if (ac[x].getAccountNum() == Convert.ToInt32(accNumAndPin[0]))
                {
                    if (ac[x].checkPin(Convert.ToInt32(accNumAndPin[1])))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public void atmClose(object sender2, EventArgs e, int account)
        {
            ac[account].decrementAtmCount();
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
            pl.BackColor = Color.White;
            return pl;
        }


    }
    /*
    *   This is the root of program and the entry point
    * 
    *   Class programm contains an array of account objects and a singel ATM object  
    * 
    */
    public partial class ATMForm : Form
    {
        private Panel screenBack;
        private Label accountNumberTextBox;
        private Label pinNumberTextBox;
        private Button[] controlButtons;
        private bool loggedIn = false;
        private Label infoMessageLabel;
        private Label loginScreenLabel;
        private Button[,] numberButtons;
        private Func<string[], bool> accountCheck;
        private Button[,] buttonsSide;
        public ATMForm(Func<string[], bool> accCheck)
        {
            this.accountCheck = accCheck;

            Formset(this);
            numberButtons = new Button[3, 3];
            int st = 1;
            for (int n = 0; n < 3; n++) // Loop for each button
            {
                for (int x = 0; x < 3; x++) // Loop for y
                {
                    numberButtons[x, n] = new Button(); // Create button
                    numberButtons[x, n].Text = st++.ToString();
                    numberButtons[x, n].SetBounds((x * 60) + 110, (n * 50) + 210, 50, 50); // Set size & position
                    int cur = st;
                    numberButtons[x, n].Click += (sender, EventArgs) => { numberButtonHandler(sender, EventArgs, cur - 1); };
                    this.Controls.Add(numberButtons[x, n]);
                    int k = int.Parse(numberButtons[x, n].Text);
                }
            }
            ATMMaker();
            Optionmaker();
            sideButtons();
            addAtmScreen();
            Label t = new Label();
            t.Text = Convert.ToString(Thread.CurrentThread.ManagedThreadId);
            t.Location = new Point(Width / 2, 0);
            Controls.Add(t); //TODO: remove
        }
        private void numberButtonHandler(object sender, EventArgs e, int num)
        {
            if (this.accountNumberTextBox.Visible == true)
            {
                if (this.accountNumberTextBox.Text.Length == 6)
                {
                    return;
                }
                this.accountNumberTextBox.Text += Convert.ToString(num);
            }
            else if (this.pinNumberTextBox.Visible == true)
            {
                if (this.pinNumberTextBox.Text.Length == 4)
                {
                    return;
                }
                this.pinNumberTextBox.Text += Convert.ToString(num);

            }
            //Debug.Print(Convert.ToString(num));

        }
        private void ATMForm_Load(object sender, EventArgs e)
        {


        }
        public Form Formset(Form pl)
        {
            pl.Width = 500;
            pl.Height = 500;
            pl.MaximumSize = this.Size;
            pl.MinimumSize = this.Size;
            pl.BackColor = Color.White;
            return pl;
        }

        public void atmClose(object sender2, EventArgs e, int account)
        {
            //ac[account].decrementAtmCount();
        }
        public void ATMMaker()
        {
            //TODO: ADD HANDLERS
            Button[] btt = new Button[3];
            btt[0] = new Button();
            btt[1] = new Button();
            btt[2] = new Button();
            btt[0].Text = "00";
            btt[1].Text = "0";
            btt[2].Text = ".";
            btt[0].SetBounds(110, 360, 50, 50); // Set size & position
            btt[1].SetBounds(170, 360, 50, 50);
            btt[2].SetBounds(230, 360, 50, 50);
            Controls.Add(btt[0]);
            Controls.Add(btt[1]);
            Controls.Add(btt[2]);
            Show();
        }
        public void Optionmaker()
        {
            controlButtons = new Button[4];
            controlButtons[0] = new Button();
            controlButtons[1] = new Button();
            controlButtons[2] = new Button();
            controlButtons[3] = new Button();
            controlButtons[0].Font = new Font("Calibri", 10);
            controlButtons[1].Font = new Font("Calibri", 10);
            controlButtons[2].Font = new Font("Calibri", 10);
            controlButtons[0].Text = "Cancel";
            controlButtons[1].Text = "Clear";
            controlButtons[2].Text = "Enter";
            controlButtons[0].BackColor = Color.Red;
            controlButtons[0].FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            controlButtons[0].FlatAppearance.BorderSize = 0;

            controlButtons[1].BackColor = Color.Yellow;
            controlButtons[1].FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            controlButtons[1].FlatAppearance.BorderSize = 0;

            controlButtons[2].BackColor = Color.Green;
            controlButtons[2].FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            controlButtons[2].FlatAppearance.BorderSize = 0;

            controlButtons[0].SetBounds(300, 213, 70, 40); // Set size & position
            controlButtons[1].SetBounds(300, 262, 70, 40);
            controlButtons[2].SetBounds(300, 314, 70, 40);
            controlButtons[3].SetBounds(300, 366, 70, 40);

            Controls.Add(controlButtons[0]);
            Controls.Add(controlButtons[1]);
            Controls.Add(controlButtons[2]);
            Controls.Add(controlButtons[3]);



            // enter 
            controlButtons[2].Click += (sender, EventArgs) => { enterButtonHandler(sender, EventArgs); };
            Show();

        }
        private void enterButtonHandler(object sender, EventArgs e)
        {
            if (this.loggedIn == false) // we are in login 
            {
                if (this.pinNumberTextBox.Visible == false)
                {
                    //we are dealing with an account number input
                    if (this.accountNumberTextBox.Text.Length < 6)
                    {
                        this.infoMessageLabel.Text = "YOUR ACCOUNT NUMBER MUST BE AT LEAST 6 DIGITS";
                        if (this.infoMessageLabel.Visible == false)
                        {
                            this.infoMessageLabel.Show();

                        }
                        //TODO: hide after certain time ?

                    }
                    else
                    {
                        this.accountNumberTextBox.Hide();
                        this.pinNumberTextBox.Show();
                        this.loginScreenLabel.Text = "Enter Pin Number";
                    }

                }
                else
                {
                    string[] accAndPin = new string[2];
                    accAndPin[0] = this.accountNumberTextBox.Text;
                    accAndPin[1] = this.pinNumberTextBox.Text;
                    if (this.accountCheck(accAndPin))
                    {
                        loggedIn = true;
                        Debug.Print("VALID ACCOUNT");
                        baseMenuScreen();
                    }
                    else
                    {
                        //TODO: INVALID ACCOUNT AND PIN COMBINATION STUFF HERE
                        loginAtmScreen();
                    }
                }
            }
            else
            {

            }
        }
        public void addAtmScreen()
        {
            screenBack = new Panel();
            screenBack.BackColor = Color.LightGray;
            screenBack.SetBounds(110, 20, 260, 180);
            Controls.Add(screenBack);
            loginAtmScreen();

        }
        private void baseMenuScreen()
        {
            screenBack.Controls.Clear();
        }
        private void loginAtmScreen()
        {


            screenBack.Controls.Clear();
            infoMessageLabel = new Label();
            infoMessageLabel.Width = screenBack.Width;
            infoMessageLabel.BorderStyle = BorderStyle.None;
            infoMessageLabel.ForeColor = Color.Red;
            infoMessageLabel.TextAlign = ContentAlignment.MiddleCenter;
            infoMessageLabel.AutoSize = false;
            infoMessageLabel.Hide();
            screenBack.Controls.Add(infoMessageLabel);
            pinNumberTextBox = new Label();
            accountNumberTextBox = new Label();

            screenBack.Controls.Add(accountNumberTextBox);
            accountNumberTextBox.Width = screenBack.Width / 2;
            accountNumberTextBox.Location = new Point((screenBack.Width / 2) - accountNumberTextBox.Width / 2, screenBack.Height / 2);
            accountNumberTextBox.TextAlign = ContentAlignment.MiddleCenter;
            screenBack.Controls.Add(pinNumberTextBox);
            pinNumberTextBox.Hide();
            loginScreenLabel = new Label();
            loginScreenLabel.Location = new Point(accountNumberTextBox.Location.X, accountNumberTextBox.Location.Y - accountNumberTextBox.Height);
            loginScreenLabel.Text = "Enter Account Number";
            loginScreenLabel.TextAlign = ContentAlignment.MiddleCenter;
            loginScreenLabel.Width = accountNumberTextBox.Width;
            screenBack.Controls.Add(loginScreenLabel);
            pinNumberTextBox.Width = screenBack.Width / 2;
            pinNumberTextBox.Location = new Point((screenBack.Width / 2) - pinNumberTextBox.Width / 2, screenBack.Height / 2);
            pinNumberTextBox.TextAlign = ContentAlignment.MiddleCenter;


        }
        public void sideButtons()
        {
            buttonsSide = new Button[2, 3];
            for (int n = 0; n < 3; n++) // Loop for each button
            {
                for (int x = 0; x < 2; x++) // Loop for y
                {
                    buttonsSide[x, n] = new Button(); // Create button
                    buttonsSide[x, n].SetBounds((x * 310) + 60, (n * 65) + 20, 50, 50); // Set size & position
                    buttonsSide[x, n].Text = Convert.ToString(x) + " " + Convert.ToString(n); //TODO : REMOVE
                    Controls.Add(buttonsSide[x, n]);
                }
            }


        }
        private void sideButtonsHandler(object sender, EventArgs e, int one, int two)
        {
            if (one == 0) // left side
            {
                switch (two)
                {
                    case 0: // top left
                        break;
                    case 1: // middle left
                        break;
                    case 2: // bottom left
                        break;
                }
            }
            else // right side
            {
                switch (two)
                {
                    case 0: // top right
                        break;
                    case 1: // middle right
                        break;
                    case 2: // bottom right
                        break;
                }

            }
        }
    }

    class Program1
    {
        private Account[] ac = new Account[3];
        private ATM1 atm;

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

            atm = new ATM1(ac);

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
        private int atmCount;
        // a constructor that takes initial values for each of the attributes (balance, pin, accountNumber)
        public Account(int balance, int pin, int accountNum)
        {
            this.balance = balance;
            this.pin = pin;
            this.accountNum = accountNum;
            this.atmCount = 0;
        }
        public int getAtmCount()
        {
            return atmCount;
        }
        public int incrementAtmCount()
        {
            atmCount++;
            return atmCount;
        }
        public void setAtmCount(int input)
        {
            atmCount = input;
        }
        public void decrementAtmCount()
        {
            atmCount--; // TODO: checking ?
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
    class ATM1
    {
        //local referance to the array of accounts
        private Account[] ac;

        //this is a referance to the account that is being used
        private Account activeAccount = null;

        // the atm constructor takes an array of account objects as a referance
        public ATM1(Account[] ac)
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
                    }
                    else
                    {
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
        private void dispBalance()
        {
            if (this.activeAccount != null)
            {
                Console.WriteLine(" your current balance is : " + activeAccount.getBalance());
                Console.WriteLine(" (prese enter to continue)");
                Console.ReadLine();
            }
        }

    }
}

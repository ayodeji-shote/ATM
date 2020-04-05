using System;
using System.Collections;
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

    public partial class BaseForm : Form
    {
        //Store our accounts
        private Account[] ac = new Account[3];
        //send a status message to the status grid on the menu form
        public void sendStatusMessage(string message)
        {
            menuForm.sendStatusMessage(message);
        }
        // store the menu form
        private MenuForm menuForm;
        public BaseForm()
        {
            // create our accounts
            ac[0] = new Account(300, 1111, 111111);
            ac[1] = new Account(750, 2222, 222222);
            ac[2] = new Account(3000, 3333, 333333);
            InitializeComponent();
            startingForm();
            Program.setIcon(this);
        }


        // setup the starting form
        public void startingForm()
        {
            Program.formset(this);
            this.Text = "The Bank to end all Banks yo";
            Label mylab = new Label();
            mylab.Text = "ATM simulator";
            mylab.Font = new Font("Calibri", 25);
            mylab.ForeColor = Color.Black;
            //mylab.AutoSize = true;
            mylab.Height = 35;
            mylab.Width = this.Width;
            mylab.TextAlign = ContentAlignment.MiddleCenter;
            mylab.Location = new Point(0, 90);
            this.Controls.Add(mylab);
            Button MyB = new Button();
            Button MyB2 = new Button();
            MyB.Text = " Data Race ";
            MyB.AutoSize = true;
            MyB.BackColor = Color.LightBlue;
            MyB.Padding = new Padding(6);
            MyB.Font = new Font("Calibri", 18);
            int mybwidth = MyB.Width;
            MyB.Location = new Point(225, 200);
            this.Controls.Add(MyB);
            MyB2.Text = " Data Race Fixed ";
            MyB2.AutoSize = true;
            int myb2width = MyB2.Width;
            MyB2.BackColor = Color.LightBlue;
            MyB2.Padding = new Padding(6);
            MyB2.Font = new Font("Calibri", 18);
            MyB2.Location = new Point(200, 280);
            Debug.Print(MyB2.Width.ToString());
            this.Controls.Add(MyB2);
            MyB.Click += (sender, EventArgs) => { MyB_Click(sender, EventArgs); }; // call base program with data race
            MyB2.Click += (sender, EventArgs) => { MyB2_Click(sender, EventArgs); }; // call changed program without data race

        }
        // base program
        private void MyB_Click(object sender, EventArgs e)
        {
            //reset our accounts
            ac[0] = new Account(300, 1111, 111111);
            ac[1] = new Account(750, 2222, 222222);
            ac[2] = new Account(3000, 3333, 333333);
            // open a menuform with data race
            menuForm = new MenuForm(ac, accountCheck, getAccount, false);
            this.Hide();
        }
        private void MyB2_Click(object sender, EventArgs e)
        {
            //reset our accounts
            ac[0] = new Account(300, 1111, 111111);
            ac[1] = new Account(750, 2222, 222222);
            ac[2] = new Account(3000, 3333, 333333);
            // open a menuform witout data race
            menuForm = new MenuForm(ac, accountCheck, getAccount, true);
            this.Hide();
        }
        // get account
        public Account getAccount(int accountNumber)
        {
            for (int x = 0; x < ac.Length; x++)
            {
                if (ac[x].getAccountNum() == accountNumber)
                {
                    return ac[x];
                }
            }
            return null;
        }
        //check account login details
        public int accountCheck(string[] accNumAndPin)
        {
            for (int x = 0; x < ac.Length; x++)
            {
                if (ac[x].getAccountNum() == Convert.ToInt32(accNumAndPin[0]))
                {
                    // if wrong info supplied too many times
                    if (ac[x].getErrorCount() >= 3)
                    {
                        return -1;
                    }
                    //pin is ok 
                    if (ac[x].checkPin(Convert.ToInt32(accNumAndPin[1])))
                    {

                        return 1;
                    }
                    // pin not ok
                    return 0;

                }
            }
            return 0;
        }

    }
    //Menu Form class
    public partial class MenuForm : Form
    {
        private delegate void SafeCallDelegate(string text); // delegate for updating status grid 
        private DataGridView accountsView; // acounts grid 
        private Account[] ac; // store our accounts
        Func<string[], int> accountCheck; // define function to be passed down to check account
        Func<int, Account> getAccount; // define function to be passed down to get account
        private bool tlock; // are we thread locking ?
        private System.Windows.Forms.Timer updateTimer = new System.Windows.Forms.Timer(); // update ui timer
        private DataGridView statusGrid; // status grid
        private ArrayList atms = new ArrayList(); // hold atmform threads
        private Label atmCountLabel; // label to hold atm count
        public void sendStatusMessage(string message)
        {

            // TODO: add status here

            if (this.statusGrid.InvokeRequired) // if accessed from a thread that requires delegation
            {
                var d = new SafeCallDelegate(sendStatusMessage); // create a delegate
                this.Invoke(d, new object[] { message }); // invoke the delegate 
            }
            else
            {
                this.statusGrid.Rows.Add(message.Split('*')); // we got delegated or called on the correct thread 
            }

        }
        public MenuForm(Account[] accs, Func<string[], int> accCheck, Func<int, Account> getAcc, bool threadLock)
        {


            this.tlock = threadLock; // are we thread locking ?
            this.Text = this.tlock == true ? "Main Bank Computer | Thread Lock Enabled " : "Main Bank Computer | Thread Lock Disabled"; // name based on thread locking status
            this.FormClosed += (sender, EventArgs) => { Program.showMainForm(); }; // add close event
            this.accountCheck = accCheck; // add account check function
            this.getAccount = getAcc; // add get account function
            ac = accs; // pass in accounts
            Program.formset(this); // set size etc
            this.Show(); // show this form
            //TODO: add button to unlock account
            Button unblockAcccountButton = new Button();
            unblockAcccountButton.Text = "Click to unblock selected account";
            unblockAcccountButton.Width = this.Width;
            unblockAcccountButton.Location = new Point(0, 90);
            unblockAcccountButton.Click += (s, n) => { unblockAccount(); }; // BUTTON TO UNBLOCK ACCOUNTS
            this.Controls.Add(unblockAcccountButton);
            accountsView = createAccountsView(this.Width); // add accounts grid
            Controls.Add(accountsView);
            createStatusGrid(); // add status grid

            Label mylab = new Label();
            mylab.Text = " How many ATM's do you want to simulate ";
            mylab.Width = this.Width;
            mylab.Height = 30;
            mylab.TextAlign = ContentAlignment.TopCenter;
            mylab.Font = new Font("Calibri", 20);
            mylab.ForeColor = Color.Black;
            mylab.Location = new Point(0, 120);
            Controls.Add(mylab);
            atmCountLabel = new Label();
            atmCountLabel.Text = "Atm Count : 0";
            atmCountLabel.Width = this.Width;
            atmCountLabel.Font = new Font("Calibri", 10);
            atmCountLabel.ForeColor = Color.Black;
            atmCountLabel.Location = new Point(0, mylab.Location.Y + mylab.Height);
            atmCountLabel.TextAlign = ContentAlignment.MiddleCenter;
            Controls.Add(atmCountLabel);
            Panel buttonPanel = new Panel();
            buttonPanel.BackColor = Color.Transparent;
            buttonPanel.Location = new Point(210, 180);
            buttonPanel.Height = 150;
            Button[,] btn = new Button[3, 3];
            int st = 1;
            for (int y = 0; y < 3; y++) // Loop for each button
            {
                for (int x = 0; x < 3; x++) // Loop for y
                {
                    btn[x, y] = new Button(); // Create button
                    btn[x, y].Text = st++.ToString();
                    btn[x, y].SetBounds((x * 60), (y * 50), 50, 50); // Set size & position
                    buttonPanel.Controls.Add(btn[x, y]);

                    int k = int.Parse(btn[x, y].Text);
                    btn[x, y].BackgroundImage = Image.FromFile("IMAGES/Button " + Convert.ToString(k) + ".png"); // image based on number
                    btn[x, y].Text = "";
                    btn[x, y].Click += (sender, EventArgs) => { btn_Click(sender, EventArgs, this, k); }; // add button event based on number
                }
            }
            this.Controls.Add(buttonPanel); // add button panel

            updateTimer.Interval = 1000; // 1 second
            updateTimer.Tick += (s, n) =>
            {
                updateTable(); // update our account table
                atmCountLabel.Text = "Atm Count : " + Convert.ToString(atms.Count); // update atm count
            };
            updateTimer.Start();
            this.FormClosed += (sender, EventArgs) => { this.updateTimer.Stop(); this.removeAllThreads(); }; // when form closed remove all threads and stop update timer
        }
        private void unblockAccount()
        {
            if (this.accountsView.SelectedRows.Count == 0) // to be safe
            {
                return;
            }

            Account acc = getAccount(Convert.ToInt32(this.accountsView.SelectedRows[0].Cells[0].Value)); // get account based on account number from accounts grid
            if (acc.getErrorCount() >= 3) // if blocked
            {
                acc.setErrorCount(0); // unblock
            }


        }
        private void updateTable()
        {
            for (int x = 0; x < accountsView.RowCount; x++) // for each row
            {
                // set all the values
                accountsView.Rows[x].Cells[0].Value = Convert.ToString(ac[x].getAccountNum());
                accountsView.Rows[x].Cells[1].Value = Convert.ToString(ac[x].getBalance());
                accountsView.Rows[x].Cells[2].Value = Convert.ToString(ac[x].getAtmCount());
                accountsView.Rows[x].Cells[3].Value = Convert.ToString(ac[x].getErrorCount());
                // colour based on error count
                if (ac[x].getErrorCount() >= 3)
                {
                    accountsView.Rows[x].Cells[3].Style.BackColor = Color.Red;
                }
                else if (ac[x].getErrorCount() >= 2)
                {
                    accountsView.Rows[x].Cells[3].Style.BackColor = Color.Orange;
                }
                else if (ac[x].getErrorCount() >= 0)
                {
                    accountsView.Rows[x].Cells[3].Style.BackColor = Color.Green;
                }
            }
        }
        //event for simulate atms 
        private void btn_Click(object sender, EventArgs e, Form F, int p)
        {

            for (int y = 0; y < p; y++) // for number of atms 
            {
                Thread thread;
                thread = new Thread(() => { Application.Run(new ATMForm(accountCheck, getAccount, removeThread, this.tlock)); }); // create new thread with atmform inside and thread locking status
                thread.Start(); // start the thread 
                atms.Add(thread); // add the thread to the arraylist
            }

        }
        // remove thread from arraylist
        private bool removeThread(int id)
        {
            ArrayList temp = new ArrayList(); // create new arraylist
            for (int x = 0; x < atms.Count; x++)
            {
                Thread cur = (Thread)atms[x]; // get this thread
                if (cur.ManagedThreadId != id) // if it is not the thread we are removing
                {
                    temp.Add(cur); // add to it 
                    //return true;
                }

            }
            atms = temp; // replace arraylist with all but removed thread
            return true;
        }
        private void removeAllThreads()
        {
            for (int x = 0; x < atms.Count; x++) // for each thread
            {
                Thread cur = (Thread)atms[x]; // get our thread
                cur.Abort(); // abort the thread 
            }
        }
        private void createStatusGrid()
        {
            // time
            // account number
            // thread no
            // type
            // value 

            this.statusGrid = new DataGridView();
            statusGrid.Size = new Size(this.Width, 140);
            statusGrid.Location = new Point(0, this.Height - 200);
            // set columns
            statusGrid.ColumnCount = 5;
            statusGrid.Columns[0].Name = "Time";
            statusGrid.Columns[1].Name = "Acc #";
            statusGrid.Columns[2].Name = "Atm #";
            statusGrid.Columns[3].Name = "Type";
            statusGrid.Columns[4].Name = "Value";
            statusGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            statusGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            statusGrid.MultiSelect = false;
            statusGrid.AllowUserToResizeColumns = false;
            statusGrid.AllowUserToAddRows = false;
            statusGrid.AllowUserToDeleteRows = false;
            statusGrid.AllowUserToResizeRows = false;
            statusGrid.AllowDrop = false;
            statusGrid.ReadOnly = true;
            statusGrid.RowHeadersVisible = false;
            // set fill for each column
            for (int x = 0; x < statusGrid.ColumnCount; x++)
            {
                DataGridViewColumn curCol = statusGrid.Columns[x];
                curCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            this.Controls.Add(statusGrid);

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
            // set columns
            accountsDataGridView.ColumnCount = 4;
            accountsDataGridView.Columns[0].Name = "Account Number";
            accountsDataGridView.Columns[1].Name = "Balance";
            accountsDataGridView.Columns[2].Name = "Atm Count";
            accountsDataGridView.Columns[3].Name = "Errors";
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
            //set fill for each column
            for (int x = 0; x < accountsDataGridView.ColumnCount; x++)
            {
                DataGridViewColumn curCol = accountsDataGridView.Columns[x];
                curCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            for (int x = 0; x < ac.Length; x++)
            {
                string[] curRow = new string[4];
                curRow[0] = Convert.ToString(ac[x].getAccountNum());
                curRow[1] = Convert.ToString(ac[x].getBalance());
                curRow[2] = Convert.ToString(ac[x].getAtmCount());
                curRow[3] = Convert.ToString(ac[x].getErrorCount());
                accountsDataGridView.Rows.Add(curRow);
            }
            return accountsDataGridView;
        }
    }
    public partial class ATMForm : Form
    {
        private Panel screenBack; // our screen panel
        private Label accountNumberTextBox; // account number 
        private Label pinNumberTextBox; // pin number
        private string pinNumberAsString = ""; // hold pin number as screen
        private Button[] controlButtons; // control buttons
        private bool loggedIn = false; // are we logged in
        private Label infoMessageLabel; // info message label for warmings
        private Label loginScreenLabel; // label for login scren
        private Button[,] numberButtons; // 2d array of number buttons
        private Func<string[], int> accountCheck; // account check function 
        private Func<int, Account> getAccount; // get account function
        private Button[,] buttonsSide; // 2d array of side buttons
        private Account account; // hold the account for this atm
        private bool tlock; // are we thread locking ?

        private bool mainScreen = false; // are we on the main screen ? 
        private bool withdrawing = false; // are we withdrawing ?
        private bool returningCard = false; // are we returning the card ?
        private bool showingBalance = false; // are we showing balance ?
        private bool lockControls = false; // are the controls locked ?
        private bool showingWithdraw = false; // are we showing the withdraw to the user ?
        private int curThread = -1; // store the thread id 
        private string[] images; // images for this bank

        private PictureBox cardOut; // picture for card out
        private PictureBox cardIn; // picture for card in
        private PictureBox cashIn; // picture for cash in
        private PictureBox cashOut; // picture for cash out
        public ATMForm(Func<string[], int> accCheck, Func<int, Account> getAcc, Func<int, bool> removeThread, bool threadLock)
        {
            this.images = Program.getImages(); // get a random image set for atm
            this.tlock = threadLock; // are we thread locking ? 
            this.accountCheck = accCheck; // passed down fucntion
            this.getAccount = getAcc; // passed down function

            Program.formset(this); // set form size etc 
            numberButtons = new Button[3, 3]; // define the number buttons 
            int st = 1;
            for (int n = 0; n < 3; n++) // Loop for each button
            {
                for (int x = 0; x < 3; x++) // Loop for y
                {
                    numberButtons[x, n] = new Button(); // Create button
                    st++;
                    numberButtons[x, n].SetBounds((x * 60) + 110 + x, (n * 50) + 210 + n, 50, 50); // Set size & position
                    int cur = st;
                    numberButtons[x, n].Click += (sender, EventArgs) => { numberButtonHandler(sender, EventArgs, cur - 1); }; // add the handler with number
                    numberButtons[x, n].BackColor = Color.Transparent;
                    numberButtons[x, n].FlatStyle = FlatStyle.Standard;
                    numberButtons[x, n].FlatAppearance.BorderSize = 0;
                    this.Controls.Add(numberButtons[x, n]);
                    numberButtons[x, n].BackgroundImage = Image.FromFile("IMAGES/Button " + Convert.ToString(cur - 1) + ".png"); // add image based on number
                }
            }
            ATMMaker();
            Optionmaker();
            sideButtons();
            addAtmScreen();
            setupImages();
            this.cashOut.Show(); // default setting
            this.FormClosed += (sender, EventArgs) => { atmClose(sender, EventArgs); removeThread(curThread); }; // add close event

            reset();

            this.Text = "ATM # : " + Convert.ToString(Thread.CurrentThread.ManagedThreadId) + ((this.tlock == true) ? " | Thread Lock Enabled " : " | Thread Lock Disabled"); //set name based on thread id and thread locking status
            this.curThread = Thread.CurrentThread.ManagedThreadId;

        }
        //setup our images
        private void setupImages()
        {
            this.cardIn = new PictureBox();
            this.cardIn.Image = Image.FromFile("IMAGES/" + images[0]);
            this.cardIn.Hide();
            this.cardIn.Size = new System.Drawing.Size(160, 225);
            this.cardIn.Location = new Point(this.Width - this.cardIn.Width - 50, this.Height - this.cardIn.Height - 100);
            this.Controls.Add(cardIn);

            this.cardOut = new PictureBox();
            this.cardOut.Image = Image.FromFile("IMAGES/" + "Card_Slot.png");
            this.cardOut.Size = new System.Drawing.Size(160, 225);
            this.cardOut.Location = new Point(this.Width - this.cardIn.Width - 50, this.Height - this.cardIn.Height - 100);
            this.Controls.Add(cardOut);
            this.cardOut.Hide();

            this.cashIn = new PictureBox();
            this.cashIn.Image = Image.FromFile("IMAGES/" + "Cash.png");
            this.cashIn.SizeMode = PictureBoxSizeMode.StretchImage;
            this.cashIn.Size = new System.Drawing.Size(screenBack.Width, 108);
            this.cashIn.Location = new Point(this.screenBack.Location.X, this.Height - this.cashIn.Height - 30);
            this.Controls.Add(cashIn);
            this.cashIn.Hide();

            this.cashOut = new PictureBox();
            this.cashOut.Image = Image.FromFile("IMAGES/" + "No Cash.png");
            this.cashOut.SizeMode = PictureBoxSizeMode.StretchImage;
            this.cashOut.Size = new System.Drawing.Size(screenBack.Width, 108);
            this.cashOut.Location = new Point(this.screenBack.Location.X, this.Height - this.cashIn.Height - 30);
            this.Controls.Add(cashOut);
            this.cashOut.Hide();


        }
        private void reset()
        {
            this.screenBack.Controls.Clear();
            this.account = null; // we have no account in reset
            //reset all status
            this.withdrawing = false;
            this.returningCard = false;
            this.loggedIn = false;
            this.mainScreen = false;
            this.showingBalance = false;
            this.lockControls = false;
            this.showingWithdraw = false;
            //fade from bank image to login screen
            System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
            screenBack.BackgroundImage = Image.FromFile("IMAGES/" + images[2]);
            this.cardIn.Show();
            t.Interval = 1500; // 1.5 seconds
            t.Tick += (s, n) =>
            {

                screenBack.BackgroundImage = null;
                screenBack.BackColor = Color.FromArgb(63, 72, 204);
                this.cardIn.Hide();
                this.cardOut.Show();
                loginAtmScreen();

                t.Stop();
            };
            t.Start();


        }
        // when atm closed and logged into an account decrement that accounts atm count
        private void atmClose(object sender, EventArgs e)
        {
            if (this.account != null)
            {
                this.account.decrementAtmCount();
            }
        }
        // handle number button stuff
        private void numberButtonHandler(object sender, EventArgs e, int num)
        {
            // do nothing when returning card or locking controls
            if (returningCard || lockControls)
            {
                return;
            }
            // if account number is visible then add to it
            if (this.accountNumberTextBox.Visible == true)
            {
                if (this.accountNumberTextBox.Text.Length == 6) // no more than 6
                {
                    return;
                }
                this.accountNumberTextBox.Text += Convert.ToString(num);
            }
            else if (this.pinNumberTextBox.Visible == true) // if pin number is visible then add to it
            {
                if (this.pinNumberTextBox.Text.Length == 4) // no more than 4
                {
                    return;
                }
                this.pinNumberTextBox.Text += "*"; // add stars to hide pin
                this.pinNumberAsString += Convert.ToString(num); // store pin as string to use
            }
        }

        public void ATMMaker()
        {
            // bottom buttons 
            Button[] btt = new Button[3];
            btt[0] = new Button();
            btt[1] = new Button();
            btt[2] = new Button();
            btt[1].BackgroundImage = Image.FromFile("IMAGES/Button " + Convert.ToString(0) + ".png"); //0 BUTTON
            btt[1].Click += (sender, EventArgs) => { numberButtonHandler(sender, EventArgs, 0); };
            btt[0].BackgroundImage = Image.FromFile("IMAGES/Button Template.png");
            btt[2].BackgroundImage = Image.FromFile("IMAGES/Button Template.png");
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



            controlButtons[2].Click += (sender, EventArgs) => { enterButtonHandler(sender, EventArgs); };
            controlButtons[1].Click += (sender, EventArge) => { clearButtonHandler(sender, EventArge); };
            controlButtons[0].Click += (sender, EventArgs) => { cancelButtonHandler(sender, EventArgs); };
            Show();

        }
        private void cancelButtonHandler(object sender, EventArgs e)
        {
            // if returning card or locking controls do nothing
            if (returningCard || lockControls)
            {
                return;
            }
            // if not logged in // return the card
            if (!loggedIn)
            {
                returnScreen();

            }
        }
        private void clearButtonHandler(object sender, EventArgs e)
        {
            // if returning card or locking controls do nothing
            if (returningCard || lockControls)
            {
                return;
            }
            // if we are not logged in then clear appropriate login box
            if (this.loggedIn == false)
            {
                if (this.pinNumberTextBox.Visible == false)
                {
                    //we are dealing with an account number input
                    this.accountNumberTextBox.Text = "";
                }
                else
                {
                    this.pinNumberTextBox.Text = "";
                    this.pinNumberAsString = "";
                }
            }
        }
        private void enterButtonHandler(object sender, EventArgs e)
        {
            // if returning card or locking controls do nothing
            if (returningCard || lockControls)
            {
                return;
            }
            if (this.loggedIn == false) // we are in login 
            {
                if (this.pinNumberTextBox.Visible == false)
                {
                    //we are dealing with an account number input
                    if (this.accountNumberTextBox.Text.Length < 6) // if less than 6 warn user 
                    {
                        this.infoMessageLabel.Text = "YOUR ACCOUNT NUMBER MUST BE AT LEAST 6 \n DIGITS";
                        this.infoMessageLabel.AutoSize = true;
                        if (this.infoMessageLabel.Visible == false)
                        {
                            this.infoMessageLabel.Show();
                            System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
                            //hold warning for 1.5 seconds
                            t.Interval = 1500;
                            t.Tick += (s, n) =>
                            {
                                this.infoMessageLabel.Hide();
                                t.Stop();
                            };
                            t.Start();

                        }

                    }
                    else // we have got a 'valid' account number
                    {
                        this.accountNumberTextBox.Hide();
                        this.pinNumberTextBox.Show();
                        this.loginScreenLabel.Text = "Enter Pin Number";
                    }

                }
                else // we now have a pin and account number // lets check if they are ok
                {
                    string[] accAndPin = new string[2];

                    // get strings to send to account check
                    accAndPin[0] = this.accountNumberTextBox.Text;
                    accAndPin[1] = this.pinNumberAsString;
                    int check = this.accountCheck(accAndPin);
                    // we have a valid account
                    if (check == 1)
                    {
                        loggedIn = true;
                        this.account = getAccount(Convert.ToInt32(accAndPin[0]));
                        this.account.incrementAtmCount();
                        baseMenuScreen();

                    }
                    else if (check == 0) // invalid pin / account combo 
                    {
                        Account account = getAccount(Convert.ToInt32(accAndPin[0]));
                        if (account != null) // if there is an account with this account number
                        {
                            account.incrementErrorCount(); // increment that accounts error count
                        }
                        this.infoMessageLabel.Text = "INVALID PIN OR ACCOUNT NUMBER \n COMBINATION"; // warming to user
                        this.infoMessageLabel.Height = screenBack.Height / 5;
                        this.infoMessageLabel.Width = screenBack.Width;
                        this.infoMessageLabel.TextAlign = ContentAlignment.MiddleCenter;
                        this.infoMessageLabel.Location = new Point(0, screenBack.Height / 2);
                        if (this.infoMessageLabel.Visible == false)
                        {
                            this.pinNumberTextBox.Hide();
                            this.loginScreenLabel.Hide();
                            this.infoMessageLabel.Show();
                            System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
                            // display error for 1.5 seconds
                            t.Interval = 1500;
                            t.Tick += (s, n) =>
                            {
                                this.infoMessageLabel.Hide();
                                loginAtmScreen();
                                t.Stop();
                            };
                            t.Start();

                        }

                    }
                    else if (check == -1) // acount is locked out
                    {
                        Account account = getAccount(Convert.ToInt32(accAndPin[0]));
                        if (account != null)
                        {
                            account.incrementErrorCount();
                        }
                        this.infoMessageLabel.Text = "YOU HAVE BEEN LOCKED OUT OF YOUR ACCOUNT PLEASE CONTACT YOUR BANK TO UNBLOCK"; // display lock out 
                        this.infoMessageLabel.Height = screenBack.Height / 3;
                        this.infoMessageLabel.Width = screenBack.Width;
                        this.infoMessageLabel.TextAlign = ContentAlignment.MiddleCenter;
                        this.infoMessageLabel.Location = new Point(0, screenBack.Height / 2);
                        if (this.infoMessageLabel.Visible == false)
                        {
                            this.pinNumberTextBox.Hide();
                            this.loginScreenLabel.Hide();
                            this.infoMessageLabel.Show();
                            System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
                            // display error for 1.5 seconds 
                            t.Interval = 1500;
                            t.Tick += (s, n) =>
                            {
                                this.infoMessageLabel.Hide();
                                loginAtmScreen();
                                t.Stop();
                            };
                            t.Start();

                        }

                    }
                }
            }
            else
            {

            }
        }
        // add our atm screen
        public void addAtmScreen()
        {
            screenBack = new Panel();
            screenBack.BackColor = Color.FromArgb(63, 72, 204);
            screenBack.SetBounds(110, 20, 260, 180);
            Controls.Add(screenBack);
            loginAtmScreen();
        }
        // menu for account logged in 
        private void baseMenuScreen()
        {
            mainScreen = true;
            //TODO: add number controls for each // TODO: add formatting
            screenBack.Controls.Clear();
            Label withdrawLabel = new Label();
            withdrawLabel.Text = "< To take money from your account ";
            withdrawLabel.AutoSize = true;
            withdrawLabel.Location = new Point(0, buttonsSide[0, 0].Location.Y);
            screenBack.Controls.Add(withdrawLabel);


            Label checkBalanceLabel = new Label();
            checkBalanceLabel.Text = "< To check your account balance ";
            checkBalanceLabel.AutoSize = true;
            checkBalanceLabel.Location = new Point(0, buttonsSide[0, 1].Location.Y);
            screenBack.Controls.Add(checkBalanceLabel);


            Label returnCardLabel = new Label();
            returnCardLabel.Text = "< To return your card";
            returnCardLabel.AutoSize = true;
            returnCardLabel.Location = new Point(0, buttonsSide[0, 2].Location.Y);
            screenBack.Controls.Add(returnCardLabel);
        }
        // withdrawing screen
        private void withdrawScreen()
        {
            screenBack.Controls.Clear();
            mainScreen = false;
            withdrawing = true;
            Label[,] withdrawLabels = new Label[2, 3];
            int i = 0;
            string[] labelText = new string[] { "< £10", "£100 >", "< £20", "£500 >", "< £40", "Return >" };
            for (int n = 0; n < 3; n++) // Loop for each button
            {
                for (int x = 0; x < 2; x++) // Loop for y
                {
                    withdrawLabels[x, n] = new Label();
                    withdrawLabels[x, n].Width = screenBack.Width / 2;
                    withdrawLabels[x, n].Text = labelText[i];
                    withdrawLabels[x, n].Location = new Point(x == 0 ? 10 : Convert.ToInt32(screenBack.Width / 2), buttonsSide[x, n].Location.Y); // location based on side buttons 
                    if (x == 1)
                    {
                        withdrawLabels[x, n].TextAlign = ContentAlignment.MiddleRight; // push right if on right
                    }
                    screenBack.Controls.Add(withdrawLabels[x, n]);
                    i++;
                }
            }


        }
        // our balance screen
        private void balanceScreen()
        {
            screenBack.Controls.Clear();
            mainScreen = false;
            showingBalance = true;
            Label accountLabel = new Label();
            accountLabel.Text = "Account : " + Convert.ToString(account.getAccountNum());
            accountLabel.TextAlign = ContentAlignment.MiddleCenter;
            accountLabel.Width = screenBack.Width;
            accountLabel.Location = new Point(0, (screenBack.Height / 2) - accountLabel.Height);
            screenBack.Controls.Add(accountLabel);
            Label balanceLabel = new Label();
            balanceLabel.Text = "Balance : £" + Convert.ToString(account.getBalance());
            balanceLabel.TextAlign = ContentAlignment.MiddleCenter;
            balanceLabel.Width = screenBack.Width;
            balanceLabel.Location = new Point(0, accountLabel.Location.Y + (accountLabel.Height));
            screenBack.Controls.Add(balanceLabel);

            Label exitLabel = new Label();
            exitLabel.Text = "Return >";
            exitLabel.TextAlign = ContentAlignment.MiddleRight;
            exitLabel.Width = screenBack.Width;
            exitLabel.Location = new Point(0, buttonsSide[1, 2].Location.Y);
            screenBack.Controls.Add(exitLabel);

        }
        // returning card screen
        private void returnScreen()
        {
            mainScreen = false;
            returningCard = true;
            loggedIn = false;
            if (this.account != null)
            {
                this.account.decrementAtmCount(); // if logged out we must decrement the atm count for that account
            }
            screenBack.Controls.Clear();
            this.infoMessageLabel.Text = "Returning Card \n Please Wait...";
            screenBack.Controls.Add(this.infoMessageLabel);
            this.infoMessageLabel.Show();
            System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
            this.infoMessageLabel.Height = screenBack.Height / 5;
            this.infoMessageLabel.Location = new Point(0, screenBack.Height / 2);
            this.cardOut.Hide();
            this.cardIn.Show();
            // display to user for 3 seconds
            t.Interval = 3000;
            t.Tick += (s, n) =>
            {
                this.cardOut.Show();
                this.cardIn.Hide();
                t.Stop();
                reset();
            };
            t.Start();
        }
        // login screen
        private void loginAtmScreen()
        {
            this.pinNumberAsString = ""; // reset pin number storage
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
                    int one = x;
                    int two = n;
                    buttonsSide[x, n].Click += (sender, EventArgs) => { sideButtonsHandler(sender, EventArgs, one, two); }; // handler based on position
                    buttonsSide[x, n].BackgroundImage = Image.FromFile("IMAGES/Button Template.png"); // button image
                    Controls.Add(buttonsSide[x, n]);
                }
            }


        }
        // we are now withdrawing 
        private void withdraw(int amnt)
        {
            withdrawing = false;
            screenBack.Controls.Clear();
            lockControls = true; // lock controls while doing stuff
            Label attempting = new Label();
            attempting.Text = "Attempting to withdraw £" + Convert.ToString(amnt);
            attempting.TextAlign = ContentAlignment.MiddleCenter;
            attempting.Width = screenBack.Width;
            attempting.Location = new Point(0, screenBack.Height / 2);
            screenBack.Controls.Add(attempting);
            System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
            t.Interval = 1; // tiny tick to allow ui to update
            t.Tick += (s, n) =>
             {

                 if (account.withdrawAmnt(amnt, this.curThread, tlock)) // we we can withdraw
                 {
                     screenBack.Controls.Clear();
                     Label success = new Label();
                     success.Text = "Successfully withdrew £" + Convert.ToString(amnt);
                     success.TextAlign = ContentAlignment.MiddleCenter;
                     success.Width = screenBack.Width;
                     success.Location = new Point(0, screenBack.Height / 5);
                     screenBack.Controls.Add(success);

                     Label newBalance = new Label();
                     newBalance.Text = "New Balance : £" + Convert.ToString(account.getBalance());
                     newBalance.TextAlign = ContentAlignment.MiddleCenter;
                     newBalance.Width = screenBack.Width;
                     newBalance.Location = new Point(0, success.Location.Y + success.Height);
                     screenBack.Controls.Add(newBalance);

                     Label exitLabel = new Label();
                     exitLabel.Text = "Return >";
                     exitLabel.TextAlign = ContentAlignment.MiddleRight;
                     exitLabel.Width = screenBack.Width;
                     exitLabel.Location = new Point(0, buttonsSide[1, 2].Location.Y);
                     screenBack.Controls.Add(exitLabel);
                     showingWithdraw = true;
                     lockControls = false;

                 }
                 else // if we cant
                 {
                     screenBack.Controls.Clear();
                     Label failure = new Label();
                     failure.Text = "Failed to withdraw £" + Convert.ToString(amnt);
                     failure.TextAlign = ContentAlignment.MiddleCenter;
                     failure.Width = screenBack.Width;
                     failure.Location = new Point(0, screenBack.Height / 5);
                     screenBack.Controls.Add(failure);

                     Label newBalance = new Label();
                     newBalance.Text = "Balance : £" + Convert.ToString(account.getBalance());
                     newBalance.TextAlign = ContentAlignment.MiddleCenter;
                     newBalance.Width = screenBack.Width;
                     newBalance.Location = new Point(0, failure.Location.Y + failure.Height);
                     screenBack.Controls.Add(newBalance);

                     Label exitLabel = new Label();
                     exitLabel.Text = "Return >";
                     exitLabel.TextAlign = ContentAlignment.MiddleRight;
                     exitLabel.Width = screenBack.Width;
                     exitLabel.Location = new Point(0, buttonsSide[1, 2].Location.Y);
                     screenBack.Controls.Add(exitLabel);
                     showingWithdraw = true;
                     lockControls = false;
                 }
                 // show the cash baby
                 this.cashOut.Hide();
                 this.cashIn.Show();
                 t.Stop();
             };
            t.Start();


        }
        private void sideButtonsHandler(object sender, EventArgs e, int one, int two)
        {
            // do nothing if returning card or controls locked
            if (returningCard || lockControls)
            {
                return;
            }
            if (one == 0) // left side
            {
                switch (two)
                {

                    case 0: // top left
                        if (mainScreen)
                        {
                            withdrawScreen();
                        }
                        else if (withdrawing)
                        {
                            withdraw(10);
                        }
                        break;
                    case 1: // middle left
                        if (mainScreen)
                        {
                            balanceScreen();
                        }
                        else if (withdrawing)
                        {
                            withdraw(20);
                        }
                        break;
                    case 2: // bottom left
                        if (mainScreen)
                        {
                            returnScreen();
                        }
                        else if (withdrawing)
                        {
                            withdraw(40);
                        }
                        break;
                }
            }
            else // right side
            {
                switch (two)
                {
                    case 0: // top right
                        if (withdrawing)
                        {
                            withdraw(100);
                        }
                        break;
                    case 1: // middle right
                        if (withdrawing)
                        {
                            withdraw(500);
                        }
                        break;
                    case 2: // bottom right
                        if (withdrawing) // return button for withdraw screen
                        {
                            withdrawing = false;
                            baseMenuScreen();
                        }
                        else if (showingBalance) // return button for balance screen
                        {
                            showingBalance = false;
                            baseMenuScreen();
                        }
                        else if (showingWithdraw) // return button for showing withdraw screen
                        {
                            showingWithdraw = false;
                            this.cashIn.Hide();
                            this.cashOut.Show();
                            baseMenuScreen();
                        }
                        break;
                }

            }
        }
    }

    public class Account
    {
        //the attributes for the account
        private int balance;
        private int pin;
        private int accountNum;
        private int atmCount;
        private int errorCount;
        object withdrawLock = new object(); // if this is static it will only allow one atm to withdraw at a time , if not it will only lock for each account // our thread lock object
        // a constructor that takes initial values for each of the attributes (balance, pin, accountNumber)
        public Account(int balance, int pin, int accountNum)
        {
            this.balance = balance;
            this.pin = pin;
            this.accountNum = accountNum;
            this.atmCount = 0;
        }
        public void incrementErrorCount()
        {
            this.errorCount++;
        }
        public int getErrorCount()
        {
            return this.errorCount;
        }
        public void setErrorCount(int err)
        {
            this.errorCount = err;
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
            atmCount--;
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
        //https://www.pluralsight.com/guides/lock-statement-access-data
        // send status message and withdraw
        public Boolean withdrawAmnt(int amnt, int threadID, bool threadLock)
        {
            //Program.sendStatusMessage(Convert.ToString(this.accountNum)+ "*" + Convert.ToString();
            Program.sendStatusMessage(DateTime.Now.ToString("HH:mm:ss") +
                "*" + Convert.ToString(this.accountNum) +
                "*" + Convert.ToString(threadID) +
                "*" + Convert.ToString("Access") +
                "*" + "£" + Convert.ToString(amnt)
                );
            if (threadLock) // if we are threadlocking
            {
                lock (withdrawLock) // lock inside here to one thread 
                {
                    Program.sendStatusMessage(DateTime.Now.ToString("HH:mm:ss") +
                    "*" + Convert.ToString(this.accountNum) +
                    "*" + Convert.ToString(threadID) +
                    "*" + Convert.ToString("Withdraw") +
                    "*" + "£" + Convert.ToString(amnt)
                    );


                    return decrementBalance(amnt);

                }
            }
            else
            {
                Program.sendStatusMessage(DateTime.Now.ToString("HH:mm:ss") +
                    "*" + Convert.ToString(this.accountNum) +
                    "*" + Convert.ToString(threadID) +
                    "*" + Convert.ToString("Withdraw") +
                    "*" + "£" + Convert.ToString(amnt)
                    );


                return decrementBalance(amnt);
            }

        }
        public Boolean decrementBalance(int amount)
        {
            if (this.balance >= amount)
            {
                Thread.Sleep(3000); // hold here to visualise data race
                balance -= amount;
                Thread.Sleep(3000); // hold here to visualise data race
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

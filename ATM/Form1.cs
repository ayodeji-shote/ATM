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
        public void sendStatusMessage(string message)
        {
            menuForm.sendStatusMessage(message);
        }
        private MenuForm menuForm;
        public Form1()
        {
            // create our accounts
            ac[0] = new Account(300, 1111, 111111);
            ac[1] = new Account(750, 2222, 222222);
            ac[2] = new Account(3000, 3333, 333333);
            InitializeComponent();
            startingForm();
            Program.setIcon(this);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Formset(this);
            Program.formset(this);
        }


        public void startingForm()
        {
            this.Text = "The Bank to end all Banks yo";
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
            MyB.Click += (sender, EventArgs) => { MyB_Click(sender, EventArgs); }; // this sets the button click to the event handler.
            MyB2.Click += (sender, EventArgs) => { MyB2_Click(sender, EventArgs); }; // this sets the button click to the event handler.

        }

        private void MyB_Click(object sender, EventArgs e)
        {
            ac[0] = new Account(300, 1111, 111111);
            ac[1] = new Account(750, 2222, 222222);
            ac[2] = new Account(3000, 3333, 333333);
            menuForm = new MenuForm(ac, accountCheck, getAccount, false);
            this.Hide();
        }
        private void MyB2_Click(object sender, EventArgs e)
        {
            ac[0] = new Account(300, 1111, 111111);
            ac[1] = new Account(750, 2222, 222222);
            ac[2] = new Account(3000, 3333, 333333);
            menuForm = new MenuForm(ac, accountCheck, getAccount, true);
            this.Hide();
        }

        public Account getAccount(int accountNumber)
        {
            int a = 0;
            for (int x = 0; x < ac.Length; x++)
            {
                if (ac[x].getAccountNum() == accountNumber)
                {
                    a = x;
                }
            }
            return ac[a];
        }
        public bool accountCheck(string[] accNumAndPin)
        {
            for (int x = 0; x < ac.Length; x++)
            {
                if (ac[x].getAccountNum() == Convert.ToInt32(accNumAndPin[0]))
                {
                    if (ac[x].checkPin(Convert.ToInt32(accNumAndPin[1])))
                    {
                        //ac[x].incrementAtmCount();
                        //this.accountsView.Invoke(new MethodInvoker(delegate
                        //{
                        //    updateAccounts();
                        //}));

                        return true;
                    }
                    return false;

                }
            }
            return false;
        }
        public void atmClose(object sender2, EventArgs e, int account)
        {
            ac[account].decrementAtmCount(); // TODO: add this to atm window close
        }
        private void MyB2_Click(object sender, EventArgs e, Form F)
        {
            Program.formset(F);
            F.Show();
            this.Hide();
        }


    }
    public partial class MenuForm : Form
    {
        private delegate void SafeCallDelegate(string text);
        private DataGridView accountsView;
        private Account[] ac;
        Func<string[], bool> accountCheck;
        Func<int, Account> getAccount;
        private bool tlock;
        private System.Windows.Forms.Timer updateTimer = new System.Windows.Forms.Timer();
        private DataGridView statusGrid;
        public void sendStatusMessage(string message)
        {

            // TODO: add status here

            if (this.statusGrid.InvokeRequired)
            {
                var d = new SafeCallDelegate(sendStatusMessage);
                this.Invoke(d, new object[] { message });
            }
            else
            {
                this.statusGrid.Rows.Add(message.Split('*'));
            }

        }
        public MenuForm(Account[] accs, Func<string[], bool> accCheck, Func<int, Account> getAcc, bool threadLock)
        {
            this.tlock = threadLock;
            this.Text = this.tlock == true ? "Thread Lock Enabled " : "Thread Lock Disabled";
            this.FormClosed += (sender, EventArgs) => { Program.showMainForm(); };
            this.accountCheck = accCheck;
            this.getAccount = getAcc;
            ac = accs;
            Program.formset(this);
            this.Show();
            accountsView = createAccountsView(this.Width);
            createStatusGrid();
            Controls.Add(accountsView);
            Label mylab = new Label();
            mylab.Text = " How many ATM's do you want to simulate ";
            mylab.AutoSize = true;
            mylab.Font = new Font("Calibri", 20);
            mylab.ForeColor = Color.Black;
            mylab.Location = new Point(0, 90);
            Controls.Add(mylab);
            Button[,] btn = new Button[3, 3];
            int st = 1;
            for (int y = 0; y < 3; y++) // Loop for each button
            {
                for (int x = 0; x < 3; x++) // Loop for y
                {
                    btn[x, y] = new Button(); // Create button
                    btn[x, y].Text = st++.ToString();
                    btn[x, y].SetBounds((x * 50) + 170, (y * 40) + 200, 40, 40); // Set size & position
                    Controls.Add(btn[x, y]);
                    int k = int.Parse(btn[x, y].Text);
                    btn[x, y].Click += (sender, EventArgs) => { btn_Click(sender, EventArgs, this, k); };
                }
            }

            updateTimer.Interval = 1000;
            updateTimer.Tick += (s, n) =>
            {
                updateTable();
                // Debug.Print("test");
            };
            updateTimer.Start();
            this.FormClosed += (sender, EventArgs) => { this.updateTimer.Stop(); };
        }
        private void updateTable()
        {
            for (int x = 0; x < accountsView.RowCount; x++)
            {
                accountsView.Rows[x].Cells[0].Value = Convert.ToString(ac[x].getAccountNum());
                accountsView.Rows[x].Cells[1].Value = Convert.ToString(ac[x].getBalance());
                accountsView.Rows[x].Cells[2].Value = Convert.ToString(ac[x].getAtmCount());
            }
        }
        private void btn_Click(object sender, EventArgs e, Form F, int p)
        {
            for (int y = 0; y < p; y++)
            {
                new Thread(new ThreadStart(delegate
                {
                    Application.Run(new ATMForm(accountCheck, getAccount, this.tlock));
                })).Start();
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
            statusGrid.Location = new Point(0, this.Height - 150);
            statusGrid.ColumnCount = 5;
            statusGrid.Columns[0].Name = "time";
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
                //Debug.Print(Convert.ToString(ac[x].getAccountNum()) + " " + Convert.ToString(ac[x].getAtmCount()));
            }
            return accountsDataGridView;
        }
    }
    public partial class ATMForm : Form
    {
        private Panel screenBack;
        private Label accountNumberTextBox;
        private Label pinNumberTextBox;
        private string pinNumberAsString = "";
        private Button[] controlButtons;
        private bool loggedIn = false;
        private Label infoMessageLabel;
        private Label loginScreenLabel;
        private Button[,] numberButtons;
        private Func<string[], bool> accountCheck;
        private Func<int, Account> getAccount;
        private Button[,] buttonsSide;
        private Account account;
        private bool tlock;

        private bool mainScreen = false;
        private bool withdrawing = false;
        private bool returningCard = false;
        private bool showingBalance = false;
        private bool lockControls = false;
        private bool showingWithdraw = false;
        private int curThread = -1;
        public ATMForm(Func<string[], bool> accCheck, Func<int, Account> getAcc, bool threadLock)
        {
            this.tlock = threadLock;
            this.accountCheck = accCheck;
            this.getAccount = getAcc;

            Program.formset(this);
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
            this.FormClosed += (sender, EventArgs) => { atmClose(sender, EventArgs); };

            reset();

            Label t = new Label();
            this.Text = "ATM # : " + Convert.ToString(Thread.CurrentThread.ManagedThreadId) + ((this.tlock == true) ? " | Thread Lock Enabled " : " | Thread Lock Disabled");
            t.AutoSize = true;
            this.curThread = Thread.CurrentThread.ManagedThreadId;
            t.Location = new Point(0, 0);
            t.TextAlign = ContentAlignment.MiddleCenter;
            Controls.Add(t);
        }
        private void reset()
        {
            this.screenBack.Controls.Clear();
            this.account = null;
            this.withdrawing = false;
            this.returningCard = false;
            this.loggedIn = false;
            this.mainScreen = false;
            this.showingBalance = false;
            this.lockControls = false;
            this.showingWithdraw = false;
            loginAtmScreen();

        }
        private void atmClose(object sender, EventArgs e)
        {
            if (this.account != null)
            {
                this.account.decrementAtmCount();
            }
        }
        private void numberButtonHandler(object sender, EventArgs e, int num)
        {
            if (returningCard || lockControls)
            {
                return;
            }
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
                this.pinNumberTextBox.Text += "*";
                this.pinNumberAsString += Convert.ToString(num);
                //TODO : CHANGE THIS ?
                //this.pinNumberTextBox.Text += Convert.ToString(num);

            }
            //Debug.Print(Convert.ToString(num));

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
            controlButtons[1].Click += (sender, EventArge) => { clearButtonHandler(sender, EventArge); };
            controlButtons[0].Click += (sender, EventArgs) => { cancelButtonHandler(sender, EventArgs); };
            Show();

        }
        private void cancelButtonHandler(object sender, EventArgs e)
        {
            if (returningCard || lockControls)
            {
                return;
            }
            if (!loggedIn)
            {
                returnScreen();

            }
            else
            {

            }
        }
        private void clearButtonHandler(object sender, EventArgs e)
        {
            if (returningCard || lockControls)
            {
                return;
            }
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
                }
            }
            else
            {

            }
        }
        private void enterButtonHandler(object sender, EventArgs e)
        {
            if (returningCard || lockControls)
            {
                return;
            }
            if (this.loggedIn == false) // we are in login 
            {
                if (this.pinNumberTextBox.Visible == false)
                {
                    //we are dealing with an account number input
                    if (this.accountNumberTextBox.Text.Length < 6)
                    {
                        this.infoMessageLabel.Text = "YOUR ACCOUNT NUMBER MUST BE AT LEAST 6 \n DIGITS";
                        this.infoMessageLabel.AutoSize = true;
                        if (this.infoMessageLabel.Visible == false)
                        {
                            this.infoMessageLabel.Show();
                            System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
                            t.Interval = 3000; // 3 seconds
                            t.Tick += (s, n) =>
                            {
                                this.infoMessageLabel.Hide();
                                t.Stop();
                            };
                            t.Start();

                        }

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
                    accAndPin[1] = this.pinNumberAsString;
                    if (this.accountCheck(accAndPin))
                    {
                        loggedIn = true;
                        this.account = getAccount(Convert.ToInt32(accAndPin[0]));
                        this.account.incrementAtmCount();
                        baseMenuScreen();

                    }
                    else
                    {
                        this.infoMessageLabel.Text = "INVALID PIN OR ACCOUNT NUMBER \n COMBINATION";
                        this.infoMessageLabel.Height = screenBack.Height / 5;
                        this.infoMessageLabel.Width = screenBack.Width;
                        //this.infoMessageLabel.AutoSize = true;
                        this.infoMessageLabel.TextAlign = ContentAlignment.MiddleCenter;
                        this.infoMessageLabel.Location = new Point(0, screenBack.Height / 2);
                        if (this.infoMessageLabel.Visible == false)
                        {
                            this.pinNumberTextBox.Hide();
                            this.loginScreenLabel.Hide();
                            this.infoMessageLabel.Show();
                            System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
                            t.Interval = 3000; // 3 seconds
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
                    withdrawLabels[x, n].Location = new Point(x == 0 ? 10 : Convert.ToInt32(screenBack.Width / 2), buttonsSide[x, n].Location.Y);
                    if (x == 1)
                    {
                        withdrawLabels[x, n].TextAlign = ContentAlignment.MiddleRight;
                    }
                    screenBack.Controls.Add(withdrawLabels[x, n]);
                    i++;
                }
            }


        }
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
        private void returnScreen()
        {
            mainScreen = false;
            returningCard = true;
            loggedIn = false;
            if (this.account != null)
            {
                this.account.decrementAtmCount();
            }
            screenBack.Controls.Clear();
            this.infoMessageLabel.Text = "Returning Card \n Please Wait...";
            screenBack.Controls.Add(this.infoMessageLabel);
            this.infoMessageLabel.Show();
            System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
            this.infoMessageLabel.Height = screenBack.Height / 5;
            this.infoMessageLabel.Location = new Point(0, screenBack.Height / 2);
            t.Interval = 3000;
            t.Tick += (s, n) =>
            {
                t.Stop();
                reset();
            };
            t.Start();







        }
        private void loginAtmScreen()
        {
            this.pinNumberAsString = "";
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
                    //buttonsSide[x, n].Text = Convert.ToString(x) + " " + Convert.ToString(n); //TODO : REMOVE
                    int one = x;
                    int two = n;
                    buttonsSide[x, n].Click += (sender, EventArgs) => { sideButtonsHandler(sender, EventArgs, one, two); };
                    Controls.Add(buttonsSide[x, n]);
                }
            }


        }
        private void customWithdrawScreen()
        {

        }
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
            t.Interval = 1;
            t.Tick += (s, n) =>
             {

                 if (account.withdrawAmnt(amnt, this.curThread, tlock))
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
                 else
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
                 t.Stop();
             };
            t.Start();


        }
        private void sideButtonsHandler(object sender, EventArgs e, int one, int two)
        {
            if (returningCard || lockControls)
            {
                return;
            }
            //Debug.Print("Side button pressed");
            if (one == 0) // left side
            {
                // Debug.Print("Left side");
                //Debug.Print(Convert.ToString(two));
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
                        if (withdrawing)
                        {
                            withdrawing = false;
                            baseMenuScreen();
                        }
                        else if (showingBalance)
                        {
                            showingBalance = false;
                            baseMenuScreen();
                        }
                        else if (showingWithdraw)
                        {
                            showingWithdraw = false;
                            baseMenuScreen();
                        }
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
    public class Account
    {
        //the attributes for the account
        private int balance;
        private int pin;
        private int accountNum;
        private int atmCount;
        private Semaphore semaphore;
        object withdrawLock = new object(); // if this is static it will only allow one atm to withdraw at a time , if not it will only lock for each account
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
        //https://www.pluralsight.com/guides/lock-statement-access-data
        public Boolean withdrawAmnt(int amnt, int threadID, bool threadLock)
        {
            //Program.sendStatusMessage(Convert.ToString(this.accountNum)+ "*" + Convert.ToString();
            Program.sendStatusMessage(DateTime.Now.ToString("h:mm:ss") +
                "*" + Convert.ToString(this.accountNum) +
                "*" + Convert.ToString(threadID) +
                "*" + Convert.ToString("Access") +
                "*" + "£" + Convert.ToString(amnt)
                );
            if (threadLock)
            {
                lock (withdrawLock)
                {
                    Program.sendStatusMessage(DateTime.Now.ToString("h:mm:ss") +
                    "*" + Convert.ToString(this.accountNum) +
                    "*" + Convert.ToString(threadID) +
                    "*" + Convert.ToString("Withdraw") +
                    "*" + "£" + Convert.ToString(amnt)
                    );
                    //Thread.Sleep(5000);

                    return decrementBalance(amnt);

                }
            }
            else
            {
                Program.sendStatusMessage(DateTime.Now.ToString("h:mm:ss") +
                    "*" + Convert.ToString(this.accountNum) +
                    "*" + Convert.ToString(threadID) +
                    "*" + Convert.ToString("Withdraw") +
                    "*" + "£" + Convert.ToString(amnt)
                    );
                //Thread.Sleep(5000);

                return decrementBalance(amnt);
            }

        }
        public Boolean decrementBalance(int amount)
        {
            if (this.balance >= amount)
            {
                Thread.Sleep(3000);
                balance -= amount;
                Thread.Sleep(3000);
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

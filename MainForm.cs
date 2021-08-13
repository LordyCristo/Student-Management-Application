using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Student_Management_Project
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// This List variable store the list of students in the database.
        /// </summary>
        private List<Person> people = new List<Person>();
        /// <summary>
        /// This variable is used to determine if the Save button add a new student or update a student detail.
        /// </summary>
        private bool operation = true;
        /// <summary>
        /// This variable is used to store the Student ID when the user wants to edit a student detail.
        /// </summary>
        private string tempid;

        public MainForm()
        {
            InitializeComponent();
            refreshButton.Click += RefreshList;
            saveButton.Click += SaveStudent;
            deleteButton.Click += DeleteStudent;
            editButton.Click += EditStudent;
            cancelButton.Click += CancelProcess;
            exportButton.Click += Export;
            errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;  
            LoadInfo();
        }
        /// <summary>
        /// This function populates the lists boxes with data from the List of Person named people.
        /// </summary>
        private void RepopulateList()
        {
            idlist.DataSource = studentsList.DataSource = null;
            idlist.DataSource = studentsList.DataSource = people;
            studentsList.DisplayMember = "FullName";
            idlist.DisplayMember = "ID";
            studentsList.ClearSelected();
            idlist.ClearSelected();
        }
        /// <summary>
        /// When the refresh button is clicked this event will refresh the list's content to make the changes visible.
        /// </summary>
        private void RefreshList(object sender, EventArgs e)
        {
            LoadInfo();
        }

        /// <summary>
        /// This function retrives the data from the database and store it to List of Person named people, <br/>then calls
        /// the RepopulateList() function to make the data visible.
        /// </summary>
        private void LoadInfo()
        {
            people = DatabaseAccess.LoadDetails();
            RepopulateList();
        }

        /// <summary>
        /// This function determine if the input fields are filled appropriately.<br/>
        /// First and last names must filled, and the Student ID must fill the masked text box.
        /// </summary>
        /// <returns>
        /// Returns the <b>true</b> if no error encountered, otherwise <b>false</b>.<br/>
        /// An error icon will also appear beside the errored field.
        /// </returns>
        private bool IsInputValid()
        {
            bool result = true;
            if (String.IsNullOrWhiteSpace(firstnameTB.Text))
            {
                errorProvider.SetError(label1, "Empty value");
                result = false;
            }
            if (String.IsNullOrWhiteSpace(lastnameTB.Text))
            {
                errorProvider.SetError(label2, "Empty value");
                result = false;
            }
            if (!idnumberTB.MaskCompleted)
            {
                errorProvider.SetError(label3, "Invalid ID value");
                result = false;
            }
            return result;
        }

        /// <summary>
        /// This event will empty the text boxes to cancel the operation.
        /// </summary>
        private void CancelProcess(object sender, EventArgs e)
        { 
            firstnameTB.Text = String.Empty;
            lastnameTB.Text = String.Empty;
            idnumberTB.Text = String.Empty;
        }

        /// <summary>
        /// This event will both Add and Update data in the database. <br/>
        /// </summary>
        private void SaveStudent(object sender, EventArgs e)
        {
            errorProvider.Clear();
            if (IsInputValid())
            {
                Person student = new Person();
                student.Fname = firstnameTB.Text;
                student.Lname = lastnameTB.Text;
                student.ID = idnumberTB.Text;

                bool temp;
                if (operation)
                    temp = DatabaseAccess.SavePerson(student);
                else
                {
                    temp = DatabaseAccess.UpdatePerson(student, tempid);
                    operation = true;
                }

                if (temp)
                {
                    LoadInfo();
                    firstnameTB.Text = String.Empty;
                    lastnameTB.Text = String.Empty;
                    idnumberTB.Text = String.Empty;
                }
                else
                    errorProvider.SetError(label3, "Duplicate Student ID detected");
            }
        }

        /// <summary>
        /// This event will retrieve all the informations related to the selected row from the list box.<br/>
        /// Set also the operation variable to false to inform the Save button event that the UPDATE query must be performed.<br/>
        /// Set also the tempid variable incase the user changes the Student ID value then we still have the original value.
        /// </summary>
        private void EditStudent(object sender, EventArgs e)
        {
            if (studentsList.SelectedItems.Count > 0)
            {
                Person temp = (Person)studentsList.SelectedItem;
                firstnameTB.Text = temp.Fname;
                lastnameTB.Text = temp.Lname;
                tempid = idnumberTB.Text = temp.ID;
                operation = false;
            }
        }

        /// <summary>
        /// This event will delete the selected row in the list box and update the database.
        /// </summary>
        private void DeleteStudent(object sender, EventArgs e)
        {
            if (studentsList.SelectedItems.Count > 0)
            {
                studentsList.ValueMember = "ID";
                DatabaseAccess.DeletePerson(studentsList.SelectedValue.ToString());
                LoadInfo();
            }
        }

        /// <summary>
        /// This event will export all the data in the database into a CSV file format.
        /// </summary>
        private void Export(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Students CSV (.csv)|*.csv";
            sfd.FilterIndex = 0;
            if(sfd.ShowDialog() == DialogResult.OK)
            {
                StreamWriter f = new StreamWriter(sfd.FileName);
                string[] temp = new string[3];
                temp[0] = "ID";
                temp[1] = "Fname";
                temp[2] = "Lname";
                f.WriteLine(string.Join(",", temp));
                foreach(Person p in people)
                {
                    temp[0] = p.ID;
                    temp[1] = p.Fname;
                    temp[2] = p.Lname;
                    f.WriteLine(string.Join(",", temp));
                }
                f.Close();
                MessageBox.Show("Exported the Students Details Successfully");
            }
        }
    }
}

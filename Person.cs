namespace Student_Management_Project
{
    [System.Serializable]
    public class Person
    {
        public string ID { set; get; }
        public string Fname { set; get; }
        public string Lname { set; get; }

        public string FullName
        {
            get
            {
                return Fname + " "+ Lname;
            }
        }
    }
}

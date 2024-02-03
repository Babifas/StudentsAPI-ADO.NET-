using System.Data;
using System.Data.SqlClient;

namespace ADO.NET_EXP.Model
{
    public interface IStudentRepository
    {
        IEnumerable<Students> GetAllStudents();
        Students GetStudentById(int id);
        void AddStudents(Students students);
        void UpdateStudents(Students students,int id);
        void DeleteStudents(int id);
        List<Students> OrderedByAge();
    }
    public class StudentRepository:IStudentRepository
    {
        private readonly string _connectionString;
        public StudentRepository(IConfiguration configuration)
        {
            _connectionString = configuration["ConnectionStrings:DefaultConnection"];
        }
        public List<Students> OrderedByAge()
        {
            using(SqlConnection conn=new SqlConnection(_connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM STUDENTS",conn);
                DataSet ds = new DataSet(); 
                adapter.Fill(ds,"STUDENTS");
                DataView view=new DataView(ds.Tables["STUDENTS"]);
                view.Sort="STUDENT_AGE DESC";
                List<Students> students = new List<Students>();
                foreach(DataRowView row in view)
                {
                    students.Add(new Students
                    {
                        StudentId = Convert.ToInt32(row["STUDENT_ID"]),
                        FirstName = Convert.ToString(row["FIRST_NAME"]),
                        LastName = Convert.ToString(row["LAST_NAME"]),
                        StudentAge = Convert.ToInt32(row["STUDENT_AGE"])
                    }); ;
                }
                return students;

            }
        }
        public IEnumerable<Students> GetAllStudents()
        {
            using (SqlConnection conn=new SqlConnection(_connectionString))
            {
               conn.Open();
                //SqlCommand command = new SqlCommand("SELECT * FROM STUDENTS", conn);

                // SqlDataReader reader = command.ExecuteReader();
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM STUDENTS", conn);
                DataSet ds = new DataSet(); 
                adapter.Fill(ds,"STUDENTS");
                    List<Students> students = new List<Students>();
                foreach(DataRow row in ds.Tables["STUDENTS"].Rows)
                {
                    students.Add(new Students
                    {
                        StudentId = Convert.ToInt32(row["STUDENT_ID"]),
                        FirstName = Convert.ToString(row["FIRST_NAME"]),
                        LastName = row.Field<string>("LAST_NAME"),
                        StudentAge = row.Field<int>("STUDENT_AGE")
                    }); ;
                }
                    //while (reader.Read())
                    //{
                    //    students.Add(new Students
                    //    {
                    //        StudentId = Convert.ToInt32(reader["STUDENT_ID"]),
                    //        FirstName = reader["FIRST_NAME"].ToString(),
                    //        LastName = reader.GetString(2),
                    //        StudentAge = reader.GetInt32(3)
                    //    });
                    //}
                    return students;
                
            }
        }
       public Students GetStudentById(int studentid)
        {
            using (SqlConnection conn=new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM STUDENTS WHERE STUDENT_ID=@studentid", conn);
                cmd.Parameters.AddWithValue("@studentid", studentid);
                SqlDataReader reader = cmd.ExecuteReader();
                if(reader.Read()) {
                    return new Students
                    {
                        StudentId = reader.GetInt32(0),
                        FirstName = reader.GetString(1),
                        LastName = reader.GetString(2),
                        StudentAge = reader.GetInt32(3)
                    };
                }
                else
                {
                    return null;
                }
            }
        }
        public void AddStudents(Students student)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                //SqlCommand cmd = new SqlCommand("INSERT INTO STUDENTS(FIRST_NAME,LAST_NAME,STUDENT_AGE) VALUES(@firstname,@lastname,@studentage)",conn);
                //cmd.Parameters.AddWithValue("@firstname", student.FirstName);
                //cmd.Parameters.AddWithValue("@lastname", student.LastName);
                //cmd.Parameters.AddWithValue("@studentage", student.StudentAge);

                //cmd.ExecuteNonQuery();
                using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM STUDENTS", conn))
                {
                    SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds, "STUDENTS");
                    //DataTable studentstable = new DataTable("STUDENTS"); 
                    DataRow newrow = ds.Tables["STUDENTS"].NewRow();
                    newrow["FIRST_NAME"] = student.FirstName;
                    newrow["LAST_NAME"] = student.LastName;
                    newrow["STUDENT_AGE"] = student.StudentAge;
                    ds.Tables["STUDENTS"].Rows.Add(newrow);
                    adapter.Update(ds, "STUDENTS");
                }
            }
        }
        public void UpdateStudents(Students student, int id)
        {
            using (SqlConnection conn=new SqlConnection(_connectionString)) {
                conn.Open();
                SqlCommand cmd = new SqlCommand("UPDATE STUDENTS SET FIRST_NAME=@firstname,LAST_NAME=@lastname,STUDENT_AGE=@studentage WHERE STUDENT_ID=@studentid",conn);
                cmd.Parameters.AddWithValue("@studentid", id);
                cmd.Parameters.AddWithValue("@firstname", student.FirstName);
                cmd.Parameters.AddWithValue("@lastname", student.LastName);
                cmd.Parameters.AddWithValue("@studentage", student.StudentAge);
                cmd.ExecuteNonQuery();
            }
        }
        public void DeleteStudents(int studentid)
        {
         using(SqlConnection conn=new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM STUDENTS WHERE STUDENT_ID=@studentid", conn);
                cmd.Parameters.AddWithValue("@studentid", studentid);
                cmd.ExecuteNonQuery();
            }
        }




    }
}

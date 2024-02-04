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
        StudentsDetails StudentsDeatails(int studentid);
        string updateAge(int id,int age);
    }
    public class StudentRepository:IStudentRepository
    {
        private readonly string _connectionString;
        public StudentRepository(IConfiguration configuration)
        {
            _connectionString = configuration["ConnectionStrings:DefaultConnection"];
        }
        public string updateAge(int id, int age)
        {
            using(SqlConnection conn=new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();
                if (age > 10)
                {
                    SqlCommand cmd = new SqlCommand("UPDATE STUDENTS SET STUDENT_AGE=@age WHERE STUDENT_Id=@id",conn, transaction);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@age", age);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if(rowsAffected > 0)
                    {
                        SqlCommand cmd2 = new SqlCommand("INSERT INTO STUDENT_UPDATED_DETAILS (STUDENT_ID,UPDATED_DATE ) VALUES (@id,GETDATE())",conn,transaction);
                        cmd2.Parameters.AddWithValue("@id", id);
                        cmd2.ExecuteNonQuery();
                        transaction.Commit();
                        return "Updated Successfully";
                    }
                    else
                    {
                        transaction.Rollback();
                        return "User not found";
                    }
                   
                }
                else
                {
                    transaction.Rollback();
                    return "Can not change age less than 10";
                }
               
            }
        }
        public StudentsDetails StudentsDeatails(int studentid)
        {
            using (SqlConnection conn=new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("STUDENT_DETAILS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@STUDENT_ID", studentid);
                cmd.Parameters.Add("@FULL_NAME", SqlDbType.VarChar,50).Direction=ParameterDirection.Output;
                cmd.Parameters.Add("@STUDENT_AGE",SqlDbType.Int).Direction=ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                string fullname = cmd.Parameters["@FULL_NAME"].Value.ToString();
                int studentage = Convert.ToInt32(cmd.Parameters["@STUDENT_AGE"].Value);
                if (fullname==null )
                {
                    return null;
                }
                else
                {
                    return new StudentsDetails
                    {

                        Studentid = studentid,
                        StudentName = fullname,
                        StudentAge = studentage
                    };
                   
                }

            //    SqlDataReader reader = cmd.ExecuteReader();
            //    if (reader.Read())
            //    {
            //        return new StudentsDetails
            //        {
            //            StudentId = studentid,
            //            StudentName = reader.GetString(0),
            //            StudentAge = reader.GetInt32(1)

            //        };
            //}
            //    else
            //{
            //    return null;
            //}

        }
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

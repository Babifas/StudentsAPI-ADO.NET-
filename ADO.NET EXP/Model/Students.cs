﻿using System.ComponentModel.DataAnnotations;

namespace ADO.NET_EXP.Model
{
    public class Students
    {
     
      public int StudentId { get; set; }
      public string? FirstName { get; set; }
      public string? LastName { get; set; }
      public int StudentAge { get; set; }
    }
    public class StudentsDetails
    {
        public int Studentid { get; set; }
        public string StudentName { get; set; }
        public int StudentAge { get; set;}
    }
}

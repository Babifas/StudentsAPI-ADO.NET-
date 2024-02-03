﻿using ADO.NET_EXP.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ADO.NET_EXP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentRepository _studentRepository;
        public StudentController(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }



        [HttpGet]
        public IActionResult GetAllStudents()
        {
          return Ok(_studentRepository.GetAllStudents());
        }
        [HttpGet("OrderByAge")]
        public IActionResult OrderedByAge()
        {
            return Ok(_studentRepository.OrderedByAge());
        }
        [HttpGet("{id}")]
        public IActionResult GetStudentByID(int id) { 
        
            if(_studentRepository.GetStudentById(id) == null)
            {
                return NotFound();
            }
            return Ok(_studentRepository.GetStudentById(id));

        }
        [HttpPost]
        public IActionResult AddStudent([FromBody]  Students students )
        {
            _studentRepository.AddStudents(students);
            return Ok("Student added succesfully");
        }
        [HttpPut("{id}")]
        public IActionResult UpdateStudent([FromBody] Students student,int id )
        {
          _studentRepository.UpdateStudents(student, id);
            return Ok("updated successfully");

        }
        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
            _studentRepository.DeleteStudents(id);
            return Ok("deleted successfully");
        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentService.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public int LastName { get; set; }
        public string Course { get; set; }
        public int Marks { get; set; }
        public string Gender { get; set; }
    }
}
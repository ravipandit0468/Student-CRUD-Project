using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace StudentService.Controllers
{
    public class StudentController : ApiController
    {
        //StudentDBEntities db=new StudentDBEntities();
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDBconn"].ConnectionString);
        Student  stud = new Student();

        [HttpGet]
        [Route("api/Student/ShowPages")]
        public HttpResponseMessage ShowPages(int start,int end)
        {
            List<Student>students = new List<Student>();
            try
            {
                SqlCommand cmd = new SqlCommand("GetStudentsByRange",con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@start", start);
                cmd.Parameters.AddWithValue("@end", end);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    Student s = new Student();
                    s.Id = (int)reader["Id"];
                    s.FirstName = reader["FirstName"].ToString();
                    s.LastName = reader["LastName"].ToString();
                    s.Course = reader["Course"].ToString();
                    s.Marks = (int)reader["Marks"];
                    s.Gender = reader["Gender"].ToString();
                    students.Add(s);
                }
                
                //var students = db.Students
                //         .OrderBy(s => s.Id)
                //         .Skip(start - 1)     
                //         .Take(end - start + 1) 
                //         .ToList();

                return Request.CreateResponse(HttpStatusCode.OK, students);
            }
            catch(Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest,e);
            }
        }

        [HttpGet]
        [Route("api/Student/GetStudents")]
        public HttpResponseMessage GetStudents(string sortby = "Id", string sortorder = "asc")
        {
            try
            {
                SqlCommand cmd = new SqlCommand("GetSortedStudents", con);
                cmd.CommandType=CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@sortby", sortby);
                cmd.Parameters.AddWithValue("@sortorder", sortorder);
                if (con.State == ConnectionState.Closed)
                    con.Open();
                List<Student> students = new List<Student>();
                SqlDataReader reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    students.Add(new Student
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        FirstName = reader["FirstName"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        Course = reader["Course"].ToString(),
                        Marks = Convert.ToInt32(reader["Marks"]),
                        Gender = reader["Gender"].ToString()
                    });
                }
                return Request.CreateResponse(HttpStatusCode.OK, students);
                //var students = db.Students.AsQueryable();
                //switch (sortby.ToLower())
                //{
                //    case "id":
                //    {
                //        students = (sortorder == "asc") ? students.OrderBy(s => s.Id) : students.OrderByDescending(s=>s.Id);
                //        return Request.CreateResponse(HttpStatusCode.OK,students.ToList());
                //    }
                //    case "firstname":
                //    {
                //        students = (sortorder == "asc") ? students.OrderBy(s => s.FirstName) : students.OrderByDescending(s=>s.FirstName);
                //        return Request.CreateResponse(HttpStatusCode.OK, students.ToList());
                //    }
                //    case "lastname":
                //    {
                //        students = (sortorder == "asc") ? students.OrderBy(s => s.LastName) : students.OrderByDescending(s=>s.LastName);
                //            return Request.CreateResponse(HttpStatusCode.OK, students.ToList());
                //        }
                //    case "course":
                //    {
                //        students = (sortorder == "asc") ? students.OrderBy(s => s.Course) : students.OrderByDescending(s=>s.Course);
                //            return Request.CreateResponse(HttpStatusCode.OK, students.ToList());
                //        }
                //    case "marks":
                //    {
                //        students = (sortorder == "asc") ? students.OrderBy(s => s.Marks) : students.OrderByDescending(s=>s.Marks);
                //            return Request.CreateResponse(HttpStatusCode.OK, students.ToList());
                //        }
                //    case "Gender":
                //    {
                //        students = (sortorder == "asc") ? students.OrderBy(s => s.Gender) : students.OrderByDescending(s => s.Gender);
                //            return Request.CreateResponse(HttpStatusCode.OK, students.ToList());
                //        }
                //    default:
                //        {
                //            students = (sortorder == "asc") ? students.OrderBy(s => s.FirstName) : students.OrderByDescending(s => s.FirstName);
                //            return Request.CreateResponse(HttpStatusCode.OK, students.ToList());
                //        }
                //}

            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }

        [HttpPost]
        [Route("api/Student/Search")]
        public HttpResponseMessage Search([FromBody] string searchStr)
        {
            List<Student> students = new List<Student>();
            try
            {
                if (string.IsNullOrWhiteSpace(searchStr))
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound,"Search String Not Matched With Table Data.");
                }
                //x => x.Gender != null && x.Gender.Equals(searchStr, StringComparison.OrdinalIgnoreCase)
                //var students = db.Students.Where(x => x.FirstName!=null && x.FirstName.ToLower().StartsWith(searchStr.ToLower())
                //                                    ||x.LastName != null && x.LastName.ToLower().StartsWith(searchStr.ToLower())).ToList();

                SqlCommand cmd = new SqlCommand("GetAllStudents",con);
                cmd.CommandType=CommandType.StoredProcedure;
                con.Open();
                SqlDataReader reader =cmd.ExecuteReader();
                while (reader.Read())
                {
                    Student s = new Student();
                    if (reader["FirstName"]!=null && reader["FirstName"].ToString().ToLower().StartsWith(searchStr.ToLower()) || reader["LastName"]!=null && reader["LastName"].ToString().ToLower().StartsWith(searchStr.ToLower()))
                    {
                        s.Id = (int)reader["Id"];
                        s.FirstName = reader["FirstName"].ToString();
                        s.LastName = reader["LastName"].ToString() ;
                        s.Course = reader["Course"].ToString();
                        s.Marks = (int)reader["Marks"];
                        s.Gender = reader["Gender"].ToString();
                        students.Add(s);
                    }
                }

                return Request.CreateResponse(HttpStatusCode.OK, students);
            }
            catch(Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }
        public List<Student>Get()
        {
            //return db.Students.ToList();
            List<Student> students = new List<Student>();
            SqlCommand getAllCmd = new SqlCommand("GetAllStudents",con);
            getAllCmd.CommandType=CommandType.StoredProcedure;
            con.Open();
            SqlDataReader AllStudents=getAllCmd.ExecuteReader();
            while (AllStudents.Read())
            {
                Student s = new Student();
                s.Id = (int)AllStudents["Id"];
                s.FirstName = AllStudents["FirstName"].ToString();
                s.LastName = AllStudents["LastName"].ToString();
                s.Course = AllStudents["Course"].ToString();
                s.Marks = (int)AllStudents["Marks"];
                s.Gender = AllStudents["Gender"].ToString();
                students.Add(s);
            }
            con.Close();
            return students;
        }
        public HttpResponseMessage Get([FromUri]int id)
        {
            //var StudentsRecord = db.Students.FirstOrDefault(x=> x.Id == id);
            SqlCommand cmd = new SqlCommand("CheckStudentWithIdExist",con);
            cmd.CommandType=CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            int result = Convert.ToInt32(cmd.ExecuteScalar());
            bool exist = (result == 1);
            if (exist)
            {
                SqlCommand getcmd = new SqlCommand("GetStudentById",con);
                getcmd.CommandType= CommandType.StoredProcedure;
                getcmd.Parameters.AddWithValue("@Id", id);
                if (con.State == ConnectionState.Closed)
                    con.Open();
                SqlDataReader reader=getcmd.ExecuteReader();
                if(reader.Read())
                {
                    Student StudentRecord=new Student();
                    StudentRecord.Id = id;
                    StudentRecord.FirstName = reader["FirstName"].ToString();
                    StudentRecord.LastName = reader["LastName"].ToString();
                    StudentRecord.Course = reader["Course"].ToString();
                    StudentRecord.Marks = (int)reader["Marks"];
                    StudentRecord.Gender = reader["Gender"].ToString();
                    return Request.CreateResponse(HttpStatusCode.OK,StudentRecord);
                }
                return Request.CreateResponse(HttpStatusCode.OK,"Record Not Able To Fetched.");
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Student With Id= " + id + " is Not Found.");
            }
            //if (StudentsRecord != null)
            //{
            //    return Request.CreateResponse(HttpStatusCode.OK, StudentsRecord);
            //}
            //else
            //{
            //    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Student With Id= " + id + " is Not Found.");
            //}
        }
        public HttpResponseMessage Post([FromBody] Student student)
        {
            try
            {
                bool exist = false;
                SqlCommand CheckCmd = new SqlCommand("CheckStudentWithIdExist", con);
                CheckCmd.CommandType = CommandType.StoredProcedure;
                CheckCmd.Parameters.AddWithValue("@Id",student.Id);
                con.Open();
                int result=Convert.ToInt32(CheckCmd.ExecuteScalar());
                exist = (result == 1);
                //var exist=db.Students.Any(x=>x.Id==student.Id);
                if (exist)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.Conflict, "Student With Id= "+student.Id+" Is Already Present In Table.");
                }
                else if(student.Id<=0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest,"Id Must Be Positive.");
                }
                //db.Students.Add(student);
                //db.SaveChanges();

                SqlCommand SaveCmd = new SqlCommand("CreateNewStudent",con);
                SaveCmd.CommandType = CommandType.StoredProcedure;
                if (con.State == ConnectionState.Closed)
                    con.Open();
                SaveCmd.Parameters.AddWithValue("@Id", student.Id);
                SaveCmd.Parameters.AddWithValue("@FirstName", student.FirstName);
                SaveCmd.Parameters.AddWithValue("@LastName", student.LastName);
                SaveCmd.Parameters.AddWithValue("@Course", student.Course);
                SaveCmd.Parameters.AddWithValue("@Marks", student.Marks);
                SaveCmd.Parameters.AddWithValue("@Gender", student.Gender);
                SaveCmd.ExecuteNonQuery();
                var message = Request.CreateResponse(HttpStatusCode.Created);
                return message;
            }
            catch (Exception ex) 
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
        public HttpResponseMessage Put([FromUri]int id,[FromBody]Student student)
        {
            try
            {
                //var entity = db.Students.FirstOrDefault(e => e.Id == id);
                SqlCommand CheckCmd = new SqlCommand("CheckStudentWithIdExist", con);
                CheckCmd.CommandType = CommandType.StoredProcedure;
                CheckCmd.Parameters.AddWithValue("@id", id);
                con.Open();
                int result =Convert.ToInt32(CheckCmd.ExecuteScalar());

                if (result == 1)
                {
                    string Fname=student.FirstName;
                    string Lname=student.LastName;
                    string Cname=student.Course;
                    int? Cmarks=student.Marks;
                    string Gname=student.Gender;
                    SqlCommand updtCmd = new SqlCommand("UpdateById", con);
                    updtCmd.CommandType = CommandType.StoredProcedure;
                    if (con.State == ConnectionState.Closed)
                        con.Open();
                    updtCmd.Parameters.AddWithValue("@Id", id);
                    updtCmd.Parameters.AddWithValue("@FirstName", Fname);
                    updtCmd.Parameters.AddWithValue("@LastName", Lname);
                    updtCmd.Parameters.AddWithValue("@Course", Cname);
                    updtCmd.Parameters.AddWithValue("@Marks", Cmarks);
                    updtCmd.Parameters.AddWithValue("@Gender", Gname);
                    int i=updtCmd.ExecuteNonQuery();
                    if (i > 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK,"Updated Student Data.");
                    }
                    return Request.CreateResponse(HttpStatusCode.OK,"Not Updated.");
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Student With Id= " + id.ToString() + " Is Not Found For Update.");
                }

                //if (entity != null)
                //{
                //    entity.FirstName = student.FirstName;
                //    entity.LastName = student.LastName;
                //    entity.Course = student.Course;
                //    entity.Marks = student.Marks;
                //    entity.Gender = student.Gender;
                //    db.SaveChanges();
                //    return Request.CreateResponse(HttpStatusCode.OK, entity);
                //}
                //else
                //{
                //    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Student With Id= " + id.ToString() + " Is Not Found For Update.");
                //}
            }
            catch(Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest,e);
            }
        }
        public HttpResponseMessage Delete([FromUri] int id)
        {
            try
            {
                //var entity = db.Students.FirstOrDefault(e => e.Id == id);

                SqlCommand CheckCmd = new SqlCommand("CheckStudentWithIdExist", con);
                CheckCmd.CommandType = CommandType.StoredProcedure;
                CheckCmd.Parameters.AddWithValue("@id", id);
                con.Open();
                int result = Convert.ToInt32(CheckCmd.ExecuteScalar());

                if (result == 1)
                {
                    SqlCommand dltcmd = new SqlCommand("DeleteStudentById", con);
                    dltcmd.CommandType=CommandType.StoredProcedure;
                    dltcmd.Parameters.AddWithValue("@id", id);
                    if (con.State == ConnectionState.Closed)
                        con.Open();
                    int i=dltcmd.ExecuteNonQuery();
                    if (i > 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, "Student Record Removed Successfully.");
                    }
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Student Record Not Removed.");
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Student With Id= " + id + " is Not Found.");
                }
                //if (entity != null)
                //{
                //    db.Students.Remove(entity);
                //    db.SaveChanges();
                //    return Request.CreateResponse(HttpStatusCode.OK, "Student Record Removed Successfully.");
                //}
                //else
                //{
                //    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Student With Id= " + id + " is Not Found.");
                //}
            }
            catch(Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest,e);
            }
        }
    }
}

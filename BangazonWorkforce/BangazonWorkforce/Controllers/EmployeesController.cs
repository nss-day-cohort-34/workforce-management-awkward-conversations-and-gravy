using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BangazonWorkforce.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace BangazonWorkforce.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IConfiguration _config;

        public EmployeesController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }
        // GET: Employees
        public ActionResult Index() 
        {
            using (SqlConnection conn = Connection) 
            {
                conn.Open(); 
                using (SqlCommand cmd = conn.CreateCommand()) 
                {


                    cmd.CommandText = @"
                                      SELECT e.Id,
                                             e.FirstName,
                                             e.LastName,
                                             e.DepartmentId,
                                             d.Name as DepartmentName
                                        FROM Employee e
                                   LEFT JOIN Department d on d.Id = e.DepartmentId
                                    ORDER BY e.LastName, e.FirstName";

                    SqlDataReader reader = cmd.ExecuteReader(); 
                    List<Employee> employees = new List<Employee>();
                    while (reader.Read())
                    {
                        employees.Add(
                            new Employee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                Department = new Department()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                    Name = reader.GetString(reader.GetOrdinal("DepartmentName")),
                                }
                            });
                    }

                    reader.Close(); 

                    return View(employees); 
                }
            }
        }

        // GET: Employees/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Employees/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }


        /*  HELPER METHODS   */
        private Employee GetEmployeeById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT e.FirstName, 
		                                       e.LastName, 
		                                       d.Name as DepartmentName, 
		                                       tp.Name as TrainingProgramName,
		                                       tp.EndDate as TrainingProgramEndDate,
		                                       c.Manufacturer + ' ' + c.Make as Computer
                                          FROM Employee e
                                     LEFT JOIN Department d 
                                            ON d.Id = e.DepartmentId
                                     LEFT JOIN ComputerEmployee ce 
                                            ON ce.EmployeeId = e.Id
                                     LEFT JOIN Computer c 
                                            ON ce.ComputerId = c.Id
                                     LEFT JOIN EmployeeTraining et 
                                            ON et.EmployeeId = e.Id
                                     LEFT JOIN TrainingProgram tp 
                                            ON tp.Id = et.TrainingProgramId 
	                                     WHERE tp.EndDate >= GETDATE() OR tp.EndDate IS NULL
                                      ORDER BY e.LastName, e.FirstName;";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    var reader = cmd.ExecuteReader();

                    Employee employee = null;
                    if (reader.Read())
                    {
                        employee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        }
                    }
                }
            }
        }
    }
}
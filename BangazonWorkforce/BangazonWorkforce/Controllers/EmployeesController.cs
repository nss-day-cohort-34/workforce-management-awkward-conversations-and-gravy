using BangazonWorkforce.Models;
using BangazonWorkforce.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

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
                                             e.IsSupervisor,
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
                                IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSupervisor")),
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
            EmployeeDetailViewModel employee = GetEmployeeById(id);
            return View(employee);
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            var viewModel = new EmployeeCreateViewModel();
            var departments = GetAllDepartments();
            var selectItems = departments
                .Select(department => new SelectListItem
                {
                    Text = department.Name,
                    Value = department.Id.ToString()
                })
                .ToList();

            selectItems.Insert(0, new SelectListItem
            {
                Text = "Choose Department...",
                Value = "0"
            });
            viewModel.Departments = selectItems;
            return View(viewModel);
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EmployeeCreateViewModel model)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO Employee
                ( FirstName, LastName, IsSupervisor, DepartmentId )
                VALUES
                ( @firstName, @lastName, @isSupervisor, @departmentId )";
                        cmd.Parameters.Add(new SqlParameter("@firstName", model.Employee.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastName", model.Employee.LastName));
                        cmd.Parameters.Add(new SqlParameter("@isSupervisor", model.Employee.IsSupervisor));
                        cmd.Parameters.Add(new SqlParameter("@departmentId", model.Employee.DepartmentId));
                        cmd.ExecuteNonQuery();

                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(int id)
        {
            var viewModel = new EmployeeEditViewModel();
            var departments = GetAllDepartments();
            var oneComputer = GetComputerById(id);
            var trainingPrograms = GetFutureTrainingPrograms();
            var selectItems = departments
                .Select(department => new SelectListItem
                {
                    Text = department.Name,
                    Value = department.Id.ToString()
                })
                .ToList();

            selectItems.Insert(0, new SelectListItem
            {
                Text = "Choose department...",
                Value = "0"
            });
            viewModel.Departments = selectItems;

            var computers = GetAllComputers();
            if (oneComputer == null)
            {
                var selectItemsComputers = computers
                    .Select(computer => new SelectListItem
                    {
                        Text = computer.Make,
                        Value = computer.Id.ToString()
                    })
                    .ToList();

                selectItemsComputers.Insert(0, new SelectListItem
                {
                    Text = "No Computer Assigned",
                    Value = "0"
                });
                viewModel.Computers = selectItemsComputers;
            }
            else
            {

                computers.Add(oneComputer);
                viewModel.ComputerId = oneComputer.Id;
                var selectItemsComputers = computers
                    .Select(computer => new SelectListItem
                    {
                        Text = computer.Make,
                        Value = computer.Id.ToString()
                    })
                    .ToList();

                selectItemsComputers.Insert(0, new SelectListItem
                {
                    Text = "No Computer Assigned",
                    Value = "0"
                });
                viewModel.Computers = selectItemsComputers;
            }

            var selectTrainingProgramsItems = trainingPrograms
                .Select(tp => new SelectListItem
                {
                    Text = tp.Name,
                    Value = tp.Id.ToString()
                }).ToList();
            viewModel.TrainingPrograms = selectTrainingProgramsItems;

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                               SELECT e.Id, e.firstName, e.lastName, e.departmentId, tp.Name as TrainingProgramName, tp.Id as TrainingProgramId
                                 FROM Employee e
                            LEFT JOIN EmployeeTraining et ON e.Id = et.EmployeeId
                            LEFT JOIN TrainingProgram tp on tp.Id = et.TrainingProgramId
                                WHERE @id = e.id
                                         ";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Employee employee = new Employee();
                    while (reader.Read())
                    {
                        employee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("firstName")),
                            LastName = reader.GetString(reader.GetOrdinal("lastName")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("departmentId")),
                            TrainingPrograms = new List<TrainingProgram>()
                        };
                        if (!reader.IsDBNull(reader.GetOrdinal("TrainingProgramId")))
                        {
                            employee.TrainingPrograms.Add(
                                new TrainingProgram()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("TrainingProgramId")),
                                    Name = reader.GetString(reader.GetOrdinal("TrainingProgramName"))
                                }
                            );
                        }
                        viewModel.Employee = employee;
                        viewModel.SelectedTrainingProgramIds = employee.TrainingPrograms.Select(tp => tp.Id).ToList();
                        
                    }

                    reader.Close();

                }
            }
            return View(viewModel);
        }



        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, EmployeeEditViewModel model)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        var oneComputer = GetComputerById(id);
                        if (model.ComputerId == 0 && oneComputer != null)
                        {
                            cmd.CommandText = @"
                                                  Update Employee set firstName = @firstName, lastName = @lastName, departmentId = @departmentId where id = @id
                                                  Update ComputerEmployee set UnassignDate = @unassignDate where EmployeeId = @employeeId
                                               ";

                        }
                        else if (model.ComputerId != 0 && oneComputer == null)
                        {
                            cmd.CommandText = @"
                                                  Update Employee set firstName = @firstName, lastName = @lastName, departmentId = @departmentId where id = @id
                                                  Insert into ComputerEmployee (EmployeeId, ComputerId, AssignDate) Values (@employeeId, @computerId, @assignDate)
                                               ";
                        }
                        else if (model.ComputerId != 0)
                        {
                            cmd.CommandText = @"
                                                  Update Employee set firstName = @firstName, lastName = @lastName, departmentId = @departmentId where id = @id
                                                  Update ComputerEmployee set UnassignDate = @unassignDate where EmployeeId = @employeeId
                                                  Insert into ComputerEmployee (EmployeeId, ComputerId, AssignDate) Values (@employeeId, @computerId, @assignDate)
                                               ";
                        }
                        else
                        {
                            cmd.CommandText = @"
                                                  Update Employee set firstName = @firstName, lastName = @lastName, departmentId = @departmentId where id = @id
                                               ";
                        }
                        var newDate = DateTime.Now;
                        cmd.Parameters.Add(new SqlParameter("@lastName", model.Employee.LastName));
                        cmd.Parameters.Add(new SqlParameter("@firstName", model.Employee.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@departmentId", model.Employee.DepartmentId));
                        cmd.Parameters.Add(new SqlParameter("@computerId", model.ComputerId));
                        cmd.Parameters.Add(new SqlParameter("@assignDate", newDate));
                        cmd.Parameters.Add(new SqlParameter("@unassignDate", newDate));
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.Parameters.Add(new SqlParameter("@employeeId", id));
                        cmd.ExecuteNonQuery();
                    }

                    return RedirectToAction(nameof(Index));
                }
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
        private EmployeeDetailViewModel GetEmployeeById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"   SELECT e.FirstName, 
												e.LastName, 
												  d.Id as DepartmentId,
                                        		  d.Name as DepartmentName, 
												  tp.Id as TrainingProgramId,
                                        		  tp.Name as TrainingProgramName,
                                        		  tp.StartDate as TrainingProgramStartDate,
												  c.Id as ComputerId,
                                                  isNull(c.Manufacturer + ' ' + c.Make, 'No Computer') as Computer
                                             FROM Employee e
                                        LEFT JOIN Department d 
                                               ON d.Id = e.DepartmentId
                                        LEFT JOIN ComputerEmployee ce 
                                               ON ce.EmployeeId = e.Id and ce.UnassignDate is null
                                        LEFT JOIN Computer c 
                                               ON ce.ComputerId = c.Id 
                                        LEFT JOIN EmployeeTraining et 
                                               ON et.EmployeeId = e.Id
                                        LEFT JOIN TrainingProgram tp 
                                               ON tp.Id = et.TrainingProgramId 
                                        	WHERE @id = e.id
                                         ORDER BY e.LastName, e.FirstName, tp.StartDate";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    var reader = cmd.ExecuteReader();

                    EmployeeDetailViewModel employee = null;
                    while (reader.Read())
                    {
                        if (employee == null)
                        {
                            employee = new EmployeeDetailViewModel
                            {
                                Id = id,
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                Department = new Department
                                {
                                    Name = reader.GetString(reader.GetOrdinal("DepartmentName"))
                                },
                                Computer = new Computer
                                {
                                    Manufacturer = reader.GetString(reader.GetOrdinal("Computer"))
                                },
                                TrainingPrograms = new List<TrainingProgram>()
                            };
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("TrainingProgramId")))
                        {
                            employee.TrainingPrograms.Add(
                                new TrainingProgram()
                                {
                                    Name = reader.GetString(reader.GetOrdinal("TrainingProgramName")),
                                    StartDate = reader.GetDateTime(reader.GetOrdinal("TrainingProgramStartDate"))
                                }
                            );
                        }
                    }
                    reader.Close();
                    return employee;
                }
            }
        }

        private List<Department> GetAllDepartments()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, name as departmentName FROM Department";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Department> departments = new List<Department>();
                    while (reader.Read())
                    {
                        departments.Add(new Department
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("departmentName")),
                        });
                    }

                    reader.Close();

                    return departments;
                }
            }
        }

        private List<Computer> GetAllComputers()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Id, c.Manufacturer + ' ' + c.Make as ComputerName
FROM Computer c
LEFT JOIN ComputerEmployee ce ON c.Id = ce.ComputerId
WHERE ce.Id IS NULL 
OR c.Id IN (
        SELECT ce.ComputerId
        FROM ComputerEmployee ce
        WHERE ce.UnassignDate IS NOT NULL and c.DecomissionDate is null
                       AND ce.ComputerId NOT IN (
                                SELECT ce.ComputerId
                                FROM ComputerEmployee ce
                                WHERE ce.UnassignDate IS NULL)
                                )";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Computer> computers = new List<Computer>();

                    while (reader.Read())
                    {
                        int Id = reader.GetInt32(reader.GetOrdinal("Id"));

                        if (computers.Find(c => c.Id == Id) == null)
                        {
                            computers.Add(new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Make = reader.GetString(reader.GetOrdinal("ComputerName")),
                            });
                        }
                    }

                    reader.Close();

                    return computers;
                }
            }
        }
        private Computer GetComputerById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        Select c. manufacturer + ' ' + c.make as ComputerName, c.id
                                from Computer c left join ComputerEmployee ce on ce.ComputerId = c.Id left join Employee e on e.Id = ce.EmployeeId 
                                where ce.EmployeeId = @id and DecomissionDate is null and UnassignDate is null";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    var reader = cmd.ExecuteReader();

                    Computer computer = null;
                    while (reader.Read())
                    {
                        if (computer == null)
                        {
                            computer = new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Make = reader.GetString(reader.GetOrdinal("ComputerName")),
                            };
                        }

                    }
                    reader.Close();
                    return computer;
                }
            }
        }

        private List<TrainingProgram> GetFutureTrainingPrograms()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT tp.Id as TrainingProgramId,
                                               tp.Name as TrainingProgramName, 
                                               tp.StartDate as TrainingProgramStartDate, 
                                               tp.EndDate as TrainingProgramEndDate, 
                                               tp.MaxAttendees as TrainingProgramMaxAttendees
                                          FROM TrainingProgram tp
                                     LEFT JOIN EmployeeTraining et
                                            ON tp.Id = et.TrainingProgramId
                                         WHERE SYSDATETIME() <= StartDate";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<TrainingProgram> trainingPrograms = new List<TrainingProgram>();

                    while (reader.Read())
                    {
                        trainingPrograms.Add(new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("TrainingProgramId")),
                            Name = reader.GetString(reader.GetOrdinal("TrainingProgramName")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("TrainingProgramStartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("TrainingProgramEndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("TrainingProgramMaxAttendees"))
                        });
                    }
                    reader.Close();
                    return trainingPrograms;
                };

            }
        }
    }
}



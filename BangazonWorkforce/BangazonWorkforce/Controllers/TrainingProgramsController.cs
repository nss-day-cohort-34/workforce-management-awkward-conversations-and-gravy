﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BangazonWorkforce.Models;
using BangazonWorkforce.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace BangazonWorkforce.Controllers
{
    public class TrainingProgramsController : Controller
    {
        private readonly IConfiguration _config;

        public TrainingProgramsController(IConfiguration config)
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
        // GET: TrainingPrograms
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {


                    cmd.CommandText = @"
                                  SELECT 
                                        Id, Name, StartDate, EndDate, MaxAttendees
                                  FROM 
                                        TrainingProgram
                                  WHERE
                                        StartDate >= GetDate()";

                    SqlDataReader reader = cmd.ExecuteReader();
                    List<TrainingProgram> trainingPrograms = new List<TrainingProgram>();
                    while (reader.Read())
                    {
                        trainingPrograms.Add(
                            new TrainingProgram
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                                MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))

                            });
                    }

                    reader.Close();

                    return View(trainingPrograms);
                }
            }
        }

        // GET: TrainingPrograms/Details/5
        public ActionResult Details(int id)
        {
            var training = GetTrainingProgramById(id);
            return View(training);
        }

        // GET: TrainingPrograms/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TrainingPrograms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TrainingProgram trainingProgram)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @" INSERT INTO TrainingProgram (Name, StartDate, EndDate, MaxAttendees)
                                                VALUES (@name, @startDate, @endDate, @maxAttendees)";
                        cmd.Parameters.Add(new SqlParameter("@name", trainingProgram.Name));
                        cmd.Parameters.Add(new SqlParameter("@startDate", trainingProgram.StartDate));
                        cmd.Parameters.Add(new SqlParameter("@endDate", trainingProgram.EndDate));
                        cmd.Parameters.Add(new SqlParameter("@maxAttendees", trainingProgram.MaxAttendees));


                        cmd.ExecuteNonQuery();
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TrainingPrograms/Edit/5
        public ActionResult Edit(int id)
        {
            var training = GetFutureTrainingProgramById(id);
            return View(training);
        }

        // POST: TrainingPrograms/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, TrainingProgram trainingProgram)
        {
            var updatedTrainingProgram = trainingProgram;
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        if (updatedTrainingProgram.MaxAttendees >= trainingProgram.EmployeeCount)
                        {
                            cmd.CommandText = @"UPDATE TrainingProgram
                                            SET Name = @name,
                                                StartDate= @startDate,
                                                EndDate = @endDate,
                                                MaxAttendees =@maxAttendees
                                                FROM TrainingProgram
                                                LEFT JOIN EmployeeTraining ON TrainingProgram.Id = EmployeeTraining.Id
                                                WHERE StartDate >= GetDate()AND TrainingProgram.Id = @id 
                                                   ";



                            cmd.Parameters.Add(new SqlParameter("@id", id));
                            cmd.Parameters.Add(new SqlParameter("@name", updatedTrainingProgram.Name));
                            cmd.Parameters.Add(new SqlParameter("@startDate", updatedTrainingProgram.StartDate));
                            cmd.Parameters.Add(new SqlParameter("@endDate", updatedTrainingProgram.EndDate));
                            cmd.Parameters.Add(new SqlParameter("@maxAttendees", updatedTrainingProgram.MaxAttendees));
                            cmd.ExecuteNonQuery();


                            return RedirectToAction(nameof(Index));
                        } else
                        {
                            cmd.CommandText = @"UPDATE TrainingProgram
                                            SET Name = @name,
                                                StartDate= @startDate,
                                                EndDate = @endDate,
                                                FROM TrainingProgram
                                                LEFT JOIN EmployeeTraining ON TrainingProgram.Id = EmployeeTraining.Id
                                                WHERE StartDate >= GetDate()AND TrainingProgram.Id = @id 
                                                   ";

                            cmd.Parameters.Add(new SqlParameter("@id", id));
                            cmd.Parameters.Add(new SqlParameter("@name", updatedTrainingProgram.Name));
                            cmd.Parameters.Add(new SqlParameter("@startDate", updatedTrainingProgram.StartDate));
                            cmd.Parameters.Add(new SqlParameter("@endDate", updatedTrainingProgram.EndDate));
                            cmd.ExecuteNonQuery();


                            return RedirectToAction(nameof(Index));

                        }
                    }
                }


            }
            catch
            {
                return View();
            }
        }


        // GET: TrainingPrograms/Delete/5
        public ActionResult Delete(int id)
        {
            var trainingProgram = GetFutureTrainingProgramById(id);
            return View(trainingProgram);
        }

        // POST: TrainingPrograms/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, TrainingProgram trainingProgram)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
	                    	DELETE et FROM EmployeeTraining et 
	                     LEFT JOIN TrainingProgram tp ON et.TrainingProgramId = tp.Id
	                    	 WHERE et.TrainingProgramId = @Id AND SYSDATETIME() <= tp.StartDate;
	                    	DELETE tp FROM TrainingProgram tp
	                     LEFT JOIN EmployeeTraining et
	                    		ON et.TrainingProgramId = tp.Id
	                         WHERE tp.Id = @Id AND SYSDATETIME() <= tp.StartDate";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        /* HELPER METHOD */
        private TrainingProgram GetFutureTrainingProgramById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT tp.Id, 
	                                           tp.Name as TrainingProgramName, 
	                                           tp.StartDate as TrainingProgramStartDate, 
	                                           tp.EndDate as TrainingProgramEndDate, 
	                                           tp.MaxAttendees as TrainingProgramMaxAttendees,
                                               et.TrainingProgramId, COUNT(et.Id)AS EmployeeCount
                                          FROM TrainingProgram tp
                                     LEFT JOIN EmployeeTraining et
                                            ON et.TrainingProgramId = tp.Id
                                         WHERE SYSDATETIME() <= tp.StartDate AND @id = tp.Id
                                        GROUP BY tp.Id,tp.Name ,tp.StartDate, tp.EndDate ,tp.MaxAttendees,et.TrainingProgramId";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    var reader = cmd.ExecuteReader();

                    TrainingProgram trainingProgram = null;

                    while (reader.Read())
                    {
                        if (trainingProgram == null)
                            trainingProgram = new TrainingProgram
                            {
                                Id = id,
                                Name = reader.GetString(reader.GetOrdinal("TrainingProgramName")),
                                StartDate = reader.GetDateTime(reader.GetOrdinal("TrainingProgramStartDate")),
                                EndDate = reader.GetDateTime(reader.GetOrdinal("TrainingProgramEndDate")),
                                MaxAttendees = reader.GetInt32(reader.GetOrdinal("TrainingProgramMaxAttendees")),
                                EmployeeCount = reader.GetInt32(reader.GetOrdinal("EmployeeCount"))

                            };
                    }
                    reader.Close();
                    return trainingProgram;
                }
            }
        }

        private TrainingProgram GetTrainingProgramById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"Select et.EmployeeId, e.FirstName ,e.LastName,tp.Name, tp.StartDate, tp.EndDate, tp.MaxAttendees
                                        From TrainingProgram tp LEFT Join EmployeeTraining et
                                        ON et.TrainingProgramId = tp.Id
                                        LEFT JOIN Employee e ON e.Id = et.EmployeeId
                                         WHERE tp.Id = @Id ";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    var reader = cmd.ExecuteReader();
                    TrainingProgram trainingProgram = null;
                    while (reader.Read())
                    {
                        if (trainingProgram == null)
                        {
                            trainingProgram = new TrainingProgram
                            {
                                Id = id,
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                                MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees")),

                            };

                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("EmployeeId")))
                        {
                            trainingProgram.Employees.Add(
                                new Employee()
                                {
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName"))
                                }
                           );
                        }
                    }
                    reader.Close();
                    return trainingProgram;
                }
            }
        }
    }
}
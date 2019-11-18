using System;
using System.Collections.Generic;
using System.Data;
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
    public class ComputersController : Controller
    {
        private readonly IConfiguration _config;

        public ComputersController(IConfiguration config)
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
        // GET: Computers
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {


                    cmd.CommandText = @"
                                                        SELECT c.Id, c.Manufacturer, c.Make, c.PurchaseDate, c.DecomissionDate
                                                        FROM Computer c
                                                        ORDER BY c.PurchaseDate, c.Manufacturer, c.Make";

                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Computer> computers = new List<Computer>();
                    while (reader.Read())
                    {
                        computers.Add(
                            new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                                // The code below checks to see if DecommissionDate is Null. If it is Null, it returns DateTime.MinValue.
                                DecomissionDate = reader.IsDBNull(reader.GetOrdinal("DecomissionDate")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("DecomissionDate"))
                            });
                    }

                    reader.Close();

                    return View(computers);
                }
            }
        }

        // GET: Computers/Details/5
        public ActionResult Details(int id)
        {
            var computer = GetComputerById(id);
            return View(computer);
        }

        // GET: Computers/Create
        public ActionResult Create()
        {

            return View();
        }

        // POST: Computers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ComputerCreateViewModel model)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                                                        INSERT INTO Computer (PurchaseDate, DecomissionDate, Manufacturer, Make)
                                                        VALUES (@PurchaseDate, Null, @Manufacturer, @Make)";
                        cmd.Parameters.Add(new SqlParameter("@PurchaseDate", model.Computer.PurchaseDate));
                        cmd.Parameters.Add(new SqlParameter("@Manufacturer", model.Computer.Manufacturer));
                        cmd.Parameters.Add(new SqlParameter("@Make", model.Computer.Make));
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

        // GET: Computers/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Computers/Edit/5
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

        // GET: Computers/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Computers/Delete/5
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

        private Computer GetComputerById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                                        SELECT c.Id, c.Manufacturer, c.Make, c.PurchaseDate, c.DecomissionDate
                                                        FROM Computer c
                                                        WHERE c.Id = @Id
                                                        ORDER BY c.PurchaseDate, c.Manufacturer, c.Make";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Computer theComputer = null;
                    if (reader.Read())
                    {
                        //if (theComputer == null)
                        {
                            theComputer = new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                                // The code below checks to see if DecommissionDate is Null. If it is Null, it returns DateTime.MinValue.
                                DecomissionDate = reader.IsDBNull(reader.GetOrdinal("DecomissionDate")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("DecomissionDate"))
                            };
                        }
                    }

                    reader.Close();
                    return theComputer;
                }
            }
        }


    }
}
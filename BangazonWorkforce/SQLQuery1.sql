SELECT e.FirstName, 
												e.LastName, 
												  d.Id as DepartmentId,
                                        		  d.Name as DepartmentName, 
												  tp.Id as TrainingProgramId,
                                        		  tp.Name as TrainingProgramName,
                                        		  tp.StartDate as TrainingProgramStartDate,
												  c.Id as ComputerId,
                                                  c.Manufacturer + ' ' + c.Make as Computer
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
                                        	WHERE 3 = e.id
                                         ORDER BY e.LastName, e.FirstName, tp.StartDate
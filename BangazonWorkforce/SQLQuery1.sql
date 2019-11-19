Select c.make + ' ' + c. manufacturer as ComputerName, c.id as ComputerId
                                from Computer c left
                                join ComputerEmployee ce on ce.ComputerId = c.Id left
                                join Employee e on e.Id = ce.EmployeeId
                                where c.DecomissionDate is null and (ce.UnassignDate > ce.AssignDate or ce.AssignDate is null) 
								group by c.Id, c.Make, c.Manufacturer
								--And ce.ComputerId not in (Select ce.ComputerId from ComputerEmployee ce where ce.UnassignDate is null)  
Select c.make, c. manufacturer, c.DecomissionDate, c.PurchaseDate
                                from Computer c left join ComputerEmployee ce on ce.ComputerId = c.Id left join Employee e on e.Id = ce.EmployeeId 
                                where ce.EmployeeId = 22 and DecomissionDate is null
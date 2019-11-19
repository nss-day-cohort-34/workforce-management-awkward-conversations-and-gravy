SELECT c.Id, c.Manufacturer, c.Make
FROM Computer c
LEFT JOIN ComputerEmployee ce ON c.Id = ce.ComputerId
WHERE ce.Id IS NULL 
OR c.Id IN (
        SELECT ce.ComputerId
        FROM ComputerEmployee ce
        WHERE ce.UnassignDate IS NOT NULL and c.DecomisasionDate is null
                       AND ce.ComputerId NOT IN (
                                SELECT ce.ComputerId
                                FROM ComputerEmployee ce
                                WHERE ce.UnassignDate IS NULL)
                                )
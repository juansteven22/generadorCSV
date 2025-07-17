SELECT f.Id, f.Nombre, p.Precio 
FROM Frutas AS f 
INNER JOIN PrecioFrutas AS p ON f.Id = p.IdFruta;


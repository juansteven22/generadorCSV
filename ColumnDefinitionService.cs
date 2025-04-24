using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CSVGeneratorSOLID
{
    public class ColumnDefinitionService
    {
        public List<ColumnDefinition> GetColumnDefinitionsFromUser(IReadOnlyList<TableMetadata> prev)
        {
            var defs = new List<ColumnDefinition>();

            Console.Write("¿Cuántas columnas tendrá esta tabla? ");
            int nCols = int.Parse(Console.ReadLine() ?? "0");

            for (int i = 0; i < nCols; i++)
            {
                Console.WriteLine($"\n— Columna #{i + 1} —");

                // ------ nombre y tipo ------
                Console.Write("Nombre: ");
                string name  = Console.ReadLine() ?? $"Columna{i+1}";
                Console.Write("Tipo (int, string, datetime, bool, decimal): ");
                string type  = Console.ReadLine()?.Trim().ToLower() ?? "string";

                // ------ reciclaje ------
                string? baseTable  = null;
                string? baseColumn = null;
                bool    allowRep   = true; // sólo se pregunta si NO se recicla

                if (prev.Any())
                {
                    Console.Write("¿Basar en tabla existente? (s/n): ");
                    if ((Console.ReadLine() ?? "n").ToLower() == "s")
                    {
                        for (int t = 0; t < prev.Count; t++)
                            Console.WriteLine($"{t + 1}. {prev[t].TableName}");

                        Console.Write("Elegir Nº tabla: ");
                        if (int.TryParse(Console.ReadLine(), out int idxT) &&
                            idxT >= 1 && idxT <= prev.Count)
                        {
                            var tabSel = prev[idxT - 1];
                            var comp   = tabSel.Columns
                                                .Where(c => c.DataType == type)
                                                .ToList();

                            if (comp.Any())
                            {
                                for (int c = 0; c < comp.Count; c++)
                                    Console.WriteLine($"{c + 1}. {comp[c].Name}");

                                Console.Write("Elegir Nº columna: ");
                                if (int.TryParse(Console.ReadLine(), out int idxC) &&
                                    idxC >= 1 && idxC <= comp.Count)
                                {
                                    baseTable  = tabSel.TableName;
                                    baseColumn = comp[idxC - 1].Name;
                                }
                            }
                            else
                            {
                                Console.WriteLine("‑ No existen columnas compatibles.");
                            }
                        }
                    }
                }

                // ------ repetición, sólo si NO se recicla ------
                if (baseTable == null)
                {
                    Console.Write("¿Permitir repeticiones? (s/n): ");
                    allowRep = (Console.ReadLine()?.ToLower() == "s");
                }

                // ------ rangos opcionales ------
                int?      iMin = null, iMax = null;
                decimal?  dMin = null, dMax = null;
                DateTime? dtMin = null, dtMax = null;

                if (type is "int" or "decimal" or "datetime")
                {
                    Console.Write("¿Limitar rango? (s/n): ");
                    if ((Console.ReadLine() ?? "n").ToLower() == "s")
                    {
                        switch (type)
                        {
                            case "int":
                                Console.Write("Cota inferior (int): ");
                                iMin = int.Parse(Console.ReadLine() ?? "0");
                                Console.Write("Cota superior (int): ");
                                iMax = int.Parse(Console.ReadLine() ?? "0");
                                break;

                            case "decimal":
                                Console.Write("Cota inferior (decimal): ");
                                dMin = decimal.Parse(Console.ReadLine() ?? "0",
                                        CultureInfo.InvariantCulture);
                                Console.Write("Cota superior (decimal): ");
                                dMax = decimal.Parse(Console.ReadLine() ?? "0",
                                        CultureInfo.InvariantCulture);
                                break;

                            case "datetime":
                                Console.Write("Fecha inicial (yyyy-MM-dd): ");
                                dtMin = DateTime.Parse(Console.ReadLine() ?? "2000-01-01");
                                Console.Write("Fecha final   (yyyy-MM-dd): ");
                                dtMax = DateTime.Parse(Console.ReadLine() ?? "2050-12-31");
                                break;
                        }
                    }
                }

                defs.Add(new ColumnDefinition
                {
                    Name            = name,  DataType = type,  AllowRepetition = allowRep,
                    BaseTableName   = baseTable,  BaseColumnName = baseColumn,
                    IntMin = iMin, IntMax = iMax,
                    DecMin = dMin, DecMax = dMax,
                    DateMin = dtMin, DateMax = dtMax
                });
            }

            return defs;
        }
    }
}

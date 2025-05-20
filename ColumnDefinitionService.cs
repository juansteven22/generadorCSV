using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
//Код спрашивает пользователя о колонках таблицы.
namespace CSVGenerador
{
    public class ColumnDefinitionService
    {
        public List<ColumnDefinition> GetColumnasDelUsuario(IReadOnlyList<TableMetadata> prev)
        {
            var defs = new List<ColumnDefinition>();

            Console.Write("¿Cuántas columnas tendrá esta tabla? ");
            int nCols = int.Parse(Console.ReadLine() ?? "0");

            for (int i = 0; i < nCols; i++)
            {
                Console.WriteLine($"\n— Columna #{i + 1} —");

                // -------- nombre y tipo --------
                Console.Write("Nombre: ");
                string name = Console.ReadLine() ?? $"Columna{i + 1}";

                Console.Write("Tipo (int, string, datetime, bool, decimal): ");
                string type = Console.ReadLine()?.Trim().ToLower() ?? "string";

                // -------- reciclaje entre tablas --------
                string? baseTable = null;
                string? baseColumn = null;
                bool allowRep = true; // sólo lo preguntamos si no se recicla

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
                            var selected = prev[idxT - 1];
                            var compatibles = selected.Columns
                                                       .Where(c => c.TipoDeDato == type)
                                                       .ToList();

                            if (compatibles.Any())
                            {
                                for (int c = 0; c < compatibles.Count; c++)
                                    Console.WriteLine($"{c + 1}. {compatibles[c].Nombre}");

                                Console.Write("Elegir Nº columna: ");
                                if (int.TryParse(Console.ReadLine(), out int idxC) &&
                                    idxC >= 1 && idxC <= compatibles.Count)
                                {
                                    baseTable = selected.TableName;
                                    baseColumn = compatibles[idxC - 1].Nombre;
                                }
                            }
                            else
                            {
                                Console.WriteLine("- No existen columnas compatibles.");
                            }
                        }
                    }
                }

                // -------- repetición (si no recicla) --------
                if (baseTable == null)
                {
                    Console.Write("¿Permitir repeticiones? (s/n): ");
                    allowRep = (Console.ReadLine()?.ToLower() == "s");
                }

                // -------- rangos opcionales --------
                int? iMin = null, iMax = null;
                decimal? dMin = null, dMax = null;
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

                // -------- NUEVO · importación de strings --------
                bool useImport = false;
                string? importPath = null;
                bool genExtra = false;
                int? totalUnique = null;

                if (type == "string")
                {
                    Console.Write("¿Importar valores desde un CSV externo? (s/n): ");
                    useImport = (Console.ReadLine()?.ToLower() == "s");

                    if (useImport)
                    {
                        Console.Write("Ruta del CSV (una columna, sin cabecera): ");
                        importPath = Console.ReadLine();

                        Console.WriteLine("  1. Usar sólo esos valores (se repetirán si faltan).");
                        Console.WriteLine("  2. Crear valores únicos extra a partir de ellos.");
                        Console.Write("Elige 1 ó 2: ");
                        string modo = Console.ReadLine();
                        if (modo == "2")
                        {
                            genExtra = true;
                            Console.Write("¿Cuántos valores únicos en total deseas? ");
                            totalUnique = int.Parse(Console.ReadLine() ?? "0");
                        }
                    }
                }

                // -------- almacenamos definición --------
                defs.Add(new ColumnDefinition
                {
                    Nombre            = name,
                    TipoDeDato        = type,
                    PermitirRepeticion = allowRep,
                    BaseTableName   = baseTable,
                    BaseColumnName  = baseColumn,
                    IntMin   = iMin, IntMax   = iMax,
                    DecMin   = dMin, DecMax   = dMax,
                    DateMin  = dtMin, DateMax = dtMax,
                    UsarListaImportada     = useImport,
                    ImportFilePath      = importPath,
                    GenerarValoresAdicionalesUnicos = genExtra,
                    TotalValoresUnicosDeseados  = totalUnique
                });
            }

            return defs;
        }
    }
}

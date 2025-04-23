using System;
using System.Collections.Generic;
using System.Linq;

namespace CSVGeneratorSOLID
{
    public class ColumnDefinitionService
    {
        public List<ColumnDefinition> GetColumnDefinitionsFromUser(IReadOnlyList<TableMetadata> tablasPrevias)
        {
            var defs = new List<ColumnDefinition>();

            Console.Write("¿Cuántas columnas tendrá esta tabla? ");
            int columnCount = int.Parse(Console.ReadLine() ?? "0");

            for (int i = 0; i < columnCount; i++)
            {
                Console.WriteLine($"\n— Columna #{i + 1} —");
                Console.Write("Nombre de la columna: ");
                string name = Console.ReadLine() ?? $"Columna{i+1}";

                Console.Write("Tipo (int, string, datetime, bool, decimal): ");
                string type = Console.ReadLine()?.ToLower() ?? "string";

                // ¿Se puede reciclar?
                string? baseTable = null;
                string? baseColumn = null;
                bool allowRepetition = true;

                if (tablasPrevias.Any())
                {
                    Console.Write("¿Quieres basarte en una columna existente? (s/n): ");
                    if ((Console.ReadLine() ?? "n").ToLower() == "s")
                    {
                        // Mostrar tablas disponibles
                        for (int t = 0; t < tablasPrevias.Count; t++)
                            Console.WriteLine($"{t + 1}. {tablasPrevias[t].TableName}");

                        Console.Write("Elige Nº de tabla: ");
                        if (int.TryParse(Console.ReadLine(), out int tablaSel)
                            && tablaSel >= 1 && tablaSel <= tablasPrevias.Count)
                        {
                            var tablaElegida = tablasPrevias[tablaSel - 1];

                            // Columnas compatibles
                            var compatibles = tablaElegida.Columns
                                .Where(c => c.DataType == type)
                                .ToList();

                            if (compatibles.Any())
                            {
                                for (int c = 0; c < compatibles.Count; c++)
                                    Console.WriteLine($"{c + 1}. {compatibles[c].Name}");

                                Console.Write("Elige Nº de columna: ");
                                if (int.TryParse(Console.ReadLine(), out int colSel)
                                    && colSel >= 1 && colSel <= compatibles.Count)
                                {
                                    baseTable = tablaElegida.TableName;
                                    baseColumn = compatibles[colSel - 1].Name;
                                    // No preguntamos por repetición, no aplica al reciclar
                                }
                            }
                            else
                            {
                                Console.WriteLine("‑ No hay columnas compatibles. Se creará desde cero.");
                            }
                        }
                    }
                }

                // Si no recicla, preguntamos repeticiones
                if (baseTable == null)
                {
                    Console.Write("¿Permitir repeticiones? (s/n): ");
                    allowRepetition = (Console.ReadLine()?.ToLower() == "s");
                }

                defs.Add(new ColumnDefinition
                {
                    Name = name,
                    DataType = type,
                    AllowRepetition = allowRepetition,
                    BaseTableName = baseTable,
                    BaseColumnName = baseColumn
                });
            }

            return defs;
        }
    }
}

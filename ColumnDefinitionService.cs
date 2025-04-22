using System;
using System.Collections.Generic;

namespace CSVGeneratorSOLID
{
    public class ColumnDefinitionService
    {
        public List<ColumnDefinition> GetColumnDefinitionsFromUser()
        {
            var columnDefinitions = new List<ColumnDefinition>();

            Console.Write("¿Cuántas columnas deseas crear? ");
            int columnCount = int.Parse(Console.ReadLine() ?? "0");

            for (int i = 0; i < columnCount; i++)
            {
                Console.WriteLine($"\nColumna #{i + 1}");
                Console.Write("Nombre de la columna: ");
                string name = Console.ReadLine() ?? $"Columna{i+1}";

                Console.Write("Tipo de dato (int, string, datetime, bool, decimal): ");
                string dataType = Console.ReadLine()?.ToLower() ?? "string";

                Console.Write("¿Permitir repeticiones? (s/n): ");
                string repInput = Console.ReadLine()?.ToLower() ?? "n";
                bool allowRepetition = (repInput == "s");

                columnDefinitions.Add(new ColumnDefinition
                {
                    Name = name,
                    DataType = dataType,
                    AllowRepetition = allowRepetition
                });
            }

            return columnDefinitions;
        }

        public int GetRecordCountFromUser()
        {
            Console.Write("\n¿Cuántos registros deseas generar? ");
            int recordCount = int.Parse(Console.ReadLine() ?? "0");
            return recordCount;
        }
    }
}

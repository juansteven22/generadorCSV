using System;
using System.Collections.Generic;

namespace CSVGeneratorSOLID
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("=== Generador Múltiple de CSV (SOLID) ===");

            var registro       = new RegistroTablas();
            var tableDefSvc    = new TableDefinitionService();
            var dataGenSvc     = new DataGenerationService();
            var writer         = new CSVWriter();

            Console.Write("¿Cuántas tablas quieres crear? ");
            int totalTablas = int.Parse(Console.ReadLine() ?? "0");

            for (int t = 0; t < totalTablas; t++)
            {
                Console.WriteLine($"\n================ TABLA {t + 1} de {totalTablas} ================");
                var (nombre, columnas, filas) = tableDefSvc.GetTableDefinition(registro.GetAll());

                // Generamos datos
                var rows = dataGenSvc.GenerateData(columnas, filas, registro);

                // Nombre completo de archivo
                string filePath = $"{nombre}.csv";
                writer.WriteToCSV(filePath, columnas, rows);

                // Registramos para uso futuro
                registro.Add(new TableMetadata
                {
                    TableName = nombre,
                    Columns   = columnas,
                    Rows      = rows
                });
            }

            Console.WriteLine("\nProceso finalizado. Pulsa ENTER para cerrar.");
            Console.ReadLine();
        }
    }
}

using System;
using System.Collections.Generic;

namespace CSVGenerador
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("=== Generador de CSV + SQLite ===");

            var registro   = new RegistroTablas();
            var defService = new TableDefinitionService();
            var genService = new DataGenerationService();
            var csvWriter  = new CSVWriter();

            // --- NUEVO: servicio de base de datos ---
            using var db = new DatabaseService("data.db");

            Console.Write("¿Cuántas tablas quieres crear? ");
            int total = int.Parse(Console.ReadLine() ?? "0");

            for (int t = 0; t < total; t++)
            {
                Console.WriteLine($"\n========= TABLA {t + 1} / {total} =========");
                var (name, cols, filas) = defService.GetTableDefinition(registro.GetAll());

                // 1) Generar datos
                var rows = genService.GenerateData(cols, filas, registro);

                // 2) CSV
                string path = $"{name}.csv";
                csvWriter.WriteToCSV(path, cols, rows);

                // 3) SQLite
                db.SaveTable(name, cols, rows);

                // 4) Registrar para tablas siguientes
                registro.Add(new TableMetadata
                {
                    TableName = name,
                    Columns   = cols,
                    Rows      = rows
                });
            }

            Console.WriteLine("\nProceso completo.  Pulsa ENTER para salir.");
            Console.ReadLine();
        }
    }
}

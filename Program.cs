using System;
using System.IO;
using Models;
using Services;
using Microsoft.Data.Sqlite;

namespace CSVGenerador
{
    class Program
    {
        // Carpeta por defecto para los CSV (NO se modifica salvo que el usuario diga lo contrario)
        private const string DEFAULT_OUTPUT_DIR = @"C:\Users\Universidad\Desktop\Trabajo\Proyecto 2 IMPORTANTE\CSVGeneratorSOLID\Data\Out";

        static void Main()
        {
            Console.WriteLine("=== Generador de CSV + SQLite ===\n");

            // -----------------------------------------------------------
            // 1) Ruta de la base de datos (obligatorio y validado)
            // -----------------------------------------------------------
            string dbPath = AskForValidDatabasePath();

            // -----------------------------------------------------------
            // 2) Carpeta de salida de los CSV (con opción a cambiarla)
            // -----------------------------------------------------------
            string outputDir = AskForOutputDirectory();

            // -----------------------------------------------------------
            // 3) Resto de servicios
            // -----------------------------------------------------------
            var registro   = new RegistroTablas();
            var defService = new TableDefinitionService();
            var genService = new DataGenerationService();
            var csvWriter  = new CSVWriter();

            // Servicio de base de datos usando la ruta proporcionada
            using var db = new DatabaseService(dbPath);

            Console.Write("¿Cuántas tablas quieres crear? ");
            int total = int.Parse(Console.ReadLine() ?? "0");

            for (int t = 0; t < total; t++)
            {
                Console.WriteLine($"\n========= TABLA {t + 1} / {total} =========");
                var (name, cols, filas) = defService.GetTableDefinition(registro.GetAll());

                // 1) Generar datos
                var rows = genService.GenerateData(cols, filas, registro);

                // 2) CSV -> combinamos con la carpeta elegida
                string path = Path.Combine(outputDir, $"{name}.csv");
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

        // ===============================================================
        //  Métodos auxiliares privados
        // ===============================================================

        /// <summary>
        /// Solicita repetidamente al usuario una ruta válida para la base de datos
        /// y no devuelve hasta que consigue abrir (o crear) el archivo .db.
        /// </summary>
        private static string AskForValidDatabasePath()
        {
            while (true)
            {
                Console.Write("Ruta completa del archivo SQLite (.db): ");
                string? input = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("La ruta no puede estar vacía.\n");
                    continue;
                }

                try
                {
                    // Intentamos abrir (o crear) la BD para validar la ruta
                    using var conn = new SqliteConnection($"Data Source={input}");
                    conn.Open();
                    conn.Close();
                    return input; // ¡Ruta válida!
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"No se pudo abrir o crear la base de datos: {ex.Message}\n");
                }
            }
        }

        /// <summary>
        /// Pregunta al usuario si quiere cambiar la carpeta de salida de los CSV.
        /// Siempre devuelve una ruta que existe (la crea si es necesario).
        /// </summary>
        private static string AskForOutputDirectory()
        {
            Console.Write($"¿Deseas cambiar la carpeta de salida de los CSV? (s/n) [por defecto: {DEFAULT_OUTPUT_DIR}]: ");
            string? respuesta = Console.ReadLine()?.Trim().ToLower();

            if (respuesta == "s")
            {
                while (true)
                {
                    Console.Write("Introduce la ruta de la carpeta destino: ");
                    string? dir = Console.ReadLine()?.Trim();

                    if (!string.IsNullOrWhiteSpace(dir))
                    {
                        try
                        {
                            Directory.CreateDirectory(dir); // Crea si no existe
                            return dir;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"No se pudo usar esa carpeta: {ex.Message}\n");
                        }
                    }
                }
            }

            // Si no cambia, nos aseguramos que la carpeta por defecto exista
            Directory.CreateDirectory(DEFAULT_OUTPUT_DIR);
            return DEFAULT_OUTPUT_DIR;
        }
    }
}

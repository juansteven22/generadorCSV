using System;
using System.IO;
using System.Collections.Generic;

namespace CSVGeneratorSOLID
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Generador de CSV (SOLID) ===");

            // 1. Instanciamos los servicios
            var columnDefinitionService = new ColumnDefinitionService();
            var dataGenerationService = new DataGenerationService();
            var csvWriter = new CSVWriter();

            // 2. Obtenemos la definición de columnas y la cantidad de registros
            List<ColumnDefinition> columnDefinitions = columnDefinitionService.GetColumnDefinitionsFromUser();
            int recordCount = columnDefinitionService.GetRecordCountFromUser();

            // 3. Generamos los datos
            List<string[]> rows = dataGenerationService.GenerateData(columnDefinitions, recordCount);

            // 4. Preguntamos por la ruta de archivo
            Console.Write("\nIngresa el nombre (o ruta) del archivo CSV de salida: ");
            string filePath = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(filePath))
            {
                filePath = "output.csv";
            }

            // 5. Escribimos el CSV
            csvWriter.WriteToCSV(filePath, columnDefinitions, rows);

            Console.WriteLine("\nProceso finalizado. Presiona ENTER para salir...");
            Console.ReadLine();
        }
    }
}

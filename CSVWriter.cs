using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CSVGeneratorSOLID
{
    public class CSVWriter
    {
        public void WriteToCSV(string filePath, List<ColumnDefinition> columnDefinitions, List<string[]> rows)
        {
            // Creamos el archivo CSV usando un StringBuilder
            StringBuilder sb = new StringBuilder();

            // 1. Escribimos la cabecera
            for (int i = 0; i < columnDefinitions.Count; i++)
            {
                sb.Append(columnDefinitions[i].Name);
                if (i < columnDefinitions.Count - 1)
                    sb.Append(",");
            }
            sb.AppendLine();

            // 2. Escribimos los registros
            foreach (var row in rows)
            {
                for (int i = 0; i < row.Length; i++)
                {
                    // Escapar comas, dobles comillas, etc. si fuera necesario
                    string value = row[i].Replace("\"", "\"\"");
                    if (value.Contains(","))
                    {
                        // En caso de contener coma, ponerlo entre comillas
                        value = $"\"{value}\"";
                    }

                    sb.Append(value);

                    if (i < row.Length - 1)
                        sb.Append(",");
                }
                sb.AppendLine();
            }

            // 3. Guardar en archivo
            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);

            Console.WriteLine($"\nCSV generado en: {filePath}");
        }
    }
}

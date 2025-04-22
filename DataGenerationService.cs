using System;
using System.Collections.Generic;

namespace CSVGeneratorSOLID
{
    public class DataGenerationService
    {
        /// <summary>
        /// Devuelve un listado de filas (cada fila es un array de string) con los datos generados.
        /// </summary>
        /// <param name="columnDefinitions">Las definiciones de cada columna.</param>
        /// <param name="recordCount">Cantidad de registros a generar.</param>
        /// <returns>Lista de filas, donde cada fila es un array de string.</returns>
        public List<string[]> GenerateData(List<ColumnDefinition> columnDefinitions, int recordCount)
        {
            // 1. Crear generadores para cada columna.
            List<IDataGenerator> generators = new List<IDataGenerator>();

            foreach (var colDef in columnDefinitions)
            {
                generators.Add(CreateDataGenerator(colDef));
            }

            // 2. Generar filas
            var rows = new List<string[]>();

            for (int i = 0; i < recordCount; i++)
            {
                string[] row = new string[columnDefinitions.Count];
                for (int j = 0; j < columnDefinitions.Count; j++)
                {
                    // Generamos valor usando el generador correspondiente
                    row[j] = generators[j].GenerateValue(
                        columnDefinitions[j].AllowRepetition,
                        i
                    );
                }
                rows.Add(row);
            }

            return rows;
        }

        private IDataGenerator CreateDataGenerator(ColumnDefinition colDef)
        {
            switch (colDef.DataType)
            {
                case "int":
                    return new IntegerDataGenerator();
                case "string":
                    // Podríamos personalizar la cadena base con el nombre de la columna
                    return new StringDataGenerator(colDef.Name);
                case "datetime":
                    return new DateTimeDataGenerator();
                case "bool":
                    return new BoolDataGenerator();
                case "decimal":
                    return new DecimalDataGenerator();
                default:
                    // Por defecto, usamos string
                    return new StringDataGenerator(colDef.Name);
            }
        }
    }
}

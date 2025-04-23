using System;
using System.Collections.Generic;
using System.Linq;

namespace CSVGeneratorSOLID
{
    public class DataGenerationService
    {
        /// <summary>
        /// Genera todas las filas de la tabla.
        /// Si una columna se basa en otra tabla y esa tabla se queda “corta”,
        /// se generan valores nuevos que no repitan los existentes.
        /// </summary>
        public List<string[]> GenerateData(
            List<ColumnDefinition> colDefs,
            int recordCount,
            RegistroTablas registro)
        {
            // 1) Generador por cada columna (SIEMPRE, aunque la columna recicle)
            var generators = colDefs
                .Select(CreateDataGenerator)
                .ToList();

            // 2) Lista de valores “heredados” por columna (puede ser null)
            var baseLists = colDefs.Select(cd =>
            {
                if (cd.BaseTableName == null) return null;

                var tabla = registro.Find(cd.BaseTableName);
                if (tabla == null) return null;

                int idx = tabla.Columns.FindIndex(c => c.Name == cd.BaseColumnName);
                return idx >= 0 ? tabla.Rows.Select(r => r[idx]).ToList() : null;
            }).ToList();

            // 3) Conjunto de valores usados por columna (para evitar duplicados)
            var usedSets = baseLists
                .Select(bl => bl != null ? new HashSet<string>(bl) : new HashSet<string>())
                .ToList();

            var rows = new List<string[]>(recordCount);

            for (int i = 0; i < recordCount; i++)
            {
                string[] row = new string[colDefs.Count];

                for (int c = 0; c < colDefs.Count; c++)
                {
                    // ¿Todavía hay datos en la lista base para esta fila?
                    if (baseLists[c] != null && i < baseLists[c]!.Count)
                    {
                        row[c] = baseLists[c]![i];
                        // Ya está en el HashSet, no hace falta añadir
                        continue;
                    }

                    // Debemos generar un valor nuevo distinto de los ya usados
                    string nuevo;
                    int   intentos = 0;

                    do
                    {
                        // Para columnas que heredan, queremos que la secuencia
                        // “continue” lógicamente: pasamos el índice global i
                        // (así Id = 1001, 1002, … si la tabla base llegaba a 1000)
                        nuevo = generators[c].GenerateValue(colDefs[c].AllowRepetition, i);
                        intentos++;
                    }
                    // Si allowRepetition == false ó la columna recicla,
                    // NO aceptamos duplicados (salvo que superemos 20 intentos)
                    while (usedSets[c].Contains(nuevo) && intentos < 20);

                    // Después de 20 intentos puede ocurrir que sigamos chocando
                    // (p. ej. tipo bool con sólo 2 posibilidades). En tal caso
                    // aceptamos la repetición para evitar bucle infinito.
                    row[c] = nuevo;
                    usedSets[c].Add(nuevo);
                }

                rows.Add(row);
            }

            return rows;
        }

        // ----------  MÉTODO AUXILIAR  ----------
        private IDataGenerator CreateDataGenerator(ColumnDefinition def) => def.DataType switch
        {
            "int"      => new IntegerDataGenerator(),
            "string"   => new StringDataGenerator(def.Name),
            "datetime" => new DateTimeDataGenerator(),
            "bool"     => new BoolDataGenerator(),
            "decimal"  => new DecimalDataGenerator(),
            _          => new StringDataGenerator(def.Name)
        };
    }
}

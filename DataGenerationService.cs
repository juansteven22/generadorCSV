using System;
using System.Collections.Generic;
using System.Linq;

namespace CSVGeneratorSOLID
{
    public class DataGenerationService
    {
        public List<string[]> GenerateData(
            List<ColumnDefinition> colDefs,
            int recordCount,
            RegistroTablas registro)
        {
            // Generadores para las columnas que NO reciclan
            var generators = colDefs.Select(cd => cd.BaseTableName == null
                                                  ? CreateDataGenerator(cd)
                                                  : null).ToList();

            // Traemos datos base por columna (cuando aplica)
            var bases = colDefs.Select(cd =>
            {
                if (cd.BaseTableName == null) return null;
                var tabla = registro.Find(cd.BaseTableName);
                if (tabla == null) return null;
                int idx = tabla.Columns.FindIndex(c => c.Name == cd.BaseColumnName);
                return idx >= 0 ? tabla.Rows.Select(r => r[idx]).ToList() : null;
            }).ToList();

            var rows = new List<string[]>(recordCount);

            for (int i = 0; i < recordCount; i++)
            {
                var row = new string[colDefs.Count];

                for (int c = 0; c < colDefs.Count; c++)
                {
                    if (bases[c] != null && i < bases[c]!.Count)
                    {
                        // Copiamos del CSV base
                        row[c] = bases[c]![i];
                    }
                    else
                    {
                        // Generamos nuevo valor
                        row[c] = generators[c]!.GenerateValue(colDefs[c].AllowRepetition, i);
                    }
                }

                rows.Add(row);
            }

            return rows;
        }

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

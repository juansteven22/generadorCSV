using System;
using System.Collections.Generic;
using System.Linq;

namespace CSVGeneratorSOLID
{
    public class DataGenerationService
    {
        public List<string[]> GenerateData(
            List<ColumnDefinition> defs,
            int requestedRows,
            RegistroTablas registro)
        {
            // ---------- 1) creamos SIEMPRE generador por columna ----------
            var generators = defs.Select(CreateGenerator).ToList();

            // ---------- 2) listas base copiadas ----------
            var bases = defs.Select(d =>
            {
                if (d.BaseTableName == null) return null;
                var t = registro.Find(d.BaseTableName);
                if (t == null) return null;
                int idx = t.Columns.FindIndex(c => c.Name == d.BaseColumnName);
                return idx >= 0 ? t.Rows.Select(r => r[idx]).ToList() : null;
            }).ToList();

            // ---------- 3) calcular máximo de filas disponibles ----------
            int maxRows = requestedRows;

            for (int c = 0; c < defs.Count; c++)
            {
                if (defs[c].AllowRepetition == false && defs[c].BaseTableName == null)
                {
                    int available = defs[c].DataType switch
                    {
                        "int"      => ((IntegerDataGenerator)generators[c]).RangeSize(),
                        "datetime" => ((DateTimeDataGenerator)generators[c]).RangeSizeDays(),
                        _          => int.MaxValue            // string / decimal: asumimos “muchos”
                    };
                    maxRows = Math.Min(maxRows, available);
                }

                // si la columna recicla sin repetición, sólo podemos usar tantos
                // como tenga la tabla base
                if (defs[c].BaseTableName != null && defs[c].AllowRepetition == false)
                {
                    maxRows = Math.Min(maxRows, bases[c]!.Count);
                }
            }

            if (maxRows < requestedRows)
            {
                Console.WriteLine(
                    $"\n[Aviso] Dado el rango / unicidad de una o más columnas, " +
                    $"solo se podrán generar {maxRows} registros (no {requestedRows}).");
            }

            // ---------- 4) conjuntos de valores usados para evitar duplicados ----------
            var used = bases.Select(b => b != null ? new HashSet<string>(b) : new HashSet<string>())
                            .ToList();

            // ---------- 5) generación de filas ----------
            var rows = new List<string[]>(maxRows);

            for (int i = 0; i < maxRows; i++)
            {
                string[] row = new string[defs.Count];

                for (int c = 0; c < defs.Count; c++)
                {
                    if (bases[c] != null && i < bases[c]!.Count)
                    {
                        row[c] = bases[c]![i];
                        continue;
                    }

                    string val;
                    int attempts = 0;

                    do
                    {
                        val = generators[c].GenerateValue(defs[c].AllowRepetition, i);
                        attempts++;
                    }
                    while (!defs[c].AllowRepetition && used[c].Contains(val) && attempts < 20);

                    used[c].Add(val);
                    row[c] = val;
                }

                rows.Add(row);
            }

            return rows;
        }

        // ------------------------ HELPER ------------------------
        private IDataGenerator CreateGenerator(ColumnDefinition d) => d.DataType switch
        {
            "int"      => new IntegerDataGenerator(d.IntMin,  d.IntMax),
            "decimal"  => new DecimalDataGenerator(d.DecMin,  d.DecMax),
            "datetime" => new DateTimeDataGenerator(d.DateMin,d.DateMax),
            "bool"     => new BoolDataGenerator(),
            "string"   => new StringDataGenerator(d.Name),
            _          => new StringDataGenerator(d.Name)
        };
    }
}



using Models;
using Utils;

namespace Services
{
    public class DataGenerationService
    {
        public List<string[]> GenerateData(
            List<ColumnDefinition> defs,
            int requestedRows,
            RegistroTablas registro)
        {
            // 1) Generador por columna
            var gens = defs.Select(CreateGenerator).ToList();

            // 2) Datos base copiados (reciclaje)
            var bases = defs.Select(d =>
            {
                if (d.BaseTableName == null) return null;
                var t = registro.Find(d.BaseTableName);
                if (t == null) return null;
                int idx = t.Columns.FindIndex(c => c.Nombre == d.BaseColumnName);
                return idx >= 0 ? t.Rows.Select(r => r[idx]).ToList() : null;
            }).ToList();

            // 3) Máximo de filas posibles (unicidad, rangos, importaciones)
            int maxRows = requestedRows;

            for (int c = 0; c < defs.Count; c++)
            {
                int available = int.MaxValue;

                if (defs[c].PermitirRepeticion == false && defs[c].BaseTableName == null)
                {
                    switch (defs[c].TipoDeDato)
                    {
                        case "int":
                            available = ((IntegerDataGenerator)gens[c]).RangeSize();
                            break;
                        case "datetime":
                            available = ((DateTimeDataGenerator)gens[c]).RangeSizeDays();
                            break;
                        case "string":
                            if (gens[c] is ImportedStringGenerator imp)
                                available = imp.UniqueCount();
                            break;
                    }
                    maxRows = Math.Min(maxRows, available);
                }

                if (defs[c].BaseTableName != null && defs[c].PermitirRepeticion == false)
                    maxRows = Math.Min(maxRows, bases[c]!.Count);
            }

            if (maxRows < requestedRows)
                Console.WriteLine($"\n[Aviso] Se generarán {maxRows} registros (no {requestedRows}).");

            // 4) Conjuntos usados p/ evitar duplicados
            var used = bases.Select(b => b != null ? new HashSet<string>(b) : new HashSet<string>())
                            .ToList();

            // 5) Generación final
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
                    int tries = 0;
                    do
                    {
                        val = gens[c].GenerateValue(defs[c].PermitirRepeticion, i);
                        tries++;
                    }
                    while (!defs[c].PermitirRepeticion && used[c].Contains(val) && tries < 20);

                    used[c].Add(val);
                    row[c] = val;
                }

                rows.Add(row);
            }

            return rows;
        }

        // ---------- helper ----------
        private IDataGenerator CreateGenerator(ColumnDefinition d) => d.TipoDeDato switch
        {
            "int"      => new IntegerDataGenerator(d.IntMin,  d.IntMax),
            "decimal"  => new DecimalDataGenerator(d.DecMin,  d.DecMax),
            "datetime" => new DateTimeDataGenerator(d.DateMin,d.DateMax),
            "bool"     => new BoolDataGenerador(),
            "string"   => d.UsarListaImportada
                              ? new ImportedStringGenerator(d)
                              : new StringDataGenerator(d.Nombre),
            _          => new StringDataGenerator(d.Nombre)
        };
    }
}

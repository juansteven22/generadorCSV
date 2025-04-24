using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CSVGeneratorSOLID
{
    /// <summary>
    /// Genera valores string usando una lista importada desde un CSV.
    /// - Si AllowRepetition = false → devuelve la lista en orden, sin repetir.
    /// - Si AllowRepetition = true  → elige valores al azar (puede repetir).
    /// Puede, opcionalmente, crear más valores únicos añadiendo sufijos.
    /// </summary>
    public class ImportedStringGenerator : IDataGenerator
    {
        private readonly List<string> _pool;
        private readonly Random _rnd = new();
        private readonly bool _allowRep;

        public ImportedStringGenerator(ColumnDefinition def)
        {
            _allowRep = def.AllowRepetition;

            // 1) Leemos el CSV (una columna, sin cabecera)
            var originales = File.ReadAllLines(def.ImportFilePath!)
                                 .Select(s => s.Trim())
                                 .Where(s => !string.IsNullOrEmpty(s))
                                 .Distinct()
                                 .ToList();

            _pool = new List<string>(originales);

            // 2) ¿Crear extras?
            if (def.GenerateExtraUnique && def.TotalUniqueDesired.HasValue)
            {
                int objetivo = def.TotalUniqueDesired.Value;
                var hash = new HashSet<string>(_pool);

                while (_pool.Count < objetivo)
                {
                    string baseWord = originales[_rnd.Next(originales.Count)];
                    string candidate = $"{baseWord}{_rnd.Next(1, 1_000_000)}";
                    if (hash.Add(candidate))
                        _pool.Add(candidate);
                }
            }
        }

        public string GenerateValue(bool allowRep, int index)
        {
            if (!_allowRep)           // secuencia sin repetición
                return _pool[index];

            return _pool[_rnd.Next(_pool.Count)];  // con repetición
        }

        public int UniqueCount() => _pool.Count;
    }
}

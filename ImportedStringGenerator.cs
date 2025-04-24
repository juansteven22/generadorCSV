using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CSVGeneratorSOLID
{
    public class ImportedStringGenerator : IDataGenerator
    {
        private readonly List<string> _pool;      // valores únicos disponibles
        private readonly Random       _rnd = new();
        private readonly bool         _allowRep;

        public ImportedStringGenerator(ColumnDefinition def)
        {
            _allowRep = def.AllowRepetition;

            // 1) leer CSV
            var raw = File.ReadAllLines(def.ImportFilePath!)
                          .Select(s => s.Trim())
                          .Where(s => !string.IsNullOrEmpty(s))
                          .Distinct()
                          .ToList();

            _pool = new List<string>(raw);

            // 2) ¿crear valores extra?
            if (def.GenerateExtraUnique && (def.TotalUniqueDesired ?? 0) > _pool.Count)
            {
                int objetivo = def.TotalUniqueDesired!.Value;
                var hash = new HashSet<string>(_pool);

                while (_pool.Count < objetivo)
                {
                    string baseWord = raw[_rnd.Next(raw.Count)];
                    string candidate = $"{baseWord}{_rnd.Next(1, 1000000)}";
                    if (hash.Add(candidate))
                        _pool.Add(candidate);
                }
            }
        }

        // ---- API ----
        public string GenerateValue(bool allowRep, int index)
        {
            if (!_allowRep)        // secuencia única
                return _pool[index];

            // repetición → aleatorio
            return _pool[_rnd.Next(_pool.Count)];
        }

        public int UniqueCount() => _pool.Count;
    }
}

using System.Collections.Generic;
using System.Linq;

namespace CSVGeneratorSOLID
{
    public class RegistroTablas
    {
        private readonly List<TableMetadata> _tablas = new();

        public void Add(TableMetadata table) => _tablas.Add(table);

        public IReadOnlyList<TableMetadata> GetAll() => _tablas;

        public TableMetadata? Find(string name) =>
            _tablas.FirstOrDefault(t => t.TableName.Equals(name, System.StringComparison.OrdinalIgnoreCase));
    }
}

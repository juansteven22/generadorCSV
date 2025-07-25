using System.Collections.Generic;

namespace Models
{
    public class TableMetadata
    {
        public string TableName { get; set; }
        public List<ColumnDefinition> Columns { get; set; }
        public List<string[]> Rows { get; set; }      // Sólo las guardamos en RAM
    }
}

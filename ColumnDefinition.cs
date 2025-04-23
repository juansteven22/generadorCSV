namespace CSVGeneratorSOLID
{
    public class ColumnDefinition
    {
        public string Name { get; set; }           // Ej: "Id"
        public string DataType { get; set; }       // int, string, datetime…
        public bool AllowRepetition { get; set; }  // sólo relevante si no se recicla
        public string? BaseTableName { get; set; } // null = no se recicla
        public string? BaseColumnName { get; set; } // null = no se recicla
    }
}

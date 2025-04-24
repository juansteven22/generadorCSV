namespace CSVGeneratorSOLID
{
    public class ColumnDefinition
    {
        public string  Name            { get; set; }
        public string  DataType        { get; set; }
        public bool    AllowRepetition { get; set; }
        public string? BaseTableName   { get; set; }
        public string? BaseColumnName  { get; set; }

        // Rangos (int, decimal, datetime)  ─ ya existentes
        public int?      IntMin   { get; set; }
        public int?      IntMax   { get; set; }
        public decimal?  DecMin   { get; set; }
        public decimal?  DecMax   { get; set; }
        public DateTime? DateMin  { get; set; }
        public DateTime? DateMax  { get; set; }

        // ------- NUEVO para importación de strings -------
        public bool      UseImportedList      { get; set; }  // true si se importa
        public string?   ImportFilePath       { get; set; }  // ruta del CSV
        public bool      GenerateExtraUnique  { get; set; }  // opción B
        public int?      TotalUniqueDesired   { get; set; }  // sólo si GenerateExtraUnique
    }
}

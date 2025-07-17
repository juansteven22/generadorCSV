
//Класс описывает одну колонку таблицы.
namespace Models
{
    public class ColumnDefinition
    {
        public string  Nombre            { get; set; }
        public string  TipoDeDato        { get; set; }
        public bool    PermitirRepeticion { get; set; }
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
        public bool      UsarListaImportada      { get; set; }  // true si se importa
        public string?   ImportFilePath       { get; set; }  // ruta del CSV
        public bool      GenerarValoresAdicionalesUnicos  { get; set; }  // opción B
        public int?      TotalValoresUnicosDeseados   { get; set; }  // sólo si TotalValoresUnicosDeseados
    }
}

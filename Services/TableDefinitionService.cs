
using Models;

namespace Services
{
    public class TableDefinitionService
    {
        private readonly ColumnDefinitionService _columnService = new();

        public (string tableName, List<ColumnDefinition> columns, int rows)
            GetTableDefinition(IReadOnlyList<TableMetadata> tablasPrevias)
        {
            Console.Write("\nNombre del archivo CSV (sin extensión): ");
            string tableName = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(tableName)) tableName = $"Tabla{DateTime.Now:yyyyMMddHHmmss}";

            // Columnas
            var columns = _columnService.GetColumnasDelUsuario(tablasPrevias);

            // Filas
            Console.Write("¿Cuántas filas deseas generar? (máx 1000000): ");
            int rowCount = int.Parse(Console.ReadLine() ?? "0");
            rowCount = Math.Clamp(rowCount, 1, 1_000_000);

            return (tableName, columns, rowCount);
        }
    }
}

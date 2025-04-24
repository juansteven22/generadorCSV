using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace CSVGeneratorSOLID
{
    /// <summary>
    /// Gestiona la base SQLite.  • Si el archivo no existe se crea.
    /// • Por cada tabla generada: DROP TABLE IF EXISTS → CREATE TABLE → INSERT masivo.
    /// </summary>
    public class DatabaseService : IDisposable
    {
        private readonly string _connectionString;
        private readonly SqliteConnection _conn;

        public DatabaseService(string dbPath = "data.db")
        {
            _connectionString = $"Data Source={dbPath}";
            _conn = new SqliteConnection(_connectionString);
            _conn.Open();
        }

        /// <summary>Guarda (o reemplaza) una tabla completa.</summary>
        public void SaveTable(string tableName,
                              IList<ColumnDefinition> columns,
                              IList<string[]> rows)
        {
            string safeName = Sanitize(tableName);

            using var tx = _conn.BeginTransaction();

            // 1) Eliminar tabla previa
            using (var cmd = _conn.CreateCommand())
            {
                cmd.Transaction = tx;
                cmd.CommandText = $"DROP TABLE IF EXISTS [{safeName}];";
                cmd.ExecuteNonQuery();
            }

            // 2) Crear tabla
            using (var cmd = _conn.CreateCommand())
            {
                cmd.Transaction = tx;
                cmd.CommandText = BuildCreateTableSql(safeName, columns);
                cmd.ExecuteNonQuery();
            }

            // 3) Preparar INSERT
            using (var cmd = _conn.CreateCommand())
            {
                cmd.Transaction = tx;
                cmd.CommandText = BuildInsertSql(safeName, columns.Count);
                for (int i = 0; i < columns.Count; i++)
                    cmd.Parameters.Add(new SqliteParameter($"@p{i}", null));

                // 4) Insertar filas
                foreach (var r in rows)
                {
                    for (int i = 0; i < r.Length; i++)
                        cmd.Parameters[i].Value = ConvertToType(r[i], columns[i].DataType);
                    cmd.ExecuteNonQuery();
                }
            }

            tx.Commit();
            Console.WriteLine($"· Tabla [{safeName}] guardada en SQLite.");
        }

        // ---------- helpers ----------
        private static string Sanitize(string name) =>
            name.Replace("[", "").Replace("]", "").Replace("\"", "").Trim();

        private static string BuildCreateTableSql(string name, IList<ColumnDefinition> cols)
        {
            var sql = $"CREATE TABLE [{name}] (";
            for (int i = 0; i < cols.Count; i++)
            {
                string colName = Sanitize(cols[i].Name);
                string typ = cols[i].DataType switch
                {
                    "int"      => "INTEGER",
                    "decimal"  => "REAL",
                    "datetime" => "TEXT",     // ISO-8601
                    "bool"     => "INTEGER",  // 0 / 1
                    _          => "TEXT"
                };
                sql += $"[{colName}] {typ}";
                if (i < cols.Count - 1) sql += ", ";
            }
            sql += ");";
            return sql;
        }

        private static string BuildInsertSql(string name, int nCols)
        {
            var cols = new string[nCols];
            var pars = new string[nCols];
            for (int i = 0; i < nCols; i++)
            {
                cols[i] = $"[{i}]";
                pars[i] = $"@p{i}";
            }
            return $"INSERT INTO [{name}] VALUES ({string.Join(',', pars)});";
        }

        private static object ConvertToType(string value, string dt)
        {
            return dt switch
            {
                "int"      => int.TryParse(value, out int i) ? i : 0,
                "decimal"  => double.TryParse(value, out double d) ? d : 0.0,
                "datetime" => value,               // ISO string
                "bool"     => (value.ToLower() == "true" || value == "1") ? 1 : 0,
                _          => value
            };
        }

        public void Dispose() => _conn?.Dispose();
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Data.Sqlite;

namespace CSVGenerador
{
    public class DatabaseService : IDisposable
    {
        private readonly SqliteConnection _conn;

        public DatabaseService(string dbPath = "data.db")
        {
            _conn = new SqliteConnection($"Data Source={dbPath}");
            _conn.Open();
        }

        // ---------------------------------------------------------------
        //  PÚBLICO
        // ---------------------------------------------------------------
        public void SaveTable(string tableName,
                              IList<ColumnDefinition> columns,
                              IList<string[]> rows)
        {
            string safe = San(tableName);

            using var tx = _conn.BeginTransaction();

            DropTable(safe, tx);
            CreateTable(safe, columns, tx);
            BulkInsert(safe, columns, rows, tx);

            tx.Commit();
            Console.WriteLine($"· Tabla [{safe}] guardada en SQLite.");
        }

        public void Dispose() => _conn?.Dispose();

        // ---------------------------------------------------------------
        //  PRIVADO
        // ---------------------------------------------------------------
        private static string San(string s) =>
            s.Replace("[", "").Replace("]", "").Replace("\"", "").Trim();

        private void DropTable(string name, SqliteTransaction tx)
        {
            using var cmd = _conn.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = $"DROP TABLE IF EXISTS [{name}];";
            cmd.ExecuteNonQuery();
        }

        private void CreateTable(string name, IList<ColumnDefinition> cols, SqliteTransaction tx)
        {
            var parts = new List<string>();
            foreach (var c in cols)
            {
                string colName = San(c.Nombre);
                string sqlType = c.TipoDeDato switch
                {
                    "int"      => "INTEGER",
                    "decimal"  => "REAL",
                    "datetime" => "DATETIME",
                    "bool"     => "INTEGER",
                    _          => "TEXT"
                };
                parts.Add($"[{colName}] {sqlType}");
            }

            using var cmd = _conn.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = $"CREATE TABLE [{name}] ({string.Join(',', parts)});";
            cmd.ExecuteNonQuery();
        }

        private void BulkInsert(string name,
                                IList<ColumnDefinition> cols,
                                IList<string[]> rows,
                                SqliteTransaction tx)
        {
            //  column list  ->  [Col1],[Col2]...
            var colList = string.Join(',', cols.Select(c => $"[{San(c.Nombre)}]"));
            var parList = string.Join(',', cols.Select((_, i) => $"@p{i}"));
            string sql = $"INSERT INTO [{name}] ({colList}) VALUES ({parList});";

            using var cmd = _conn.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = sql;

            for (int i = 0; i < cols.Count; i++)
                cmd.Parameters.Add(new SqliteParameter($"@p{i}", null));

            foreach (var r in rows)
            {
                for (int i = 0; i < r.Length; i++)
                    cmd.Parameters[i].Value = ToSQLiteType(r[i], cols[i].TipoDeDato);

                cmd.ExecuteNonQuery();
            }
        }

        private static object ToSQLiteType(string val, string dt) => dt switch
        {
            "int"      => int.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture, out var i) ? i : 0,
            "decimal"  => double.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture, out var d) ? d : 0.0,
            "datetime" => DateTime.TryParse(val, out var dtVal) ? dtVal : DateTime.MinValue,
            "bool"     => (val.Equals("true", StringComparison.OrdinalIgnoreCase) || val == "1") ? 1 : 0,
            _          => val
        };
    }
}

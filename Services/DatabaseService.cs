using System;                                      // Yo necesito tipos básicos y la interfaz IDisposable
using System.Collections.Generic;                  // Para listas genéricas
using System.Globalization;                        // Para convertir números/fechas con cultura invariante
using Microsoft.Data.Sqlite;
using Models;                       // Cliente oficial SQLite para .NET

namespace CSVGenerador
{

    /// Servicio que gestiona la conexión a SQLite
    /// y permite guardar una tabla entera de una sola vez.

    public class DatabaseService : IDisposable
    {
        private readonly SqliteConnection _conn;   // Mantengo abierta la conexión mientras la instancia viva

        public DatabaseService(string dbPath = "data.db") // Al crearme, abro (o creo) el archivo .db
        {
            _conn = new SqliteConnection($"Data Source={dbPath}");
            _conn.Open();
        }

        // =========================================================
        //  API PÚBLICA
        // =========================================================

        /// Crea la tabla, inserta las filas y confirma la transacción.

        public void SaveTable(string tableName,
                              IList<ColumnDefinition> columns,
                              IList<string[]> rows)
        {
            string safe = San(tableName);                  // Saneé el nombre por seguridad

            using var tx = _conn.BeginTransaction();       // Empiezo transacción

            DropTable(safe, tx);                           // Tiro la tabla si ya existía
            CreateTable(safe, columns, tx);                // Vuelvo a crearla con su esquema
            BulkInsert(safe, columns, rows, tx);           // Inserto todas las filas

            tx.Commit();                                   // Confirmo cambios
            Console.WriteLine($"· Tabla [{safe}] guardada en SQLite.");
        }

        public void Dispose() => _conn?.Dispose();         // Libero la conexión al finalizar

        // =========================================================
        //  MÉTODOS PRIVADOS
        // =========================================================
        /// Elimino [, ], " y espacios sobrantes.
        private static string San(string s) =>
            s.Replace("[", "").Replace("]", "").Replace("\"", "").Trim();

        /// <summary>Borro la tabla si existe (idempotente).</summary>
        private void DropTable(string name, SqliteTransaction tx)
        {
            using var cmd = _conn.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = $"DROP TABLE IF EXISTS [{name}];";
            cmd.ExecuteNonQuery();
        }

        //Creo la tabla con tipos SQLite equivalentes.
        private void CreateTable(string name, IList<ColumnDefinition> cols, SqliteTransaction tx)
        {
            var parts = new List<string>();

            foreach (var c in cols)
            {
                string colName = San(c.Nombre);            // Nombre seguro
                string sqlType = c.TipoDeDato switch       // Mapear mis tipos a SQLite
                {
                    "int"      => "INTEGER",
                    "decimal"  => "REAL",
                    "datetime" => "DATETIME",
                    "bool"     => "INTEGER",               // 0 / 1
                    _          => "TEXT"
                };
                parts.Add($"[{colName}] {sqlType}");
            }

            using var cmd = _conn.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = $"CREATE TABLE [{name}] ({string.Join(',', parts)});";
            cmd.ExecuteNonQuery();
        }

        //Inserto todas las filas usando parámetros para evitar inyección.
        private void BulkInsert(string name,
                                IList<ColumnDefinition> cols,
                                IList<string[]> rows,
                                SqliteTransaction tx)
        {
            // Lista de columnas: [Col1],[Col2],...
            var colList = string.Join(',', cols.Select(c => $"[{San(c.Nombre)}]"));
            // Lista de parámetros: @p0,@p1,...
            var parList = string.Join(',', cols.Select((_, i) => $"@p{i}"));
            string sql  = $"INSERT INTO [{name}] ({colList}) VALUES ({parList});";

            using var cmd = _conn.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = sql;

            // Creo los parámetros una sola vez
            for (int i = 0; i < cols.Count; i++)
                cmd.Parameters.Add(new SqliteParameter($"@p{i}", null));

            // Recorro cada fila y asigno valores convertidos
            foreach (var r in rows)
            {
                for (int i = 0; i < r.Length; i++)
                    cmd.Parameters[i].Value = ToSQLiteType(r[i], cols[i].TipoDeDato);

                cmd.ExecuteNonQuery();
            }
        }

        //
        // Convierto cada string al tipo que SQLite espera,
        // devolviendo un objeto compatible con ADO.NET.
        //
        private static object ToSQLiteType(string val, string dt) => dt switch
        {
            "int"      => int.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture, out var i) ? i : 0,
            "decimal"  => double.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture, out var d) ? d : 0.0,
            "datetime" => DateTime.TryParse(val, out var dtVal) ? dtVal : DateTime.MinValue,
            "bool"     => (val.Equals("true", StringComparison.OrdinalIgnoreCase) || val == "1") ? 1 : 0,
            _          => val                                         // Texto tal cual
        };
    }
}

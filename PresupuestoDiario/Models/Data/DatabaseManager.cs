using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace PresupuestoDiario.Data
{
    public class DatabaseManager
    {
        private readonly string _connectionString;

        public DatabaseManager()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string dataDirectory = Path.Combine(appDataPath, "PresupuestoDiario");
            Directory.CreateDirectory(dataDirectory);

            string dbPath = Path.Combine(dataDirectory, "presupuesto.db");
            _connectionString = $"Data Source={dbPath}";
            InicializarTabla();
        }

        public SqliteConnection CrearConexion()
        {
            return new SqliteConnection(_connectionString);
        }

        private void InicializarTabla()
        {
            using var conexion = CrearConexion();
            conexion.Open();

            string sql = @"
                CREATE TABLE IF NOT EXISTS Movimientos (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Tipo TEXT NOT NULL,
                    Concepto TEXT NOT NULL,
                    Categoria TEXT NOT NULL,
                    DetalleOrigen TEXT NOT NULL,
                    Fecha TEXT NOT NULL,
                    Monto REAL NOT NULL
                );";

            using var cmd = new SqliteCommand(sql, conexion);
            cmd.ExecuteNonQuery();
        }
    }
}
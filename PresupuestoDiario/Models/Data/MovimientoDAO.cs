using Microsoft.Data.Sqlite;
using PresupuestoDiario.Models;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace PresupuestoDiario.Data
{
    public class MovimientoDAO
    {
        private readonly DatabaseManager _db;

        public MovimientoDAO()
        {
            _db = new DatabaseManager();
        }

        // CREATE
        public void Insertar(MovimientoFinanciero mov, string detalleOrigen)
        {
            using var conexion = _db.CrearConexion();
            conexion.Open();

            string sql = @"INSERT INTO Movimientos (Tipo, Concepto, Categoria, DetalleOrigen, Fecha, Monto)
                           VALUES (@Tipo, @Concepto, @Categoria, @DetalleOrigen, @Fecha, @Monto);";

            using var cmd = new SqliteCommand(sql, conexion);
            cmd.Parameters.AddWithValue("@Tipo", mov.Tipo);
            cmd.Parameters.AddWithValue("@Concepto", mov.Concepto);
            cmd.Parameters.AddWithValue("@Categoria", mov.Categoria);
            cmd.Parameters.AddWithValue("@DetalleOrigen", detalleOrigen);
            cmd.Parameters.AddWithValue("@Fecha", mov.Fecha);
            cmd.Parameters.AddWithValue("@Monto", mov.Monto);
            cmd.ExecuteNonQuery();
        }

        // READ
        public List<MovimientoFinanciero> ObtenerTodos()
        {
            var lista = new List<MovimientoFinanciero>();

            using var conexion = _db.CrearConexion();
            conexion.Open();

            string sql = "SELECT Id, Tipo, Concepto, Categoria, DetalleOrigen, Fecha, Monto FROM Movimientos ORDER BY Id DESC;";
            using var cmd = new SqliteCommand(sql, conexion);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string tipo = reader.GetString(1);
                decimal monto = reader.GetDecimal(6);

                MovimientoFinanciero mov;
                if (tipo == "Ingreso")
                {
                    mov = new Ingreso
                    {
                        Fuente = reader.GetString(4)
                    };
                }
                else
                {
                    mov = new Gasto
                    {
                        MedioPago = reader.GetString(4)
                    };
                }

                mov.Id = reader.GetInt32(0);
                mov.Concepto = reader.GetString(2);
                mov.Categoria = reader.GetString(3);
                mov.Fecha = reader.GetString(5);
                mov.Monto = monto;

                lista.Add(mov);
            }

            return lista;
        }

        // UPDATE
        public void Actualizar(MovimientoFinanciero mov, string detalleOrigen)
        {
            using var conexion = _db.CrearConexion();
            conexion.Open();

            string sql = @"UPDATE Movimientos
                           SET Tipo = @Tipo,
                               Concepto = @Concepto,
                               Categoria = @Categoria,
                               DetalleOrigen = @DetalleOrigen,
                               Fecha = @Fecha,
                               Monto = @Monto
                           WHERE Id = @Id;";

            using var cmd = new SqliteCommand(sql, conexion);
            cmd.Parameters.AddWithValue("@Id", mov.Id);
            cmd.Parameters.AddWithValue("@Tipo", mov.Tipo);
            cmd.Parameters.AddWithValue("@Concepto", mov.Concepto);
            cmd.Parameters.AddWithValue("@Categoria", mov.Categoria);
            cmd.Parameters.AddWithValue("@DetalleOrigen", detalleOrigen);
            cmd.Parameters.AddWithValue("@Fecha", mov.Fecha);
            cmd.Parameters.AddWithValue("@Monto", mov.Monto);
            if (cmd.ExecuteNonQuery() == 0)
                throw new InvalidOperationException("No se encontró el movimiento que se intentó actualizar.");
        }

        // DELETE
        public void Eliminar(int id)
        {
            using var conexion = _db.CrearConexion();
            conexion.Open();

            string sql = "DELETE FROM Movimientos WHERE Id = @Id;";
            using var cmd = new SqliteCommand(sql, conexion);
            cmd.Parameters.AddWithValue("@Id", id);
            if (cmd.ExecuteNonQuery() == 0)
                throw new InvalidOperationException("No se encontró el movimiento que se intentó eliminar.");
        }
    }
}
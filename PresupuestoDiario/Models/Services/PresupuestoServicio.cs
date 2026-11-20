using PresupuestoDiario.Data;
using PresupuestoDiario.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace PresupuestoDiario.Services
{
    public record ResumenFinanciero(decimal TotalIngresos, decimal TotalGastos, decimal SaldoDisponible);

    public class PresupuestoServicio
    {
        private readonly MovimientoDAO _dao;

        public PresupuestoServicio()
        {
            _dao = new MovimientoDAO();
        }

        private (MovimientoFinanciero mov, string detalle) ValidarYConstruir(
            int id, string tipo, string concepto, string categoria, string detalle, string fecha, string montoTexto)
        {
            if (string.IsNullOrWhiteSpace(concepto))
                throw new ArgumentException("El concepto no puede estar vacío.");

            if (string.IsNullOrWhiteSpace(categoria))
                throw new ArgumentException("Debes indicar una categoría.");

            if (string.IsNullOrWhiteSpace(detalle))
                throw new ArgumentException(tipo == "Ingreso"
                    ? "Indica la fuente del ingreso (ej. Mesada, Freelance)."
                    : "Indica el medio de pago (ej. Efectivo, Nequi, Tarjeta).");

            if (string.IsNullOrWhiteSpace(fecha))
                throw new ArgumentException("La fecha no puede estar vacía.");

            if (!DateOnly.TryParseExact(fecha.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out DateOnly fechaValidada))
                throw new ArgumentException("La fecha debe ser válida y tener el formato AAAA-MM-DD.");

            string normalizado = montoTexto.Replace(',', '.').Trim();
            if (!decimal.TryParse(normalizado, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal monto) || monto <= 0)
                throw new ArgumentException("El monto debe ser un número válido mayor que 0.");

            MovimientoFinanciero mov = tipo switch
            {
                "Ingreso" => new Ingreso { Fuente = detalle.Trim() },
                "Gasto" => new Gasto { MedioPago = detalle.Trim() },
                _ => throw new ArgumentException("Selecciona si el movimiento es un ingreso o un gasto.")
            };

            mov.Id = id;
            mov.Concepto = concepto.Trim();
            mov.Categoria = categoria.Trim();
            mov.Fecha = fechaValidada.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            mov.Monto = monto; // Dispara validación de encapsulamiento

            return (mov, detalle.Trim());
        }

        public void Registrar(string tipo, string concepto, string categoria, string detalle, string fecha, string montoTexto)
        {
            var (mov, det) = ValidarYConstruir(0, tipo, concepto, categoria, detalle, fecha, montoTexto);
            _dao.Insertar(mov, det);
        }

        public List<MovimientoFinanciero> Listar() => _dao.ObtenerTodos();

        public void Modificar(int id, string tipo, string concepto, string categoria, string detalle, string fecha, string montoTexto)
        {
            if (id <= 0)
                throw new ArgumentException("Selecciona un movimiento de la lista para actualizar.");

            var (mov, det) = ValidarYConstruir(id, tipo, concepto, categoria, detalle, fecha, montoTexto);
            _dao.Actualizar(mov, det);
        }

        public void Eliminar(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Selecciona un movimiento de la lista para eliminar.");

            _dao.Eliminar(id);
        }

        public ResumenFinanciero CalcularResumen(List<MovimientoFinanciero> movimientos)
        {
            decimal ingresos = movimientos.OfType<Ingreso>().Sum(m => m.Monto);
            decimal gastos = movimientos.OfType<Gasto>().Sum(m => m.Monto);
            decimal saldo = movimientos.Sum(m => m.ImpactoEnSaldo()); // Polimorfismo en acción
            return new ResumenFinanciero(ingresos, gastos, saldo);
        }
    }
}
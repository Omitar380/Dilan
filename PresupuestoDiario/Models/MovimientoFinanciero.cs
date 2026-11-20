using System;

namespace PresupuestoDiario.Models
{
    public abstract class MovimientoFinanciero
    {
        private decimal _monto;

        public int Id { get; set; }
        public string Concepto { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Fecha { get; set; } = DateTime.Today.ToString("yyyy-MM-dd");

        // Encapsulamiento: impide montos menores o iguales a cero
        public decimal Monto
        {
            get => _monto;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("El monto debe ser mayor que cero.");
                _monto = value;
            }
        }

        public abstract string Tipo { get; }

        // Efecto neto sobre el saldo: positivo para ingreso, negativo para gasto
        public abstract decimal ImpactoEnSaldo();

        public abstract string DescribirMovimiento();
    }
}
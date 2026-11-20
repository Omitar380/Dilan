namespace PresupuestoDiario.Models
{
    public class Gasto : MovimientoFinanciero
    {
        public string MedioPago { get; set; } = string.Empty;

        public override string Tipo => "Gasto";

        public override decimal ImpactoEnSaldo() => -Monto;

        public override string DescribirMovimiento()
        {
            return $"[-Gasto] {Concepto} ({Categoria} - {MedioPago}) | {Fecha} | ${Monto:N0}";
        }
    }
}
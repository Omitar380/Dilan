namespace PresupuestoDiario.Models
{
    public class Ingreso : MovimientoFinanciero
    {
        public string Fuente { get; set; } = string.Empty;

        public override string Tipo => "Ingreso";

        public override decimal ImpactoEnSaldo() => Monto;

        public override string DescribirMovimiento()
        {
            return $"[+Ingreso] {Concepto} ({Categoria} - {Fuente}) | {Fecha} | ${Monto:N0}";
        }
    }
}
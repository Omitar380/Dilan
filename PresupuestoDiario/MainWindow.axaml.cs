using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using PresupuestoDiario.Models;
using PresupuestoDiario.Services;
using Microsoft.Data.Sqlite;
using System;

namespace PresupuestoDiario
{
    public partial class MainWindow : Window
    {
        private readonly PresupuestoServicio _servicio;
        private int _idSeleccionado = 0;

        public MainWindow()
        {
            InitializeComponent();
            _servicio = new PresupuestoServicio();
            TxtFecha.Text = DateTime.Today.ToString("yyyy-MM-dd");
            RefrescarVista();
        }

        private void RefrescarVista()
        {
            var lista = _servicio.Listar();
            LstMovimientos.ItemsSource = lista;

            var resumen = _servicio.CalcularResumen(lista);
            TxtTotalIngresos.Text = $"${resumen.TotalIngresos:N2}";
            TxtTotalGastos.Text = $"${resumen.TotalGastos:N2}";
            TxtSaldo.Text = $"${resumen.SaldoDisponible:N2}";
            TxtSaldo.Foreground = resumen.SaldoDisponible < 0 ? Brushes.Crimson : Brushes.DodgerBlue;
        }

        private string ObtenerComboTexto(ComboBox combo)
        {
            return (combo.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty;
        }

        private void MostrarMensaje(string texto, bool esError)
        {
            TxtMensaje.Text = texto;
            TxtMensaje.Foreground = esError ? Brushes.Crimson : Brushes.ForestGreen;
        }

        private void EjecutarOperacion(Action operacion)
        {
            try
            {
                operacion();
            }
            catch (ArgumentException ex)
            {
                MostrarMensaje($"Error: {ex.Message}", true);
            }
            catch (InvalidOperationException ex)
            {
                MostrarMensaje($"Error: {ex.Message}", true);
            }
            catch (SqliteException ex)
            {
                MostrarMensaje($"No se pudo completar la operación en la base de datos: {ex.Message}", true);
            }
        }

        private void LimpiarCampos()
        {
            _idSeleccionado = 0;
            CmbTipo.SelectedIndex = 1;
            TxtConcepto.Text = string.Empty;
            TxtCategoria.Text = string.Empty;
            TxtDetalle.Text = string.Empty;
            TxtFecha.Text = DateTime.Today.ToString("yyyy-MM-dd");
            TxtMonto.Text = string.Empty;
            LstMovimientos.SelectedItem = null;
        }

        private void CmbTipo_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (LblDetalle == null || TxtDetalle == null) return;

            if (ObtenerComboTexto(CmbTipo) == "Ingreso")
            {
                LblDetalle.Text = "Fuente del ingreso:";
                TxtDetalle.PlaceholderText = "Ej. Auxilio / Freelance";
            }
            else
            {
                LblDetalle.Text = "Medio de pago:";
                TxtDetalle.PlaceholderText = "Ej. Efectivo / Nequi";
            }
        }

        private void BtnGuardar_Click(object? sender, RoutedEventArgs e)
        {
            EjecutarOperacion(() =>
            {
                _servicio.Registrar(
                    ObtenerComboTexto(CmbTipo),
                    TxtConcepto.Text ?? "",
                    TxtCategoria.Text ?? "",
                    TxtDetalle.Text ?? "",
                    TxtFecha.Text ?? "",
                    TxtMonto.Text ?? ""
                );

                LimpiarCampos();
                RefrescarVista();
                MostrarMensaje("Movimiento guardado en SQLite.", false);
            });
        }

        private void BtnActualizar_Click(object? sender, RoutedEventArgs e)
        {
            EjecutarOperacion(() =>
            {
                _servicio.Modificar(
                    _idSeleccionado,
                    ObtenerComboTexto(CmbTipo),
                    TxtConcepto.Text ?? "",
                    TxtCategoria.Text ?? "",
                    TxtDetalle.Text ?? "",
                    TxtFecha.Text ?? "",
                    TxtMonto.Text ?? ""
                );

                LimpiarCampos();
                RefrescarVista();
                MostrarMensaje("Movimiento actualizado correctamente.", false);
            });
        }

        private void BtnEliminar_Click(object? sender, RoutedEventArgs e)
        {
            EjecutarOperacion(() =>
            {
                _servicio.Eliminar(_idSeleccionado);
                LimpiarCampos();
                RefrescarVista();
                MostrarMensaje("Movimiento eliminado.", false);
            });
        }

        private void BtnLimpiar_Click(object? sender, RoutedEventArgs e)
        {
            LimpiarCampos();
            TxtMensaje.Text = string.Empty;
        }

        private void LstMovimientos_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (LstMovimientos.SelectedItem is MovimientoFinanciero mov)
            {
                _idSeleccionado = mov.Id;
                TxtConcepto.Text = mov.Concepto;
                TxtCategoria.Text = mov.Categoria;
                TxtFecha.Text = mov.Fecha;
                TxtMonto.Text = mov.Monto.ToString(System.Globalization.CultureInfo.InvariantCulture);

                if (mov is Ingreso ing)
                {
                    CmbTipo.SelectedIndex = 0;
                    TxtDetalle.Text = ing.Fuente;
                }
                else if (mov is Gasto gas)
                {
                    CmbTipo.SelectedIndex = 1;
                    TxtDetalle.Text = gas.MedioPago;
                }
            }
        }
    }
}
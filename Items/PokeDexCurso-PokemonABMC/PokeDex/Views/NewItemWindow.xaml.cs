using System;
using System.Windows;
using DAL.Adapters;
using DAL.Models;

namespace PokeDex.Views
{
    public partial class NewItemWindow : Window
    {
        private readonly ItemAdapterApi _itemAdapter;

        public Items CreatedItem { get; private set; }

        public NewItemWindow()
        {
            InitializeComponent();
            _itemAdapter = new ItemAdapterApi();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validar nombre
                if (string.IsNullOrWhiteSpace(NameTextBox.Text))
                {
                    MessageBox.Show("El nombre del item no puede estar vacío.", "Validación",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Validar efecto
                if (string.IsNullOrWhiteSpace(EffectTextBox.Text))
                {
                    MessageBox.Show("El efecto del item no puede estar vacío.", "Validación",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Validar StockMaximo
                if (!int.TryParse(MaxStockTextBox.Text, out int stock) || stock < 0)
                {
                    MessageBox.Show("El Stock máximo debe ser un número entero positivo.", "Validación",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Validar nombre único
                var allItems = await _itemAdapter.ObtenerItemsCompleto();
                if (allItems.Exists(i => i.NombreItem.Equals(NameTextBox.Text, StringComparison.OrdinalIgnoreCase)))
                {
                    MessageBox.Show("Ya existe un item con ese nombre.", "Validación",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Crear objeto
                var nuevoItem = new Items
                {
                    NombreItem = NameTextBox.Text,
                    StockMaximo = stock,
                    Efecto = EffectTextBox.Text,
                    ItemActivo = true,
                    FechaCreacion = DateTime.Now
                };

                var creado = await _itemAdapter.CargarItem(nuevoItem);

                if (creado != null)
                {
                    MessageBox.Show("Item creado correctamente ✅", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    CreatedItem = creado;
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("No se pudo crear el item ❌", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al crear el item: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}

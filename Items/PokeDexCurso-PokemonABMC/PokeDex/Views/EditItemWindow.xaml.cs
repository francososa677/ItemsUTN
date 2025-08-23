using System;
using System.Windows;
using DAL.Adapters;
using DAL.Models;

namespace PokeDex.Views
{
    public partial class EditItemWindow : Window
    {
        private readonly ItemAdapterApi _itemAdapter;
        private Items CurrentItem;

        public EditItemWindow(Items item)
        {
            InitializeComponent();
            _itemAdapter = new ItemAdapterApi();
            CurrentItem = item;

            if (CurrentItem != null)
                LoadItemData();
        }

        private void LoadItemData()
        {
            NameTextBox.Text = CurrentItem.NombreItem;
            MaxStockTextBox.Text = CurrentItem.StockMaximo.ToString();
            EffectTextBox.Text = CurrentItem.Efecto;
            CreationDateTextBox.Text = CurrentItem.FechaCreacion.ToString("dd/MM/yyyy");
            CreationDateTextBox.IsReadOnly = true;
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

                // Validar nombre único (excluyendo el item actual)
                var allItems = await _itemAdapter.ObtenerItemsCompleto();
                if (allItems.Exists(i => i.NombreItem.Equals(NameTextBox.Text, StringComparison.OrdinalIgnoreCase)
                                         && i.Id != CurrentItem.Id))
                {
                    MessageBox.Show("Ya existe un item con ese nombre.", "Validación",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                CurrentItem.NombreItem = NameTextBox.Text;
                CurrentItem.StockMaximo = stock;
                CurrentItem.Efecto = EffectTextBox.Text;

                var actualizado = await _itemAdapter.ModificarItem(CurrentItem);

                if (actualizado != null)
                {
                    MessageBox.Show("Item modificado correctamente ✅", "Éxito",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("No se pudo modificar el item ❌", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al modificar item: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}

using System;
using System.Windows;
using DAL.Adapters;
using DAL.Models;

namespace PokeDex.Views
{
    public partial class ItemWindow : Window
    {
        private readonly ItemAdapterApi _itemAdapter;
        private Items CurrentItem;

        public ItemWindow(Items item)
        {
            InitializeComponent();
            _itemAdapter = new ItemAdapterApi();
            CurrentItem = item;

            if (CurrentItem != null)
                LoadItemDetails();
        }

        /// <summary>
        /// Carga los datos del item en la UI
        /// </summary>
        private void LoadItemDetails()
        {
            ItemNameLabel.Text = CurrentItem.NombreItem;

            ItemDescriptionText.Text = $"Stock máximo: {CurrentItem.StockMaximo}\n" +
                                       $"Efecto: {CurrentItem.Efecto}\n" +
                                       $"Fecha de creación: {CurrentItem.FechaCreacion:dd/MM/yyyy}";
        }

        /// <summary>
        /// Abrir ventana de edición
        /// </summary>
        private void ModifyButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentItem != null)
            {
                EditItemWindow editWindow = new EditItemWindow(CurrentItem);
                bool? result = editWindow.ShowDialog();

                if (result == true)
                {
                    // Refrescar los datos del item
                    LoadItemDetails();
                    this.DialogResult = true; // Avisar al ItemControl que hubo un cambio
                }
            }
        }

        /// <summary>
        /// Soft delete: marcar item como inactivo
        /// </summary>
        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentItem == null) return;

            MessageBoxResult result = MessageBox.Show(
                "¿Está seguro que desea eliminar este item?",
                "Confirmar",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    CurrentItem.ItemActivo = false;
                    var actualizado = await _itemAdapter.ModificarItem(CurrentItem);

                    if (actualizado != null)
                    {
                        MessageBox.Show("Item eliminado correctamente ✅", "Éxito",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        this.DialogResult = true; // Indica al ItemControl que debe refrescar
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("No se pudo eliminar el item ❌", "Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al eliminar item: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

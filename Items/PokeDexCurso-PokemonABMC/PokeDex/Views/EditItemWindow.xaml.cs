using PokeDex.Entities;
using System.Windows;

namespace PokeDex.Views
{
    public partial class EditItemWindow : Window
    {
        private Item CurrentItem;

        // Constructor que recibe el item a editar
        public EditItemWindow(Item item)
        {
            InitializeComponent();
            CurrentItem = item;

            if (CurrentItem != null)
                LoadItemData();
        }

        // Carga los datos del item en los controles
        private void LoadItemData()
        {
            // TODO: asignar los valores reales de CurrentItem a los controles de la ventana
            // Por ejemplo:
            // NameTextBox.Text = CurrentItem.Nombre;
            // MaxStockTextBox.Text = CurrentItem.StockMaximo.ToString();
            // EffectTextBox.Text = CurrentItem.Efecto;
            // CreationDateTextBox.Text = CurrentItem.FechaCreacion.ToShortDateString();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: implementar guardado del item editado en la base de datos
            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

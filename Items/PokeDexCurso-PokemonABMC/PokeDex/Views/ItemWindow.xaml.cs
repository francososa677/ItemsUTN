using PokeDex.Entities;
using PokeDex.Logs;
using System.Windows;

namespace PokeDex.Views
{
    /// <summary>
    /// Lógica de interacción para ItemWindow.xaml
    /// </summary>
    public partial class ItemWindow : Window
    {
        private Item CurrentItem;

        /// <summary>
        /// Constructor. Si item es null, se creará un nuevo item.
        /// </summary>
        public ItemWindow(Item item)
        {
            InitializeComponent();
            CurrentItem = item;

            if (CurrentItem != null)
                LoadItemDetails();
            else
                PrepareForNewItem();
        }

        /// <summary>
        /// Carga los datos del item en la ventana.
        /// </summary>
        private void LoadItemDetails()
        {
            // TODO: asignar valores reales desde CurrentItem
            ItemNameLabel.Content = CurrentItem.Nombre;
            ItemDescriptionText.Text = CurrentItem.Descripcion;
        }

        /// <summary>
        /// Prepara la ventana para crear un nuevo item.
        /// </summary>
        private void PrepareForNewItem()
        {
            ItemNameLabel.Content = "Nuevo Item";
            ItemDescriptionText.Text = "";
            ModifyButton.Content = "Guardar";
            DeleteButton.Visibility = Visibility.Collapsed; // No se puede eliminar un item nuevo
        }

        private void ModifyButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentItem != null)
            {
                // Abrir ventana de edición
                EditItemWindow editWindow = new EditItemWindow(CurrentItem);
                bool? result = editWindow.ShowDialog();

                if (result == true)
                {
                    // Opcional: actualizar la información en ItemWindow después de editar
                    LoadItemDetails();
                }
            }
            else
            {
                // Aquí iría la lógica para guardar un nuevo item si CurrentItem es null
                this.DialogResult = true;
                this.Close();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: eliminar item de la base de datos
            MessageBoxResult result = MessageBox.Show("¿Está seguro que desea eliminar este item?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                // Eliminar CurrentItem de la DB
                this.DialogResult = true;
                this.Close();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

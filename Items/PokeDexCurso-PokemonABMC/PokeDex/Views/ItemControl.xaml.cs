using PokeDex.Entities;
using PokeDex.Logs;
using System.Windows;
using System.Windows.Controls;

namespace PokeDex.Views
{
    /// <summary>
    /// Lógica de interacción para ItemControl.xaml
    /// </summary>
    public partial class ItemControl : UserControl
    {
        public ItemControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Evento que se dispara al cargar el UserControl.
        /// Aquí se deberían cargar los items desde la base de datos.
        /// </summary>
        private void UserItems_Loaded(object sender, RoutedEventArgs e)
        {
            // TODO: Cargar los items desde la base de datos
            // Ejemplo:
            // List<Item> allItems = Item.GetAll();
            // foreach(Item item in allItems)
            // {
            //     Button btn = new Button()
            //     {
            //         Content = item.Nombre,
            //         Width = 200,
            //         Height = 100,
            //         Margin = new Thickness(10),
            //         Tag = item
            //     };
            //     btn.Click += ItemButton_Click;
            //     MainWrap.Children.Add(btn);
            // }
        }

        /// <summary>
        /// Evento de click para cada botón de item.
        /// Abre la ventana de detalle del item.
        /// </summary>
        private void ItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Item item)
            {
                ItemWindow window = new ItemWindow(item);
                window.ShowDialog();

                // TODO: refrescar el MainWrap si se modificó o eliminó el item
            }
        }

        /// <summary>
        /// Evento del botón "Agregar Item".
        /// Abre una ventana para crear un nuevo item.
        /// </summary>
        private void NewItemButton_Click(object sender, RoutedEventArgs e)
        {
            NewItemWindow window = new NewItemWindow();
            window.ShowDialog();

            // TODO: refrescar MainWrap con el nuevo item si se creó
        }
    }
}

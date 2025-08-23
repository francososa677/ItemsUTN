using DAL.Adapters;
using DAL.Models;
using PokeDex.Entities;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PokeDex.Views
{
    public partial class ItemControl : UserControl
    {
        private readonly ItemAdapterApi _itemAdapter;

        public ItemControl()
        {
            InitializeComponent();
            _itemAdapter = new ItemAdapterApi();
        }

        /// <summary>
        /// Carga los items activos en el WrapPanel
        /// </summary>
        private async void UserItems_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadActiveItems();
        }

        /// <summary>
        /// Método que carga solo los items activos
        /// </summary>
        private async System.Threading.Tasks.Task LoadActiveItems()
        {
            try
            {
                var allItems = await _itemAdapter.ObtenerItemsCompleto();

                // Ordenar alfabéticamente
                var activeItems = allItems
                    .Where(i => i.ItemActivo)
                    .OrderBy(i => i.NombreItem)
                    .ToList();

                MainWrap.Children.Clear();

                foreach (var item in activeItems)
                {
                    Button btn = new Button()
                    {
                        Width = 150,
                        Height = 150, // cuadrado
                        Margin = new Thickness(10),
                        Tag = item,
                        FontSize = 20,
                        FontFamily = new FontFamily("Courier New"),
                        FontWeight = FontWeights.SemiBold,
                        Foreground = Brushes.Black,               // texto oscuro
                        Background = Brushes.White,               // fondo blanco
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        BorderBrush = Brushes.Gray,               // borde gris
                        BorderThickness = new Thickness(2),
                        Content = new StackPanel
                        {
                            Orientation = Orientation.Vertical,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            Children =
                            {
                                new TextBlock
                                {
                                    Text = item.NombreItem,
                                    FontSize = 16,
                                    FontWeight = FontWeights.SemiBold,
                                    Foreground = Brushes.Black,
                                    TextAlignment = TextAlignment.Center,
                                    TextWrapping = TextWrapping.Wrap
                                },
                                new TextBlock
                                {
                                    Text = $"Stock: {item.StockMaximo}",
                                    FontSize = 14,
                                    Foreground = Brushes.DarkGray,
                                    TextAlignment = TextAlignment.Center
                                }
                            }
                        }
                    };

                    btn.Click += ItemButton_Click;
                    MainWrap.Children.Add(btn);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error al cargar items: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Evento click en un botón de item
        /// </summary>
        private async void ItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Items item)
            {
                ItemWindow window = new ItemWindow(item);
                bool? result = window.ShowDialog();

                // Si se eliminó o modificó, recargar la lista
                if (result == true)
                {
                    await LoadActiveItems();
                }
            }
        }

        /// <summary>
        /// Botón para agregar un nuevo item
        /// </summary>
        private async void NewItemButton_Click(object sender, RoutedEventArgs e)
        {
            NewItemWindow window = new NewItemWindow();
            if (window.ShowDialog() == true && window.CreatedItem != null)
            {
                // Agregar el nuevo item al wrap
                await LoadActiveItems();
            }
        }
    }
}

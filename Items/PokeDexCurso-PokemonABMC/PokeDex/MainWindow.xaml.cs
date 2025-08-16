using Microsoft.VisualBasic.FileIO;
using PokeDex.Resources;
using PokeDex.Views;
using System;
using System.Data;
using System.IO;
using System.Media;
using System.Security.Policy;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace PokeDex
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Atributos

        private Grid InitForm;
        private PokedexControl PokedexForm = new();
        private int CurrentOption = -1;

        #endregion

        #region Constructor e inicador
        public MainWindow()
        {
            //Con esto mantengo las proporciones HD.
            double proportions = (double)1080 / (double)1920;
            double Height = SystemParameters.PrimaryScreenHeight * 0.9;
            this.Height = Height;
            this.Width = Height / proportions;
            
            InitializeComponent();
            //Esto es para poder mostrarlo mas adelante
            InitForm = this.InitGrid;
        }
        private void MainWin_Loaded(object sender, RoutedEventArgs e)
        {
            FormManager(0);
        }
        #endregion

        #region Capa Presentacion
        private void MainWin_Activated(object sender, EventArgs e) => ActivateWindows(true);
        private void MainWin_Deactivated(object sender, EventArgs e) => ActivateWindows(false);
        private void MainWin_Closing(object sender, System.ComponentModel.CancelEventArgs e) => ClosingWindows(e);
        private void HomeButton_Click(object sender, RoutedEventArgs e) => FormManager(0);
        private void PokedexButton_Click(object sender, RoutedEventArgs e) => FormManager(1);
        private void CombatsButton_Click(object sender, RoutedEventArgs e) => FormManager(2);
        private void TrainersButton_Click(object sender, RoutedEventArgs e) => FormManager(3);
        private void UrlButton_Click(object sender, RoutedEventArgs e)
            => System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("cmd", $"/c start https://www.youtube.com/watch?v=dQw4w9WgXcQ") { CreateNoWindow = true });
        #endregion

        #region Capa Negocio
        private void ActivateWindows(bool isActive)
        {
            if (isActive) this.Opacity = 1;
            else this.Opacity = 0.5;
        }
        private void ClosingWindows(System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult cierra = MessageBox.Show("Esta seguro que quiere cerrar la ventana?", "Atencion", MessageBoxButton.YesNo, MessageBoxImage.Question);
            //Si el usuario apreta "No" entonces cancelamos el evento de cierre
            if (cierra.Equals(MessageBoxResult.No)) e.Cancel = true;
        }
        
        private void FormManager(int option)
        {
            if(CurrentOption == option) return;
            else
            {
                HomeButton.IsEnabled = true;
                PokedexButton.IsEnabled = true;
                CombatsButton.IsEnabled = true;
                TrainersButton.IsEnabled = true;
                FormGrid.Children.Clear();
                CurrentOption = option;
                switch (option)
                {
                    case 0:
                        FormGrid.Children.Add(InitForm);
                        HomeButton.IsEnabled = false;
                        break;
                    case 1:
                        FormGrid.Children.Add(PokedexForm);
                        PokedexButton.IsEnabled = false;
                        break;
                    case 2:
                        CombatsButton.IsEnabled = false;
                        break;
                    case 3:
                        TrainersButton.IsEnabled = false;
                        break;
                    default:
                        FormGrid.Children.Add(InitForm);
                        break;
                }
                //Sacamos el focus asi nos queda mejor visualmente los botones
                PokeBorder.Focus();
            }
        }
        #endregion
    }
}
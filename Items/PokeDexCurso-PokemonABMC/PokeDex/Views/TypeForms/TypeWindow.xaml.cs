using Microsoft.VisualBasic;
using PokeDex.Entities;
using PokeDex.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PokeDex.Views.TypeForms
{
    /// <summary>
    /// Lógica de interacción para TypeWindow.xaml
    /// </summary>
    public partial class TypeWindow : Window
    {
        private List<Tipo> AllTypes = Tipo.GetAllTypes(); 
        private bool ModType = false;
        private Tipo? SelectedType;
        public TypeWindow(bool modType)
        {
            InitializeComponent();
            ModType = modType;
        }

        private void TypeWin_Loaded(object sender, RoutedEventArgs e)
        {
            //Es una modificacion
            if(ModType && AllTypes.Count > 0)
            {
                //Con esto configuramos el combo box para que eliga la primera opcion
                TypesCombos.ItemsSource = AllTypes;
                //Hacemos que muestre los nombres de los tipos
                TypesCombos.DisplayMemberPath = "Nombre";
                //Primera opcion
                TypesCombos.SelectedIndex = 0;
                //Seleccionamos el primero
                SelectedType = AllTypes[0];
                
                NameTextBox.Text  = SelectedType.Nombre;
                ShortTextBox.Text = SelectedType.Abreviatura;
                ColorTextBox.Text = SelectedType.Color;
                TitleLabel.Content = "Modificar Tipo";
                VarButton.Content  = "Modificar";
                //Cambiamos la visibilidad
                NameTextBox.Visibility = Visibility.Collapsed;
                TypesCombos.Visibility = Visibility.Visible;
                DeleteButton.Visibility = Visibility.Visible;
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e) => Generics.ManageBox((TextBox)sender,Brushes.Black);
        private void TextBox_LostFocus(object sender, RoutedEventArgs e) => Generics.ManageBox((TextBox)sender, Brushes.Gray);
        private void TypesCombos_SelectionChanged(object sender, SelectionChangedEventArgs e) => SelectType();
        private void DeleteButton_Click(object sender, RoutedEventArgs e) => DeleteType();
        private void VarButton_Click(object sender, RoutedEventArgs e) => ActionType();
        /// <summary>
        /// Lo creamos en el atributo estatico que se borra una vez que cerramos la aplicacion
        /// </summary>
        private void ActionType()
        {
            //Verificamos que no este vacio y no sea su placeholder
            if (NameTextBox.Text.Trim() != "" && NameTextBox.Text != NameTextBox.Tag.ToString() &&
                ShortTextBox.Text.Trim() != "" && ShortTextBox.Text != ShortTextBox.Tag.ToString() &&
                ColorTextBox.Text.Trim() != "" && ColorTextBox.Text != ColorTextBox.Tag.ToString())
            {
                //Si tiene algo seleccionado esto es no null
                if (ModType && SelectedType != null)
                {
                    SelectedType.Nombre = NameTextBox.Text;
                    SelectedType.Abreviatura = ShortTextBox.Text;
                    SelectedType.Color = ColorTextBox.Text;
                    this.Close();
                }
                else
                {
                    AllTypes.Add(
                            new(AllTypes.Count, NameTextBox.Text, ShortTextBox.Text, ColorTextBox.Text)
                    );
                    this.Close();
                }

            }
            else MessageBox.Show("ERROR: Hay uno de los campos sin completar.", "", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        private void DeleteType()
        {
            //Si tiene algo seleccionado esto es no null
            if (ModType && SelectedType != null)
            {
                AllTypes.Remove( SelectedType );
                this.Close();
            }
        }
        private void SelectType()
        {
            //Cambiamos el nombre
            if(TypesCombos.SelectedIndex != -1)
            {
                SelectedType = AllTypes[TypesCombos.SelectedIndex];
                NameTextBox.Text = SelectedType.Nombre;
                ShortTextBox.Text = SelectedType.Abreviatura;
                ColorTextBox.Text = SelectedType.Color;
            }
        }

    }
}
using DAL.Adapters;
using PokeDex.Entities;
using PokeDex.Logs;
using PokeDex.Resources.Templates;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PokeDex.Views
{
    /// <summary>
    /// Lógica de interacción para PokemonWindow.xaml
    /// </summary>
    public partial class PokemonWindow : Window
    {
        private List<Tipo> AllTypes = Tipo.GetAllTypes();
        public Pokemon? ModPokemon;
        public Pokemon? CreatedPokemon;
        public PokemonWindow(Pokemon? modPokemon)
        {
            InitializeComponent();
            ModPokemon = modPokemon;
        }

        private void PokeWin_Loaded(object sender, RoutedEventArgs e)
        {
            int pokeID = 1;
            FirstComboBox.ItemsSource = AllTypes;
            FirstComboBox.DisplayMemberPath = "Nombre";
            SecondComboBox.ItemsSource = AllTypes;
            SecondComboBox.DisplayMemberPath = "Nombre";

            //Configuramos para un nuevo pokemon
            if (ModPokemon == null)
            {
                //El siguiente pokemon al ultimo
                pokeID = Pokemon.GetLastEntryId()+1;
                FirstComboBox.SelectedIndex = -1;
                SecondComboBox.SelectedIndex = -1;

                ModButton.IsEnabled = false;
                DeleteButton.IsEnabled = false;
            }
            else
            {
                pokeID = ModPokemon.Id;
                NameTextBox.Text = ModPokemon.Nombre;
                SizeTextBox.Text = ModPokemon.Altura.ToString();
                WeightTextBox.Text = ModPokemon.Peso.ToString();
                GenTextBox.Text = ModPokemon.Generacion.ToString();

                int index = AllTypes.FindIndex(t => t.Id == ModPokemon.TipoPrimario.Id);

                FirstComboBox.SelectedIndex = index;
                index = AllTypes.FindIndex(t => t.Id == ModPokemon.TipoSecundario?.Id);

                SecondComboBox.SelectedIndex = index;

                CreateButton.IsEnabled = false;

            }
            int len = pokeID.ToString().Length;
            string format = "";
            //Dependiendo el largo del numero vemos cuantos 0 agregamos
            for (int i = 0; i < (3 - len); i++) format += "0";
            format = format + pokeID;
            try { PreImage.Source = new BitmapImage(new Uri("https://www.pokemon.com/static-assets/content-assets/cms2/img/pokedex/full/" + format + ".png")); }
            catch (Exception ex) { Logger.RegistrarERROR(ex, "Ocurrio un error al buscar la imagen del pokemon"); }
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e) => CreatePokemon();
        private void restoreButton_Click(object sender, RoutedEventArgs e) => SecondComboBox.SelectedIndex = -1;
        private void ModButton_Click(object sender, RoutedEventArgs e) => UpdatePokemon();
        private void DeleteButton_Click(object sender, RoutedEventArgs e) => DeletePokemon();

        private void CreatePokemon()
        {
            //Con un metodo verificamos que toda la informacion es correcta.
            if (CheckFields())
            {
                PokemonAdapterAPI consultor = new();

                CreatedPokemon = Pokemon.CreateNewPokemon(
                        new(0, NameTextBox.Text.Trim(),
                        float.Parse(SizeTextBox.Text),
                        float.Parse(WeightTextBox.Text),
                        int.Parse(GenTextBox.Text), 
                        AllTypes[FirstComboBox.SelectedIndex],

                        //Si el indice es igual a -1 envia null
                        (SecondComboBox.SelectedIndex != -1)?  AllTypes[SecondComboBox.SelectedIndex] : null
                        )
                    );
                if (CreatedPokemon.Id != -1)
                {
                    MessageBox.Show("EXITO: Pokemon creado exitosamente! Revise la Pokedex para encontrarlo", "Exito", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else
                {
                    //Lo limpiamos para que no ocurra un error en la pantalla anterior
                    CreatedPokemon = null;
                    MessageBox.Show("ERROR: Ocurrio un problema a la hora de cargar el pokemon", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
  
        }
        private void UpdatePokemon()
        {
            //Con un metodo verificamos que toda la informacion es correcta.
            if (CheckFields())
            {
                PokemonAdapterAPI consultor = new();

                bool response = Pokemon.UpdatePokemon(
                        new(0, NameTextBox.Text.Trim(),
                        float.Parse(SizeTextBox.Text),
                        float.Parse(WeightTextBox.Text),
                        int.Parse(GenTextBox.Text),
                        AllTypes[FirstComboBox.SelectedIndex],

                        //Si el indice es igual a -1 envia null
                        (SecondComboBox.SelectedIndex != -1) ? AllTypes[SecondComboBox.SelectedIndex] : null
                        )
                    );
                if (response && ModPokemon != null)
                {
                    //Si los datos al actualizar no dieron error significa que podemos actualizar el pokemon sin problemas
                    ModPokemon.Nombre = NameTextBox.Text.Trim();
                    ModPokemon.Altura = float.Parse(SizeTextBox.Text);
                    ModPokemon.Peso = float.Parse(WeightTextBox.Text);
                    ModPokemon.Generacion = int.Parse(GenTextBox.Text);
                    ModPokemon.TipoPrimario = AllTypes[FirstComboBox.SelectedIndex];
                    ModPokemon.TipoSecundario = (SecondComboBox.SelectedIndex != -1) ? AllTypes[SecondComboBox.SelectedIndex] : null;

                    MessageBox.Show("EXITO: Pokemon actualizado!", "Exito", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("ERROR: Ocurrio un problema a la hora de cargar el pokemon", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void DeletePokemon()
        {
            MessageBoxResult confirm = MessageBox.Show("ATENCION: Esta apunto de eliminar este pokemon, puede traer consecuencias en la base"
                , "Atencion", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirm == MessageBoxResult.Yes && ModPokemon != null)
            {
                if (Pokemon.DeletePokemon(ModPokemon.Id))
                {
                    ModPokemon = null;
                    MessageBox.Show("EXITO: Pokemon Eliminado!", "Exito", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else MessageBox.Show("ERROR: Ocurrio un problema. Intente nuevamente", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        private bool CheckFields()
        {
            float weigth;
            float size;
            int gen;
            //Nombre no vacio mayor a una letra y menor a 20
            if(NameTextBox.Text.Trim() != "" && NameTextBox.Text.Length > 1 && NameTextBox.Text.Length < 20)
            {
                try
                {
                    size = float.Parse(SizeTextBox.Text);
                    weigth = float.Parse(WeightTextBox.Text);
                    gen = int.Parse(GenTextBox.Text);
                    if(gen < 9)
                    {
                        //El maximo por ahora
                        int index1 = FirstComboBox.SelectedIndex;
                        int index2 = SecondComboBox.SelectedIndex;

                        //Si cumple todas las condiciones damos true
                        if (index1 != -1 && index1 != index2) return true;
                        else
                        {
                            MessageBox.Show("ERROR: El primer tipo no puede ser vacio ni igual al segundo", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return false;
                        }
                    }
                    else
                    {
                        MessageBox.Show("ERROR: Generacion 9 maximo", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("ERROR: No se pudo convertir un valor numerico", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            else
            {
                MessageBox.Show("ERROR: No se escribio un nombre valido", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}
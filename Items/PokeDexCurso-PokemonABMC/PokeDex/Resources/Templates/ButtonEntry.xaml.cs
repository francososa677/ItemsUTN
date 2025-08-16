using PokeDex.Entities;
using PokeDex.Logs;
using PokeDex.Views;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;

namespace PokeDex.Resources.Templates
{
    /// <summary>
    /// Lógica de interacción para ButtonEntry.xaml
    /// </summary>
    public partial class ButtonEntry : UserControl
    {
        private static string ResourcesDirectory = "pack://application:,,,/PokeDex;component/Resources/PokeImages/";
        private readonly Pokemon PokeEntry;
        private string FormatEntry = "";
        public ButtonEntry(Pokemon pokeEntry)
        {
            InitializeComponent();
            this.PokeEntry = pokeEntry;
        }

        private void EntryCard_Loaded(object sender, RoutedEventArgs e)
        {
            LoadPokeData();
        }
        private void Mainbutton_Click(object sender, RoutedEventArgs e) => UpdateInfoPokemon();
        private void UpdateInfoPokemon()
        {
            PokemonWindow CreationWindows = new(PokeEntry);
            CreationWindows.ShowDialog();

            //Si es null es porque lo eliminaron
            if(CreationWindows.ModPokemon != null) LoadPokeData();
            else this.Visibility = Visibility.Collapsed;
        }
       
        private void LoadPokeData()
        {
            FormatEntry = "";
            int len = PokeEntry.Id.ToString().Length;
            //Dependiendo el largo del numero vemos cuantos 0 agregamos
            for (int i = 0; i < (3-len); i++) FormatEntry += "0";
            FormatEntry = "#"+ FormatEntry + PokeEntry.Id.ToString();
            EntryBigLabel.Content = FormatEntry;
            EntrySmallLabel.Content = FormatEntry;
            NameLabel.Content = PokeEntry.Nombre;
            WeightLabel.Content = PokeEntry.Peso + " Kg";
            SizeLabel.Content = PokeEntry.Altura + " m";

            SearchImage();
            //Intentamos crear el color desde el hexadecimal
            FirstTypeBorder.Background = Generics.CreateColorFromHex(PokeEntry.TipoPrimario.Color);

            FirstLabel.Content = PokeEntry.TipoPrimario.Nombre;
            if (PokeEntry.TipoSecundario != null)
            {
                SecondTypeBorder.Background = Generics.CreateColorFromHex(PokeEntry.TipoSecundario.Color);
                SecondLabel.Content = PokeEntry.TipoSecundario.Nombre;
                SecondTypeBorder.Visibility = Visibility.Visible;
                FirstTypeBorder.Margin = new Thickness(5);
            }
            //Si tiene un solo tipo debemos borrar el segundo tipo
            else
            {
                SecondTypeBorder.Visibility = Visibility.Collapsed;
                //Con esto nos queda al medio
                FirstTypeBorder.Margin = new Thickness(80, 5, 5, 5);
            }

        }
        private void SearchImage()
        {
           // "https://www.pokemon.com/static-assets/content-assets/cms2/img/pokedex/full/152.png";
            //Creamos el nombre del archivo e intentamos encontrarlo.
            string imageName = ResourcesDirectory + PokeEntry.Nombre+".png";
            try
            {
                PokeImage.Source = new BitmapImage(new Uri("https://www.pokemon.com/static-assets/content-assets/cms2/img/pokedex/full/" + FormatEntry.Replace("#","") + ".png"));
            }
            catch (Exception ex)
            {
                Logger.RegistrarERROR(ex,"Ocurrio un error al buscar la imagen del pokemon");
            }
        }
        /// <summary>
        /// Esta funcion muestra en los casos que sea uno de sus tipos o sea el valor -1
        /// </summary>
        /// <param name="idType">Un valor numerico que representa al tipo</param>
        public void FilterByType(int idType)
        {
            //Con esto pregunto si debo o no mostrar este tipo
            if(idType == -1 || PokeEntry.TipoPrimario.Id == idType || PokeEntry.TipoSecundario?.Id == idType)this.Visibility = Visibility.Visible;
            else this.Visibility = Visibility.Collapsed;
        }

    }
}

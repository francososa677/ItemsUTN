using DAL.Adapters;
using PokeDex.Entities;
using PokeDex.Logs;
using PokeDex.Resources;
using PokeDex.Resources.Templates;
using PokeDex.Views;
using PokeDex.Views.TypeForms;
using System.Windows;
using System.Windows.Controls;

namespace PokeDex.Views
{
    /// <summary>
    /// Lógica de interacción para PokedexControl.xaml
    /// </summary>
    public partial class PokedexControl : UserControl
    {
        private List<Tipo> AllTypes = Tipo.GetAllTypes();
        private List<Pokemon> AllPokemon = Pokemon.GetAll();
        private Style? ButStyle;
        private int SelectedType = -1;
        public PokedexControl()
        {
            InitializeComponent();
        }

        private void UserPrueba_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            LoadPokedex();
            LoadTypes();
        }
        /// <summary>
        /// Esta accion generica se le asigna a todos los botones de tipos para poder asi filtrar correctamente
        /// </summary>
        /// <param name="sender">El boton que recibio el click</param>
        /// <param name="e">El evento que lo inicio</param>
        private void Type_Click(object sender,RoutedEventArgs e) => TypeManager((Button) sender );
        private void NewTypeButton_Click(object sender, RoutedEventArgs e) => ActionType(false);
        private void ModTypeButton_Click(object sender, RoutedEventArgs e) => ActionType(true);
        private void NewPokeButton_Click(object sender, RoutedEventArgs e) => CreatePokemon();
        private void LoadPokedex()
        {
            //PokemonAdapterAPI adapter = new();
            //List<DAL.Models.Pokemon> PruebaAPI = adapter.ObtenerPokemones();
             
            MainWrap.Children.Clear();
            foreach (Pokemon pokemon in AllPokemon)
            {
                ButtonEntry entry = new(pokemon);
                MainWrap.Children.Add(entry);
            }
        }
        private void LoadTypes()
        {
            TypesWrap.Children.Clear();
            //Intento buscar el estilo en para asignarselo a los botones
            try{ButStyle = Application.Current.FindResource("TypeButtonStyle") as Style;}
            catch (Exception ex)
            {
                Logger.RegistrarERROR(ex, "No se pudo encontrar el estilo para este boton");
                ButStyle = new();
            }
            
            Button btn = new()
            {
                Content = "Todos",
                Style = ButStyle,
                Background = Generics.CreateColorFromHex("FF878890"),
                Tag = -1 //Identificamos todos con esto
            };
            //Con esto le agregamos un evento por codigo
            btn.Click += Type_Click;
            //Lo agregamos al wrap de arriba
            TypesWrap.Children.Add(btn);

            foreach (Tipo tipoDisponibles in AllTypes)
            {
                btn = new()
                {
                    Content = tipoDisponibles.Nombre,
                    Style = ButStyle,
                    Background = Generics.CreateColorFromHex(tipoDisponibles.Color),
                    Tag = tipoDisponibles.Id //Al agregarle un ID podemos identifacar quien hizo el click
                };
                btn.Click += Type_Click;
                TypesWrap.Children.Add(btn);
            }
        }
        /// <summary>
        /// Esta funcion va a servir para filtrar las entradas dependiendo del tipo seleccionado.
        /// </summary>
        private void TypeManager(Button sender)
        {
            int idType = int.Parse(sender.Tag.ToString()??"-1");

            //Si apretamos 2 veces el mismo boton no cambia en nada
            if(SelectedType != idType)
            {
                SelectedType = idType;
                //Si agregamos otro tipo de valor al wrap debemos cambiar esto a var
                foreach (ButtonEntry item in MainWrap.Children) item.FilterByType(idType);
            }
            else return;
        }

        private void ActionType(bool modType)
        {
            TypeWindow typeWindow = new(modType);
            typeWindow.ShowDialog();
            LoadTypes();
        }
        private void CreatePokemon()
        {
            PokemonWindow CreationWindows = new(null);
            CreationWindows.ShowDialog();

            //Si creamos un pokemon nos deberia aparecer como no null
            if(CreationWindows.CreatedPokemon != null)
            {
                ButtonEntry entry = new(CreationWindows.CreatedPokemon);
                MainWrap.Children.Add(entry);
            }
        }
        private void UpdatePokemon()
        {

        }
    }
}

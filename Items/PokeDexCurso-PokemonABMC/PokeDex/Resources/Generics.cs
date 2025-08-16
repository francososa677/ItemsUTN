using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace PokeDex.Resources
{
    public class Generics
    {
        /// <summary>
        /// Estos numeros escritos a mano se utilizan para pasar de decimal a texto coloquial. Principalmente en documentacion
        /// </summary>
        private static readonly string[] numeros = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        /// <summary>
        /// Esta funcion recibe un color hexadecimal (sin el #) y lo convierte en un SolidColorBrush
        /// </summary>
        /// <param name="hexaColor">Un conjunto aceptado de hexadecimales que representan un color</param>
        /// <returns>Un brush para utilizar en el front</returns>
        public static Brush CreateColorFromHex(string hexaColor)
        {
            try
            {
                //Intentamos crear el color
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#" + hexaColor));
            }
            catch (Exception)
            {
                return Brushes.Black;
            }
            
        }
        /// <summary>
        /// Valida utilizando un atributo local que el string pasado sea un numero y devuelve un booleano
        /// </summary>
        /// <param name="a"> El string 'a' de longitud 1 que deseamos validar </param>
        /// <returns> True si es un numero, False si no lo es </returns>
        public static bool IsNumber(string a)
        {
            // Revisa que sea un numero lo que se mando
            foreach (string x in numeros) if (x.Equals(a)) return true;
            return false;
        }
        /// <summary>
        /// Manage Box es el equivalente a Placeholder de HTML y funciona unicamente si nosotros completamos el TAG 
        /// de nuestro textbox. Si nuestro textbox esta vacio lo reemplaza con este texto.
        /// </summary>
        /// <param name="tb">El textbox</param>
        /// <param name="color">El color que queremos que tome cuando lo seleccionamos o deseleccionamos</param>
        public static void ManageBox(TextBox tb, Brush color)
        {
            //Sino le pusimos tag directamente no hace nada.
            if (tb.Tag == null) return;
            tb.Foreground = color;
            if (tb.Text.Equals(tb.Tag.ToString())) tb.Text = "";
            else
            {
                if (tb.Text.Equals("")) tb.Text = tb.Tag.ToString();
            }
        }
        /// <summary>
        /// Funcion de apoyo utilizada en los MaskedTextBox para eliminar un solo caracter de un textbox y acomodar su puntero
        /// </summary>
        /// <param name="tx"> El TextBox que deseamos eliminar </param>
        /// <param name="i"> El index en el cual se ubica este caracter </param>
        private static void EraseChar(TextBox tx, int i)
        {
            if (i < tx.Text.Length)
            {
                tx.Text = tx.Text.Remove(i, 1);
                tx.CaretIndex = tx.Text.Length;
                return;
            }
        }

        /// <summary>
        /// Suele usarse con el textChanged para corroborar que el textbox contenga solo numeros
        /// </summary>
        /// <param name="tx"> Es el textBox que va a tener esta masscara </param>
        public static void MaskedNumber(TextBox tx)
        {
            //Cada vez que se cambia el texto tanto como cuando hay una entrada por teclado o se pega algo revisa que sea un numero
            if (!tx.IsInitialized) return;
            if (tx.Text.Equals("")) return;
            if (tx.Tag != null && tx.Text == tx.Tag.ToString()) return;
            string x;
            for (int i = 0; i < tx.Text.Length; i++)
            {
                x = tx.Text.Substring(i, 1);
                if (!IsNumber(x)) EraseChar(tx, i);
            }
            tx.CaretIndex = tx.Text.Length;
        }
        /// <summary>
        /// Suele usarse con el textChanged para corroborar que el textbox contenga solo numeros
        /// </summary>
        /// <param name="tx"> Es el textBox que va a tener esta masscara </param>
        public static void MaskedNumber(TextBox tx, int maxLenght)
        {
            //Cada vez que se cambia el texto tanto como cuando hay una entrada por teclado o se pega algo revisa que sea un numero
            if (!tx.IsInitialized) return;
            if (tx.Text.Equals("")) return;
            if (tx.Tag != null && tx.Text == tx.Tag.ToString()) return;
            if (tx.Text.Length > maxLenght)
            {
                EraseChar(tx, tx.Text.Length - 1);
                tx.CaretIndex = tx.Text.Length;
                return;
            }
            if (tx.Text.Equals(""))
            {
                return;
            }
            string x;
            for (int i = 0; i < tx.Text.Length; i++)
            {
                x = tx.Text.Substring(i, 1);
                if (!IsNumber(x)) EraseChar(tx, i);
            }
            tx.CaretIndex = tx.Text.Length;
        }
        /// <summary>
        /// Una mascara simple que es para controlar el largo del texto un texbox
        /// </summary>
        /// <param name="tx"> El textbox a controlar</param>
        /// <param name="maxLenght"> La longitud maxima que permitimos</param>
        public static void MaskedString(TextBox tx, int maxLenght)
        {
            if (!tx.IsInitialized) return;
            if (tx.Text.Equals("")) return;
            if (tx.Tag != null && tx.Text == tx.Tag.ToString()) return;
            if (tx.Text.Length > maxLenght)
            {
                EraseChar(tx, tx.Text.Length - 1);
                tx.CaretIndex = tx.Text.Length;
                return;
            }
        }
    }
}
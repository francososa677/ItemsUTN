using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class TipoPokemon
    {
        public TipoPokemon(int id, string name, string abreviatura, string color)
        {
            this.id = id;
            this.name = name;
            this.abreviatura = abreviatura;
            this.color = color;
        }

        public int id { get; set; }
        public string name { get; set; }
        public string abreviatura { get; set; }
        public string color { get; set; }
    }
}

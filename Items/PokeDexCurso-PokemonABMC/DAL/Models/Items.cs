using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Items
    {
        /// <summary>
        /// Id único del item.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre del item.
        /// </summary>
        public string NombreItem { get; set; }

        /// <summary>
        /// Stock máximo del item.
        /// </summary>
        public int StockMaximo { get; set; }

        /// <summary>
        /// Fecha de creación del item.
        /// </summary>
        public DateTime FechaCreacion { get; set; }

        /// <summary>
        /// Efecto del item.
        /// </summary>
        public string Efecto { get; set; }

        /// <summary>
        /// Indica si el item está activo.
        /// </summary>
        public bool ItemActivo { get; set; }

        /// <summary>
        /// Constructor vacío.
        /// </summary>
        public Items() { }

        /// <summary>
        /// Constructor con parámetros para inicializar un item.
        /// </summary>
        public Items(int id, string nombreItem, int stockMaximo, DateTime fechaCreacion, string efecto, bool itemActivo)
        {
            Id = id;
            NombreItem = nombreItem;
            StockMaximo = stockMaximo;
            FechaCreacion = fechaCreacion;
            Efecto = efecto;
            ItemActivo = itemActivo;
        }

        /// <summary>
        /// Constructor que inicializa un item a partir de un DataRow.
        /// </summary>
        public Items(DataRow row)
        {
            Id = Convert.ToInt32(row["id"]);
            NombreItem = row["nombre_item"].ToString();
            StockMaximo = Convert.ToInt32(row["stock_maximo"]);
            FechaCreacion = Convert.ToDateTime(row["fecha_creacion"]);
            Efecto = row["efecto"].ToString();
            ItemActivo = Convert.ToBoolean(row["item_activo"]);
        }

    }
}
using System.Data;

/// <summary>
/// Representa un Item del sistema.
/// </summary>
public class Item
{
    public int Id { get; set; }
    public string Nombre_Item { get; set; }
    public int Stock_Maximo { get; set; }
    public DateTime Fecha_Creacion { get; set; }
    public string Efecto { get; set; }
    public bool Item_Activo { get; set; }

    /// <summary>
    /// Constructor que inicializa un Item a partir de un DataRow.
    /// </summary>
    /// <param name="fila">Fila de datos de la consulta SQL</param>
    public Item(DataRow fila)
    {
        Id = int.Parse(fila["id"].ToString() ?? "0");
        Nombre_Item = fila["nombre_item"].ToString() ?? "ERROR";
        Stock_Maximo = int.Parse(fila["stock_maximo"].ToString() ?? "0");
        Fecha_Creacion = DateTime.Parse(fila["fecha_creacion"].ToString() ?? DateTime.MinValue.ToString());
        Efecto = fila["efecto"].ToString() ?? "";
        Item_Activo = bool.Parse(fila["item_activo"].ToString() ?? "false");
    }
}

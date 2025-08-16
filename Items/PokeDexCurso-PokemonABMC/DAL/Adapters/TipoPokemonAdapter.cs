using DAL.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Adapters
{
    public class TipoPokemonAdapter
    {
        public TipoPokemonAdapter()
        {

        }
        public List<TipoPokemon> GetAllTypes()
        {
            GeneralAdapterAPIRest consult = new();
            //No cree el endpoint en la API asi que directamente copio la respuesta esperada
            string result = "[{ 'id':0,'nombre':'Bicho','abreviatura':'Bic','color':'729f3f'},{ 'id':1,'nombre':'Dragon','abreviatura':'Dra','color':'f16e57'},{ 'id':2,'nombre':'Hada','abreviatura':'Had','color':'fdb9e9'},{ 'id':3,'nombre':'Fuego','abreviatura':'Fue','color':'fd7d24'},{ 'id':4,'nombre':'Fantasma','abreviatura':'Fan','color':'7b62a3'},{ 'id':5,'nombre':'Tierra','abreviatura':'Tie','color':'f7de3f'},{ 'id':6,'nombre':'Normal','abreviatura':'Nor','color':'a3abaf'},{ 'id':7,'nombre':'Psiquico','abreviatura':'Psi','color':'f366b9'},{ 'id':8,'nombre':'Acero','abreviatura':'Ace','color':'9eb7b8'},{ 'id':9,'nombre':'Siniestro','abreviatura':'Sin','color':707070},{ 'id':10,'nombre':'Electrico','abreviatura':'Ele','color':'eed535'},{ 'id':11,'nombre':'Pelea','abreviatura':'Pel','color':'d56723'},{ 'id':12,'nombre':'Volador','abreviatura':'Vol','color':'bdb9b8'},{ 'id':13,'nombre':'Planta','abreviatura':'Pla','color':'9bcc50'},{ 'id':14,'nombre':'Hielo','abreviatura':'Hie','color':'51c4e7'},{ 'id':15,'nombre':'Veneno','abreviatura':'Ven','color':'b97fc9'},{ 'id':16,'nombre':'Roca','abreviatura':'Roc','color':'a38c21'},{ 'id':17,'nombre':'Agua','abreviatura':'Agu','color':'4592c4'}]";

            //No reconoce los nombres asi que los carge de esta forma
            return new()
            {
                new(0,"Bicho","Bic","729f"),
                new(1,"Dragon","Dra","f16e57" ),
                new(2,"Hada","Had","fdb9e9"),
                new(3,"Fuego","Fue","fd7d24"),
                new(4,"Fantasma","Fan","7b62a3"),
                new(5,"Tierra","Tie","f7de3f"),
                new(6,"Normal","Nor","a3abaf"),
                new(7,"Psiquico","Psi","f366b9"),
                new(8,"Acero","Ace","9eb7b8"),
                new(9,"Siniestro","Sin","707070"),
                new(10,"Electrico","Ele","eed535"),
                new(11,"Pelea","Pel","d56723"),
                new(12,"Volador","Vol","bdb9b8"),
                new(13,"Planta","Pla","9bcc50"),
                new(14,"Hielo","Hie","51c4e7"),
                new(15,"Veneno","Ven","b97fc9"),
                new(16,"Roca","Roc","a38c21"),
                new(17,"Agua","Agu","4592c4"),
            };
            

            if (result.Trim() != "" && result != "ERROR") return JsonConvert.DeserializeObject<List<TipoPokemon>>(result) ?? new();
            else return new();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace APISoapAlergenos
{
    /// <summary>
    /// Summary description for ObtenerAlergenosyPlatos
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class ObtenerAlergenosyPlatos : System.Web.Services.WebService
    {

        private dbRestauranteEntities gDB = new dbRestauranteEntities();

        //Creamos las clase sin herencia para poder devolver
        public class AlergenoSinHerencia
        {
            public int id_Alergeno { get; set; }
            public string Nombre_Alergeno { get; set; }
            public string Descripcion_Alergeno { get; set; }
        }

        public class PlatoSinHerencia
        {
            public int Id_Plato { get; set; }
            public string Nombre_Plato { get; set; }
            public string Descripcion_Plato { get; set; }
        }

        //Creammos las clases para poder insertar en la tabla tbIng_Plato y tbAlerg_Ing
        public class IngXPlato
        {
            public int idIng { get; set; }
            public int Cantidad { get; set; }
            public int idIngXPlato { get; set; }
        }

        public class AlergXIng
        {
            public int idAlergXIng { get; set; }
            public int idAlerg { get; set; }
        }

        //WebMMehtods
        [WebMethod]
        public AlergenoSinHerencia[] ObtieneAlergenos(int pIdPlato)
        {
            tbPlatos vPlato;
            vPlato = gDB.tbPlatos.Find(pIdPlato);
            tbAlergenos[] aAlergenos = { };
            List<AlergenoSinHerencia> lAlergenos = new List<AlergenoSinHerencia>();

            if (vPlato != null)
            {
                aAlergenos = RecuperarAlergenos(vPlato).ToArray();
            }
            foreach (var vAlerg in aAlergenos)
            {
                AlergenoSinHerencia vAuxAlergeno = new AlergenoSinHerencia();
                vAuxAlergeno.id_Alergeno = vAlerg.id_Alergeno;
                vAuxAlergeno.Nombre_Alergeno = vAlerg.Nombre_Alergeno;
                vAuxAlergeno.Descripcion_Alergeno = vAlerg.Descripcion_Alergeno;
                lAlergenos.Add(vAuxAlergeno);

            }
            return lAlergenos.ToArray();
        }
        [WebMethod]
        public PlatoSinHerencia[] ObtienePlatos(int pIdAlergeno)
        {
            tbAlergenos vAlerg;
            vAlerg = gDB.tbAlergenos.Find(pIdAlergeno);
            tbPlatos[] aPlatos = { };
            List<PlatoSinHerencia> lPlatos = new List<PlatoSinHerencia>();

            if (vAlerg != null)
            {
                aPlatos = RecuperarPlatos(vAlerg).ToArray();

                foreach (var vPlato in aPlatos)
                {
                    PlatoSinHerencia vAuxPlato = new PlatoSinHerencia();
                    vAuxPlato.Id_Plato = vPlato.Id_Plato;
                    vAuxPlato.Nombre_Plato = vPlato.Nombre_Plato;
                    vAuxPlato.Descripcion_Plato = vPlato.Descripcion_Plato;
                    lPlatos.Add(vAuxPlato);
                }
            }
            return lPlatos.ToArray();
        }

        [WebMethod]
        public void InsertarPlato(int pIdPlato, string pNombrePlato, string pDescripcionPlato, IngXPlato[] pIngs)
        {
            tbPlatos vAuxPlato = new tbPlatos();
            vAuxPlato.Id_Plato = pIdPlato;
            vAuxPlato.Nombre_Plato = pNombrePlato;
            vAuxPlato.Descripcion_Plato = pDescripcionPlato;
            if (pIngs != null)
            {
                foreach (var vIng in pIngs)
                {
                    tbIng_Plato vAuxIngPlato = new tbIng_Plato();
                    vAuxIngPlato.id_Ingrediente = vIng.idIng;
                    vAuxIngPlato.Cantidad = vIng.Cantidad;
                    vAuxIngPlato.id_Ing_Plato = vIng.idIngXPlato;
                    vAuxIngPlato.Id_Plato = pIdPlato;
                    vAuxPlato.tbIng_Plato.Add(vAuxIngPlato);
                }
            }
            gDB.tbPlatos.Add(vAuxPlato);

            gDB.SaveChanges();        
        }

        [WebMethod]
        public void InsertarIng(int pIdIng, string pNombreIng, string pDescripcionIng, AlergXIng[] pAlergs)
        {
            tbIngredientes vAuxIng = new tbIngredientes();
            vAuxIng.Id_Ingrediente = pIdIng;
            vAuxIng.Nombre_Ingrediente = pNombreIng;
            vAuxIng.Descripcion_Ingrediente = pDescripcionIng;
            if (pAlergs != null)
            {
                foreach (var vAlerg in pAlergs)
                {
                    tbAlerg_Ing vAuxAlergIng = new tbAlerg_Ing();
                    vAuxAlergIng.id_Alerg_ing = vAlerg.idAlergXIng;
                    vAuxAlergIng.id_Alergeno = vAlerg.idAlerg;
                    vAuxAlergIng.id_Ingrediente = pIdIng;
                    vAuxIng.tbAlerg_Ing.Add(vAuxAlergIng);
                }
            }
            gDB.tbIngredientes.Add(vAuxIng);

            gDB.SaveChanges();
        }

        [WebMethod]
        public void InsertarAlerg(int pIdAlerg, string pNombreAlerg, string pDescripcionAlerg)
        {
            tbAlergenos vAuxAlerg = new tbAlergenos();
            vAuxAlerg.id_Alergeno = pIdAlerg;
            vAuxAlerg.Nombre_Alergeno = pNombreAlerg;
            vAuxAlerg.Descripcion_Alergeno = pDescripcionAlerg;

            gDB.tbAlergenos.Add(vAuxAlerg);

            gDB.SaveChanges();
        }

        //Funcion que recupera los alergenos a partir de un plato
        private List<tbAlergenos> RecuperarAlergenos(tbPlatos pPlato)
        {
            List<tbAlergenos> lRespAlerg = new List<tbAlergenos>();

            foreach (var vIngXPlato in pPlato.tbIng_Plato)
            {
                foreach (var vAlergXIng in vIngXPlato.tbIngredientes.tbAlerg_Ing)
                {
                    if (!lRespAlerg.Exists(elemento => elemento.id_Alergeno == vAlergXIng.tbAlergenos.id_Alergeno))
                    {
                        lRespAlerg.Add(vAlergXIng.tbAlergenos);
                    }
                }
            }
            return lRespAlerg;
        }

        //Funcion que recupera los platos a partir de un Alergeno
        private List<tbPlatos> RecuperarPlatos(tbAlergenos pAlerg)
        {
            List<tbPlatos> lRespPlatos = new List<tbPlatos>();

            foreach (var vIngXAlerg in pAlerg.tbAlerg_Ing)
            {
                foreach (var vIngXPlato in vIngXAlerg.tbIngredientes.tbIng_Plato)
                {
                    if (!lRespPlatos.Exists(elemento => elemento.Id_Plato == vIngXPlato.tbPlatos.Id_Plato))
                        lRespPlatos.Add(vIngXPlato.tbPlatos);
                }
            }
            return lRespPlatos;
        }
    }
}

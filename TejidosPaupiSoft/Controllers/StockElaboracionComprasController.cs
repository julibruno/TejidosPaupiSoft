using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TejidosPaupiSoft.Models;

namespace TejidosPaupiSoft.Controllers
{
    public class StockElaboracionComprasController : Controller
    {
        private TejidosPaupiDBEntities db = new TejidosPaupiDBEntities();
        // GET: StockElaboracionCompras
        public ActionResult Index()
        {
            return View();
        }
        // Esta funcion es para sacar el stock del Articulo
        //Le paso el id de la InsumoxFabricacion
        public void Elaboracion(int id, string AgregaroQuitar)
        {
            StringBuilder mensaje = new StringBuilder();

            Log log = new Log();
            log.Ts = DateTime.Now;
            log.TipoOperacion = "Elaboracion - Quitar cantidad de Insumo";

            // Se procede a sumar el stock
            try { 
           

            InsumosXFabricacion insumosXFabricacion = db.InsumosXFabricacion.Find(id);

            mensaje.AppendLine("Se procede a " +  AgregaroQuitar+ " el stock del articulo " + insumosXFabricacion.Insumos.Descripcion );


            mensaje.AppendLine("Stock Actual: " + insumosXFabricacion.Insumos.Cantidad);

            mensaje.AppendLine("Cantidad de Elaboracion por "+ AgregaroQuitar+ ": "   + insumosXFabricacion.Cantidad);

            mensaje.AppendLine("Id de InsumosXFabricacion => " + insumosXFabricacion.Id);

            mensaje.AppendLine("El producto a fabricar es => " + insumosXFabricacion.FabricacionProducto.Producto.Descripcion);


            Insumos insumos = db.Insumos.Find(insumosXFabricacion.IdInsumo);


                if(AgregaroQuitar == "Agregar")
                {
                    //Aca lo quitamos porque se agrego a la elaboracion, se quita stock
                    insumos.Cantidad -= insumosXFabricacion.Cantidad;
                }
                else if(AgregaroQuitar =="Quitar")
                {
                    //Aca la accion fue eliminar de la Elaboracion, se le suma al stock
                    insumos.Cantidad += insumosXFabricacion.Cantidad;
                }
                else
                {
                    mensaje.AppendLine("Ha sugido un error con el parametro agregar o quitar, el valor es " + AgregaroQuitar);
                }
            
                mensaje.AppendLine("Stock Final=> " + insumosXFabricacion.Insumos.Cantidad);

                log.LogMensaje = mensaje.ToString();
                db.Log.Add(log);
                db.SaveChanges();
               

                //db.Entry(log).State = EntityState.Modified;

                db.Entry(insumos).State = EntityState.Modified;
                db.SaveChanges();


            }
            catch
            {
                mensaje.AppendLine("Surgio un error al quitar valor del Stock metodo 'Elaboracion'");

                log.LogMensaje = mensaje.ToString();
                db.Log.Add(log);
                db.SaveChanges();
           
            }
        }

        // Esta funcion es para AGREGAR el stock del Articulo
        //Le paso el id de la InsumoxCompra

        public void Compra(int id, string AgregaroQuitar)
        {
            StringBuilder mensaje = new StringBuilder();

            Log log = new Log();
            log.Ts = DateTime.Now;
            log.TipoOperacion = "Compra - Agregar cantidad de Insumo";

            // Se procede a sumar el stock
            try
            {

                InsumosXCompra insumosXCompra = db.InsumosXCompra.Find(id);

                mensaje.AppendLine("Se procede a " + AgregaroQuitar +" el stock del articulo " + insumosXCompra.Insumos.Descripcion);


                mensaje.AppendLine("Stock Actual: " + insumosXCompra.Insumos.Cantidad);

                mensaje.AppendLine("Cantidad de Compra por" + AgregaroQuitar + ": " + insumosXCompra.Cantidad);

                mensaje.AppendLine("Id de InsumosXFabricacion => " + insumosXCompra.Id);




                Insumos insumos = db.Insumos.Find(insumosXCompra.IdInsumos);


                if(AgregaroQuitar == "Agregar") { 
                insumos.Cantidad += insumosXCompra.Cantidad;
                }
                else if(AgregaroQuitar == "Quitar")
                {
                    insumos.Cantidad -= insumosXCompra.Cantidad;
                }
                else
                {
                    mensaje.AppendLine("Hay un error con el parametro de AgregaroQuitar");
                }

                mensaje.AppendLine("Stock Final=> " + insumosXCompra.Insumos.Cantidad);

                log.LogMensaje = mensaje.ToString();
                db.Log.Add(log);
                db.SaveChanges();


                //db.Entry(log).State = EntityState.Modified;

                db.Entry(insumos).State = EntityState.Modified;
                db.SaveChanges();


            }
            catch
            {
                mensaje.AppendLine("Surgio un error al agregar valor del Stock metodo 'Compra'");

                log.LogMensaje = mensaje.ToString();
                db.Log.Add(log);
                db.SaveChanges();

            }
        }


    }
}
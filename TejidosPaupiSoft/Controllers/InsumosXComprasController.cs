using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TejidosPaupiSoft.Models;

namespace TejidosPaupiSoft.Controllers
{
    public class InsumosXComprasController : Controller
    {
        private TejidosPaupiDBEntities db = new TejidosPaupiDBEntities();

        // GET: InsumosXCompras
        public ActionResult Index(int? IdCompraGenerada, int? InsumoUtilizadoEnElaboracion)
        {
            if(IdCompraGenerada != null) { 

                if(InsumoUtilizadoEnElaboracion!= null)
                {
                    InsumosXFabricacion insumosXFabricacion = db.InsumosXFabricacion.Find(InsumoUtilizadoEnElaboracion);

                    ViewBag.ElaboracionNro = insumosXFabricacion.FabricacionProducto.NroElaboracion;
                    ViewBag.ProductoElaboracion = insumosXFabricacion.FabricacionProducto.Producto.Descripcion;
                
                }

                Compras compras = new Compras();
            compras = db.Compras.Where(x => x.Id == IdCompraGenerada).FirstOrDefault();

            ViewBag.CompraId = compras.Id;
            ViewBag.NroCompra = compras.Numero;
            ViewBag.FechaCompra = compras.Fecha;
            
            ViewBag.Observaciones = compras.Observaciones;
            ViewBag.SubTotal = compras.Subtotal;
            ViewBag.Descuentos = compras.Descuentos;
            ViewBag.Total = compras.Total;
            

            var insumosXCompra = db.InsumosXCompra.Include(i => i.Compras).Include(i => i.Insumos).Where(x=>x.IdCompras== IdCompraGenerada);
            return View(insumosXCompra.ToList());

            }
            else
            {
              return RedirectToAction("Index", "Compras");
            }

        }

        // GET: InsumosXCompras/Details/5
        public ActionResult Details(int? IdCompraGenerada)
        {
            if (IdCompraGenerada == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Compras compras = db.Compras.Find(IdCompraGenerada);
            ViewBag.NroCompra = compras.Numero;
            ViewBag.FechaCompra = compras.Fecha;

            ViewBag.Subtotal = compras.Subtotal;
            ViewBag.Descuentos = compras.Descuentos;
            ViewBag.Total = compras.Total;
            ViewBag.Observaciones = compras.Observaciones;

            var insumosXCompra = db.InsumosXCompra.Include(i => i.Compras).Include(i => i.Insumos).Where(x => x.IdCompras == IdCompraGenerada);
            return View(insumosXCompra.ToList());


            
           
        }

        // GET: InsumosXCompras/Create
        public ActionResult Create(int idInsumo, int idcompra )
        {
            
            InsumosXCompra objeto = new InsumosXCompra();
            objeto.IdCompras = idcompra;
            objeto.IdInsumos = idInsumo;
            ViewBag.InsumoDescripcion = db.Insumos.Where(x => x.Id == idInsumo).Select(x => x.Descripcion).FirstOrDefault();
            ViewBag.InsumoUnidadMedida = db.Insumos.Where(x => x.Id == idInsumo).Select(x => x.TipoInsumo.UnidadMedida.Descripcion).FirstOrDefault();

            if (!ValidarExistenciaInsumoEnCompra(objeto))
            {
               
                return RedirectToAction("PreviaCreate","InsumosXCompras",new { IdCompraGenerada =idcompra, ObjetoRepetido = ViewBag.InsumoDescripcion });
            } 

            

          
               
            return View(objeto);
           

        }

        private bool ValidarExistenciaInsumoEnCompra(InsumosXCompra objeto)
        {
            //Voy a validar si el insumo ya esta en la compra
            InsumosXCompra objetoParaValidar = db.InsumosXCompra.Where(x => x.IdCompras == objeto.IdCompras && x.IdInsumos == objeto.IdInsumos).FirstOrDefault();

            if(objetoParaValidar != null)
            {
                return false;
            }
            return true;
            
        }

        // POST: InsumosXCompras/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,IdCompras,IdInsumos,Cantidad,Precio")] InsumosXCompra insumosXCompra)
        {
            if (ModelState.IsValid)
            {

                db.InsumosXCompra.Add(insumosXCompra);
                db.SaveChanges();

                //Aca pongo los Descuentos y valor final de la compra
                UpdateCompra(insumosXCompra.IdCompras);


                //Le sumo el Stock al insumo
                StockElaboracionComprasController stockElaboracionCompras = new StockElaboracionComprasController();
                stockElaboracionCompras.Compra(insumosXCompra.Id,"Agregar");
           
                return RedirectToAction("Index", "InsumosXCompras",new { IdCompraGenerada=insumosXCompra.IdCompras });
            }

            ViewBag.IdCompras = new SelectList(db.Compras, "Id", "Observaciones", insumosXCompra.IdCompras);
            ViewBag.IdInsumos = new SelectList(db.Insumos, "Id", "Descripcion", insumosXCompra.IdInsumos);



            return View(insumosXCompra);
        }

        public void UpdateCompra(int idCompras)
        {
           
            Compras compras = db.Compras.Find(idCompras);
            compras.FechaActualizacion = DateTime.Now;

            InsumosXCompra InsumosXCompras = db.InsumosXCompra.Where(x => x.IdCompras == idCompras).FirstOrDefault();
            if (InsumosXCompras != null) {
                compras.Subtotal = db.InsumosXCompra.Where(x => x.IdCompras == idCompras).Sum(x => x.Precio);
                    } 

            else {
                compras.Subtotal = 0;
                     }

            compras.Total = compras.Subtotal - compras.Descuentos;
            
            db.Entry(compras).State = EntityState.Modified;
            db.SaveChanges();


        }

        // GET: InsumosXCompras/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InsumosXCompra insumosXCompra = db.InsumosXCompra.Find(id);
            if (insumosXCompra == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdCompras = new SelectList(db.Compras, "Id", "Observaciones", insumosXCompra.IdCompras);
            ViewBag.IdInsumos = new SelectList(db.Insumos, "Id", "Descripcion", insumosXCompra.IdInsumos);
            return View(insumosXCompra);

        }

        // POST: InsumosXCompras/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,IdCompras,IdInsumos,Cantidad,Precio")] InsumosXCompra insumosXCompra)
        {
            if (ModelState.IsValid)
            {
                db.Entry(insumosXCompra).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdCompras = new SelectList(db.Compras, "Id", "Observaciones", insumosXCompra.IdCompras);
            ViewBag.IdInsumos = new SelectList(db.Insumos, "Id", "Descripcion", insumosXCompra.IdInsumos);
            return View(insumosXCompra);
        }

        // GET: InsumosXCompras/Delete/5
           public ActionResult Delete(int? IdCompraGenerada)
          {
            if (IdCompraGenerada != null)
            {

                Compras compras = db.Compras.Find(IdCompraGenerada);
                ViewBag.NroCompra = compras.Numero;
                ViewBag.FechaCompra = compras.Fecha;

                ViewBag.Subtotal = compras.Subtotal;
                ViewBag.Descuentos = compras.Descuentos;
                ViewBag.Total = compras.Total;
                ViewBag.Observaciones = compras.Observaciones;


                var insumosXCompra = db.InsumosXCompra.Include(i => i.Compras).Include(i => i.Insumos).Where(x => x.IdCompras == IdCompraGenerada);
                return View(insumosXCompra.ToList());

            }
            else
            {
                return RedirectToAction("Index", "Compras");
            }
        } 

        // POST: InsumosXCompras/Delete/5
        [HttpPost, ActionName("Delete")]
          [ValidateAntiForgeryToken]
          public ActionResult DeleteConfirmed(int? IdCompraGenerada)
          {
              Compras compras = db.Compras.Find(IdCompraGenerada);


            List<InsumosXCompra> ListaInsumosXCompra = db.InsumosXCompra.Where(x => x.IdCompras == compras.Id).ToList();
         

            if (ListaInsumosXCompra.Count() > 0) {

                //Valido que ninguno ha sido utilizado en Compras

                InsumosXFabricacion insumosXFabricacion = ValidarExistenciaInsumoEnCompraDeTodosLosInsumos(ListaInsumosXCompra);
                        if (insumosXFabricacion != null)
                        {
                            return RedirectToAction("Index", "InsumosXCompras", new { IdCompraGenerada = compras.Id, InsumoUtilizadoEnElaboracion = insumosXFabricacion.Id });
                        }

               // Elimino uno por uno
                        if (!EliminarInsumoXCompra(ListaInsumosXCompra))
                        {
                            return RedirectToAction("Delete", "InsumosXCompras", new { IdCompraGenerada = compras.Id});
                        }
             }



            if (!EliminarCompra(compras))
            {
                return RedirectToAction("Delete", "InsumosXCompras", new { IdCompraGenerada = compras.Id });

            }
            return RedirectToAction("Index", "Compras");
        }

        private InsumosXFabricacion ValidarExistenciaInsumoEnCompraDeTodosLosInsumos(List<InsumosXCompra> listaInsumosXCompra)
        {
            StringBuilder mensaje = new StringBuilder();
            LogsController logsController = new LogsController();

            Log objetoLog = new Log();
            objetoLog.TipoOperacion = "ValidarExistenciaInsumoEnCompraDeTodosLosInsumos";
            mensaje.AppendLine("Se procede a verificar si algun insumo de la compra no ha sido utilizado para alguna Elaboracion");
            int Acumulador = 0;

            foreach (var item in listaInsumosXCompra)
            {         
                Acumulador++;
                mensaje.AppendLine("Analizando Insumos nro => "+ Acumulador + " de "+ listaInsumosXCompra.Count() +" totales");
                mensaje.AppendLine("Analizando => "+ item.Insumos.Descripcion + "- ID: "+ item.Insumos.Id);
                InsumosXFabricacion insumosXFabricacion = ValidarInsumoUtilizadoEnElaboracion(item);

                if(insumosXFabricacion != null)
                {
                    mensaje.AppendLine("El Insumo pertenece a la compra ha sido utilizado en la Compra Nro "+ insumosXFabricacion.FabricacionProducto.NroElaboracion);
                    mensaje.AppendLine("Se cancela el proceso de Eliminacion");
                    objetoLog.LogMensaje = mensaje.ToString();
                    logsController.Create(objetoLog);

                    return insumosXFabricacion;
                }
                mensaje.AppendLine("El insumo no pertecene a niguna Elaboracion - Costo");
            }

            //Si no retorno nada antes es por que el los insumos no han formado parte del Costo de ninguna compra.
            mensaje.AppendLine("Finalizacion de Validacion. Los Insumos estan OK para ser eliminados");
            objetoLog.LogMensaje = mensaje.ToString();
            logsController.Create(objetoLog);

            return null;
        }

        private bool EliminarCompra(Compras compras)
        {
            StringBuilder mensaje = new StringBuilder();
            LogsController logsController = new LogsController();

            Log objetoLog = new Log();
            objetoLog.TipoOperacion = "EliminarCompra ";

            try
            {
                mensaje.AppendLine("Se procede a borrar la Compra Nro => "+ compras.Numero + ". Con el ID => " +compras.Id);
               
                db.Compras.Remove(compras);
                db.SaveChanges();
            
                mensaje.AppendLine("Finalizo la eliminacion de la Compra Correctamente");

                objetoLog.LogMensaje = mensaje.ToString();

                logsController.Create(objetoLog);
                return true;
            }
            catch
            {
                mensaje.AppendLine("Fallo el metodo EliminarCompra ");
                objetoLog.LogMensaje = mensaje.ToString();

                logsController.Create(objetoLog);

                return false;
            }

        }
       
        private bool EliminarInsumoXCompra(List<InsumosXCompra> ListainsumosXCompras)
        {
            StringBuilder mensaje = new StringBuilder();
            LogsController logsController = new LogsController();

            Log objetoLog = new Log();
            objetoLog.TipoOperacion = "EliminarInsumoXCompra - Recorrido de los insumos";

            try
            {
                int CantidadRegistros = ListainsumosXCompras.Count();

                mensaje.AppendLine("Los insumos de la compra son => " + CantidadRegistros);
                mensaje.AppendLine("Se procese a eliminar los registros...");
                int Contador = 0;

                foreach (var item in ListainsumosXCompras)
                {
                    Contador++;
                    mensaje.AppendLine("Insumo nro :" + Contador + " de => "+ CantidadRegistros + " Registros totales");
                    mensaje.AppendLine("Se procese a eliminar =>" + item.Insumos.Descripcion + " - " + item.Cantidad);
                    StockElaboracionComprasController stockElaboracionCompras = new StockElaboracionComprasController();
                    stockElaboracionCompras.Compra(item.Id, "Quitar");
                    db.InsumosXCompra.Remove(item);
                    db.SaveChanges();
                }

                mensaje.AppendLine("Finalizo la eliminacion de Insumos X Compra correctamente");
    
                objetoLog.LogMensaje = mensaje.ToString();

                logsController.Create(objetoLog);
                return true;
            }
            catch
            {
                mensaje.AppendLine("Fallo el metodo EliminarInsumoXCompra ");
                objetoLog.LogMensaje = mensaje.ToString();

                logsController.Create(objetoLog);

                return false;
            }

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        public ActionResult PreviaCreate(int? IdCompraGenerada, string ObjetoRepetido)
        {

            if(ObjetoRepetido != "") { 
            ViewBag.ObjetoRepetido = ObjetoRepetido;
            }

            if (IdCompraGenerada != null) { 
            ViewBag.IdCompra = IdCompraGenerada;
          
            //ViewBag.IdInsumos = new SelectList(db.Insumos, "Id", "Descripcion");

            List<Insumos> objetos = db.Insumos.Where(x=> x.Estado == true).ToList();

            ViewBag.ListaInsumos = objetos;

            return View();
            }
            else
            {
                return RedirectToAction("Index", "Compras");
            }

        }

        public ActionResult BorrarInsumo(int? id)
        {
            InsumosXCompra insumosXCompra = db.InsumosXCompra.Find(id);

            InsumosXFabricacion insumosXFabricacion = ValidarInsumoUtilizadoEnElaboracion(insumosXCompra);

            if( insumosXFabricacion == null)
            {
                //Stock
                StockElaboracionComprasController stockElaboracionCompras = new StockElaboracionComprasController();
                stockElaboracionCompras.Compra(insumosXCompra.Id, "Quitar");

                //Remover Insumo
                db.InsumosXCompra.Remove(insumosXCompra);
                db.SaveChanges();
                UpdateCompra(insumosXCompra.IdCompras);

                return RedirectToAction("Index", "InsumosXCompras", new { IdCompraGenerada = insumosXCompra.IdCompras });
            }
            else
            {
                return RedirectToAction("Index", "InsumosXCompras", new { IdCompraGenerada = insumosXCompra.IdCompras, InsumoUtilizadoEnElaboracion= insumosXFabricacion.Id });
            }


    
        }

        private InsumosXFabricacion ValidarInsumoUtilizadoEnElaboracion(InsumosXCompra insumosXCompra)
        {
            InsumosXFabricacion insumosXFabricacion = db.InsumosXFabricacion
                .Where(x => x.IdInsumosXCompra == insumosXCompra.Id)
                
                .FirstOrDefault();

            if (insumosXFabricacion == null)
            {
                //No se ha utilizado para Niguna compra, se puede borrar
                return null;
            }
            else
            {
                //Se ha utilizado para una compra, se devuelve el objeto
                return insumosXFabricacion;
            }
        }
    }
}

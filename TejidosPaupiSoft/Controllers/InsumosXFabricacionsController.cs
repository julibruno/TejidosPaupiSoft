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
    public class InsumosXFabricacionsController : Controller
    {
        private TejidosPaupiDBEntities db = new TejidosPaupiDBEntities();

        // GET: InsumosXFabricacions
        public ActionResult Index(int? IdFabricacionGenerada)
        {
            if (IdFabricacionGenerada != null)
            {
                FabricacionProducto fabricacion = db.FabricacionProducto.Where(x => x.Id == IdFabricacionGenerada).FirstOrDefault();
                ViewBag.FabricacionId = fabricacion.Id;
                ViewBag.NroFabricacion = fabricacion.NroElaboracion;
                ViewBag.Producto = fabricacion.Producto.Descripcion;
                ViewBag.Observaciones = fabricacion.Observaciones;
                ViewBag.Costo = fabricacion.Costo;
                ViewBag.PrecioSugerido = fabricacion.PrecioSugerido;

                var insumosXFabricacion = db.InsumosXFabricacion.Where(i => i.IdFabricacionProducto == IdFabricacionGenerada).Include(i => i.FabricacionProducto).Include(i => i.Insumos);
                return View(insumosXFabricacion.ToList());

            }
            else
            {
                return RedirectToAction("Index", "FabricacionProductos");
            }


           
        }

        // GET: InsumosXFabricacions/Details/5
        public ActionResult Details(int? IdFabricacionGenerada)
        {
            if(IdFabricacionGenerada != null)
            {
                FabricacionProducto fabricacion = db.FabricacionProducto.Where(x => x.Id == IdFabricacionGenerada).FirstOrDefault();
                ViewBag.FabricacionId = fabricacion.Id;
                ViewBag.NroFabricacion = fabricacion.NroElaboracion;

                ViewBag.Producto = fabricacion.Producto.Descripcion;
                ViewBag.Observaciones = fabricacion.Observaciones;
                ViewBag.Costo = fabricacion.Costo;
                ViewBag.PrecioSugerido = fabricacion.PrecioSugerido;

                var insumosXFabricacion = db.InsumosXFabricacion.Where(i => i.IdFabricacionProducto == IdFabricacionGenerada).Include(i => i.FabricacionProducto).Include(i => i.Insumos);
                return View(insumosXFabricacion.ToList());

            }
            else
            {
                return RedirectToAction("Index", "FabricacionProductos");
            }
        }

        


        // GET: InsumosXFabricacions/Create
        public ActionResult Create(int idInsumo, int idFabricacion, bool? SinStock, bool? ErrorCalculoCosto)
        {
           
            if(SinStock == true)
            {
                ViewBag.SinStock = "Sin Stock";
            }

            if (ErrorCalculoCosto == true)
            {
                ViewBag.Error = "Ha surgido un error al querer calcular el costo. Reintente más tarde.";
            }

            Insumos insumos = db.Insumos.Find(idInsumo);

            if (!ValidarStockInsumo(insumos))
            {
                
                return RedirectToAction("PreviaCreateFabricacion", "InsumosXFabricacions", new { IdFabricacionGenerada = idFabricacion, InsumoSinStock = insumos.Descripcion });
            }

            if (!ValidarExistenciaInsumoEnElaboracion(idInsumo,idFabricacion))
            {
                
                return RedirectToAction("PreviaCreateFabricacion", "InsumosXFabricacions", new { IdFabricacionGenerada = idFabricacion, ObjetoRepetido = insumos.Descripcion });
            }


            ViewBag.StockActual = insumos.Cantidad;


            InsumosXFabricacion objeto = new InsumosXFabricacion();

            objeto.IdInsumo = idInsumo;
            objeto.IdFabricacionProducto = idFabricacion;

           

            ViewBag.DescripcionInsumo = insumos.Descripcion;
            ViewBag.UnidadMedidaInsumo = insumos.TipoInsumo.UnidadMedida.Descripcion;

            ViewBag.IdFabricacionProducto = new SelectList(db.FabricacionProducto, "Id", "Observaciones");
            ViewBag.IdInsumo = new SelectList(db.Insumos, "Id", "Descripcion");
            return View(objeto);
        }

        private bool ValidarExistenciaInsumoEnElaboracion(int idInsumo, int idFabricacion)
        {
            //Voy a validar si el insumo ya esta en la elaboracion

            InsumosXFabricacion insumosXFabricacion = db.InsumosXFabricacion.Where(x => x.IdFabricacionProducto == idFabricacion && x.IdInsumo == idInsumo).FirstOrDefault();



            if (insumosXFabricacion != null)
            {
                return false;
            }
            return true;

        }

        private bool ValidarStockInsumo(Insumos insumos)
        {
            if(insumos.Cantidad <= 0)
            {
                return false;
            }

            return true;
        }

        // POST: InsumosXFabricacions/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,IdFabricacionProducto,IdInsumo,Cantidad,Costo")] InsumosXFabricacion insumosXFabricacion)
        {
            if (ModelState.IsValid)
            {

                if (!ValidarStockVsCantidadIngresada(insumosXFabricacion))
                {

                    return RedirectToAction("Create", "InsumosXFabricacions", new { IdInsumo = insumosXFabricacion.IdInsumo, idFabricacion = insumosXFabricacion.IdFabricacionProducto, SinStock = true  });
                }


                // Saco el Costo del producto
                 insumosXFabricacion = CalcularCostoInsumo(insumosXFabricacion);
               
                if(insumosXFabricacion.IdInsumosXCompra == null)
                {
                    return RedirectToAction("Create", "InsumosXFabricacions", new { idInsumo= insumosXFabricacion.IdInsumo, idFabricacion = insumosXFabricacion.IdFabricacionProducto, ErrorCalculoCosto = true });
                }


                db.InsumosXFabricacion.Add(insumosXFabricacion);
                db.SaveChanges();

                // Aqui instando el objeto de stock, y quiero del stock la cantidad. 
                StockElaboracionComprasController stockElaboracionCompras = new StockElaboracionComprasController();

                stockElaboracionCompras.Elaboracion(insumosXFabricacion.Id,"Agregar");

                //Actualizar el costo de la Elaboracion

                ActualizarCostoElaboracion(insumosXFabricacion.IdFabricacionProducto);

                // actualizar el precio sugerido

                ActualizarPrecioSugerido(insumosXFabricacion.IdFabricacionProducto);


                return RedirectToAction("Index", "InsumosXFabricacions", new { IdFabricacionGenerada = insumosXFabricacion.IdFabricacionProducto });
            }

            ViewBag.IdFabricacionProducto = new SelectList(db.FabricacionProducto, "Id", "Observaciones", insumosXFabricacion.IdFabricacionProducto);
            ViewBag.IdInsumo = new SelectList(db.Insumos, "Id", "Descripcion", insumosXFabricacion.IdInsumo);
            return View(insumosXFabricacion);
        }

        private bool ValidarStockVsCantidadIngresada(InsumosXFabricacion objeto)
        {
            int StockActual = db.Insumos.Where(x => x.Id == objeto.IdInsumo).Select(x => x.Cantidad).FirstOrDefault();

            if(objeto.Cantidad > StockActual)
            {
                return false;
            }

            return true;

        }



        private void ActualizarPrecioSugerido(int idFabricacionProducto)
        {
            StringBuilder mensaje = new StringBuilder();
            LogsController logsController = new LogsController();
            Log objetoLog = new Log();
            objetoLog.TipoOperacion = "Actualizar Precio Sugerido - InsumosXFabricaciones";

            try
            {

                int variableMultiplicadora = 3;

                FabricacionProducto fabricacionProducto = db.FabricacionProducto.Find(idFabricacionProducto);

                mensaje.AppendLine("Se procede a Actualizar el  Precio Sugerido  de Elaboracion para la Fabricacion con id " + fabricacionProducto.Id);

                mensaje.AppendLine("Producto => " + fabricacionProducto.Producto.Descripcion);

                InsumosXFabricacion insumosXFabricacion = db.InsumosXFabricacion.Where(x => x.IdFabricacionProducto == idFabricacionProducto).FirstOrDefault();

                fabricacionProducto.PrecioSugerido = variableMultiplicadora * fabricacionProducto.Costo;


                mensaje.AppendLine("El Costo es de =>" + fabricacionProducto.Costo);
                mensaje.AppendLine("El Precio Sugerido es de =>" + fabricacionProducto.PrecioSugerido);

                objetoLog.LogMensaje = mensaje.ToString();
                logsController.Create(objetoLog);

                //Guardo el objeto con el nuevo Costo
                db.Entry(fabricacionProducto).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {
                mensaje.AppendLine("Error en el metodo ActualizarPrecioSugerido. IdFabricacionProducto" + idFabricacionProducto);
                logsController.Create(objetoLog);
            }


        }

        private void ActualizarCostoElaboracion(int IdFabricacionProducto)
        {
            StringBuilder mensaje = new StringBuilder();
            LogsController logsController = new LogsController();
            Log objetoLog = new Log();
            objetoLog.TipoOperacion = "ActualizarCostoElaboracion - InsumosXFabricaciones";
            
            try
            { 
            FabricacionProducto fabricacionProducto = db.FabricacionProducto.Find(IdFabricacionProducto);

            mensaje.AppendLine("Se procede a Actualizar el Costo de Elaboracion para la Fabricacion con id " + fabricacionProducto.Id);

            mensaje.AppendLine("Producto => " + fabricacionProducto.Producto.Descripcion);

            InsumosXFabricacion insumosXFabricacion = db.InsumosXFabricacion.Where(x => x.IdFabricacionProducto == IdFabricacionProducto).FirstOrDefault();

            if(insumosXFabricacion != null)
            {
                fabricacionProducto.Costo = db.InsumosXFabricacion.Where(x => x.IdFabricacionProducto == IdFabricacionProducto).Sum(x => x.Costo);
                
                mensaje.AppendLine("HAY registros en InsumosXFabricacion. El nuevo Costo es =>" + fabricacionProducto.Costo);

            }
            else
            {
                mensaje.AppendLine("NO hay registro en InsumosXFabricacion. El nuevo Costo es => 0");

                fabricacionProducto.Costo = 0;
            }

                objetoLog.LogMensaje = mensaje.ToString();
                logsController.Create(objetoLog);

                //Guardo el objeto con el nuevo Costo
                db.Entry(fabricacionProducto).State = EntityState.Modified;
            db.SaveChanges();
            }
            catch
            {
                mensaje.AppendLine("Error en el metodo ActualizarCostoElaboracion. IdFabricacionProducto" + IdFabricacionProducto);
                logsController.Create(objetoLog);
            }


        }

        private InsumosXFabricacion CalcularCostoInsumo(InsumosXFabricacion insumosXFabricacion)
        {

            StringBuilder mensaje = new StringBuilder();
            LogsController logsController = new LogsController();

            Log objetoLog = new Log();
            objetoLog.TipoOperacion = "Calcular Costo Insumo - InsumosXFabricaciones";
            

            try { 
            mensaje.AppendLine("Se procede a Calcular el costo del insumo => " +insumosXFabricacion.IdInsumo+")");
             //Ultima compra para ese insumo

            InsumosXCompra insumosXCompraUltimaFecha = db.InsumosXCompra.Where(x => x.Compras.Estado == true && x.IdInsumos == insumosXFabricacion.IdInsumo).OrderByDescending(x => x.Compras.Fecha).FirstOrDefault();

            mensaje.AppendLine("Ultima compra generada segun calendario=>" + insumosXCompraUltimaFecha.Compras.Fecha );

            mensaje.AppendLine("Compra Nro=>" + insumosXCompraUltimaFecha.Compras.Numero);

            mensaje.AppendLine("Calculando el Costo por "+ insumosXFabricacion.Cantidad );


                insumosXFabricacion.IdInsumosXCompra = insumosXCompraUltimaFecha.Id;


                //Regla de 3 simple en base a la cantidad que se va a Fabricar

             int Costo = Convert.ToInt32((insumosXFabricacion.Cantidad * insumosXCompraUltimaFecha.Precio) / insumosXCompraUltimaFecha.Cantidad);

            mensaje.AppendLine("Si "+ insumosXCompraUltimaFecha.Cantidad+ " son $"+ insumosXCompraUltimaFecha.Precio+ ". Entonces "+ insumosXFabricacion.Cantidad + " son $"+ Costo);

                insumosXFabricacion.Costo = Costo;

                

                objetoLog.LogMensaje = mensaje.ToString();

                logsController.Create(objetoLog);

                return insumosXFabricacion;
            }
            catch
            {
                mensaje.AppendLine("Surgio un error en la funcion CalcularCostoInsumo(IdInsumosXFab - Producto)" + insumosXFabricacion.Id +" - " + insumosXFabricacion.IdInsumo );

                objetoLog.LogMensaje = mensaje.ToString();

                logsController.Create(objetoLog);

                return insumosXFabricacion;
            }
        }

   
        // GET: InsumosXFabricacions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InsumosXFabricacion insumosXFabricacion = db.InsumosXFabricacion.Find(id);
            if (insumosXFabricacion == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdFabricacionProducto = new SelectList(db.FabricacionProducto, "Id", "Observaciones", insumosXFabricacion.IdFabricacionProducto);
            ViewBag.IdInsumo = new SelectList(db.Insumos, "Id", "Descripcion", insumosXFabricacion.IdInsumo);
            return View(insumosXFabricacion);
        }

        // POST: InsumosXFabricacions/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,IdFabricacionProducto,IdInsumo,Cantidad,Costo")] InsumosXFabricacion insumosXFabricacion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(insumosXFabricacion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdFabricacionProducto = new SelectList(db.FabricacionProducto, "Id", "Observaciones", insumosXFabricacion.IdFabricacionProducto);
            ViewBag.IdInsumo = new SelectList(db.Insumos, "Id", "Descripcion", insumosXFabricacion.IdInsumo);
            return View(insumosXFabricacion);
        }

        // GET: InsumosXFabricacions/Delete/5
        public ActionResult Delete(int? IdFabricacionGenerada)
        {
            if (IdFabricacionGenerada != null)
            {
                FabricacionProducto fabricacion = db.FabricacionProducto.Where(x => x.Id == IdFabricacionGenerada).FirstOrDefault();
                ViewBag.FabricacionId = fabricacion.Id;
                ViewBag.NroFabricacion = fabricacion.NroElaboracion;

                ViewBag.Producto = fabricacion.Producto.Descripcion;
                ViewBag.Observaciones = fabricacion.Observaciones;
                ViewBag.Costo = fabricacion.Costo;
                ViewBag.PrecioSugerido = fabricacion.PrecioSugerido;

                var insumosXFabricacion = db.InsumosXFabricacion.Where(i => i.IdFabricacionProducto == IdFabricacionGenerada).Include(i => i.FabricacionProducto).Include(i => i.Insumos);
                return View(insumosXFabricacion.ToList());

            }
            else
            {
                return RedirectToAction("Index", "FabricacionProductos");
            }
        }

        // POST: InsumosXFabricacions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int IdFabricacionGenerada)
        {
            
            InsumosXFabricacion insumosXFabricacion = db.InsumosXFabricacion.Find(IdFabricacionGenerada);

            if (!ValidarExistenciaProductoVendido(IdFabricacionGenerada))
            {
                return RedirectToAction("Delete", "InsumosXFabricacions", new { IdFabricacionGenerada = insumosXFabricacion.IdFabricacionProducto });

            }


            if (!EliminarInsumoXFabricacion(IdFabricacionGenerada))
            {
                return RedirectToAction("Delete", "InsumosXFabricacions", new { IdFabricacionGenerada = insumosXFabricacion.IdFabricacionProducto });
            }

            if (!EliminarFabricacion(IdFabricacionGenerada))
            {
                return RedirectToAction("Delete", "InsumosXFabricacions", new { IdFabricacionGenerada = insumosXFabricacion.IdFabricacionProducto });

            }
            return RedirectToAction("Index", "FabricacionProductos");
        }

        private bool ValidarExistenciaProductoVendido(int idFabricacionGenerada)
        {
            Venta venta = db.Venta.Where(x => x.IdFabricacionProducto == idFabricacionGenerada).FirstOrDefault();

            if(venta == null)
            {
                //No hay venta
                return true;
            }

            return false;
        }

        private bool EliminarFabricacion(int idFabricacionGenerada)
        {
            StringBuilder mensaje = new StringBuilder();
            LogsController logsController = new LogsController();

            Log objetoLog = new Log();
            objetoLog.TipoOperacion = "EliminarInsumoXFabricacion - InsumosXFabricaciones";

            try
            {
                FabricacionProducto fabricacionProducto = db.FabricacionProducto.Find(idFabricacionGenerada);

                mensaje.AppendLine("Se procede a eliminar la Fabricacion con ID: " + idFabricacionGenerada);

                mensaje.AppendLine("Producto => " + fabricacionProducto.Producto.Descripcion);


                db.FabricacionProducto.Remove(fabricacionProducto);
                db.SaveChanges();

                mensaje.AppendLine("Finalizo el Borrado de Insumos y Fabricacion");

                objetoLog.LogMensaje = mensaje.ToString();

                logsController.Create(objetoLog);
                return true;
            }
            catch
            { 
                mensaje.AppendLine("Fallo la el metodo de EliminarFabricacion " + idFabricacionGenerada);
                objetoLog.LogMensaje = mensaje.ToString();

                logsController.Create(objetoLog);
                return false;
            }



        }

        private bool EliminarInsumoXFabricacion(int idFabricacionGenerada)
        {
            StringBuilder mensaje = new StringBuilder();
            LogsController logsController = new LogsController();

            Log objetoLog = new Log();
            objetoLog.TipoOperacion = "EliminarInsumoXFabricacion - InsumosXFabricaciones";


            try
            { 
         

            List<InsumosXFabricacion> ListaInsumosXFabricacions = db.InsumosXFabricacion.Where(x => x.IdFabricacionProducto == idFabricacionGenerada).ToList();

            int CantidadInsumos = ListaInsumosXFabricacions.Count();

            mensaje.AppendLine("Los insumos de la compra son => " + CantidadInsumos);


            if (CantidadInsumos > 0)
            {
                mensaje.AppendLine("Se procese a eliminar los registros...");
                for (int i = 0; i < CantidadInsumos; i++)
                {
                    InsumosXFabricacion insumosXFabricacion =  db.InsumosXFabricacion.Where(x => x.IdFabricacionProducto == idFabricacionGenerada).FirstOrDefault();
                     mensaje.AppendLine("Se procese a eliminar =>" +insumosXFabricacion.Insumos.Descripcion +" - "+insumosXFabricacion.Cantidad);
                    
                    StockElaboracionComprasController stockElaboracionCompras = new StockElaboracionComprasController();
                    stockElaboracionCompras.Elaboracion(insumosXFabricacion.Id, "Quitar");

                    db.InsumosXFabricacion.Remove(insumosXFabricacion);
                    db.SaveChanges();

                    int registros = CantidadInsumos - (i+1);

                    mensaje.AppendLine("Queda por eliminar=> " +registros);


                }
                mensaje.AppendLine("Finalizo la eliminacion");

            }
            //Actualizo el Nuevo costo
            ActualizarCostoElaboracion(idFabricacionGenerada);

            //Actualizo el Nuevo Precio Sugerido
            ActualizarPrecioSugerido(idFabricacionGenerada);
                objetoLog.LogMensaje = mensaje.ToString();

                logsController.Create(objetoLog);
                return true;
            }
            catch
            {
                mensaje.AppendLine("Fallo el metodo EliminarInsumoXFabricacion - " +idFabricacionGenerada );
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


        public ActionResult PreviaCreateFabricacion(int? IdFabricacionGenerada, string ObjetoRepetido, string InsumoSinStock)
        {

            if (ObjetoRepetido != "")
            {
                ViewBag.ObjetoRepetido = ObjetoRepetido;
            }

            if (InsumoSinStock != "")
            {
                ViewBag.InsumoSinStock = InsumoSinStock;
            }
            if (IdFabricacionGenerada != null)
            {
                ViewBag.IdFabricacion = IdFabricacionGenerada;

                List<Insumos> objetos = db.Insumos.Where(x => x.Estado == true).ToList();

                ViewBag.ListaInsumos = objetos;
                //FabricacionProducto fabricacion = db.FabricacionProducto.Where(x => x.Id == IdFabricacionGenerada).FirstOrDefault();

                return View();

            }
            else
            {
                return RedirectToAction("Index", "FabricacionProductos");
            }
        }


        public ActionResult BorrarInsumo(int? id)
        {
            InsumosXFabricacion insumosXFabricacion = db.InsumosXFabricacion.Find(id);
            // Vuelvo a colocar el stock como estaba
            StockElaboracionComprasController stockElaboracionCompras = new StockElaboracionComprasController();
            stockElaboracionCompras.Elaboracion(insumosXFabricacion.Id, "Quitar");

            //Remuevo el insumo de la fabricacion
            db.InsumosXFabricacion.Remove(insumosXFabricacion);
            db.SaveChanges();

            //Actualizo el Nuevo costo
            ActualizarCostoElaboracion(insumosXFabricacion.IdFabricacionProducto);

            //Actualizo el Nuevo Precio Sugerido
            ActualizarPrecioSugerido(insumosXFabricacion.IdFabricacionProducto);
            //retorno a la fabricacion
            return RedirectToAction("Index", "InsumosXFabricacions", new { IdFabricacionGenerada = insumosXFabricacion.IdFabricacionProducto });
        }
    }
}

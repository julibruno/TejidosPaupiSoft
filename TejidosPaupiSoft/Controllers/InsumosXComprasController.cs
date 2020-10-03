using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TejidosPaupiSoft.Models;

namespace TejidosPaupiSoft.Controllers
{
    public class InsumosXComprasController : Controller
    {
        private TejidosPaupiDBEntities db = new TejidosPaupiDBEntities();

        // GET: InsumosXCompras
        public ActionResult Index(int? IdCompraGenerada)
        {
            if(IdCompraGenerada != null) { 
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
        public ActionResult Details(int? id)
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
            return View(insumosXCompra);
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
                UpdateCompra(insumosXCompra.IdCompras);
           
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
        public ActionResult Delete(int? id)
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
            return View(insumosXCompra);
        }

        // POST: InsumosXCompras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            InsumosXCompra insumosXCompra = db.InsumosXCompra.Find(id);
            db.InsumosXCompra.Remove(insumosXCompra);
            db.SaveChanges();
            return RedirectToAction("Index");
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
            db.InsumosXCompra.Remove(insumosXCompra);
            db.SaveChanges();
            UpdateCompra(insumosXCompra.IdCompras);

            return RedirectToAction("Index", "InsumosXCompras", new { IdCompraGenerada = insumosXCompra.IdCompras});
        }

     

    }
}

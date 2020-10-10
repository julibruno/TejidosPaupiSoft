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
    public class FabricacionProductosController : Controller
    {
        private TejidosPaupiDBEntities db = new TejidosPaupiDBEntities();

        // GET: FabricacionProductos
        public ActionResult Index()
        {
            var fabricacionProducto = db.FabricacionProducto.Include(f => f.Producto);
            return View(fabricacionProducto.ToList());
        }

        // GET: FabricacionProductos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FabricacionProducto fabricacionProducto = db.FabricacionProducto.Find(id);
            if (fabricacionProducto == null)
            {
                return HttpNotFound();
            }
            return View(fabricacionProducto);
        }

        // GET: FabricacionProductos/Create
        public ActionResult Create()
        {
            ViewBag.IdProducto = new SelectList(db.Producto, "Id", "Descripcion");
            return View();
        }

        // POST: FabricacionProductos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,IdProducto,Observaciones,Costo,PrecioSugerido,FechaCreacion,FechaActualizacion,Estado")] FabricacionProducto fabricacionProducto)
        {
            if (ModelState.IsValid)
            {
                db.FabricacionProducto.Add(fabricacionProducto);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdProducto = new SelectList(db.Producto, "Id", "Descripcion", fabricacionProducto.IdProducto);
            return View(fabricacionProducto);
        }

        // GET: FabricacionProductos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FabricacionProducto fabricacionProducto = db.FabricacionProducto.Find(id);
            if (fabricacionProducto == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdProducto = new SelectList(db.Producto.Where(x=>x.Estado == true), "Id", "Descripcion", fabricacionProducto.IdProducto);
            return View(fabricacionProducto);
        }

        // POST: FabricacionProductos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,IdProducto,Observaciones,Costo,PrecioSugerido,FechaCreacion,FechaActualizacion,Estado,NroElaboracion")] FabricacionProducto fabricacionProducto)
        {
            if (ModelState.IsValid)
            {
                fabricacionProducto.FechaActualizacion = DateTime.Now;
                db.Entry(fabricacionProducto).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "InsumosXFabricacions", new { IdFabricacionGenerada = fabricacionProducto.Id });

            }
            ViewBag.IdProducto = new SelectList(db.Producto, "Id", "Descripcion", fabricacionProducto.IdProducto);
            return View(fabricacionProducto);
        }

        // GET: FabricacionProductos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FabricacionProducto fabricacionProducto = db.FabricacionProducto.Find(id);
            if (fabricacionProducto == null)
            {
                return HttpNotFound();
            }
            return View(fabricacionProducto);
        }

        // POST: FabricacionProductos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FabricacionProducto fabricacionProducto = db.FabricacionProducto.Find(id);
            db.FabricacionProducto.Remove(fabricacionProducto);
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

        public ActionResult IndexParaVenta()
        {
            // Retorno los Productos fabricados que tienen insumos. Es decir que tienen Costo y PrecioSugerido.
            return View(db.FabricacionProducto.Where(x => x.Estado == true && x.Costo > 0 && x.PrecioSugerido>0).ToList());
        }
    }
}

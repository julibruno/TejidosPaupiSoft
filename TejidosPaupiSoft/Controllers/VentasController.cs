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
    public class VentasController : Controller
    {
        private TejidosPaupiDBEntities db = new TejidosPaupiDBEntities();

        // GET: Ventas
        public ActionResult Index()
        {
            var venta = db.Venta.Include(v => v.FabricacionProducto);
            return View(venta.ToList());
        }

        // GET: Ventas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Venta venta = db.Venta.Find(id);
            if (venta == null)
            {
                return HttpNotFound();
            }
            return View(venta);
        }

        // GET: Ventas/Create
        public ActionResult Create(int? idFabricacionSeleccionada)
        {
            if(idFabricacionSeleccionada != null)
            {
                Venta venta = new Venta();
                venta.Fecha = DateTime.Now;
                venta.IdFabricacionProducto = idFabricacionSeleccionada.GetValueOrDefault();

                FabricacionProducto fabricacionProducto = db.FabricacionProducto.Find(idFabricacionSeleccionada);

                venta.FabricacionProducto= fabricacionProducto;
               


                //ViewBag.IdFabricacionProducto = new SelectList(db.FabricacionProducto, "Id", "Observaciones");
                return View(venta);
            }
            else {


                return View("Index");
            }
        }

        // POST: Ventas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,NroVenta,Fecha,IdFabricacionProducto,Cliente,PrecioFinal,Observaciones,FechaCreacion,FechaActualizacion")] Venta venta)
        {
            if (ModelState.IsValid)
            {
                venta.FechaCreacion = DateTime.Now;
                venta.NroVenta = UltimaVenta();

                db.Venta.Add(venta);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdFabricacionProducto = new SelectList(db.FabricacionProducto, "Id", "Observaciones", venta.IdFabricacionProducto);
            return View(venta);
        }

        private int UltimaVenta()
        {
            int Ultimo = 0;

            if (db.Venta.FirstOrDefault() == null)
            {
                Ultimo = 1;
            }
            else
            {
                Ultimo = db.Venta.Max(x => x.NroVenta);
                Ultimo++;
            }

            return Ultimo;

        }

        // GET: Ventas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Venta venta = db.Venta.Find(id);
            if (venta == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdFabricacionProducto = new SelectList(db.FabricacionProducto, "Id", "Observaciones", venta.IdFabricacionProducto);
            return View(venta);
        }

        // POST: Ventas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,NroVenta,Fecha,IdFabricacionProducto,Cliente,PrecioFinal,Observaciones,FechaCreacion,FechaActualizacion")] Venta venta)
        {
            if (ModelState.IsValid)
            {
                db.Entry(venta).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdFabricacionProducto = new SelectList(db.FabricacionProducto, "Id", "Observaciones", venta.IdFabricacionProducto);
            return View(venta);
        }

        // GET: Ventas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Venta venta = db.Venta.Find(id);
            if (venta == null)
            {
                return HttpNotFound();
            }
            return View(venta);
        }

        // POST: Ventas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Venta venta = db.Venta.Find(id);
            db.Venta.Remove(venta);
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
    }
}

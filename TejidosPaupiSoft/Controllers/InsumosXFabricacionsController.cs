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
                ViewBag.Producto = fabricacion.Producto.Descripcion;
                ViewBag.Observaciones = fabricacion.Observaciones;
                ViewBag.Costo = fabricacion.Costo;
                ViewBag.PrecioSugerido = fabricacion.PrecioSugerido;

                var insumosXFabricacion = db.InsumosXFabricacion.Include(i => i.FabricacionProducto).Include(i => i.Insumos);
                return View(insumosXFabricacion.ToList());

            }
            else
            {
                return RedirectToAction("Index", "FabricacionProductos");
            }


           
        }

        // GET: InsumosXFabricacions/Details/5
        public ActionResult Details(int? id)
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
            return View(insumosXFabricacion);
        }

        // GET: InsumosXFabricacions/Create
        public ActionResult Create(int idInsumo, int idFabricacion)
        {
            InsumosXFabricacion objeto = new InsumosXFabricacion();

            objeto.IdInsumo = idInsumo;
            objeto.IdFabricacionProducto = idFabricacion;



            ViewBag.IdFabricacionProducto = new SelectList(db.FabricacionProducto, "Id", "Observaciones");
            ViewBag.IdInsumo = new SelectList(db.Insumos, "Id", "Descripcion");
            return View();
        }

        // POST: InsumosXFabricacions/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,IdFabricacionProducto,IdInsumo,Cantidad")] InsumosXFabricacion insumosXFabricacion)
        {
            if (ModelState.IsValid)
            {
                db.InsumosXFabricacion.Add(insumosXFabricacion);
                db.SaveChanges();
                // Aca falta hacer un update de los stocks
                //Actualizar el costo del producto
                // actualizar el precio sugerido


                return RedirectToAction("Index", "InsumosXFabricacions", new { IdFabricacionGenerada = insumosXFabricacion.IdFabricacionProducto });
            }

            ViewBag.IdFabricacionProducto = new SelectList(db.FabricacionProducto, "Id", "Observaciones", insumosXFabricacion.IdFabricacionProducto);
            ViewBag.IdInsumo = new SelectList(db.Insumos, "Id", "Descripcion", insumosXFabricacion.IdInsumo);
            return View(insumosXFabricacion);
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
        public ActionResult Edit([Bind(Include = "Id,IdFabricacionProducto,IdInsumo,Cantidad")] InsumosXFabricacion insumosXFabricacion)
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
        public ActionResult Delete(int? id)
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
            return View(insumosXFabricacion);
        }

        // POST: InsumosXFabricacions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            InsumosXFabricacion insumosXFabricacion = db.InsumosXFabricacion.Find(id);
            db.InsumosXFabricacion.Remove(insumosXFabricacion);
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


        public ActionResult PreviaCreateFabricacion(int? IdFabricacionGenerada, string ObjetoRepetido)
        {

            if (ObjetoRepetido != "")
            {
                ViewBag.ObjetoRepetido = ObjetoRepetido;
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
    }
}

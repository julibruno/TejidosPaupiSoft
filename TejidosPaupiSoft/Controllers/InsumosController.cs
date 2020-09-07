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
    public class InsumosController : Controller
    {
        private TejidosPaupiDBEntities db = new TejidosPaupiDBEntities();

        public JsonResult ObtenerMoneda(int idPais)
        {
            string moneda = "1";

            return Json(moneda, JsonRequestBehavior.AllowGet);
        }

        public bool VerificarObjetoRepetido(Insumos objeto)
        {
            Insumos objeto1 = db.Insumos.Where(x => x.Descripcion == objeto.Descripcion && x.Estado == true).FirstOrDefault();

            if (objeto1 == null)
            { //No existe
                return true;
            }
            else
            {//Existe
                return false;
            }

        }

        // GET: Insumos
        public ActionResult Index()
        {
            var insumos = db.Insumos.Include(i => i.TipoInsumo).Include(i => i.TipoLana).Where(x=>x.Estado==true);
            return View(insumos.ToList());
        }

        // GET: Insumos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insumos insumos = db.Insumos.Find(id);
            if (insumos == null)
            {
                return HttpNotFound();
            }
            return View(insumos);
        }

        // GET: Insumos/Create
        public ActionResult Create()
        {
            ViewBag.IdTipoInsumo = new SelectList(db.TipoInsumo.Where(x=>x.Estado==true), "Id", "Descripcion");
            ViewBag.IdTipoLana = new SelectList(db.TipoLana.Where(x => x.Estado == true), "Id", "Descripcion");
            return View();
        }

        // POST: Insumos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Descripcion,IdTipoInsumo,IdTipoLana,Color,Cantidad,Observaciones,FechaCreacion,FechaActualizacion,Estado")] Insumos insumos)
        {
            if (ModelState.IsValid)
            {

                if (!VerificarObjetoRepetido(insumos))
                {
                    ViewBag.ObjetoRepetido = insumos.Descripcion;
                    ViewBag.IdTipoInsumo = new SelectList(db.TipoInsumo.Where(x => x.Estado == true), "Id", "Descripcion");
                    ViewBag.IdTipoLana = new SelectList(db.TipoLana.Where(x => x.Estado == true), "Id", "Descripcion");
                }
                else
                {
                  
                    insumos.Descripcion = ColocarDescripcion(insumos);
                    insumos.Estado = true;
                    insumos.FechaCreacion = DateTime.Now;
                    insumos.Cantidad = 0;
                    
                    db.Insumos.Add(insumos);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            ViewBag.IdTipoInsumo = new SelectList(db.TipoInsumo.Where(x => x.Estado == true), "Id", "Descripcion");
            ViewBag.IdTipoLana = new SelectList(db.TipoLana.Where(x => x.Estado == true), "Id", "Descripcion");
            return View(insumos);
        }

        public string ColocarDescripcion(Insumos insumos)
        {
            string TipoLanaDescripcion = insumos.IdTipoLana == null ? "" : db.TipoLana.Where(x => x.Id == insumos.IdTipoLana).Select(x => x.Descripcion).FirstOrDefault();
            string Color = insumos.Color == null ? "" : insumos.Color;
            string TipoInsumosDescripcion = db.TipoInsumo.Where(x => x.Id == insumos.IdTipoInsumo).Select(x => x.Descripcion).FirstOrDefault();

            string Descripcion = TipoInsumosDescripcion + " - " + TipoLanaDescripcion + " - " + Color;
            return Descripcion;
        }


        // GET: Insumos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insumos insumos = db.Insumos.Find(id);
            if (insumos == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdTipoInsumo = new SelectList(db.TipoInsumo, "Id", "Descripcion", insumos.IdTipoInsumo);
            ViewBag.IdTipoLana = new SelectList(db.TipoLana, "Id", "Descripcion", insumos.IdTipoLana);
    
            return View(insumos);
        }

        // POST: Insumos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Descripcion,IdTipoInsumo,IdTipoLana,Color,Cantidad,Observaciones,FechaCreacion,FechaActualizacion,Estado")] Insumos insumos)
        {
            ViewBag.IdTipoInsumo = new SelectList(db.TipoInsumo, "Id", "Descripcion", insumos.IdTipoInsumo);
            ViewBag.IdTipoLana = new SelectList(db.TipoLana, "Id", "Descripcion", insumos.IdTipoLana);
            if (ModelState.IsValid)
            {
                if (!VerificarObjetoRepetido(insumos))
                {
                    ViewBag.ObjetoRepetido = insumos.Descripcion;
                    return View();
                }
                else
                {
                    Insumos insumosAnterior = db.Insumos.Find(insumos.Id);
                    //Lo anterior queda igual (Datos que no esta en el form, por eso vienen en null del modelo)
                    insumos.FechaCreacion = insumosAnterior.FechaCreacion;
                    insumos.Estado = true;
                    insumos.Cantidad = insumosAnterior.Cantidad;
                    //Lo nuevo
                    insumos.FechaActualizacion = DateTime.Now;
                    insumos.Descripcion = ColocarDescripcion(insumos);
                    

                    db.Entry(insumos).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
           
            return View(insumos);
        }

        // GET: Insumos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insumos insumos = db.Insumos.Find(id);
            if (insumos == null)
            {
                return HttpNotFound();
            }
            return View(insumos);
        }

        // POST: Insumos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Insumos insumos = db.Insumos.Find(id);
            insumos.Estado = false;
            //db.Insumos.Remove(insumos);
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

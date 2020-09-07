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
    public class TipoLanasController : Controller
    {
        private TejidosPaupiDBEntities db = new TejidosPaupiDBEntities();




        public bool VerificarObjetoRepetido(TipoLana objeto)
        {
            TipoLana TipoLana1 = db.TipoLana.Where(x => x.Descripcion == objeto.Descripcion && x.Estado == true).FirstOrDefault();

            if (TipoLana1 == null)
            { //No existe
                return true;
            }
            else
            {//Existe
                return false;
            }

        }
        // GET: TipoLanas
        public ActionResult Index()
        {
            return View(db.TipoLana.ToList().Where(x=>x.Estado==true));
        }

        // GET: TipoLanas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoLana tipoLana = db.TipoLana.Find(id);
            if (tipoLana == null)
            {
                return HttpNotFound();
            }
            return View(tipoLana);
        }

        // GET: TipoLanas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TipoLanas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Descripcion,Estado")] TipoLana tipoLana)
        {
            if (ModelState.IsValid)
            {

                if (!VerificarObjetoRepetido(tipoLana))
                {
                    ViewBag.ObjetoRepetido = tipoLana.Descripcion;
                    return View();
                }
                else
                {
                    tipoLana.Estado = true;
                    db.TipoLana.Add(tipoLana);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            return View(tipoLana);
        }

        // GET: TipoLanas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoLana tipoLana = db.TipoLana.Find(id);
            if (tipoLana == null)
            {
                return HttpNotFound();
            }
            return View(tipoLana);
        }

        // POST: TipoLanas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Descripcion,Estado")] TipoLana tipoLana)
        {
            if (ModelState.IsValid)
            {
                if (!VerificarObjetoRepetido(tipoLana))
                {
                    ViewBag.ObjetoRepetido = tipoLana.Descripcion;
                    return View();
                }
                else
                {
                    tipoLana.Estado = true;
                    db.Entry(tipoLana).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(tipoLana);
        }

        // GET: TipoLanas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoLana tipoLana = db.TipoLana.Find(id);
            if (tipoLana == null)
            {
                return HttpNotFound();
            }
            return View(tipoLana);
        }

        // POST: TipoLanas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TipoLana tipoLana = db.TipoLana.Find(id);

            Insumos insumos = db.Insumos.Where(x => x.IdTipoInsumo == tipoLana.Id).FirstOrDefault();

            if (insumos == null)
            {
                db.TipoLana.Remove(tipoLana);
            }
            else
            {
                tipoLana.Estado = false;
            }
           
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

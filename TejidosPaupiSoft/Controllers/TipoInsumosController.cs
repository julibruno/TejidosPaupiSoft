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
    public class TipoInsumosController : Controller
    {
        private TejidosPaupiDBEntities db = new TejidosPaupiDBEntities();


        public bool VerificarObjetoRepetido(TipoInsumo objeto)
        {
            TipoInsumo TipoInsumo1 = db.TipoInsumo.Where(x => x.Descripcion == objeto.Descripcion && x.Estado == true).FirstOrDefault();

            if (TipoInsumo1 == null)
            { //No existe
                return true;
            }
            else
            {//Existe
                return false;
            }

        }

        // GET: TipoInsumos
        public ActionResult Index()
        {
            var tipoInsumo = db.TipoInsumo.Include(t => t.UnidadMedida);
            return View(tipoInsumo.ToList());
        }

        // GET: TipoInsumos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoInsumo tipoInsumo = db.TipoInsumo.Find(id);
            if (tipoInsumo == null)
            {
                return HttpNotFound();
            }
            return View(tipoInsumo);
        }

        // GET: TipoInsumos/Create
        public ActionResult Create()
        {
            ViewBag.IdUnidadMedida = new SelectList(db.UnidadMedida.Where(x => x.Estado == true), "Id", "Descripcion");
            return View();
        }

        // POST: TipoInsumos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Descripcion,IdUnidadMedida,Estado")] TipoInsumo tipoInsumo)
        {
            if (ModelState.IsValid)
                {

                    if (!VerificarObjetoRepetido(tipoInsumo))
                    {
                        ViewBag.ObjetoRepetido = tipoInsumo.Descripcion;
                        ViewBag.IdUnidadMedida = new SelectList(db.UnidadMedida.Where(x => x.Estado == true), "Id", "Descripcion", tipoInsumo.IdUnidadMedida);
                    }
                    else
                    {
                        tipoInsumo.Estado = true;
                        db.TipoInsumo.Add(tipoInsumo);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }

                }

            ViewBag.IdUnidadMedida = new SelectList(db.UnidadMedida.Where(x => x.Estado == true), "Id", "Descripcion", tipoInsumo.IdUnidadMedida);
            return View(tipoInsumo);
        }

        // GET: TipoInsumos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoInsumo tipoInsumo = db.TipoInsumo.Find(id);
            if (tipoInsumo == null)
            {
                return HttpNotFound();
            }

            //int idUnidadMedida1 = db.UnidadMedida.Where(x => x.Id == tipoInsumo.IdUnidadMedida).Select(x => x.Id).FirstOrDefault(); ;


            ViewBag.IdUnidadMedida = new SelectList(db.UnidadMedida.Where(x=>x.Estado==true), "Id", "Descripcion", db.UnidadMedida.Where(x => x.Id == tipoInsumo.IdUnidadMedida && x.Estado==true).Select(x => x.Id).FirstOrDefault());
            return View(tipoInsumo);
        }

        // POST: TipoInsumos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Descripcion,IdUnidadMedida,Estado")] TipoInsumo tipoInsumo)
        {
            ViewBag.IdUnidadMedida = new SelectList(db.UnidadMedida.Where(x => x.Estado == true), "Id", "Descripcion", tipoInsumo.IdUnidadMedida);
            if (ModelState.IsValid)
            {
                if (!VerificarObjetoRepetido(tipoInsumo))
                {
                    ViewBag.ObjetoRepetido = tipoInsumo.Descripcion;
                    return View();
                }
                else
                {
                    tipoInsumo.Estado = true;
                    db.Entry(tipoInsumo).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

            }
            
            return View(tipoInsumo);
        }

        // GET: TipoInsumos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoInsumo tipoInsumo = db.TipoInsumo.Find(id);
            if (tipoInsumo == null)
            {
                return HttpNotFound();
            }
            return View(tipoInsumo);
        }

        // POST: TipoInsumos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            TipoInsumo tipoInsumo = db.TipoInsumo.Find(id);
           

            Insumos insumos = db.Insumos.Where(x => x.IdTipoInsumo == tipoInsumo.Id).FirstOrDefault();

            if (insumos == null)
            {
                db.TipoInsumo.Remove(tipoInsumo);
            }
            else
            {
                tipoInsumo.Estado = false;
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

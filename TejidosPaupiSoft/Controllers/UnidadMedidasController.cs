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
    public class UnidadMedidasController : Controller
    {
        private TejidosPaupiDBEntities db = new TejidosPaupiDBEntities();

        // GET: UnidadMedidas
        public ActionResult Index()
        {
            return View(db.UnidadMedida.ToList().Where(x=>x.Estado==true));
        }

        // GET: UnidadMedidas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UnidadMedida unidadMedida = db.UnidadMedida.Find(id);
            if (unidadMedida == null)
            {
                return HttpNotFound();
            }
            return View(unidadMedida);
        }

        // GET: UnidadMedidas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UnidadMedidas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Descripcion,Estado")] UnidadMedida unidadMedida)
        {
           
            if (ModelState.IsValid)
            {
                
                if (!VerificarObjetoRepetido(unidadMedida))
                {
                    ViewBag.ObjetoRepetido = unidadMedida.Descripcion;
                    return View();
                }
                else
                {
                      unidadMedida.Estado = true;
                        db.UnidadMedida.Add(unidadMedida);
                        db.SaveChanges();   
                        return RedirectToAction("Index");
                }     
            }

            return View(unidadMedida);

        }

        public bool VerificarObjetoRepetido(UnidadMedida objeto)
        {
            UnidadMedida unidadMedida1 = db.UnidadMedida.Where(x => x.Descripcion == objeto.Descripcion && x.Estado == true).FirstOrDefault();

            if(unidadMedida1== null)
            { //No existe
                return true;
            }else
            {//Existe
                return false;
            }
           
        }



        // GET: UnidadMedidas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UnidadMedida unidadMedida = db.UnidadMedida.Find(id);
            if (unidadMedida == null)
            {
                return HttpNotFound();
            }

            return View(unidadMedida);
        }

        // POST: UnidadMedidas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Descripcion,Estado")] UnidadMedida unidadMedida)
        {
            if (ModelState.IsValid)
            {
                if (!VerificarObjetoRepetido(unidadMedida))
                {
                    ViewBag.ObjetoRepetido = unidadMedida.Descripcion;
                    return View();
                }
                else
                {
                    unidadMedida.Estado = true;
                    db.Entry(unidadMedida).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(unidadMedida);
        }

        // GET: UnidadMedidas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UnidadMedida unidadMedida = db.UnidadMedida.Find(id);
            if (unidadMedida == null)
            {
                return HttpNotFound();
            }
            return View(unidadMedida);
        }

        // POST: UnidadMedidas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UnidadMedida unidadMedida = db.UnidadMedida.Find(id);
            // db.UnidadMedida.Remove(unidadMedida);
            TipoInsumo tipoInsumo = db.TipoInsumo.Where(x=>x.IdUnidadMedida== unidadMedida.Id).FirstOrDefault();

            if(tipoInsumo == null)
            {
                db.UnidadMedida.Remove(unidadMedida);
            }
            else
            {
                unidadMedida.Estado = false;
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

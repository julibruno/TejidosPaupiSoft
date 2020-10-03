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
    public class ComprasController : Controller
    {
        private TejidosPaupiDBEntities db = new TejidosPaupiDBEntities();

        // GET: Compras
        public ActionResult Index()
        {


            return View(db.Compras.ToList().Where(x=>x.Estado==true));
        }

        // GET: Compras/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Compras");
            }
            Compras compras = db.Compras.Find(id);
            if (compras == null)
            {
                return HttpNotFound();
            }
            return View(compras);
        }

        // GET: Compras/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Compras/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Numero,Fecha,Observaciones,Subtotal,Descuentos,Total,FechaCreacion,FechaActualizacion,Estado")] Compras compras)
        {
            if (ModelState.IsValid)
            {
                compras.Descuentos = 0;
                compras.Subtotal = 0;
                compras.Total = 0;
                compras.Estado = true;
                compras.FechaCreacion = DateTime.Now;
                compras.Numero = ObtenerUltimoNumero();
          
                db.Compras.Add(compras);
                db.SaveChanges();
                return RedirectToAction("index","InsumosXCompras",new { IdCompraGenerada = compras.Id });
            }

            return View(compras);
        }

        public int ObtenerUltimoNumero()
        {
            Compras registro = db.Compras.Where(x => x.Estado == true).FirstOrDefault();

            if(registro!= null) { 

            int UltimoNumero = db.Compras.Where(x=>x.Estado==true).Max(x => x.Numero);
            return UltimoNumero + 1;
            }
            else
            {
                return 1;
            }
         

        }

        // GET: Compras/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Compras");
            }
            Compras compras = db.Compras.Find(id);
            if (compras == null)
            {
                return HttpNotFound();
            }
            return View(compras);
        }

        // POST: Compras/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Numero,Fecha,Observaciones,Subtotal,Descuentos,Total,FechaCreacion,FechaActualizacion,Estado")] Compras compras)
        {
            if (ModelState.IsValid)
            {

                compras.FechaActualizacion = DateTime.Now;

                compras.Total= compras.Subtotal - compras.Descuentos;

                db.Entry(compras).State = EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(compras);
        }



        // GET: Compras/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Compras");
            }
            Compras compras = db.Compras.Find(id);
            if (compras == null)
            {
                return HttpNotFound();
            }
            return View(compras);
        }

        // POST: Compras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Compras compras = db.Compras.Find(id);
            //db.Compras.Remove(compras);
            compras.Estado = false;
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

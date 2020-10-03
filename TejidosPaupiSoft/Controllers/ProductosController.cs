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
    public class ProductosController : Controller
    {
        private TejidosPaupiDBEntities db = new TejidosPaupiDBEntities();


        public bool VerificarObjetoRepetido(Producto objeto)
        {
            Producto producto = db.Producto.Where(x => x.Descripcion == objeto.Descripcion && x.Estado == true).FirstOrDefault();

            if (producto == null)
            { //No existe
                return true;
            }
            else
            {//Existe
                return false;
            }

        }
        // GET: Productos
        public ActionResult Index()
        {
            return View(db.Producto.Where(x => x.Estado == true).ToList());
        }

        // GET: Productos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Producto producto = db.Producto.Find(id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            return View(producto);
        }

        // GET: Productos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Productos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Descripcion,FechaCreacion")] Producto producto)
        {
            if (ModelState.IsValid)
            {
                if (!VerificarObjetoRepetido(producto))
                {
                    ViewBag.ObjetoRepetido = producto.Descripcion;
                }
                else
                {
                    producto.FechaCreacion = DateTime.Now;
                    producto.Estado = true;
                    db.Producto.Add(producto);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }  
            }

            return View(producto);
        }

        // GET: Productos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Producto producto = db.Producto.Find(id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            return View(producto);
        }

        // POST: Productos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Descripcion,FechaCreacion")] Producto producto)
        {
            if (ModelState.IsValid)
            {
                if (!VerificarObjetoRepetido(producto))
                {
                    ViewBag.ObjetoRepetido = producto.Descripcion;
                    return View();
                }
                else
                {
                    producto.Estado = true;
                    db.Entry(producto).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(producto);
        }

        // GET: Productos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Producto producto = db.Producto.Find(id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            return View(producto);
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            Producto producto = db.Producto.Find(id);
            if (producto == null)
            {
                return HttpNotFound();
            }

            FabricacionProducto productoEnFabricacionActiva = db.FabricacionProducto.Where(x => x.IdProducto == id).FirstOrDefault();

            if(productoEnFabricacionActiva == null )
            {
                db.Producto.Remove(producto);
           
            }
            else
            {
                producto.Estado = false;  
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

        // GET: Productos
        public ActionResult IndexParaFabricacion()
        {
            return View(db.Producto.Where(x=>x.Estado==true).ToList());
        }

         public ActionResult ProductoParaElaboracion(int idProductoSeleccionado)
        {


            FabricacionProducto NuevoObjeto = new FabricacionProducto();

            NuevoObjeto.IdProducto = idProductoSeleccionado;
            NuevoObjeto.FechaCreacion = DateTime.Now;
          
            NuevoObjeto.Estado = true;
            db.FabricacionProducto.Add(NuevoObjeto);
            db.SaveChanges();

            int IdGenerado = db.FabricacionProducto.Where(x => x.IdProducto == NuevoObjeto.IdProducto && x.Estado==true).Max(x => x.Id);

            return RedirectToAction("Index", "InsumosXFabricacions", new { IdFabricacionGenerada = IdGenerado });
            //return RedirectToAction("Index");
           
        }
    }
}

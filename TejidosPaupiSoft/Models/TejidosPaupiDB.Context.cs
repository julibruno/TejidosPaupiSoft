﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TejidosPaupiSoft.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class TejidosPaupiDBEntities : DbContext
    {
        public TejidosPaupiDBEntities()
            : base("name=TejidosPaupiDBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Insumos> Insumos { get; set; }
        public virtual DbSet<TipoInsumo> TipoInsumo { get; set; }
        public virtual DbSet<TipoLana> TipoLana { get; set; }
        public virtual DbSet<UnidadMedida> UnidadMedida { get; set; }
        public virtual DbSet<Compras> Compras { get; set; }
        public virtual DbSet<InsumosXCompra> InsumosXCompra { get; set; }
        public virtual DbSet<FabricacionProducto> FabricacionProducto { get; set; }
        public virtual DbSet<InsumosXFabricacion> InsumosXFabricacion { get; set; }
        public virtual DbSet<Producto> Producto { get; set; }
    }
}

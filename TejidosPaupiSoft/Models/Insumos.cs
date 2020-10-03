//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class Insumos
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Insumos()
        {
            this.InsumosXCompra = new HashSet<InsumosXCompra>();
            this.InsumosXFabricacion = new HashSet<InsumosXFabricacion>();
        }
    
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public int IdTipoInsumo { get; set; }
        public Nullable<int> IdTipoLana { get; set; }
        public string Color { get; set; }
        public int Cantidad { get; set; }
        public string Observaciones { get; set; }
        public System.DateTime FechaCreacion { get; set; }
        public Nullable<System.DateTime> FechaActualizacion { get; set; }
        public bool Estado { get; set; }
    
        public virtual TipoInsumo TipoInsumo { get; set; }
        public virtual TipoLana TipoLana { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InsumosXCompra> InsumosXCompra { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InsumosXFabricacion> InsumosXFabricacion { get; set; }
    }
}

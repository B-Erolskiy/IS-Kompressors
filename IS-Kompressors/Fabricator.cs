//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IS_Kompressors
{
    using System;
    using System.Collections.Generic;
    
    public partial class Fabricator
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Fabricator()
        {
            this.Equipments = new HashSet<Equipment>();
        }
    
        public int id_fabricator { get; set; }
        public string name_fabricator { get; set; }
        public Nullable<decimal> tel_fabricator { get; set; }
        public Nullable<decimal> inn_fabricator { get; set; }
        public string description_fabricator { get; set; }
        public string doing_fabricator { get; set; }
        public string contactName_fabricator { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Equipment> Equipments { get; set; }
    }
}
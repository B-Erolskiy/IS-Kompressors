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
    
    public partial class Part
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Part()
        {
            this.Actions_Parts = new HashSet<Actions_Parts>();
        }
    
        public int id_part { get; set; }
        public Nullable<int> equipment_part { get; set; }
        public string code_part { get; set; }
        public string name_part { get; set; }
        public string description_part { get; set; }
        public decimal col_part { get; set; }
        public decimal price_part { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Actions_Parts> Actions_Parts { get; set; }
        public virtual Equipment Equipment { get; set; }
    }
}

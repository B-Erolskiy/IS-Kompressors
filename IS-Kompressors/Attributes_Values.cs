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
    
    public partial class Attributes_Values
    {
        public int id_aValues { get; set; }
        public Nullable<int> attribute_aValues { get; set; }
        public Nullable<int> equipment_aValues { get; set; }
        public string value_aValues { get; set; }
    
        public virtual Attribute Attribute { get; set; }
        public virtual Equipment Equipment { get; set; }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KurortApp
{
    using System;
    using System.Collections.Generic;
    
    public partial class Barcodes
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string Barcode { get; set; }
    
        public virtual Orders Orders { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASPMVCProjektSklep.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }

        public virtual ICollection<CartProduct> CartProducts { get; set; }
    }
}
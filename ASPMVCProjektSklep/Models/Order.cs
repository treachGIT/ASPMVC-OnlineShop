using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASPMVCProjektSklep.Models
{ 
    public class Order
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string status { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
        public virtual Adress Adress { get; set; }
    }
}
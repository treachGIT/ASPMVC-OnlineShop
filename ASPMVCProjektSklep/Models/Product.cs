using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASPMVCProjektSklep.Models
{
    public class Product
    {
        public int Id { get; set; }

        [DisplayName("Nazwa")]
        [Required]
        [StringLength(30, ErrorMessage = "Nazwa jest za długa")]
        public string Name { get; set; }

        [DisplayName("Cena")]
        [Required]
        public float Price { get; set; }

        [DisplayName("Opis")]
        [Required]
        [StringLength(2000, ErrorMessage = "Opis jest za długi")]
        public string Description { get; set; }

        [DisplayName("Status")]
        public string status { get; set; }

        [DisplayName("Data dodania")]
        public DateTime creationDate { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<Image> Images { get; set; }
        public virtual ICollection<Sale> Sales { get; set; }
        public virtual ICollection<CartProduct> CartProducts { get; set; }
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ASPMVCProjektSklep.Models
{
    public class Category
    {
        public int Id { get; set; }

        [DisplayName("Nazwa")]
        [Required]
        [StringLength(30, ErrorMessage = "Nazwa jest za długa")]
        public string Name { get; set; }
        public string status { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
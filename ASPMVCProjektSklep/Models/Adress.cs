using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ASPMVCProjektSklep.Models
{
    public class Adress
    {
        public int Id { get; set; }

        [DisplayName("Miasto")]
        [Required]
        public string city { get; set; }

        [DisplayName("Ulica")]
        [Required]
        public string street { get; set; }

        [DisplayName("Budynek")]
        [Required]
        public string building { get; set; }

        [DisplayName("Mieszkanie")]
        [Required]
        public string apartment { get; set; }

        [DisplayName("Kod pocztowy")]
        [Required]
        [StringLength(10, ErrorMessage = "Kod pocztowy jest nieprawidłowy")]
        public string postalCode { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
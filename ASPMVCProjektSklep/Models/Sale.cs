using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ASPMVCProjektSklep.Models
{
    public class Sale
    {
        public int Id { get; set; }

        [Required]
        [Range(1, 99, ErrorMessage = "Discount have to be in range 1, 99")]
        public int Discount { get; set; }

        [Required]
        [DataType(DataType.Date, ErrorMessage = "To nie jest data dd/MM/yyyy") ]
        public DateTime startDate { get; set; }

        [Required]
        [DataType(DataType.Date, ErrorMessage = "To nie jest data dd/MM/yyyy")]
        public DateTime endDate { get; set; }
        public virtual Product Product { get; set; }
    }
}
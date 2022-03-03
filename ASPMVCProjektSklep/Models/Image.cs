using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ASPMVCProjektSklep.Models
{
    public class Image
    {
        public int Id { get; set; }

        [DisplayName("Tytuł")]
        [Required]
        [StringLength(30, ErrorMessage = "Tytuł jest za długi")]
        public string Title { get; set; }

        [DisplayName("Źródło")]
        [Required]
        [StringLength(700, ErrorMessage = "Źródło jest za długie")]
        public string Source { get; set; }
        public virtual Product Product { get; set; }
    }
}
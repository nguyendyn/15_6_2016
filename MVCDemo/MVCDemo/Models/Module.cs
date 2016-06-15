namespace MVCDemo.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Module")]
    public partial class Module
    {
        public Module()
        {
            Features = new HashSet<Feature>();
        }

        public int ID { get; set; }

        [StringLength(200)]
        [MinLength(3, ErrorMessage = "Module Name need more than 3 letters")]
        [Required]
        public string ModuleName { get; set; }

        [StringLength(200)]
        [MinLength(5, ErrorMessage = "Module Description need more than 5 letters")]
        [Required]
        public string ModuleDescription { get; set; }

        public int? ProjectId { get; set; }

        public bool? isSelected { get; set; }
        public virtual ICollection<Feature> Features { get; set; }

        public virtual Project Project { get; set; }
    }
}

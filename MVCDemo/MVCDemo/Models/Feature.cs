namespace MVCDemo.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Feature")]
    public partial class Feature
    {
        public Feature()
        {
           
        }

        public int ID { get; set; }

        [StringLength(200)]
        [MinLength(5, ErrorMessage = "Feature Name need more than 5 letters")]
        [Required]
        public string FeatureName { get; set; }

        [StringLength(200)]
        [Required]
        [MinLength(5, ErrorMessage = "Feature Description need more than 5 letters")]
        public string FeatureDescription { get; set; }

        public int? ModuleId { get; set; }
        public bool isSelected { get; set; }
      

        public virtual Module Module { get; set; }
       
    }
}

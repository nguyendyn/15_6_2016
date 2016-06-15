namespace MVCDemo.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Project")]
    public partial class Project
    {
        public Project()
        {
            Modules = new HashSet<Module>();
        }

        public int ID { get; set; }

        [StringLength(200)]
        [MinLength(3, ErrorMessage = "Project Name need more than 3 letters")]
        [Required]
        public string ProjectName { get; set; }

        [StringLength(200)]
        [Required]
        public string ProjectDescription { get; set; }

        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required]
        public DateTime? DateStart { get; set; }
      

        public virtual ICollection<Module> Modules { get; set; }
    }
}

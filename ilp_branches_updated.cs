namespace TourlistDataLayer.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

//added by samsuri (CR#57259)  on 2 Jan 2024
    public partial class ilp_branches_updated
    {
        [Key]
        public Guid ilp_add_branches_upd_idx { get; set; }

        [Required]
        public Guid stub_ref { get; set; }

        public Nullable<Int16> Is_change_address { get; set; }
        
        //new info
        [StringLength(400)]
        public string new_branch_addr_1 { get; set; }

        [StringLength(400)]
        public string new_branch_addr_2 { get; set; }

        [StringLength(400)]
        public string new_branch_addr_3 { get; set; }

        [StringLength(60)]
        public string new_branch_postcode { get; set; }

        public Guid? new_branch_city { get; set; }

        public Guid? new_branch_state { get; set; }

        //old info
        [StringLength(400)]
        public string old_branch_addr_1 { get; set; }

        [StringLength(400)]
        public string old_branch_addr_2 { get; set; }

        [StringLength(400)]
        public string old_branch_addr_3 { get; set; }

        [StringLength(60)]
        public string old_branch_postcode { get; set; }

        public Guid? old_branch_city { get; set; }

        public Guid? old_branch_state { get; set; }

        public Byte? active_status { get; set; } = 1; //tinyint in sql = Byte in C#

        public DateTime created_at { get; set; }
        public Guid created_by { get; set; }
        public DateTime modified_at { get; set; }
        public Guid modified_by { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] trx_dt { get; set; }

       // public Nullable<System.Guid> organization_ref { get; set; }

        

        
        

        
    }
}

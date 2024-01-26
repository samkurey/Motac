namespace TourlistDataLayer.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ilp_branches
    {
        [Key]
        public Guid ilp_branches_idx { get; set; }

        [Required]
        public Guid ilp_license_idx { get; set; }

        public decimal? paid_capital { get; set; }

        public decimal? authorized_capital { get; set; }

        [StringLength(400)]
        public string branch_addr_1 { get; set; }

        [StringLength(400)]
        public string branch_addr_2 { get; set; }

        [StringLength(400)]
        public string branch_addr_3 { get; set; }

        [StringLength(60)]
        public string branch_postcode { get; set; }

        public Guid? active_status { get; set; }
        public Guid? branch_city { get; set; }

        public Guid? branch_state { get; set; }

        [StringLength(60)]
        public string branch_name { get; set; }

        [StringLength(60)]
        public string branch_mobile_no { get; set; }

        [StringLength(60)]
        public string branch_phone_no { get; set; }

        [StringLength(60)]
        public string branch_fax_no { get; set; }

        [StringLength(60)]
        public string branch_email { get; set; }

        [StringLength(60)]
        public string branch_website { get; set; }

        public int? branch_size { get; set; }

        public Guid? utility { get; set; }

        [StringLength(60)]
        public string utility_others { get; set; }

        public DateTime created_dt { get; set; }

        public DateTime modified_dt { get; set; }

        public Guid created_by { get; set; }

        public Guid modified_by { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] trx_dt { get; set; }
        public string branch_license_ref_code { get; set; }
        public Nullable<System.Guid> organization_ref { get; set; }

        public Nullable<System.Guid> pbt_ref { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourlistBusinessLayer.Models
{
    public class ILPBranch
    {
        public Guid ilp_branch_idx;
        public Guid ilp_license_idx;
        public String branch_name; //added by samsuri (CR#57259)  on 27 Dec 2023
        public String branch_addr_1;
        public String branch_addr_2;
        public String branch_addr_3;
        public String branch_mobile_no;
        public String branch_postcode;
        public String branch_phone_no;
        public String branch_name;
        public String branch_license_ref_code;
        public Guid branch_city;
        public Guid? organization_ref;
        public Guid? active_status;
        public String status;
        public String branch_fax_no;
        public Guid branch_state;
        public String branch_email;
        public String branch_website;
        public Guid utility;
        public String utility_others;
        public int? branch_size;
        public Decimal authorized_capital;
        public Decimal paid_capital;
        public List<ILPMultiSelect> multi_utility;
        public Guid? pbt_ref;

        //added by samsuri (CR#57259)  on 3 jan 2024
        public Guid ilp_Stub_idx;
        public List<ILPBranchUpdated> branch_updated; 
        public String fileNameSewaBeliPremis;
        public String fileLocSewaBeliPremis;
        public String fileNamepelanLantai;
        public String fileLocpelanLantai;
        public String fileNameGambarPermis;
        public String fileLocGambarPermis;



    }
}

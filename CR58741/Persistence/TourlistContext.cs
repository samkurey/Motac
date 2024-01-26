using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Reflection;
using System.Web;
using TourlistDataLayer;
using TourlistDataLayer.DataModel;

namespace TourlistDataLayer.Persistence
{
    public class TourlistContext : DbContext
    {        
        //private const string ToulistConnectionString = "Data Source=47.254.249.24\\iZ8ps0e77bjeysZ\\MSSQLSERVER2,1433;Initial Catalog=dev_tourlist;Persist Security Info=True;User ID=sa;Password=r00t!@#;MultipleActiveResultSets=True;application name=EntityFramework;";

        //private const string tourlist_devname = "name=MotacEntities";

        public TourlistContext(string connectionString)
            : base(connectionString)
        {
            try
            {
                Database.SetInitializer(new ValidateDatabase<TourlistContext>());
                ((IObjectContextAdapter)this).ObjectContext.ContextOptions.LazyLoadingEnabled = true;
            }
            catch { }            
        }

        public virtual DbSet<core_users> Users { get; set; }
        public virtual DbSet<core_persons> Persons { get; set; }
        public virtual DbSet<core_person_qualifications> PersonQualifications { get; set; }
        public virtual DbSet<core_person_work_experiences> PersonWorkExperiences { get; set; }
        public virtual DbSet<core_person_courses> PersonCourses { get; set; }
        public virtual DbSet<core_user_roles> UserRoles { get; set; }
        public virtual DbSet<flow_application_stubs> FlowApplicationStubs { get; set; }
        public virtual DbSet<core_sol_modules> CoreModules { get; set; }
        public virtual DbSet<tobtab_licenses> TobtabLicenses { get; set; }

        public virtual DbSet<mm2h_licenses> MM2HLicenses { get; set; } //added by samsuri (#CR58741) on 26 jan 2024
        public virtual DbSet<core_organizations> CoreOrganizations { get; set; }
        public virtual DbSet<core_organization_shareholders> CoreOrganizationShareholders { get; set; }
        public virtual DbSet<core_organization_directors> CoreOrganizationDirectors { get; set; }
        public virtual DbSet<core_chklst_lists> Core_Chklst_Lists { get; set; }
        public virtual DbSet<core_chklst_items> Core_Chklst_Items { get; set; }
        public virtual DbSet<ilp_licenses> IlpLicenses { get; set; }
        public virtual DbSet<core_chklist_instances> ChecklistInstances { get; set; }
        public virtual DbSet<core_chkitems_instances> ChecklistItemsInstances { get; set; }
        public virtual DbSet<ref_geo_postcodes> Postcodes { get; set; }
        public virtual DbSet<ref_geo_towns> Towns { get; set; }
        public virtual DbSet<ref_geo_districts> Districts { get; set; }
        public virtual DbSet<ref_geo_states> States { get; set; }
        public virtual DbSet<ref_geo_countries> Countries { get; set; }
        public virtual DbSet<ref_sequence> Sequence { get; set; }
        public virtual DbSet<core_acknowledgements> Acknowledgements { get; set; }
        public virtual DbSet<ilp_branches> IlpBranches { get; set; }
        public virtual DbSet<tobtab_add_branches> TobtabBranches { get; set; } //added by samsuri (CR#57258) on 10 Jan 2024
        public virtual DbSet<tobtab_add_branches_updated> TobtabBranchesUpdated { get; set; } //added by samsuri (CR#57258) on 10 Jan 2024
        public virtual DbSet<ilp_branches_updated> IlpBranchesUpdated { get; set; } //added by samsuri (CR#57259) on 28 Dec 2023
        public virtual DbSet<mm2h_add_branches> MM2HBranches { get; set; } //added by samsuri (CR#58741) on 24 Jan 2024
        public virtual DbSet<mm2h_add_branches_updated> MM2HBranchesUpdated { get; set; } //added by samsuri (CR#58741) on 26 Jan 2024
        public virtual DbSet<ilp_permits> IlpPermits { get; set; }
        public virtual DbSet<ref_references> References { get; set; }
        public virtual DbSet<ref_information> RefInformation { get; set; }
        public virtual DbSet<ref_references_types> ReferenceTypes { get; set; }
        public virtual DbSet<ilp_person_references> IlpPersonReferences { get; set; }
        public virtual DbSet<ilp_instructor_courses> IlpInstructorCourses { get; set; }
        public virtual DbSet<ilp_terminate_licenses> IlpTerminateLicenses { get; set; }
        public virtual DbSet<ilp_terminate_branches> IlpTerminateBranches { get; set; }
        public virtual DbSet<ilp_multi_selects> IlpMultiSelects { get; set; }
        public virtual DbSet<ilp_uploads> IlpUploads { get; set; }
        public virtual DbSet<incentive_applications> Incentive_Applications { get; set; }
        public virtual DbSet<incentive_types> Incentive_Types { get; set; }
        public virtual DbSet<vw_ref_references_by_ref_types> VwRefReferencesByRefTypes { get; set; }
        public virtual DbSet<process_application> ProcessApplication { get; set; }
        public virtual DbSet<process_application_detail> ProcessApplicationDetail { get; set; }
        public virtual DbSet<core_uploads_freeform_by_persons> CoreUploadsFreeformByPersons { get; set; }
        public virtual DbSet<core_uploads_freeform_by_modules> CoreUploadsFreeformByModules { get; set; }
        public virtual DbSet<ref_person_identifier_types> RefPersonIdentifierTypes { get; set; }
        public virtual DbSet<ref_status_record> RefStatusRecord { get; set; }
        public virtual DbSet<ref_user_organizations> RefUserOrganizations { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var typesToRegister = Assembly.GetExecutingAssembly().GetTypes()
            .Where(type => !String.IsNullOrEmpty(type.Namespace))
            .Where(type => type.BaseType != null && type.BaseType.IsGenericType &&
                type.BaseType.GetGenericTypeDefinition() == typeof(TourlistEntityTypeConfiguration<>));
            foreach (var type in typesToRegister)
            {
                dynamic configurationInstance = Activator.CreateInstance(type);
                modelBuilder.Configurations.Add(configurationInstance);
            }
            modelBuilder.Conventions.Remove<DecimalPropertyConvention>();
            modelBuilder.Conventions.Add(new DecimalPropertyConvention(18, 4));
            base.OnModelCreating(modelBuilder);
        }
    }

    public class ValidateDatabase<TContext> : IDatabaseInitializer<TContext> where TContext : DbContext
    {
        public static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);

        public void InitializeDatabase(TContext context)
        {
            if (!context.Database.Exists())
            {
                Logger.Error("Database does not exist");
                throw new ApplicationException("Database does not exist");
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TourlistDataLayer.Core;
using TourlistDataLayer.Core.Repositories;
using TourlistDataLayer.Persistence.Repositories;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;
using System.Web.UI;
using TourlistDataLayer.DataModel;
using System.Runtime.Remoting.Contexts;

namespace TourlistDataLayer.Persistence
{
    public class TourlistUnitOfWork
    {
        private readonly TourlistContext _context;

        public TourlistUnitOfWork(TourlistContext context)
        {
            _context = context;

            Users = new UserRepository(_context);
            Persons = new PersonsRepository(_context);
            UserRoles = new UserRoleRepository(_context);
            CoreModules = new CoreModulesRepository(_context);
            CoreLicenseRepository = new CoreLicenseRepository(_context);
            CoreUserRolesRepository = new CoreUserRolesRepository(_context);
            CoreUserPermissionsRepository = new CoreUserPermissionsRepository(_context);
            CoreUserRolesPermisssionRepository = new CoreUserRolesPermisssionRepository(_context);
            CorePersonsRepository = new CorePersonsRepository(_context);
            CoreUsersRepository = new CoreUsersRepository(_context);
            CoreUsersRolesSpecificUsersRepository = new CoreUsersRolesSpecificUsersRepository(_context);
            FlowApplicationStubs = new FlowApplicationStubsRepository(context);
            VwFlowApplicationListRepository = new VwFlowApplicationListRepository(context);
            TobtabLicenses = new TobtabLicensesRepository(context);
            CoreOrganizations = new CoreOrganizationsRepository(context);
            CoreOrganizationsUpdatedRepository = new CoreOrganizationsUpdatedRepository(context);
            CoreOrganizationShareholders = new CoreOrganizationShareholdersRepository(context);
            CoreOrganizationFinancialInfoRepository = new CoreOrganizationFinancialInfoRepository(context);
            CoreOrgShareholdersUpdatedRepository = new CoreOrgShareholdersUpdatedRepository(context);
            CoreOrgDirectorsUpdatedRepository = new CoreOrgDirectorsUpdatedRepository(context);
            CoreOrganizationChargesRepository = new CoreOrganizationChargesRepository(context);
            RefStatesRepository = new RefStatesRepository(context);
            RefCountriesRepository = new RefCountriesRepository(context);
            RefGeoCountriesRepository = new RefGeoCountriesRepository(context);
            RefReferencesRepository = new RefReferencesRepository(context);
            RefReferencesTypesRepository = new RefReferencesTypesRepository(context);
            RefUserTypesRepository = new RefUserTypesRepository(context);
            RefUserGradesRepository = new RefUserGradesRepository(context);
            RefUserPositionsRepository = new RefUserPositionsRepository(context);
            RefUserDepartmentRepository = new RefUserDepartmentRepository(context);
            RefUserOrganizationsRepository = new RefUserOrganizationsRepository(context);
            RefUserLocationsRepository = new RefUserLocationsRepository(context);
            RefUserRolesRepository = new RefUserRolesRepository(context);
            RefAdminRepository = new RefAdminRepository(context);
            CoreSolContextsRepository = new CoreSolContextsRepository(context);
            RefPersonIdentifierTypesRepository = new RefPersonIdentifierTypesRepository(context);
            CoreAcknowledgementsRepository = new CoreAcknowledgementsRepository(context);
            CoreChkItemsInstancesRepository = new CoreChkItemsInstancesRepository(context);
            CoreChkListInstancesRepository = new CoreChkListInstancesRepository(context);
            CoreChklstItemsRepository = new CoreChklstItemsRepository(context);
            CoreChklstListsRepository = new CoreChklstListsRepository(context);
            MM2HLicensesRepository = new MM2HLicensesRepository(context);
            MM2HTerminateLicensesRepositry = new MM2HTerminateLicensesRepositry(context);
            TGLicenseRepository = new TGLicenseRepository(context);
            TobtabTGTravelingRepository = new TobtabTGTravelingRepository(context);
            VwRefReferencesRepository = new VwRefReferencesRepository(context);
            PPPReportGradeRepository = new PPPReportGradeRepository(context);
            RefIndustriesRepository = new RefIndustriesRepository(context);
            TGLicenseLanguagesRepository = new TGLicenseLanguagesRepository(context);
            CorePersonQualificationsRepository = new CorePersonQualificationsRepository(context);
            CorePersonCoursesRepository = new CorePersonCoursesRepository(context);
            CorePersonAssociationsRepository = new CorePersonAssociationsRepository(context);
            CorePersonsUpdatedRepository = new CorePersonsUpdatedRepository(context);
            RefGeoZonesRepository = new RefGeoZonesRepository(context);
            RefGeoDistrictsRepository = new RefGeoDistrictsRepository(context);
            RefGeoPostcodesRepository = new RefGeoPostcodesRepository(context);
            RefGeoStatesRepository = new RefGeoStatesRepository(context);
            RefGeoTownsRepository = new RefGeoTownsRepository(context);
            VwGeoListRepository = new VwGeoListRepository(context);
            TGLicenseHistoryRepository = new TGLicensesHistoryRepository(context);
            TGAppAcknowledgmentRepository = new TGAppAcknowledgmentRepository(context);
            CorePersonWorkExperiencesRepository = new CorePersonWorkExperiencesRepository(context);
            CoreUploadsFreeFormPersonRepository = new CoreUploadsFreeFormPersonRepository(context);
            CoreUploadsFreeformModulesRepository = new CoreUploadsFreeformModulesRepository(context);
            CoreUploadsFreeformOrganizationsRepository = new CoreUploadsFreeformOrganizationsRepository(context);
            VwUsersListRepository = new VwUsersListRepository(context);
            VwDashboardListRepository = new VwDashboardListRepository(context);
            VwCommonIntegrationRepository = new VwCommonIntegrationRepository(context);
            VwCoreUsersMultiPositionsRepository = new VwCoreUsersMultiPositionsRepository(context);
            VwCommonAuditTrailRepository = new VwCommonAuditTrailRepository(context);
            VwAppslistingTgRepository = new VwAppslistingTgRepository(context);
            VwAppslistingPppRepository = new VwAppslistingPppRepository(context);
            VwAppslistingSpapukRepository = new VwAppslistingSpapukRepository(context);
            VwAppslistingBpkspRepository = new VwAppslistingBpkspRepository(context);
            VwAppslistingTobtabRepository = new VwAppslistingTobtabRepository(context);
            VwAppslistingTgExceptionRepository = new VwAppslistingTgExceptionRepository(context);
            VwAppslistingUmrahRepository = new VwAppslistingUmrahRepository(context);
            VwAppslistingMmhRepository = new VwAppslistingMmhRepository(context);
            VwAppslistingIlpRepository = new VwAppslistingIlpRepository(context);
            VwAppslistingCarnivalRepository = new VwAppslistingCarnivalRepository(context);
            VwCarnivalApplicationRepository = new VwCarnivalApplicationRepository(context);
            VwIlpApplicationRepository = new VwIlpApplicationRepository(context);
            VwTobtabApplicationRepository = new VwTobtabApplicationRepository(context);
            VwBpkspApplicationRepository = new VwBpkspApplicationRepository(context);
            VwTGApplicationRepository = new VwTGApplicationRepository(context);
            VwMM2HApplicationRepository = new VwMM2HApplicationRepository(context);
            VwMM2HDashboardListRepository = new VwMM2HDashboardListRepository(context);
            VwMM2HShareHolderRepository = new VwMM2HShareHolderRepository(context);
            VwCoreOrgShareholderRepository = new VwCoreOrgShareholderRepository(context);
            VwCoreOrgShareholderSearchRepository = new VwCoreOrgShareholderSearchRepository(context);
            vwChangeStatusShareholdersRepository = new VwChangeStatusShareholdersRepository(context);
            VwChangeStatusDirectorsRepository = new VwChangeStatusDirectorsRepository(context);
            vwCoreOrgDirectorRepository = new VwCoreOrgDirectorRepository(context);
            VwPPPSetQuestionRepository = new VwPPPSetQuestionRepository(context);
            VwPPPCategoriesListRepository = new VwPPPCategoriesListRepository(context);
            VwPPPSetFormSubcriteriaRepository = new VwPPPSetFormSubcriteriaRepository(context);
            VwPPPSetFormQuestionRepository = new VwPPPSetFormQuestionRepository(context);
            VwPPPSetFormAnswerRepository = new VwPPPSetFormAnswerRepository(context);
            VwPPPSetSubQuestionRepository = new VwPPPSetSubQuestionRepository(context);
            VwPPPSetSubcriteriaQuestionRepository = new VwPPPSetSubcriteriaQuestionRepository(context);
            VwPPPSetFormScoreRepository = new VwPPPSetFormScoreRepository(context);
            CoreOrganizationDirectorsRepository = new CoreOrganizationDirectorsRepository(context);
            MM2HAddBranchesRepository = new MM2HAddBranchesRepository(context);
            RefStatusRecordRepository = new RefStatusRecordRepository(context);
            CoreSolSolutionsRepository = new CoreSolSolutionsRepository(context);
            CoreSolModulesRepository = new CoreSolModulesRepository(context);
            IlpLicenses = new IlpLicensesRepository(context);
            IlpBranches = new IlpBranchesRepository(context);
            IlpBranchesUpdated = new IlpBranchesUpdatedRepository(context); //added by samsuri on 28 Dec 2023
            IlpPermits = new IlpPermitsRepository(context);
            IlpPersonReferences = new IlpPersonReferencesRepository(context);
            IlpInstructorCourses = new IlpInstructorCoursesRepository(context);
            IlpTerminateLicenses = new IlpTerminateLicensesRepository(context);
            IlpTerminateBranches = new IlpTerminateBranchesRepository(context);
            IlpMultiSelects = new IlpMultiSelectsRepository(context);
            IlpUploads = new IlpUploadsRepository(context);
            VwCoreOrgShareholderRepository = new VwCoreOrgShareholderRepository(context);
            VwCoreOrgShareholderSearchRepository = new VwCoreOrgShareholderSearchRepository(context);
            CoreSolModulesRepository = new CoreSolModulesRepository(context);
            CarnivalLicenseRepository = new CarnivalLicenseRepository(context);
            carnivals_attendeesRepository = new carnivals_attendeesRepository(context);
            EnforcementRepository = new EnforcementRepository(context);
            IncentiveTypesRepository = new IncentiveTypesRepository(context);
            IncentiveApplicationsRepository = new IncentiveApplicationsRepository(context);
            RefFormattingRepository = new RefFormattingRepository(context);
            RefInformationRepository = new RefInformationRepository(context);
            VwEnforcementRepository = new VwEnforcementRepository(context);
            VwEnforcementActivityRepository = new VwEnforcementActivityRepository(context);
            BPKSPLicensesRepository = new BPKSPLicensesRepository(context);
            BPKSPLicensesDetailsRepository = new BPKSPLicensesDetailsRepository(context);
            BPKSPLicensesVehicleReplaceRepository = new BPKSPLicensesVehicleReplaceRepository(context);
            BPKSPVehicleTypeRepository = new BPKSPVehicleTypeRepository(context);
            BPKSPJpjSubmitLogRepository = new BPKSPJpjSubmitLogRepository(context);
            CommonAuditTrailLoginsRepository = new CommonAuditTrailLoginsRepository(context);
            CommonAuditTrailApplicationRepository = new CommonAuditTrailApplicationRepository(context);
            CommonAuditTrailTxnRepository = new CommonAuditTrailTxnRepository(context);
            PaymentMasterRepository = new PaymentMasterRepository(context);
            PaymentDetailsRepository = new PaymentDetailsRepository(context);
            PaymentDetailsModeRepository = new PaymentDetailsModeRepository(context);
            PaymentCollectionRepository = new PaymentCollectionRepository(context);
            PaymentCollectionDetailsRepository = new PaymentCollectionDetailsRepository(context);
            PaymentRefundRepository = new PaymentRefundRepository(context);
            PaymentReprintReceiptRepository = new PaymentReprintReceiptRepository(context);
            CommonDashboardsRepository = new CommonDashboardsRepository(context);
            TobtabForeignPackagesRepository = new TobtabForeignPackagesRepository(context);
            TobtabForeignPartnersRepository = new TobtabForeignPartnersRepository(context);
            TobtabTerminateLicenseRepository = new TobtabTerminateLicenseRepository(context);
            TobtabAddBranchesRepository = new TobtabAddBranchesRepository(context);
            TobtabMarketingAgentRepository = new TobtabMarketingAgentRepository(context);
            TobtabMarketingAreaRepository = new TobtabMarketingAreaRepository(context);
            TobtabTGExceptionsRepository = new TobtabTGExceptionsRepository(context);
            RefPaymentFeeRepository = new RefPaymentFeeRepository(context);
            CoreUserTypeMenuRepository = new CoreUserTypeMenuRepository(context);
            VwPaidListRepository = new VwPaidListRepository(context);
            VwPaidReceiptCancelRepository = new VwPaidReceiptCancelRepository(context);
            VwPaymentsListRepository = new VwPaymentsListRepository(context);
            VwRefPaymentsFeeRepository = new VwRefPaymentsFeeRepository(context);
            ProcessApplicationRepository = new ProcessApplicationRepository(context);
            ProcessAppointmentRepository = new ProcessAppointmentRepository(context);
            RefSequenceRepository = new RefSequenceRepository(context);
            ProcessApplicationDetailRepository = new ProcessApplicationDetailRepository(context);
            EnforcementInvestigationsRepository = new EnforcementInvestigationsRepository(context);
            EnforcementFreeUploadsRepository = new EnforcementFreeUploadsRepository(context);
            PPPRegistrationRepository = new PPPRegistrationRepository(context);
            VWPPPAplicationRepository = new VWPPPAplicationRepository(context);
            VwPPPFormApplicationRepository = new VwPPPFormApplicationRepository(context);
            PPPAssociationRepository = new PPPAssociationRepository(context);
            PPPOwnerRepository = new PPPOwnerRepository(context);
            PPPOperatorRepository = new PPPOperatorRepository(context);
            PPPRoomDetailRepository = new PPPRoomDetailRepository(context);
            PPPGradingRepository = new PPPGradingRepository(context);
            PPPGradingFormRepository = new PPPGradingFormRepository(context);
            PPPRatingPanelRepository = new PPPRatingPanelRepository(context);
            VWPPPratingApplicationRepository = new VwPPPratingApplicationRepository(context);
            VwPPPUsersListsRepository = new VwPPPUsersListsRepository(context);
            VwPPPSetFormCriteriaRepository = new VwPPPSetFormCriteriaRepository(context);
            RefPPPAnswerRepository = new RefPPPAnswerRepository(context);
            RefPPPCriteriaRepository = new RefPPPCriteriaRepository(context);
            PPPRoomRestSpaDetailRepository = new PPPRoomRestSpaDetailRepository(context);
            RefPPPQuestionRepository = new RefPPPQuestionRepository(context);
            RefPPPSubcriteriaRepository = new RefPPPSubcriteriaRepository(context);
            RefPPPSetFormRepository = new RefPPPSetFormRepository(context);
            RefPPPSetFormCriteriaRepository = new RefPPPSetFormCriteriaRepository(context);
            RefPPPSetFormSubcriteriaRepository = new RefPPPSetFormSubcriteriaRepository(context);
            RefPPPSetFormQuestionRepository = new RefPPPSetFormQuestionRepository(context);
            RefPPPSetFormAnswerRepository = new RefPPPSetFormAnswerRepository(context);
            ProcessEnquiryRepository = new ProcessEnquiryRepository(context);
            ProcessEnquiryDetailRepository = new ProcessEnquiryDetailRepository(context);
            ProcessAppealRepository = new ProcessAppealRepository(context);
            ProcessAppealDetailRepository = new ProcessAppealDetailRepository(context);
            ProcessMeetingInspRepository = new ProcessMeetingInspRepository(context);
            MobileMainSliderRepository = new MobileMainSliderRepository(context);
            MobileAnnouncementRepository = new MobileAnnouncementRepository(context);
            ProcessMeetingInspDetailRepository = new ProcessMeetingInspDetailRepository(context);
            ProcessMeetingInspOfficerRepository = new ProcessMeetingInspOfficerRepository(context);
            ProcessUploadRepository = new ProcessUploadRepository(context);
            CoreUsersMultiPositionsRepository = new CoreUsersMultiPositionsRepository(context);
            CompoundRepository = new CompoundRepository(context);
            CompoundAppealRepository = new CompoundAppealRepository(context);
            CompoundBtkRepository = new CompoundBtkRepository(context);
            VwEnforcementInvestigationsRepository = new VwEnforcementInvestigationsRepository(context);
            CardMgmtRepository = new CardMgmtRepository(context);
            CardMgmtDetailRepository = new CardMgmtDetailRepository(context);
            CardMgmtMCRepository = new CardMgmtMCRepository(context);
            VwPaymentRefundRepository = new VwPaymentRefundRepository(context);
            Enforcement_investigations_sessionRepository = new Enforcement_investigations_sessionRepository(context);
            VwEnforcementDateSessionRepository = new VwEnforcementDateSessionRepository(context);
            VwEnforcementTeamRepository = new VwEnforcementTeamRepository(context);
            EnforcementTeamRepository = new EnforcementTeamRepository(context);
            TGAppReferenceRepository = new TGAppReferenceRepository(context);
            CoreUserAttendanceRepository = new CoreUserAttendanceRepository(context);
            CoreUserMenuRepository = new CoreUserMenuRepository(context);
            RefUserTeamRepository = new RefUserTeamRepository(context);
            CoreUserTeamRepository = new CoreUserTeamRepository(context);
            IgfmaBatchReportRepository = new IgfmaBatchReportRepository(context);
            TobtabUmrahScheduleRepository = new TobtabUmrahScheduleRepository(context);
            TobtabUmrahLanguageRepository = new TobtabUmrahLanguageRepository(context);
            TobtabUmrahAdvertisingRepository = new TobtabUmrahAdvertisingRepository(context);
            CoreUserMenuDetailsRepository = new CoreUserMenuDetailsRepository(context);
            VwPaymentReceiptCancelRepository = new VwPaymentReceiptCancelRepository(context);
            VwCommonUsersLoginRepository = new VwCommonUsersLoginRepository(context);
            VwEnforcementDiaryRepository = new VwEnforcementDiaryRepository(context);
            VwPaymentOnlineCollectionRepository = new VwPaymentOnlineCollectionRepository(context);
            VwPaymentCollectionListRepository = new VwPaymentCollectionListRepository(context);
            RefActSectionRepository = new RefActSectionRepository(context);
            VwEnforcementDiaryDetailRepository = new VwEnforcementDiaryDetailRepository(context);
            ReportModuleRefRepository = new ReportModuleRefRepository(context);
            EnforcementDiaryRepository = new EnforcementDiaryRepository(context);
            VwEnforcementFIRRepository = new VwEnforcementFIRRepository(context);
            CoreWorkFlowRepository = new CoreWorkflowRepository(context);
            VwEnforcementFIRDetailRepository = new VwEnforcementFIRDetailRepository(context);
            EnforcementInvestigationsFirRepository = new EnforcementInvestigationsFirRepository(context);
            EnforcementInvestigationsFirDetRepository = new EnforcementInvestigationsFirDetRepository(context);
            VwEnforcementStaffsRepository = new VwEnforcementStaffsRepository(context);
            TempCardInfoRepository = new TempCardInfoRepository(context);
            EnforcementActRepository = new EnforcementActRepository(context);
            EnforcementVehicleNoticesRepository = new EnforcementVehicleNoticesRepository(context);
            VwEnforcementMonitoringRepository = new VwEnforcementMonitoringRepository(context);
            VwMobileIlpRepository = new VwMobileIlpRepository(context);
            VwMobileBpkspListRepository = new VwMobileBpkspListRepository(context);
            VwMobileBpkspRepository = new VwMobileBpkspRepository(context);
            VwMobileGetVehicleRepository = new VwMobileGetVehicleRepository(context);
            VwMobileCarnivalRepository = new VwMobileCarnivalRepository(context);
            VwMobileHubungiKamiRepository = new VwMobileHubungiKamiRepository(context);
            VwMobileKompaunRepository = new VwMobileKompaunRepository(context);
            vwMobileMm2HRepository = new VwMobileMm2hRepository(context);
            VwMobileMaklumatPerusahaanRepository = new VwMobileMaklumatPerusahaanRepository(context);
            VwMobilePppSpaPukRatingRepository = new VwMobilePppSpaPukRatingRepository(context);
            VwMobilePppSpaPukRepository = new VwMobilePppSpaPukRepository(context);
            VwMobileSenBayaranRepository = new VwMobileSenBayaranRepository(context);
            VwMobileSemakStatusRepository = new VwMobileSemakStatusRepository(context);
            VwMobileRekodTransaksiRepository = new VwMobileRekodTransaksiRepository(context);
            VwMobileSenEntitiBatalRepository = new VwMobileSenEntitiBatalRepository(context);
            VwMobileTgRepository = new VwMobileTgRepository(context);
            VwMobileTobtabRepository = new VwMobileTobtabRepository(context);
            VwMobileUmrahRepository = new VwMobileUmrahRepository(context);
            VwEnforcementActRepository = new VwEnforcementActRepository(context);
            VwCompoundPaymentRepository = new VwCompoundPaymentRepository(context);
            VwCompoundMonitorRepository = new VwCompoundMonitorRepository(context);
            VwCompoundDirectIzinRepository = new VwCompoundDirectIzinRepository(context);
            VwCompoundIndirectIzinRepository = new VwCompoundIndirectIzinRepository(context);
            VwCompoundRenewalRepository = new VwCompoundRenewalRepository(context);
            VwCompoundChgStatusRepository = new VwCompoundChgStatusRepository(context);
            VwCompoundBtkRepository = new VwCompoundBtkRepository(context);
            VwCompoundBtkNotifyRepository = new VwCompoundBtkNotifyRepository(context);
            VwCompoundReminderRepository = new VwCompoundReminderRepository(context);
            VwCompoundAppealRepository = new VwCompoundAppealRepository(context);
            VwCompoundAppealNotifyRepository = new VwCompoundAppealNotifyRepository(context);
            VwEnforcementIPRepository = new VwEnforcementIPRepository(context);
            VwIgfmasPaymentListRepository = new VwIgfmasPaymentListRepository(context);
            VwFeeLicenseListRepository = new VwFeeLicenseListRepository(context);
            IgfmasCalendarRepository = new IgfmasCalendarRepository(context);
            VwDailyReportListRepository = new VwDailyReportListRepository(context);
            CompoundSuspensionRepository = new CompoundSuspensionRepository(context);
            PPPRatingReviewRepository = new PPPRatingReviewRepository(context);
            PPPRoomAORRepository = new PPPRoomAORRepository(context);
            PPPTouristCountRepository = new PPPTouristCountRepository(context);
            PPPPremiseRoomRepository = new PPPPremiseRoomRepository(context);
            PPPPremiseStaffRepository = new PPPPremiseStaffRepository(context);
            RefMonthRepository = new RefMonthRepository(context);
            VwNotificationCountsRepository = new VwNotificationCountsRepository(context);
            VwEnforcementIPSyorRepository = new VwEnforcementIPSyorRepository(context);
            VwReceiptCancelListRepository = new VwReceiptCancelListRepository(context);
            VwPPPSetQuestionByCriteriaRepository = new VwPPPSetQuestionByCriteriaRepository(context);
            VwReprintReceiptRepository = new VwReprintReceiptRepository(context);
            VwReceiptCancelListRepository = new VwReceiptCancelListRepository(context);
            VwPPPSetFormQuestionMainRepository = new VwPPPSetFormQuestionMainRepository(context);
            PPPRegisteredGradedRepository = new PPPRegisteredGradedRepository(context);
            VwPaidTabListRepository = new VwPaidTabListRepository(context);
            RefPbtRepository = new RefPbtRepository(context);
            VwPPPGradingFormCriteriaRepository = new VwPPPGradingFormCriteriaRepository(context);
            VwPPPGradingFormSubCriteriaRepository = new VwPPPGradingFormSubCriteriaRepository(context);
            VwPPPGradingFormQuestionRepository = new VwPPPGradingFormQuestionRepository(context);
            VwPPPGradingFormSubQuestionRepository = new VwPPPGradingFormSubQuestionRepository(context);
            VwPPPGradingFormAnswerRepository = new VwPPPGradingFormAnswerRepository(context);
            VwCompoundSSMRepository = new VwCompoundSSMRepository(context);
            VwEnforceUserDetailRepository = new VwEnforceUserDetailRepository(context);
            PPPJustificationDetailRepository = new PPPJustificationDetailRepository(context);
            EnforcementCalendarRepository = new EnforcementCalendarRepository(context);
            UtilityAuditTrailRepository = new UtilityAuditTrailRepository(context);
            VwMeetingListRepository = new VwMeetingListRepository(context);
            OfficerDashboardRepository = new OfficerDashboardRepository(context);
        }
        public OfficerDashboardRepository OfficerDashboardRepository { get; private set; }
        public VwMeetingListRepository VwMeetingListRepository { get; private set; }
        public VwFeeLicenseListRepository VwFeeLicenseListRepository { get; private set; }
        public VwPaidListRepository VwPaidListRepository { get; private set; }
        public VwPaidReceiptCancelRepository VwPaidReceiptCancelRepository { get; private set; }
        public RefUserDepartmentRepository RefUserDepartmentRepository { get; private set; }
        public CommonAuditTrailApplicationRepository CommonAuditTrailApplicationRepository { get; private set; }
        public CoreLicenseRepository CoreLicenseRepository { get; private set; }
        public TobtabTGTravelingRepository TobtabTGTravelingRepository { get; private set; }
        public TobtabUmrahAdvertisingRepository TobtabUmrahAdvertisingRepository { get; private set; }
        public TobtabUmrahLanguageRepository TobtabUmrahLanguageRepository { get; private set; }
        public TobtabUmrahScheduleRepository TobtabUmrahScheduleRepository { get; private set; }
        public CardMgmtDetailRepository CardMgmtDetailRepository { get; private set; }
        public CardMgmtRepository CardMgmtRepository { get; private set; }
        public CardMgmtMCRepository CardMgmtMCRepository { get; private set; }
        public UserRepository Users { get; private set; }
        public UserRoleRepository UserRoles { get; private set; }
        public PersonsRepository Persons { get; private set; }
        public CoreUsersRepository CoreUsersRepository { get; private set; }
        public CoreUserPermissionsRepository CoreUserPermissionsRepository { get; private set; }
        public CoreUserRolesPermisssionRepository CoreUserRolesPermisssionRepository { get; private set; }
        public CoreModulesRepository CoreModules { get; private set; }
        public CoreUserRolesRepository CoreUserRolesRepository { get; private set; }
        public CoreUsersRolesSpecificUsersRepository CoreUsersRolesSpecificUsersRepository { get; private set; }
        public CorePersonsRepository CorePersonsRepository { get; private set; }
        public CorePersonsUpdatedRepository CorePersonsUpdatedRepository { get; private set; }
        public FlowApplicationStubsRepository FlowApplicationStubs { get; private set; }
        public VwFlowApplicationListRepository VwFlowApplicationListRepository { get; private set; }

        public TobtabLicensesRepository TobtabLicenses { get; private set; }
        public CoreOrganizationsRepository CoreOrganizations { get; private set; }

        public CoreOrganizationFinancialInfoRepository CoreOrganizationFinancialInfoRepository { get; }

        public CoreOrganizationChargesRepository CoreOrganizationChargesRepository { get; }

        public CoreOrganizationsUpdatedRepository CoreOrganizationsUpdatedRepository { get; private set; }

        public CoreOrganizationShareholdersRepository CoreOrganizationShareholders { get; private set; }

        public CoreOrgShareholdersUpdatedRepository CoreOrgShareholdersUpdatedRepository { get; private set; }

        public CoreOrgDirectorsUpdatedRepository CoreOrgDirectorsUpdatedRepository { get; private set; }
        public RefStatesRepository RefStatesRepository { get; private set; }
        public RefCountriesRepository RefCountriesRepository { get; private set; }
        public RefGeoCountriesRepository RefGeoCountriesRepository { get; private set; }
        public RefStatusRecordRepository RefStatusRecordRepository { get; private set; }
        public VwRefReferencesRepository VwRefReferencesRepository { get; private set; }
        public RefReferencesRepository RefReferencesRepository { get; private set; }
        public RefInformationRepository RefInformationRepository { get; private set; }
        public PPPReportGradeRepository PPPReportGradeRepository { get; private set; }
        public RefReferencesTypesRepository RefReferencesTypesRepository { get; private set; }
        public RefUserTypesRepository RefUserTypesRepository { get; private set; }
        public CoreSolSolutionsRepository CoreSolSolutionsRepository { get; private set; }
        public RefUserGradesRepository RefUserGradesRepository { get; private set; }
        public RefUserPositionsRepository RefUserPositionsRepository { get; private set; }
        public RefUserOrganizationsRepository RefUserOrganizationsRepository { get; private set; }
        public RefUserLocationsRepository RefUserLocationsRepository { get; private set; }
        public RefAdminRepository RefAdminRepository { get; private set; }
        public RefUserRolesRepository RefUserRolesRepository { get; private set; }
        public CoreSolContextsRepository CoreSolContextsRepository { get; private set; }
        public CoreSolModulesRepository CoreSolModulesRepository { get; private set; }
        public RefPersonIdentifierTypesRepository RefPersonIdentifierTypesRepository { get; private set; }
        public CoreAcknowledgementsRepository CoreAcknowledgementsRepository { get; private set; }
        public CoreChkItemsInstancesRepository CoreChkItemsInstancesRepository { get; private set; }
        public CoreChkListInstancesRepository CoreChkListInstancesRepository { get; private set; }
        public CoreChklstItemsRepository CoreChklstItemsRepository { get; private set; }
        public CoreChklstListsRepository CoreChklstListsRepository { get; private set; }
        public MM2HLicensesRepository MM2HLicensesRepository { get; private set; }
        public MM2HTerminateLicensesRepositry MM2HTerminateLicensesRepositry { get; private set; }
        public PPPAssociationRepository PPPAssociationRepository { get; private set; }
        public RefPPPAnswerRepository RefPPPAnswerRepository { get; private set; }
        public RefPPPCriteriaRepository RefPPPCriteriaRepository { get; private set; }
        public PPPRoomRestSpaDetailRepository PPPRoomRestSpaDetailRepository { get; private set; }
        public RefPPPQuestionRepository RefPPPQuestionRepository { get; private set; }
        public RefPPPSubcriteriaRepository RefPPPSubcriteriaRepository { get; private set; }
        public RefPPPSetFormCriteriaRepository RefPPPSetFormCriteriaRepository { get; private set; }
        public RefPPPSetFormSubcriteriaRepository RefPPPSetFormSubcriteriaRepository { get; private set; }
        public RefPPPSetFormQuestionRepository RefPPPSetFormQuestionRepository { get; private set; }
        public RefPPPSetFormAnswerRepository RefPPPSetFormAnswerRepository { get; private set; }
        public VwPPPFormApplicationRepository VwPPPFormApplicationRepository { get; private set; }
        public RefPPPSetFormRepository RefPPPSetFormRepository { get; private set; }
        public TGLicenseRepository TGLicenseRepository { get; private set; }
        public RefIndustriesRepository RefIndustriesRepository { get; private set; }
        public TGLicenseLanguagesRepository TGLicenseLanguagesRepository { get; private set; }
        public CorePersonQualificationsRepository CorePersonQualificationsRepository { get; private set; }
        public CorePersonCoursesRepository CorePersonCoursesRepository { get; private set; }
        public CorePersonAssociationsRepository CorePersonAssociationsRepository { get; private set; }
        public RefGeoZonesRepository RefGeoZonesRepository { get; private set; }
        public RefGeoDistrictsRepository RefGeoDistrictsRepository { get; private set; }
        public RefGeoPostcodesRepository RefGeoPostcodesRepository { get; private set; }
        public RefGeoStatesRepository RefGeoStatesRepository { get; private set; }
        public RefGeoTownsRepository RefGeoTownsRepository { get; private set; }
        public VwGeoListRepository VwGeoListRepository { get; private set; }
        public TGLicensesHistoryRepository TGLicenseHistoryRepository { get; private set; }
        public TGAppAcknowledgmentRepository TGAppAcknowledgmentRepository { get; private set; }
        public CorePersonWorkExperiencesRepository CorePersonWorkExperiencesRepository { get; private set; }
        public CoreUploadsFreeFormPersonRepository CoreUploadsFreeFormPersonRepository { get; private set; }
        public CoreUploadsFreeformModulesRepository CoreUploadsFreeformModulesRepository { get; private set; }
        public CoreUploadsFreeformOrganizationsRepository CoreUploadsFreeformOrganizationsRepository { get; private set; }

        public VwUsersListRepository VwUsersListRepository { get; private set; }
        public VwDashboardListRepository VwDashboardListRepository { get; private set; }
        public VwCommonIntegrationRepository VwCommonIntegrationRepository { get; private set; }
        public VwCoreUsersMultiPositionsRepository VwCoreUsersMultiPositionsRepository { get; private set; }
        public VwCommonAuditTrailRepository VwCommonAuditTrailRepository { get; private set; }
        public VwMM2HApplicationRepository VwMM2HApplicationRepository { get; private set; }
        public VwCarnivalApplicationRepository VwCarnivalApplicationRepository { get; private set; }
        public VwIlpApplicationRepository VwIlpApplicationRepository { get; private set; }
        public VwTobtabApplicationRepository VwTobtabApplicationRepository { get; private set; }
        public VwBpkspApplicationRepository VwBpkspApplicationRepository { get; private set; }
        public VwTGApplicationRepository VwTGApplicationRepository { get; private set; }
        public VwMM2HDashboardListRepository VwMM2HDashboardListRepository { get; private set; }
        public VwMM2HShareHolderRepository VwMM2HShareHolderRepository { get; private set; }
        public VwCoreOrgShareholderRepository VwCoreOrgShareholderRepository { get; private set; }
        public VwCoreOrgShareholderSearchRepository VwCoreOrgShareholderSearchRepository { get; private set; }
        public VwCoreOrgDirectorRepository vwCoreOrgDirectorRepository { get; private set; }
        public VwChangeStatusDirectorsRepository VwChangeStatusDirectorsRepository { get; set; }
        public VwChangeStatusShareholdersRepository vwChangeStatusShareholdersRepository { get; set; }
        public VwAppslistingTgRepository VwAppslistingTgRepository { get; private set; }
        public VwAppslistingPppRepository VwAppslistingPppRepository { get; private set; }
        public VwAppslistingSpapukRepository VwAppslistingSpapukRepository { get; private set; }
        public VwAppslistingBpkspRepository VwAppslistingBpkspRepository { get; private set; }
        public VwAppslistingTobtabRepository VwAppslistingTobtabRepository { get; private set; }
        public VwAppslistingTgExceptionRepository VwAppslistingTgExceptionRepository { get; private set; }
        public VwAppslistingUmrahRepository VwAppslistingUmrahRepository { get; private set; }
        public VwAppslistingMmhRepository VwAppslistingMmhRepository { get; private set; }
        public VwAppslistingIlpRepository VwAppslistingIlpRepository { get; private set; }
        public VwAppslistingCarnivalRepository VwAppslistingCarnivalRepository { get; private set; }


        public VwPPPSetQuestionRepository VwPPPSetQuestionRepository { get; protected set; }

        public VwPPPCategoriesListRepository VwPPPCategoriesListRepository { get; protected set; }

        public VwPPPSetFormCriteriaRepository VwPPPSetFormCriteriaRepository { get; private set; }

        public VwPPPSetFormSubcriteriaRepository VwPPPSetFormSubcriteriaRepository { get; private set; }

        public VwPPPSetFormQuestionRepository VwPPPSetFormQuestionRepository { get; private set; }

        public VwPPPSetFormQuestionMainRepository VwPPPSetFormQuestionMainRepository { get; protected set; }

        public VwPPPSetFormAnswerRepository VwPPPSetFormAnswerRepository { get; private set; }

        public VwPPPSetSubQuestionRepository VwPPPSetSubQuestionRepository { get; private set; }

        public VwPPPSetSubcriteriaQuestionRepository VwPPPSetSubcriteriaQuestionRepository { get; private set; }

        public VwPPPSetFormScoreRepository VwPPPSetFormScoreRepository { get; private set; }
        public CoreOrganizationDirectorsRepository CoreOrganizationDirectorsRepository { get; private set; }
        public MM2HAddBranchesRepository MM2HAddBranchesRepository { get; private set; }
        public IlpLicensesRepository IlpLicenses { get; private set; }
        public IlpBranchesRepository IlpBranches { get; private set; }

        public IlpBranchesUpdatedRepository IlpBranchesUpdated { get; private set; } //added by samsuri on 28 De 2023
        public IlpPermitsRepository IlpPermits { get; private set; }
        public IlpPersonReferencesRepository IlpPersonReferences { get; private set; }
        public IlpInstructorCoursesRepository IlpInstructorCourses { get; private set; }
        public IlpTerminateLicensesRepository IlpTerminateLicenses { get; private set; }
        public IlpTerminateBranchesRepository IlpTerminateBranches { get; private set; }
        public IlpMultiSelectsRepository IlpMultiSelects { get; private set; }
        public IlpUploadsRepository IlpUploads { get; private set; }
        public PPPRegistrationRepository PPPRegistrationRepository { get; private set; }
        public PPPOwnerRepository PPPOwnerRepository { get; private set; }
        public PPPOperatorRepository PPPOperatorRepository { get; private set; }
        public PPPRoomDetailRepository PPPRoomDetailRepository { get; private set; }
        public PPPGradingRepository PPPGradingRepository { get; private set; }
        public PPPGradingFormRepository PPPGradingFormRepository { get; private set; }
        public PPPRatingPanelRepository PPPRatingPanelRepository { get; private set; }
        public PPPPremiseRoomRepository PPPPremiseRoomRepository { get; private set; }
        public PPPPremiseStaffRepository PPPPremiseStaffRepository { get; private set; }
        public VWPPPAplicationRepository VWPPPAplicationRepository { get; private set; }
        public VwPPPratingApplicationRepository VWPPPratingApplicationRepository { get; private set; }
        public VwPPPUsersListsRepository VwPPPUsersListsRepository { get; private set; }
        public CarnivalLicenseRepository CarnivalLicenseRepository { get; private set; }
        public carnivals_attendeesRepository carnivals_attendeesRepository { get; private set; }
        public EnforcementRepository EnforcementRepository { get; private set; }
        public IncentiveApplicationsRepository IncentiveApplicationsRepository { get; private set; }
        public IncentiveTypesRepository IncentiveTypesRepository { get; private set; }
        public RefFormattingRepository RefFormattingRepository { get; private set; }
        public VwEnforcementRepository VwEnforcementRepository { get; private set; }
        public VwEnforcementActivityRepository VwEnforcementActivityRepository { get; private set; }
        public BPKSPLicensesRepository BPKSPLicensesRepository { get; private set; }
        public BPKSPLicensesDetailsRepository BPKSPLicensesDetailsRepository { get; private set; }
        public BPKSPLicensesVehicleReplaceRepository BPKSPLicensesVehicleReplaceRepository { get; private set; }
        public BPKSPVehicleTypeRepository BPKSPVehicleTypeRepository { get; private set; }
        public BPKSPJpjSubmitLogRepository BPKSPJpjSubmitLogRepository { get; private set; }
        public CommonAuditTrailLoginsRepository CommonAuditTrailLoginsRepository { get; private set; }
        public CommonAuditTrailTxnRepository CommonAuditTrailTxnRepository { get; private set; }
        public TobtabForeignPackagesRepository TobtabForeignPackagesRepository { get; private set; }
        public TobtabForeignPartnersRepository TobtabForeignPartnersRepository { get; private set; }
        public TobtabTerminateLicenseRepository TobtabTerminateLicenseRepository { get; private set; }
        public TobtabAddBranchesRepository TobtabAddBranchesRepository { get; private set; }
        public TobtabMarketingAgentRepository TobtabMarketingAgentRepository { get; private set; }
        public TobtabMarketingAreaRepository TobtabMarketingAreaRepository { get; private set; }
        public TobtabTGExceptionsRepository TobtabTGExceptionsRepository { get; private set; }
        public PaymentMasterRepository PaymentMasterRepository { get; private set; }
        public PaymentDetailsRepository PaymentDetailsRepository { get; private set; }
        public PaymentDetailsModeRepository PaymentDetailsModeRepository { get; private set; }
        public PaymentCollectionRepository PaymentCollectionRepository { get; private set; }
        public PaymentCollectionDetailsRepository PaymentCollectionDetailsRepository { get; private set; }
        public PaymentRefundRepository PaymentRefundRepository { get; private set; }
        public PaymentReprintReceiptRepository PaymentReprintReceiptRepository { get; private set; }
        public CommonDashboardsRepository CommonDashboardsRepository { get; private set; }
        public RefPaymentFeeRepository RefPaymentFeeRepository { get; private set; }
        public CoreUserTypeMenuRepository CoreUserTypeMenuRepository { get; private set; }
        public VwPaymentsListRepository VwPaymentsListRepository { get; private set; }
        public VwRefPaymentsFeeRepository VwRefPaymentsFeeRepository { get; private set; }
        public ProcessApplicationRepository ProcessApplicationRepository { get; private set; }
        public ProcessAppointmentRepository ProcessAppointmentRepository { get; private set; }
        public EnforcementFreeUploadsRepository EnforcementFreeUploadsRepository { get; private set; }
        public RefSequenceRepository RefSequenceRepository { get; private set; }
        public ProcessApplicationDetailRepository ProcessApplicationDetailRepository { get; private set; }
        public EnforcementInvestigationsRepository EnforcementInvestigationsRepository { get; set; }
        public ProcessEnquiryRepository ProcessEnquiryRepository { get; private set; }
        public ProcessEnquiryDetailRepository ProcessEnquiryDetailRepository { get; private set; }
        public ProcessAppealRepository ProcessAppealRepository { get; private set; }
        public ProcessAppealDetailRepository ProcessAppealDetailRepository { get; private set; }
        public ProcessMeetingInspRepository ProcessMeetingInspRepository { get; private set; }
        public MobileMainSliderRepository MobileMainSliderRepository { get; private set; }
        public MobileAnnouncementRepository MobileAnnouncementRepository { get; private set; }
        public ProcessMeetingInspDetailRepository ProcessMeetingInspDetailRepository { get; private set; }
        public ProcessMeetingInspOfficerRepository ProcessMeetingInspOfficerRepository { get; private set; }
        public ProcessUploadRepository ProcessUploadRepository { get; private set; }
        public CoreUsersMultiPositionsRepository CoreUsersMultiPositionsRepository { get; private set; }
        public CompoundRepository CompoundRepository { get; private set; }
        public CompoundAppealRepository CompoundAppealRepository { get; private set; }
        public CompoundBtkRepository CompoundBtkRepository { get; private set; }
        public EnforcementCalendarRepository EnforcementCalendarRepository { get; private set; }
        public VwEnforcementInvestigationsRepository VwEnforcementInvestigationsRepository { get; private set; }
        public Enforcement_investigations_sessionRepository Enforcement_investigations_sessionRepository { get; set; }
        public VwEnforcementDateSessionRepository VwEnforcementDateSessionRepository { get; set; }
        public VwPaymentRefundRepository VwPaymentRefundRepository { get; private set; }
        public VwEnforcementTeamRepository VwEnforcementTeamRepository { get; set; }

        public EnforcementTeamRepository EnforcementTeamRepository { get; set; }
        public CoreUserAttendanceRepository CoreUserAttendanceRepository { get; private set; }

        public TGAppReferenceRepository TGAppReferenceRepository { get; private set; }
        public CoreUserMenuRepository CoreUserMenuRepository { get; private set; }
        public RefUserTeamRepository RefUserTeamRepository { get; private set; }
        public CoreUserTeamRepository CoreUserTeamRepository { get; private set; }
        public IgfmaBatchReportRepository IgfmaBatchReportRepository { get; private set; }
        public CoreUserMenuDetailsRepository CoreUserMenuDetailsRepository { get; private set; }
        public VwPaymentReceiptCancelRepository VwPaymentReceiptCancelRepository { get; private set; }
        public VwCommonUsersLoginRepository VwCommonUsersLoginRepository { get; private set; }
        public VwEnforcementDiaryRepository VwEnforcementDiaryRepository { get; set; }
        public VwPaymentOnlineCollectionRepository VwPaymentOnlineCollectionRepository { get; private set; }
        public VwPaymentCollectionListRepository VwPaymentCollectionListRepository { get; private set; }
        public RefActSectionRepository RefActSectionRepository { get; set; }
        public VwEnforcementDiaryDetailRepository VwEnforcementDiaryDetailRepository { get; set; }
        public EnforcementDiaryRepository EnforcementDiaryRepository { get; set; }
        public VwEnforcementFIRRepository VwEnforcementFIRRepository { get; set; }
        public CoreWorkflowRepository CoreWorkFlowRepository { get; }
        public VwEnforcementFIRDetailRepository VwEnforcementFIRDetailRepository { get; set; }
        public EnforcementInvestigationsFirRepository EnforcementInvestigationsFirRepository { get; set; }
        public EnforcementInvestigationsFirDetRepository EnforcementInvestigationsFirDetRepository { get; set; }
        public VwEnforcementStaffsRepository VwEnforcementStaffsRepository { get; set; }
        public TempCardInfoRepository TempCardInfoRepository { get; set; }
        public ReportModuleRefRepository ReportModuleRefRepository { get; set; }
        public EnforcementActRepository EnforcementActRepository { get; set; }
        public EnforcementVehicleNoticesRepository EnforcementVehicleNoticesRepository { get; set; }
        public PPPJustificationDetailRepository PPPJustificationDetailRepository { get; set; }
        public VwEnforcementMonitoringRepository VwEnforcementMonitoringRepository { get; set; }
        public VwMobileIlpRepository VwMobileIlpRepository { get; set; }
        public VwMobileBpkspListRepository VwMobileBpkspListRepository { get; set; }
        public VwMobileBpkspRepository VwMobileBpkspRepository { get; set; }
        public VwMobileGetVehicleRepository VwMobileGetVehicleRepository { get; set; }
        public VwMobileCarnivalRepository VwMobileCarnivalRepository { get; set; }
        public VwMobileHubungiKamiRepository VwMobileHubungiKamiRepository { get; set; }
        public VwMobileMm2hRepository vwMobileMm2HRepository { get; set; }
        public VwMobilePppSpaPukRatingRepository VwMobilePppSpaPukRatingRepository { get; set; }
        public VwMobilePppSpaPukRepository VwMobilePppSpaPukRepository { get; set; }
        public VwMobileSenBayaranRepository VwMobileSenBayaranRepository { get; set; }
        public VwMobileSemakStatusRepository VwMobileSemakStatusRepository { get; set; }
        public VwMobileRekodTransaksiRepository VwMobileRekodTransaksiRepository { get; set; }
        public VwMobileSenEntitiBatalRepository VwMobileSenEntitiBatalRepository { get; set; }
        public VwMobileTgRepository VwMobileTgRepository { get; set; }
        public VwMobileTobtabRepository VwMobileTobtabRepository { get; set; }
        public VwMobileUmrahRepository VwMobileUmrahRepository { get; set; }
        public VwMobileKompaunRepository VwMobileKompaunRepository { get; set; }
        public VwMobileMaklumatPerusahaanRepository VwMobileMaklumatPerusahaanRepository { get; set; }
        public VwEnforcementActRepository VwEnforcementActRepository { get; set; }
        public VwCompoundPaymentRepository VwCompoundPaymentRepository { get; set; }
        public VwCompoundMonitorRepository VwCompoundMonitorRepository { get; set; }
        public VwCompoundDirectIzinRepository VwCompoundDirectIzinRepository { get; set; }
        public VwCompoundIndirectIzinRepository VwCompoundIndirectIzinRepository { get; set; }
        public VwCompoundRenewalRepository VwCompoundRenewalRepository { get; set; }
        public VwCompoundChgStatusRepository VwCompoundChgStatusRepository { get; set; }
        public VwCompoundBtkRepository VwCompoundBtkRepository { get; set; }
        public VwCompoundBtkNotifyRepository VwCompoundBtkNotifyRepository { get; set; }
        public VwCompoundReminderRepository VwCompoundReminderRepository { get; set; }
        public VwCompoundAppealRepository VwCompoundAppealRepository { get; set; }
        public VwCompoundAppealNotifyRepository VwCompoundAppealNotifyRepository { get; set; }
        public VwIgfmasPaymentListRepository VwIgfmasPaymentListRepository { get; set; }

        public VwEnforcementIPRepository VwEnforcementIPRepository { get; set; }
        public IgfmasCalendarRepository IgfmasCalendarRepository { get; set; }
        public CompoundSuspensionRepository CompoundSuspensionRepository { get; set; }
        public VwDailyReportListRepository VwDailyReportListRepository { get; set; }
        public PPPRatingReviewRepository PPPRatingReviewRepository { get; set; }
        public VwNotificationCountsRepository VwNotificationCountsRepository { get; set; }


        public VwEnforcementIPSyorRepository VwEnforcementIPSyorRepository { get; set; }
        public PPPRoomAORRepository PPPRoomAORRepository { get; set; }
        public PPPTouristCountRepository PPPTouristCountRepository { get; set; }
        public RefMonthRepository RefMonthRepository { get; set; }
        public VwReceiptCancelListRepository VwReceiptCancelListRepository { get; set; }
        public VwPPPSetQuestionByCriteriaRepository VwPPPSetQuestionByCriteriaRepository { get; set; }
        public VwReprintReceiptRepository VwReprintReceiptRepository { get; set; }
        public PPPRegisteredGradedRepository PPPRegisteredGradedRepository { get; set; }
        public VwPaidTabListRepository VwPaidTabListRepository { get; set; }
        public RefPbtRepository RefPbtRepository { get; set; }
        public VwPPPGradingFormCriteriaRepository VwPPPGradingFormCriteriaRepository { get; set; }
        public VwPPPGradingFormSubCriteriaRepository VwPPPGradingFormSubCriteriaRepository { get; set; }
        public VwPPPGradingFormQuestionRepository VwPPPGradingFormQuestionRepository { get; set; }
        public VwPPPGradingFormSubQuestionRepository VwPPPGradingFormSubQuestionRepository { get; set; }
        public VwPPPGradingFormAnswerRepository VwPPPGradingFormAnswerRepository { get; set; }
        public VwCompoundSSMRepository VwCompoundSSMRepository { get; set; }
        public VwEnforceUserDetailRepository VwEnforceUserDetailRepository { get; set; }
        public UtilityAuditTrailRepository UtilityAuditTrailRepository { get; set; }

        public int Complete()
        {
            int ret = 0;

            try
            {
                ret = _context.SaveChanges();
            }
            catch (Exception)
            {
                this.RollbackChanges();
                throw;
            }

            return ret;
        }


        public void RollbackChanges()
        {
            foreach (var modifiedEntry in _context.ChangeTracker.Entries().Where(e => e.State != System.Data.Entity.EntityState.Unchanged))
            {
                var entity = modifiedEntry.Entity;

                if (entity == null) continue;

                switch (modifiedEntry.State)
                {
                    //case EntityState.Detached:
                    case System.Data.Entity.EntityState.Deleted:
                    case System.Data.Entity.EntityState.Modified:
                        modifiedEntry.Reload();
                        break;
                    case System.Data.Entity.EntityState.Added:
                        modifiedEntry.State = System.Data.Entity.EntityState.Detached;
                        ////if entity is in Added state, remove it. (there will be problems with Set methods if entity is of proxy type, in that case you need entity base type
                        //var set = this.Set(entity.GetType());
                        //set.Remove(entity);
                        break;

                }
            }
        }

        public DbRawSqlQuery<T> SQLQuery<T>(string sql, params object[] parameters)
        {
            return _context.Database.SqlQuery<T>(sql, parameters);
        }

    }
}
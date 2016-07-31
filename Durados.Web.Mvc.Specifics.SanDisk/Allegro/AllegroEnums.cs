// Enum for Allegro1
namespace Durados.Web.Mvc.Specifics.SanDisk.Allegro {
    using System;
    
    
    public enum Allegro1Views {
        
        Architecture,
        
        UserRole,
        
        v_ASIC,
        
        AsicFoundary,
        
        AsicSiliconTechnology,
        
        BC,
        
        BusWidth,
        
        ChipsFIM,
        
        DateChange,
        
        DateType,
        
        Density,
        
        Durados_Log,
        
        Durados_Session,
        
        ECCType,
        
        Endurance,
        
        EngRevision,
        
        FIMCard,
        
        HostBUS,
        
        HostSpeed,
        
        IdlePower,
        
        v_MDE,
        
        v_Nand,
        
        PlaneInt,
        
        v_PLM,
        
        PLMHostSpeed,
        
        PLMPower,
        
        PLMSDA_MMCA,
        
        PLMStatus,
        
        v_POR,
        
        v_PORCapacity,
        
        PORChannel,
        
        PORInitiator,
        
        PORPriority,
        
        PORStage,
        
        Power,
        
        ReasonForChange,
        
        SDA_MMCA,
        
        SleepPower,
        
        SpeedClass,
        
        v_Test,
        
        TestFlow,
        
        TestFlowType,
        
        v_TestTestTypeCommitDate,
        
        TestType,
        
        UHSGrade,
        
        User,
        
        v_BE_FW,
        
        BEGroup,
        
        ProductClass,
        
        ProductClassCapacity,
        
        ProductLine,
        
        ProductCapacity,
        
        PORProductClassCapacity,
        
        Technology,
        
        Memory,
        
        v_RequestedDatePORFromPLM,
        
        v_CommittedDatePLMToPOR,
        
        v_ForecastDatePLMToPOR,
        
        ProductConfiguration,
        
        ProductConfigurationParameters,
        
        DistributionList,
        
        DistributionListRole,
        
        DistributionListUser,
        
        v_UserForDistributionList,
        
        v_UserRoleForDistributionList,
        
        v_SamplesRequest,
        
        ASICController,
        
        ASICControllerFamily,
        
        ASICRevision,
        
        ASICSiliconDensity,
        
        ASICStatus,
        
        v_ASICTechnicalSpec,
        
        v_RequiredTapeoutPLMFromASIC,
        
        BE_FIM,
        
        BEBitsCell,
        
        BEGroup2,
        
        BELabel,
        
        BEMeasureDaily,
        
        BEMemory,
        
        BENANDBusWidth,
        
        BEReasonForChangeRCDate,
        
        BEReleaseType,
        
        BEStatus,
        
        BETechnical,
        
        BETechnicalStatus,
        
        PLMGroup,
        
        PLMGroupProductLine,
        
        v_ASICRoadmap,
        
        v_ASICRoadmap2,
        
        ProductionRevision,
        
        v_DistributionListEmail,
        
        ProductLineParameter,
        
        Parameter,
        
        v_TechnologyProductClassCapacity,
        
        DieInt,
        
        v_EngMarketingReport,
        
        TechnologyProductClassCapacityHostSpeed,
        
        TechnologyProductClassCapacityPower,
        
        TechnologyProductClassCapacitySDA_MMCA,
        
        v_RequiredCSPMMFromPLM,
        
        v_RequiredESPMMFromPLM,
        
        MRD,
        
        MemoryRev,
        
        v_NANDRoadmapRev,
        
        NANDRoadmapRevName,
        
        v_NandMemoryMemoryRev,
        
        ProcessCondition,
        
        MDEPrimaryCons,
    }
    
    public enum Architecture {
        
        Id,
        
        Name,
        
        FK_Nand_Architecture_Children,
    }
    
    public enum UserRole {
        
        Name,
        
        Description,
        
        FK_User_UserRole_Children,
    }
    
    public enum v_ASIC {
        
        FK_ASIC_ASICSiliconDensity_Parent,
        
        FK_ASIC_ASICStatus_Parent,
        
        FK_ASIC_ASICTechnicalSpec_Parent,
        
        FK_ASIC_User_Parent,
        
        FK_ASIC_User1_Parent,
        
        ASICController_v_ASIC_Parent,
        
        ASICControllerFamily_v_ASIC_Parent,
        
        FK_ASIC_ASICRevision1_Parent,
        
        Id,
        
        ASICReqCompleteForecast,
        
        ASICReqCompleteCommit,
        
        FPGAForROMDevForecast,
        
        FPGAForROMDevCommit,
        
        PADRingForecast,
        
        PADRingCommit,
        
        FinalROMDeliveryForecast,
        
        FinalROMDeliveryCommit,
        
        TapeoutRequired,
        
        TapeoutCommit,
        
        ReasonForChangeForecast,
        
        TapeoutForecast,
        
        WaferOutForecast,
        
        WaferOutCommit,
        
        BlindBuildPackagePartsShipForecast,
        
        BlindBuildPackagePartsShipCommit,
        
        TestedASICForProductQualForecast,
        
        TestedASICForProductQualCommit,
        
        ASICConditionalQualForecast,
        
        ASICConditionalQualCommit,
        
        CharacterizationCompleteForecast,
        
        CharacterizationCompleteCommit,
        
        ASICFullQualForecast,
        
        ASICFullQualCommit,
        
        PNs,
        
        ASICProjectCharter,
        
        CreateDate,
        
        ModifiedDate,
        
        ProjectId,
        
        ASICProjectWebpageURL,
        
        ASICDisplayName,
        
        FK_POR_ASIC_Children,
        
        FK_ASIC_v_PLM_Children,
        
        FK_ASIC_v_Test_Children,
        
        FK_v_ASIC_v_SamplesRequest_Children,
    }
    
    public enum AsicFoundary {
        
        Id,
        
        Name,
    }
    
    public enum AsicSiliconTechnology {
        
        Id,
        
        Name,
    }
    
    public enum BC {
        
        Id,
        
        Name,
        
        FK_Nand_BC_Children,
    }
    
    public enum BusWidth {
        
        Id,
        
        Name,
        
        FK_PLM_BusWidth_Children,
        
        FK_ProductConfigurationParameters_BusWidth_Children,
        
        BusWidth_TechnologyProductClassCapacity_Children,
        
        BusWidth_v_EngMarketingReport_Children,
        
        BusWidth_v_EngMarketingReport1_Children,
    }
    
    public enum ChipsFIM {
        
        Id,
        
        Name,
        
        FK_PLM_ChipsFIM_Children,
        
        FK_ProductConfigurationParameters_ChipsFIM_Children,
        
        FK_BETechnical_ChipsFIM_Children,
        
        ChipsFIM_TechnologyProductClassCapacity_Children,
        
        ChipsFIM_v_EngMarketingReport_Children,
        
        ChipsFIM_v_EngMarketingReport1_Children,
    }
    
    public enum DateChange {
        
        FK_DateChange_DateType_Parent,
        
        FK_DateChange_ReasonForChange_Parent,
        
        FK_DateChange_User_Parent,
        
        Id,
        
        DateTypeObjId,
        
        Date,
        
        ReasonForChange,
        
        DateCreated,
    }
    
    public enum DateType {
        
        Id,
        
        Name,
        
        FK_DateChange_DateType_Children,
        
        FK_DateChange_DateType1_Children,
        
        FK_DateChange_DateType2_Children,
        
        FK_DateChange_DateType3_Children,
        
        FK_DateType_v_RequiredTapeoutPLMFromASIC_Children,
        
        FK_DateType_v_RequiredESPMMFromPLM_Children,
        
        FK_DateType_v_RequiredCSPMMFromPLM_Children,
    }
    
    public enum Density {
        
        Id,
        
        Name,
        
        FK_Nand_Density_Children,
    }
    
    public enum Durados_Log {
        
        ID,
        
        ApplicationName,
        
        Username,
        
        MachineName,
        
        Time,
        
        Controller,
        
        Action,
        
        MethodName,
        
        LogType,
        
        ExceptionMessage,
        
        Trace,
        
        FreeText,
        
        Guid,
    }
    
    public enum Durados_Session {
        
        SessionID,
        
        Name,
        
        Scalar,
        
        SerializedObject,
        
        TypeCode,
        
        ObjectType,
    }
    
    public enum ECCType {
        
        Id,
        
        Name,
        
        FK_ECCType_v_Nand_Children,
    }
    
    public enum Endurance {
        
        Id,
        
        Name,
        
        FK_EnduranceInitial_v_MDE_Children,
        
        FK_Endurance_Intial_v_Nand_Children,
        
        Endurance_Target_v_Nand_Children,
    }
    
    public enum EngRevision {
        
        Id,
        
        Name,
        
        FK_MDE_EngRevision_Children,
    }
    
    public enum FIMCard {
        
        Id,
        
        Name,
        
        FK_PLM_FIMCard_Children,
        
        FK_ProductConfigurationParameters_FIMCard_Children,
        
        FIMCard_TechnologyProductClassCapacity_Children,
        
        FIMCard_v_EngMarketingReport_Children,
        
        FIMCard_v_EngMarketingReport1_Children,
    }
    
    public enum HostBUS {
        
        Id,
        
        Name,
        
        FK_PLM_HostBUS_Children,
        
        FK_ProductConfigurationParameters_HostBUS_Children,
        
        HostBUS_TechnologyProductClassCapacity_Children,
        
        HostBUS_v_EngMarketingReport_Children,
        
        HostBUS_v_EngMarketingReport1_Children,
    }
    
    public enum HostSpeed {
        
        Id,
        
        Name,
        
        FK_PLMHostSpeed_HostSpeed_Children,
        
        HostSpeed_TechnologyProductClassCapacityHostSpeed_Children,
    }
    
    public enum IdlePower {
        
        Id,
        
        Name,
        
        FK_PLM_IdlePower_Children,
        
        FK_ProductConfigurationParameters_IdlePower_Children,
        
        IdlePower_TechnologyProductClassCapacity_Children,
        
        IdlePower_v_EngMarketingReport_Children,
        
        IdlePower_v_EngMarketingReport1_Children,
    }
    
    public enum v_MDE {
        
        FK_MDE_EngRevision_Parent,
        
        FK_ProductionRevision_v_MDE_Parent,
        
        FK_ProcessCondition_v_MDE_Parent,
        
        FK_EnduranceInitial_v_MDE_Parent,
        
        FK_Memory_v_MDE_Parent,
        
        FK_Technology_v_MDE_Parent,
        
        FK_ProductLine_v_MDE_Parent,
        
        Id,
        
        NANDECCId,
        
        TSBPN,
        
        Silicon,
        
        DieQualComplete,
        
        DSParametersRelease,
        
        ProductionOut,
        
        Comments,
        
        SNDK_PN,
        
        MaskChange,
        
        KGDPN,
        
        DieSortVersion,
        
        KGDProgram,
        
        MemSpeed,
        
        TechComments,
        
        MaskTO,
        
        MaskJudge,
        
        PreDATWafers_CR_up,
        
        PreDATWafers_Ex_J,
        
        DATWafersCR_up,
        
        DATWafersEx_J,
        
        CardQualWafersCR_up,
        
        CardQualWafersEx_J,
        
        EngrSampleWafersCR_up,
        
        EngrSampleWafersEx_J,
        
        KGDParameterRelease,
        
        PrimaryForConsumption,
        
        FK_v_MDE_MDEPrimaryCons_Children,
    }
    
    public enum v_Nand {
        
        FK_Nand_Architecture_Parent,
        
        FK_Nand_BC_Parent,
        
        FK_Nand_Density_Parent,
        
        FK_Nand_Memory_Parent,
        
        FK_NANDRoadmapRev_v_Nand_Parent,
        
        FK_Technology_v_Nand_Parent,
        
        FK_NANDRoadmapRevName_v_Nand_Parent,
        
        FK_ECCType_v_Nand_Parent,
        
        FK_Endurance_Intial_v_Nand_Parent,
        
        Endurance_Target_v_Nand_Parent,
        
        Id,
        
        GDPW,
        
        Tapeout,
        
        MemoryName,
        
        DieSize,
        
        ECC,
        
        RawMemory,
        
        SiDensity,
        
        Tapeout1,
        
        Silicon,
        
        ESWaferOut,
        
        DieQual,
        
        CSWaferOut,
        
        ProductionOut,
        
        FK_POR_Nand_Children,
        
        FK_v_Nand_v_PLM_Children,
        
        FK_v_Nand_v_Test_Children,
        
        FK_v_Nand_v_SamplesRequest_Children,
    }
    
    public enum PlaneInt {
        
        Id,
        
        Name,
        
        FK_PLM_PlaneInt_Children,
        
        PlaneInt_TechnologyProductClassCapacity_Children,
        
        PlaneInt_v_EngMarketingReport_Children,
        
        PlaneInt_v_EngMarketingReport1_Children,
    }
    
    public enum v_PLM {
        
        FK_PLM_BusWidth_Parent,
        
        FK_PLM_ChipsFIM_Parent,
        
        FK_PLM_FIMCard_Parent,
        
        FK_PLM_HostBUS_Parent,
        
        FK_PLM_IdlePower_Parent,
        
        FK_PLM_PlaneInt_Parent,
        
        FK_PLM_PLMStatus_Parent,
        
        FK_PLM_SleepPower_Parent,
        
        FK_PLM_SpeedClass_Parent,
        
        FK_PLM_UHSGrade_Parent,
        
        FK_PLM_User_Parent,
        
        FK_PORStage_v_PLM_Parent,
        
        FK_ProductClass_v_PLM_Parent,
        
        FK_PORPriority_v_PLM_Parent,
        
        FK_PORChannel_v_PLM_Parent,
        
        FK_ASIC_v_PLM_Parent,
        
        FK_v_Nand_v_PLM_Parent,
        
        FK_BE_FW_FT_v_PLM_Parent,
        
        FK_BEGroup_v_PLM_Parent,
        
        FK_ProductLine_v_PLM_Parent,
        
        FK_ASICRevision_v_PLM_Parent,
        
        DieInt_v_PLM_Parent,
        
        FK_ProductCapacity_v_PLM_Parent,
        
        FK_v_NandMemoryMemoryRev_v_PLM_Parent,
        
        Id,
        
        Name,
        
        PLGroupNumber,
        
        RawPerformance,
        
        SequenceWrite,
        
        SequenceRead,
        
        MaxSequenceWrite,
        
        MaxSequenceRead,
        
        RandWrite,
        
        BandRead,
        
        BurstWrite,
        
        RecoveredWrite,
        
        NotesForBE,
        
        NotesForPLM,
        
        ConfigID,
        
        BEConfigID,
        
        ModifiedDate,
        
        ProjectId,
        
        ProjectCounter,
        
        RequiredACT,
        
        CommitACT,
        
        ForecastACT,
        
        ProjectDescription,
        
        TapeoutCommit,
        
        TapeoutForecast,
        
        Beta,
        
        RCRequired,
        
        RCCommit,
        
        RCForecast,
        
        ProjectCounter1,
        
        BE_ESReleaseRequired,
        
        ESReleaseRequired,
        
        ESReleaseCommitWeeks,
        
        BE_ESReleaseCommit,
        
        BE_ESReleaseCommitWeeks,
        
        BE_RCRequired,
        
        BEBetaRequired,
        
        TapeoutRequired,
        
        BlindBuildPackagePartsShipCommit,
        
        WaferOutCommit,
        
        Silicon,
        
        DieQual,
        
        TechnologyId,
        
        Dies,
        
        Power,
        
        SDA_MMCA,
        
        HostSpeed,
        
        FK_PLMHostSpeed_PLM_Children,
        
        FK_PLMPower_PLM_Children,
        
        FK_PLMSDA_MMCA_PLM_Children,
        
        FK_PLM_v_CommittedDatePLMToPOR_Children,
        
        FK_PLM_v_ForecastDatePLMToPOR_Children,
        
        FK_v_PLM_v_PORCapacity_Children,
        
        FK_v_PLM_v_RequiredTapeoutPLMFromASIC_Children,
    }
    
    public enum PLMHostSpeed {
        
        FK_PLMHostSpeed_HostSpeed_Parent,
        
        FK_PLMHostSpeed_PLM_Parent,
        
        Id,
    }
    
    public enum PLMPower {
        
        FK_PLMPower_PLM_Parent,
        
        FK_PLMPower_Power_Parent,
        
        Id,
    }
    
    public enum PLMSDA_MMCA {
        
        FK_PLMSDA_MMCA_PLM_Parent,
        
        FK_PLMSDA_MMCA_SDA_MMCA_Parent,
        
        Id,
    }
    
    public enum PLMStatus {
        
        Id,
        
        Name,
        
        FK_PLM_PLMStatus_Children,
    }
    
    public enum v_POR {
        
        FK_POR_ASIC_Parent,
        
        FK_POR_Nand_Parent,
        
        FK_POR_PORChannel_Parent,
        
        FK_POR_PORInitiator_Parent,
        
        FK_POR_PORPriority_Parent,
        
        FK_POR_PORStage_Parent,
        
        FK_POR_User_Parent,
        
        FK_POR_User1_Parent,
        
        ProductClass_POR_Parent,
        
        FK_ProductLine_v_POR_Parent,
        
        FK_Technology_v_POR_Parent,
        
        Id,
        
        Comment,
        
        MemLead,
        
        CtrlLead,
        
        CreateDate,
        
        ModifiedDate,
        
        RequiredACT,
        
        ComittedACT,
        
        ForecastACT,
        
        Status,
        
        Capacities,
        
        EccTypeId,
        
        PORId,
        
        StatusDescription,
        
        FK_PORCapacity_POR_Children,
        
        POR_PORProductClassCapacity_Children,
        
        FK_v_POR_v_RequestedDatePORFromPLM_Children,
    }
    
    public enum v_PORCapacity {
        
        FK_PORCapacity_POR_Parent,
        
        ProductCapacity_PORCapacity_Parent,
        
        FK_v_PLM_v_PORCapacity_Parent,
        
        FK_Test_v_PORCapacity_Parent,
        
        FK_v_SamplesRequest_v_PORCapacity_Parent,
        
        FK_SpeedClass_v_PORCapacity_Parent,
        
        FK_MemoryRev_v_PORCapacity_Parent,
        
        Id,
        
        ComittedACT,
        
        ForecastACT,
        
        Status,
        
        StatusDescription,
        
        SequenceWrite,
        
        SequenceRead,
        
        FK_Test_PORCapacity_Children,
    }
    
    public enum PORChannel {
        
        Id,
        
        Name,
        
        FK_POR_PORChannel_Children,
        
        FK_PORChannel_v_PLM_Children,
        
        FK_PORChannel_v_Test_Children,
        
        FK_BETechnical_PORChannel_Children,
        
        FK_PORChannel_v_SamplesRequest_Children,
    }
    
    public enum PORInitiator {
        
        Id,
        
        Name,
        
        FK_POR_PORInitiator_Children,
        
        FK_PORInitiator_v_Test_Children,
    }
    
    public enum PORPriority {
        
        Id,
        
        Name,
        
        FK_POR_PORPriority_Children,
        
        FK_PORPriority_v_PLM_Children,
    }
    
    public enum PORStage {
        
        Id,
        
        Name,
        
        Ordinal,
        
        FK_POR_PORStage_Children,
        
        FK_PORStage_v_PLM_Children,
        
        FK_PORStage_v_Test_Children,
        
        PORStage_v_EngMarketingReport_Children,
        
        FK_PORStage_v_SamplesRequest_Children,
    }
    
    public enum Power {
        
        Id,
        
        Name,
        
        FK_PLMPower_Power_Children,
        
        Power_TechnologyProductClassCapacityPower_Children,
    }
    
    public enum ReasonForChange {
        
        Id,
        
        Name,
        
        FK_DateChange_ReasonForChange_Children,
        
        FK_DateChange_ReasonForChange1_Children,
        
        FK_DateChange_ReasonForChange2_Children,
        
        FK_DateChange_ReasonForChange3_Children,
        
        FK_ReasonForChange_v_RequiredTapeoutPLMFromASIC_Children,
        
        FK_ReasonForChange_v_RequiredESPMMFromPLM_Children,
        
        FK_ReasonForChange_v_RequiredCSPMMFromPLM_Children,
    }
    
    public enum SDA_MMCA {
        
        Id,
        
        Name,
        
        FK_PLMSDA_MMCA_SDA_MMCA_Children,
        
        SDA_MMCA_TechnologyProductClassCapacitySDA_MMCA_Children,
    }
    
    public enum SleepPower {
        
        Id,
        
        Name,
        
        FK_PLM_SleepPower_Children,
        
        FK_ProductConfigurationParameters_SleepPower_Children,
        
        SleepPower_TechnologyProductClassCapacity_Children,
        
        SleepPower_v_EngMarketingReport_Children,
        
        SleepPower_v_EngMarketingReport1_Children,
    }
    
    public enum SpeedClass {
        
        Id,
        
        Name,
        
        FK_PLM_SpeedClass_Children,
        
        FK_ProductConfigurationParameters_SpeedClass_Children,
        
        FK_BETechnical_SpeedClass_Children,
        
        SpeedClass_TechnologyProductClassCapacity_Children,
        
        SpeedClass_v_EngMarketingReport_Children,
        
        SpeedClass_v_EngMarketingReport1_Children,
        
        FK_SpeedClass_v_PORCapacity_Children,
    }
    
    public enum v_Test {
        
        FK_Test_PORCapacity_Parent,
        
        FK_Test_TestFlow_Parent,
        
        FK_PORStage_v_Test_Parent,
        
        FK_PORInitiator_v_Test_Parent,
        
        FK_ProductClass_v_Test_Parent,
        
        FK_Technology_v_Test_Parent,
        
        FK_PORChannel_v_Test_Parent,
        
        FK_v_Nand_v_Test_Parent,
        
        FK_ASIC_v_Test_Parent,
        
        FK_ProductLine_v_Test_Parent,
        
        Id,
        
        MemWafers,
        
        CtrlWafers,
        
        RequiredACT,
        
        CapacityName,
        
        TestName,
        
        ProjectId,
        
        FK_TestTestTypeCommitDate_Test_Children,
        
        FK_Test_v_PORCapacity_Children,
    }
    
    public enum TestFlow {
        
        Id,
        
        Name,
        
        Description,
        
        Active,
        
        FK_Test_TestFlow_Children,
        
        FK_TestFlowType_TestFlow_Children,
    }
    
    public enum TestFlowType {
        
        FK_TestFlowType_TestFlow_Parent,
        
        FK_TestFlowType_TestType_Parent,
        
        Id,
        
        Ordinal,
    }
    
    public enum v_TestTestTypeCommitDate {
        
        FK_TestTestTypeCommitDate_Test_Parent,
        
        FK_TestType_v_TestTestTypeCommitDate_Parent,
        
        Id,
        
        CommitDate,
        
        RequiredDate,
    }
    
    public enum TestType {
        
        Id,
        
        Active,
        
        Name,
        
        LeadTime,
        
        Description,
        
        FK_TestType_v_TestTestTypeCommitDate_Children,
        
        FK_TestFlowType_TestType_Children,
    }
    
    public enum UHSGrade {
        
        Id,
        
        Name,
        
        FK_PLM_UHSGrade_Children,
        
        FK_ProductConfigurationParameters_UHSGrade_Children,
        
        UHSGrade_TechnologyProductClassCapacity_Children,
        
        UHSGrade_v_EngMarketingReport_Children,
        
        UHSGrade_v_EngMarketingReport1_Children,
    }
    
    public enum User {
        
        FK_User_UserRole_Parent,
        
        ID,
        
        Username,
        
        FirstName,
        
        LastName,
        
        Email,
        
        Password,
        
        Guid,
        
        Signature,
        
        SignatureHTML,
        
        FK_DateChange_User_Children,
        
        FK_PLM_User_Children,
        
        FK_POR_User_Children,
        
        FK_POR_User1_Children,
        
        FK_DateChange_User1_Children,
        
        FK_DateChange_User2_Children,
        
        FK_DateChange_User3_Children,
        
        FK_ASIC_User_Children,
        
        FK_ASIC_User1_Children,
        
        FK_User_v_RequiredTapeoutPLMFromASIC_Children,
        
        FK_User_v_RequiredESPMMFromPLM_Children,
        
        FK_User_v_RequiredCSPMMFromPLM_Children,
    }
    
    public enum v_BE_FW {
        
        FK_BE_FW_FT_BEGroup_Parent,
        
        FK_ProductLine_v_BE_FW_Parent,
        
        FK_BEReasonForChangeRCDate_v_BE_FW_Parent,
        
        FK_BEStatus_v_BE_FW_Parent,
        
        FK_BEGroup2_v_BE_FW_Parent,
        
        FK_BEReleaseType_v_BE_FW_Parent,
        
        v_ASICTechnicalSpec_v_BE_FW_Parent,
        
        Id,
        
        ProjectName,
        
        FC,
        
        Beta,
        
        RCRequired,
        
        RCCommit,
        
        RCForecast,
        
        FK_BE_FW_FT_v_PLM_Children,
        
        FK_v_BE_FW_BETechnical_Children,
    }
    
    public enum BEGroup {
        
        Id,
        
        Name,
        
        FK_BE_FW_FT_BEGroup_Children,
        
        FK_BEGroup_v_PLM_Children,
    }
    
    public enum ProductClass {
        
        FK_ProductClass_ProductLine_Parent,
        
        Id,
        
        Name,
        
        FK_ProductTechnology_ProductClass_Children,
        
        ProductClass_POR_Children,
        
        FK_ProductClass_v_PLM_Children,
        
        FK_ProductClass_v_Test_Children,
        
        ProductClass_v_EngMarketingReport_Children,
        
        FK_ProductClass_v_TechnologyProductClassCapacity_Children,
        
        FK_ProductClass_v_SamplesRequest_Children,
    }
    
    public enum ProductClassCapacity {
        
        FK_ProductTechnology_ProductClass_Parent,
        
        FK_ProductClassCapacity_ProductCapacity_Parent,
        
        Id,
        
        FK_TechnologyProductClassCapacity_ProductClassCapacity_Children,
    }
    
    public enum ProductLine {
        
        FK_ProductLine_MRD_Parent,
        
        Id,
        
        Name,
        
        FK_ProductClass_ProductLine_Children,
        
        FK_ProductLine_v_POR_Children,
        
        FK_ProductLine_ProductConfiguration_Children,
        
        FK_ProductLine_v_PLM_Children,
        
        FK_ProductLine_v_BE_FW_Children,
        
        FK_ProductLine_PLMGroupProductLine_Children,
        
        FK_ProductLine_v_Test_Children,
        
        FK_ProductLineParameter_ProductLine_Children,
        
        ProductLine_v_EngMarketingReport_Children,
        
        FK_ProductLine_v_TechnologyProductClassCapacity_Children,
        
        FK_ProductLine_v_SamplesRequest_Children,
        
        FK_ProductLine_v_MDE_Children,
        
        FK_MDEPrimaryCons_ProductLine_Children,
    }
    
    public enum ProductCapacity {
        
        Id,
        
        Name,
        
        Number,
        
        Ordinal,
        
        FK_ProductClassCapacity_ProductCapacity_Children,
        
        ProductCapacity_PORCapacity_Children,
        
        FK_PORProductClassCapacity_ProductCapacity_Children,
        
        ProductCapacity_v_EngMarketingReport_Children,
        
        FK_ProductCapacity_v_SamplesRequest_Children,
        
        FK_ProductCapacity_v_PLM_Children,
    }
    
    public enum PORProductClassCapacity {
        
        POR_PORProductClassCapacity_Parent,
        
        FK_PORProductClassCapacity_ProductCapacity_Parent,
        
        Id,
    }
    
    public enum Technology {
        
        Id,
        
        Name,
        
        FK_Technology_v_POR_Children,
        
        FK_ProductConfiguration_Technology_Children,
        
        FK_Technology_v_Test_Children,
        
        Technology_TechnologyProductClassCapacity_Children,
        
        Technology_v_EngMarketingReport_Children,
        
        FK_Technology_v_SamplesRequest_Children,
        
        FK_Technology_NANDRoadmapRev_Children,
        
        FK_Technology_v_Nand_Children,
        
        FK_Technology_v_MDE_Children,
    }
    
    public enum Memory {
        
        Id,
        
        Name,
        
        Memory,
        
        FK_Nand_Memory_Children,
        
        Memory_v_EngMarketingReport_Children,
        
        FK_Memory_v_NandMemoryMemoryRev_Children,
        
        FK_Memory_v_MDE_Children,
    }
    
    public enum v_RequestedDatePORFromPLM {
        
        FK_DateChange_DateType1_Parent,
        
        FK_DateChange_ReasonForChange1_Parent,
        
        FK_DateChange_User1_Parent,
        
        FK_v_POR_v_RequestedDatePORFromPLM_Parent,
        
        Id,
        
        Date,
        
        ReasonForChange,
        
        DateCreated,
    }
    
    public enum v_CommittedDatePLMToPOR {
        
        FK_DateChange_DateType2_Parent,
        
        FK_DateChange_ReasonForChange2_Parent,
        
        FK_DateChange_User2_Parent,
        
        FK_PLM_v_CommittedDatePLMToPOR_Parent,
        
        Id,
        
        Date,
        
        ReasonForChange,
        
        DateCreated,
    }
    
    public enum v_ForecastDatePLMToPOR {
        
        FK_DateChange_DateType3_Parent,
        
        FK_DateChange_ReasonForChange3_Parent,
        
        FK_DateChange_User3_Parent,
        
        FK_PLM_v_ForecastDatePLMToPOR_Parent,
        
        Id,
        
        Date,
        
        ReasonForChange,
        
        DateCreated,
    }
    
    public enum ProductConfiguration {
        
        FK_ProductConfiguration_Technology_Parent,
        
        FK_ProductLine_ProductConfiguration_Parent,
        
        Id,
    }
    
    public enum ProductConfigurationParameters {
        
        FK_ProductConfigurationParameters_BusWidth_Parent,
        
        FK_ProductConfigurationParameters_ChipsFIM_Parent,
        
        FK_ProductConfigurationParameters_FIMCard_Parent,
        
        FK_ProductConfigurationParameters_HostBUS_Parent,
        
        FK_ProductConfigurationParameters_IdlePower_Parent,
        
        FK_ProductConfigurationParameters_SleepPower_Parent,
        
        FK_ProductConfigurationParameters_SpeedClass_Parent,
        
        FK_ProductConfigurationParameters_UHSGrade_Parent,
        
        Id,
        
        ProductConfigurationId,
        
        RawPerformance,
        
        SequenceWrite,
        
        SequenceRead,
        
        MaxSequenceWrite,
        
        MaxSequenceRead,
        
        RandWrite,
        
        BandRead,
        
        BurstWrite,
        
        RecoveredWrite,
    }
    
    public enum DistributionList {
        
        Id,
        
        Name,
        
        FK_DistributionListRole_DistributionList_Children,
        
        FK_DistributionListUser_DistributionList_Children,
    }
    
    public enum DistributionListRole {
        
        FK_DistributionListRole_DistributionList_Parent,
        
        FK_v_UserRoleForDistributionList_DistributionListRole_Parent,
        
        Id,
    }
    
    public enum DistributionListUser {
        
        FK_DistributionListUser_DistributionList_Parent,
        
        FK_v_UserForDistributionList_DistributionListUser_Parent,
        
        Id,
        
        PointOfContact,
    }
    
    public enum v_UserForDistributionList {
        
        FK_v_UserRoleForDistributionList_v_UserForDistributionList_Parent,
        
        ID,
        
        Username,
        
        FirstName,
        
        LastName,
        
        Email,
        
        FK_v_UserForDistributionList_DistributionListUser_Children,
    }
    
    public enum v_UserRoleForDistributionList {
        
        Name,
        
        Description,
        
        FK_v_UserRoleForDistributionList_v_UserForDistributionList_Children,
        
        FK_v_UserRoleForDistributionList_DistributionListRole_Children,
    }
    
    public enum v_SamplesRequest {
        
        FK_ProductCapacity_v_SamplesRequest_Parent,
        
        FK_PORStage_v_SamplesRequest_Parent,
        
        FK_ProductClass_v_SamplesRequest_Parent,
        
        FK_PORChannel_v_SamplesRequest_Parent,
        
        FK_v_Nand_v_SamplesRequest_Parent,
        
        FK_v_ASIC_v_SamplesRequest_Parent,
        
        FK_Technology_v_SamplesRequest_Parent,
        
        FK_ProductLine_v_SamplesRequest_Parent,
        
        RequiredACT,
        
        PORId,
        
        Id,
        
        BE_ESReleaseCommit,
        
        RequiredES,
        
        RequiredCS,
        
        FK_v_SamplesRequest_v_RequiredCSPMMFromPLM_Children,
        
        FK_v_SamplesRequest_v_RequiredESPMMFromPLM_Children,
        
        FK_v_SamplesRequest_v_PORCapacity_Children,
    }
    
    public enum ASICController {
        
        FK_ASICController_ASICControllerFamily_Parent,
        
        Id,
        
        Name,
        
        FK_ASICTechnicalSpec_ASICController_Children,
        
        ASICController_v_ASIC_Children,
        
        ASICController_v_EngMarketingReport_Children,
    }
    
    public enum ASICControllerFamily {
        
        Id,
        
        Name,
        
        FK_ASICController_ASICControllerFamily_Children,
        
        ASICControllerFamily_v_ASIC_Children,
        
        FK_ASICControllerFamily_v_ASICTechnicalSpec_Children,
        
        FK_ASICControllerFamily_v_ASICRoadmap_Children,
    }
    
    public enum ASICRevision {
        
        Id,
        
        Name,
        
        FK_ASIC_ASICRevision1_Children,
        
        FK_ASICRevision_v_PLM_Children,
        
        FK_ASICRevision_v_ASICRoadmap2_Children,
    }
    
    public enum ASICSiliconDensity {
        
        Id,
        
        Name,
        
        FK_ASIC_ASICSiliconDensity_Children,
    }
    
    public enum ASICStatus {
        
        Id,
        
        Name,
        
        FK_ASIC_ASICStatus_Children,
    }
    
    public enum v_ASICTechnicalSpec {
        
        FK_ASICTechnicalSpec_ASICController_Parent,
        
        FK_ASICControllerFamily_v_ASICTechnicalSpec_Parent,
        
        Id,
        
        LeadProduct,
        
        ProjectDescription,
        
        ASICProjectWebpageURL,
        
        BESystemArchitecture,
        
        ProjectID,
        
        GDPW,
        
        FABTechnology,
        
        FK_ASIC_ASICTechnicalSpec_Children,
        
        v_ASICTechnicalSpec_v_BE_FW_Children,
    }
    
    public enum v_RequiredTapeoutPLMFromASIC {
        
        FK_v_PLM_v_RequiredTapeoutPLMFromASIC_Parent,
        
        FK_User_v_RequiredTapeoutPLMFromASIC_Parent,
        
        FK_ReasonForChange_v_RequiredTapeoutPLMFromASIC_Parent,
        
        FK_DateType_v_RequiredTapeoutPLMFromASIC_Parent,
        
        Id,
        
        Date,
        
        ReasonForChange,
        
        DateCreated,
    }
    
    public enum BE_FIM {
        
        Id,
        
        Name,
        
        FK_BETechnical_BE_FIM_Children,
    }
    
    public enum BEBitsCell {
        
        Id,
        
        Name,
        
        FK_BETechnical_BEBitsCell_Children,
    }
    
    public enum BEGroup2 {
        
        Id,
        
        Name,
        
        FK_BEGroup2_v_BE_FW_Children,
    }
    
    public enum BELabel {
        
        Id,
        
        Name,
        
        FK_BETechnical_BELabel_Children,
    }
    
    public enum BEMeasureDaily {
        
        Id,
        
        Name,
        
        FK_BETechnical_BEMeasureDaily_Children,
    }
    
    public enum BEMemory {
        
        Id,
        
        Name,
        
        FK_BETechnical_BEMemory_Children,
    }
    
    public enum BENANDBusWidth {
        
        Id,
        
        Name,
        
        FK_BETechnical_BENANDBusWidth_Children,
    }
    
    public enum BEReasonForChangeRCDate {
        
        Id,
        
        Name,
        
        FK_BEReasonForChangeRCDate_v_BE_FW_Children,
    }
    
    public enum BEReleaseType {
        
        Id,
        
        Name,
        
        FK_BEReleaseType_v_BE_FW_Children,
    }
    
    public enum BEStatus {
        
        Id,
        
        Name,
        
        FK_BEStatus_v_BE_FW_Children,
    }
    
    public enum BETechnical {
        
        FK_BETechnical_BE_FIM_Parent,
        
        FK_BETechnical_BEBitsCell_Parent,
        
        FK_BETechnical_BELabel_Parent,
        
        FK_BETechnical_BEMeasureDaily_Parent,
        
        FK_BETechnical_BEMemory_Parent,
        
        FK_BETechnical_BENANDBusWidth_Parent,
        
        FK_BETechnical_BETechnicalStatus_Parent,
        
        FK_BETechnical_ChipsFIM_Parent,
        
        FK_BETechnical_PORChannel_Parent,
        
        FK_BETechnical_SpeedClass_Parent,
        
        FK_v_BE_FW_BETechnical_Parent,
        
        Id,
        
        Release,
        
        GateToRelease,
        
        ActualReleaseDate,
        
        TestByExt,
        
        RegByExtID,
        
        PerfByExtID,
        
        PerfDailyID,
        
        ASICId,
        
        ProductLineId,
        
        DieCapacityId,
        
        CardCapacityId,
        
        DiesChipId,
        
        ParallelFolds,
        
        DieInterleave,
        
        PlaneInterleave,
        
        HostSpeed,
        
        TransferMode,
        
        RawPerf,
        
        SeqWrite,
        
        MaxSeqWrite,
        
        SeqRead,
        
        MaxSeqRead,
        
        RandWriteBurst,
        
        RandWriteRecovered,
        
        RandWriteSustained,
        
        RandRead,
        
        WHQL,
        
        PCMarkPerf,
        
        CrystalMarkSeqWrite,
        
        CrystalMarkSeqRead,
        
        CrystalMarkRandWrite4K,
        
        CrystalMarkRandRead4K,
        
        CrystalMarkRandWrite4KNCQ,
        
        CrystalMarkRandRead4KNCQ,
        
        CrystalMarkRandWrite512K,
        
        CrystalMarkRandRead512K,
        
        WritePower,
        
        ReadPower,
        
        SleepPower,
        
        IdlePower,
        
        Power8BitMode,
        
        HostBusWidth,
    }
    
    public enum BETechnicalStatus {
        
        Id,
        
        Name,
        
        FK_BETechnical_BETechnicalStatus_Children,
    }
    
    public enum PLMGroup {
        
        Id,
        
        Name,
        
        FK_PLMGroupProductLine_PLMGroup_Children,
    }
    
    public enum PLMGroupProductLine {
        
        FK_PLMGroupProductLine_PLMGroup_Parent,
        
        FK_ProductLine_PLMGroupProductLine_Parent,
        
        Id,
    }
    
    public enum v_ASICRoadmap {
        
        FK_ASICControllerFamily_v_ASICRoadmap_Parent,
        
        BESystemArchitecture,
        
        GDPW,
        
        FABTechnology,
        
        Id,
        
        FK_v_ASICRoadmap_v_ASICRoadmap2_Children,
    }
    
    public enum v_ASICRoadmap2 {
        
        FK_v_ASICRoadmap_v_ASICRoadmap2_Parent,
        
        FK_ASICRevision_v_ASICRoadmap2_Parent,
        
        ASICReqCompleteForecast,
        
        FPGAForROMDevForecast,
        
        TapeoutRequired,
        
        WaferOutForecast,
        
        BlindBuildPackagePartsShipForecast,
        
        TestedASICForProductQualForecast,
        
        ASICConditionalQualForecast,
        
        CharacterizationCompleteForecast,
        
        ASICFullQualForecast,
        
        Id,
    }
    
    public enum ProductionRevision {
        
        Id,
        
        Name,
        
        FK_ProductionRevision_v_MDE_Children,
    }
    
    public enum v_DistributionListEmail {
        
        Name,
        
        Email,
        
        PointOfContact,
    }
    
    public enum ProductLineParameter {
        
        FK_ProductLineParameter_ProductLine_Parent,
        
        FK_ProductLineParameter_Parameter_Parent,
        
        Id,
    }
    
    public enum Parameter {
        
        Id,
        
        DisplayName,
        
        Name,
        
        FK_ProductLineParameter_Parameter_Children,
    }
    
    public enum v_TechnologyProductClassCapacity {
        
        FK_TechnologyProductClassCapacity_ProductClassCapacity_Parent,
        
        Technology_TechnologyProductClassCapacity_Parent,
        
        ChipsFIM_TechnologyProductClassCapacity_Parent,
        
        FIMCard_TechnologyProductClassCapacity_Parent,
        
        HostBUS_TechnologyProductClassCapacity_Parent,
        
        BusWidth_TechnologyProductClassCapacity_Parent,
        
        SpeedClass_TechnologyProductClassCapacity_Parent,
        
        UHSGrade_TechnologyProductClassCapacity_Parent,
        
        SleepPower_TechnologyProductClassCapacity_Parent,
        
        IdlePower_TechnologyProductClassCapacity_Parent,
        
        PlaneInt_TechnologyProductClassCapacity_Parent,
        
        FK_TechnologyProductClassCapacity_DieInt_Parent,
        
        FK_ProductClass_v_TechnologyProductClassCapacity_Parent,
        
        FK_ProductLine_v_TechnologyProductClassCapacity_Parent,
        
        Id,
        
        RawPerformance,
        
        SequenceWrite,
        
        SequenceRead,
        
        MaxSequenceWrite,
        
        MaxSequenceRead,
        
        RandWrite,
        
        BandRead,
        
        BurstWrite,
        
        RecoveredWrite,
        
        Power,
        
        HostSpeed,
        
        SDA_MMCA,
        
        TechnologyProductClassCapacity_TechnologyProductClassCapacityHostSpeed_Children,
        
        TechnologyProductClassCapacity_TechnologyProductClassCapacityPower_Children,
        
        TechnologyProductClassCapacity_TechnologyProductClassCapacitySDA_MMCA_Children,
    }
    
    public enum DieInt {
        
        Id,
        
        Number,
        
        FK_TechnologyProductClassCapacity_DieInt_Children,
        
        DieInt_v_EngMarketingReport_Children,
        
        DieInt_v_EngMarketingReport1_Children,
        
        DieInt_v_PLM_Children,
    }
    
    public enum v_EngMarketingReport {
        
        PORStage_v_EngMarketingReport_Parent,
        
        ProductCapacity_v_EngMarketingReport_Parent,
        
        Memory_v_EngMarketingReport_Parent,
        
        ASICController_v_EngMarketingReport_Parent,
        
        ChipsFIM_v_EngMarketingReport_Parent,
        
        ChipsFIM_v_EngMarketingReport1_Parent,
        
        FIMCard_v_EngMarketingReport_Parent,
        
        FIMCard_v_EngMarketingReport1_Parent,
        
        HostBUS_v_EngMarketingReport_Parent,
        
        HostBUS_v_EngMarketingReport1_Parent,
        
        BusWidth_v_EngMarketingReport_Parent,
        
        SpeedClass_v_EngMarketingReport_Parent,
        
        SpeedClass_v_EngMarketingReport1_Parent,
        
        UHSGrade_v_EngMarketingReport_Parent,
        
        UHSGrade_v_EngMarketingReport1_Parent,
        
        SleepPower_v_EngMarketingReport_Parent,
        
        SleepPower_v_EngMarketingReport1_Parent,
        
        IdlePower_v_EngMarketingReport_Parent,
        
        IdlePower_v_EngMarketingReport1_Parent,
        
        PlaneInt_v_EngMarketingReport_Parent,
        
        PlaneInt_v_EngMarketingReport1_Parent,
        
        DieInt_v_EngMarketingReport_Parent,
        
        DieInt_v_EngMarketingReport1_Parent,
        
        BusWidth_v_EngMarketingReport1_Parent,
        
        Technology_v_EngMarketingReport_Parent,
        
        ProductLine_v_EngMarketingReport_Parent,
        
        ProductClass_v_EngMarketingReport_Parent,
        
        idCounter,
        
        PLMId,
        
        TechnologyProductClassCapacityId,
        
        Dies,
        
        PMM_RawPerformance,
        
        Eng_RawPerformance,
        
        PMM_SequenceWrite,
        
        Eng_SequenceWrite,
        
        PMM_SequenceRead,
        
        Eng_SequenceRead,
        
        PMM_MaxSequenceWrite,
        
        Eng_MaxSequenceWrite,
        
        PMM_MaxSequenceRead,
        
        Eng_MaxSequenceRead,
        
        PMM_RandWrite,
        
        Eng_RandWrite,
        
        PMM_BandRead,
        
        Eng_BandRead,
        
        PMM_BurstWrite,
        
        Eng_BurstWrite,
        
        PMM_RecoveredWrite,
        
        Eng_RecoveredWrite,
        
        PMM_HostSpeed,
        
        Eng_HostSpeed,
        
        PMM_Power,
        
        Eng_Power,
        
        PMM_SDA_MMCA,
        
        Eng_SDA_MMCA,
    }
    
    public enum TechnologyProductClassCapacityHostSpeed {
        
        TechnologyProductClassCapacity_TechnologyProductClassCapacityHostSpeed_Parent,
        
        HostSpeed_TechnologyProductClassCapacityHostSpeed_Parent,
        
        Id,
    }
    
    public enum TechnologyProductClassCapacityPower {
        
        TechnologyProductClassCapacity_TechnologyProductClassCapacityPower_Parent,
        
        Power_TechnologyProductClassCapacityPower_Parent,
        
        Id,
    }
    
    public enum TechnologyProductClassCapacitySDA_MMCA {
        
        TechnologyProductClassCapacity_TechnologyProductClassCapacitySDA_MMCA_Parent,
        
        SDA_MMCA_TechnologyProductClassCapacitySDA_MMCA_Parent,
        
        Id,
    }
    
    public enum v_RequiredCSPMMFromPLM {
        
        FK_v_SamplesRequest_v_RequiredCSPMMFromPLM_Parent,
        
        FK_ReasonForChange_v_RequiredCSPMMFromPLM_Parent,
        
        FK_DateType_v_RequiredCSPMMFromPLM_Parent,
        
        FK_User_v_RequiredCSPMMFromPLM_Parent,
        
        Date,
        
        ReasonForChange,
        
        DateCreated,
        
        Id,
    }
    
    public enum v_RequiredESPMMFromPLM {
        
        FK_v_SamplesRequest_v_RequiredESPMMFromPLM_Parent,
        
        FK_ReasonForChange_v_RequiredESPMMFromPLM_Parent,
        
        FK_User_v_RequiredESPMMFromPLM_Parent,
        
        FK_DateType_v_RequiredESPMMFromPLM_Parent,
        
        Date,
        
        ReasonForChange,
        
        DateCreated,
        
        Id,
    }
    
    public enum MRD {
        
        Id,
        
        MRD,
        
        Title,
        
        Comments,
        
        FK_ProductLine_MRD_Children,
    }
    
    public enum MemoryRev {
        
        Id,
        
        Name,
        
        FK_MemoryRev_v_NandMemoryMemoryRev_Children,
        
        FK_MemoryRev_v_PORCapacity_Children,
    }
    
    public enum v_NANDRoadmapRev {
        
        FK_RoadmapRev_RoadmapRevName_Parent,
        
        FK_Technology_NANDRoadmapRev_Parent,
        
        Id,
        
        RevName,
        
        FK_NANDRoadmapRev_v_Nand_Children,
    }
    
    public enum NANDRoadmapRevName {
        
        Id,
        
        Number,
        
        LastRev,
        
        FK_RoadmapRev_RoadmapRevName_Children,
        
        FK_NANDRoadmapRevName_v_Nand_Children,
    }
    
    public enum v_NandMemoryMemoryRev {
        
        FK_Memory_v_NandMemoryMemoryRev_Parent,
        
        FK_MemoryRev_v_NandMemoryMemoryRev_Parent,
        
        Id,
        
        FK_v_NandMemoryMemoryRev_v_PLM_Children,
    }
    
    public enum ProcessCondition {
        
        Id,
        
        Name,
        
        FK_ProcessCondition_v_MDE_Children,
    }
    
    public enum MDEPrimaryCons {
        
        FK_MDEPrimaryCons_ProductLine_Parent,
        
        FK_v_MDE_MDEPrimaryCons_Parent,
        
        Id,
    }
}

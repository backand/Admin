// Enum for Shade
namespace Durados.Web.Mvc.Specifics.Projects.CRMShade {
    using System;
    
    
    public enum ShadeViews {
        
        V_Contact,
        
        CountryType,
        
        durados_Html,
        
        IndustryType,
        
        v_JobMemberCommission,
        
        JobStatusCategoryType,
        
        JobStatusType,
        
        JobStatusUpdateTime,
        
        v_JobVendor,
        
        LeadSourceType,
        
        MemberType,
        
        Organization,
        
        OrgType,
        
        OrgStatusType,
        
        v_Proposal,
        
        ProposalItemIBOB,
        
        ProposalItemType,
        
        StatusUpdateTime,
        
        Task,
        
        TaskPriorityType,
        
        TaskType,
        
        TaskStatusType,
        
        Template,
        
        Time,
        
        User,
        
        v_TaskOpen,
        
        v_TasksAll,
        
        Job,
        
        ProposalItem,
        
        V_Vendor,
        
        Fruction,
        
        Product,
        
        ProductCategory,
        
        ProductPriceCategory,
        
        ProductPrice,
        
        Payment,
        
        PaymentType,
        
        Tax,
        
        JobNotes,
        
        v_JobTotal,
        
        v_ProposalTotal,
        
        Durados_Log,
        
        UserRole,
        
        JobVendorDocumentType,
        
        JobVendorDocument,
        
        v_TaskAlert,
        
        InstallationType,
        
        MotorizedSolution,
        
        WindowTreatmentRoom,
        
        WindowsSize,
        
        Address,
        
        AddressDirectionType,
        
        AddressStreetType,
        
        CountryStateType,
        
        V_BuildingManagement,
        
        Block,
    }
    
    public enum V_Contact {
        
        FK_Contact_Contact_Parent,
        
        FK_Contact_LeadSourceType_Parent,
        
        FK_Contact_Organization_Parent,
        
        FK_Contact_User_Parent,
        
        User_V_Contact_Parent,
        
        FK_Address_V_Contact_Parent,
        
        FK_AddressOther_V_Contact_Parent,
        
        Id,
        
        FirstName,
        
        LastName,
        
        Phone,
        
        Cellular,
        
        Email,
        
        Title,
        
        Description,
        
        MailingStreet,
        
        MailingCity,
        
        MailingState,
        
        MailingZip,
        
        MailingCountry,
        
        OtherStreet,
        
        OtherCity,
        
        OtherState,
        
        OtherZip,
        
        OtherCountry,
        
        Birthday,
        
        FullName,
        
        d_LastUpdateDate,
        
        FK_Contact_Contact_Children,
        
        FK_Task_Contact_Children,
        
        FK_V_Contact_Job_Children,
        
        FK_V_Contact_v_TaskOpen_Children,
        
        FK_V_Contact_v_TasksAll_Children,
        
        V_Contact_JobTotal_Children,
        
        V_Contact_v_TaskAlert_Children,
    }
    
    public enum CountryType {
        
        Id,
        
        Country,
        
        CountryNotes,
        
        Active,
        
        FK_CountryType_CountryStateType_Children,
    }
    
    public enum durados_Html {
        
        Name,
        
        Text,
    }
    
    public enum IndustryType {
        
        Id,
        
        Name,
        
        FK_Organization_IndustryType_Children,
    }
    
    public enum v_JobMemberCommission {
        
        FK_JobMember_MemberType_Parent,
        
        FK_JobMember_User_Parent,
        
        FK_JobMember_Job_Parent,
        
        Id,
        
        Note,
        
        Hours,
        
        HourRate,
        
        Percents,
        
        Commision,
        
        AdditionalCommission,
    }
    
    public enum JobStatusCategoryType {
        
        Id,
        
        Name,
        
        FK_JobStatusType_JobStatusCategoryType_Children,
        
        JobStatusCategoryType_Job_Children,
    }
    
    public enum JobStatusType {
        
        FK_JobStatusType_JobStatusCategoryType_Parent,
        
        Id,
        
        Name,
        
        FK_Job_JobStatusType_Children,
        
        FK_Job_JobStatusType1_Children,
    }
    
    public enum JobStatusUpdateTime {
        
        FK_JobStatusUpdateTime_StatusUpdateTime_Parent,
        
        FK_JobStatusUpdateTime_Job_Parent,
        
        Id,
    }
    
    public enum v_JobVendor {
        
        FK_JobVendor_Job_Parent,
        
        FK_V_Vendor_JobVendor_Parent,
        
        Id,
        
        VendorCostDocument,
        
        OrderDocument,
        
        TotalCost,
        
        Description,
        
        v_JobVendor_JobVendorDocument_Children,
    }
    
    public enum LeadSourceType {
        
        Id,
        
        Name,
        
        FK_Contact_LeadSourceType_Children,
        
        FK_LeadSourceType_Job_Children,
    }
    
    public enum MemberType {
        
        Id,
        
        Name,
        
        FK_JobMember_MemberType_Children,
    }
    
    public enum Organization {
        
        FK_Organization_IndustryType_Parent,
        
        FK_Organization_Organization_Parent,
        
        FK_Organization_OrgStatusType_Parent,
        
        FK_Organization_OrgType_Parent,
        
        User_Organization_Parent,
        
        FK_Organization_Address_Parent,
        
        FK_Organization_Address1_Parent,
        
        Id,
        
        Name,
        
        BillingStreet,
        
        BillingCity,
        
        BillingState,
        
        BillingZip,
        
        BillingCountry,
        
        Phone,
        
        Fax,
        
        Website,
        
        ShippingStreet,
        
        ShippingCity,
        
        ShippingState,
        
        ShippingZip,
        
        ShippingCountry,
        
        Description,
        
        Employees,
        
        AnnualRevenue,
        
        d_LastUpdateDate,
        
        FK_Contact_Organization_Children,
        
        FK_Organization_Organization_Children,
        
        FK_Task_Organization_Children,
        
        FK_Task_Organization1_Children,
        
        FK_Task_Organization2_Children,
        
        FK_Job_Organization_Children,
        
        Organization_v_TaskAlert_Children,
    }
    
    public enum OrgType {
        
        Id,
        
        Name,
        
        FK_Organization_OrgType_Children,
        
        OrgType_V_Vendor_Children,
        
        FK_OrgType_V_BuildingManagement_Children,
    }
    
    public enum OrgStatusType {
        
        Id,
        
        Name,
        
        FK_Organization_OrgStatusType_Children,
        
        FK_OrgStatusType_V_BuildingManagement_Children,
    }
    
    public enum v_Proposal {
        
        FK_Proposal_Template_Parent,
        
        FK_Proposal_Job_Parent,
        
        Tax_Proposal_Parent,
        
        User_v_Proposal_Parent,
        
        Id,
        
        Subject,
        
        Date,
        
        WordDocument,
        
        PDFDocument,
        
        Active,
        
        SubTotal,
        
        Total,
        
        TotalCost,
        
        TotalTax,
        
        d_LastUpdateDate,
        
        DataColumn1,
        
        FK_ProposalItem_Proposal_Children,
        
        v_Proposal_v_ProposalTotal_Children,
    }
    
    public enum ProposalItemIBOB {
        
        Id,
        
        Name,
        
        FK_ProposalItem_ProposalItemIBOB_Children,
    }
    
    public enum ProposalItemType {
        
        id,
        
        Name,
        
        FK_ProposalItem_ProposalItemType_Children,
    }
    
    public enum StatusUpdateTime {
        
        Id,
        
        StartStatusId,
        
        EndStatusId,
        
        Date,
        
        EndDate,
        
        FK_JobStatusUpdateTime_StatusUpdateTime_Children,
    }
    
    public enum Task {
        
        FK_Task_Contact_Parent,
        
        FK_Task_Organization_Parent,
        
        FK_Task_TaskPriorityType_Parent,
        
        FK_Task_TaskStatusType_Parent,
        
        FK_Task_TaskType_Parent,
        
        FK_Task_Time_Parent,
        
        FK_Task_User_Parent,
        
        Id,
        
        Subject,
        
        DueDate,
        
        Comments,
        
        ReminderDate,
        
        SendNotificationEmail,
        
        ReminderSentDate,
        
        FK_Task_Job_Children,
        
        FK_Job_Task_Children,
    }
    
    public enum TaskPriorityType {
        
        Id,
        
        Name,
        
        FK_Task_TaskPriorityType_Children,
        
        FK_Task_TaskPriorityType1_Children,
        
        FK_Task_TaskPriorityType2_Children,
        
        TaskPriorityType_v_TaskAlert_Children,
    }
    
    public enum TaskType {
        
        Id,
        
        Name,
        
        FK_Task_TaskType_Children,
        
        FK_Task_TaskType1_Children,
        
        FK_Task_TaskType2_Children,
        
        TaskType_v_TaskAlert_Children,
    }
    
    public enum TaskStatusType {
        
        Id,
        
        Name,
        
        FK_Task_TaskStatusType_Children,
        
        FK_Task_TaskStatusType1_Children,
        
        FK_Task_TaskStatusType2_Children,
        
        TaskStatusType_v_TaskAlert_Children,
    }
    
    public enum Template {
        
        Id,
        
        Name,
        
        DocumentLocation,
        
        FK_Proposal_Template_Children,
    }
    
    public enum Time {
        
        Hour,
        
        Military,
        
        USA,
        
        FK_Task_Time_Children,
        
        FK_Task_Time1_Children,
        
        FK_Task_Time2_Children,
        
        Time_v_TaskAlert_Children,
    }
    
    public enum User {
        
        UserRole_User_Parent,
        
        ID,
        
        Username,
        
        FirstName,
        
        LastName,
        
        Email,
        
        Password,
        
        Guid,
        
        FK_Contact_User_Children,
        
        FK_JobMember_User_Children,
        
        FK_Task_User_Children,
        
        FK_Task_User1_Children,
        
        FK_Task_User2_Children,
        
        FK_User_Job_Children,
        
        FK_JobNotes_User_Children,
        
        User_Job_Children,
        
        User_V_Contact_Children,
        
        User_v_Proposal_Children,
        
        User_Organization_Children,
        
        User_v_TaskAlert_Children,
    }
    
    public enum v_TaskOpen {
        
        FK_Task_Organization1_Parent,
        
        FK_Task_TaskPriorityType1_Parent,
        
        FK_Task_TaskStatusType1_Parent,
        
        FK_Task_TaskType1_Parent,
        
        FK_Task_Time1_Parent,
        
        FK_Task_User1_Parent,
        
        FK_V_Contact_v_TaskOpen_Parent,
        
        Id,
        
        Subject,
        
        DueDate,
        
        Comments,
        
        ReminderDate,
        
        SendNotificationEmail,
        
        ReminderSentDate,
    }
    
    public enum v_TasksAll {
        
        FK_Task_Organization2_Parent,
        
        FK_Task_TaskPriorityType2_Parent,
        
        FK_Task_TaskStatusType2_Parent,
        
        FK_Task_TaskType2_Parent,
        
        FK_Task_Time2_Parent,
        
        FK_Task_User2_Parent,
        
        FK_V_Contact_v_TasksAll_Parent,
        
        Id,
        
        Subject,
        
        DueDate,
        
        Comments,
        
        ReminderDate,
        
        SendNotificationEmail,
        
        ReminderSentDate,
    }
    
    public enum Job {
        
        FK_Job_JobStatusType_Parent,
        
        FK_Job_Organization_Parent,
        
        FK_V_Contact_Job_Parent,
        
        FK_User_Job_Parent,
        
        FK_Task_Job_Parent,
        
        JobStatusCategoryType_Job_Parent,
        
        User_Job_Parent,
        
        FK_Job_InstallationType_Parent,
        
        FK_Job_MotorizedSolution_Parent,
        
        FK_Job_WindowsSize_Parent,
        
        FK_Job_WindowTreatmentRoom_Parent,
        
        FK_LeadSourceType_Job_Parent,
        
        FK_ProductCategory_Job_Parent,
        
        FK_Job_Address_Parent,
        
        Id,
        
        Name,
        
        Description,
        
        ClientName,
        
        ClientPhone,
        
        ClientCellular,
        
        ClientEmail,
        
        JobStreet,
        
        JobCity,
        
        JobState,
        
        JobZip,
        
        JobCountry,
        
        CreationDate,
        
        d_LastUpdateDate,
        
        COIDocument,
        
        WorkersCompDocument,
        
        LeadInfoText,
        
        NumWindowsFloorsFrom,
        
        NumWindowsFloorsTo,
        
        WindowsSizeText,
        
        WindowTreatmentText,
        
        WindowTreatmentRoomsText,
        
        SpaceDescription,
        
        LeadAddress,
        
        FK_JobMember_Job_Children,
        
        FK_JobStatusUpdateTime_Job_Children,
        
        FK_JobVendor_Job_Children,
        
        FK_Proposal_Job_Children,
        
        FK_Payment_Payment_Children,
        
        FK_JobNotes_Job_Children,
        
        Job_v_JobTotal_Children,
    }
    
    public enum ProposalItem {
        
        FK_ProposalItem_Proposal_Parent,
        
        FK_ProposalItem_ProposalItemIBOB_Parent,
        
        FK_ProposalItem_ProposalItemType_Parent,
        
        FK_ProposalItem_Vndor_Parent,
        
        FK_ProposalItem_Fruction_Parent,
        
        FK_ProposalItem_Fruction1_Parent,
        
        FK_ProposalItem_Product_Parent,
        
        id,
        
        Description,
        
        Width,
        
        Height,
        
        Seamed,
        
        Color,
        
        Qty,
        
        Rate,
        
        Total,
        
        Cost,
        
        TotalDiscount,
    }
    
    public enum V_Vendor {
        
        OrgType_V_Vendor_Parent,
        
        FK_ShippingAddress_V_Vendor_Parent,
        
        FK_BillingAddress_V_Vendor_Parent,
        
        Id,
        
        OrgStatusId,
        
        Name,
        
        ParentOrgId,
        
        BillingStreet,
        
        BillingCity,
        
        BillingState,
        
        BillingZip,
        
        BillingCountry,
        
        Phone,
        
        Fax,
        
        Website,
        
        ShippingStreet,
        
        ShippingCity,
        
        ShippingState,
        
        ShippingZip,
        
        ShippingCountry,
        
        Description,
        
        Multiply,
        
        FK_V_Vendor_JobVendor_Children,
        
        FK_ProposalItem_Vndor_Children,
    }
    
    public enum Fruction {
        
        Id,
        
        Name,
        
        FK_ProposalItem_Fruction_Children,
        
        FK_ProposalItem_Fruction1_Children,
    }
    
    public enum Product {
        
        FK_Product_ProductCategory_Parent,
        
        FK_Product_ProductPriceCategory_Parent,
        
        Id,
        
        Name,
        
        Description,
        
        FK_ProposalItem_Product_Children,
    }
    
    public enum ProductCategory {
        
        Id,
        
        Name,
        
        FK_Product_ProductCategory_Children,
        
        FK_ProductCategory_Job_Children,
    }
    
    public enum ProductPriceCategory {
        
        Id,
        
        Name,
        
        FK_Product_ProductPriceCategory_Children,
        
        FK_ProductPrice_ProductCategory_Children,
    }
    
    public enum ProductPrice {
        
        FK_ProductPrice_ProductCategory_Parent,
        
        Id,
        
        Width,
        
        Height,
        
        Cost,

        Price,
        
        Seamed,
    }
    
    public enum Payment {
        
        FK_Payment_Payment_Parent,
        
        FK_Payment_PaymentType_Parent,
        
        ID,
        
        Amount,
        
        Date,
    }
    
    public enum PaymentType {
        
        ID,
        
        Name,
        
        FK_Payment_PaymentType_Children,
    }
    
    public enum Tax {
        
        ID,
        
        Region,
        
        Value,
        
        Tax_Proposal_Children,
    }
    
    public enum JobNotes {
        
        FK_JobNotes_Job_Parent,
        
        FK_JobNotes_User_Parent,
        
        ID,
        
        Note,
        
        Created,
        
        Auto,
    }
    
    public enum v_JobTotal {
        
        FK_Job_JobStatusType1_Parent,
        
        FK_Job_Task_Parent,
        
        V_Contact_JobTotal_Parent,
        
        Job_v_JobTotal_Parent,
        
        OrgId,
        
        SalesUserId,
        
        Name,
        
        Description,
        
        ClientName,
        
        ClientPhone,
        
        ClientCellular,
        
        ClientEmail,
        
        JobStreet,
        
        JobCity,
        
        JobState,
        
        JobZip,
        
        JobCountry,
        
        TotalBalance,
        
        TotalCost,
        
        TotalCommisions,
        
        TotalPayments,
    }
    
    public enum v_ProposalTotal {
        
        v_Proposal_v_ProposalTotal_Parent,
        
        SubTotal,
        
        Total,
        
        TotalCost,
        
        Tax,
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
    
    public enum UserRole {
        
        Name,
        
        Description,
        
        UserRole_User_Children,
    }
    
    public enum JobVendorDocumentType {
        
        ID,
        
        Name,
        
        FK_JobVendorDocument_JobVendorDocumentType_Children,
    }
    
    public enum JobVendorDocument {
        
        FK_JobVendorDocument_JobVendorDocumentType_Parent,
        
        v_JobVendor_JobVendorDocument_Parent,
        
        ID,
        
        Doc,
        
        Description,
    }
    
    public enum v_TaskAlert {
        
        Organization_v_TaskAlert_Parent,
        
        TaskPriorityType_v_TaskAlert_Parent,
        
        TaskStatusType_v_TaskAlert_Parent,
        
        TaskType_v_TaskAlert_Parent,
        
        Time_v_TaskAlert_Parent,
        
        User_v_TaskAlert_Parent,
        
        V_Contact_v_TaskAlert_Parent,
        
        Id,
        
        Subject,
        
        DueDate,
        
        Comments,
        
        ReminderDate,
        
        SendNotificationEmail,
        
        ReminderSentDate,
    }
    
    public enum InstallationType {
        
        Id,
        
        Name,
        
        FK_Job_InstallationType_Children,
    }
    
    public enum MotorizedSolution {
        
        Id,
        
        Name,
        
        FK_Job_MotorizedSolution_Children,
    }
    
    public enum WindowTreatmentRoom {
        
        Id,
        
        Name,
        
        FK_Job_WindowTreatmentRoom_Children,
    }
    
    public enum WindowsSize {
        
        Id,
        
        Name,
        
        FK_Job_WindowsSize_Children,
    }
    
    public enum Address {
        
        FK_Address_AddressDirectionType_Parent,
        
        FK_Address_AddressStreetType_Parent,
        
        FK_CountryStateType_Address_Parent,
        
        FK_Address_V_BuildingManagement_Parent,
        
        Id,
        
        Number,
        
        Street,
        
        Unit,
        
        City,
        
        ZipCode,
        
        FullAddress,
        
        FK_Address_V_Contact_Children,
        
        FK_AddressOther_V_Contact_Children,
        
        FK_ShippingAddress_V_Vendor_Children,
        
        FK_BillingAddress_V_Vendor_Children,
        
        FK_Organization_Address_Children,
        
        FK_Organization_Address1_Children,
        
        FK_Job_Address_Children,
    }
    
    public enum AddressDirectionType {
        
        Id,
        
        Name,
        
        FK_Address_AddressDirectionType_Children,
    }
    
    public enum AddressStreetType {
        
        Id,
        
        Name,
        
        FK_Address_AddressStreetType_Children,
    }
    
    public enum CountryStateType {
        
        FK_CountryType_CountryStateType_Parent,
        
        Id,
        
        Code,
        
        Name,
        
        FK_CountryStateType_Address_Children,
    }
    
    public enum V_BuildingManagement {
        
        FK_OrgStatusType_V_BuildingManagement_Parent,
        
        FK_OrgType_V_BuildingManagement_Parent,
        
        Id,
        
        Name,
        
        Phone,
        
        Fax,
        
        Website,
        
        Description,
        
        FK_Address_V_BuildingManagement_Children,
    }
    
    public enum Block {
        
        Tag,
    }
}

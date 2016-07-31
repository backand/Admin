// Enum for Shade
namespace Durados.Web.Mvc.Specifics.Shade.CRM {
    using System;
    
    
    public enum ShadeViews {
        
        V_Contact,
        
        CountryType,
        
        durados_Html,
        
        IndustryType,
        
        JobMember,
        
        JobStatusCategoryType,
        
        JobStatusType,
        
        JobStatusUpdateTime,
        
        JobVendor,
        
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
    }
    
    public enum V_Contact {
        
        FK_Contact_Contact_Parent,
        
        FK_Contact_LeadSourceType_Parent,
        
        FK_Contact_Organization_Parent,
        
        FK_Contact_User_Parent,
        
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
        
        FK_Contact_Contact_Children,
        
        FK_Task_Contact_Children,
        
        FK_V_Contact_Job_Children,
        
        FK_V_Contact_v_TaskOpen_Children,
        
        FK_V_Contact_v_TasksAll_Children,
    }
    
    public enum CountryType {
        
        Id,
        
        Country,
        
        CountryNotes,
        
        Active,
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
    
    public enum JobMember {
        
        FK_JobMember_MemberType_Parent,
        
        FK_JobMember_User_Parent,
        
        FK_JobMember_Job_Parent,
        
        Id,
        
        Note,
        
        Hours,
        
        HourRate,
        
        Percents,
        
        AddedAmount,
    }
    
    public enum JobStatusCategoryType {
        
        Id,
        
        Name,
        
        FK_JobStatusType_JobStatusCategoryType_Children,
    }
    
    public enum JobStatusType {
        
        FK_JobStatusType_JobStatusCategoryType_Parent,
        
        Id,
        
        Name,
        
        FK_Job_JobStatusType_Children,
    }
    
    public enum JobStatusUpdateTime {
        
        FK_JobStatusUpdateTime_StatusUpdateTime_Parent,
        
        FK_JobStatusUpdateTime_Job_Parent,
        
        Id,
    }
    
    public enum JobVendor {
        
        FK_JobVendor_Job_Parent,
        
        FK_V_Vendor_JobVendor_Parent,
        
        Id,
        
        VendorCostDocument,
        
        OrderDocument,
        
        TotalCost,
        
        Description,
    }
    
    public enum LeadSourceType {
        
        Id,
        
        Name,
        
        FK_Contact_LeadSourceType_Children,
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
        
        FK_Contact_Organization_Children,
        
        FK_Organization_Organization_Children,
        
        FK_Task_Organization_Children,
        
        FK_Task_Organization1_Children,
        
        FK_Task_Organization2_Children,
        
        FK_Job_Organization_Children,
    }
    
    public enum OrgType {
        
        Id,
        
        Name,
        
        FK_Organization_OrgType_Children,
    }
    
    public enum OrgStatusType {
        
        Id,
        
        Name,
        
        FK_Organization_OrgStatusType_Children,
    }
    
    public enum v_Proposal {
        
        FK_Proposal_Template_Parent,
        
        FK_Proposal_Job_Parent,
        
        Id,
        
        Subject,
        
        Date,
        
        WordDocument,
        
        PDFDocument,
        
        Active,
        
        Total,
        
        SubTotal,
        
        TotalCost,
        
        Tax,
        
        FK_ProposalItem_Proposal_Children,
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
    }
    
    public enum TaskPriorityType {
        
        Id,
        
        Name,
        
        FK_Task_TaskPriorityType_Children,
        
        FK_Task_TaskPriorityType1_Children,
        
        FK_Task_TaskPriorityType2_Children,
    }
    
    public enum TaskType {
        
        Id,
        
        Name,
        
        FK_Task_TaskType_Children,
        
        FK_Task_TaskType1_Children,
        
        FK_Task_TaskType2_Children,
    }
    
    public enum TaskStatusType {
        
        Id,
        
        Name,
        
        FK_Task_TaskStatusType_Children,
        
        FK_Task_TaskStatusType1_Children,
        
        FK_Task_TaskStatusType2_Children,
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
    }
    
    public enum User {
        
        ID,
        
        Username,
        
        FirstName,
        
        LastName,
        
        Email,
        
        Password,
        
        Role,
        
        FK_Contact_User_Children,
        
        FK_JobMember_User_Children,
        
        FK_Task_User_Children,
        
        FK_Task_User1_Children,
        
        FK_Task_User2_Children,
        
        FK_User_Job_Children,
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
        
        Id,
        
        Name,
        
        Description,
        
        TotalBalance,
        
        BalanceDue,
        
        Payments,
        
        ClientName,
        
        ClientPhone,
        
        ClientCellular,
        
        ClientEmail,
        
        JobStreet,
        
        JobCity,
        
        JobState,
        
        JobZip,
        
        JobCountry,
        
        FK_JobMember_Job_Children,
        
        FK_JobStatusUpdateTime_Job_Children,
        
        FK_JobVendor_Job_Children,
        
        FK_Proposal_Job_Children,
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
        
        Seamed,
    }
}

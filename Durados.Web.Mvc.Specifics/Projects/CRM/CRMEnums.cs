// Enum for CRM
namespace Durados.Web.Mvc.Specifics.Projects.CRM {
    using System;
    
    
    public enum CRMViews {
        
        V_Contact,
        
        IndustryType,
        
        LeadSourceType,
        
        Organization,
        
        OrgStatusType,
        
        OrgType,
        
        Proposal,
        
        Task,
        
        TaskPriorityType,
        
        TaskStatusType,
        
        TaskType,
        
        Template,
        
        User,
        
        HarborType,
        
        OrgTrade,
        
        CountryType,
        
        ShippingType,
        
        TradeType,
        
        durados_Html,
        
        Time,
        
        v_TaskOpen,
        
        v_TasksAll,
        
        v_ProposalLast2Months,
        
        v_TaskAlert,
    }
    
    public enum V_Contact {
        
        FK_Contact_Contact_Parent,
        
        FK_Contact_LeadSourceType_Parent,
        
        FK_Contact_Organization_Parent,
        
        V_Contact_User_Parent,
        
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
        
        FK_Proposal_Contact_Children,
        
        FK_Task_Contact_Children,
        
        V_Contact_v_TasksAll_Children,
        
        V_Contact_v_TaskOpen_Children,
        
        V_Contact_v_ProposalLast2Months_Children,
        
        V_Contact_Task1_Children,
    }
    
    public enum IndustryType {
        
        Id,
        
        Name,
        
        FK_Organization_IndustryType_Children,
    }
    
    public enum LeadSourceType {
        
        Id,
        
        Name,
        
        FK_Contact_LeadSourceType_Children,
    }
    
    public enum Organization {
        
        FK_Organization_IndustryType_Parent,
        
        FK_Organization_Organization_Parent,
        
        FK_Organization_OrgStatusType_Parent,
        
        FK_Organization_OrgType_Parent,
        
        FK_Organization_ShippingType_Parent,
        
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
        
        CustomsBroker,
        
        AnnualRevenue,
        
        FK_Contact_Organization_Children,
        
        FK_Organization_Organization_Children,
        
        FK_Proposal_Organization_Children,
        
        FK_Task_Organization_Children,
        
        FK_OrgTrade_Organization_Children,
        
        FK_Task_Organization1_Children,
        
        FK_Task_Organization2_Children,
        
        FK_Proposal_Organization1_Children,
        
        FK_Task_Organization3_Children,
    }
    
    public enum OrgStatusType {
        
        Id,
        
        Name,
        
        FK_Organization_OrgStatusType_Children,
    }
    
    public enum OrgType {
        
        Id,
        
        Name,
        
        FK_Organization_OrgType_Children,
    }
    
    public enum Proposal {
        
        FK_Proposal_Contact_Parent,
        
        FK_Proposal_Organization_Parent,
        
        FK_Proposal_Task_Parent,
        
        FK_Proposal_Template_Parent,
        
        User_Proposal_Parent,
        
        Id,
        
        Subject,
        
        Date,
        
        WordDocument,
        
        PDFDocument,
    }
    
    public enum Task {
        
        FK_Task_Contact_Parent,
        
        FK_Task_Organization_Parent,
        
        FK_Task_TaskPriorityType_Parent,
        
        FK_Task_TaskStatusType_Parent,
        
        FK_Task_TaskType_Parent,
        
        Task_User_Parent,
        
        FK_Task_Time_Parent,
        
        Id,
        
        Subject,
        
        DueDate,
        
        Comments,
        
        ReminderDate,
        
        SendNotificationEmail,
        
        FK_Proposal_Task_Children,
        
        FK_Proposal_Task1_Children,
    }
    
    public enum TaskPriorityType {
        
        Id,
        
        Name,
        
        FK_Task_TaskPriorityType_Children,
        
        FK_Task_TaskPriorityType1_Children,
        
        FK_Task_TaskPriorityType2_Children,
        
        FK_Task_TaskPriorityType3_Children,
    }
    
    public enum TaskStatusType {
        
        Id,
        
        Name,
        
        FK_Task_TaskStatusType_Children,
        
        FK_Task_TaskStatusType1_Children,
        
        FK_Task_TaskStatusType2_Children,
        
        FK_Task_TaskStatusType3_Children,
    }
    
    public enum TaskType {
        
        Id,
        
        Name,
        
        FK_Task_TaskType_Children,
        
        FK_Task_TaskType1_Children,
        
        FK_Task_TaskType2_Children,
        
        FK_Task_TaskType3_Children,
    }
    
    public enum Template {
        
        Id,
        
        Name,
        
        DocumentLocation,
        
        FK_Proposal_Template_Children,
        
        FK_Proposal_Template1_Children,
    }
    
    public enum User {
        
        ID,
        
        Username,
        
        FirstName,
        
        LastName,
        
        Email,
        
        Password,
        
        Role,
        
        Task_User_Children,
        
        V_Contact_User_Children,
        
        FK_Task_User_Children,
        
        FK_Task_User1_Children,
        
        User_Proposal_Children,
        
        FK_Proposal_User_Children,
        
        FK_Task_User2_Children,
    }
    
    public enum HarborType {
        
        FK_HarborType_CountryType_Parent,
        
        Id,
        
        Name,
        
        FK_OrgTrade_HarborType_Children,
    }
    
    public enum OrgTrade {
        
        FK_OrgTrade_HarborType_Parent,
        
        FK_OrgTrade_TradeType_Parent,
        
        FK_OrgTrade_Organization_Parent,
        
        Id,
        
        Comments,
    }
    
    public enum CountryType {
        
        Id,
        
        Country,
        
        CountryNotes,
        
        Active,
        
        FK_HarborType_CountryType_Children,
    }
    
    public enum ShippingType {
        
        Id,
        
        Name,
        
        FK_Organization_ShippingType_Children,
    }
    
    public enum TradeType {
        
        Id,
        
        Name,
        
        FK_OrgTrade_TradeType_Children,
    }
    
    public enum durados_Html {
        
        Name,
        
        Text,
    }
    
    public enum Time {
        
        Hour,
        
        Military,
        
        USA,
        
        FK_Task_Time_Children,
        
        FK_Task_Time1_Children,
        
        FK_Task_Time2_Children,
        
        FK_Task_Time3_Children,
    }
    
    public enum v_TaskOpen {
        
        FK_Task_Organization1_Parent,
        
        FK_Task_TaskPriorityType1_Parent,
        
        FK_Task_TaskStatusType1_Parent,
        
        FK_Task_TaskType1_Parent,
        
        FK_Task_Time1_Parent,
        
        FK_Task_User_Parent,
        
        V_Contact_v_TaskOpen_Parent,
        
        Id,
        
        Subject,
        
        DueDate,
        
        Comments,
        
        ReminderDate,
        
        SendNotificationEmail,
    }
    
    public enum v_TasksAll {
        
        FK_Task_Organization2_Parent,
        
        FK_Task_TaskPriorityType2_Parent,
        
        FK_Task_TaskStatusType2_Parent,
        
        FK_Task_TaskType2_Parent,
        
        FK_Task_Time2_Parent,
        
        FK_Task_User1_Parent,
        
        V_Contact_v_TasksAll_Parent,
        
        Id,
        
        Subject,
        
        DueDate,
        
        Comments,
        
        ReminderDate,
        
        SendNotificationEmail,
    }
    
    public enum v_ProposalLast2Months {
        
        FK_Proposal_Organization1_Parent,
        
        FK_Proposal_Task1_Parent,
        
        FK_Proposal_Template1_Parent,
        
        FK_Proposal_User_Parent,
        
        V_Contact_v_ProposalLast2Months_Parent,
        
        Id,
        
        Subject,
        
        Date,
        
        WordDocument,
        
        PDFDocument,
    }
    
    public enum v_TaskAlert {
        
        FK_Task_Organization3_Parent,
        
        FK_Task_TaskPriorityType3_Parent,
        
        FK_Task_TaskStatusType3_Parent,
        
        FK_Task_TaskType3_Parent,
        
        FK_Task_Time3_Parent,
        
        FK_Task_User2_Parent,
        
        V_Contact_Task1_Parent,
        
        Id,
        
        Subject,
        
        DueDate,
        
        Comments,
        
        ReminderDate,
        
        SendNotificationEmail,
        
        ReminderSentDate,
    }
}

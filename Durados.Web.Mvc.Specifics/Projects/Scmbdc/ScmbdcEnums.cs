// Enum for Scmbdc
namespace Durados.Web.Mvc.Specifics.Projects.Scmbdc {
    using System;
    
    
    public enum ScmbdcViews {
        
        ASSOCIATE,
        
        MBE,
        
        MBE_GROSS_ANNUAL_SALE,
        
        STATE_LIST,
    }
    
    public enum ASSOCIATE {
        
        STATE_LIST_ASSOCIATE_Parent,
        
        assc_ID,
        
        assc_business_name,
        
        key_contact_title,
        
        key_contact_Fname,
        
        key_contact_Lname,
        
        assc_address1,
        
        assc_address2,
        
        assc_city,
        
        assc_zipcode,
        
        assc_phone,
        
        assc_fax_num,
        
        assc_email,
        
        assc_website,
        
        assc_mail_address1,
        
        assc_mail_address2,
        
        assc_mail_city,
        
        assc_mail_state,
        
        assc_mail_zipcode,
        
        assc_payment_type,
        
        assc_pdf_filename,
        
        assc_date,
        
        assc_status,
        
        assc_fee_amt,
        
        assc_fee_status,
        
        assc_type,
        
        LastModificationTime,
        
        EntryID,
        
        insurance_inquiry,
        
        insurance_CA_employees,
        
        assc_other_phone,
        
        email_status,
        
        email_country,
        
        email_LastAttempt,
        
        email_LastResponse,
        
        email_VerificationResult,
        
        FK_MBE_ASSOCIATE_Children,
    }
    
    public enum MBE {
        
        FK_MBE_ASSOCIATE_Parent,
        
        mbe_cert_num,
        
        council_ID_num,
        
        mbe_business_type,
        
        mbe_business_product_service,
        
        mbe_other_cert,
        
        mbe_federal_IRS_num,
        
        mbe_business_establish_date,
        
        mbe_date_certified,
        
        iso9000_cert,
        
        mbe_parent_company,
        
        mbe_recert_date1,
        
        mbe_recert_date2,
        
        mbe_recert_date3,
        
        mbe_recert_date4,
        
        mbe_minority_emp_count,
        
        mbe_gross_annual_sale,
        
        mbe_geographical_mrkt,
        
        mbe_HUB_zone,
        
        RMSDC_code,
        
        mbe_legal_structure,
        
        mbe_owner_salutation,
        
        major_customer,
        
        mbe_owner_title,
        
        owner_Lname,
        
        owner_Fname,
        
        mbe_other_contact_name,
        
        mbe_other_contact_title,
        
        mbe_8a_cert,
        
        mbe_employee_count,
        
        subsidiary,
        
        ownership_change,
        
        full_time_emp_count,
        
        pending_lawsuit,
        
        cert_rejection_test,
        
        bankruptcy,
        
        subsidiary_test,
        
        member_history,
        
        mbe_DB_num,
        
        part_time_emp_count,
        
        cert_rejection_reason,
        
        method_of_acquisition,
        
        check_num,
        
        mbe_home_base_business,
        
        mbe_8a_cert_num,
        
        sinking_fund_num,
        
        mbe_email_subscription,
        
        iso9001_cert,
        
        council,
        
        local_regional_council,
        
        iso9002_cert,
        
        contract_termination_date,
        
        mbe_employer_SSN,
        
        minority_memo,
        
        date_acquisition,
        
        mbe_additional_facility,
        
        minority_class,
        
        mbe_cert_expiration_date,
        
        mbe_MBEIC_inv,
        
        mbe_other_contact_email,
        
        otherNMSDCCert,
        
        otherNMSDCCertCouncil,
        
        otherNMSDCCertDate,
        
        denyCert,
        
        denyCertCouncil,
        
        denyCertDate,
        
        govtClear,
        
        clearLevel,
        
        unionAffiliation,
        
        bonding,
        
        bondingAmount,
        
        bondingCompany,
        
        sharedResources,
        
        sharedResourcesDesc,
        
        otherCompany,
        
        otherCompanyDesc,
        
        premisesStatus,
        
        otherAgreements,
        
        pending_lawsuit_details,
        
        bankruptcyDetails,
        
        history,
        
        marketingChannel,
        
        avgDollarValueInv,
        
        corporatePlus,
        
        pword,
        
        lastDeterminationDate,
        
        corpPlusSponsor,
        
        customerNatContracts,
        
        phoneExtension,
        
        remarks,
        
        FK_MBE_GROSS_ANNUAL_SALE_MBE_Children,
    }
    
    public enum MBE_GROSS_ANNUAL_SALE {
        
        FK_MBE_GROSS_ANNUAL_SALE_MBE_Parent,
        
        gross_annual_sale_date,
        
        gross_annual_sale,
    }
    
    public enum STATE_LIST {
        
        Code,
        
        Name,
        
        STATE_LIST_ASSOCIATE_Children,
    }
}

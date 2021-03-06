USE [IMedBetipulNetTest]
GO

/****** Object:  Table [dbo].[tbl_AuthorTypes]    Script Date: 09/08/2011 10:22:44 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_AuthorTypes](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](50) NULL,
 CONSTRAINT [PK_tbl_AuthorTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
INSERT INTO [dbo].[tbl_AuthorTypes] ([Id],[Name])  VALUES (2,N'מרכז רפואי')     
INSERT INTO [dbo].[tbl_AuthorTypes] ([Id],[Name])  VALUES (3,N'כותב, מתרגם')
INSERT INTO [dbo].[tbl_AuthorTypes] ([Id],[Name])  VALUES (4,N'מומחה הפורטל')
INSERT INTO [dbo].[tbl_AuthorTypes] ([Id],[Name])  VALUES (5,N'אדמין')
	
GO




SET IDENTITY_INSERT [dbo].[tbl_Translators] ON
GO
INSERT INTO [dbo].[tbl_Translators]
           ([Id],[Name]
           ,[IsActive])
     VALUES
           (0,N'אין',1)
GO
SET IDENTITY_INSERT [dbo].[tbl_Translators] OFF
GO



---------------------------------------------------------------
CREATE VIEW [dbo].[v_Articles]
AS
SELECT     ID, Title, Summary, ArticleBody, Credit, AuthorID, AuthorType, AuthorFirstName, AuthorLastName, AuthorImagePath, AuthorTitle, AuthorPrimaryRole, ImagePath, 
                      CanEditAuthor, SourceUrl, TranslatorID, DisplayOrder, SEOTitle, SEODesc, SEOKeywords, IsPublished, IsApproved, ApprovedBy, ApprovedDate, LastUpdateDate, 
                      AddedDate, UpdateDate, DirectoryName, UpdatedByAdmin, SpecialtyIDsList, SpecialtyNamesList, CategoryIDsList, CategoryNamesList, TopTags, ArticleTypeID, 
                      FixedBody, ImageWidth, ImageHeight, MidPageBannerSource, IsPromotional, NoIndex, NoFollow, ISNULL(AuthorTitle, '') + ' ' + ISNULL(AuthorFirstName, '') 
                      + ' ' + AuthorLastName AS AuthorFullName
FROM         dbo.tbl_Articles

-----------------------------------------------------------------------
CREATE VIEW [dbo].[v_DefaultSpecialtiesInFocus]
AS
SELECT     DoctorsInFocusSpecialtyID, InstituteInFocusSpecialtyID, ID
FROM         dbo.tbl_HomepageSettings

GO
-----------------------------------------------------------------------
CREATE VIEW [dbo].[v_ParentSpecialties]
AS
SELECT     ID, Name
FROM         dbo.tbl_Specialties
WHERE     (ParentID IS NULL)

GO
-----------------------------------------------------------------------

GO
/****** Object:  Table [dbo].[tbl_InstitueType]    Script Date: 09/13/2011 14:38:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_InstitueType](
	[ID] [int] NOT NULL,
	[Name] [nvarchar](50) NULL,
 CONSTRAINT [PK_tbl_InstitueType] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

insert into dbo.tbl_InstitueType(ID,Name) values(0,N'')
insert into dbo.tbl_InstitueType(ID,Name) values(1,N'בית חולים')
insert into dbo.tbl_InstitueType(ID,Name) values(2,N'מכון רפואי')
insert into dbo.tbl_InstitueType(ID,Name) values(3,N'בית אבות')
insert into dbo.tbl_InstitueType(ID,Name) values(10,N'אחר')


/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/

CREATE TABLE dbo.Tmp_tbl_InstitueType
	(
	ID smallint NOT NULL IDENTITY (1, 1),
	Name nvarchar(50) NULL
	)  ON [PRIMARY]
GO
SET IDENTITY_INSERT dbo.Tmp_tbl_InstitueType ON
GO
IF EXISTS(SELECT * FROM dbo.tbl_InstitueType)
	 EXEC('INSERT INTO dbo.Tmp_tbl_InstitueType (ID, Name)
		SELECT CONVERT(smallint, ID), Name FROM dbo.tbl_InstitueType WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_tbl_InstitueType OFF
GO
DROP TABLE dbo.tbl_InstitueType
GO
EXECUTE sp_rename N'dbo.Tmp_tbl_InstitueType', N'tbl_InstitueType', 'OBJECT' 
GO
ALTER TABLE dbo.tbl_InstitueType ADD CONSTRAINT
	PK_tbl_InstitueType PRIMARY KEY CLUSTERED 
	(
	ID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT


ALTER TABLE dbo.tbl_Cities
	DROP CONSTRAINT DF_tbl_Cities_TopCity
GO
CREATE TABLE dbo.Tmp_tbl_Cities
	(
	ID smallint NOT NULL,
	AreaId tinyint NOT NULL,
	Name nvarchar(50) NOT NULL,
	orderField int NULL,
	TopCity bit NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_tbl_Cities ADD CONSTRAINT
	DF_tbl_Cities_TopCity DEFAULT ((0)) FOR TopCity
GO
IF EXISTS(SELECT * FROM dbo.tbl_Cities)
	 EXEC('INSERT INTO dbo.Tmp_tbl_Cities (ID, AreaId, Name, orderField, TopCity)
		SELECT CONVERT(smallint, ID), AreaId, Name, orderField, TopCity FROM dbo.tbl_Cities WITH (HOLDLOCK TABLOCKX)')
GO
DROP TABLE dbo.tbl_Cities
GO
EXECUTE sp_rename N'dbo.Tmp_tbl_Cities', N'tbl_Cities', 'OBJECT' 
GO
ALTER TABLE dbo.tbl_Cities ADD CONSTRAINT
	PK_SubAreas PRIMARY KEY CLUSTERED 
	(
	ID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT


USE [IMedBetipulNetTest]
GO

/****** Object:  Table [dbo].[tbl_storage]    Script Date: 09/13/2011 20:55:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_storage](
	[ID] [int] NULL,
	[Name] [nvarchar](500) NULL
) ON [PRIMARY]

GO


insert into [tbl_storage](ID,Name) values(0,N'')
insert into [tbl_storage](ID,Name) values(1,N'לשמור במיכל סגור במקום קריר ויבש, הרחק מהישג ידם של ילדים')
insert into [tbl_storage](ID,Name) values(2,N'לשמור במיכל סגור במקום קריר ויבש, הרחק מהישג ידם של ילדים. להגן מפני אור')
insert into [tbl_storage](ID,Name) values(3,N'לשמור במקרר, אך לא במקפיא')
insert into [tbl_storage](ID,Name) values(4,N'לשמור במקרר, להגן מפני אור')

/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.Tmp_tbl_storage
	(
	ID int NOT NULL IDENTITY (1, 1),
	Name nvarchar(500) NULL
	)  ON [PRIMARY]
GO
SET IDENTITY_INSERT dbo.Tmp_tbl_storage ON
GO
IF EXISTS(SELECT * FROM dbo.tbl_storage)
	 EXEC('INSERT INTO dbo.Tmp_tbl_storage (ID, Name)
		SELECT ID, Name FROM dbo.tbl_storage WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_tbl_storage OFF
GO
DROP TABLE dbo.tbl_storage
GO
EXECUTE sp_rename N'dbo.Tmp_tbl_storage', N'tbl_storage', 'OBJECT' 
GO
ALTER TABLE dbo.tbl_storage ADD CONSTRAINT
	PK_tbl_storage PRIMARY KEY CLUSTERED 
	(
	ID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT

select * from feedtxttbl
insert into feedtxttbl(feedID,theTxt) values(0,'')
insert into oldtxttbl(oldID,theTxt) values(0,'')
SET IDENTITY_INSERT alcholtxttbl ON
insert into alcholtxttbl(alcID,theTxt) values(0,'')
SET IDENTITY_INSERT alcholtxttbl off
SET IDENTITY_INSERT childtxttbl ON
insert into childtxttbl(childID,theTxt) values(0,'')
SET IDENTITY_INSERT childtxttbl off
SET IDENTITY_INSERT drivetxttbl ON
insert into drivetxttbl(driveID,theTxt) values(0,'')
SET IDENTITY_INSERT drivetxttbl off

insert into operationtxttbl(operID,theTxt) values (0,'')

ALTER TABLE dbo.operationTxtTbl ADD CONSTRAINT
	PK_operationTxtTbl PRIMARY KEY CLUSTERED 
	(
	operID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT

delete from druginformtbl where druginformid=3471

CREATE TABLE dbo.Tmp_usageTbl
	(
	usageID int NOT NULL,
	usageName nvarchar(50) NULL
	)  ON [PRIMARY]
GO
IF EXISTS(SELECT * FROM dbo.usageTbl)
	 EXEC('INSERT INTO dbo.Tmp_usageTbl (usageID, usageName)
		SELECT usageID, usageName FROM dbo.usageTbl WITH (HOLDLOCK TABLOCKX)')
GO
DROP TABLE dbo.usageTbl
GO
EXECUTE sp_rename N'dbo.Tmp_usageTbl', N'usageTbl', 'OBJECT' 
GO
ALTER TABLE dbo.usageTbl ADD CONSTRAINT
	PK_usageTbl PRIMARY KEY CLUSTERED 
	(
	usageID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT


insert into drugsComp (drugCompId,compName) values (0,N'')
insert into tbl_authorTypes(id,name) values(0,N'')

alter table tbl_examinations alter column authorType int;

-----------------------------------------------------------------------------
ALTER TABLE dbo.tbl_MedicalInstitutes
	DROP CONSTRAINT FK_tbl_MedicalInstitutes_tbl_InstitueType
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.tbl_MedicalInstitutes
	DROP CONSTRAINT FK_tbl_MedicalInstitutes_PhoneType
GO
ALTER TABLE dbo.tbl_MedicalInstitutes
	DROP CONSTRAINT FK_tbl_MedicalInstitutes_PhoneType1
GO
ALTER TABLE dbo.tbl_MedicalInstitutes
	DROP CONSTRAINT FK_tbl_MedicalInstitutes_PhoneType2
GO
ALTER TABLE dbo.tbl_MedicalInstitutes
	DROP CONSTRAINT FK_tbl_MedicalInstitutes_PhoneType3
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.tbl_Cities
	DROP CONSTRAINT DF_tbl_Cities_TopCity
GO
CREATE TABLE dbo.Tmp_tbl_Cities
	(
	ID int NOT NULL,
	AreaId tinyint NOT NULL,
	Name nvarchar(50) NOT NULL,
	orderField int NULL,
	TopCity bit NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_tbl_Cities ADD CONSTRAINT
	DF_tbl_Cities_TopCity DEFAULT ((0)) FOR TopCity
GO
IF EXISTS(SELECT * FROM dbo.tbl_Cities)
	 EXEC('INSERT INTO dbo.Tmp_tbl_Cities (ID, AreaId, Name, orderField, TopCity)
		SELECT CONVERT(int, ID), AreaId, Name, orderField, TopCity FROM dbo.tbl_Cities WITH (HOLDLOCK TABLOCKX)')
GO
ALTER TABLE dbo.tbl_MedicalInstitutes
	DROP CONSTRAINT FK_tbl_MedicalInstitutes_tbl_Cities
GO
DROP TABLE dbo.tbl_Cities
GO
EXECUTE sp_rename N'dbo.Tmp_tbl_Cities', N'tbl_Cities', 'OBJECT' 
GO
ALTER TABLE dbo.tbl_Cities ADD CONSTRAINT
	PK_SubAreas PRIMARY KEY CLUSTERED 
	(
	ID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.tbl_MedicalInstitutes
	DROP CONSTRAINT DF_tbl_MedicalInstitutes_IsDataVerified
GO
ALTER TABLE dbo.tbl_MedicalInstitutes
	DROP CONSTRAINT DF_tbl_MedicalInstitutes_WholeCountry
GO
CREATE TABLE dbo.Tmp_tbl_MedicalInstitutes
	(
	ID int NOT NULL IDENTITY (1, 1),
	Name nvarchar(100) NULL,
	InstituteType smallint NULL,
	Email_1 nvarchar(50) NULL,
	Email_2 nvarchar(50) NULL,
	Phone_1 nvarchar(20) NULL,
	Phone_1_Type smallint NULL,
	Phone_2 nvarchar(20) NULL,
	Phone_2_Type smallint NULL,
	Phone_3 nvarchar(20) NULL,
	Phone_3_Type smallint NULL,
	Phone_4 nvarchar(20) NULL,
	Phone_4_Type smallint NULL,
	Address nvarchar(150) NULL,
	CityID int NULL,
	CityName nvarchar(30) NULL,
	WebSite nvarchar(150) NULL,
	Description ntext NULL,
	Accreditations ntext NULL,
	LogoPath nvarchar(150) NULL,
	LogoWidth smallint NULL,
	LogoHeight smallint NULL,
	LogoThumbPath nvarchar(150) NULL,
	ImagePath nvarchar(150) NULL,
	ImageWidth smallint NULL,
	ImageHeight smallint NULL,
	ImageThumbPath nvarchar(150) NULL,
	SEOKeywords nvarchar(250) NULL,
	IsPromoted bit NULL,
	PromotionLevel smallint NULL,
	IsLegalExpert bit NULL,
	IsPhoneVerified bit NULL,
	IsSiteExpert bit NULL,
	HasPrivateService bit NULL,
	IsAlternative bit NULL,
	IsQualified bit NULL,
	GivesMedicalOpinions bit NULL,
	IsDataVerified bit NULL,
	NumExperts int NULL,
	DataSourceLink nvarchar(150) NULL,
	DataSourceName nvarchar(50) NULL,
	AddedDate datetime NULL,
	UpdatedDate datetime NULL,
	UpdatedByAdmin smallint NULL,
	CategoryIDsList nvarchar(250) NULL,
	SpecialtyIDsList nvarchar(250) NULL,
	AreaIDsList nvarchar(50) NULL,
	WholeCountry bit NULL,
	PromotionPhrase nvarchar(100) NULL,
	AskQuestionEnabled bit NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_tbl_MedicalInstitutes ADD CONSTRAINT
	DF_tbl_MedicalInstitutes_IsDataVerified DEFAULT ((0)) FOR IsDataVerified
GO
ALTER TABLE dbo.Tmp_tbl_MedicalInstitutes ADD CONSTRAINT
	DF_tbl_MedicalInstitutes_WholeCountry DEFAULT ((0)) FOR WholeCountry
GO
SET IDENTITY_INSERT dbo.Tmp_tbl_MedicalInstitutes ON
GO
IF EXISTS(SELECT * FROM dbo.tbl_MedicalInstitutes)
	 EXEC('INSERT INTO dbo.Tmp_tbl_MedicalInstitutes (ID, Name, InstituteType, Email_1, Email_2, Phone_1, Phone_1_Type, Phone_2, Phone_2_Type, Phone_3, Phone_3_Type, Phone_4, Phone_4_Type, Address, CityID, CityName, WebSite, Description, Accreditations, LogoPath, LogoWidth, LogoHeight, LogoThumbPath, ImagePath, ImageWidth, ImageHeight, ImageThumbPath, SEOKeywords, IsPromoted, PromotionLevel, IsLegalExpert, IsPhoneVerified, IsSiteExpert, HasPrivateService, IsAlternative, IsQualified, GivesMedicalOpinions, IsDataVerified, NumExperts, DataSourceLink, DataSourceName, AddedDate, UpdatedDate, UpdatedByAdmin, CategoryIDsList, SpecialtyIDsList, AreaIDsList, WholeCountry, PromotionPhrase, AskQuestionEnabled)
		SELECT ID, Name, InstituteType, Email_1, Email_2, Phone_1, Phone_1_Type, Phone_2, Phone_2_Type, Phone_3, Phone_3_Type, Phone_4, Phone_4_Type, Address, CONVERT(int, CityID), CityName, WebSite, Description, Accreditations, LogoPath, LogoWidth, LogoHeight, LogoThumbPath, ImagePath, ImageWidth, ImageHeight, ImageThumbPath, SEOKeywords, IsPromoted, PromotionLevel, IsLegalExpert, IsPhoneVerified, IsSiteExpert, HasPrivateService, IsAlternative, IsQualified, GivesMedicalOpinions, IsDataVerified, NumExperts, DataSourceLink, DataSourceName, AddedDate, UpdatedDate, UpdatedByAdmin, CategoryIDsList, SpecialtyIDsList, AreaIDsList, WholeCountry, PromotionPhrase, AskQuestionEnabled FROM dbo.tbl_MedicalInstitutes WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_tbl_MedicalInstitutes OFF
GO
ALTER TABLE dbo.tbl_ExpertRoles
	DROP CONSTRAINT FK_tbl_ExpertRoles_tbl_MedicalInstitutes
GO
ALTER TABLE dbo.tbl_ExpertMedicalInstituteRelations
	DROP CONSTRAINT FK_tbl_ExpertMedicalInstituteRelations_tbl_MedicalInstitutes
GO
ALTER TABLE dbo.tbl_MedicalInstituteBranches
	DROP CONSTRAINT FK_tbl_MedicalInstituteBranches_tbl_MedicalInstitutes
GO
ALTER TABLE dbo.tbl_InstituteActiveThroughInstitueRelations
	DROP CONSTRAINT FK_tbl_InstituteActiveThroughInstitueRelations_tbl_MedicalInstitutes
GO
ALTER TABLE dbo.tbl_InstituteSubTypeRelations
	DROP CONSTRAINT FK_tbl_InstituteSubTypeRelations_tbl_MedicalInstitutes
GO
ALTER TABLE dbo.tbl_InstituteExaminationRelations
	DROP CONSTRAINT FK_tbl_InstituteExaminationRelations_tbl_MedicalInstitutes
GO
ALTER TABLE dbo.tbl_InstituteSpecialtyRelations
	DROP CONSTRAINT FK_tbl_InstituteSpecialtyRelations_tbl_MedicalInstitutes
GO
ALTER TABLE dbo.tbl_InstituteTreatmentRelations
	DROP CONSTRAINT FK_tbl_InstituteTreatmentRelations_tbl_MedicalInstitutes
GO
ALTER TABLE dbo.tbl_InstituteAreaRelations
	DROP CONSTRAINT FK_tbl_InstituteAreaRelations_tbl_MedicalInstitutes
GO
ALTER TABLE dbo.tbl_InstituteCategoryRelations
	DROP CONSTRAINT FK_tbl_InstituteCategoryRelations_tbl_MedicalInstitutes
GO
DROP TABLE dbo.tbl_MedicalInstitutes
GO
EXECUTE sp_rename N'dbo.Tmp_tbl_MedicalInstitutes', N'tbl_MedicalInstitutes', 'OBJECT' 
GO
ALTER TABLE dbo.tbl_MedicalInstitutes ADD CONSTRAINT
	PK_tbl_NonMedicalInstitues PRIMARY KEY CLUSTERED 
	(
	ID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.tbl_MedicalInstitutes ADD CONSTRAINT
	FK_tbl_MedicalInstitutes_PhoneType FOREIGN KEY
	(
	Phone_1_Type
	) REFERENCES dbo.PhoneType
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.tbl_MedicalInstitutes ADD CONSTRAINT
	FK_tbl_MedicalInstitutes_PhoneType1 FOREIGN KEY
	(
	Phone_2_Type
	) REFERENCES dbo.PhoneType
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.tbl_MedicalInstitutes ADD CONSTRAINT
	FK_tbl_MedicalInstitutes_PhoneType2 FOREIGN KEY
	(
	Phone_3_Type
	) REFERENCES dbo.PhoneType
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.tbl_MedicalInstitutes ADD CONSTRAINT
	FK_tbl_MedicalInstitutes_PhoneType3 FOREIGN KEY
	(
	Phone_4_Type
	) REFERENCES dbo.PhoneType
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.tbl_MedicalInstitutes ADD CONSTRAINT
	FK_tbl_MedicalInstitutes_tbl_InstitueType FOREIGN KEY
	(
	InstituteType
	) REFERENCES dbo.tbl_InstitueType
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.tbl_MedicalInstitutes ADD CONSTRAINT
	FK_tbl_MedicalInstitutes_tbl_Cities1 FOREIGN KEY
	(
	CityID
	) REFERENCES dbo.tbl_Cities
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.tbl_InstituteCategoryRelations ADD CONSTRAINT
	FK_tbl_InstituteCategoryRelations_tbl_MedicalInstitutes FOREIGN KEY
	(
	InstituteID
	) REFERENCES dbo.tbl_MedicalInstitutes
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.tbl_InstituteAreaRelations ADD CONSTRAINT
	FK_tbl_InstituteAreaRelations_tbl_MedicalInstitutes FOREIGN KEY
	(
	InstituteID
	) REFERENCES dbo.tbl_MedicalInstitutes
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.tbl_InstituteTreatmentRelations ADD CONSTRAINT
	FK_tbl_InstituteTreatmentRelations_tbl_MedicalInstitutes FOREIGN KEY
	(
	InstituteID
	) REFERENCES dbo.tbl_MedicalInstitutes
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.tbl_InstituteSpecialtyRelations ADD CONSTRAINT
	FK_tbl_InstituteSpecialtyRelations_tbl_MedicalInstitutes FOREIGN KEY
	(
	InstituteID
	) REFERENCES dbo.tbl_MedicalInstitutes
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.tbl_InstituteExaminationRelations ADD CONSTRAINT
	FK_tbl_InstituteExaminationRelations_tbl_MedicalInstitutes FOREIGN KEY
	(
	InstituteID
	) REFERENCES dbo.tbl_MedicalInstitutes
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.tbl_InstituteSubTypeRelations ADD CONSTRAINT
	FK_tbl_InstituteSubTypeRelations_tbl_MedicalInstitutes FOREIGN KEY
	(
	InstituteID
	) REFERENCES dbo.tbl_MedicalInstitutes
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.tbl_InstituteActiveThroughInstitueRelations ADD CONSTRAINT
	FK_tbl_InstituteActiveThroughInstitueRelations_tbl_MedicalInstitutes FOREIGN KEY
	(
	InstituteID
	) REFERENCES dbo.tbl_MedicalInstitutes
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.tbl_MedicalInstituteBranches ADD CONSTRAINT
	FK_tbl_MedicalInstituteBranches_tbl_MedicalInstitutes FOREIGN KEY
	(
	InstituteID
	) REFERENCES dbo.tbl_MedicalInstitutes
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.tbl_ExpertMedicalInstituteRelations ADD CONSTRAINT
	FK_tbl_ExpertMedicalInstituteRelations_tbl_MedicalInstitutes FOREIGN KEY
	(
	InstituteID
	) REFERENCES dbo.tbl_MedicalInstitutes
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.tbl_ExpertRoles ADD CONSTRAINT
	FK_tbl_ExpertRoles_tbl_MedicalInstitutes FOREIGN KEY
	(
	InstituteID
	) REFERENCES dbo.tbl_MedicalInstitutes
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.tbl_Pharmacies
	DROP CONSTRAINT DF_tbl_Pharmacies_AllowContactBox
GO
CREATE TABLE dbo.Tmp_tbl_Pharmacies
	(
	PharmacyID int NOT NULL IDENTITY (1, 1),
	PharmaName nvarchar(50) NOT NULL,
	PharmaCityID smallint NULL,
	PharmaStreet nvarchar(50) NULL,
	PharmaStreetNumber nvarchar(20) NULL,
	PharmaPhone nvarchar(20) NULL,
	PharmaFax nvarchar(20) NULL,
	TopBannerSource nvarchar(4000) NULL,
	BottomBannerSource nvarchar(4000) NULL,
	EmailAddress nvarchar(100) NULL,
	WebSite nvarchar(100) NULL,
	HoursOfActivity nvarchar(200) NULL,
	GeneralDescription nvarchar(1000) NULL,
	AllowContactBox bit NULL,
	PromotionPhrase nvarchar(100) NULL,
	ImagePath nvarchar(150) NULL,
	ImageWidth int NULL,
	ImageHeight int NULL,
	PreDesc nvarchar(1000) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_tbl_Pharmacies ADD CONSTRAINT
	DF_tbl_Pharmacies_AllowContactBox DEFAULT ((0)) FOR AllowContactBox
GO
SET IDENTITY_INSERT dbo.Tmp_tbl_Pharmacies ON
GO
IF EXISTS(SELECT * FROM dbo.tbl_Pharmacies)
	 EXEC('INSERT INTO dbo.Tmp_tbl_Pharmacies (PharmacyID, PharmaName, PharmaCityID, PharmaStreet, PharmaStreetNumber, PharmaPhone, PharmaFax, TopBannerSource, BottomBannerSource, EmailAddress, WebSite, HoursOfActivity, GeneralDescription, AllowContactBox, PromotionPhrase, ImagePath, ImageWidth, ImageHeight, PreDesc)
		SELECT PharmacyID, PharmaName, CONVERT(smallint, PharmaCityID), PharmaStreet, PharmaStreetNumber, PharmaPhone, PharmaFax, TopBannerSource, BottomBannerSource, EmailAddress, WebSite, HoursOfActivity, GeneralDescription, AllowContactBox, PromotionPhrase, ImagePath, ImageWidth, ImageHeight, PreDesc FROM dbo.tbl_Pharmacies WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_tbl_Pharmacies OFF
GO
ALTER TABLE dbo.tbl_PharmacyActiveThroughRelations
	DROP CONSTRAINT FK_tbl_PharmacyActiveThroughRelations_tbl_Pharmacies
GO
ALTER TABLE dbo.tbl_PharmacyAcceptsReceiptsFromRelations
	DROP CONSTRAINT FK_tbl_PharmacyAcceptsReceiptsFromRelations_tbl_Pharmacies
GO
DROP TABLE dbo.tbl_Pharmacies
GO
EXECUTE sp_rename N'dbo.Tmp_tbl_Pharmacies', N'tbl_Pharmacies', 'OBJECT' 
GO
ALTER TABLE dbo.tbl_Pharmacies ADD CONSTRAINT
	PK_tbl_Pharmacies PRIMARY KEY CLUSTERED 
	(
	PharmacyID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.tbl_PharmacyAcceptsReceiptsFromRelations ADD CONSTRAINT
	FK_tbl_PharmacyAcceptsReceiptsFromRelations_tbl_Pharmacies FOREIGN KEY
	(
	PharmacyID
	) REFERENCES dbo.tbl_Pharmacies
	(
	PharmacyID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.tbl_PharmacyActiveThroughRelations ADD CONSTRAINT
	FK_tbl_PharmacyActiveThroughRelations_tbl_Pharmacies FOREIGN KEY
	(
	PharmacyID
	) REFERENCES dbo.tbl_Pharmacies
	(
	PharmacyID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
CREATE FULLTEXT INDEX ON dbo.tbl_MedicalInstitutes
( 
	Name LANGUAGE 1033, 
	Description LANGUAGE 1033, 
	SEOKeywords LANGUAGE 1033
 )
KEY INDEX PK_tbl_NonMedicalInstitues
ON institutes_full_text
 WITH  CHANGE_TRACKING  AUTO 
GO
ALTER FULLTEXT INDEX ON dbo.tbl_MedicalInstitutes
ENABLE 
GO
-------------------------------------------------------------------------------------------

alter table dbo.tbl_Pharmacies alter column PharmaCityID int

delete  from tbl_leads where messageid =568


--BEGIN TRANSACTION
--GO
--EXECUTE sp_rename N'dbo.tbl_PromotionLinks.LinkTarget', N'Tmp_LinkTargetID', 'COLUMN' 
--GO
--EXECUTE sp_rename N'dbo.tbl_PromotionLinks.Tmp_LinkTargetID', N'LinkTargetID', 'COLUMN' 
--GO
--COMMIT


-----------------------------------------------------------------------------------------
alter table [tbl_PromotionLinks] alter column  [LinkTarget] nvarchar(10) not null
  
CREATE TABLE dbo.tbl_LinkTargetType
	(
	ID nvarchar(10) primary key,
	Name nvarchar(50) NULL
	--,EngValue nvarchar(50) NULL
	)  ON [PRIMARY]
GO
insert into tbl_LinkTargetType(Id,name) values('_self','רגיל')
insert into tbl_LinkTargetType(Id,name) values('','חלון חדש')
---------------------------------------------------------------------------------
--select * from tbl_LinkTargetType
--update  [tbl_PromotionLinks] set [LinkTarget]=1
--alter table [tbl_PromotionLinks] drop constraint DF_tbl_PromotionLinks_LinkTarget
--alter table [tbl_PromotionLinks] alter column  [LinkTarget] int not null
--sp_RENAME '[tbl_PromotionLinks].[LinkTargetID]', 'LinkTargetID' , 'COLUMN'
--ALTER TABLE dbo.tbl_PromotionLinks
--	DROP CONSTRAINT FK_tbl_PromotionLinks_tbl_LinkTargetType
--GO
--COMMIT
--BEGIN TRANSACTION
--GO
--ALTER TABLE dbo.tbl_PromotionLinks ADD CONSTRAINT
--	FK_tbl_PromotionLinks_tbl_LinkTargetType1 FOREIGN KEY
--	(
--	LinkTargetID
--	) REFERENCES dbo.tbl_LinkTargetType
--	(
--	ID
--	) ON UPDATE  NO ACTION 
--	 ON DELETE  NO ACTION 
	
--GO
--COMMIT

------------------------------------------------------------------------------
ALTER TABLE dbo.tbl_Cities ALTER COLUMN AreaId int
SET IDENTITY_INSERT dbo.tbl_areas ON
insert into tbl_areas(id,name,displayOrder) values (1,N'אינטרנט',9)
SET IDENTITY_INSERT dbo.tbl_areas off

-------------------------------------------------------------------------------


/****** Object:  Table [dbo].[tbl_SearchObjectsForAdmin]    Script Date: 09/19/2011 12:38:56 ******/
USE [IMedBetipulNetTest]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--drop table [tbl_SearchObjectsForAdmin]
CREATE TABLE [dbo].[tbl_SearchObjectsForAdmin](
	[ID] [int]  primary key,
	[Name] [nvarchar](50) NULL
) ON [PRIMARY]

GO
USE [IMedBetipulNetTest]
GO
insert  into [tbl_SearchObjectsForAdmin](ID,Name) values  (1,N'רופאים')
insert  into [tbl_SearchObjectsForAdmin](ID,Name) values  (2,N'מכונים')
insert  into [tbl_SearchObjectsForAdmin](ID,Name) values  (3,N'רופאי שיניים')
insert  into [tbl_SearchObjectsForAdmin](ID,Name) values  (4,N'מטפלים אלטרנטיביים')
insert  into [tbl_SearchObjectsForAdmin](ID,Name) values  (7,N'תרופות')
insert  into [tbl_SearchObjectsForAdmin](ID,Name) values  (8,N'בדיקות')
insert  into [tbl_SearchObjectsForAdmin](ID,Name) values  (11,N'מחלות')

--------------------------------------------------------------------------------------

drop table xrisk
create table xrisk
(id int identity(0,1) primary key not null,
Name nvarchar(50) null
)
set identity_insert dbo.xrisk on
insert into xrisk(id,name) values(0,N'')
set identity_insert xrisk off
insert into xrisk(name) values(N'נמוך')
insert into xrisk(name) values(N'בינוני')
insert into xrisk(name) values(N'גבוה')



alter table dbo.xImmunTbl alter column danger int
alter table dbo.xImmunTarget alter column [neededID] int
alter table dbo.xImmunTbl alter column [routineID] int

-------------------------------------------------------------------
drop table xRiskLevel
create table xRiskLevel
(id nvarchar(2) primary key not null,
)

insert into xRiskLevel(id) values(N'A')
insert into xRiskLevel(id) values(N'B')
insert into xRiskLevel(id) values(N'C')
insert into xRiskLevel(id) values(N'D')
insert into xRiskLevel(id) values(N'')
alter table dbo.xImmunTbl alter column [sikunID] nvarchar(2)
alter table dbo.xImmunTbl alter column [oldid] nvarchar(2)
alter table dbo.xImmunTbl alter column [pregID] nvarchar(2)
alter table dbo.xImmunTbl alter column [babyID] nvarchar(2)
alter table dbo.xImmunTbl alter column [feedID] nvarchar(2)

-----------------------------------------------------------------------
delete from dbo.[tbl_VaccineExaminationRelations] where VaccineID=52
delete from dbo.tbl_VaccineInsSubTypeRelations where VaccineID=52
------------------------------------------------------------------------
drop table xAgeUnits
create table xAgeUnits
(id int identity(0,1) primary key not null,
name  nvarchar(10)
)
insert into xAgeUnits(name) values(N'')
insert into xAgeUnits(name) values(N'חודשים')
insert into xAgeUnits(name) values(N'שנים')
--select * from xAgeUnits
alter table dbo.xImmunTbl alter column ageUnit int 

alter table [xImmunTbl] add constraint FK_xImmunTbl_xAgeUnits foreign key 
(ageUnit) references xAgeUnits(id)

----------------------------------------------------
CREATE VIEW [dbo].[v_immunCountryRelation]
AS
SELECT     immuneCountryID, countryID, immunID, neededID, neededTxt, negate
FROM         dbo.xImmunCountry
WHERE     (countryID IS NOT NULL) AND (countryID <> 0)

GO
CREATE VIEW [dbo].[v_immunGeogRealtion]
AS
SELECT     immuneCountryID, geogAreaID, immunID, neededID, neededTxt, negate
FROM         dbo.xImmunCountry
WHERE     (geogAreaID IS NOT NULL) AND (geogAreaID <> 0)

GO
-----------------------------------------------------------
alter table ximmuncountry alter column neededid int
---------
insert into xneededtbl (neededid,neededname) values(0,'')
-----------------------------------------------------------
alter table tbl_experts alter column Address_2_CityID int
alter table tbl_experts alter column Address_3_CityID int
--------------------------------------------------------------
/****** Script for SelectTopNRows command from SSMS  ******/
CREATE VIEW [dbo].[v_ChannelExpertRelation]
AS
SELECT  [ChannelTypeId],[EntityId],[EntityType] FROM [dbo].[tbl_ChannelRelations]  where [EntityType]=1
CREATE VIEW [dbo].[v_ChannelMedicalInstituteRelation]
AS
SELECT  [ChannelTypeId],[EntityId],[EntityType] FROM [dbo].[tbl_ChannelRelations]  where [EntityType]=2
CREATE VIEW [dbo].[v_ChannelExaminationRelation]
AS
SELECT  [ChannelTypeId],[EntityId],[EntityType] FROM [dbo].[tbl_ChannelRelations]  where [EntityType]=3
CREATE VIEW [dbo].[v_ChannelTreatmentRelation]
AS
SELECT  [ChannelTypeId],[EntityId],[EntityType] FROM [dbo].[tbl_ChannelRelations]  where [EntityType]=4
CREATE VIEW [dbo].[v_ChannelQuestionRelation]
AS
SELECT  [ChannelTypeId],[EntityId],[EntityType] FROM [dbo].[tbl_ChannelRelations]  where [EntityType]=5
CREATE VIEW [dbo].[v_ChannelDiseaseRelation]
AS
SELECT  [ChannelTypeId],[EntityId],[EntityType] FROM [dbo].[tbl_ChannelRelations]  where [EntityType]=6
CREATE VIEW [dbo].[v_ChannelDrugRelation]
AS
SELECT  [ChannelTypeId],[EntityId],[EntityType] FROM [dbo].[tbl_ChannelRelations]  where [EntityType]=7
CREATE VIEW [dbo].[v_ChannelVaccineRelation]
AS
SELECT  [ChannelTypeId],[EntityId],[EntityType] FROM [dbo].[tbl_ChannelRelations]  where [EntityType]=8
  
create table tbl_entityType
(
Id int primary key not null,
Name varchar(50) not null
)
insert into tbl_entityType (Id,name) values(1,'Expert')
insert into tbl_entityType (Id,name) values(2,'MedicalInstitute')
insert into tbl_entityType (Id,name) values(3,'Examination')
insert into tbl_entityType (Id,name) values(4,'Treatment')
insert into tbl_entityType (Id,name) values(5,'Question')
insert into tbl_entityType (Id,name) values(6,'Disease')
insert into tbl_entityType (Id,name) values(7,'Drug')
insert into tbl_entityType (Id,name) values(8,'Vaccine')

create table tbl_ChannelType
(
Id int identity(1,1) primary key not null,
Name nvarchar(50) not null,
hebName nvarchar(50) null
)

insert into tbl_ChannelType (name,hebName) values('ImedWebsite',N'פורטל Infomed')
insert into tbl_ChannelType (name,hebName) values('Aesthetic',N'ערוץ אסטטיקה')
-------------------------------------------------  
update ximmuntbl set promotionlevel=5  where promotionlevel =50
--------------------------------------------------------
create view v_pharmacyClicksStatistics
as
SELECT [StatEntityId],  count( A.StatEntityId) Clicks,DATEADD(dd, 0, DATEDIFF(dd, 0,  A.[StatDate])) [Dates]
FROM [tbl_Stats] A 
where A.[StatType]=1 and statEntitytype=3
group by [StatEntityId], DATEADD(dd, 0, DATEDIFF(dd, 0,  A.[StatDate]))
 go
create view v_pharmacyWatchesStatistics
as
SELECT [StatEntityId],  count( A.StatEntityId) Watches,DATEADD(dd, 0, DATEDIFF(dd, 0,  A.[StatDate])) [Dates]
FROM [tbl_Stats] A 
where A.[StatType]=2 and statEntitytype=3
group by [StatEntityId], DATEADD(dd, 0, DATEDIFF(dd, 0,  A.[StatDate]))
 go

create view v_InstituteClicksStatistics
as
SELECT [StatEntityId],  count( A.StatEntityId) Clicks,DATEADD(dd, 0, DATEDIFF(dd, 0,  A.[StatDate])) [Dates]
FROM  [tbl_Stats] A 
where A.[StatType]=1 and statEntitytype=2
group by [StatEntityId], DATEADD(dd, 0, DATEDIFF(dd, 0,  A.[StatDate]))
go
create view v_InstituteWatchesStatistics
as
SELECT [StatEntityId],  count( A.StatEntityId) Watches,DATEADD(dd, 0, DATEDIFF(dd, 0,  A.[StatDate])) [Dates]
FROM [dbo].[tbl_Stats] A 
where A.[StatType]=2 and statEntitytype=2
group by [StatEntityId], DATEADD(dd, 0, DATEDIFF(dd, 0,  A.[StatDate]))
go

create view v_ExpertClicksStatistics
as
SELECT [StatEntityId],  count( A.StatEntityId) Clicks,DATEADD(dd, 0, DATEDIFF(dd, 0,  A.[StatDate])) [Dates]
FROM [dbo].[tbl_Stats] A 
where A.[StatType]=1 and statEntitytype=1
group by [StatEntityId], DATEADD(dd, 0, DATEDIFF(dd, 0,  A.[StatDate]))
go

create view v_ExpertWatchesStatistics
as
SELECT [StatEntityId],  count( A.StatEntityId) Watches,DATEADD(dd, 0, DATEDIFF(dd, 0,  A.[StatDate])) [Dates]
FROM [dbo].[tbl_Stats] A 
where A.[StatType]=2 and statEntitytype=1
group by [StatEntityId], DATEADD(dd, 0, DATEDIFF(dd, 0,  A.[StatDate]))
--------------------------------------------------------------------------------------------------------
CREATE VIEW [dbo].[v_Promotion]
AS
SELECT     ID, PromotionType, EntityType, EntityID, EntityName, PromotionLevel, StartDate, EndDate, Comments, AreaIDsList, WholeCountry, SpecialtyIDsList, SubTypeIDsList, 
                      LastUpdateDate, UpdatedBy, ReasonForEnding, Active, CASE WHEN EntityType = 1 THEN EntityID ELSE NULL END AS ExpertID, 
                      CASE WHEN EntityType = 2 THEN EntityID ELSE NULL END AS InstituteID, CASE WHEN EntityType = 6 THEN EntityID ELSE NULL END AS PharmacyID
FROM         dbo.tbl_Promotions

GO
--------------------------------------------------------------------------------------------------------

CREATE VIEW [dbo].[v_sponsership]
AS
SELECT     ID, PromotionID, EntityID, EntityType, BannerFilePath, BannerLink, LogoFilePath, PromoText, Email, typeId, CASE WHEN EntityType = 1 THEN EntityID ELSE NULL 
                      END AS ExpertID, CASE WHEN EntityType = 2 THEN EntityID ELSE NULL END AS InstituteID, CASE WHEN EntityType = 6 THEN EntityID ELSE NULL 
                      END AS PharmacyID
FROM         dbo.tbl_Sponsorships

GO
--------------------------------------------------------------------------------------------------------
CREATE TABLE [dbo].[tbl_sponsorshipBoxType](
	[ID] [int] NOT NULL,
	[Name] [nvarchar](20) NULL,
 CONSTRAINT [PK_tbl_sponsorshipBoxType] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

insert into tbl_sponsorshipBoxType values(1,'InPageBox')
insert into tbl_sponsorshipBoxType values(2,'SidePageBox')
-------------------------------------------------------------------------------------------------------
create procedure p_ExpertCategory_Delete
@expertid int,
@categoryId int
as
delete from tbl_ExpertCategoriesRelations  where ExpertID=@expertid and CategoryID in (select id from tbl_Categories where ParentID=@categoryId)
-------------------------------------------------------------------------------------------------------


GO



/****** Object:  View [dbo].[v_MajorCategoriesExpertRealtions]    Script Date: 10/06/2011 12:12:49 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
----------------------------------------------------------------------
--Medical Institute
drop view 
create view  [dbo].[v_InstituteMajorCategoriesRelations]
as
SELECT      R.InstituteID, R.[CategoryID]
FROM         dbo.tbl_InstituteCategoryRelations R,[tbl_Categories] C 
WHERE   R.[CategoryID] =C.Id and  C.ParentID IS NULL

GO

create view  [dbo].v_InstituteSubCategoriesRelations
as
SELECT     R.InstituteID,R.[CategoryID],R.ID ,C.[ParentID]
FROM         tbl_InstituteCategoryRelations R,[tbl_Categories] C 
WHERE   R.[CategoryID] =C.Id and  C.ParentID IS NOT NULL


GO

----------------------------------------------------------------------
--Articles

create view  [dbo].[v_ArticleMajorCategoriesRelations]
as
SELECT      R.ArticleID, R.[CategoryID]
FROM         tbl_ArticleCategoryRelations R,[tbl_Categories] C 
WHERE   R.[CategoryID] =C.Id and  C.ParentID IS NULL

GO

create view  v_ArticleSubCategoriesRelations
as
SELECT     R.ArticleID, R.ID,R.[CategoryID],C.[ParentID]
FROM         tbl_ArticleCategoryRelations R,[tbl_Categories] C 
WHERE   R.[CategoryID] =C.Id and  C.ParentID IS NOT NULL


GO
----------------------------------------------------------------------
--diseases

create view  [dbo].[v_DiseaseMajorCategoriesRelations]
as
SELECT      R.DiseaseID, R.[CategoryID]
FROM        dbo.tbl_DiseaseCategoryRelations  R,[tbl_Categories] C 
WHERE   R.[CategoryID] =C.Id and  C.ParentID IS NULL

GO

create view  [dbo].v_DiseaseSubCategoriesRelations
as
SELECT     R.DiseaseID, R.ID,R.[CategoryID],C.[ParentID]
FROM        dbo.tbl_DiseaseCategoryRelations  R,[tbl_Categories] C 
WHERE   R.[CategoryID] =C.Id and  C.ParentID IS NOT NULL


GO
-------------------------------------------------------
--------------------------------------------------------------------
--Drug

create view  v_DrugMajorCategoriesRelations
as
SELECT      R.DrugID, R.[CategoryID]
FROM        tbl_DrugCategoryRelations  R,[tbl_Categories] C 
WHERE   R.[CategoryID] =C.Id and  C.ParentID IS NULL

GO

create view  v_DrugSubCategoriesRelations
as
SELECT     R.DrugID, R.ID,R.[CategoryID],C.[ParentID]
FROM        tbl_DrugCategoryRelations  R,[tbl_Categories] C 
WHERE   R.[CategoryID] =C.Id and  C.ParentID IS NOT NULL


GO
----------------------------------------------------------------------
--Vaccine
create view  v_VaccineMajorCategoriesRelations
as
SELECT      R.VaccineID, R.[CategoryID]
FROM        tbl_VaccineCategoryRelations  R,[tbl_Categories] C 
WHERE   R.[CategoryID] =C.Id and  C.ParentID IS NULL

GO

create view  v_VaccineSubCategoriesRelations
as
SELECT     R.VaccineID, R.ID,R.[CategoryID],C.[ParentID]
FROM        tbl_VaccineCategoryRelations  R,[tbl_Categories] C 
WHERE   R.[CategoryID] =C.Id and  C.ParentID IS NOT NULL


GO
----------------------------------------------------------------------
--Treatment
create view  v_TreatmentMajorCategoriesRelations
as
SELECT      R.TreatmentID, R.[CategoryID]
FROM        tbl_TreatmentCategoryRelations  R,[tbl_Categories] C 
WHERE   R.[CategoryID] =C.Id and  C.ParentID IS NULL

GO
create view  v_TreatmentSubCategoriesRelations
as
SELECT     R.TreatmentID, R.ID,R.[CategoryID],C.[ParentID]
FROM        tbl_TreatmentCategoryRelations  R,[tbl_Categories] C 
WHERE   R.[CategoryID] =C.Id and  C.ParentID IS NOT NULL

GO
----------------------------------------------------------------------
--Question
create view  v_QuestionMajorCategoriesRelations
as
SELECT      R.QuestionID, R.[CategoryID]
FROM        tbl_QuestionCategoryRelations  R,[tbl_Categories] C 
WHERE   R.[CategoryID] =C.Id and  C.ParentID IS NULL

GO

create view  v_QuestionSubCategoriesRelations
as
SELECT     R.QuestionID, R.ID,R.[CategoryID],C.[ParentID]
FROM        tbl_QuestionCategoryRelations  R,[tbl_Categories] C 
WHERE   R.[CategoryID] =C.Id and  C.ParentID IS NOT NULL

GO
----------------------------------------------------------------------
--Examination
create view  v_ExaminationMajorCategoriesRelations
as
SELECT      R.ExaminationID, R.[CategoryID]
FROM        tbl_ExaminationCategoryRelations  R,[tbl_Categories] C 
WHERE   R.[CategoryID] =C.Id and  C.ParentID IS NULL

GO

create view  v_ExaminationSubCategoriesRelations
as
SELECT     R.ExaminationID, R.ID,R.[CategoryID],C.[ParentID]
FROM        tbl_ExaminationCategoryRelations  R,[tbl_Categories] C 
WHERE   R.[CategoryID] =C.Id and  C.ParentID IS NOT NULL

go
----------------------------------------------------------------------
--Category
create view  v_CategoryMajorCategoriesRelations
as
SELECT      R.CategoryID1, R.CategoryID2
FROM        tbl_CategoryCategoryRelations  R,[tbl_Categories] C 
WHERE   R.[CategoryID2] =C.Id and  C.ParentID IS NULL

GO

create view  v_CategorySubCategoriesRelations
as
SELECT     R.CategoryID1,  R.ID,R.CategoryID2,C.[ParentID]
FROM        tbl_CategoryCategoryRelations  R,[tbl_Categories] C 
WHERE   R.[CategoryID2] =C.Id and  C.ParentID IS NOT NULL

go
----------------------------------------------------------------------
--Definition
create view  v_DefinitionMajorCategoriesRelations
as
SELECT      R.DefinitionID, R.CategoryID
FROM        tbl_DefinitionCategoryRelations  R,[tbl_Categories] C 
WHERE   R.[CategoryID] =C.Id and  C.ParentID IS NULL

GO

create view  v_DefinitionSubCategoriesRelations
as
SELECT     R.DefinitionID,  R.ID,R.CategoryID,C.[ParentID]
FROM        tbl_DefinitionCategoryRelations  R,[tbl_Categories] C 
WHERE   R.[CategoryID] =C.Id and  C.ParentID IS NOT NULL

go
----------------------------------------------------------------------------

GO

/****** Object:  View [dbo].[v_MajorCategoriesExpertRealtions]    Script Date: 10/06/2011 12:12:49 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
----------------------------------------------------------------------
--Medical Institute

create view  [dbo].[v_ExpertMajorSpecialtiesRelations]
as
SELECT      R.ExpertID,R.[SpecialtyID]
FROM        dbo.tbl_ExpertSpecialtyRelations R,[tbl_Specialties] S 
WHERE  R.[SpecialtyID] =S.Id and  S.ParentID IS NULL

GO

create view  [dbo].v_ExpertSubSpecialtiesRelations
as
SELECT     R.ExpertID,R.[SpecialtyID],R.ID ,S.[ParentID],S.Name [Name]
FROM         dbo.tbl_ExpertSpecialtyRelations R,[tbl_Specialties] S 
WHERE  R.[SpecialtyID] =S.Id and  S.ParentID IS NOT NULL


GO

----------------------------------------------------------------------
--Medical Institute

create view  [dbo].[v_InstituteMajorSpecialtiesRelations]
as
SELECT      R.InstituteID,R.[SpecialtyID]
FROM         dbo.tbl_InstituteSpecialtyRelations R,[tbl_Specialties] S 
WHERE  R.[SpecialtyID] =S.Id and  S.ParentID IS NULL

GO

create view  [dbo].v_InstituteSubSpecialtiesRelations
as
SELECT     R.InstituteID,R.[SpecialtyID],R.ID ,S.[ParentID],S.Name [Name]
FROM         tbl_InstituteSpecialtyRelations R,[tbl_Specialties] S 
WHERE  R.[SpecialtyID] =S.Id and  S.ParentID IS NOT NULL


GO

----------------------------------------------------------------------
--Articles

create view  [dbo].[v_ArticleMajorSpecialtiesRelations]
as
SELECT      R.ArticleID,R.[SpecialtyID]
FROM         tbl_ArticleSpecialtyRelations R,[tbl_Specialties] S 
WHERE  R.[SpecialtyID] =S.Id and  S.ParentID IS NULL

GO

create view  v_ArticleSubSpecialtiesRelations
as
SELECT     R.ArticleID, R.ID,R.[SpecialtyID],S.[ParentID],S.Name [Name]
FROM         tbl_ArticleSpecialtyRelations R,[tbl_Specialties] S 
WHERE  R.[SpecialtyID] =S.Id and  S.ParentID IS NOT NULL


GO
----------------------------------------------------------------------
--diseases

create view  [dbo].[v_DiseaseMajorSpecialtiesRelations]
as
SELECT      R.DiseaseID,R.[SpecialtyID]
FROM        dbo.tbl_DiseaseSpecialtyRelations  R,[tbl_Specialties] S 
WHERE  R.[SpecialtyID] =S.Id and  S.ParentID IS NULL

GO

create view  [dbo].v_DiseaseSubSpecialtiesRelations
as
SELECT     R.DiseaseID, R.ID,R.[SpecialtyID],S.[ParentID],S.Name [Name]
FROM        dbo.tbl_DiseaseSpecialtyRelations  R,[tbl_Specialties] S 
WHERE  R.[SpecialtyID] =S.Id and  S.ParentID IS NOT NULL


GO
-------------------------------------------------------
--------------------------------------------------------------------
--Drug

create view  v_DrugMajorSpecialtiesRelations
as
SELECT      R.DrugID,R.[SpecialtyID]
FROM        tbl_DrugSpecialtyRelations  R,[tbl_Specialties] S 
WHERE  R.[SpecialtyID] =S.Id and  S.ParentID IS NULL

GO

create view  v_DrugSubSpecialtiesRelations
as
SELECT     R.DrugID, R.ID,R.[SpecialtyID],S.[ParentID],S.Name [Name]
FROM        tbl_DrugSpecialtyRelations  R,[tbl_Specialties] S 
WHERE  R.[SpecialtyID] =S.Id and  S.ParentID IS NOT NULL


GO
----------------------------------------------------------------------
--Vaccine
create view  v_VaccineMajorSpecialtiesRelations
as
SELECT      R.VaccineID,R.[SpecialtyID]
FROM        tbl_VaccineSpecialtyRelations  R,[tbl_Specialties] S 
WHERE  R.[SpecialtyID] =S.Id and  S.ParentID IS NULL

GO

create view  v_VaccineSubSpecialtiesRelations
as
SELECT     R.VaccineID, R.ID,R.[SpecialtyID],S.[ParentID],S.Name [Name]
FROM        tbl_VaccineSpecialtyRelations  R,[tbl_Specialties] S 
WHERE  R.[SpecialtyID] =S.Id and  S.ParentID IS NOT NULL


GO
----------------------------------------------------------------------
--Treatment
create view  v_TreatmentMajorSpecialtiesRelations
as
SELECT      R.TreatmentID,R.[SpecialtyID]
FROM        tbl_TreatmentSpecialtyRelations  R,[tbl_Specialties] S 
WHERE  R.[SpecialtyID] =S.Id and  S.ParentID IS NULL

GO
create view  v_TreatmentSubSpecialtiesRelations
as
SELECT     R.TreatmentID, R.ID,R.[SpecialtyID],S.[ParentID],S.Name [Name]
FROM         dbo.tbl_TreatmentSpecialtyRelations R,[tbl_Specialties] S 
WHERE  R.[SpecialtyID] =S.Id and  S.ParentID IS NOT NULL

GO
----------------------------------------------------------------------
--Examination
create view  v_ExaminationMajorSpecialtiesRelations
as
SELECT      R.ExaminationID,R.[SpecialtyID]
FROM        tbl_ExaminationSpecialtyRelations  R,[tbl_Specialties] S 
WHERE  R.[SpecialtyID] =S.Id and  S.ParentID IS NULL

GO

create view  v_ExaminationSubSpecialtiesRelations
as
SELECT     R.ExaminationID, R.ID,R.[SpecialtyID],S.[ParentID],S.Name [Name]
FROM        tbl_ExaminationSpecialtyRelations  R,[tbl_Specialties] S 
WHERE  R.[SpecialtyID] =S.Id and  S.ParentID IS NOT NULL

go
----------------------------------------------------------------------
--Category
create view  v_CategoryMajorSpecialtiesRelations
as
SELECT      R.CategoryID, R.[SpecialtyID]
FROM        tbl_CategorySpecialtyRelations  R,[tbl_Specialties] S 
WHERE   R.[SpecialtyID] =S.Id and  S.ParentID IS NULL

GO

create view  v_CategorySubSpecialtiesRelations
as
SELECT     R.CategoryID,  R.ID,R.[SpecialtyID],S.[ParentID],S.Name [Name]
FROM        tbl_CategorySpecialtyRelations  R,[tbl_Specialties] S 
WHERE   R.[SpecialtyID] =S.Id and  S.ParentID IS NOT NULL

go
----------------------------------------------------------------------
--Definition
create view  v_DefinitionMajorSpecialtiesRelations
as
SELECT      R.DefinitionID, R.[SpecialtyID]
FROM        tbl_DefinotionsSpecialtiesRelations  R,[tbl_Specialties] S 
WHERE  R.[SpecialtyID] =S.Id and  S.ParentID IS NULL

GO

create view  v_DefinitionSubSpecialtiesRelations
as
SELECT     R.DefinitionID,  R.ID,R.[SpecialtyID],S.[ParentID],S.Name [Name]
FROM        tbl_DefinotionsSpecialtiesRelations  R,[tbl_Specialties] S 
WHERE  R.[SpecialtyID] =S.Id and  S.ParentID IS NOT NULL

go


---------------------------------------------------------------------------------
--delete duplicates
delete  from  tbl_InstituteCategoryRelations where CategoryID is null or InstituteId is null

DELETE tbl_InstituteCategoryRelations 
FROM tbl_InstituteCategoryRelations
LEFT OUTER JOIN (
   SELECT MIN(id) as RowId, InstituteId, CategoryID 
   FROM tbl_InstituteCategoryRelations 
   GROUP BY InstituteId, CategoryID 
) as KeepRows ON
   tbl_InstituteCategoryRelations.id = KeepRows.RowId
WHERE
   KeepRows.RowId IS NULL
------------------------------
 delete  from  tbl_QuestionCategoryRelations where CategoryID is null or QuestionId is null

DELETE tbl_QuestionCategoryRelations 
FROM tbl_QuestionCategoryRelations
LEFT OUTER JOIN (
   SELECT MIN(id) as RowId, QuestionId, CategoryID 
   FROM tbl_QuestionCategoryRelations 
   GROUP BY QuestionId, CategoryID 
) as KeepRows ON
   tbl_QuestionCategoryRelations.id = KeepRows.RowId
WHERE
   KeepRows.RowId IS NULL
   ------------------------------------
delete  from dbo.tbl_InstituteSpecialtyRelations  where SpecialtyID is null or InstituteId is null

DELETE tbl_InstituteSpecialtyRelations 
FROM tbl_InstituteSpecialtyRelations
LEFT OUTER JOIN (
   SELECT MIN(id) as RowId, InstituteId, SpecialtyID 
   FROM tbl_InstituteSpecialtyRelations 
   GROUP BY InstituteId, SpecialtyID 
) as KeepRows ON
   tbl_InstituteSpecialtyRelations.id = KeepRows.RowId
WHERE
   KeepRows.RowId IS NULL
   --------------------
   --delete duplicates from tbl_ArticleSpecialtyRelations
delete  from dbo.tbl_ArticleSpecialtyRelations  where SpecialtyID is null or articleId is null

DELETE tbl_ArticleSpecialtyRelations 
FROM tbl_ArticleSpecialtyRelations
LEFT OUTER JOIN (
   SELECT MIN(id) as RowId, articleId, SpecialtyID 
   FROM tbl_ArticleSpecialtyRelations 
   GROUP BY ArticleID, SpecialtyID 
) as KeepRows ON
   tbl_ArticleSpecialtyRelations.id = KeepRows.RowId
WHERE
   KeepRows.RowId IS NULL
   --------------------------------
   delete  from dbo.tbl_ExpertSpecialtyRelations  where SpecialtyID is null or ExpertId is null

DELETE tbl_ExpertSpecialtyRelations 
FROM tbl_ExpertSpecialtyRelations
LEFT OUTER JOIN (
   SELECT MIN(id) as RowId, ExpertId, SpecialtyID 
   FROM tbl_ExpertSpecialtyRelations 
   GROUP BY ExpertId, SpecialtyID 
) as KeepRows ON
   tbl_ExpertSpecialtyRelations.id = KeepRows.RowId
WHERE
   KeepRows.RowId IS NULL
   ------------------------------------
   
DELETE tbl_VaccineSpecialtyRelations 
FROM tbl_VaccineSpecialtyRelations
LEFT OUTER JOIN (
   SELECT MIN(id) as RowId, VaccineID, SpecialtyID 
   FROM tbl_VaccineSpecialtyRelations 
   GROUP BY VaccineID, SpecialtyID 
) as KeepRows ON
   tbl_VaccineSpecialtyRelations.id = KeepRows.RowId
WHERE
   KeepRows.RowId IS NULL
----------------------------------------------------------------------------------   
  --delete duplicates
delete  from dbo.tbl_DiseaseSpecialtyRelations  where SpecialtyID is null or DiseaseID is null

DELETE tbl_DiseaseSpecialtyRelations 
FROM tbl_DiseaseSpecialtyRelations
LEFT OUTER JOIN (
   SELECT MIN(id) as RowId, DiseaseID, SpecialtyID 
   FROM tbl_DiseaseSpecialtyRelations 
   GROUP BY DiseaseID, SpecialtyID 
) as KeepRows ON
   tbl_DiseaseSpecialtyRelations.id = KeepRows.RowId
WHERE
   KeepRows.RowId IS NULL
   -----------------------------------
   
   --------------------------------------------------------
create view v_ExaminationCategoryRelations
as
SELECT     ID, ExaminationID, CategoryID,
                          (SELECT     ParentID
                            FROM          dbo.tbl_Categories  with(nolock)
                            WHERE      (dbo.tbl_ExaminationCategoryRelations.CategoryID = ID)) AS ParentID
FROM         dbo.tbl_ExaminationCategoryRelations  with(nolock)
go
create view v_DiseasCategoryRelations
as
SELECT     ID, DiseaseID, CategoryID,
                          (SELECT     ParentID
                            FROM          dbo.tbl_Categories with(nolock)
                            WHERE      (dbo.tbl_DiseaseCategoryRelations.CategoryID = ID)) AS ParentID
FROM         dbo.tbl_DiseaseCategoryRelations  with(nolock)
go
create view v_ExpertCategoryRelations
as
SELECT     ID, ExpertID, CategoryID,
                          (SELECT     ParentID
                            FROM          dbo.tbl_Categories  with(nolock)
                            WHERE      (dbo.tbl_ExpertCategoriesRelations.CategoryID = ID)) AS ParentID
FROM         dbo.tbl_ExpertCategoriesRelations  with(nolock)
go

create view v_DefinitionCategoryRelations
as
SELECT     ID, DefinitionID, CategoryID,
                          (SELECT     ParentID
                            FROM          dbo.tbl_Categories  with(nolock)
                            WHERE      (dbo.tbl_DefinitionCategoryRelations.CategoryID = ID)) AS ParentID
FROM         dbo.tbl_DefinitionCategoryRelations  with(nolock)
go
create view v_VaccineCategoryRelations
as
SELECT     ID, VaccineID, CategoryID,
                          (SELECT     ParentID
                            FROM          dbo.tbl_Categories with(nolock)
                            WHERE      (dbo.tbl_VaccineCategoryRelations.CategoryID = ID)) AS ParentID
FROM         dbo.tbl_VaccineCategoryRelations with(nolock)
go
create view v_TreatmentCategoryRelations
as
SELECT     ID, TreatmentID, CategoryID,
                          (SELECT     ParentID
                            FROM          dbo.tbl_Categories with(nolock)
                            WHERE      (dbo.tbl_TreatmentCategoryRelations.CategoryID = ID)) AS ParentID
FROM         dbo.tbl_TreatmentCategoryRelations with(nolock)
go
create view v_ArticleCategoryRelations
as
SELECT     ID, ArticleID, CategoryID,
                          (SELECT     ParentID
                            FROM          dbo.tbl_Categories with(nolock)
                            WHERE      (dbo.tbl_ArticleCategoryRelations.CategoryID = ID)) AS ParentID
FROM         dbo.tbl_ArticleCategoryRelations with(nolock)
go
create view v_QuestionCategoryRelations
as
SELECT     ID, QuestionID, CategoryID,
                          (SELECT     ParentID
                            FROM          dbo.tbl_Categories with(nolock)
                            WHERE      (dbo.tbl_QuestionCategoryRelations.CategoryID = ID)) AS ParentID
FROM         dbo.tbl_QuestionCategoryRelations with(nolock)
go
create view v_CategoryCategoryRelations
as
SELECT     ID, CategoryID1, CategoryID2,
                          (SELECT     ParentID
                            FROM          dbo.tbl_Categories
                            WHERE      (dbo.tbl_CategoryCategoryRelations.CategoryID1 = ID)) AS ParentID
FROM         dbo.tbl_CategoryCategoryRelations
go
create view v_SpecialtyCategoryRelations
as
SELECT     ID, SpecialtyID, CategoryID,
                          (SELECT     ParentID
                            FROM          dbo.tbl_Categories with(nolock)
                            WHERE      (dbo.tbl_CategorySpecialtyRelations.CategoryID = ID)) AS ParentID
FROM         dbo.tbl_CategorySpecialtyRelations with(nolock)
go
create view v_SponsorshipCategoryRelations
as
SELECT     ID, SponsorshipID, CategoryID,
                          (SELECT     ParentID
                            FROM          dbo.tbl_Categories with(nolock)
                            WHERE      (dbo.tbl_SponsorshipCategories.CategoryID = ID)) AS ParentID
FROM         dbo.tbl_SponsorshipCategories with(nolock)
go

create view v_DrugCategoryRelations
as
SELECT     ID, DrugID, CategoryID,
                          (SELECT     ParentID
                            FROM          dbo.tbl_Categories with(nolock)
                            WHERE      (dbo.tbl_DrugCategoryRelations.CategoryID = ID)) AS ParentID
FROM         dbo.tbl_DrugCategoryRelations with(nolock)
go

create view v_InstituteSubTypeCategoryRelations
as
SELECT     ID, SubTypeID, CategoryID,
                          (SELECT     ParentID
                            FROM          dbo.tbl_Categories with(nolock)
                            WHERE      (dbo.tbl_InstituteSubTypeCategoryRelations.CategoryID = ID)) AS ParentID
FROM         dbo.tbl_InstituteSubTypeCategoryRelations with(nolock)
go

create view v_InstituteCategoryRelations
as
SELECT     ID, InstituteID, CategoryID,
                          (SELECT     ParentID
                            FROM          dbo.tbl_Categories with(nolock)
                            WHERE      (dbo.tbl_InstituteCategoryRelations.CategoryID = ID)) AS ParentID
FROM         dbo.tbl_InstituteCategoryRelations with(nolock)
go
--------------------------------------------------------
create view v_ExaminationSpecialtyRelations
as
SELECT     ID, ExaminationID, SpecialtyID,
                          (SELECT     ParentID
                            FROM          dbo.tbl_Specialties with(nolock)
                            WHERE      (dbo.tbl_ExaminationSpecialtyRelations.SpecialtyID = ID)) AS ParentID
FROM         dbo.tbl_ExaminationSpecialtyRelations with(nolock)
go
   --------------------------------------------------------2

create view v_DiseasSpecialtyRelations
as
SELECT     ID, DiseaseID, SpecialtyID,
                          (SELECT     ParentID
                            FROM          dbo.tbl_Categories with(nolock)
                            WHERE      (dbo.tbl_DiseaseSpecialtyRelations.SpecialtyID = ID)) AS ParentID
FROM         dbo.tbl_DiseaseSpecialtyRelations with(nolock)
go
   --------------------------------------------------------3

create view v_ExpertSpecialtyRelations
as
SELECT     ID, ExpertID, SpecialtyID,
                          (SELECT     ParentID
                            FROM          dbo.tbl_Categories with(nolock)
                            WHERE      (dbo.tbl_ExpertSpecialtyRelations.SpecialtyID = ID)) AS ParentID
FROM         dbo.tbl_ExpertSpecialtyRelations with(nolock)
go
   --------------------------------------------------------4

create view v_DefinitionSpecialtyRelations
as
SELECT     ID, DefinitionID, SpecialtyID,
                          (SELECT     ParentID
                            FROM          dbo.tbl_Categories with(nolock)
                            WHERE      (dbo.tbl_DefinotionsSpecialtiesRelations.SpecialtyID = ID)) AS ParentID
FROM         dbo.tbl_DefinotionsSpecialtiesRelations with(nolock)
go
   --------------------------------------------------------5
create view v_VaccineSpecialtyRelations
as
SELECT     ID, VaccineID, SpecialtyID,
                          (SELECT     ParentID
                            FROM          dbo.tbl_Categories with(nolock)
                            WHERE      (dbo.tbl_VaccineSpecialtyRelations.SpecialtyID = ID)) AS ParentID
FROM         dbo.tbl_VaccineSpecialtyRelations with(nolock)
go
   --------------------------------------------------------7

create view v_TreatmentSpecialtyRelations
as
SELECT     ID, TreatmentID, SpecialtyID,
                          (SELECT     ParentID
                            FROM          dbo.tbl_Categories with(nolock)
                            WHERE      (dbo.tbl_TreatmentSpecialtyRelations.SpecialtyID = ID)) AS ParentID
FROM         dbo.tbl_TreatmentSpecialtyRelations with(nolock)
go
   --------------------------------------------------------8
create view v_ArticleSpecialtyRelations
as
SELECT     ID, ArticleID, SpecialtyID,
                          (SELECT     ParentID
                            FROM          dbo.tbl_Categories with(nolock)
                            WHERE      (dbo.tbl_ArticleSpecialtyRelations.SpecialtyID = ID)) AS ParentID
FROM         dbo.tbl_ArticleSpecialtyRelations with(nolock)
go
   --------------------------------------------------------9
create view v_DrugSpecialtyRelations
as
SELECT     ID, DrugID, SpecialtyID,
                          (SELECT     ParentID
                            FROM          dbo.tbl_Categories with(nolock)
                            WHERE      (dbo.tbl_DrugSpecialtyRelations.SpecialtyID = ID)) AS ParentID
FROM         dbo.tbl_DrugSpecialtyRelations with(nolock)
go

--------------------------------------------------------10
create view v_InstituteSubTypeSpecialtyRelations
as
SELECT     ID, SubTypeID, SpecialtyID,
                          (SELECT     ParentID
                            FROM          dbo.tbl_Categories with(nolock)
                            WHERE      (dbo.tbl_InstituteSubTypeSpecialtyRelations.SpecialtyID = ID)) AS ParentID
FROM         dbo.tbl_InstituteSubTypeSpecialtyRelations with(nolock)
go
--------------------------------------------------------11
create view v_InstituteSpecialtyRelations
as
SELECT     ID, InstituteID, SpecialtyID,
                          (SELECT     ParentID
                            FROM          dbo.tbl_Categories with(nolock)
                            WHERE      (dbo.tbl_InstituteSpecialtyRelations.SpecialtyID = ID)) AS ParentID
FROM         dbo.tbl_InstituteSpecialtyRelations with(nolock)
go
--------------------------------------------------------
update tbl_MedicalInstitutes set promotionlevel=8 where id in(
select id from dbo.tbl_MedicalInstitutes where promotionlevel>8)

update tbl_experts set promotionlevel=8 where id in(
select id from dbo.tbl_experts where promotionlevel>8)
--------------------------------------------------------
alter  table tbl_articles alter column FixedBody nvarchar(MAX) null
--------------------------------------------------------------------
create view v_DoctorOfTheDay
as
SELECT     tbl_PromotionHomepageRelations.ID, tbl_PromotionHomepageRelations.PromotionDate, tbl_PromotionHomepageRelations.PromotionPhrase,   tbl_Promotions.EntityName, tbl_Promotions.EntityID  FROM       tbl_PromotionHomepageRelations INNER JOIN tbl_Promotions ON tbl_PromotionHomepageRelations.PromotionID = tbl_Promotions.ID
create view v_DoctorOfTheDay_Expert
 as 
select id,fullname+' ('+specialtynameslist+') ID:'+cast(id as nvarchar(20)) as Expert from tbl_experts
--------------------------------------------------------------------

  create  Procedure [dbo].[p_Promotions_ValidateDateTodaysDoctorDates]
  @date as date
  As
  Begin
  select
      @date exists(SELECT PromotionDate
	FROM [tbl_PromotionHomepageRelations]
	WHERE PromotionDate>=GETDATE() and @date>getdate() 
END
----------------------------------------------------------------------
create table tbl_PromotionInstituteSubTypeRelation
  (
  id int identity(1,1) primary key,
  promotionID int foreign key references tbl_Promotions(ID) not null,
  SubTypeID int foreign key references tbl_InstituteSubTypesOptions(ID)  not null
  )
  -------------------------------------------------------------------------
  create PROCEDURE [dbo].[p_Promotions_UpdateInstituteSubTypeRelationForPromotion]

	@PromotionID INT

AS
BEGIN


	DECLARE @SubTypeIDsList NVARCHAR(250)
	SELECT @SubTypeIDsList = SubTypeIDsList FROM tbl_Promotions WHERE ID = @PromotionID

	delete tbl_PromotionInstituteSubTypeRelation
	where promotionID=@PromotionID

	insert into tbl_PromotionInstituteSubTypeRelation(promotionID,SubTypeID)
	SELECT @PromotionID, ID
	FROM tbl_InstituteSubTypesOptions S WITH(NOLOCK)
	WHERE ID IN (SELECT Item FROM dbo.fn_Split(@SubTypeIDsList, '|'))

END
USE [Webteb]
GO

/****** Object:  StoredProcedure [dbo].[p_Promotions_UpdateInstituteSubTypeRelationForAllPromotions]    Script Date: 11/01/2011 11:01:17 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Udi Shomer
-- Create date: 11/7/09
-- Description:	Get the specialties of a specific promotion
/*
[p_Promotions_GetInstituteSubTypesForPromotion] 1
*/
-- =============================================
create PROCEDURE [dbo].[p_Promotions_UpdateInstituteSubTypeRelationForAllPromotions]


AS
BEGIN

begin tran a
	declare 	@PromotionID INT
	DECLARE @SubTypeIDsList NVARCHAR(250)
	DECLARE @getPromotionID CURSOR
	
	SET @getPromotionID = CURSOR FOR
	SELECT ID
	FROM tbl_Promotions where [EntityType]=2
	OPEN @getPromotionID
	FETCH NEXT
	FROM @getPromotionID INTO @PromotionID
	WHILE @@FETCH_STATUS = 0
	BEGIN
	exec p_Promotions_UpdateInstituteSubTypeRelationForPromotion @PromotionID
	FETCH NEXT
	FROM @getPromotionID INTO @PromotionID
	END
	CLOSE @getPromotionID
	DEALLOCATE @getPromotionID
		
	
	
commit tran a
END

GO
exec [p_Promotions_UpdateInstituteSubTypeRelationForAllPromotions]

create table tbl_PromotionAreaRelation
(
ID int identity(1,1) primary key not null,
PromotionID int foreign key references tbl_promotions(ID) not null,
AreaID int  foreign key references tbl_Areas(ID) not null
)

create PROCEDURE [dbo].[p_Promotions_UpdatePromotionAreaRelationForPromotion]

	@PromotionID INT

AS
BEGIN


	DECLARE @AreaIDsList NVARCHAR(250)
	SELECT @AreaIDsList = AreaIDsList FROM tbl_Promotions WHERE ID = @PromotionID

	delete tbl_PromotionAreaRelation
	where promotionID=@PromotionID

	insert into tbl_PromotionAreaRelation(promotionID,AreaID)
	SELECT @PromotionID, ID
	FROM tbl_Areas S WITH(NOLOCK)
	WHERE ID IN (SELECT Item FROM dbo.fn_Split(@AreaIDsList, '|'))

END

GO

/****** Object:  StoredProcedure [dbo].[p_Promotions_UpdateInstituteSubTypeRelationForAllPromotions]    Script Date: 11/01/2011 11:01:17 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Udi Shomer
-- Create date: 11/7/09
-- Description:	Get the specialties of a specific promotion
/*
[p_Promotions_GetInstituteSubTypesForPromotion] 1
*/
-- =============================================

create PROCEDURE [dbo].[p_Promotions_UpdatePromotionAreaRelationForAllPromotions]


AS
BEGIN

begin tran a
	declare 	@PromotionID INT
	DECLARE @AreaIDsList NVARCHAR(250)
	DECLARE @getPromotionID CURSOR
	
	SET @getPromotionID = CURSOR FOR
	SELECT ID
	FROM tbl_Promotions where PromotionType=1
	OPEN @getPromotionID
	FETCH NEXT
	FROM @getPromotionID INTO @PromotionID
	WHILE @@FETCH_STATUS = 0
	BEGIN
	exec [p_Promotions_UpdatePromotionAreaRelationForPromotion] @PromotionID
	FETCH NEXT
	FROM @getPromotionID INTO @PromotionID
	END
	CLOSE @getPromotionID
	DEALLOCATE @getPromotionID
		
	
	
commit tran a
END

GO
exec [p_Promotions_UpdatePromotionAreaRelationForAllPromotions]

------------------------------------------------------------------------
create  PROCEDURE [dbo].[p_SetListsInPromotionInstitute_SubType]
	@id int
AS
BEGIN
--set @id=(select expertId from tbl_ExpertAreaRelations with(nolock) where id=@id);

exec [pd_EntitySetIDsListFieldsSeperator]
	'tbl_PromotionInstituteSubTypeRelation','SubTypeID','promotionID','tbl_Promotions','SubTypeIDsList','ID',@id,'|';
end

create  PROCEDURE [dbo].[p_SetListsInPromotionArea]
	@id int
AS
BEGIN
--set @id=(select expertId from tbl_ExpertAreaRelations with(nolock) where id=@id);

exec [pd_EntitySetIDsListFieldsSeperator]
	'tbl_PromotionAreaRelation','AreaID','promotionID','tbl_Promotions','AreaIDsList','ID',@id,'|';
end

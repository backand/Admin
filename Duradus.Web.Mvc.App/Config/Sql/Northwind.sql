/****** Object:  Table [dbo].[Purchase_Order_Status]    Script Date: 09/05/2012 17:37:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Purchase_Order_Status](
	[Status ID] [int] NOT NULL,
	[Status] [nvarchar](50) NULL,
 CONSTRAINT [aaaaaPurchase Order Status_PK] PRIMARY KEY CLUSTERED 
(
	[Status ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
CREATE TABLE [dbo].[durados_WF_Info](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ViewName] [nvarchar](150) NOT NULL,
	[GraphState] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_durados_WF_Info] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
CREATE UNIQUE NONCLUSTERED INDEX [IX_durados_WF_Info] ON [dbo].[durados_WF_Info] 
(
	[ViewName] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE TABLE [dbo].[durados_session](
	[SessionID] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[Scalar] [nvarchar](max) NULL,
	[SerializedObject] [ntext] NULL,
	[TypeCode] [nvarchar](50) NULL,
	[ObjectType] [nvarchar](500) NULL,
 CONSTRAINT [PK_Durados_Session] PRIMARY KEY CLUSTERED 
(
	[SessionID] ASC,
	[Name] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
CREATE TABLE [dbo].[durados_UserRole](
	[Name] [nvarchar](256) NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
	[FirstView] [nvarchar](200) NULL,
 CONSTRAINT [PK_durados_UserRole] PRIMARY KEY CLUSTERED 
(
	[Name] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
CREATE TABLE [dbo].[Inventory_Transaction_Types](
	[ID] [smallint] NOT NULL,
	[Type Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [aaaaaInventory Transaction Types_PK] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
CREATE TABLE [dbo].[Employees](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Company] [nvarchar](50) NULL,
	[Last Name] [nvarchar](50) NULL,
	[First Name] [nvarchar](50) NULL,
	[E-mail Address] [nvarchar](50) NULL,
	[Job Title] [nvarchar](50) NULL,
	[Business Phone] [nvarchar](25) NULL,
	[Home Phone] [nvarchar](25) NULL,
	[Mobile Phone] [nvarchar](25) NULL,
	[Fax Number] [nvarchar](25) NULL,
	[Address] [nvarchar](max) NULL,
	[City] [nvarchar](50) NULL,
	[State/Province] [nvarchar](50) NULL,
	[ZIP/Postal Code] [nvarchar](15) NULL,
	[Country/Region] [nvarchar](50) NULL,
	[Web Page] [nvarchar](max) NULL,
	[Notes] [nvarchar](max) NULL,
	[Attachments] [nvarchar](max) NULL,
 CONSTRAINT [aaaaaEmployees_PK] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
CREATE NONCLUSTERED INDEX [City] ON [dbo].[Employees] 
(
	[City] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE NONCLUSTERED INDEX [Company] ON [dbo].[Employees] 
(
	[Company] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE NONCLUSTERED INDEX [First Name] ON [dbo].[Employees] 
(
	[First Name] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE NONCLUSTERED INDEX [Last Name] ON [dbo].[Employees] 
(
	[Last Name] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE NONCLUSTERED INDEX [Postal Code] ON [dbo].[Employees] 
(
	[ZIP/Postal Code] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE NONCLUSTERED INDEX [State/Province] ON [dbo].[Employees] 
(
	[State/Province] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE TABLE [dbo].[durados_Html](
	[Name] [nvarchar](50) NOT NULL,
	[Text] [ntext] NOT NULL,
 CONSTRAINT [PK_durados_Html_1] PRIMARY KEY CLUSTERED 
(
	[Name] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
CREATE TABLE [dbo].[durados_Folder](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
	[UserId] [int] NOT NULL,
	[Ordinal] [int] NOT NULL,
 CONSTRAINT [PK_durados_Folder] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[durados_CustomViews](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[ViewName] [nvarchar](300) NOT NULL,
	[CustomView] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_durados_CustomViews] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
CREATE UNIQUE NONCLUSTERED INDEX [IX_durados_CustomViews] ON [dbo].[durados_CustomViews] 
(
	[UserId] ASC,
	[ViewName] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE TABLE [dbo].[durados_ChangeHistoryField](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ChangeHistoryId] [int] NOT NULL,
	[FieldName] [nvarchar](500) NOT NULL,
	[ColumnNames] [nvarchar](500) NOT NULL,
	[OldValue] [nvarchar](max) NOT NULL,
	[NewValue] [nvarchar](max) NOT NULL,
	[OldValueKey] [nvarchar](max) NULL,
	[NewValueKey] [nvarchar](max) NULL,
 CONSTRAINT [PK_durados_ChangeHistoryField] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
CREATE TABLE [dbo].[durados_ChangeHistory](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[ViewName] [nvarchar](150) NOT NULL,
	[PK] [nvarchar](250) NOT NULL,
	[ActionId] [int] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[UpdateUserId] [int] NOT NULL,
	[Comments] [nvarchar](max) NULL,
	[TransactionName] [nvarchar](50) NULL,
	[Version] [nvarchar](50) NULL,
	[Workspace] [nvarchar](50) NULL,
 CONSTRAINT [PK_durados_ChangeHistory] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
CREATE TABLE [dbo].[durados_Action](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_durados_Action] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO
CREATE TABLE [dbo].[CustomersJob Title](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NULL,
	[Ordinal] [int] NULL,
 CONSTRAINT [PK_CustomersJob Title] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
CREATE TABLE [dbo].[Durados_Log](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ApplicationName] [nvarchar](250) NULL,
	[Username] [nvarchar](250) NULL,
	[MachineName] [nvarchar](250) NULL,
	[Time] [datetime] NULL,
	[Controller] [nvarchar](250) NULL,
	[Action] [nvarchar](250) NULL,
	[MethodName] [nvarchar](250) NULL,
	[LogType] [int] NULL,
	[ExceptionMessage] [nvarchar](max) NULL,
	[Trace] [nvarchar](max) NULL,
	[FreeText] [nvarchar](max) NULL,
	[Guid] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Durados_Log] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
CREATE TABLE [dbo].[durados_MessageBoard](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Subject] [nvarchar](250) NULL,
	[Message] [nvarchar](max) NULL,
	[OriginatedUserId] [int] NULL,
	[ViewName] [nvarchar](250) NULL,
	[ViewDisplayName] [nvarchar](250) NULL,
	[PK] [nvarchar](250) NULL,
	[RowDisplayName] [nvarchar](250) NULL,
	[CreatedDate] [datetime] NULL,
	[RowLink] [nvarchar](350) NULL,
	[ViewLink] [nvarchar](350) NULL,
 CONSTRAINT [PK_durados_MessageBoard] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
CREATE TABLE [dbo].[Products](
	[Supplier IDs] [nvarchar](max) NULL,
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Product Code] [nvarchar](25) NULL,
	[Product Name] [nvarchar](50) NULL,
	[Description] [nvarchar](max) NULL,
	[Standard Cost] [money] NULL,
	[List Price] [money] NOT NULL,
	[Reorder Level] [smallint] NULL,
	[Target Level] [int] NULL,
	[Quantity Per Unit] [nvarchar](50) NULL,
	[Discontinued] [bit] NOT NULL,
	[Minimum Reorder Quantity] [smallint] NULL,
	[Category] [nvarchar](50) NULL,
	[Attachments] [nvarchar](max) NULL,
 CONSTRAINT [aaaaaProducts_PK] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
CREATE NONCLUSTERED INDEX [Product Code] ON [dbo].[Products] 
(
	[Product Code] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE TABLE [dbo].[Privileges](
	[Privilege ID] [int] IDENTITY(1,1) NOT NULL,
	[Privilege Name] [nvarchar](50) NULL,
 CONSTRAINT [aaaaaPrivileges_PK] PRIMARY KEY CLUSTERED 
(
	[Privilege ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
CREATE TABLE [dbo].[OrdersPayment Type](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NULL,
	[Ordinal] [int] NULL,
 CONSTRAINT [PK_OrdersPayment Type] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
CREATE TABLE [dbo].[Orders_Tax_Status](
	[ID] [smallint] NOT NULL,
	[Tax Status Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [aaaaaOrders Tax Status_PK] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
CREATE TABLE [dbo].[Orders_Status](
	[Status ID] [smallint] NOT NULL,
	[Status Name] [nvarchar](50) NOT NULL,
	[Ordinal] [int] NULL,
 CONSTRAINT [aaaaaOrders Status_PK] PRIMARY KEY CLUSTERED 
(
	[Status ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
CREATE TABLE [dbo].[Order_Details_Status](
	[Status ID] [int] NOT NULL,
	[Status Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [aaaaaOrder Details Status_PK] PRIMARY KEY CLUSTERED 
(
	[Status ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
CREATE TABLE [dbo].[Suppliers](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Company] [nvarchar](50) NULL,
	[Last Name] [nvarchar](50) NULL,
	[First Name] [nvarchar](50) NULL,
	[E-mail Address] [nvarchar](50) NULL,
	[Job Title] [nvarchar](50) NULL,
	[Business Phone] [nvarchar](25) NULL,
	[Home Phone] [nvarchar](25) NULL,
	[Mobile Phone] [nvarchar](25) NULL,
	[Fax Number] [nvarchar](25) NULL,
	[Address] [nvarchar](max) NULL,
	[City] [nvarchar](50) NULL,
	[State/Province] [nvarchar](50) NULL,
	[ZIP/Postal Code] [nvarchar](15) NULL,
	[Country/Region] [nvarchar](50) NULL,
	[Web Page] [nvarchar](max) NULL,
	[Notes] [nvarchar](250) NULL,
	[Attachments] [nvarchar](max) NULL,
 CONSTRAINT [aaaaaSuppliers_PK] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
CREATE NONCLUSTERED INDEX [City] ON [dbo].[Suppliers] 
(
	[City] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE NONCLUSTERED INDEX [Company] ON [dbo].[Suppliers] 
(
	[Company] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE NONCLUSTERED INDEX [First Name] ON [dbo].[Suppliers] 
(
	[First Name] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE NONCLUSTERED INDEX [Last Name] ON [dbo].[Suppliers] 
(
	[Last Name] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE NONCLUSTERED INDEX [Postal Code] ON [dbo].[Suppliers] 
(
	[ZIP/Postal Code] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE NONCLUSTERED INDEX [State/Province] ON [dbo].[Suppliers] 
(
	[State/Province] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE TABLE [dbo].[Strings](
	[String ID] [int] IDENTITY(1,1) NOT NULL,
	[String Data] [nvarchar](255) NULL,
 CONSTRAINT [aaaaaStrings_PK] PRIMARY KEY CLUSTERED 
(
	[String ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
CREATE TABLE [dbo].[Shippers](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Company] [nvarchar](50) NULL,
	[Last Name] [nvarchar](50) NULL,
	[First Name] [nvarchar](50) NULL,
	[E-mail Address] [nvarchar](50) NULL,
	[Job Title] [nvarchar](50) NULL,
	[Business Phone] [nvarchar](25) NULL,
	[Home Phone] [nvarchar](25) NULL,
	[Mobile Phone] [nvarchar](25) NULL,
	[Fax Number] [nvarchar](25) NULL,
	[Address] [nvarchar](max) NULL,
	[City] [nvarchar](50) NULL,
	[State/Province] [nvarchar](50) NULL,
	[ZIP/Postal Code] [nvarchar](15) NULL,
	[Country/Region] [nvarchar](50) NULL,
	[Web Page] [nvarchar](max) NULL,
	[Notes] [nvarchar](max) NULL,
	[Attachments] [nvarchar](max) NULL,
 CONSTRAINT [aaaaaShippers_PK] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
CREATE NONCLUSTERED INDEX [City] ON [dbo].[Shippers] 
(
	[City] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE NONCLUSTERED INDEX [Company] ON [dbo].[Shippers] 
(
	[Company] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE NONCLUSTERED INDEX [First Name] ON [dbo].[Shippers] 
(
	[First Name] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE NONCLUSTERED INDEX [Last Name] ON [dbo].[Shippers] 
(
	[Last Name] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE NONCLUSTERED INDEX [Postal Code] ON [dbo].[Shippers] 
(
	[ZIP/Postal Code] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE NONCLUSTERED INDEX [State/Province] ON [dbo].[Shippers] 
(
	[State/Province] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE TABLE [dbo].[Sales_Reports](
	[Group By] [nvarchar](50) NOT NULL,
	[Display] [nvarchar](50) NULL,
	[Title] [nvarchar](50) NULL,
	[Filter Row Source] [nvarchar](max) NULL,
	[Default] [bit] NOT NULL,
 CONSTRAINT [aaaaaSales Reports_PK] PRIMARY KEY CLUSTERED 
(
	[Group By] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
CREATE TABLE [dbo].[Purchase_OrdersPayment Method](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NULL,
	[Ordinal] [int] NULL,
 CONSTRAINT [PK_Purchase_OrdersPayment Method] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
CREATE TABLE [dbo].[Purchase_Orders](
	[Purchase Order ID] [int] IDENTITY(1,1) NOT NULL,
	[Supplier ID] [int] NULL,
	[Created By] [int] NULL,
	[Submitted Date] [datetime] NULL,
	[Creation Date] [datetime] NULL,
	[Status ID] [int] NULL,
	[Expected Date] [datetime] NULL,
	[Shipping Fee] [money] NOT NULL,
	[Taxes] [money] NOT NULL,
	[Payment Date] [datetime] NULL,
	[Payment Amount] [money] NULL,
	[Payment Method] [nvarchar](50) NULL,
	[Notes] [nvarchar](max) NULL,
	[Approved By] [int] NULL,
	[Approved Date] [datetime] NULL,
	[Submitted By] [int] NULL,
	[Payment MethodId] [int] NULL,
 CONSTRAINT [aaaaaPurchase Orders_PK] PRIMARY KEY CLUSTERED 
(
	[Purchase Order ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
CREATE UNIQUE NONCLUSTERED INDEX [ID] ON [dbo].[Purchase_Orders] 
(
	[Purchase Order ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE NONCLUSTERED INDEX [New_EmployeesOnPurchaseOrder] ON [dbo].[Purchase_Orders] 
(
	[Created By] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE NONCLUSTERED INDEX [New_PurchaseOrderStatusLookup] ON [dbo].[Purchase_Orders] 
(
	[Status ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE NONCLUSTERED INDEX [New_SuppliersOnPurchaseOrder] ON [dbo].[Purchase_Orders] 
(
	[Supplier ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE NONCLUSTERED INDEX [PurchaseOrderStatusLookup] ON [dbo].[Purchase_Orders] 
(
	[Status ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE NONCLUSTERED INDEX [Status ID] ON [dbo].[Purchase_Orders] 
(
	[Status ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE NONCLUSTERED INDEX [SupplierID] ON [dbo].[Purchase_Orders] 
(
	[Supplier ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE NONCLUSTERED INDEX [SuppliersOnPurchaseOrder] ON [dbo].[Purchase_Orders] 
(
	[Supplier ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
CREATE PROCEDURE [dbo].[Durados_LogInsert]
    @ApplicationName nvarchar(250) = null,
	@Username nvarchar(250) = null,
	@MachineName nvarchar(250) = null,
	@Time datetime = null,
	@Controller nvarchar(250) = null,
	@Action nvarchar(250) = null,
	@MethodName nvarchar(250) = null,
	@LogType  int = null,
    @ExceptionMessage nvarchar(max) = null,
	@Trace nvarchar(max) = null,
	@FreeText nvarchar(max) = null,
	@Guid uniqueIdentifier = null
AS
BEGIN

INSERT INTO [dbo].[Durados_Log]
			([ApplicationName]
			,[Username]
			,[MachineName]
			,[Time]
			,[Controller]
			,[Action]
			,[MethodName]
			,[LogType]
			,[ExceptionMessage]
			,[Trace]
			,[FreeText]
			,[Guid])
		VALUES
			(@ApplicationName
			,@Username
			,@MachineName
			,@Time
			,@Controller
			,@Action
			,@MethodName
			,@LogType
			,@ExceptionMessage
			,@Trace
			,@FreeText
			,@Guid)
    
    RETURN 0
END
GO
-- =============================================
-- Author:		<Author,,Relly>
-- Create date: <Create Date,,2009-10-14>
-- Description:	<Description,,Clear the log of all applications>
-- =============================================
CREATE PROCEDURE [dbo].[Durados_LogClear] 
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	truncate table Durados_Log;
END
GO
CREATE TABLE [dbo].[durados_Link](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[LinkType] [smallint] NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[Ordinal] [int] NOT NULL,
	[ViewName] [nvarchar](150) NULL,
	[ControllerName] [nvarchar](150) NULL,
	[Guid] [nvarchar](150) NULL,
	[Url] [nvarchar](500) NULL,
	[Filter] [nvarchar](max) NULL,
	[SortColumn] [nvarchar](150) NULL,
	[SortDirection] [varchar](5) NULL,
	[PageNo] [smallint] NOT NULL,
	[PageSize] [smallint] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[Description] [nvarchar](2000) NULL,
	[FolderId] [int] NULL,
 CONSTRAINT [PK_durados_Link] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
CREATE TABLE [dbo].[Customers](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Company] [nvarchar](50) NULL,
	[Last Name] [nvarchar](50) NULL,
	[First Name] [nvarchar](50) NULL,
	[E-mail Address] [nvarchar](50) NULL,
	[Job Title] [nvarchar](50) NULL,
	[Business Phone] [nvarchar](25) NULL,
	[Home Phone] [nvarchar](25) NULL,
	[Mobile Phone] [nvarchar](25) NULL,
	[Fax Number] [nvarchar](25) NULL,
	[Address] [nvarchar](max) NULL,
	[City] [nvarchar](50) NULL,
	[State/Province] [nvarchar](50) NULL,
	[ZIP/Postal Code] [nvarchar](15) NULL,
	[Country/Region] [nvarchar](50) NULL,
	[Web Page] [nvarchar](max) NULL,
	[Notes] [nvarchar](max) NULL,
	[Attachments] [nvarchar](max) NULL,
	[Job TitleId] [int] NULL,
 CONSTRAINT [aaaaaCustomers_PK] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
CREATE NONCLUSTERED INDEX [City] ON [dbo].[Customers] 
(
	[City] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE NONCLUSTERED INDEX [Company] ON [dbo].[Customers] 
(
	[Company] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE NONCLUSTERED INDEX [First Name] ON [dbo].[Customers] 
(
	[First Name] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE NONCLUSTERED INDEX [Last Name] ON [dbo].[Customers] 
(
	[Last Name] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE NONCLUSTERED INDEX [Postal Code] ON [dbo].[Customers] 
(
	[ZIP/Postal Code] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE NONCLUSTERED INDEX [State/Province] ON [dbo].[Customers] 
(
	[State/Province] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE TABLE [dbo].[Employee_Privileges](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Employee ID] [int] NOT NULL,
	[Privilege ID] [int] NOT NULL,
 CONSTRAINT [aaaaaEmployee Privileges_PK] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
CREATE NONCLUSTERED INDEX [EmployeePriviligesforEmployees] ON [dbo].[Employee_Privileges] 
(
	[Employee ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE NONCLUSTERED INDEX [EmployeePriviligesLookup] ON [dbo].[Employee_Privileges] 
(
	[Privilege ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE NONCLUSTERED INDEX [New_EmployeePriviligesforEmployees] ON [dbo].[Employee_Privileges] 
(
	[Employee ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE NONCLUSTERED INDEX [New_EmployeePriviligesLookup] ON [dbo].[Employee_Privileges] 
(
	[Privilege ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE NONCLUSTERED INDEX [Privilege ID] ON [dbo].[Employee_Privileges] 
(
	[Privilege ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE TABLE [dbo].[durados_User](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](256) NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[Email] [nvarchar](250) NOT NULL,
	[Password] [nvarchar](20) NULL,
	[Role] [nvarchar](256) NOT NULL,
	[Guid] [uniqueidentifier] NOT NULL,
	[Signature] [nvarchar](4000) NULL,
	[SignatureHTML] [nvarchar](4000) NULL,
	[IsApproved] [bit] NULL,
	[NewUser] [bit] NULL,
	[Comments] [nvarchar](max) NULL,
 CONSTRAINT [PK_durados_User] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO
CREATE PROCEDURE [dbo].[durados_UpdateWorkflowGraphState] 
@ViewName nvarchar(300),
@State nvarchar(MAX)

AS
SET transaction isolation level SERIALIZABLE
BEGIN TRANSACTION Upsert
DECLARE @err int, @rowcount int

UPDATE [dbo].[durados_WF_Info] SET [GraphState] = @State WHERE ViewName = @ViewName

set @rowcount = @@ROWCOUNT
set @err = @@ERROR

IF @err = 0 AND @rowcount = 0
BEGIN
INSERT INTO [dbo].[durados_WF_Info] ([ViewName], [GraphState]) VALUES (@ViewName,@State)
END
COMMIT TRANSACTION Upsert
GO
CREATE PROCEDURE [dbo].[durados_UpdateUserCustomView] 
@UserId int,
@ViewName nvarchar(300),
@CustomView nvarchar(MAX)

AS
SET transaction isolation level SERIALIZABLE
BEGIN TRANSACTION Upsert
DECLARE @err int, @rowcount int

UPDATE [dbo].[durados_CustomViews] SET [CustomView] = @CustomView WHERE UserId = @UserId AND ViewName = @ViewName

set @rowcount = @@ROWCOUNT
set @err = @@ERROR

IF @err = 0 AND @rowcount = 0
BEGIN
INSERT INTO [dbo].[durados_CustomViews] ([UserId], [ViewName], [CustomView]) VALUES (@UserId, @ViewName, @CustomView)
END
COMMIT TRANSACTION Upsert
GO
CREATE PROCEDURE [dbo].[durados_setsession]
	
	(
	@Name nvarchar(250) = null,
	@SessionID nvarchar(50) = null,
	@Scalar nvarchar(max) = null,
	@TypeCode nvarchar(50) = null
	)
	
AS
	DECLARE @isExist nvarchar(250)
	
	select @isExist = [Name] from durados_session with(nolock) where SessionID=@SessionID  and [Name]=@Name
	
	IF @isExist IS NOT NULL 
	BEGIN
        update durados_session set Scalar=@Scalar, TypeCode=@TypeCode, SerializedObject=null, ObjectType=null where SessionID=@SessionID and [Name]=@Name
	end
	else
	begin
		insert into durados_session(SessionID, [Name], Scalar, TypeCode) values (@SessionID, @Name, @Scalar, @TypeCode)

	end
GO
CREATE TABLE [dbo].[durados_MessageStatus](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MessageId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[Deleted] [bit] NULL,
	[Reviewed] [bit] NULL,
	[Important] [bit] NULL,
	[ActionRequired] [bit] NULL,
 CONSTRAINT [PK_durados_MessageStatus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO
CREATE VIEW [dbo].[durados_v_ChangeHistory]
AS
SELECT        ROW_NUMBER() OVER (ORDER BY dbo.durados_ChangeHistory.Id) AS AutoId, dbo.durados_ChangeHistory.ViewName, dbo.durados_ChangeHistory.PK, 
dbo.durados_ChangeHistory.ActionId, dbo.durados_ChangeHistory.UpdateDate, dbo.durados_ChangeHistory.UpdateUserId, dbo.durados_ChangeHistoryField.FieldName, 
dbo.durados_ChangeHistoryField.ColumnNames, dbo.durados_ChangeHistoryField.OldValue, dbo.durados_ChangeHistoryField.NewValue, dbo.durados_ChangeHistoryField.Id, 
dbo.durados_ChangeHistoryField.ChangeHistoryId, dbo.durados_ChangeHistory.Comments, dbo.durados_ChangeHistory.Version, dbo.durados_ChangeHistory.Workspace, 
cast(CASE WHEN Workspace = 'Admin' THEN - 1 ELSE 0 END AS bit) AS Admin, cast(CASE WHEN Workspace = 'Admin' AND TransactionName IS NULL 
THEN 0 ELSE - 1 END AS bit) AS Committed
FROM            dbo.durados_ChangeHistory WITH (NOLOCK) LEFT OUTER JOIN
                         dbo.durados_ChangeHistoryField WITH (NOLOCK) ON dbo.durados_ChangeHistory.id = dbo.durados_ChangeHistoryField.ChangeHistoryId
GO
CREATE VIEW [dbo].[v_Employees]
AS
SELECT ID, Company, [Last Name], [First Name], [E-mail Address], [Job Title], [Business Phone], [Home Phone], [Mobile Phone], [Fax Number], Address, City, 
               [State/Province], [ZIP/Postal Code], [Country/Region], [Web Page], Notes, Attachments, [First Name] + ' ' + [Last Name] AS FullName
FROM  dbo.Employees
GO
CREATE VIEW [dbo].[v_durados_User]
AS
SELECT        dbo.[durados_User].ID, dbo.[durados_User].Username, dbo.[durados_User].FirstName, dbo.[durados_User].LastName, dbo.[durados_User].Email, dbo.[durados_User].[Password], dbo.[durados_User].[Role], dbo.[durados_User].[Guid], 
                         dbo.[durados_User].[Signature], dbo.[durados_User].SignatureHTML, IsNull(IsApproved, 1) as IsApproved, dbo.[durados_User].FirstName + ' ' + dbo.[durados_User].LastName AS FullName, 
                         dbo.[durados_User].NewUser, dbo.[durados_User].Comments
FROM            dbo.[durados_User] with (NOLOCK)
GO
CREATE VIEW [dbo].[durados_v_MessageBoard]
AS
SELECT dbo.durados_MessageStatus.Id, dbo.durados_MessageBoard.Subject, dbo.durados_MessageBoard.Message, dbo.durados_MessageBoard.OriginatedUserId, 
                  dbo.durados_MessageBoard.ViewName, dbo.durados_MessageBoard.ViewDisplayName, dbo.durados_MessageBoard.PK, dbo.durados_MessageBoard.RowDisplayName, 
                  dbo.durados_MessageBoard.CreatedDate, dbo.durados_MessageBoard.RowLink, dbo.durados_MessageBoard.ViewLink, dbo.durados_MessageStatus.UserId, 
                  ISNULL(dbo.durados_MessageStatus.Deleted, 0) AS Deleted, ISNULL(dbo.durados_MessageStatus.Reviewed, 0) AS Reviewed, 
                  ISNULL(dbo.durados_MessageStatus.Important, 0) AS Important, ISNULL(dbo.durados_MessageStatus.ActionRequired, 0) AS ActionRequired, 
                  CASE WHEN dbo.durados_MessageStatus.Reviewed = 1 THEN '' ELSE 'Bold' END AS Css
FROM     dbo.durados_MessageBoard INNER JOIN
                  dbo.durados_MessageStatus ON dbo.durados_MessageBoard.Id = dbo.durados_MessageStatus.MessageId
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Durados_MessageBoard_Action] 
	-- Add the parameters for the stored procedure here
@MessageId int,
@UserId int,
@ActionID int,
@ActionValue bit


AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	declare @StatusId int 
	set @StatusId = null
	
	select @StatusId = Id from dbo.durados_MessageStatus
	where MessageId = @MessageId and UserId = @UserId

	if @StatusId is null
	begin
		if (@ActionId = 1) -- delete
			insert into dbo.durados_MessageStatus (MessageId, UserId, Deleted)
			values (@MessageId, @UserId, @ActionValue)
		else if (@ActionId = 2) -- reviewed
			insert into dbo.durados_MessageStatus (MessageId, UserId, Reviewed)
			values (@MessageId, @UserId, @ActionValue)
		else if (@ActionId = 3) -- important
			insert into dbo.durados_MessageStatus (MessageId, UserId, Important)
			values (@MessageId, @UserId, @ActionValue)
		else if (@ActionId = 4) -- required
			insert into dbo.durados_MessageStatus (MessageId, UserId, ActionRequired)
			values (@MessageId, @UserId, @ActionValue)
	end
	else
	begin
		if (@ActionId = 1) -- delete
			update dbo.durados_MessageStatus
			set Deleted = @ActionValue
			where Id = @StatusId
		else if (@ActionId = 2) -- reviewed
			update dbo.durados_MessageStatus
			set Reviewed = @ActionValue
			where Id = @StatusId
		else if (@ActionId = 3) -- important
			update dbo.durados_MessageStatus
			set Important = @ActionValue
			where Id = @StatusId
		else if (@ActionId = 4) -- required
			update dbo.durados_MessageStatus
			set ActionRequired = @ActionValue
			where Id = @StatusId
	
	end
END
GO
CREATE TABLE [dbo].[Orders](
	[Order ID] [int] IDENTITY(1,1) NOT NULL,
	[Employee ID] [int] NULL,
	[Customer ID] [int] NULL,
	[Order Date] [datetime] NULL,
	[Shipped Date] [datetime] NULL,
	[Shipper ID] [int] NULL,
	[Ship Name] [nvarchar](50) NULL,
	[Ship Address] [nvarchar](max) NULL,
	[Ship City] [nvarchar](50) NULL,
	[Ship State/Province] [nvarchar](50) NULL,
	[Ship ZIP/Postal Code] [nvarchar](50) NULL,
	[Ship Country/Region] [nvarchar](50) NULL,
	[Shipping Fee] [money] NULL,
	[Taxes] [money] NULL,
	[Payment Type] [nvarchar](50) NULL,
	[Paid Date] [datetime] NULL,
	[Notes] [nvarchar](4000) NULL,
	[Tax Rate] [float] NULL,
	[Tax Status] [smallint] NULL,
	[Status ID] [smallint] NULL,
	[Payment TypeId] [int] NULL,
 CONSTRAINT [aaaaaOrders_PK] PRIMARY KEY CLUSTERED 
(
	[Order ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
CREATE NONCLUSTERED INDEX [CustomerID] ON [dbo].[Orders] 
(
	[Customer ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE NONCLUSTERED INDEX [CustomerOnOrders] ON [dbo].[Orders] 
(
	[Customer ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE NONCLUSTERED INDEX [EmployeeID] ON [dbo].[Orders] 
(
	[Employee ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE NONCLUSTERED INDEX [EmployeesOnOrders] ON [dbo].[Orders] 
(
	[Employee ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE NONCLUSTERED INDEX [ID] ON [dbo].[Orders] 
(
	[Order ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE NONCLUSTERED INDEX [New_CustomerOnOrders] ON [dbo].[Orders] 
(
	[Customer ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE NONCLUSTERED INDEX [New_EmployeesOnOrders] ON [dbo].[Orders] 
(
	[Employee ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

CREATE NONCLUSTERED INDEX [New_OrderStatus] ON [dbo].[Orders] 
(
	[Status ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

CREATE NONCLUSTERED INDEX [New_ShipperOnOrder] ON [dbo].[Orders] 
(
	[Shipper ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

CREATE NONCLUSTERED INDEX [New_TaxStatusOnOrders] ON [dbo].[Orders] 
(
	[Tax Status] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

CREATE NONCLUSTERED INDEX [OrderStatus] ON [dbo].[Orders] 
(
	[Status ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

CREATE NONCLUSTERED INDEX [ShipperID] ON [dbo].[Orders] 
(
	[Shipper ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

CREATE NONCLUSTERED INDEX [ShipperOnOrder] ON [dbo].[Orders] 
(
	[Shipper ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

CREATE NONCLUSTERED INDEX [Status ID] ON [dbo].[Orders] 
(
	[Status ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

CREATE NONCLUSTERED INDEX [TaxStatusOnOrders] ON [dbo].[Orders] 
(
	[Tax Status] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

CREATE NONCLUSTERED INDEX [ZIP/Postal Code] ON [dbo].[Orders] 
(
	[Ship ZIP/Postal Code] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE TABLE [dbo].[Order_Details](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Order ID] [int] NOT NULL,
	[Product ID] [int] NULL,
	[Quantity] [decimal](18, 4) NOT NULL,
	[Unit Price] [money] NULL,
	[Discount] [float] NOT NULL,
	[Status ID] [int] NULL,
	[Date Allocated] [datetime] NULL,
	[Purchase Order ID] [int] NULL,
	[Inventory ID] [int] NULL,
 CONSTRAINT [aaaaaOrder Details_PK] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)

CREATE NONCLUSTERED INDEX [ID] ON [dbo].[Order_Details] 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

CREATE NONCLUSTERED INDEX [Inventory ID] ON [dbo].[Order_Details] 
(
	[Inventory ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

CREATE NONCLUSTERED INDEX [New_OrderDetails] ON [dbo].[Order_Details] 
(
	[Order ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

CREATE NONCLUSTERED INDEX [New_OrderStatusLookup] ON [dbo].[Order_Details] 
(
	[Status ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

CREATE NONCLUSTERED INDEX [New_ProductsOnOrders] ON [dbo].[Order_Details] 
(
	[Product ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

CREATE NONCLUSTERED INDEX [OrderDetails] ON [dbo].[Order_Details] 
(
	[Order ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

CREATE NONCLUSTERED INDEX [OrderID] ON [dbo].[Order_Details] 
(
	[Order ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

CREATE NONCLUSTERED INDEX [OrderStatusLookup] ON [dbo].[Order_Details] 
(
	[Status ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

CREATE NONCLUSTERED INDEX [ProductID] ON [dbo].[Order_Details] 
(
	[Product ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

CREATE NONCLUSTERED INDEX [ProductsOnOrders] ON [dbo].[Order_Details] 
(
	[Product ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

CREATE NONCLUSTERED INDEX [Purchase Order ID] ON [dbo].[Order_Details] 
(
	[Purchase Order ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

CREATE NONCLUSTERED INDEX [Status ID] ON [dbo].[Order_Details] 
(
	[Status ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE TABLE [dbo].[Invoices](
	[Invoice ID] [int] IDENTITY(1,1) NOT NULL,
	[Order ID] [int] NULL,
	[Invoice Date] [datetime] NULL,
	[Due Date] [datetime] NULL,
	[Tax] [money] NULL,
	[Shipping] [money] NULL,
	[Amount Due] [money] NULL,
 CONSTRAINT [aaaaaInvoices_PK] PRIMARY KEY CLUSTERED 
(
	[Invoice ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)

CREATE NONCLUSTERED INDEX [New_OrderInvoice] ON [dbo].[Invoices] 
(
	[Order ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

CREATE NONCLUSTERED INDEX [Order ID] ON [dbo].[Invoices] 
(
	[Order ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

CREATE NONCLUSTERED INDEX [OrderInvoice] ON [dbo].[Invoices] 
(
	[Order ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE TABLE [dbo].[Inventory_Transactions](
	[Transaction ID] [int] IDENTITY(1,1) NOT NULL,
	[Transaction Type] [smallint] NOT NULL,
	[Transaction Created Date] [datetime] NULL,
	[Transaction Modified Date] [datetime] NULL,
	[Product ID] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[Purchase Order ID] [int] NULL,
	[Customer Order ID] [int] NULL,
	[Comments] [nvarchar](255) NULL,
 CONSTRAINT [aaaaaInventory Transactions_PK] PRIMARY KEY CLUSTERED 
(
	[Transaction ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)

CREATE NONCLUSTERED INDEX [Customer Order ID] ON [dbo].[Inventory_Transactions] 
(
	[Customer Order ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

CREATE NONCLUSTERED INDEX [New_OrdersOnInventoryTransactions] ON [dbo].[Inventory_Transactions] 
(
	[Customer Order ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

CREATE NONCLUSTERED INDEX [New_ProductOnInventoryTransaction] ON [dbo].[Inventory_Transactions] 
(
	[Product ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

CREATE NONCLUSTERED INDEX [New_PuchaseOrdersonInventoryTransactions] ON [dbo].[Inventory_Transactions] 
(
	[Purchase Order ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

CREATE NONCLUSTERED INDEX [New_TransactionTypesOnInventoryTransactiosn] ON [dbo].[Inventory_Transactions] 
(
	[Transaction Type] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

CREATE NONCLUSTERED INDEX [OrdersOnInventoryTransactions] ON [dbo].[Inventory_Transactions] 
(
	[Customer Order ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

CREATE NONCLUSTERED INDEX [Product ID] ON [dbo].[Inventory_Transactions] 
(
	[Product ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

CREATE NONCLUSTERED INDEX [ProductOnInventoryTransaction] ON [dbo].[Inventory_Transactions] 
(
	[Product ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

CREATE NONCLUSTERED INDEX [PuchaseOrdersonInventoryTransactions] ON [dbo].[Inventory_Transactions] 
(
	[Purchase Order ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

CREATE NONCLUSTERED INDEX [Purchase Order ID] ON [dbo].[Inventory_Transactions] 
(
	[Purchase Order ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

CREATE NONCLUSTERED INDEX [TransactionTypesOnInventoryTransactiosn] ON [dbo].[Inventory_Transactions] 
(
	[Transaction Type] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

GO
create  view [dbo].[v_Products_On_Back_Order] as  SELECT [Order_Details].[Product ID] AS ID, Sum([Order_Details].Quantity) AS [Quantity On Back Order]
FROM [Order_Details]
WHERE ((([Order_Details].[Status ID])=4))
GROUP BY [Order_Details].[Product ID];
GO
CREATE view [dbo].[v_Inventory_On_Hold] as SELECT Inventory_Transactions.[Product ID] as ID, Sum(Inventory_Transactions.Quantity) AS [Quantity On Hold]
FROM Inventory_Transactions
WHERE (((Inventory_Transactions.[Transaction Type])=3))
GROUP BY Inventory_Transactions.[Product ID];
GO
CREATE TABLE [dbo].[Purchase_Order_Details](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Purchase Order ID] [int] NOT NULL,
	[Product ID] [int] NULL,
	[Quantity] [decimal](18, 4) NOT NULL,
	[Unit Cost] [money] NOT NULL,
	[Date Received] [datetime] NULL,
	[Posted To Inventory] [bit] NOT NULL,
	[Inventory ID] [int] NULL,
 CONSTRAINT [aaaaaPurchase Order Details_PK] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
CREATE NONCLUSTERED INDEX [ID] ON [dbo].[Purchase_Order_Details] 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
CREATE NONCLUSTERED INDEX [Inventory ID] ON [dbo].[Purchase_Order_Details] 
(
	[Inventory ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

CREATE NONCLUSTERED INDEX [InventoryTransactionsOnPurchaseOrders] ON [dbo].[Purchase_Order_Details] 
(
	[Inventory ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

CREATE NONCLUSTERED INDEX [New_InventoryTransactionsOnPurchaseOrders] ON [dbo].[Purchase_Order_Details] 
(
	[Inventory ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

CREATE NONCLUSTERED INDEX [New_ProductOnPurchaseOrderDetails] ON [dbo].[Purchase_Order_Details] 
(
	[Product ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

CREATE NONCLUSTERED INDEX [New_PurchaseOrderDeatilsOnPurchaseOrder] ON [dbo].[Purchase_Order_Details] 
(
	[Purchase Order ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

CREATE NONCLUSTERED INDEX [OrderID] ON [dbo].[Purchase_Order_Details] 
(
	[Purchase Order ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

CREATE NONCLUSTERED INDEX [ProductID] ON [dbo].[Purchase_Order_Details] 
(
	[Product ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

CREATE NONCLUSTERED INDEX [ProductOnPurchaseOrderDetails] ON [dbo].[Purchase_Order_Details] 
(
	[Product ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

CREATE NONCLUSTERED INDEX [PurchaseOrderDeatilsOnPurchaseOrder] ON [dbo].[Purchase_Order_Details] 
(
	[Purchase Order ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
CREATE view [dbo].[v_Inventory_Sold] as SELECT Inventory_Transactions.[Product ID] as ID, Sum(Inventory_Transactions.Quantity) AS [Quantity Sold]
FROM Inventory_Transactions
WHERE (((Inventory_Transactions.[Transaction Type])=2))
GROUP BY Inventory_Transactions.[Product ID];
GO
create view [dbo].[v_Inventory_Purchased] as SELECT Inventory_Transactions.[Product ID] as ID , Sum(Inventory_Transactions.Quantity) AS [Quantity Purchased]
FROM Inventory_Transactions
WHERE (((Inventory_Transactions.[Transaction Type])=1))
GROUP BY Inventory_Transactions.[Product ID];
GO
create view [dbo].[v_Inventory_On_Order] as SELECT [Purchase_Order_Details].[Product ID] AS ID, Sum([Purchase_Order_Details].Quantity) AS [Quantity On Order]
FROM [Purchase_Order_Details]
WHERE ((([Purchase_Order_Details].[Posted To Inventory])=0))
GROUP BY [Purchase_Order_Details].[Product ID];
GO
CREATE VIEW [dbo].[v_Inventory_base]
AS
SELECT ID, [Product Name], [Product Code], ISNULL
                   ((SELECT SUM(Quantity) AS Expr1
                     FROM   dbo.Inventory_Transactions WITH (NOLOCK)
                     WHERE ([Transaction Type] = 1) AND ([Product ID] = dbo.Products.ID)
                     GROUP BY [Product ID]), 0) AS [Qty Purchased], ISNULL
                   ((SELECT SUM(Quantity) AS Expr1
                     FROM   dbo.Inventory_Transactions AS Inventory_Transactions_10 WITH (NOLOCK)
                     WHERE ([Transaction Type] = 2) AND ([Product ID] = dbo.Products.ID)
                     GROUP BY [Product ID]), 0) AS [Quantity Sold], ISNULL
                   ((SELECT SUM(Quantity) AS Expr1
                     FROM   dbo.Inventory_Transactions AS Inventory_Transactions_9 WITH (NOLOCK)
                     WHERE ([Transaction Type] = 3) AND ([Product ID] = dbo.Products.ID)
                     GROUP BY [Product ID]), 0) AS [Qty On Hold], ISNULL
                   ((SELECT SUM(Quantity) AS Expr1
                     FROM   dbo.Purchase_Order_Details WITH (NOLOCK)
                     WHERE ([Posted To Inventory] = 0) AND ([Product ID] = dbo.Products.ID)
                     GROUP BY [Product ID]), 0) AS [Quantity On Order],
                   (SELECT ISNULL(SUM(Quantity), 0) AS Expr1
                    FROM   dbo.Inventory_Transactions AS Inventory_Transactions_8 WITH (NOLOCK)
                    WHERE ([Transaction Type] = 1) AND ([Product ID] = dbo.Products.ID)
                    GROUP BY [Product ID]) -
                   (SELECT ISNULL(SUM(Quantity), 0) AS Expr1
                    FROM   dbo.Inventory_Transactions AS Inventory_Transactions_7 WITH (NOLOCK)
                    WHERE ([Transaction Type] = 2) AND ([Product ID] = dbo.Products.ID)
                    GROUP BY [Product ID]) AS [Qty On Hand], ISNULL
                   ((SELECT SUM(Quantity) AS Expr1
                     FROM   dbo.Inventory_Transactions AS Inventory_Transactions_6 WITH (NOLOCK)
                     WHERE ([Transaction Type] = 1) AND ([Product ID] = dbo.Products.ID)
                     GROUP BY [Product ID]), 0) - ISNULL
                   ((SELECT SUM(Quantity) AS Expr1
                     FROM   dbo.Inventory_Transactions AS Inventory_Transactions_5 WITH (NOLOCK)
                     WHERE ([Transaction Type] = 2) AND ([Product ID] = dbo.Products.ID)
                     GROUP BY [Product ID]), 0) - ISNULL
                   ((SELECT SUM(Quantity) AS Expr1
                     FROM   dbo.Inventory_Transactions AS Inventory_Transactions_4 WITH (NOLOCK)
                     WHERE ([Transaction Type] = 3) AND ([Product ID] = dbo.Products.ID)
                     GROUP BY [Product ID]), 0) AS [Qty Available], ISNULL
                   ((SELECT SUM(Quantity) AS Expr1
                     FROM   dbo.Order_Details WITH (NOLOCK)
                     WHERE ([Status ID] = 4) AND ([Product ID] = dbo.Products.ID)
                     GROUP BY [Product ID]), 0) AS [Quantity On Back Order], [Reorder Level], [Target Level], ISNULL
                   ((SELECT ISNULL(SUM(Quantity), 0) AS Expr1
                     FROM   dbo.Inventory_Transactions AS Inventory_Transactions_3 WITH (NOLOCK)
                     WHERE ([Transaction Type] = 1) AND ([Product ID] = dbo.Products.ID)
                     GROUP BY [Product ID]) -
                   (SELECT ISNULL(SUM(Quantity), 0) AS Expr1
                    FROM   dbo.Inventory_Transactions AS Inventory_Transactions_2 WITH (NOLOCK)
                    WHERE ([Transaction Type] = 2) AND ([Product ID] = dbo.Products.ID)
                    GROUP BY [Product ID]) -
                   (SELECT ISNULL(SUM(Quantity), 0) AS Expr1
                    FROM   dbo.Inventory_Transactions AS Inventory_Transactions_1 WITH (NOLOCK)
                    WHERE ([Transaction Type] = 3) AND ([Product ID] = dbo.Products.ID)
                    GROUP BY [Product ID]), 0) + ISNULL
                   ((SELECT SUM(Quantity) AS Expr1
                     FROM   dbo.Purchase_Order_Details AS Purchase_Order_Details_1 WITH (NOLOCK)
                     WHERE ([Posted To Inventory] = 0) AND ([Product ID] = dbo.Products.ID)
                     GROUP BY [Product ID]), 0) - ISNULL
                   ((SELECT SUM(Quantity) AS Expr1
                     FROM   dbo.Order_Details AS Order_Details_1 WITH (NOLOCK)
                     WHERE ([Status ID] = 4) AND ([Product ID] = dbo.Products.ID)
                     GROUP BY [Product ID]), 0) AS [Current Level], [Minimum Reorder Quantity]
FROM  dbo.Products
GO
CREATE VIEW [dbo].[v_Products]
AS
SELECT dbo.Products.ID, dbo.Products.[Product Code], dbo.Products.[Product Name], dbo.Products.[Reorder Level], dbo.Products.[Target Level], 
               dbo.Products.[Minimum Reorder Quantity], ISNULL(dbo.v_Inventory_Purchased.[Quantity Purchased], 0) AS [Quantity Purchased], 
               ISNULL(dbo.v_Inventory_Sold.[Quantity Sold], 0) AS [Quantity Sold], ISNULL(dbo.v_Inventory_On_Hold.[Quantity On Hold], 0) AS [Quantity On Hold], 
               ISNULL(dbo.v_Inventory_On_Order.[Quantity On Order], 0) AS [Quantity On Order], ISNULL(dbo.v_Products_On_Back_Order.[Quantity On Back Order], 0) 
               AS [Quantity On Back Order]
FROM  dbo.Products LEFT OUTER JOIN
               dbo.v_Inventory_On_Order ON dbo.Products.ID = dbo.v_Inventory_On_Order.ID LEFT OUTER JOIN
               dbo.v_Products_On_Back_Order ON dbo.Products.ID = dbo.v_Products_On_Back_Order.ID LEFT OUTER JOIN
               dbo.v_Inventory_On_Hold ON dbo.Products.ID = dbo.v_Inventory_On_Hold.ID LEFT OUTER JOIN
               dbo.v_Inventory_Sold ON dbo.Products.ID = dbo.v_Inventory_Sold.ID LEFT OUTER JOIN
               dbo.v_Inventory_Purchased ON dbo.Products.ID = dbo.v_Inventory_Purchased.ID
GO
CREATE VIEW [dbo].[v_Inventory]
AS
SELECT dbo.Products.ID, dbo.Products.[Product Code], dbo.Products.[Product Name], dbo.Products.[Reorder Level], dbo.Products.[Target Level], 
               dbo.Products.[Minimum Reorder Quantity], ISNULL(dbo.v_Inventory_Purchased.[Quantity Purchased], 0) AS [Quantity Purchased], 
               ISNULL(dbo.v_Inventory_Sold.[Quantity Sold], 0) AS [Quantity Sold], ISNULL(dbo.v_Inventory_On_Hold.[Quantity On Hold], 0) AS [Quantity On Hold], 
               ISNULL(dbo.v_Inventory_On_Order.[Quantity On Order], 0) AS [Quantity On Order], ISNULL(dbo.v_Products_On_Back_Order.[Quantity On Back Order], 0) 
               AS [Quantity On Back Order]
FROM  dbo.Products LEFT OUTER JOIN
               dbo.v_Inventory_On_Order ON dbo.Products.ID = dbo.v_Inventory_On_Order.ID LEFT OUTER JOIN
               dbo.v_Products_On_Back_Order ON dbo.Products.ID = dbo.v_Products_On_Back_Order.ID LEFT OUTER JOIN
               dbo.v_Inventory_On_Hold ON dbo.Products.ID = dbo.v_Inventory_On_Hold.ID LEFT OUTER JOIN
               dbo.v_Inventory_Sold ON dbo.Products.ID = dbo.v_Inventory_Sold.ID LEFT OUTER JOIN
               dbo.v_Inventory_Purchased ON dbo.Products.ID = dbo.v_Inventory_Purchased.ID
GO
/****** Object:  Default [DF_durados_ChangeHistory_UpdateDate]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[durados_ChangeHistory] ADD  CONSTRAINT [DF_durados_ChangeHistory_UpdateDate]  DEFAULT (getdate()) FOR [UpdateDate]

/****** Object:  Default [DF_durados_MessageBoard_CreatedDate]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[durados_MessageBoard] ADD  CONSTRAINT [DF_durados_MessageBoard_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]

/****** Object:  Default [DF__Products__Standa__34C8D9D1]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Products] ADD  CONSTRAINT [DF__Products__Standa__34C8D9D1]  DEFAULT ((0)) FOR [Standard Cost]

/****** Object:  Default [DF__Products__List P__35BCFE0A]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Products] ADD  CONSTRAINT [DF__Products__List P__35BCFE0A]  DEFAULT ((0)) FOR [List Price]

/****** Object:  Default [DF__Products__Discon__36B12243]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Products] ADD  CONSTRAINT [DF__Products__Discon__36B12243]  DEFAULT ((0)) FOR [Discontinued]

/****** Object:  Default [DF__Sales Rep__Defau__4CA06362]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Sales_Reports] ADD  CONSTRAINT [DF__Sales Rep__Defau__4CA06362]  DEFAULT ((0)) FOR [Default]

/****** Object:  Default [DF__Purchase __Creat__440B1D61]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Purchase_Orders] ADD  CONSTRAINT [DF__Purchase __Creat__440B1D61]  DEFAULT (getdate()) FOR [Creation Date]

/****** Object:  Default [DF__Purchase __Statu__44FF419A]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Purchase_Orders] ADD  CONSTRAINT [DF__Purchase __Statu__44FF419A]  DEFAULT ((0)) FOR [Status ID]

/****** Object:  Default [DF__Purchase __Shipp__45F365D3]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Purchase_Orders] ADD  CONSTRAINT [DF__Purchase __Shipp__45F365D3]  DEFAULT ((0)) FOR [Shipping Fee]

/****** Object:  Default [DF__Purchase __Taxes__46E78A0C]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Purchase_Orders] ADD  CONSTRAINT [DF__Purchase __Taxes__46E78A0C]  DEFAULT ((0)) FOR [Taxes]

/****** Object:  Default [DF__Purchase __Payme__47DBAE45]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Purchase_Orders] ADD  CONSTRAINT [DF__Purchase __Payme__47DBAE45]  DEFAULT ((0)) FOR [Payment Amount]

/****** Object:  Default [DF__durados_L__Ordin__5D95E53A]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[durados_Link] ADD  DEFAULT ((1)) FOR [Ordinal]

/****** Object:  Default [DF__durados_L__PageN__5E8A0973]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[durados_Link] ADD  DEFAULT ((1)) FOR [PageNo]

/****** Object:  Default [DF__durados_L__PageS__5F7E2DAC]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[durados_Link] ADD  DEFAULT ((20)) FOR [PageSize]

/****** Object:  Default [DF__durados_L__Creat__5BAD9CC8]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[durados_Link] ADD  DEFAULT (getdate()) FOR [CreationDate]

/****** Object:  Default [DF_durados_User_Guid]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[durados_User] ADD  CONSTRAINT [DF_durados_User_Guid]  DEFAULT (newid()) FOR [Guid]

/****** Object:  Default [DF__Orders__Order Da__59063A47]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Orders] ADD  CONSTRAINT [DF__Orders__Order Da__59063A47]  DEFAULT (getdate()) FOR [Order Date]

/****** Object:  Default [DF__Orders__Shipping__59FA5E80]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Orders] ADD  CONSTRAINT [DF__Orders__Shipping__59FA5E80]  DEFAULT ((0)) FOR [Shipping Fee]

/****** Object:  Default [DF__Orders__Taxes__5AEE82B9]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Orders] ADD  CONSTRAINT [DF__Orders__Taxes__5AEE82B9]  DEFAULT ((0)) FOR [Taxes]

/****** Object:  Default [DF__Orders__Tax Rate__5BE2A6F2]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Orders] ADD  CONSTRAINT [DF__Orders__Tax Rate__5BE2A6F2]  DEFAULT ((0)) FOR [Tax Rate]

/****** Object:  Default [DF__Orders__Status I__5CD6CB2B]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Orders] ADD  CONSTRAINT [DF__Orders__Status I__5CD6CB2B]  DEFAULT ((0)) FOR [Status ID]

/****** Object:  Default [DF__Order Det__Quant__1B0907CE]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Order_Details] ADD  DEFAULT ((0)) FOR [Quantity]

/****** Object:  Default [DF__Order Det__Unit __1BFD2C07]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Order_Details] ADD  DEFAULT ((0)) FOR [Unit Price]

/****** Object:  Default [DF__Order Det__Disco__1CF15040]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Order_Details] ADD  DEFAULT ((0)) FOR [Discount]

/****** Object:  Default [DF__Invoices__Invoic__1367E606]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Invoices] ADD  DEFAULT (getdate()) FOR [Invoice Date]

/****** Object:  Default [DF__Invoices__Tax__145C0A3F]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Invoices] ADD  DEFAULT ((0)) FOR [Tax]

/****** Object:  Default [DF__Invoices__Shippi__15502E78]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Invoices] ADD  DEFAULT ((0)) FOR [Shipping]

/****** Object:  Default [DF__Invoices__Amount__164452B1]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Invoices] ADD  DEFAULT ((0)) FOR [Amount Due]

/****** Object:  Default [DF__Inventory__Trans__0DAF0CB0]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Inventory_Transactions] ADD  DEFAULT (getdate()) FOR [Transaction Created Date]

/****** Object:  Default [DF__Inventory__Trans__0EA330E9]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Inventory_Transactions] ADD  DEFAULT (getdate()) FOR [Transaction Modified Date]

/****** Object:  Default [DF__Purchase __Poste__3B75D760]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Purchase_Order_Details] ADD  DEFAULT ((0)) FOR [Posted To Inventory]

/****** Object:  Check [CK Purchase Orders Approved Date]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Purchase_Orders]  WITH CHECK ADD  CONSTRAINT [CK Purchase Orders Approved Date] CHECK  (([Approved Date]>='1/1/1900'))

ALTER TABLE [dbo].[Purchase_Orders] CHECK CONSTRAINT [CK Purchase Orders Approved Date]

/****** Object:  Check [CK Purchase Orders Creation Date]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Purchase_Orders]  WITH CHECK ADD  CONSTRAINT [CK Purchase Orders Creation Date] CHECK  (([Creation Date]>='1/1/1900'))

ALTER TABLE [dbo].[Purchase_Orders] CHECK CONSTRAINT [CK Purchase Orders Creation Date]

/****** Object:  Check [CK Purchase Orders Expected Date]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Purchase_Orders]  WITH CHECK ADD  CONSTRAINT [CK Purchase Orders Expected Date] CHECK  (([Expected Date]>='1/1/1900'))

ALTER TABLE [dbo].[Purchase_Orders] CHECK CONSTRAINT [CK Purchase Orders Expected Date]

/****** Object:  Check [CK Purchase Orders Payment Date]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Purchase_Orders]  WITH CHECK ADD  CONSTRAINT [CK Purchase Orders Payment Date] CHECK  (([Payment Date]>='1/1/1900'))

ALTER TABLE [dbo].[Purchase_Orders] CHECK CONSTRAINT [CK Purchase Orders Payment Date]

/****** Object:  Check [CK Purchase Orders Submitted Date]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Purchase_Orders]  WITH CHECK ADD  CONSTRAINT [CK Purchase Orders Submitted Date] CHECK  (([Submitted Date]>='1/1/1900'))

ALTER TABLE [dbo].[Purchase_Orders] CHECK CONSTRAINT [CK Purchase Orders Submitted Date]

/****** Object:  Check [CK Orders Order Date]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [CK Orders Order Date] CHECK  (([Order Date]>='1/1/1900'))

ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [CK Orders Order Date]

/****** Object:  Check [CK Orders Paid Date]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [CK Orders Paid Date] CHECK  (([Paid Date]>='1/1/1900'))

ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [CK Orders Paid Date]

/****** Object:  Check [CK Orders Shipped Date]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [CK Orders Shipped Date] CHECK  (([Shipped Date]>='1/1/1900'))

ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [CK Orders Shipped Date]

/****** Object:  Check [CK Order Details Date Allocated]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Order_Details]  WITH CHECK ADD  CONSTRAINT [CK Order Details Date Allocated] CHECK  (([Date Allocated]>='1/1/1900'))

ALTER TABLE [dbo].[Order_Details] CHECK CONSTRAINT [CK Order Details Date Allocated]

/****** Object:  Check [CK Order Details Discount]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Order_Details]  WITH CHECK ADD  CONSTRAINT [CK Order Details Discount] CHECK  (([Discount]<=(1) AND [Discount]>=(0)))

ALTER TABLE [dbo].[Order_Details] CHECK CONSTRAINT [CK Order Details Discount]

/****** Object:  Check [CK Invoices Due Date]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Invoices]  WITH CHECK ADD  CONSTRAINT [CK Invoices Due Date] CHECK  (([Due Date]>='1/1/1900'))

ALTER TABLE [dbo].[Invoices] CHECK CONSTRAINT [CK Invoices Due Date]

/****** Object:  Check [CK Invoices Invoice Date]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Invoices]  WITH CHECK ADD  CONSTRAINT [CK Invoices Invoice Date] CHECK  (([Invoice Date]>='1/1/1900'))

ALTER TABLE [dbo].[Invoices] CHECK CONSTRAINT [CK Invoices Invoice Date]

/****** Object:  Check [CK Inventory Transactions Transaction Created Date]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Inventory_Transactions]  WITH CHECK ADD  CONSTRAINT [CK Inventory Transactions Transaction Created Date] CHECK  (([Transaction Created Date]>='1/1/1900'))

ALTER TABLE [dbo].[Inventory_Transactions] CHECK CONSTRAINT [CK Inventory Transactions Transaction Created Date]

/****** Object:  Check [CK Inventory Transactions Transaction Modified Date]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Inventory_Transactions]  WITH CHECK ADD  CONSTRAINT [CK Inventory Transactions Transaction Modified Date] CHECK  (([Transaction Modified Date]>='1/1/1900'))

ALTER TABLE [dbo].[Inventory_Transactions] CHECK CONSTRAINT [CK Inventory Transactions Transaction Modified Date]

/****** Object:  Check [CK Purchase Order Details Date Received]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Purchase_Order_Details]  WITH CHECK ADD  CONSTRAINT [CK Purchase Order Details Date Received] CHECK  (([Date Received]>='1/1/1900'))

ALTER TABLE [dbo].[Purchase_Order_Details] CHECK CONSTRAINT [CK Purchase Order Details Date Received]

/****** Object:  ForeignKey [FK_Purchase_Orders_Purchase_OrdersPayment Method_Payment MethodId]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Purchase_Orders]  WITH CHECK ADD  CONSTRAINT [FK_Purchase_Orders_Purchase_OrdersPayment Method_Payment MethodId] FOREIGN KEY([Payment MethodId])
REFERENCES [dbo].[Purchase_OrdersPayment Method] ([Id])

ALTER TABLE [dbo].[Purchase_Orders] CHECK CONSTRAINT [FK_Purchase_Orders_Purchase_OrdersPayment Method_Payment MethodId]

/****** Object:  ForeignKey [Purchase Orders_FK00]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Purchase_Orders]  WITH CHECK ADD  CONSTRAINT [Purchase Orders_FK00] FOREIGN KEY([Created By])
REFERENCES [dbo].[Employees] ([ID])

ALTER TABLE [dbo].[Purchase_Orders] CHECK CONSTRAINT [Purchase Orders_FK00]

/****** Object:  ForeignKey [Purchase Orders_FK01]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Purchase_Orders]  WITH CHECK ADD  CONSTRAINT [Purchase Orders_FK01] FOREIGN KEY([Status ID])
REFERENCES [dbo].[Purchase_Order_Status] ([Status ID])

ALTER TABLE [dbo].[Purchase_Orders] CHECK CONSTRAINT [Purchase Orders_FK01]

/****** Object:  ForeignKey [Purchase Orders_FK02]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Purchase_Orders]  WITH CHECK ADD  CONSTRAINT [Purchase Orders_FK02] FOREIGN KEY([Supplier ID])
REFERENCES [dbo].[Suppliers] ([ID])

ALTER TABLE [dbo].[Purchase_Orders] CHECK CONSTRAINT [Purchase Orders_FK02]

/****** Object:  ForeignKey [FK_durados_Link_durados_Folder]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[durados_Link]  WITH CHECK ADD  CONSTRAINT [FK_durados_Link_durados_Folder] FOREIGN KEY([FolderId])
REFERENCES [dbo].[durados_Folder] ([Id])

ALTER TABLE [dbo].[durados_Link] CHECK CONSTRAINT [FK_durados_Link_durados_Folder]

/****** Object:  ForeignKey [FK_Customers_CustomersJob Title_Job TitleId]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Customers]  WITH CHECK ADD  CONSTRAINT [FK_Customers_CustomersJob Title_Job TitleId] FOREIGN KEY([Job TitleId])
REFERENCES [dbo].[CustomersJob Title] ([Id])

ALTER TABLE [dbo].[Customers] CHECK CONSTRAINT [FK_Customers_CustomersJob Title_Job TitleId]

/****** Object:  ForeignKey [Employee Privileges_FK00]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Employee_Privileges]  WITH CHECK ADD  CONSTRAINT [Employee Privileges_FK00] FOREIGN KEY([Employee ID])
REFERENCES [dbo].[Employees] ([ID])

ALTER TABLE [dbo].[Employee_Privileges] CHECK CONSTRAINT [Employee Privileges_FK00]

/****** Object:  ForeignKey [Employee Privileges_FK01]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Employee_Privileges]  WITH CHECK ADD  CONSTRAINT [Employee Privileges_FK01] FOREIGN KEY([Privilege ID])
REFERENCES [dbo].[Privileges] ([Privilege ID])

ALTER TABLE [dbo].[Employee_Privileges] CHECK CONSTRAINT [Employee Privileges_FK01]

/****** Object:  ForeignKey [FK_User_durados_UserRole]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[durados_User]  WITH CHECK ADD  CONSTRAINT [FK_User_durados_UserRole] FOREIGN KEY([Role])
REFERENCES [dbo].[durados_UserRole] ([Name])

ALTER TABLE [dbo].[durados_User] CHECK CONSTRAINT [FK_User_durados_UserRole]

/****** Object:  ForeignKey [FK_durados_MessageStatus_durados_MessageBoard]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[durados_MessageStatus]  WITH CHECK ADD  CONSTRAINT [FK_durados_MessageStatus_durados_MessageBoard] FOREIGN KEY([MessageId])
REFERENCES [dbo].[durados_MessageBoard] ([Id])

ALTER TABLE [dbo].[durados_MessageStatus] CHECK CONSTRAINT [FK_durados_MessageStatus_durados_MessageBoard]

/****** Object:  ForeignKey [FK_Orders_OrdersPayment Type_Payment TypeId]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_OrdersPayment Type_Payment TypeId] FOREIGN KEY([Payment TypeId])
REFERENCES [dbo].[OrdersPayment Type] ([Id])

ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_OrdersPayment Type_Payment TypeId]

/****** Object:  ForeignKey [Orders_FK00]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [Orders_FK00] FOREIGN KEY([Customer ID])
REFERENCES [dbo].[Customers] ([ID])

ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [Orders_FK00]

/****** Object:  ForeignKey [Orders_FK01]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [Orders_FK01] FOREIGN KEY([Employee ID])
REFERENCES [dbo].[Employees] ([ID])

ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [Orders_FK01]

/****** Object:  ForeignKey [Orders_FK02]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [Orders_FK02] FOREIGN KEY([Status ID])
REFERENCES [dbo].[Orders_Status] ([Status ID])

ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [Orders_FK02]

/****** Object:  ForeignKey [Orders_FK03]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [Orders_FK03] FOREIGN KEY([Shipper ID])
REFERENCES [dbo].[Shippers] ([ID])

ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [Orders_FK03]

/****** Object:  ForeignKey [Orders_FK04]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [Orders_FK04] FOREIGN KEY([Tax Status])
REFERENCES [dbo].[Orders_Tax_Status] ([ID])

ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [Orders_FK04]

/****** Object:  ForeignKey [Order Details_FK00]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Order_Details]  WITH CHECK ADD  CONSTRAINT [Order Details_FK00] FOREIGN KEY([Order ID])
REFERENCES [dbo].[Orders] ([Order ID])
ON DELETE CASCADE

ALTER TABLE [dbo].[Order_Details] CHECK CONSTRAINT [Order Details_FK00]

/****** Object:  ForeignKey [Order Details_FK01]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Order_Details]  WITH CHECK ADD  CONSTRAINT [Order Details_FK01] FOREIGN KEY([Status ID])
REFERENCES [dbo].[Order_Details_Status] ([Status ID])

ALTER TABLE [dbo].[Order_Details] CHECK CONSTRAINT [Order Details_FK01]

/****** Object:  ForeignKey [Order Details_FK02]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Order_Details]  WITH CHECK ADD  CONSTRAINT [Order Details_FK02] FOREIGN KEY([Product ID])
REFERENCES [dbo].[Products] ([ID])

ALTER TABLE [dbo].[Order_Details] CHECK CONSTRAINT [Order Details_FK02]

/****** Object:  ForeignKey [Invoices_FK00]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Invoices]  WITH CHECK ADD  CONSTRAINT [Invoices_FK00] FOREIGN KEY([Order ID])
REFERENCES [dbo].[Orders] ([Order ID])
ON DELETE CASCADE

ALTER TABLE [dbo].[Invoices] CHECK CONSTRAINT [Invoices_FK00]

/****** Object:  ForeignKey [Inventory Transactions_FK00]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Inventory_Transactions]  WITH CHECK ADD  CONSTRAINT [Inventory Transactions_FK00] FOREIGN KEY([Customer Order ID])
REFERENCES [dbo].[Orders] ([Order ID])

ALTER TABLE [dbo].[Inventory_Transactions] CHECK CONSTRAINT [Inventory Transactions_FK00]

/****** Object:  ForeignKey [Inventory Transactions_FK01]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Inventory_Transactions]  WITH CHECK ADD  CONSTRAINT [Inventory Transactions_FK01] FOREIGN KEY([Product ID])
REFERENCES [dbo].[Products] ([ID])

ALTER TABLE [dbo].[Inventory_Transactions] CHECK CONSTRAINT [Inventory Transactions_FK01]

/****** Object:  ForeignKey [Inventory Transactions_FK02]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Inventory_Transactions]  WITH CHECK ADD  CONSTRAINT [Inventory Transactions_FK02] FOREIGN KEY([Purchase Order ID])
REFERENCES [dbo].[Purchase_Orders] ([Purchase Order ID])

ALTER TABLE [dbo].[Inventory_Transactions] CHECK CONSTRAINT [Inventory Transactions_FK02]

/****** Object:  ForeignKey [Inventory Transactions_FK03]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Inventory_Transactions]  WITH CHECK ADD  CONSTRAINT [Inventory Transactions_FK03] FOREIGN KEY([Transaction Type])
REFERENCES [dbo].[Inventory_Transaction_Types] ([ID])

ALTER TABLE [dbo].[Inventory_Transactions] CHECK CONSTRAINT [Inventory Transactions_FK03]

/****** Object:  ForeignKey [Purchase Order Details_FK00]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Purchase_Order_Details]  WITH CHECK ADD  CONSTRAINT [Purchase Order Details_FK00] FOREIGN KEY([Inventory ID])
REFERENCES [dbo].[Inventory_Transactions] ([Transaction ID])

ALTER TABLE [dbo].[Purchase_Order_Details] CHECK CONSTRAINT [Purchase Order Details_FK00]

/****** Object:  ForeignKey [Purchase Order Details_FK01]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Purchase_Order_Details]  WITH CHECK ADD  CONSTRAINT [Purchase Order Details_FK01] FOREIGN KEY([Product ID])
REFERENCES [dbo].[Products] ([ID])

ALTER TABLE [dbo].[Purchase_Order_Details] CHECK CONSTRAINT [Purchase Order Details_FK01]

/****** Object:  ForeignKey [Purchase Order Details_FK02]    Script Date: 09/05/2012 17:37:32 ******/
ALTER TABLE [dbo].[Purchase_Order_Details]  WITH CHECK ADD  CONSTRAINT [Purchase Order Details_FK02] FOREIGN KEY([Purchase Order ID])
REFERENCES [dbo].[Purchase_Orders] ([Purchase Order ID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Purchase_Order_Details] CHECK CONSTRAINT [Purchase Order Details_FK02]


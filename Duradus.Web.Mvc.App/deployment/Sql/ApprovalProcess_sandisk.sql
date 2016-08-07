




/*******  Sandisk Specifics  *******/




SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ApprovalProcessTypeProductLine](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductLineId] [int] NOT NULL,
	[ApprovalProcessTypeId] [int] NOT NULL,
 CONSTRAINT [PK_ApprovalProcessTypeProductLine] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ApprovalProcessTypeProductLine]  WITH CHECK ADD  CONSTRAINT [FK_ApprovalProcessTypeProductLine_ApprovalProcessType] FOREIGN KEY([ApprovalProcessTypeId])
REFERENCES [dbo].[durados_ApprovalProcessType] ([Id])
GO

ALTER TABLE [dbo].[ApprovalProcessTypeProductLine] CHECK CONSTRAINT [FK_ApprovalProcessTypeProductLine_ApprovalProcessType]
GO



SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ApprovalProcessTypeProductLineDefaultUser](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ApprovalProcessTypeProductLineId] [int] NOT NULL,
	[DefaultUserId] [int] NOT NULL,
 CONSTRAINT [PK_ApprovalProcessTypeDefaultUser] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ApprovalProcessTypeProductLineDefaultUser]  WITH CHECK ADD  CONSTRAINT [FK_ApprovalProcessTypeDefaultUser_ApprovalProcessType] FOREIGN KEY([ApprovalProcessTypeProductLineId])
REFERENCES [dbo].[ApprovalProcessTypeProductLine] ([Id])
GO

ALTER TABLE [dbo].[ApprovalProcessTypeProductLineDefaultUser] CHECK CONSTRAINT [FK_ApprovalProcessTypeDefaultUser_ApprovalProcessType]
GO

ALTER TABLE [dbo].[ApprovalProcessTypeProductLineDefaultUser]  WITH CHECK ADD  CONSTRAINT [FK_ApprovalProcessTypeDefaultUser_User] FOREIGN KEY([DefaultUserId])
REFERENCES [dbo].[User] ([ID])
GO

ALTER TABLE [dbo].[ApprovalProcessTypeProductLineDefaultUser] CHECK CONSTRAINT [FK_ApprovalProcessTypeDefaultUser_User]
GO


/********************     POR      *********************/


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ApprovalProcessPOR_durados_ApprovalProcess]') AND parent_object_id = OBJECT_ID(N'[dbo].[ApprovalProcessPOR]'))
ALTER TABLE [dbo].[ApprovalProcessPOR] DROP CONSTRAINT [FK_ApprovalProcessPOR_durados_ApprovalProcess]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ApprovalProcessPOR_POR]') AND parent_object_id = OBJECT_ID(N'[dbo].[ApprovalProcessPOR]'))
ALTER TABLE [dbo].[ApprovalProcessPOR] DROP CONSTRAINT [FK_ApprovalProcessPOR_POR]
GO


/****** Object:  Table [dbo].[ApprovalProcessPOR]    Script Date: 12/12/2011 10:54:52 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ApprovalProcessPOR]') AND type in (N'U'))
DROP TABLE [dbo].[ApprovalProcessPOR]
GO


/****** Object:  Table [dbo].[ApprovalProcessPOR]    Script Date: 12/12/2011 10:54:52 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ApprovalProcessPOR](
	[ApprovalProcessId] [int] NOT NULL,
	[PORId] [int] NULL,
 CONSTRAINT [PK_ApprovalProcessPOR] PRIMARY KEY CLUSTERED 
(
	[ApprovalProcessId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ApprovalProcessPOR]  WITH CHECK ADD  CONSTRAINT [FK_ApprovalProcessPOR_durados_ApprovalProcess] FOREIGN KEY([ApprovalProcessId])
REFERENCES [dbo].[durados_ApprovalProcess] ([Id])
GO

ALTER TABLE [dbo].[ApprovalProcessPOR] CHECK CONSTRAINT [FK_ApprovalProcessPOR_durados_ApprovalProcess]
GO

ALTER TABLE [dbo].[ApprovalProcessPOR]  WITH CHECK ADD  CONSTRAINT [FK_ApprovalProcessPOR_POR] FOREIGN KEY([PORId])
REFERENCES [dbo].[POR] ([Id])
GO

ALTER TABLE [dbo].[ApprovalProcessPOR] CHECK CONSTRAINT [FK_ApprovalProcessPOR_POR]
GO


/****** Object:  View [dbo].[v_ApprovalProcessPOR]    Script Date: 12/12/2011 10:56:46 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[v_ApprovalProcessPOR]'))
DROP VIEW [dbo].[v_ApprovalProcessPOR]
GO


/****** Object:  View [dbo].[v_ApprovalProcessPOR]    Script Date: 12/12/2011 10:56:47 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[v_ApprovalProcessPOR]
AS
SELECT dbo.durados_ApprovalProcess.Id, dbo.durados_ApprovalProcess.ApprovalStatusId, dbo.durados_ApprovalProcess.ApprovalProcessTypeId, 
               dbo.durados_ApprovalProcess.Active, dbo.ApprovalProcessPOR.PORId, dbo.durados_ApprovalProcess.ParentView
FROM  dbo.ApprovalProcessPOR INNER JOIN
               dbo.durados_ApprovalProcess ON dbo.ApprovalProcessPOR.ApprovalProcessId = dbo.durados_ApprovalProcess.Id

GO



CREATE PROCEDURE [dbo].[durados_CreateApprovalProcessPOR]
	-- Add the parameters for the stored procedure here
	@ApprovalProcessTypeId	INT,
	@pk						INT,
	@ParentView				nvarchar(250),
	@ID						INT output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here

	INSERT INTO [dbo].[durados_ApprovalProcess]
			   ([ApprovalStatusId]
			   ,[ApprovalProcessTypeId]
			   ,[Active]
			   ,[ParentView])
		 VALUES
			   (1
			   ,@ApprovalProcessTypeId
			   ,1
			   ,@ParentView)


	SET @ID = SCOPE_IDENTITY()

	
	update [dbo].[v_ApprovalProcessPOR]
	set [Active] = 0
	where PORId = @pk
	
		INSERT INTO [dbo].[ApprovalProcessPOR]
			   ([ApprovalProcessId]
			   ,[PORId])
		 VALUES
			   (@ID
			   ,@pk)

	INSERT INTO [dbo].[durados_ApprovalProcessUser]
			   ([ApprovalProcessId]
			   ,[UserId]
			   ,[ApprovalStatusId]
			   ,[SignedDate]
			   ,[CreatedDate])
		SELECT @ID, ApprovalProcessTypeProductLineDefaultUser.DefaultUserId, 1, NULL, GETDATE()
FROM     ApprovalProcessTypeProductLineDefaultUser WITH (NOLOCK) INNER JOIN
                  ApprovalProcessTypeProductLine WITH (NOLOCK) ON ApprovalProcessTypeProductLineDefaultUser.ApprovalProcessTypeProductLineId = ApprovalProcessTypeProductLine.Id
WHERE  (ApprovalProcessTypeProductLine.ApprovalProcessTypeId = @ApprovalProcessTypeId) AND (ApprovalProcessTypeProductLine.ProductLineId = (SELECT TOP (1) v_POR.ProductLineId
FROM     v_POR 
WHERE  (v_POR.Id = @pk)))
           

END



/* 

create Approval Process for POR
1. run the following sql:
1.1 ApprovalProcess.sql
1.2 ApprovalProcessPOR.sql

2. Add new views and create relations to the following views:
2.1 durados_ApprovalStatus						(generic)
2.2 durados_ApprovalProcessType					(generic)
2.3 v_ApprovalProcessPOR						(specific)
2.4 durados_ApprovalProcessUser					(generic)
2.5 ApprovalProcessTypeProductLine				(specific)
2.6 ApprovalProcessTypeProductLineDefaultUser	(specific)

3. Enter Data
3.1 Configure View to open in the setting menu
3.2 Enter data into 
3.2.1 durados_ApprovalProcessType
3.2.2 ApprovalProcessTypeProductLine
3.2.3 ApprovalProcessTypeProductLineDefaultUser

4. Example for Workflow rule configuration:
4.1 view: v_POR
4.2 Name: approval 2 Exit
4.3	Data Action: AfterCompleteStepBeforeCommit
4.4 Workflow Action: Approval
4.5 Where condition: PhaseId=8
4.6 Use Sql Parser: True
4.7 Parameters:
4.7.1 ApprovalProcessViewName: v_ApprovalProcessPOR
4.7.2 ApprovalProcessType: 1
4.7.3 StoredProcedureName: durados_CreateApprovalProcessPOR
4.7.4 ApprovalProcessMessageKey: ApprovalProcessMessage
4.7.5 Add to Durados Content (durados_Html) an appropriate message with dictionary tokens
4.7.6 ApprovalProcessSubjectKey: 2 Exit Approval

5. additional configuration
5.1 Show in grid the Approval Process children of POR
5.2 Show in grid the Users children of Approval Process
5.3 add to v_POR_WithoutStatus the Approval Process Status by add the following to the select statement: 
 (SELECT TOP (1) ApprovalStatusId
                    FROM   dbo.v_ApprovalProcessPOR AS ap WITH (NOLOCK)
                    WHERE (Active <> 0) AND (PORId = p.Id)) AS ApprovalStatusId
                    
    add the ApprovalStatusId to the views: v_POR_WithoutStatusDesc and to v_POR
    sync v_POR and relate     ApprovalStatusId to durados_ApprovalStatus
5.4 configure the steps that depend on that field

*/


/*

create Approval Process for POR Required ACT

1. insert into durados_ApprovalProcessType (Name, Ordinal) values ('Required ACT', 1000)  // id=8 
2. add rule; name: POR Required ACT, view: v_RequestedDatePORFromPLM, Data Action: AfterCreateBeforeCommit, WorkflowAction: Approval, where: 1=1
2.1 add Parameter; name: ApprovalProcessViewName, value: v_ApprovalProcessPOR
2.2 add Parameter; name: ApprovalProcessType, value: 8
2.3 add Parameter; name: StoredProcedureName, value: durados_CreateApprovalProcessPORRequiredACT
2.4 add Parameter; name: ApprovalProcessMessageKey, value: ApprovalProcessMessage
2.5 add Parameter; name: ApprovalProcessSubjectKey, value: ApprovalProcessSubject
2.6 add Parameter; name: ParentView, value: v_RequestedDatePORFromPLM
3. sync v_ApprovalProcessPOR and set DateChangeId Related Table to v_RequestedDatePORFromPLM
4. sync durados_ApprovalProcessUser to get the new Message field
5. sync v_RequestedDatePORFromPLM to get the new ApprovalStatusId field and relate it to durados_ApprovalStatus


*/
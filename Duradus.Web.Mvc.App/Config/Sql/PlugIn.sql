USE [__DB_NAME__]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_durados_PlugIn_durados_User]') AND parent_object_id = OBJECT_ID(N'[dbo].[durados_PlugIn]'))
ALTER TABLE [dbo].[durados_PlugIn] DROP CONSTRAINT [FK_durados_PlugIn_durados_User]
GO

/****** Object:  Table [dbo].[durados_PlugIn]    Script Date: 02/03/2013 06:57:48 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[durados_PlugIn]') AND type in (N'U'))
DROP TABLE [dbo].[durados_PlugIn]
GO


/****** Object:  Table [dbo].[durados_PlugIn]    Script Date: 02/03/2013 06:57:48 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[durados_PlugIn](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Ordinal] [int] NULL,
	[DefaultUserId] [int] NULL,
 CONSTRAINT [PK_durados_PlugIn] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[durados_PlugIn]  WITH CHECK ADD  CONSTRAINT [FK_durados_PlugIn_durados_User] FOREIGN KEY([DefaultUserId])
REFERENCES [dbo].[durados_User] ([ID])
GO

ALTER TABLE [dbo].[durados_PlugIn] CHECK CONSTRAINT [FK_durados_PlugIn_durados_User]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_durados_SampleApp_durados_App]') AND parent_object_id = OBJECT_ID(N'[dbo].[durados_SampleApp]'))
ALTER TABLE [dbo].[durados_SampleApp] DROP CONSTRAINT [FK_durados_SampleApp_durados_App]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_durados_SampleApp_durados_PlugIn]') AND parent_object_id = OBJECT_ID(N'[dbo].[durados_SampleApp]'))
ALTER TABLE [dbo].[durados_SampleApp] DROP CONSTRAINT [FK_durados_SampleApp_durados_PlugIn]
GO


/****** Object:  Table [dbo].[durados_SampleApp]    Script Date: 02/03/2013 06:58:17 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[durados_SampleApp]') AND type in (N'U'))
DROP TABLE [dbo].[durados_SampleApp]
GO

/****** Object:  Table [dbo].[durados_SampleApp]    Script Date: 02/03/2013 06:58:17 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[durados_SampleApp](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PlugInId] [int] NULL,
	[AppId] [int] NULL,
	[ViewName] [nvarchar](500) NULL,
	[Name] [nvarchar](50) NULL,
	[Description] [nvarchar](max) NULL,
	[Ordinal] [int] NULL,
	[AppNamePrefix] [nvarchar](50) NULL,
	[GenerationScript] [nvarchar](250) NULL,
	[DatabaseName] [nvarchar](50) NULL,
	[DbCount] [int] NULL,
 CONSTRAINT [PK_durados_SampleApp] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[durados_SampleApp]  WITH CHECK ADD  CONSTRAINT [FK_durados_SampleApp_durados_App] FOREIGN KEY([AppId])
REFERENCES [dbo].[durados_App] ([Id])
GO

ALTER TABLE [dbo].[durados_SampleApp] CHECK CONSTRAINT [FK_durados_SampleApp_durados_App]
GO

ALTER TABLE [dbo].[durados_SampleApp]  WITH CHECK ADD  CONSTRAINT [FK_durados_SampleApp_durados_PlugIn] FOREIGN KEY([PlugInId])
REFERENCES [dbo].[durados_PlugIn] ([Id])
GO

ALTER TABLE [dbo].[durados_SampleApp] CHECK CONSTRAINT [FK_durados_SampleApp_durados_PlugIn]
GO


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_durados_PlugInInstance_durados_App]') AND parent_object_id = OBJECT_ID(N'[dbo].[durados_PlugInInstance]'))
ALTER TABLE [dbo].[durados_PlugInInstance] DROP CONSTRAINT [FK_durados_PlugInInstance_durados_App]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_durados_PlugInInstance_durados_PlugIn]') AND parent_object_id = OBJECT_ID(N'[dbo].[durados_PlugInInstance]'))
ALTER TABLE [dbo].[durados_PlugInInstance] DROP CONSTRAINT [FK_durados_PlugInInstance_durados_PlugIn]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_durados_PlugInInstance_durados_SampleApp]') AND parent_object_id = OBJECT_ID(N'[dbo].[durados_PlugInInstance]'))
ALTER TABLE [dbo].[durados_PlugInInstance] DROP CONSTRAINT [FK_durados_PlugInInstance_durados_SampleApp]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_durados_PlugInInstance_durados_User]') AND parent_object_id = OBJECT_ID(N'[dbo].[durados_PlugInInstance]'))
ALTER TABLE [dbo].[durados_PlugInInstance] DROP CONSTRAINT [FK_durados_PlugInInstance_durados_User]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_durados_PlugInInstance_Selected]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[durados_PlugInInstance] DROP CONSTRAINT [DF_durados_PlugInInstance_Selected]
END

GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_durados_PlugInUser_durados_PlugIn]') AND parent_object_id = OBJECT_ID(N'[dbo].[durados_PlugInUser]'))
ALTER TABLE [dbo].[durados_PlugInUser] DROP CONSTRAINT [FK_durados_PlugInUser_durados_PlugIn]
GO

/****** Object:  Table [dbo].[durados_PlugInUser]    Script Date: 04/03/2013 08:26:57 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[durados_PlugInUser]') AND type in (N'U'))
DROP TABLE [dbo].[durados_PlugInUser]
GO

/****** Object:  Table [dbo].[durados_PlugInUser]    Script Date: 04/03/2013 08:26:57 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[durados_PlugInUser](
	[PlugInUserId] [nvarchar](50) NOT NULL,
	[PlugInId] [int] NULL,
	[FirstName] [nvarchar](50) NULL,
	[LastName] [nvarchar](50) NULL,
	[Email] [nvarchar](50) NULL,
 CONSTRAINT [PK_durados_PlugInUser2] PRIMARY KEY CLUSTERED 
(
	[PlugInUserId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[durados_PlugInUser]  WITH CHECK ADD  CONSTRAINT [FK_durados_PlugInUser_durados_PlugIn] FOREIGN KEY([PlugInId])
REFERENCES [dbo].[durados_PlugIn] ([Id])
GO

ALTER TABLE [dbo].[durados_PlugInUser] CHECK CONSTRAINT [FK_durados_PlugInUser_durados_PlugIn]
GO

/****** Object:  Table [dbo].[durados_PlugInNotRegisteredUser]    Script Date: 04/03/2013 08:28:13 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[durados_PlugInNotRegisteredUser]') AND type in (N'U'))
DROP TABLE [dbo].[durados_PlugInNotRegisteredUser]
GO

/****** Object:  Table [dbo].[durados_PlugInNotRegisteredUser]    Script Date: 04/03/2013 08:28:13 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[durados_PlugInNotRegisteredUser](
	[Id] [int] NULL,
	[PlugInUserId] [nvarchar](50) NOT NULL,
	[PlugInId] [int] NULL,
	[FirstName] [nvarchar](50) NULL,
	[LastName] [nvarchar](50) NULL,
	[Email] [nvarchar](50) NULL,
	[NotRegisteredUserId] [int] NULL,
	[CreateDate] [datetime] NULL
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[durados_PlugInNotRegisteredUser]  WITH CHECK ADD  CONSTRAINT [FK_durados_PlugInNotRegisteredUser_durados_PlugIn] FOREIGN KEY([PlugInId])
REFERENCES [dbo].[durados_PlugIn] ([Id])
GO

ALTER TABLE [dbo].[durados_PlugInNotRegisteredUser] CHECK CONSTRAINT [FK_durados_PlugInNotRegisteredUser_durados_PlugIn]
GO

ALTER TABLE [dbo].[durados_PlugInNotRegisteredUser]  WITH CHECK ADD  CONSTRAINT [FK_durados_PlugInNotRegisteredUser_durados_User] FOREIGN KEY([NotRegisteredUserId])
REFERENCES [dbo].[durados_User] ([ID])
GO

ALTER TABLE [dbo].[durados_PlugInNotRegisteredUser] CHECK CONSTRAINT [FK_durados_PlugInNotRegisteredUser_durados_User]
GO

ALTER TABLE [dbo].[durados_PlugInNotRegisteredUser] ADD  CONSTRAINT [DF_durados_PlugInNotRegisteredUser_CreateDate]  DEFAULT (getdate()) FOR [CreateDate]
GO


/****** Object:  Table [dbo].[durados_PlugInInstance]    Script Date: 02/03/2013 06:58:48 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[durados_PlugInInstance]') AND type in (N'U'))
DROP TABLE [dbo].[durados_PlugInInstance]
GO


/****** Object:  Table [dbo].[durados_PlugInInstance]    Script Date: 02/03/2013 06:58:48 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[durados_PlugInInstance](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InstanceId] [nvarchar](250) NOT NULL,
	[SampleAppId] [int] NULL,
	[Selected] [bit] NOT NULL,
	[AppId] [int] NULL,
	[ViewName] [nvarchar](500) NULL,
	[PlugInId] [int] NULL,
	[UserId] [int] NULL,
	[PlugInUserId] [nvarchar](250) NULL,
	[SelectionDate] [datetime] NULL,
 CONSTRAINT [PK_durados_PlugInInstance] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[durados_PlugInInstance]  WITH CHECK ADD  CONSTRAINT [FK_durados_PlugInInstance_durados_App] FOREIGN KEY([AppId])
REFERENCES [dbo].[durados_App] ([Id])
GO

ALTER TABLE [dbo].[durados_PlugInInstance] CHECK CONSTRAINT [FK_durados_PlugInInstance_durados_App]
GO

ALTER TABLE [dbo].[durados_PlugInInstance]  WITH CHECK ADD  CONSTRAINT [FK_durados_PlugInInstance_durados_PlugIn] FOREIGN KEY([PlugInId])
REFERENCES [dbo].[durados_PlugIn] ([Id])
GO

ALTER TABLE [dbo].[durados_PlugInInstance] CHECK CONSTRAINT [FK_durados_PlugInInstance_durados_PlugIn]
GO

ALTER TABLE [dbo].[durados_PlugInInstance]  WITH CHECK ADD  CONSTRAINT [FK_durados_PlugInInstance_durados_PlugInUser] FOREIGN KEY([PlugInUserId])
REFERENCES [dbo].[durados_PlugInUser] ([PlugInUserId])
GO

ALTER TABLE [dbo].[durados_PlugInInstance] CHECK CONSTRAINT [FK_durados_PlugInInstance_durados_PlugInUser]
GO

ALTER TABLE [dbo].[durados_PlugInInstance]  WITH CHECK ADD  CONSTRAINT [FK_durados_PlugInInstance_durados_SampleApp] FOREIGN KEY([SampleAppId])
REFERENCES [dbo].[durados_SampleApp] ([Id])
GO

ALTER TABLE [dbo].[durados_PlugInInstance] CHECK CONSTRAINT [FK_durados_PlugInInstance_durados_SampleApp]
GO

ALTER TABLE [dbo].[durados_PlugInInstance]  WITH CHECK ADD  CONSTRAINT [FK_durados_PlugInInstance_durados_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[durados_User] ([ID])
GO

ALTER TABLE [dbo].[durados_PlugInInstance] CHECK CONSTRAINT [FK_durados_PlugInInstance_durados_User]
GO

ALTER TABLE [dbo].[durados_PlugInInstance] ADD  CONSTRAINT [DF_durados_PlugInInstance_Selected]  DEFAULT ((0)) FOR [Selected]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_durados_PlugInNotRegisteredUser_durados_PlugIn]') AND parent_object_id = OBJECT_ID(N'[dbo].[durados_PlugInNotRegisteredUser]'))
ALTER TABLE [dbo].[durados_PlugInNotRegisteredUser] DROP CONSTRAINT [FK_durados_PlugInNotRegisteredUser_durados_PlugIn]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_durados_PlugInNotRegisteredUser_durados_User]') AND parent_object_id = OBJECT_ID(N'[dbo].[durados_PlugInNotRegisteredUser]'))
ALTER TABLE [dbo].[durados_PlugInNotRegisteredUser] DROP CONSTRAINT [FK_durados_PlugInNotRegisteredUser_durados_User]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_durados_PlugInNotRegisteredUser_CreateDate]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[durados_PlugInNotRegisteredUser] DROP CONSTRAINT [DF_durados_PlugInNotRegisteredUser_CreateDate]
END

GO




/****** Object:  StoredProcedure [dbo].[durados_GelAvailableApp]    Script Date: 02/03/2013 07:00:11 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[durados_GelAvailableApp]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[durados_GelAvailableApp]
GO


/****** Object:  StoredProcedure [dbo].[durados_GelAvailableApp]    Script Date: 02/03/2013 07:00:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[durados_GelAvailableApp]
	-- Add the parameters for the stored procedure here
	@TemplateId int, 
	@UserId int, 
	@AppId int output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @MyTableVar table (
    Id int NOT NULL);
    -- Insert statements for procedure here
	update dbo.durados_App
	set Creator = @UserId
		OUTPUT inserted.Id
			INTO @MyTableVar
    where id = (select top(1) id from durados_App where Creator is null and TemplateId = @TemplateId order by Id asc)
    
    select @AppId = Id from @MyTableVar
    
END

GO

/****** Object:  StoredProcedure [dbo].[durados_SaveSelectedInstance]    Script Date: 02/03/2013 07:00:57 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[durados_SaveSelectedInstance]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[durados_SaveSelectedInstance]
GO

/****** Object:  StoredProcedure [dbo].[durados_SaveSelectedInstance]    Script Date: 02/03/2013 07:00:57 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[durados_SaveSelectedInstance]  --[durados_SaveSelectedInstance] '12f1630c-ca58-451b-22f6-406ca55df572TPWdgt-36ee93f6-d989-ff0a-eb9d-2311b36156b2',510,'List510',1,2,161,'B0467BDB-31D3-4B30-87C6-AEA13D928553'
	-- Add the parameters for the stored procedure here
	@instanceId nvarchar(250), 
	@appId int, 
	@viewName nvarchar(500),
	@sampleAppId int = null, 
	@plugInId int, 
	@userId int,
	@PlugInUserId nvarchar(250)
AS

BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	begin tran a
	
	declare @now datetime
	set @now = getdate()
	
	update durados_PlugInInstance
	set selected = 0
	where instanceId = @instanceId
	
    -- Insert statements for procedure here
    
    if not exists(select * from durados_PlugInUser where PlugInUserId = @PlugInUserId)
	begin
		insert into durados_PlugInUser (PlugInUserId, PlugInId) values (@PlugInUserId, @PlugInId)
	end
    
	

	--if exists(select * from durados_PlugInInstance where instanceId = @instanceId and (sampleAppId = @sampleAppId or (@sampleAppId is null and  sampleAppId is null)))
	--begin
	--select 1
	--	update durados_PlugInInstance
	--	set appId = @appId,
	--	viewName = @viewName,
	--	selected = 1,
	--	SelectionDate = @now
	--	where instanceId = @instanceId and (sampleAppId = @sampleAppId or (@sampleAppId is null and  sampleAppId is null))
	--end
	--else
	--begin
	--select 2
	--	insert into durados_PlugInInstance (InstanceId, SampleAppId, Selected, AppId, ViewName, PlugInId, UserId, PlugInUserId, SelectionDate) values (@instanceId, @sampleAppId, 1, @appId, @viewName, @plugInId, @userId, @PlugInUserId, @now)
	--end
	insert into durados_PlugInInstance (InstanceId, SampleAppId, Selected, AppId, ViewName, PlugInId, UserId, PlugInUserId, SelectionDate) values (@instanceId, @sampleAppId, 1, @appId, @viewName, @plugInId, @userId, @PlugInUserId, @now)
	select * from durados_PlugInInstance where instanceId = @instanceId and (sampleAppId = @sampleAppId or (@sampleAppId is null and  sampleAppId is null))
	
	commit tran a
END

/****** Object:  Table [dbo].[durados_Plan]    Script Date: 04/03/2013 09:05:45 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[durados_Plan]') AND type in (N'U'))
DROP TABLE [dbo].[durados_Plan]
GO

/****** Object:  Table [dbo].[durados_Plan]    Script Date: 04/03/2013 09:06:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[durados_Plan](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Ordinal] [int] NULL,
 CONSTRAINT [PK_durados_Plan] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
INSERT [dbo].[durados_Plan] ([Id], [Name], [Ordinal]) VALUES (1, N'Enterprise', 1)
INSERT [dbo].[durados_Plan] ([Id], [Name], [Ordinal]) VALUES (2, N'Wix Premium', 2)
INSERT [dbo].[durados_Plan] ([Id], [Name], [Ordinal]) VALUES (3, N'Wix Free', 3)

GO

INSERT INTO [dbo].[durados_PlugIn]([Name],[Ordinal]) VALUES ('ModuBiz',1)
INSERT INTO [dbo].[durados_PlugIn]([Name],[Ordinal]) VALUES ('WIX',2)
GO


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_durados_AppPlan_durados_App]') AND parent_object_id = OBJECT_ID(N'[dbo].[durados_AppPlan]'))
ALTER TABLE [dbo].[durados_AppPlan] DROP CONSTRAINT [FK_durados_AppPlan_durados_App]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_durados_AppPlan_durados_Plan]') AND parent_object_id = OBJECT_ID(N'[dbo].[durados_AppPlan]'))
ALTER TABLE [dbo].[durados_AppPlan] DROP CONSTRAINT [FK_durados_AppPlan_durados_Plan]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_durados_AppPlan_PurchaseDate]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[durados_AppPlan] DROP CONSTRAINT [DF_durados_AppPlan_PurchaseDate]
END

GO

/****** Object:  Table [dbo].[durados_AppPlan]    Script Date: 04/04/2013 06:59:12 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[durados_AppPlan]') AND type in (N'U'))
DROP TABLE [dbo].[durados_AppPlan]
GO


CREATE TABLE [dbo].[durados_AppPlan](
      [Id] [int] IDENTITY(1,1) NOT NULL,
      [AppId] [int] NULL,
      [PlanId] [int] NULL,
      [PurchaseDate] [datetime] NULL,
CONSTRAINT [PK_durados_AppPlan] PRIMARY KEY CLUSTERED 
(
      [Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[durados_AppPlan]  WITH CHECK ADD  CONSTRAINT [FK_durados_AppPlan_durados_App] FOREIGN KEY([AppId])
REFERENCES [dbo].[durados_App] ([Id])
GO

ALTER TABLE [dbo].[durados_AppPlan] CHECK CONSTRAINT [FK_durados_AppPlan_durados_App]
GO

ALTER TABLE [dbo].[durados_AppPlan]  WITH CHECK ADD  CONSTRAINT [FK_durados_AppPlan_durados_Plan] FOREIGN KEY([PlanId])
REFERENCES [dbo].[durados_Plan] ([Id])
GO

ALTER TABLE [dbo].[durados_AppPlan] CHECK CONSTRAINT [FK_durados_AppPlan_durados_Plan]
GO

ALTER TABLE [dbo].[durados_AppPlan] ADD  CONSTRAINT [DF_durados_AppPlan_PurchaseDate]  DEFAULT (getdate()) FOR [PurchaseDate]
GO

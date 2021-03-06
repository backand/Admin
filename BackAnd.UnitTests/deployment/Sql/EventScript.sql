/****** Object:  Table [dbo].[website_EventType]    Script Date: 10/17/2013 06:17:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[website_EventType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
	[Ordinal] [int] NULL,
	[Action] [int] NULL,
	[Active] [bit] NULL,
 CONSTRAINT [PK_website_EventType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[website_EventType] ON
INSERT [dbo].[website_EventType] ([Id], [Name], [Ordinal], [Action], [Active]) VALUES (1, N'SiteMainMsg', NULL, 2, 1)
SET IDENTITY_INSERT [dbo].[website_EventType] OFF
/****** Object:  Table [dbo].[website_EventTypeOption]    Script Date: 10/17/2013 06:17:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[website_EventTypeOption](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EventTypeId] [int] NULL,
	[Content] [nvarchar](max) NULL,
	[Index] [nvarchar](100) NULL,
 CONSTRAINT [PK_website_EventTypeOption] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[website_EventTypeOption] ON
INSERT [dbo].[website_EventTypeOption] ([Id], [EventTypeId], [Content], [Index]) VALUES (1, 1, N'First Msg<br> Second Line<br>', N'Wix')
INSERT [dbo].[website_EventTypeOption] ([Id], [EventTypeId], [Content], [Index]) VALUES (5, 1, N'Next Msg<br>OK<br>', N'Else')
INSERT [dbo].[website_EventTypeOption] ([Id], [EventTypeId], [Content], [Index]) VALUES (6, 1, N'<h1 style="margin-bottom:10px;">The Online<br/> Back-end Generator</h1><h3>Connect your database and <br/> automatically generate a fully <br/>customized back-end interface</h3>', N'Default')
SET IDENTITY_INSERT [dbo].[website_EventTypeOption] OFF
/****** Object:  Table [dbo].[website_users_event]    Script Date: 10/17/2013 06:17:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[website_users_event](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EventTypeOptionId] [int] NULL,
	[website_CookiId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_website_users_event] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


/****** Object:  StoredProcedure [dbo].[website_GetAndSaveUserMainMsg]    Script Date: 10/17/2013 06:17:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================

--exec [website_GetAndSaveUserMainMsg] '6be0fd15-1cbf-48ba-8058-ed02fd78c6e3',''
CREATE PROCEDURE  [dbo].[website_GetAndSaveUserMainMsg]
	-- Add the parameters for the stored procedure here
	@cookieGuid uniqueidentifier,
	@eventTypeOption nvarchar(50),
	@EventTypeId int=1
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE  @content NVARCHAR(MAX) =''
	DECLARE  @eventTypeOptionId int
	
    SELECT @content=content,@eventTypeOptionId=Id FROM  dbo.website_EventTypeOption WITH(NOLOCK) WHERE EventTypeId=@EventTypeId AND [Index]=@eventTypeOption
    
    IF @content IS NULL OR @content ='' OR @eventTypeOption =''
    BEGIN 
    	SELECT @content=content,@eventTypeOptionId=Id FROM  dbo.website_EventTypeOption WITH(NOLOCK) WHERE EventTypeId=@EventTypeId AND [Index]='Default'
	END
    
    BEGIN TRY 
     -- Set @content=''
		INSERT INTO dbo.website_users_event(website_CookiId,EventTypeOptionId) VALUES (@cookieGuid,@eventTypeOptionId)
    END TRY
    BEGIN CATCH
		DECLARE @time DATETIME =GETDATE()
		DECLARE @ErrorMsg nvarchar(400) =ERROR_MESSAGE()
		
		Exec Durados_LogInsert 'Modubiz Site','dev@devitout.com',@@SERVERNAME,@time,'WebsiteAccount','UpdateDynamicContent','GetAndSaveSiteMainMsg',5,@ErrorMsg
		
    END CATCH
    SELECT @content
	
END
GO
/****** Object:  ForeignKey [FK_website_EventTypeOption_website_EventType]    Script Date: 10/17/2013 06:17:08 ******/
ALTER TABLE [dbo].[website_EventTypeOption]  WITH CHECK ADD  CONSTRAINT [FK_website_EventTypeOption_website_EventType] FOREIGN KEY([EventTypeId])
REFERENCES [dbo].[website_EventType] ([Id])
GO
ALTER TABLE [dbo].[website_EventTypeOption] CHECK CONSTRAINT [FK_website_EventTypeOption_website_EventType]
GO
/****** Object:  ForeignKey [FK_website_users_event_website_EventTypeOption]    Script Date: 10/17/2013 06:17:08 ******/
ALTER TABLE [dbo].[website_users_event]  WITH CHECK ADD  CONSTRAINT [FK_website_users_event_website_EventTypeOption] FOREIGN KEY([EventTypeOptionId])
REFERENCES [dbo].[website_EventTypeOption] ([Id])
GO
ALTER TABLE [dbo].[website_users_event] CHECK CONSTRAINT [FK_website_users_event_website_EventTypeOption]
GO
/****** Object:  ForeignKey [FK_website_users_event_website_TrackingCookie]    Script Date: 10/17/2013 06:17:08 ******/
ALTER TABLE [dbo].[website_users_event]  WITH CHECK ADD  CONSTRAINT [FK_website_users_event_website_TrackingCookie] FOREIGN KEY([website_CookiId])
REFERENCES [dbo].[website_TrackingCookie] ([Guid])
GO
ALTER TABLE [dbo].[website_users_event] CHECK CONSTRAINT [FK_website_users_event_website_TrackingCookie]
GO

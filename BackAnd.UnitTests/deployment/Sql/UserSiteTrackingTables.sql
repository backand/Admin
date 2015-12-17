/****** Object:  Table [dbo].[website_TrackingCookie]    Script Date: 09/03/2013 11:42:25 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_website_TrackingCookie_Guid]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[website_TrackingCookie] DROP CONSTRAINT [DF_website_TrackingCookie_Guid]
END
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_website_TrackingCookie_CreateDate]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[website_TrackingCookie] DROP CONSTRAINT [DF_website_TrackingCookie_CreateDate]
END
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[website_TrackingCookie]') AND type in (N'U'))
DROP TABLE [dbo].[website_TrackingCookie]
GO
/****** Object:  Table [dbo].[website_UsersCookie]    Script Date: 09/03/2013 11:42:25 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[website_UsersCookie]') AND type in (N'U'))
DROP TABLE [dbo].[website_UsersCookie]
GO
/****** Object:  Table [dbo].[website_UsersCookie]    Script Date: 09/03/2013 11:42:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[website_UsersCookie]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[website_UsersCookie](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[CookieGuid] [uniqueidentifier] NULL,
	[CreateDate] [datetime] NULL,
 CONSTRAINT [PK_website_UsersCookie] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[website_TrackingCookie]    Script Date: 09/03/2013 11:42:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[website_TrackingCookie]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[website_TrackingCookie](
	[Guid] [uniqueidentifier] NOT NULL CONSTRAINT [DF_website_TrackingCookie_Guid]  DEFAULT (newid()),
	[Referer] [nvarchar](max) NULL,
	[AllHeader] [text] NULL,
	[QueryString] [nvarchar](max) NULL,
	[LinkedGuid] [uniqueidentifier] NULL,
	[ShowDemo] [bit] NULL,
	[CreateDate] [datetime] NULL CONSTRAINT [DF_website_TrackingCookie_CreateDate]  DEFAULT (getdate()),
 CONSTRAINT [PK_website_TrackingCookie] PRIMARY KEY CLUSTERED 
(
	[Guid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO


/****** Object:  Table [dbo].[website_Users]    Script Date: 09/07/2013 18:36:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[website_Users](
	[Id] [int] IDENTITY(-1000,-1) NOT NULL,
	[FullName] [nvarchar](500) NOT NULL,
	[Email] [nvarchar](250) NOT NULL,
	[Comments] [nvarchar](max) NULL,
	[CreateDate] [datetime] NULL,	
	[CookieGuid] [uniqueidentifier] NULL,
 CONSTRAINT [PK_website_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


/****** Object:  Table [dbo].[website_TrackingCookie]    Script Date: 10/03/2013 13:06:12 ******/
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
ALTER TABLE dbo.website_TrackingCookie ADD
	Email nvarchar(150) NULL,
	FullName nvarchar(150) NULL,
	DBType int NULL,
	Phone nvarchar(150) NULL
GO
ALTER TABLE dbo.website_TrackingCookie SET (LOCK_ESCALATION = TABLE)
GO
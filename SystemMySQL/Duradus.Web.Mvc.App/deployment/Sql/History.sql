USE [__DB_NAME__]

/****** Object:  Table [dbo].[durados_Action]    Script Date: 12/01/2011 13:21:28 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[durados_Action]') AND type in (N'U'))
DROP TABLE [dbo].[durados_Action]


/****** Object:  Table [dbo].[durados_Action]    Script Date: 12/01/2011 13:21:31 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[durados_Action](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_durados_Action] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

INSERT INTO [dbo].[durados_Action] ([Id],[Name]) VALUES (1,'Insert')
INSERT INTO [dbo].[durados_Action] ([Id],[Name]) VALUES (2,'Update')
INSERT INTO [dbo].[durados_Action] ([Id],[Name]) VALUES (3,'Delete')


IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_durados_ChangeHistory_UpdateDate]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[durados_ChangeHistory] DROP CONSTRAINT [DF_durados_ChangeHistory_UpdateDate]
END

/****** Object:  Table [dbo].[durados_ChangeHistory]    Script Date: 12/01/2011 13:22:27 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[durados_ChangeHistory]') AND type in (N'U'))
DROP TABLE [dbo].[durados_ChangeHistory]

/****** Object:  Table [dbo].[durados_ChangeHistory]    Script Date: 12/01/2011 13:22:30 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

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
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[durados_ChangeHistory] ADD  CONSTRAINT [DF_durados_ChangeHistory_UpdateDate]  DEFAULT (getdate()) FOR [UpdateDate]

/****** Object:  Table [dbo].[durados_ChangeHistoryField]    Script Date: 12/01/2011 13:23:02 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[durados_ChangeHistoryField]') AND type in (N'U'))
DROP TABLE [dbo].[durados_ChangeHistoryField]

/****** Object:  Table [dbo].[durados_ChangeHistoryField]    Script Date: 12/01/2011 13:23:05 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

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
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/****** Object:  View [dbo].[durados_v_ChangeHistory]    Script Date: 12/01/2011 13:23:40 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[durados_v_ChangeHistory]'))
DROP VIEW [dbo].[durados_v_ChangeHistory]
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




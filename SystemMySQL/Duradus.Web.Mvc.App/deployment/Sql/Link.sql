USE [__DB_NAME__]

/****** Object:  Table [dbo].[durados_Html]    Script Date: 01/17/2011 11:45:31 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[durados_Folder](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
	[UserId] [int] NOT NULL,
	[Ordinal] [int] NOT NULL,
 CONSTRAINT [PK_durados_Folder] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]


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
	[Filter] [nvarchar](MAX) NULL,
	[SortColumn] [nvarchar](150) NULL,
	[SortDirection] [varchar](5) NULL,
	[PageNo] [smallint] NOT NULL,
	[PageSize] [smallint] NOT NULL,
	[CreationDate] [datetime] NOT NULL DEFAULT getdate(),
	[Description] [nvarchar](2000) NULL,
	[FolderId] [int] NULL,
 CONSTRAINT [PK_durados_Link] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]


SET ANSI_PADDING OFF


ALTER TABLE [dbo].[durados_Link]  WITH CHECK ADD  CONSTRAINT [FK_durados_Link_durados_Folder] FOREIGN KEY([FolderId])
REFERENCES [dbo].[durados_Folder] ([Id])

ALTER TABLE [dbo].[durados_Link] CHECK CONSTRAINT [FK_durados_Link_durados_Folder]

ALTER TABLE [dbo].[durados_Link] ADD  DEFAULT ((1)) FOR [Ordinal]

ALTER TABLE [dbo].[durados_Link] ADD  DEFAULT ((1)) FOR [PageNo]

ALTER TABLE [dbo].[durados_Link] ADD  DEFAULT ((20)) FOR [PageSize]
USE [__DB_NAME__]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_durados_DnsAlias_durados_App]') AND parent_object_id = OBJECT_ID(N'[dbo].[durados_DnsAlias]'))
ALTER TABLE [dbo].[durados_DnsAlias] DROP CONSTRAINT [FK_durados_DnsAlias_durados_App]
GO

/****** Object:  Table [dbo].[durados_DnsAlias]    Script Date: 10/21/2012 12:32:48 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[durados_DnsAlias]') AND type in (N'U'))
DROP TABLE [dbo].[durados_DnsAlias]
GO

/****** Object:  Table [dbo].[durados_DnsAlias]    Script Date: 10/21/2012 12:32:48 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[durados_DnsAlias](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Alias] [nvarchar](250) NOT NULL,
	[AppId] [int] NOT NULL,
 CONSTRAINT [PK_durados_DnsAlias] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[durados_DnsAlias]  WITH CHECK ADD  CONSTRAINT [FK_durados_DnsAlias_durados_App] FOREIGN KEY([AppId])
REFERENCES [dbo].[durados_App] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[durados_DnsAlias] CHECK CONSTRAINT [FK_durados_DnsAlias_durados_App]
GO

/****** Object:  Index [IX_durados_DnsAlias]    Script Date: 10/21/2012 13:25:27 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[durados_DnsAlias]') AND name = N'IX_durados_DnsAlias')
DROP INDEX [IX_durados_DnsAlias] ON [dbo].[durados_DnsAlias] WITH ( ONLINE = OFF )
GO

/****** Object:  Index [IX_durados_DnsAlias]    Script Date: 10/21/2012 13:25:27 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_durados_DnsAlias] ON [dbo].[durados_DnsAlias] 
(
	[Alias] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

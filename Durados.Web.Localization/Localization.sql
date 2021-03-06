USE [Durados_Localization]
begin tran a
/****** Object:  Table [dbo].[Durados_TranslationKey]    Script Date: 05/20/2010 12:15:06 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Durados_TranslationKey]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Durados_TranslationKey](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Key] [nvarchar](2000) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_Durados_TranslationKey] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Durados_TranslationKey]') AND name = N'IX_Durados_TranslationKey')
CREATE UNIQUE NONCLUSTERED INDEX [IX_Durados_TranslationKey] ON [dbo].[Durados_TranslationKey] 
(
	[Key] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

/****** Object:  Table [dbo].[Durados_Language]    Script Date: 05/20/2010 12:15:06 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Durados_Language]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Durados_Language](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Name] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[NativeName] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Direction] [nvarchar](5) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[CodePage] [smallint] NOT NULL,
	[Active] [bit] NOT NULL,
 CONSTRAINT [PK_Durados_Language] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Durados_Language]') AND name = N'IX_Durados_Language_Name')
CREATE UNIQUE NONCLUSTERED INDEX [IX_Durados_Language_Name] ON [dbo].[Durados_Language] 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Durados_Language]') AND name = N'IX_Durados_Language_NativeName')
CREATE UNIQUE NONCLUSTERED INDEX [IX_Durados_Language_NativeName] ON [dbo].[Durados_Language] 
(
	[NativeName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

SET IDENTITY_INSERT [dbo].[Durados_Language] ON
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (159, N'en-us', N'English', N'English', N'LTR', 1252, 0)

INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (173, N'he', N'Hebrew', N'עברית', N'RTL', 1255, 0)
SET IDENTITY_INSERT [dbo].[Durados_Language] OFF
/****** Object:  Table [dbo].[Durados_Translation]    Script Date: 05/20/2010 12:15:06 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Durados_Translation]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Durados_Translation](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[TranslationKeyID] [int] NOT NULL,
	[LanguageID] [int] NOT NULL,
	[Translation] [nvarchar](2000) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_Durados_Translation] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Durados_Translation]') AND name = N'IX_Durados_Translation_Key_Language')
CREATE UNIQUE NONCLUSTERED INDEX [IX_Durados_Translation_Key_Language] ON [dbo].[Durados_Translation] 
(
	[TranslationKeyID] ASC,
	[LanguageID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

/****** Object:  Default [DF_Durados_Language_CodePage]    Script Date: 05/20/2010 12:15:06 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_Durados_Language_CodePage]') AND parent_object_id = OBJECT_ID(N'[dbo].[Durados_Language]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Durados_Language_CodePage]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Durados_Language] ADD  CONSTRAINT [DF_Durados_Language_CodePage]  DEFAULT ((1252)) FOR [CodePage]
END


End

/****** Object:  Default [DF_Durados_Language_Active]    Script Date: 05/20/2010 12:15:06 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_Durados_Language_Active]') AND parent_object_id = OBJECT_ID(N'[dbo].[Durados_Language]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Durados_Language_Active]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Durados_Language] ADD  CONSTRAINT [DF_Durados_Language_Active]  DEFAULT ((0)) FOR [Active]
END


End

/****** Object:  ForeignKey [FK_Durados_Translation_Durados_Language]    Script Date: 05/20/2010 12:15:06 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Durados_Translation_Durados_Language]') AND parent_object_id = OBJECT_ID(N'[dbo].[Durados_Translation]'))
ALTER TABLE [dbo].[Durados_Translation]  WITH CHECK ADD  CONSTRAINT [FK_Durados_Translation_Durados_Language] FOREIGN KEY([LanguageID])
REFERENCES [dbo].[Durados_Language] ([ID])
ON DELETE CASCADE

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Durados_Translation_Durados_Language]') AND parent_object_id = OBJECT_ID(N'[dbo].[Durados_Translation]'))
ALTER TABLE [dbo].[Durados_Translation] CHECK CONSTRAINT [FK_Durados_Translation_Durados_Language]

/****** Object:  ForeignKey [FK_Durados_Translation_Durados_TranslationKey]    Script Date: 05/20/2010 12:15:06 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Durados_Translation_Durados_TranslationKey]') AND parent_object_id = OBJECT_ID(N'[dbo].[Durados_Translation]'))
ALTER TABLE [dbo].[Durados_Translation]  WITH CHECK ADD  CONSTRAINT [FK_Durados_Translation_Durados_TranslationKey] FOREIGN KEY([TranslationKeyID])
REFERENCES [dbo].[Durados_TranslationKey] ([ID])
ON DELETE CASCADE

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Durados_Translation_Durados_TranslationKey]') AND parent_object_id = OBJECT_ID(N'[dbo].[Durados_Translation]'))
ALTER TABLE [dbo].[Durados_Translation] CHECK CONSTRAINT [FK_Durados_Translation_Durados_TranslationKey]

commit tran a
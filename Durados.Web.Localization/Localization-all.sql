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
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (143, N'af', N'Afrikaans', N'Afrikaans', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (144, N'ar-sa', N'Arabic (Saudi Arabia)', N'العربية', N'RTL', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (145, N'ar-eg', N'Arabic (Egypt)', N'Arabic (Egypt)', N'RTL', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (146, N'ar-dz', N'Arabic (Algeria)', N'Arabic (Algeria)', N'RTL', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (147, N'ar-tn', N'Arabic (Tunisia)', N'Arabic (Tunisia)', N'RTL', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (148, N'ar-ye', N'Arabic (Yemen)', N'Arabic (Yemen)', N'RTL', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (149, N'ar-jo', N'Arabic (Jordan)', N'Arabic (Jordan)', N'RTL', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (150, N'ar-kw', N'Arabic (Kuwait)', N'Arabic (Kuwait)', N'RTL', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (151, N'ar-bh', N'Arabic (Bahrain)', N'Arabic (Bahrain)', N'RTL', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (152, N'eu', N'Basque', N'Euskara', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (153, N'be', N'Belarusian', N'Беларуская', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (154, N'zh-tw', N'Chinese (Taiwan)', N'中文', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (155, N'zh-hk', N'Chinese (Hong Kong SAR)', N'Chinese (Hong Kong SAR)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (156, N'hr', N'Croatian', N'Hrvatski', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (157, N'da', N'Danish', N'Dansk', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (158, N'nl-be', N'Dutch (Belgium)', N'Nederlands', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (159, N'en-us', N'English (United States)', N'English (United States)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (160, N'en-au', N'English (Australia)', N'English (Australia)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (161, N'en-nz', N'English (New Zealand)', N'English (New Zealand)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (162, N'en-za', N'English (South Africa)', N'English (South Africa)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (163, N'en', N'English (Caribbean)', N'English (Caribbean)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (164, N'en-tt', N'English (Trinidad)', N'English (Trinidad)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (165, N'fo', N'Faeroese', N'Føroyskt', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (166, N'fi', N'Finnish', N'Suomi', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (167, N'fr-be', N'French (Belgium)', N'Français', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (168, N'fr-ch', N'French (Switzerland)', N'French (Switzerland)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (169, N'gd', N'Gaelic (Scotland)', N'Gàidhlig', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (170, N'de', N'German (Standard)', N'Deutsch', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (171, N'de-at', N'German (Austria)', N'German (Austria)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (172, N'de-li', N'German (Liechtenstein)', N'German (Liechtenstein)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (173, N'he', N'Hebrew', N'עברית', N'RTL', 1255, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (174, N'hu', N'Hungarian', N'Magyar', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (175, N'id', N'Indonesian', N'Bahasa Indonesia', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (176, N'it-ch', N'Italian (Switzerland)', N'Italiano', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (177, N'ko', N'Korean', N'한국어', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (178, N'lv', N'Latvian', N'Latviešu', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (179, N'mk', N'Macedonian (FYROM)', N'Македонски', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (180, N'mt', N'Maltese', N'bil-Malti', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (181, N'no', N'Norwegian (Nynorsk)', N'Norsk (bokmål / riksmål)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (182, N'pt-br', N'Portuguese (Brazil)', N'Português', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (183, N'rm', N'Rhaeto-Romanic', N'Rumantsch', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (184, N'ro-mo', N'Romanian (Republic of Moldova)', N'Română', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (185, N'ru-mo', N'Russian (Republic of Moldova)', N'Русский', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (186, N'sr', N'Serbian (Cyrillic)', N'Српски', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (187, N'sk', N'Slovak', N'Slovenčina', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (188, N'sb', N'Sorbian', N'Sorbian', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (189, N'es-mx', N'Spanish (Mexico)', N'Español', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (190, N'es-cr', N'Spanish (Costa Rica)', N'Spanish (Costa Rica)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (191, N'es-do', N'Spanish (Dominican Republic)', N'Spanish (Dominican Republic)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (192, N'es-co', N'Spanish (Colombia)', N'Spanish (Colombia)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (193, N'es-ar', N'Spanish (Argentina)', N'Spanish (Argentina)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (194, N'es-cl', N'Spanish (Chile)', N'Spanish (Chile)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (195, N'es-py', N'Spanish (Paraguay)', N'Spanish (Paraguay)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (196, N'es-sv', N'Spanish (El Salvador)', N'Spanish (El Salvador)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (197, N'es-ni', N'Spanish (Nicaragua)', N'Spanish (Nicaragua)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (198, N'sx', N'Sutu', N'Sutu', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (199, N'sv-fi', N'Swedish (Finland)', N'Svenska', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (200, N'ts', N'Tsonga', N'Xitsonga', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (201, N'tr', N'Turkish', N'Türkçe', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (202, N'ur', N'Urdu', N'اردو', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (203, N'vi', N'Vietnamese', N'Tiếng Việt', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (204, N'ji', N'Yiddish', N'Yiddish', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (205, N'sq', N'Albanian', N'Shqip', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (206, N'ar-iq', N'Arabic (Iraq)', N'Arabic (Iraq)', N'RTL', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (207, N'ar-ly', N'Arabic (Libya)', N'Arabic (Libya)', N'RTL', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (208, N'ar-ma', N'Arabic (Morocco)', N'Arabic (Morocco)', N'RTL', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (209, N'ar-om', N'Arabic (Oman)', N'Arabic (Oman)', N'RTL', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (210, N'ar-sy', N'Arabic (Syria)', N'Arabic (Syria)', N'RTL', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (211, N'ar-lb', N'Arabic (Lebanon)', N'Arabic (Lebanon)', N'RTL', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (212, N'ar-ae', N'Arabic (U.A.E.)', N'Arabic (U.A.E.)', N'RTL', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (213, N'ar-qa', N'Arabic (Qatar)', N'Arabic (Qatar)', N'RTL', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (214, N'bg', N'Bulgarian', N'Български', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (215, N'ca', N'Catalan', N'Català', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (216, N'zh-cn', N'Chinese (PRC)', N'Chinese (PRC)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (217, N'zh-sg', N'Chinese (Singapore)', N'Chinese (Singapore)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (218, N'cs', N'Czech', N'Česky', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (219, N'nl', N'Dutch (Standard)', N'Dutch (Standard)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (220, N'en', N'English', N'English', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (221, N'en-gb', N'English (United Kingdom)', N'English (United Kingdom)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (222, N'en-ca', N'English (Canada)', N'English (Canada)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (223, N'en-ie', N'English (Ireland)', N'English (Ireland)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (224, N'en-jm', N'English (Jamaica)', N'English (Jamaica)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (225, N'en-bz', N'English (Belize)', N'English (Belize)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (226, N'et', N'Estonian', N'Eesti', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (227, N'fa', N'Farsi', N'فارسی', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (228, N'fr', N'French (Standard)', N'French (Standard)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (229, N'fr-ca', N'French (Canada)', N'French (Canada)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (230, N'fr-lu', N'French (Luxembourg)', N'French (Luxembourg)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (231, N'ga', N'Irish', N'Gaeilge', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (232, N'de-ch', N'German (Switzerland)', N'German (Switzerland)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (233, N'de-lu', N'German (Luxembourg)', N'German (Luxembourg)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (234, N'el', N'Greek', N'Ελληνικά', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (235, N'hi', N'Hindi', N'हिन्दी', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (236, N'is', N'Icelandic', N'Íslenska', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (237, N'it', N'Italian (Standard)', N'Italian (Standard)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (238, N'ja', N'Japanese', N'日本語', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (239, N'ko', N'Korean (Johab)', N'Korean (Johab)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (240, N'lt', N'Lithuanian', N'Lietuvių', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (241, N'ms', N'Malaysian', N'Bahasa Melayu', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (242, N'no', N'Norwegian (Bokmal)', N'Norwegian (Bokmal)', N'LTR', 1252, 0)

print 'Processed 100 total records'
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (243, N'pl', N'Polish', N'Polski', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (244, N'pt', N'Portuguese (Portugal)', N'Portuguese (Portugal)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (245, N'ro', N'Romanian', N'Romanian', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (246, N'ru', N'Russian', N'Russian', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (247, N'sz', N'Sami (Lappish)', N'Sami (Lappish)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (248, N'sr', N'Serbian (Latin)', N'Serbian (Latin)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (249, N'sl', N'Slovenian', N'Slovenščina', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (250, N'es', N'Spanish (Spain)', N'Spanish (Spain)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (251, N'es-gt', N'Spanish (Guatemala)', N'Spanish (Guatemala)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (252, N'es-pa', N'Spanish (Panama)', N'Spanish (Panama)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (253, N'es-ve', N'Spanish (Venezuela)', N'Spanish (Venezuela)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (254, N'es-pe', N'Spanish (Peru)', N'Spanish (Peru)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (255, N'es-ec', N'Spanish (Ecuador)', N'Spanish (Ecuador)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (256, N'es-uy', N'Spanish (Uruguay)', N'Spanish (Uruguay)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (257, N'es-bo', N'Spanish (Bolivia)', N'Spanish (Bolivia)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (258, N'es-hn', N'Spanish (Honduras)', N'Spanish (Honduras)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (259, N'es-pr', N'Spanish (Puerto Rico)', N'Spanish (Puerto Rico)', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (260, N'sv', N'Swedish', N'Swedish', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (261, N'th', N'Thai', N'ไทย / Phasa Thai', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (262, N'tn', N'Tswana', N'Setswana', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (263, N'uk', N'Ukrainian', N'Українська', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (264, N've', N'Venda', N'Tshivenḓa', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (265, N'xh', N'Xhosa', N'isiXhosa', N'LTR', 1252, 0)
INSERT [dbo].[Durados_Language] ([ID], [Code], [Name], [NativeName], [Direction], [CodePage], [Active]) VALUES (266, N'zu', N'Zulu', N'isiZulu', N'LTR', 1252, 0)
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
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
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
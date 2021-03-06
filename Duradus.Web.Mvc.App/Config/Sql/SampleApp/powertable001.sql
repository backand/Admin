
CREATE TABLE [dbo].[PowerTableMulti](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NULL,
	[Ordinal] [int] NULL,
 CONSTRAINT [PK_PowerTableMulti] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[PowerTableMulti] ON
INSERT [dbo].[PowerTableMulti] ([Id], [Name], [Ordinal]) VALUES (1, N'Multi 1', 1)
INSERT [dbo].[PowerTableMulti] ([Id], [Name], [Ordinal]) VALUES (2, N'Multi 2', 2)
INSERT [dbo].[PowerTableMulti] ([Id], [Name], [Ordinal]) VALUES (3, N'Multi 3', 3)
INSERT [dbo].[PowerTableMulti] ([Id], [Name], [Ordinal]) VALUES (4, N'Multi 4', 4)
SET IDENTITY_INSERT [dbo].[PowerTableMulti] OFF
/****** Object:  Table [dbo].[PowerTableList]    Script Date: 02/11/2013 16:19:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PowerTableList](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NULL,
	[Ordinal] [int] NULL,
 CONSTRAINT [PK_PowerTableList] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[PowerTableList] ON
INSERT [dbo].[PowerTableList] ([Id], [Name], [Ordinal]) VALUES (1, N'Option 1', 10)
INSERT [dbo].[PowerTableList] ([Id], [Name], [Ordinal]) VALUES (2, N'Option 2', 20)
INSERT [dbo].[PowerTableList] ([Id], [Name], [Ordinal]) VALUES (3, N'Option 3', 30)
SET IDENTITY_INSERT [dbo].[PowerTableList] OFF
/****** Object:  Table [dbo].[PowerTable]    Script Date: 02/11/2013 16:19:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PowerTable](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ShortText] [nvarchar](250) NULL,
	[LongText] [nvarchar](250) NULL,
	[Numeric] [float] NULL,
	[Date] [datetime] NULL,
	[Boolean] [bit] NULL,
	[ListId] [int] NULL,
 CONSTRAINT [PK_PowerTable] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[PowerTable] ON
INSERT [dbo].[PowerTable] ([Id], [ShortText], [LongText], [Numeric], [Date], [Boolean], [ListId]) VALUES (1, N'Short text', N'Long text Long text Long text Long text
Long text Long text Long text Long text
Long text Long text', 23, CAST(0x0000A14200000000 AS DateTime), 1, 1)
INSERT [dbo].[PowerTable] ([Id], [ShortText], [LongText], [Numeric], [Date], [Boolean], [ListId]) VALUES (2, N'Short text', N'Long text Long text Long text Long text
Long text Long text Long text Long text
Long text Long text', 23, CAST(0x0000A14200000000 AS DateTime), 1, 1)
INSERT [dbo].[PowerTable] ([Id], [ShortText], [LongText], [Numeric], [Date], [Boolean], [ListId]) VALUES (3, N'Short text', N'Long text Long text Long text Long text
Long text Long text Long text Long text
Long text Long text', 86, CAST(0x0000A14200000000 AS DateTime), 0, 1)
INSERT [dbo].[PowerTable] ([Id], [ShortText], [LongText], [Numeric], [Date], [Boolean], [ListId]) VALUES (4, N'Short text', N'Long text Long text Long text Long text
Long text Long text Long text Long text
Long text Long text', 23, CAST(0x0000A14200000000 AS DateTime), 1, 1)
INSERT [dbo].[PowerTable] ([Id], [ShortText], [LongText], [Numeric], [Date], [Boolean], [ListId]) VALUES (5, N'Short text', N'Long text Long text Long text Long text
Long text Long text Long text Long text
Long text Long text', 23, CAST(0x0000A14200000000 AS DateTime), 1, 1)
SET IDENTITY_INSERT [dbo].[PowerTable] OFF
/****** Object:  Table [dbo].[v_PowerTable_PowerTableMulti]    Script Date: 02/11/2013 16:19:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[v_PowerTable_PowerTableMulti](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[v_PowerTableId] [int] NULL,
	[PowerTableMultiId] [int] NULL,
 CONSTRAINT [PK_v_PowerTable_PowerTableMulti] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[v_PowerTable]    Script Date: 02/11/2013 16:19:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[v_PowerTable] AS select ListId, Boolean, Date, Numeric, LongText, ShortText, Id from [PowerTable]
GO
/****** Object:  ForeignKey [FK_PowerTable_PowerTableList_ListId]    Script Date: 02/11/2013 16:19:46 ******/
ALTER TABLE [dbo].[PowerTable]  WITH CHECK ADD  CONSTRAINT [FK_PowerTable_PowerTableList_ListId] FOREIGN KEY([ListId])
REFERENCES [dbo].[PowerTableList] ([Id])
GO
ALTER TABLE [dbo].[PowerTable] CHECK CONSTRAINT [FK_PowerTable_PowerTableList_ListId]
GO
/****** Object:  ForeignKey [FK_v_PowerTable_PowerTableMulti_PowerTable_v_PowerTableId]    Script Date: 02/11/2013 16:19:46 ******/
ALTER TABLE [dbo].[v_PowerTable_PowerTableMulti]  WITH CHECK ADD  CONSTRAINT [FK_v_PowerTable_PowerTableMulti_PowerTable_v_PowerTableId] FOREIGN KEY([v_PowerTableId])
REFERENCES [dbo].[PowerTable] ([Id])
GO
ALTER TABLE [dbo].[v_PowerTable_PowerTableMulti] CHECK CONSTRAINT [FK_v_PowerTable_PowerTableMulti_PowerTable_v_PowerTableId]
GO
/****** Object:  ForeignKey [FK_v_PowerTable_PowerTableMulti_PowerTableMulti_PowerTableMultiId]    Script Date: 02/11/2013 16:19:46 ******/
ALTER TABLE [dbo].[v_PowerTable_PowerTableMulti]  WITH CHECK ADD  CONSTRAINT [FK_v_PowerTable_PowerTableMulti_PowerTableMulti_PowerTableMultiId] FOREIGN KEY([PowerTableMultiId])
REFERENCES [dbo].[PowerTableMulti] ([Id])
GO
ALTER TABLE [dbo].[v_PowerTable_PowerTableMulti] CHECK CONSTRAINT [FK_v_PowerTable_PowerTableMulti_PowerTableMulti_PowerTableMultiId]
GO

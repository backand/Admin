CREATE TABLE [dbo].[v_Car_Inventory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NULL,
	[Ordinal] [int] NULL,
 CONSTRAINT [PK_v_Car_Inventory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Car InventoryYear]    Script Date: 02/12/2013 12:11:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Car InventoryYear](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NULL,
	[Ordinal] [int] NULL,
 CONSTRAINT [PK_Car InventoryYear] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Car InventoryYear] ON
INSERT [dbo].[Car InventoryYear] ([Id], [Name], [Ordinal]) VALUES (1, N'2006', 10)
INSERT [dbo].[Car InventoryYear] ([Id], [Name], [Ordinal]) VALUES (2, N'2007', 20)
INSERT [dbo].[Car InventoryYear] ([Id], [Name], [Ordinal]) VALUES (3, N'2008', 30)
INSERT [dbo].[Car InventoryYear] ([Id], [Name], [Ordinal]) VALUES (4, N'2009', 40)
INSERT [dbo].[Car InventoryYear] ([Id], [Name], [Ordinal]) VALUES (5, N'2010', 50)
INSERT [dbo].[Car InventoryYear] ([Id], [Name], [Ordinal]) VALUES (6, N'2011', 60)
INSERT [dbo].[Car InventoryYear] ([Id], [Name], [Ordinal]) VALUES (7, N'2012', 70)
INSERT [dbo].[Car InventoryYear] ([Id], [Name], [Ordinal]) VALUES (8, N'2013', 80)
INSERT [dbo].[Car InventoryYear] ([Id], [Name], [Ordinal]) VALUES (9, N'Brand New', 90)
SET IDENTITY_INSERT [dbo].[Car InventoryYear] OFF
/****** Object:  Table [dbo].[Car InventoryTransmission]    Script Date: 02/12/2013 12:11:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Car InventoryTransmission](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NULL,
	[Ordinal] [int] NULL,
 CONSTRAINT [PK_Car InventoryTransmission] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Car InventoryTransmission] ON
INSERT [dbo].[Car InventoryTransmission] ([Id], [Name], [Ordinal]) VALUES (1, N'Automatic', 10)
INSERT [dbo].[Car InventoryTransmission] ([Id], [Name], [Ordinal]) VALUES (2, N'Manual', 20)
SET IDENTITY_INSERT [dbo].[Car InventoryTransmission] OFF
/****** Object:  Table [dbo].[Car InventoryStatus]    Script Date: 02/12/2013 12:11:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Car InventoryStatus](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NULL,
	[Ordinal] [int] NULL,
 CONSTRAINT [PK_Car InventoryStatus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Car InventoryStatus] ON
INSERT [dbo].[Car InventoryStatus] ([Id], [Name], [Ordinal]) VALUES (1, N'New', 10)
INSERT [dbo].[Car InventoryStatus] ([Id], [Name], [Ordinal]) VALUES (2, N'Used', 20)
SET IDENTITY_INSERT [dbo].[Car InventoryStatus] OFF
/****** Object:  Table [dbo].[Car InventoryInterior]    Script Date: 02/12/2013 12:11:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Car InventoryInterior](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NULL,
	[Ordinal] [int] NULL,
 CONSTRAINT [PK_Car InventoryInterior] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Car InventoryInterior] ON
INSERT [dbo].[Car InventoryInterior] ([Id], [Name], [Ordinal]) VALUES (1, N'Black', 10)
INSERT [dbo].[Car InventoryInterior] ([Id], [Name], [Ordinal]) VALUES (2, N'Silver', 20)
SET IDENTITY_INSERT [dbo].[Car InventoryInterior] OFF
/****** Object:  Table [dbo].[Car InventoryExterior]    Script Date: 02/12/2013 12:11:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Car InventoryExterior](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NULL,
	[Ordinal] [int] NULL,
 CONSTRAINT [PK_Car InventoryExterior] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Car InventoryExterior] ON
INSERT [dbo].[Car InventoryExterior] ([Id], [Name], [Ordinal]) VALUES (1, N'Silver', 10)
INSERT [dbo].[Car InventoryExterior] ([Id], [Name], [Ordinal]) VALUES (2, N'Red', 20)
INSERT [dbo].[Car InventoryExterior] ([Id], [Name], [Ordinal]) VALUES (3, N'Black', 30)
SET IDENTITY_INSERT [dbo].[Car InventoryExterior] OFF
/****** Object:  Table [dbo].[Car InventoryCompany]    Script Date: 02/12/2013 12:11:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Car InventoryCompany](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NULL,
	[Ordinal] [int] NULL,
 CONSTRAINT [PK_Car InventoryCompany] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Car InventoryCompany] ON
INSERT [dbo].[Car InventoryCompany] ([Id], [Name], [Ordinal]) VALUES (1, N'BMW', 10)
SET IDENTITY_INSERT [dbo].[Car InventoryCompany] OFF
/****** Object:  Table [dbo].[Car Inventory]    Script Date: 02/12/2013 12:11:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Car Inventory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ExteriorId] [int] NULL,
	[InteriorId] [int] NULL,
	[TransmissionId] [int] NULL,
	[Mileage] [float] NULL,
	[YearId] [int] NULL,
	[CompanyId] [int] NULL,
	[Model] [nvarchar](250) NULL,
	[Price] [float] NULL,
	[Description] [nvarchar](250) NULL,
	[imageURL] [nvarchar](250) NULL,
	[StatusId] [int] NULL,
 CONSTRAINT [PK_Car Inventory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Car Inventory] ON
INSERT [dbo].[Car Inventory] ([Id], [ExteriorId], [InteriorId], [TransmissionId], [Mileage], [YearId], [CompanyId], [Model], [Price], [Description], [imageURL], [StatusId]) VALUES (1, 2, 1, 1, 50000, 1, 1, N'BMW 3-Series 325i Sedan', 12000, N'<font color="#ff0000">Great cal low milage</font>', N'http://static.wix.com/media/84770f_e3ce96a97eab1f5f76d7e65d112216a8.jpg_256', 1)
INSERT [dbo].[Car Inventory] ([Id], [ExteriorId], [InteriorId], [TransmissionId], [Mileage], [YearId], [CompanyId], [Model], [Price], [Description], [imageURL], [StatusId]) VALUES (2, 1, 1, 1, 40000, 5, 1, N'BMW 3-Series 327i Sedan', 15000, NULL, N'http://static.wix.com/media/84770f_e04bf00f904ba16a0d0682b6b7839a41.jpg_256', 1)
INSERT [dbo].[Car Inventory] ([Id], [ExteriorId], [InteriorId], [TransmissionId], [Mileage], [YearId], [CompanyId], [Model], [Price], [Description], [imageURL], [StatusId]) VALUES (3, 3, 1, 1, 80000, 3, 1, N'BMW 3-Series 325i Sedan', 13000, NULL, N'http://static.wix.com/media/84770f_b60c1bfdf5d89bc52ff82d1687229d13.jpg_256', 2)
SET IDENTITY_INSERT [dbo].[Car Inventory] OFF
/****** Object:  View [dbo].[v_Car Inventory]    Script Date: 02/12/2013 12:11:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[v_Car Inventory] AS select StatusId, imageURL, Description, Price, Model, CompanyId, YearId, Mileage, TransmissionId, InteriorId, ExteriorId, Id from [Car Inventory]
GO
/****** Object:  ForeignKey [FK_Car Inventory_Car InventoryCompany_CompanyId]    Script Date: 02/12/2013 12:11:25 ******/
ALTER TABLE [dbo].[Car Inventory]  WITH CHECK ADD  CONSTRAINT [FK_Car Inventory_Car InventoryCompany_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[Car InventoryCompany] ([Id])
GO
ALTER TABLE [dbo].[Car Inventory] CHECK CONSTRAINT [FK_Car Inventory_Car InventoryCompany_CompanyId]
GO
/****** Object:  ForeignKey [FK_Car Inventory_Car InventoryExterior_ExteriorId]    Script Date: 02/12/2013 12:11:25 ******/
ALTER TABLE [dbo].[Car Inventory]  WITH CHECK ADD  CONSTRAINT [FK_Car Inventory_Car InventoryExterior_ExteriorId] FOREIGN KEY([ExteriorId])
REFERENCES [dbo].[Car InventoryExterior] ([Id])
GO
ALTER TABLE [dbo].[Car Inventory] CHECK CONSTRAINT [FK_Car Inventory_Car InventoryExterior_ExteriorId]
GO
/****** Object:  ForeignKey [FK_Car Inventory_Car InventoryInterior_InteriorId]    Script Date: 02/12/2013 12:11:25 ******/
ALTER TABLE [dbo].[Car Inventory]  WITH CHECK ADD  CONSTRAINT [FK_Car Inventory_Car InventoryInterior_InteriorId] FOREIGN KEY([InteriorId])
REFERENCES [dbo].[Car InventoryInterior] ([Id])
GO
ALTER TABLE [dbo].[Car Inventory] CHECK CONSTRAINT [FK_Car Inventory_Car InventoryInterior_InteriorId]
GO
/****** Object:  ForeignKey [FK_Car Inventory_Car InventoryStatus_StatusId]    Script Date: 02/12/2013 12:11:25 ******/
ALTER TABLE [dbo].[Car Inventory]  WITH CHECK ADD  CONSTRAINT [FK_Car Inventory_Car InventoryStatus_StatusId] FOREIGN KEY([StatusId])
REFERENCES [dbo].[Car InventoryStatus] ([Id])
GO
ALTER TABLE [dbo].[Car Inventory] CHECK CONSTRAINT [FK_Car Inventory_Car InventoryStatus_StatusId]
GO
/****** Object:  ForeignKey [FK_Car Inventory_Car InventoryTransmission_TransmissionId]    Script Date: 02/12/2013 12:11:25 ******/
ALTER TABLE [dbo].[Car Inventory]  WITH CHECK ADD  CONSTRAINT [FK_Car Inventory_Car InventoryTransmission_TransmissionId] FOREIGN KEY([TransmissionId])
REFERENCES [dbo].[Car InventoryTransmission] ([Id])
GO
ALTER TABLE [dbo].[Car Inventory] CHECK CONSTRAINT [FK_Car Inventory_Car InventoryTransmission_TransmissionId]
GO
/****** Object:  ForeignKey [FK_Car Inventory_Car InventoryYear_YearId]    Script Date: 02/12/2013 12:11:25 ******/
ALTER TABLE [dbo].[Car Inventory]  WITH CHECK ADD  CONSTRAINT [FK_Car Inventory_Car InventoryYear_YearId] FOREIGN KEY([YearId])
REFERENCES [dbo].[Car InventoryYear] ([Id])
GO
ALTER TABLE [dbo].[Car Inventory] CHECK CONSTRAINT [FK_Car Inventory_Car InventoryYear_YearId]
GO

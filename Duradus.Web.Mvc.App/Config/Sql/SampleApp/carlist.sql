CREATE TABLE [dbo].[$CarListYear$](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NULL,
	[Ordinal] [int] NULL,
 CONSTRAINT [PK_$CarListYear$] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[$CarListYear$] ON
INSERT [dbo].[$CarListYear$] ([Id], [Name], [Ordinal]) VALUES (1, N'2013', 10)
INSERT [dbo].[$CarListYear$] ([Id], [Name], [Ordinal]) VALUES (2, N'2012', 20)
INSERT [dbo].[$CarListYear$] ([Id], [Name], [Ordinal]) VALUES (3, N'2011', 30)
INSERT [dbo].[$CarListYear$] ([Id], [Name], [Ordinal]) VALUES (4, N'2010', 40)
INSERT [dbo].[$CarListYear$] ([Id], [Name], [Ordinal]) VALUES (5, N'2009', 50)
SET IDENTITY_INSERT [dbo].[$CarListYear$] OFF
/****** Object:  Table [dbo].[$CarListTransmission$]    Script Date: 03/13/2013 15:12:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[$CarListTransmission$](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NULL,
	[Ordinal] [int] NULL,
 CONSTRAINT [PK_$CarListTransmission$] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[$CarListTransmission$] ON
INSERT [dbo].[$CarListTransmission$] ([Id], [Name], [Ordinal]) VALUES (1, N'Automatic', 10)
INSERT [dbo].[$CarListTransmission$] ([Id], [Name], [Ordinal]) VALUES (2, N'Manual', 20)
INSERT [dbo].[$CarListTransmission$] ([Id], [Name], [Ordinal]) VALUES (3, N'Other', 30)
SET IDENTITY_INSERT [dbo].[$CarListTransmission$] OFF
/****** Object:  Table [dbo].[$CarListInterior$]    Script Date: 03/13/2013 15:12:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[$CarListInterior$](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NULL,
	[Ordinal] [int] NULL,
 CONSTRAINT [PK_$CarListInterior$] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[$CarListInterior$] ON
INSERT [dbo].[$CarListInterior$] ([Id], [Name], [Ordinal]) VALUES (1, N'Black', 10)
INSERT [dbo].[$CarListInterior$] ([Id], [Name], [Ordinal]) VALUES (2, N'Red', 20)
INSERT [dbo].[$CarListInterior$] ([Id], [Name], [Ordinal]) VALUES (3, N'Silver', 30)
SET IDENTITY_INSERT [dbo].[$CarListInterior$] OFF
/****** Object:  Table [dbo].[$CarListExterior$]    Script Date: 03/13/2013 15:12:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[$CarListExterior$](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NULL,
	[Ordinal] [int] NULL,
 CONSTRAINT [PK_$CarListExterior$] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[$CarListExterior$] ON
INSERT [dbo].[$CarListExterior$] ([Id], [Name], [Ordinal]) VALUES (1, N'Black', 10)
INSERT [dbo].[$CarListExterior$] ([Id], [Name], [Ordinal]) VALUES (2, N'Red', 20)
INSERT [dbo].[$CarListExterior$] ([Id], [Name], [Ordinal]) VALUES (3, N'Silver', 30)
SET IDENTITY_INSERT [dbo].[$CarListExterior$] OFF
/****** Object:  Table [dbo].[$CarListCompany$]    Script Date: 03/13/2013 15:12:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[$CarListCompany$](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NULL,
	[Ordinal] [int] NULL,
 CONSTRAINT [PK_$CarListCompany$] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[$CarListCompany$] ON
INSERT [dbo].[$CarListCompany$] ([Id], [Name], [Ordinal]) VALUES (1, N'BMW', 10)
INSERT [dbo].[$CarListCompany$] ([Id], [Name], [Ordinal]) VALUES (2, N'Audi', 20)
SET IDENTITY_INSERT [dbo].[$CarListCompany$] OFF
/****** Object:  Table [dbo].[CarList]    Script Date: 03/13/2013 15:12:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[$CarList$](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Picture] [nvarchar](250) NULL,
	[YearId] [int] NULL,
	[CompanyId] [int] NULL,
	[Model] [nvarchar](250) NULL,
	[Price] [float] NULL,
	[ExteriorId] [int] NULL,
	[InteriorId] [int] NULL,
	[TransmissionId] [int] NULL,
	[Mileage] [float] NULL,
	[Description] [nvarchar](4000) NULL,
 CONSTRAINT [PK_$CarList$] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[$CarList$] ON
INSERT [dbo].[$CarList$] ([Id], [Picture], [YearId], [CompanyId], [Model], [Price], [ExteriorId], [InteriorId], [TransmissionId], [Mileage], [Description]) VALUES (1, N'car1.jpg', 1, 1, N'BMW 3-Series 325i Sedan', 12000, 1, 2, 1, 50000, N'Fast forward to today and BMW is the new Mercedes and the full-sized luxury segment is getting crowded with entries from Audi, Porsche and an XJ that spends enough time running to count')
INSERT [dbo].[$CarList$] ([Id], [Picture], [YearId], [CompanyId], [Model], [Price], [ExteriorId], [InteriorId], [TransmissionId], [Mileage], [Description]) VALUES (2, N'car2.jpg', 3, 1, N'BMW 3-Series 327i Sedan', 15000, 1, 2, 1, 40000, N'Fast forward to today and BMW is the new Mercedes and the full-sized luxury segment is getting crowded with entries from Audi, Porsche and an XJ that spends enough time running to count')
INSERT [dbo].[$CarList$] ([Id], [Picture], [YearId], [CompanyId], [Model], [Price], [ExteriorId], [InteriorId], [TransmissionId], [Mileage], [Description]) VALUES (3, N'car3.jpg', 5, 1, N'BMW 3-Series 325i Sedan', 19000, 1, 2, 1, 30000, N'Fast forward to today and BMW is the new Mercedes and the full-sized luxury segment is getting crowded with entries from Audi, Porsche and an XJ that spends enough time running to count')
SET IDENTITY_INSERT [dbo].[$CarList$] OFF
/****** Object:  ForeignKey [FK_$CarList$_$CarListCompany$_CompanyId]    Script Date: 03/13/2013 15:12:29 ******/
ALTER TABLE [dbo].[$CarList$]  WITH CHECK ADD  CONSTRAINT [FK_$CarList$_$CarListCompany$_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[$CarListCompany$] ([Id])
GO
ALTER TABLE [dbo].[$CarList$] CHECK CONSTRAINT [FK_$CarList$_$CarListCompany$_CompanyId]
GO
/****** Object:  ForeignKey [FK_$CarList$_$CarListExterior$_ExteriorId]    Script Date: 03/13/2013 15:12:29 ******/
ALTER TABLE [dbo].[$CarList$]  WITH CHECK ADD  CONSTRAINT [FK_$CarList$_$CarListExterior$_ExteriorId] FOREIGN KEY([ExteriorId])
REFERENCES [dbo].[$CarListExterior$] ([Id])
GO
ALTER TABLE [dbo].[$CarList$] CHECK CONSTRAINT [FK_$CarList$_$CarListExterior$_ExteriorId]
GO
/****** Object:  ForeignKey [FK_$CarList$_$CarListInterior$_InteriorId]    Script Date: 03/13/2013 15:12:29 ******/
ALTER TABLE [dbo].[$CarList$]  WITH CHECK ADD  CONSTRAINT [FK_$CarList$_$CarListInterior$_InteriorId] FOREIGN KEY([InteriorId])
REFERENCES [dbo].[$CarListInterior$] ([Id])
GO
ALTER TABLE [dbo].[$CarList$] CHECK CONSTRAINT [FK_$CarList$_$CarListInterior$_InteriorId]
GO
/****** Object:  ForeignKey [FK_$CarList$_$CarListTransmission$_TransmissionId]    Script Date: 03/13/2013 15:12:29 ******/
ALTER TABLE [dbo].[$CarList$]  WITH CHECK ADD  CONSTRAINT [FK_$CarList$_$CarListTransmission$_TransmissionId] FOREIGN KEY([TransmissionId])
REFERENCES [dbo].[$CarListTransmission$] ([Id])
GO
ALTER TABLE [dbo].[$CarList$] CHECK CONSTRAINT [FK_$CarList$_$CarListTransmission$_TransmissionId]
GO
/****** Object:  ForeignKey [FK_$CarList$_$CarListYear$_YearId]    Script Date: 03/13/2013 15:12:29 ******/
ALTER TABLE [dbo].[$CarList$]  WITH CHECK ADD  CONSTRAINT [FK_$CarList$_$CarListYear$_YearId] FOREIGN KEY([YearId])
REFERENCES [dbo].[$CarListYear$] ([Id])
GO
ALTER TABLE [dbo].[$CarList$] CHECK CONSTRAINT [FK_$CarList$_$CarListYear$_YearId]
GO

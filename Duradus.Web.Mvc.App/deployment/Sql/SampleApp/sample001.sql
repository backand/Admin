CREATE TABLE [dbo].[$v_Car_Inventory$](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NULL,
	[Ordinal] [int] NULL,
 CONSTRAINT [PK_$v_Car_Inventory$] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[$PowerTableThisIsListField$]    Script Date: 02/12/2013 12:24:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[$PowerTableThisIsListField$](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NULL,
	[Ordinal] [int] NULL,
 CONSTRAINT [PK_$PowerTableThisIsListField$] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[$PowerTableThisIsListField$] ON
INSERT [dbo].[$PowerTableThisIsListField$] ([Id], [Name], [Ordinal]) VALUES (1, N'Option 1', 10)
INSERT [dbo].[$PowerTableThisIsListField$] ([Id], [Name], [Ordinal]) VALUES (2, N'Option 2', 20)
INSERT [dbo].[$PowerTableThisIsListField$] ([Id], [Name], [Ordinal]) VALUES (3, N'Option 3', 30)
SET IDENTITY_INSERT [dbo].[$PowerTableThisIsListField$] OFF
/****** Object:  Table [dbo].[$PowerTable2This_is_Auto_Complete_Field$]    Script Date: 02/12/2013 12:24:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[$PowerTable2This_is_Auto_Complete_Field$](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NULL,
	[Ordinal] [int] NULL,
 CONSTRAINT [PK_$PowerTable2This_is_Auto_Complete_Field$] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[$PowerTable2This_is_Auto_Complete_Field$] ON
INSERT [dbo].[$PowerTable2This_is_Auto_Complete_Field$] ([Id], [Name], [Ordinal]) VALUES (1, N'Option 1', 10)
INSERT [dbo].[$PowerTable2This_is_Auto_Complete_Field$] ([Id], [Name], [Ordinal]) VALUES (2, N'Option 2', 20)
INSERT [dbo].[$PowerTable2This_is_Auto_Complete_Field$] ([Id], [Name], [Ordinal]) VALUES (3, N'Option 3', 30)
SET IDENTITY_INSERT [dbo].[$PowerTable2This_is_Auto_Complete_Field$] OFF
/****** Object:  Table [dbo].[$Car InventoryYear$]    Script Date: 02/12/2013 12:24:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[$Car InventoryYear$](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NULL,
	[Ordinal] [int] NULL,
 CONSTRAINT [PK_$Car InventoryYear$] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[$Car InventoryYear$] ON
INSERT [dbo].[$Car InventoryYear$] ([Id], [Name], [Ordinal]) VALUES (1, N'2006', 10)
INSERT [dbo].[$Car InventoryYear$] ([Id], [Name], [Ordinal]) VALUES (2, N'2007', 20)
INSERT [dbo].[$Car InventoryYear$] ([Id], [Name], [Ordinal]) VALUES (3, N'2008', 30)
INSERT [dbo].[$Car InventoryYear$] ([Id], [Name], [Ordinal]) VALUES (4, N'2009', 40)
INSERT [dbo].[$Car InventoryYear$] ([Id], [Name], [Ordinal]) VALUES (5, N'2010', 50)
INSERT [dbo].[$Car InventoryYear$] ([Id], [Name], [Ordinal]) VALUES (6, N'2011', 60)
INSERT [dbo].[$Car InventoryYear$] ([Id], [Name], [Ordinal]) VALUES (7, N'2012', 70)
INSERT [dbo].[$Car InventoryYear$] ([Id], [Name], [Ordinal]) VALUES (8, N'2013', 80)
INSERT [dbo].[$Car InventoryYear$] ([Id], [Name], [Ordinal]) VALUES (9, N'Brand New', 90)
SET IDENTITY_INSERT [dbo].[$Car InventoryYear$] OFF
/****** Object:  Table [dbo].[$Car InventoryTransmission$]    Script Date: 02/12/2013 12:24:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[$Car InventoryTransmission$](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NULL,
	[Ordinal] [int] NULL,
 CONSTRAINT [PK_$Car InventoryTransmission$] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[$Car InventoryTransmission$] ON
INSERT [dbo].[$Car InventoryTransmission$] ([Id], [Name], [Ordinal]) VALUES (1, N'Automatic', 10)
INSERT [dbo].[$Car InventoryTransmission$] ([Id], [Name], [Ordinal]) VALUES (2, N'Manual', 20)
SET IDENTITY_INSERT [dbo].[$Car InventoryTransmission$] OFF
/****** Object:  Table [dbo].[$Car InventoryStatus$]    Script Date: 02/12/2013 12:24:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[$Car InventoryStatus$](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NULL,
	[Ordinal] [int] NULL,
 CONSTRAINT [PK_$Car InventoryStatus$] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[$Car InventoryStatus$] ON
INSERT [dbo].[$Car InventoryStatus$] ([Id], [Name], [Ordinal]) VALUES (1, N'New', 10)
INSERT [dbo].[$Car InventoryStatus$] ([Id], [Name], [Ordinal]) VALUES (2, N'Used', 20)
SET IDENTITY_INSERT [dbo].[$Car InventoryStatus$] OFF
/****** Object:  Table [dbo].[$Car InventoryInterior$]    Script Date: 02/12/2013 12:24:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[$Car InventoryInterior$](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NULL,
	[Ordinal] [int] NULL,
 CONSTRAINT [PK_$Car InventoryInterior$] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[$Car InventoryInterior$] ON
INSERT [dbo].[$Car InventoryInterior$] ([Id], [Name], [Ordinal]) VALUES (1, N'Black', 10)
INSERT [dbo].[$Car InventoryInterior$] ([Id], [Name], [Ordinal]) VALUES (2, N'Silver', 20)
SET IDENTITY_INSERT [dbo].[$Car InventoryInterior$] OFF
/****** Object:  Table [dbo].[$Car InventoryExterior$]    Script Date: 02/12/2013 12:24:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[$Car InventoryExterior$](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NULL,
	[Ordinal] [int] NULL,
 CONSTRAINT [PK_$Car InventoryExterior$] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[$Car InventoryExterior$] ON
INSERT [dbo].[$Car InventoryExterior$] ([Id], [Name], [Ordinal]) VALUES (1, N'Silver', 10)
INSERT [dbo].[$Car InventoryExterior$] ([Id], [Name], [Ordinal]) VALUES (2, N'Red', 20)
INSERT [dbo].[$Car InventoryExterior$] ([Id], [Name], [Ordinal]) VALUES (3, N'Black', 30)
SET IDENTITY_INSERT [dbo].[$Car InventoryExterior$] OFF
/****** Object:  Table [dbo].[$Car InventoryCompany$]    Script Date: 02/12/2013 12:24:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[$Car InventoryCompany$](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NULL,
	[Ordinal] [int] NULL,
 CONSTRAINT [PK_$Car InventoryCompany$] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[$Car InventoryCompany$] ON
INSERT [dbo].[$Car InventoryCompany$] ([Id], [Name], [Ordinal]) VALUES (1, N'BMW', 10)
SET IDENTITY_INSERT [dbo].[$Car InventoryCompany$] OFF
/****** Object:  Table [dbo].[$DefaultGrid$]    Script Date: 02/12/2013 12:24:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[$DefaultGrid$](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Field_1] [nvarchar](250) NULL,
	[Field_2] [nvarchar](250) NULL,
	[Field_3] [nvarchar](250) NULL,
	[Field_4] [nvarchar](250) NULL,
	[Field_5] [nvarchar](250) NULL,
 CONSTRAINT [PK_$DefaultGrid$] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[$DefaultGrid$] ON
INSERT [dbo].[$DefaultGrid$] ([Id], [Field_1], [Field_2], [Field_3], [Field_4], [Field_5]) VALUES (1, N'24', N'26', N'32', N'40', N'100')
INSERT [dbo].[$DefaultGrid$] ([Id], [Field_1], [Field_2], [Field_3], [Field_4], [Field_5]) VALUES (2, N'Something', N'Text', N'Other', N'Text', N'What is')
INSERT [dbo].[$DefaultGrid$] ([Id], [Field_1], [Field_2], [Field_3], [Field_4], [Field_5]) VALUES (3, N'Text', N'Text', N'123', N'Something', N'Other')
INSERT [dbo].[$DefaultGrid$] ([Id], [Field_1], [Field_2], [Field_3], [Field_4], [Field_5]) VALUES (4, N'Adi', N'Ben-Noun', N'123456', N'Male', N'1964')
INSERT [dbo].[$DefaultGrid$] ([Id], [Field_1], [Field_2], [Field_3], [Field_4], [Field_5]) VALUES (5, N'Itay', N'Hershkovitz', N'123456', N'Male', N'1970')
SET IDENTITY_INSERT [dbo].[$DefaultGrid$] OFF
/****** Object:  Table [dbo].[$PhotographerHow_Many$]    Script Date: 02/12/2013 12:24:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[$PhotographerHow_Many$](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NULL,
	[Ordinal] [int] NULL,
 CONSTRAINT [PK_$PhotographerHow_Many$] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[$PhotographerHow_Many$] ON
INSERT [dbo].[$PhotographerHow_Many$] ([Id], [Name], [Ordinal]) VALUES (1, N'0', 10)
INSERT [dbo].[$PhotographerHow_Many$] ([Id], [Name], [Ordinal]) VALUES (2, N'1', 20)
INSERT [dbo].[$PhotographerHow_Many$] ([Id], [Name], [Ordinal]) VALUES (3, N'2', 30)
INSERT [dbo].[$PhotographerHow_Many$] ([Id], [Name], [Ordinal]) VALUES (4, N'3', 40)
INSERT [dbo].[$PhotographerHow_Many$] ([Id], [Name], [Ordinal]) VALUES (5, N'4', 50)
INSERT [dbo].[$PhotographerHow_Many$] ([Id], [Name], [Ordinal]) VALUES (6, N'5', 60)
SET IDENTITY_INSERT [dbo].[$PhotographerHow_Many$] OFF
/****** Object:  Table [dbo].[$Photographer$]    Script Date: 02/12/2013 12:24:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[$Photographer$](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Picture] [nvarchar](250) NULL,
	[Description] [nvarchar](250) NULL,
	[Picture_Date] [datetime] NULL,
	[Rank] [nvarchar](250) NULL,
	[How_ManyId] [int] NULL,
 CONSTRAINT [PK_$Photographer$] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[$Photographer$] ON
INSERT [dbo].[$Photographer$] ([Id], [Picture], [Description], [Picture_Date], [Rank], [How_ManyId]) VALUES (1, N'http://static.wix.com/media/41d000_b2bcd9d5839c29669eaba3976f446302.jpg_srz_733_545_85_22_0.50_1.20_0.00_jpg_srz', N'I like it', CAST(0x0000A11C00000000 AS DateTime), N'rewrwerew', 4)
INSERT [dbo].[$Photographer$] ([Id], [Picture], [Description], [Picture_Date], [Rank], [How_ManyId]) VALUES (2, N'http://static.wix.com/media/41d000_776935ee108506e124bb0b50284ceed3.jpg_srz_916_545_85_22_0.50_1.20_0.00_jpg_srz', N'What a cool picture', CAST(0x0000A12B00000000 AS DateTime), N'rewrwerew', 4)
INSERT [dbo].[$Photographer$] ([Id], [Picture], [Description], [Picture_Date], [Rank], [How_ManyId]) VALUES (3, N'http://static.wix.com/media/41d000_4d636a2b076bf37fe994ad5103fcbf9d.jpg_srz_470_540_75_22_0.50_1.20_0.00_jpg_srz', N'Why?', CAST(0x0000A12300000000 AS DateTime), N'rewrwerew', 4)
INSERT [dbo].[$Photographer$] ([Id], [Picture], [Description], [Picture_Date], [Rank], [How_ManyId]) VALUES (4, N'http://static.wix.com/media/41d000_7f86151931bede5af4633a35d61cc432.jpg_400', N'It''s OK', CAST(0x0000A11D00000000 AS DateTime), N'rewrwerew', 4)
SET IDENTITY_INSERT [dbo].[$Photographer$] OFF
/****** Object:  Table [dbo].[$Contact$]    Script Date: 02/12/2013 12:24:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[$Contact$](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NULL,
	[Email] [nvarchar](250) NULL,
	[Subject] [nvarchar](250) NULL,
	[Message] [nvarchar](250) NULL,
	[ItemIdId] [int] NULL,
	[Pic] [nvarchar](250) NULL,
 CONSTRAINT [PK_$Contact$] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[$Contact$] ON
INSERT [dbo].[$Contact$] ([Id], [Name], [Email], [Subject], [Message], [ItemIdId], [Pic]) VALUES (1, N'John Smith', N'johns@backandtest.com', N'BMW 325i', N'When can I see the car?', NULL, N'emp1.png')
INSERT [dbo].[$Contact$] ([Id], [Name], [Email], [Subject], [Message], [ItemIdId], [Pic]) VALUES (2, N'Andrew Cencini', N'andrewc@backandtest.com', N'consulting', N'When can I schedule a meeting ?', NULL, N'emp2.png')
INSERT [dbo].[$Contact$] ([Id], [Name], [Email], [Subject], [Message], [ItemIdId], [Pic]) VALUES (3, N'Jan Kotas', N'jank@backandtest.com', N'house 322 in ads', N'When can I see the house?', NULL, N'emp3.png')
INSERT [dbo].[$Contact$] ([Id], [Name], [Email], [Subject], [Message], [ItemIdId], [Pic]) VALUES (4, N'Mariya Sergienko', N'mariya@backandtest.com', N'car i325', N'When can I see the car?', NULL, N'emp5.png')
INSERT [dbo].[$Contact$] ([Id], [Name], [Email], [Subject], [Message], [ItemIdId], [Pic]) VALUES (5, N'Steven Thorpe', N'steven@backandtest.com', N'consulting', N'When can I schedule a meeting ?', NULL, N'emp4.png')
INSERT [dbo].[$Contact$] ([Id], [Name], [Email], [Subject], [Message], [ItemIdId], [Pic]) VALUES (6, N'Robert Zare', N'robert@backandtest.com', N'house 322 in ads', N'When can I see the house?', NULL, N'emp9.png')
SET IDENTITY_INSERT [dbo].[$Contact$] OFF
/****** Object:  Table [dbo].[Car Inventory]    Script Date: 02/12/2013 12:24:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[$Car Inventory$](
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
 CONSTRAINT [PK_$Car Inventory$] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[$Car Inventory$] ON
INSERT [dbo].[$Car Inventory$] ([Id], [ExteriorId], [InteriorId], [TransmissionId], [Mileage], [YearId], [CompanyId], [Model], [Price], [Description], [imageURL], [StatusId]) VALUES (1, 2, 1, 1, 50000, 1, 1, N'BMW 3-Series 325i Sedan', 12000, N'<font color="#ff0000">Great cal low milage</font>', N'http://static.wix.com/media/84770f_e3ce96a97eab1f5f76d7e65d112216a8.jpg_256', 1)
INSERT [dbo].[$Car Inventory$] ([Id], [ExteriorId], [InteriorId], [TransmissionId], [Mileage], [YearId], [CompanyId], [Model], [Price], [Description], [imageURL], [StatusId]) VALUES (2, 1, 1, 1, 40000, 5, 1, N'BMW 3-Series 327i Sedan', 15000, NULL, N'http://static.wix.com/media/84770f_e04bf00f904ba16a0d0682b6b7839a41.jpg_256', 1)
INSERT [dbo].[$Car Inventory$] ([Id], [ExteriorId], [InteriorId], [TransmissionId], [Mileage], [YearId], [CompanyId], [Model], [Price], [Description], [imageURL], [StatusId]) VALUES (3, 3, 1, 1, 80000, 3, 1, N'BMW 3-Series 325i Sedan', 13000, NULL, N'http://static.wix.com/media/84770f_b60c1bfdf5d89bc52ff82d1687229d13.jpg_256', 2)
SET IDENTITY_INSERT [dbo].[$Car Inventory$] OFF
/****** Object:  Table [dbo].[PowerTable2]    Script Date: 02/12/2013 12:24:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[$PowerTable2$](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[This_is_Email_Field] [nvarchar](250) NULL,
	[This_is_Password_Field] [nvarchar](250) NULL,
	[This_is_Phone_Field] [nvarchar](250) NULL,
	[This_is_Paragraph_Field] [nvarchar](4000) NULL,
	[This_is_Auto_Complete_FieldId] [int] NULL,
	[This_is_Currency_Field] [float] NULL,
	[This_is_Percentage_Field] [float] NULL,
 CONSTRAINT [PK_$PowerTable2$] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[$PowerTable2$] ON
INSERT [dbo].[$PowerTable2$] ([Id], [This_is_Email_Field], [This_is_Password_Field], [This_is_Phone_Field], [This_is_Paragraph_Field], [This_is_Auto_Complete_FieldId], [This_is_Currency_Field], [This_is_Percentage_Field]) VALUES (1, N'my@domain.com', N'******', N'(515) 123-4567', N'I''m a paragraph.
If you want to grow your business and take it to the next level, look no further than Advisor & co.', 1, 1000, 0.2)
INSERT [dbo].[$PowerTable2$] ([Id], [This_is_Email_Field], [This_is_Password_Field], [This_is_Phone_Field], [This_is_Paragraph_Field], [This_is_Auto_Complete_FieldId], [This_is_Currency_Field], [This_is_Percentage_Field]) VALUES (2, N'my@domain.com', N'******', N'(515) 123-4567', N'I''m a paragraph.
If you want to grow your business and take it to the next level, look no further than Advisor & co.', 2, 2000, 0.5)
INSERT [dbo].[$PowerTable2$] ([Id], [This_is_Email_Field], [This_is_Password_Field], [This_is_Phone_Field], [This_is_Paragraph_Field], [This_is_Auto_Complete_FieldId], [This_is_Currency_Field], [This_is_Percentage_Field]) VALUES (3, N'my@domain.com', N'******', N'(515) 123-4567', N'I''m a paragraph.
If you want to grow your business and take it to the next level, look no further than Advisor & co.', 3, 3000, 1)
SET IDENTITY_INSERT [dbo].[$PowerTable2$] OFF
/****** Object:  Table [dbo].[PowerTable]    Script Date: 02/12/2013 12:24:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[$PowerTable$](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ThisIsTextField] [nvarchar](250) NULL,
	[ThisIsNumberField] [float] NULL,
	[ThisIsTrueFalseField] [bit] NULL,
	[ThisIsDateField] [datetime] NULL,
	[ThisIsListFieldId] [int] NULL,
	[ThisIsPictureField] [nvarchar](250) NULL,
	[az1] [nvarchar](250) NULL,
 CONSTRAINT [PK_$PowerTable$] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[$PowerTable$] ON
INSERT [dbo].[$PowerTable$] ([Id], [ThisIsTextField], [ThisIsNumberField], [ThisIsTrueFalseField], [ThisIsDateField], [ThisIsListFieldId], [ThisIsPictureField], [az1]) VALUES (1, N'This is a text example I', 1000, 1, CAST(0x0000A13900000000 AS DateTime), 1, N'image1.jpg', N'arrows-minimalist.png')
INSERT [dbo].[$PowerTable$] ([Id], [ThisIsTextField], [ThisIsNumberField], [ThisIsTrueFalseField], [ThisIsDateField], [ThisIsListFieldId], [ThisIsPictureField], [az1]) VALUES (2, N'This is a text example II', 2000, 1, CAST(0x0000A13A00000000 AS DateTime), 2, N'image2.jpg', N'headmenubutton.png')
INSERT [dbo].[$PowerTable$] ([Id], [ThisIsTextField], [ThisIsNumberField], [ThisIsTrueFalseField], [ThisIsDateField], [ThisIsListFieldId], [ThisIsPictureField], [az1]) VALUES (3, N'This is a text example III', 3000, 1, CAST(0x0000A13B00000000 AS DateTime), 3, N'image3.jpg', N'backand.png')
SET IDENTITY_INSERT [dbo].[$PowerTable$] OFF
/****** Object:  View [dbo].[v_$DefaultGrid$]    Script Date: 02/12/2013 12:24:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[v_$DefaultGrid$] AS select Field_5,Field_4,Field_3,Field_2,Field_1,Id from [$DefaultGrid$]
GO
/****** Object:  View [dbo].[v_$PowerTable2$]    Script Date: 02/12/2013 12:24:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[v_$PowerTable2$] AS select This_is_Percentage_Field,This_is_Currency_Field,This_is_Auto_Complete_FieldId, This_is_Paragraph_Field, This_is_Phone_Field, This_is_Password_Field, This_is_Email_Field, Id from [$PowerTable2$]
GO
/****** Object:  View [dbo].[v_$PowerTable$]    Script Date: 02/12/2013 12:24:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[v_$PowerTable$] AS select ThisIsPictureField,ThisIsListFieldId, ThisIsDateField, ThisIsTrueFalseField, ThisIsNumberField, ThisIsTextField, Id, az1 from [$PowerTable$]
GO
/****** Object:  View [dbo].[v_$Photographer$]    Script Date: 02/12/2013 12:24:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[v_$Photographer$] AS select How_ManyId, Rank, Picture_Date, Description, Picture, Id from [$Photographer$]
GO
/****** Object:  View [dbo].[v_$Contact$]    Script Date: 02/12/2013 12:24:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[v_$Contact$] AS select Pic,ItemIdId, Message, Subject, Email, Name, Id from [$Contact$]
GO
/****** Object:  View [dbo].[v_$Car Inventory$]    Script Date: 02/12/2013 12:24:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[v_$Car Inventory$] AS select StatusId, imageURL, Description, Price, Model, CompanyId, YearId, Mileage, TransmissionId, InteriorId, ExteriorId, Id from [$Car Inventory$]
GO
/****** Object:  ForeignKey [FK_$Car Inventory$_$Car InventoryCompany$_CompanyId]    Script Date: 02/12/2013 12:24:25 ******/
ALTER TABLE [dbo].[$Car Inventory$]  WITH CHECK ADD  CONSTRAINT [FK_$Car Inventory$_$Car InventoryCompany$_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[$Car InventoryCompany$] ([Id])
GO
ALTER TABLE [dbo].[$Car Inventory$] CHECK CONSTRAINT [FK_$Car Inventory$_$Car InventoryCompany$_CompanyId]
GO
/****** Object:  ForeignKey [FK_$Car Inventory$_$Car InventoryExterior$_ExteriorId]    Script Date: 02/12/2013 12:24:25 ******/
ALTER TABLE [dbo].[$Car Inventory$]  WITH CHECK ADD  CONSTRAINT [FK_$Car Inventory$_$Car InventoryExterior$_ExteriorId] FOREIGN KEY([ExteriorId])
REFERENCES [dbo].[$Car InventoryExterior$] ([Id])
GO
ALTER TABLE [dbo].[$Car Inventory$] CHECK CONSTRAINT [FK_$Car Inventory$_$Car InventoryExterior$_ExteriorId]
GO
/****** Object:  ForeignKey [FK_Car Inventory_$Car InventoryInterior$_InteriorId]    Script Date: 02/12/2013 12:24:25 ******/
ALTER TABLE [dbo].[$Car Inventory$]  WITH CHECK ADD  CONSTRAINT [FK_$Car Inventory$_$Car InventoryInterior$_InteriorId] FOREIGN KEY([InteriorId])
REFERENCES [dbo].[$Car InventoryInterior$] ([Id])
GO
ALTER TABLE [dbo].[$Car Inventory$] CHECK CONSTRAINT [FK_$Car Inventory$_$Car InventoryInterior$_InteriorId]
GO
/****** Object:  ForeignKey [FK_Car Inventory_$Car InventoryStatus$_StatusId]    Script Date: 02/12/2013 12:24:25 ******/
ALTER TABLE [dbo].[$Car Inventory$]  WITH CHECK ADD  CONSTRAINT [FK_$Car Inventory$_$Car InventoryStatus$_StatusId] FOREIGN KEY([StatusId])
REFERENCES [dbo].[$Car InventoryStatus$] ([Id])
GO
ALTER TABLE [dbo].[$Car Inventory$] CHECK CONSTRAINT [FK_$Car Inventory$_$Car InventoryStatus$_StatusId]
GO
/****** Object:  ForeignKey [FK_Car Inventory_$Car InventoryTransmission$_TransmissionId]    Script Date: 02/12/2013 12:24:25 ******/
ALTER TABLE [dbo].[$Car Inventory$]  WITH CHECK ADD  CONSTRAINT [FK_$Car Inventory$_$Car InventoryTransmission$_TransmissionId] FOREIGN KEY([TransmissionId])
REFERENCES [dbo].[$Car InventoryTransmission$] ([Id])
GO
ALTER TABLE [dbo].[$Car Inventory$] CHECK CONSTRAINT [FK_$Car Inventory$_$Car InventoryTransmission$_TransmissionId]
GO
/****** Object:  ForeignKey [FK_Car Inventory_$Car InventoryYear$_YearId]    Script Date: 02/12/2013 12:24:25 ******/
ALTER TABLE [dbo].[$Car Inventory$]  WITH CHECK ADD  CONSTRAINT [FK_$Car Inventory$_$Car InventoryYear$_YearId] FOREIGN KEY([YearId])
REFERENCES [dbo].[$Car InventoryYear$] ([Id])
GO
ALTER TABLE [dbo].[$Car Inventory$] CHECK CONSTRAINT [FK_$Car Inventory$_$Car InventoryYear$_YearId]
GO
/****** Object:  ForeignKey [FK_$Contact$_$v_Car_Inventory$_ItemIdId]    Script Date: 02/12/2013 12:24:25 ******/
ALTER TABLE [dbo].[$Contact$]  WITH CHECK ADD  CONSTRAINT [FK_$Contact$_$v_Car_Inventory$_ItemIdId] FOREIGN KEY([ItemIdId])
REFERENCES [dbo].[$v_Car_Inventory$] ([Id])
GO
ALTER TABLE [dbo].[$Contact$] CHECK CONSTRAINT [FK_$Contact$_$v_Car_Inventory$_ItemIdId]
GO
/****** Object:  ForeignKey [FK_$Photographer$_$PhotographerHow_Many$_How_ManyId]    Script Date: 02/12/2013 12:24:25 ******/
ALTER TABLE [dbo].[$Photographer$]  WITH CHECK ADD  CONSTRAINT [FK_$Photographer$_$PhotographerHow_Many$_How_ManyId] FOREIGN KEY([How_ManyId])
REFERENCES [dbo].[$PhotographerHow_Many$] ([Id])
GO
ALTER TABLE [dbo].[$Photographer$] CHECK CONSTRAINT [FK_$Photographer$_$PhotographerHow_Many$_How_ManyId]
GO
/****** Object:  ForeignKey [FK_$PowerTable$_$PowerTableThisIsListField$_ThisIsListFieldId]    Script Date: 02/12/2013 12:24:25 ******/
ALTER TABLE [dbo].[$PowerTable$]  WITH CHECK ADD  CONSTRAINT [FK_$PowerTable$_$PowerTableThisIsListField$_ThisIsListFieldId] FOREIGN KEY([ThisIsListFieldId])
REFERENCES [dbo].[$PowerTableThisIsListField$] ([Id])
GO
ALTER TABLE [dbo].[$PowerTable$] CHECK CONSTRAINT [FK_$PowerTable$_$PowerTableThisIsListField$_ThisIsListFieldId]
GO
/****** Object:  ForeignKey [FK_$PowerTable2$_$PowerTable2This_is_Auto_Complete_Field$_This_is_Auto_Complete_FieldId]    Script Date: 02/12/2013 12:24:25 ******/
ALTER TABLE [dbo].[$PowerTable2$]  WITH CHECK ADD  CONSTRAINT [FK_$PowerTable2$_$PowerTable2This_is_Auto_Complete_Field$_This_is_Auto_Complete_FieldId] FOREIGN KEY([This_is_Auto_Complete_FieldId])
REFERENCES [dbo].[$PowerTable2This_is_Auto_Complete_Field$] ([Id])
GO
ALTER TABLE [dbo].[$PowerTable2$] CHECK CONSTRAINT [FK_$PowerTable2$_$PowerTable2This_is_Auto_Complete_Field$_This_is_Auto_Complete_FieldId]
GO

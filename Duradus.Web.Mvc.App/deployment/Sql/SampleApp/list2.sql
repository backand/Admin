CREATE TABLE [dbo].[$ListThis_is_Auto_Complete_Field$](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NULL,
	[Ordinal] [int] NULL,
 CONSTRAINT [PK_$ListThis_is_Auto_Complete_Field$] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[$ListThis_is_Auto_Complete_Field$] ON
INSERT [dbo].[$ListThis_is_Auto_Complete_Field$] ([Id], [Name], [Ordinal]) VALUES (1, N'Option 1 text example', 10)
INSERT [dbo].[$ListThis_is_Auto_Complete_Field$] ([Id], [Name], [Ordinal]) VALUES (2, N'Option 2 text example', 20)
INSERT [dbo].[$ListThis_is_Auto_Complete_Field$] ([Id], [Name], [Ordinal]) VALUES (3, N'Option 3 text example', 30)
INSERT [dbo].[$ListThis_is_Auto_Complete_Field$] ([Id], [Name], [Ordinal]) VALUES (4, N'Option 4 text example', 40)
INSERT [dbo].[$ListThis_is_Auto_Complete_Field$] ([Id], [Name], [Ordinal]) VALUES (5, N'Option 5 text example', 50)
INSERT [dbo].[$ListThis_is_Auto_Complete_Field$] ([Id], [Name], [Ordinal]) VALUES (6, N'Option 6 text example', 60)
INSERT [dbo].[$ListThis_is_Auto_Complete_Field$] ([Id], [Name], [Ordinal]) VALUES (7, N'Option 7 text example', 70)
INSERT [dbo].[$ListThis_is_Auto_Complete_Field$] ([Id], [Name], [Ordinal]) VALUES (8, N'Option 8 text example', 80)
INSERT [dbo].[$ListThis_is_Auto_Complete_Field$] ([Id], [Name], [Ordinal]) VALUES (9, N'Option 9 text example', 90)
INSERT [dbo].[$ListThis_is_Auto_Complete_Field$] ([Id], [Name], [Ordinal]) VALUES (10, N'Option 10 text example', 100)
INSERT [dbo].[$ListThis_is_Auto_Complete_Field$] ([Id], [Name], [Ordinal]) VALUES (11, N'Option 11 text example', 110)
INSERT [dbo].[$ListThis_is_Auto_Complete_Field$] ([Id], [Name], [Ordinal]) VALUES (12, N'Option 12 text example', 120)
INSERT [dbo].[$ListThis_is_Auto_Complete_Field$] ([Id], [Name], [Ordinal]) VALUES (13, N'Option 13 text example', 130)
INSERT [dbo].[$ListThis_is_Auto_Complete_Field$] ([Id], [Name], [Ordinal]) VALUES (14, N'Option 14 text example', 140)
INSERT [dbo].[$ListThis_is_Auto_Complete_Field$] ([Id], [Name], [Ordinal]) VALUES (15, N'Option 15 text example', 150)
INSERT [dbo].[$ListThis_is_Auto_Complete_Field$] ([Id], [Name], [Ordinal]) VALUES (16, N'Option 16 text example', 160)
INSERT [dbo].[$ListThis_is_Auto_Complete_Field$] ([Id], [Name], [Ordinal]) VALUES (17, N'Option 17 text example', 170)
INSERT [dbo].[$ListThis_is_Auto_Complete_Field$] ([Id], [Name], [Ordinal]) VALUES (18, N'Option 18 text example', 180)
SET IDENTITY_INSERT [dbo].[$ListThis_is_Auto_Complete_Field$] OFF
/****** Object:  Table [dbo].[$List$]    Script Date: 03/13/2013 15:06:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[$List$](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[This_is_Email_Field] [nvarchar](250) NULL,
	[This_is_Currency_Field] [float] NULL,
	[This_is_Percentage_Field] [float] NULL,
	[This_is_Phone_Field] [nvarchar](250) NULL,
	[This_is_Paragraph_Field] [nvarchar](4000) NULL,
	[This_is_Auto_Complete_FieldId] [int] NULL,
 CONSTRAINT [PK_$List$] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[$List$] ON
INSERT [dbo].[$List$] ([Id], [This_is_Email_Field], [This_is_Currency_Field], [This_is_Percentage_Field], [This_is_Phone_Field], [This_is_Paragraph_Field], [This_is_Auto_Complete_FieldId]) VALUES (1, N'name@domain.com', 1000, 1, N'(555) 333-5551', N'<b>I''m a paragraph</b>. If you want to grow your business and take it to the next level, look no further than Advisor &amp; co.', 1)
INSERT [dbo].[$List$] ([Id], [This_is_Email_Field], [This_is_Currency_Field], [This_is_Percentage_Field], [This_is_Phone_Field], [This_is_Paragraph_Field], [This_is_Auto_Complete_FieldId]) VALUES (2, N'name@domain.com', 2000, 0.8, N'(555) 333-5552', N'<b>I''m a paragraph</b>. If you want to grow your business and take it to the next level, look no further than Advisor &amp; co.', 2)
INSERT [dbo].[$List$] ([Id], [This_is_Email_Field], [This_is_Currency_Field], [This_is_Percentage_Field], [This_is_Phone_Field], [This_is_Paragraph_Field], [This_is_Auto_Complete_FieldId]) VALUES (3, N'name@domain.com', 3000, 0.9, N'(555) 333-5553', N'<b>I''m a paragraph</b>. If you want to grow your business and take it to the next level, look no further than Advisor &amp; co.', 3)
INSERT [dbo].[$List$] ([Id], [This_is_Email_Field], [This_is_Currency_Field], [This_is_Percentage_Field], [This_is_Phone_Field], [This_is_Paragraph_Field], [This_is_Auto_Complete_FieldId]) VALUES (4, N'name4@domain.com', 4000, 0.2, N'(555) 333-5554', N'<b>I''m a paragraph</b>. If you want to grow your business and take it to the next level, look no further than Advisor &amp; co.', 4)
INSERT [dbo].[$List$] ([Id], [This_is_Email_Field], [This_is_Currency_Field], [This_is_Percentage_Field], [This_is_Phone_Field], [This_is_Paragraph_Field], [This_is_Auto_Complete_FieldId]) VALUES (5, N'name5@domain.com', 5000, 0.8, N'(555) 333-5555', N'<b>I''m a paragraph</b>. If you want to grow your business and take it to the next level, look no further than Advisor &amp; co.', 5)
INSERT [dbo].[$List$] ([Id], [This_is_Email_Field], [This_is_Currency_Field], [This_is_Percentage_Field], [This_is_Phone_Field], [This_is_Paragraph_Field], [This_is_Auto_Complete_FieldId]) VALUES (6, N'name6@domain.com', 6000, 1, N'(555) 333-5556', N'<b>I''m a paragraph</b>. If you want to grow your business and take it to the next level, look no further than Advisor &amp; co.', 6)
INSERT [dbo].[$List$] ([Id], [This_is_Email_Field], [This_is_Currency_Field], [This_is_Percentage_Field], [This_is_Phone_Field], [This_is_Paragraph_Field], [This_is_Auto_Complete_FieldId]) VALUES (7, N'name7@domain.com', 7000, 0.8, N'(555) 333-5557', N'<b>I''m a paragraph</b>. If you want to grow your business and take it to the next level, look no further than Advisor &amp; co.', 7)
INSERT [dbo].[$List$] ([Id], [This_is_Email_Field], [This_is_Currency_Field], [This_is_Percentage_Field], [This_is_Phone_Field], [This_is_Paragraph_Field], [This_is_Auto_Complete_FieldId]) VALUES (8, N'name8@domain.com', 8000, 0.9, N'(555) 333-5558', N'<b>I''m a paragraph</b>. If you want to grow your business and take it to the next level, look no further than Advisor &amp; co.', 8)
INSERT [dbo].[$List$] ([Id], [This_is_Email_Field], [This_is_Currency_Field], [This_is_Percentage_Field], [This_is_Phone_Field], [This_is_Paragraph_Field], [This_is_Auto_Complete_FieldId]) VALUES (9, N'name9@domain.com', 9000, 0.9, N'(555) 333-5559', N'<b>I''m a paragraph</b>. If you want to grow your business and take it to the next level, look no further than Advisor &amp; co.', 9)
INSERT [dbo].[$List$] ([Id], [This_is_Email_Field], [This_is_Currency_Field], [This_is_Percentage_Field], [This_is_Phone_Field], [This_is_Paragraph_Field], [This_is_Auto_Complete_FieldId]) VALUES (10, N'name10@domain.com', 10000, 0.2, N'(555) 333-5560', N'<b>I''m a paragraph</b>. If you want to grow your business and take it to the next level, look no further than Advisor &amp; co.', 10)
INSERT [dbo].[$List$] ([Id], [This_is_Email_Field], [This_is_Currency_Field], [This_is_Percentage_Field], [This_is_Phone_Field], [This_is_Paragraph_Field], [This_is_Auto_Complete_FieldId]) VALUES (11, N'name11@domain.com', 11000, 0.8, N'(555) 333-5561', N'<b>I''m a paragraph</b>. If you want to grow your business and take it to the next level, look no further than Advisor &amp; co.', 11)
INSERT [dbo].[$List$] ([Id], [This_is_Email_Field], [This_is_Currency_Field], [This_is_Percentage_Field], [This_is_Phone_Field], [This_is_Paragraph_Field], [This_is_Auto_Complete_FieldId]) VALUES (12, N'name12@domain.com', 12000, 0.3, N'(555) 333-5562', N'<b>I''m a paragraph</b>. If you want to grow your business and take it to the next level, look no further than Advisor &amp; co.', 12)
INSERT [dbo].[$List$] ([Id], [This_is_Email_Field], [This_is_Currency_Field], [This_is_Percentage_Field], [This_is_Phone_Field], [This_is_Paragraph_Field], [This_is_Auto_Complete_FieldId]) VALUES (13, N'name13@domain.com', 13000, 1, N'(555) 333-5563', N'<b>I''m a paragraph</b>. If you want to grow your business and take it to the next level, look no further than Advisor &amp; co.', 13)
INSERT [dbo].[$List$] ([Id], [This_is_Email_Field], [This_is_Currency_Field], [This_is_Percentage_Field], [This_is_Phone_Field], [This_is_Paragraph_Field], [This_is_Auto_Complete_FieldId]) VALUES (14, N'name14@domain.com', 14000, 0.8, N'(555) 333-5564', N'<b>I''m a paragraph</b>. If you want to grow your business and take it to the next level, look no further than Advisor &amp; co.', 14)
INSERT [dbo].[$List$] ([Id], [This_is_Email_Field], [This_is_Currency_Field], [This_is_Percentage_Field], [This_is_Phone_Field], [This_is_Paragraph_Field], [This_is_Auto_Complete_FieldId]) VALUES (15, N'name15@domain.com', 15000, 0.9, N'(555) 333-5565', N'<b>I''m a paragraph</b>. If you want to grow your business and take it to the next level, look no further than Advisor &amp; co.', 15)
INSERT [dbo].[$List$] ([Id], [This_is_Email_Field], [This_is_Currency_Field], [This_is_Percentage_Field], [This_is_Phone_Field], [This_is_Paragraph_Field], [This_is_Auto_Complete_FieldId]) VALUES (16, N'name16@domain.com', 16000, 1, N'(555) 333-5566', N'<b>I''m a paragraph</b>. If you want to grow your business and take it to the next level, look no further than Advisor &amp; co.', 16)
INSERT [dbo].[$List$] ([Id], [This_is_Email_Field], [This_is_Currency_Field], [This_is_Percentage_Field], [This_is_Phone_Field], [This_is_Paragraph_Field], [This_is_Auto_Complete_FieldId]) VALUES (17, N'name17@domain.com', 17000, 0.8, N'(555) 333-5567', N'<b>I''m a paragraph</b>. If you want to grow your business and take it to the next level, look no further than Advisor &amp; co.', 17)
INSERT [dbo].[$List$] ([Id], [This_is_Email_Field], [This_is_Currency_Field], [This_is_Percentage_Field], [This_is_Phone_Field], [This_is_Paragraph_Field], [This_is_Auto_Complete_FieldId]) VALUES (18, N'name18@domain.com', 18000, 1, N'(555) 333-5568', N'<b>I''m a paragraph</b>. If you want to grow your business and take it to the next level, look no further than Advisor &amp; co.', 18)
SET IDENTITY_INSERT [dbo].[$List$] OFF
/****** Object:  ForeignKey [FK_$List$_$ListThis_is_Auto_Complete_Field$_This_is_Auto_Complete_FieldId]    Script Date: 03/13/2013 15:06:09 ******/
ALTER TABLE [dbo].[$List$]  WITH CHECK ADD  CONSTRAINT [FK_$List$_$ListThis_is_Auto_Complete_Field$_This_is_Auto_Complete_FieldId] FOREIGN KEY([This_is_Auto_Complete_FieldId])
REFERENCES [dbo].[$ListThis_is_Auto_Complete_Field$] ([Id])
GO
ALTER TABLE [dbo].[$List$] CHECK CONSTRAINT [FK_$List$_$ListThis_is_Auto_Complete_Field$_This_is_Auto_Complete_FieldId]
GO

/****** Object:  Table [dbo].[List]    Script Date: 03/05/2013 15:37:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[$List$](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[This_is_Picture_Field] [nvarchar](250) NULL,
	[This_Is_Text_Field] [nvarchar](250) NULL,
	[This_Is_Number_Field] [float] NULL,
	[This_Is_Yes_No_Field] [bit] NULL,
	[This_Is_Date_Field] [datetime] NULL,
	[This_Is_URL_Field] [nvarchar](250) NULL,
 CONSTRAINT [PK_$List$] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[$List$] ON
INSERT [dbo].[$List$] ([Id], [This_is_Picture_Field], [This_Is_Text_Field], [This_Is_Number_Field], [This_Is_Yes_No_Field], [This_Is_Date_Field], [This_Is_URL_Field]) VALUES (1, N'image1.jpg', N'This is a text example I', 1000, 1, CAST(0x0000A17400000000 AS DateTime), N'link to|_blank|http://')
INSERT [dbo].[$List$] ([Id], [This_is_Picture_Field], [This_Is_Text_Field], [This_Is_Number_Field], [This_Is_Yes_No_Field], [This_Is_Date_Field], [This_Is_URL_Field]) VALUES (2, N'image2.png', N'This is a text example 2', 2000, 1, CAST(0x0000A19300000000 AS DateTime), N'link to|_blank|http://')
INSERT [dbo].[$List$] ([Id], [This_is_Picture_Field], [This_Is_Text_Field], [This_Is_Number_Field], [This_Is_Yes_No_Field], [This_Is_Date_Field], [This_Is_URL_Field]) VALUES (3, N'image3.jpg', N'This is a text example 3', 3000, 1, CAST(0x0000A1B100000000 AS DateTime), N'link to|_blank|http://')
INSERT [dbo].[$List$] ([Id], [This_is_Picture_Field], [This_Is_Text_Field], [This_Is_Number_Field], [This_Is_Yes_No_Field], [This_Is_Date_Field], [This_Is_URL_Field]) VALUES (4, N'image3.jpg', N'This is a text example 4', 4000, 1, CAST(0x0000A1B100000000 AS DateTime), N'link to|_blank|http://')
INSERT [dbo].[$List$] ([Id], [This_is_Picture_Field], [This_Is_Text_Field], [This_Is_Number_Field], [This_Is_Yes_No_Field], [This_Is_Date_Field], [This_Is_URL_Field]) VALUES (5, N'image3.jpg', N'This is a text example 5', 5000, 1, CAST(0x0000A1B100000000 AS DateTime), N'link to|_blank|http://')
INSERT [dbo].[$List$] ([Id], [This_is_Picture_Field], [This_Is_Text_Field], [This_Is_Number_Field], [This_Is_Yes_No_Field], [This_Is_Date_Field], [This_Is_URL_Field]) VALUES (6, N'image3.jpg', N'This is a text example 6', 6000, 1, CAST(0x0000A1D000000000 AS DateTime), N'link to|_blank|http://')
INSERT [dbo].[$List$] ([Id], [This_is_Picture_Field], [This_Is_Text_Field], [This_Is_Number_Field], [This_Is_Yes_No_Field], [This_Is_Date_Field], [This_Is_URL_Field]) VALUES (7, N'image3.jpg', N'This is a text example 7', 7000, 1, CAST(0x0000A1EE00000000 AS DateTime), N'link to|_blank|http://')
INSERT [dbo].[$List$] ([Id], [This_is_Picture_Field], [This_Is_Text_Field], [This_Is_Number_Field], [This_Is_Yes_No_Field], [This_Is_Date_Field], [This_Is_URL_Field]) VALUES (8, N'image3.jpg', N'This is a text example 8', 8000, 1, CAST(0x0000A20D00000000 AS DateTime), N'link to|_blank|http://')

SET IDENTITY_INSERT [dbo].[$List$] OFF
GO

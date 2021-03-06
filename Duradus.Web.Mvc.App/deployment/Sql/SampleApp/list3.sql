/****** Object:  Table [dbo].[List]    Script Date: 05/07/2013 19:53:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[$List$](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Column1] [nvarchar](250) NULL,
	[Column2] [nvarchar](250) NULL,
	[Column3] [nvarchar](4000) NULL,
	[Column4] [nvarchar](250) NULL,
 CONSTRAINT [PK_$List$] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[$List$] ON
INSERT [dbo].[$List$] ([Id], [Column1], [Column2], [Column3], [Column4]) VALUES (1, N'Write text here', N'Write text here', N'Here you can let someone else add a long text without interfering your design', N'image1.jpg')
INSERT [dbo].[$List$] ([Id], [Column1], [Column2], [Column3], [Column4]) VALUES (2, N'Write text here', N'Write text here', N'Provide a link and a password to your content editor or client so they can update the content while you focus on design', N'image2.jpg')
INSERT [dbo].[$List$] ([Id], [Column1], [Column2], [Column3], [Column4]) VALUES (3, N'Write text here', N'Write text here', N'You can upload any picture and use it in your list. Columns can contain various types of content: images, links (URL), email, date and more...', N'image3.jpg')
SET IDENTITY_INSERT [dbo].[$List$] OFF


CREATE TABLE [dbo].[Table1](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[c1] [nvarchar](250) NULL,
	[c2] [nvarchar](250) NULL,
 CONSTRAINT [PK_Table1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Table1] ON
INSERT [dbo].[Table1] ([Id], [c1], [c2]) VALUES (1, N'a', N'b')
INSERT [dbo].[Table1] ([Id], [c1], [c2]) VALUES (2, N'4654', N'b2')
INSERT [dbo].[Table1] ([Id], [c1], [c2]) VALUES (3, N'a3', N'b3')
INSERT [dbo].[Table1] ([Id], [c1], [c2]) VALUES (4, N'a4', N'b5')
INSERT [dbo].[Table1] ([Id], [c1], [c2]) VALUES (5, N'a4', N'b4')
SET IDENTITY_INSERT [dbo].[Table1] OFF
/****** Object:  View [dbo].[v_Table1]    Script Date: 02/11/2013 11:13:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[v_Table1] AS select c2,c1,Id from [Table1]
GO

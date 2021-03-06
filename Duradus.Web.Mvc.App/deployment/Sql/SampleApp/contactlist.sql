CREATE TABLE [dbo].[$Contact$](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Picture] [nvarchar](250) NULL,
	[Name] [nvarchar](250) NULL,
	[Email] [nvarchar](250) NULL,
	[Phone] [nvarchar](250) NULL,
	[Comments] [nvarchar](250) NULL,
	[Address] [nvarchar](250) NULL,
 CONSTRAINT [PK_$Contact$] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[$Contact$] ON
INSERT [dbo].[$Contact$] ([Id], [Picture], [Name], [Email], [Phone], [Comments], [Address]) VALUES (1, N'emp4.png', N'John Smith', N'johns@domain.com', N'(555) 111-2222', N'This is a comment text', N'This is an address text')
INSERT [dbo].[$Contact$] ([Id], [Picture], [Name], [Email], [Phone], [Comments], [Address]) VALUES (2, N'emp5.png', N'Andrew Cencini', N'andrewc@domain.com', N'(555) 111-3333', N'This is a comment text', N'This is an address text')
INSERT [dbo].[$Contact$] ([Id], [Picture], [Name], [Email], [Phone], [Comments], [Address]) VALUES (3, N'emp8.png', N'Mariya Sergienko', N'mariya@domain.com', N'(555) 222-4444', N'This is a comment text', N'This is an address text')
INSERT [dbo].[$Contact$] ([Id], [Picture], [Name], [Email], [Phone], [Comments], [Address]) VALUES (4, N'emp2.png', N'Bedecs Anna', N'bedecsa@domain.com', N'(555) 222-4444', N'This is a comment text', N'This is an address text')
INSERT [dbo].[$Contact$] ([Id], [Picture], [Name], [Email], [Phone], [Comments], [Address]) VALUES (5, N'emp5.png', N'Thomas Axen', N'thomasa@domain.com', N'(555) 111-3333', N'This is a comment text', N'This is an address text')
INSERT [dbo].[$Contact$] ([Id], [Picture], [Name], [Email], [Phone], [Comments], [Address]) VALUES (6, N'emp2.png', N'Martin O’Donnell', N'martino@domain.com', N'(555) 111-2222', N'This is a comment text', N'This is an address text')
INSERT [dbo].[$Contact$] ([Id], [Picture], [Name], [Email], [Phone], [Comments], [Address]) VALUES (7, N'emp8.png', N'Christina Lee', N'christinal@domain.com', N'(555) 222-4444', N'This is a comment text', N'This is an address text')
INSERT [dbo].[$Contact$] ([Id], [Picture], [Name], [Email], [Phone], [Comments], [Address]) VALUES (8, N'emp2.png', N'Francisco Pérez-Olaeta', N'franciscop@domain.com', N'(555) 111-2222', N'This is a comment text', N'This is an address text')
INSERT [dbo].[$Contact$] ([Id], [Picture], [Name], [Email], [Phone], [Comments], [Address]) VALUES (9, N'emp4.png', N'John Edwards', N'johne@domain.com', N'(555) 111-2222', N'This is a comment text', N'This is an address text')
INSERT [dbo].[$Contact$] ([Id], [Picture], [Name], [Email], [Phone], [Comments], [Address]) VALUES (10, N'emp5.png', N'Andre Ludick', N'andrel@domain.com', N'(555) 111-3333', N'This is a comment text', N'This is an address text')
INSERT [dbo].[$Contact$] ([Id], [Picture], [Name], [Email], [Phone], [Comments], [Address]) VALUES (11, N'emp8.png', N'Helena Kupkova', N'helenak@domain.com', N'(555) 222-4444', N'This is a comment text', N'This is an address text')
INSERT [dbo].[$Contact$] ([Id], [Picture], [Name], [Email], [Phone], [Comments], [Address]) VALUES (12, N'emp4.png', N'Daniel Goldschmidt', N'danielg@domain.com', N'(555) 111-3333', N'This is a comment text', N'This is an address text')
SET IDENTITY_INSERT [dbo].[$Contact$] OFF

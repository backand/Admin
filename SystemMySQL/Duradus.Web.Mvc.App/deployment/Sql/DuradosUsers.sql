/****** Object:  Table [dbo].[durados_UserRole]    Script Date: 07/19/2011 16:41:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[durados_UserRole](
	[Name] [nvarchar](256) NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
	[FirstView] [nvarchar](200) NULL,
 CONSTRAINT [PK_durados_UserRole] PRIMARY KEY CLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/****** Object:  Table [dbo].[durados_User]    Script Date: 07/19/2011 16:41:14 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[durados_User](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](256) NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[Email] [nvarchar](250) NOT NULL,
	[Password] [nvarchar](20) NULL,
	[Role] [nvarchar](256) NOT NULL,
	[Guid] [uniqueidentifier] NOT NULL,
	[Signature] [nvarchar](4000) NULL,
	[SignatureHTML] [nvarchar](4000) NULL,
	[IsApproved] [bit] NULL,
	[NewUser] [bit] NULL,
	[Comments] [nvarchar](max) NULL,
 CONSTRAINT [PK_durados_User] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[durados_User]  WITH CHECK ADD  CONSTRAINT [FK_User_durados_UserRole] FOREIGN KEY([Role])
REFERENCES [dbo].[durados_UserRole] ([Name])
GO

ALTER TABLE [dbo].[durados_User] CHECK CONSTRAINT [FK_User_durados_UserRole]
GO

ALTER TABLE [dbo].[durados_User] ADD  CONSTRAINT [DF_durados_User_Guid]  DEFAULT (newid()) FOR [Guid]
GO

CREATE UNIQUE NONCLUSTERED INDEX IX_durados_Username ON dbo.durados_User
	(
	Username
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

/****** Object:  View [dbo].[v_User]    Script Date: 07/19/2011 16:41:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[v_durados_User]
AS
SELECT        dbo.[durados_User].ID, dbo.[durados_User].Username, dbo.[durados_User].FirstName, dbo.[durados_User].LastName, dbo.[durados_User].Email, dbo.[durados_User].[Password], dbo.[durados_User].[Role], dbo.[durados_User].[Guid], 
                         dbo.[durados_User].[Signature], dbo.[durados_User].SignatureHTML, IsNull(IsApproved, 1) as IsApproved, dbo.[durados_User].FirstName + ' ' + dbo.[durados_User].LastName AS FullName, 
                         dbo.[durados_User].NewUser, dbo.[durados_User].Comments
FROM            dbo.[durados_User] with (NOLOCK) 
GO

INSERT INTO [dbo].[durados_UserRole] ([Name],[Description]) values ('Developer','Full Control')
INSERT INTO [dbo].[durados_UserRole] ([Name],[Description]) values ('Admin','Power User')
INSERT INTO [dbo].[durados_UserRole] ([Name],[Description]) values ('View Owner','Power user view owner')
INSERT INTO [dbo].[durados_UserRole] ([Name],[Description]) values ('User','User')
INSERT INTO [dbo].[durados_UserRole] ([Name],[Description]) values ('Public','Use for internet public access')
INSERT INTO [dbo].[durados_UserRole] ([Name],[Description]) values ('ReadOnly','Read-Only access')
/****** Object:  Table [dbo].[UserRole]    Script Date: 07/19/2011 16:41:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UserRole](
	[Name] [nvarchar](20) NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
	[FirstView] [nvarchar](200) NULL,
 CONSTRAINT [PK_UserRole] PRIMARY KEY CLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/****** Object:  Table [dbo].[User]    Script Date: 07/19/2011 16:41:14 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[User](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](50) NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[Email] [nvarchar](250) NOT NULL,
	[Password] [nvarchar](20) NULL,
	[Role] [nvarchar](20) NOT NULL,
	[Guid] [uniqueidentifier] NOT NULL,
	[Signature] [nvarchar](4000) NULL,
	[SignatureHTML] [nvarchar](4000) NULL,
	[IsApproved] [bit] NULL,
	[NewUser] [bit] NULL,
	[Comments] [nvarchar](max) NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_UserRole] FOREIGN KEY([Role])
REFERENCES [dbo].[UserRole] ([Name])
GO

ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_UserRole]
GO

ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_Guid]  DEFAULT (newid()) FOR [Guid]
GO

/****** Object:  View [dbo].[v_User]    Script Date: 07/19/2011 16:41:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[v_User]
AS
SELECT        dbo.[User].ID, dbo.[User].Username, dbo.[User].FirstName, dbo.[User].LastName, dbo.[User].Email, dbo.[User].Password, dbo.[User].Role, dbo.[User].Guid, 
                         dbo.[User].Signature, dbo.[User].SignatureHTML, dbo.aspnet_Membership.IsApproved, dbo.[User].FirstName + ' ' + dbo.[User].LastName AS FullName, 
                         dbo.[User].NewUser, dbo.[User].Comments
FROM            dbo.aspnet_Membership INNER JOIN
                         dbo.aspnet_Users ON dbo.aspnet_Membership.UserId = dbo.aspnet_Users.UserId INNER JOIN
                         dbo.[User] ON dbo.aspnet_Users.UserName = dbo.[User].Username

GO
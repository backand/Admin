/****** Object:  Table [dbo].[DistributionList]    Script Date: 09/12/2011 15:33:21 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DistributionList](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_DistributionList] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/****** Object:  Table [dbo].[DistributionListRole]    Script Date: 09/12/2011 15:33:35 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DistributionListRole](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DistributionListId] [int] NOT NULL,
	[Role] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_DistributionListRole] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[DistributionListRole]  WITH CHECK ADD  CONSTRAINT [FK_DistributionListRole_DistributionList] FOREIGN KEY([DistributionListId])
REFERENCES [dbo].[DistributionList] ([Id])
GO

ALTER TABLE [dbo].[DistributionListRole] CHECK CONSTRAINT [FK_DistributionListRole_DistributionList]
GO

ALTER TABLE [dbo].[DistributionListRole]  WITH CHECK ADD  CONSTRAINT [FK_DistributionListRole_UserRole] FOREIGN KEY([Role])
REFERENCES [dbo].[UserRole] ([Name])
GO

ALTER TABLE [dbo].[DistributionListRole] CHECK CONSTRAINT [FK_DistributionListRole_UserRole]
GO

/****** Object:  Table [dbo].[DistributionListUser]    Script Date: 09/12/2011 15:34:14 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DistributionListUser](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DistributionListId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[PointOfContact] [bit] NOT NULL,
 CONSTRAINT [PK_DistributionListUser] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[DistributionListUser]  WITH CHECK ADD  CONSTRAINT [FK_DistributionListUser_DistributionList] FOREIGN KEY([DistributionListId])
REFERENCES [dbo].[DistributionList] ([Id])
GO

ALTER TABLE [dbo].[DistributionListUser] CHECK CONSTRAINT [FK_DistributionListUser_DistributionList]
GO

ALTER TABLE [dbo].[DistributionListUser]  WITH CHECK ADD  CONSTRAINT [FK_DistributionListUser_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([ID])
GO

ALTER TABLE [dbo].[DistributionListUser] CHECK CONSTRAINT [FK_DistributionListUser_User]
GO

ALTER TABLE [dbo].[DistributionListUser] ADD  CONSTRAINT [DF_DistributionListUser_PointOfContact]  DEFAULT ((0)) FOR [PointOfContact]
GO

/****** Object:  View [dbo].[v_DistributionListRoleEmail]    Script Date: 09/12/2011 15:34:48 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[v_DistributionListRoleEmail]
AS
SELECT     dbo.DistributionList.Name, dbo.[User].Email, CONVERT(bit, 0) AS PointOfContact
FROM         dbo.DistributionList WITH (NOLOCK) INNER JOIN
                      dbo.DistributionListRole WITH (NOLOCK) ON dbo.DistributionList.Id = dbo.DistributionListRole.DistributionListId INNER JOIN
                      dbo.[User] WITH (NOLOCK) ON dbo.DistributionListRole.Role = dbo.[User].Role

GO
/****** Object:  View [dbo].[v_DistributionListUserEmail]    Script Date: 09/12/2011 15:35:04 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[v_DistributionListUserEmail]
AS
SELECT     dbo.DistributionList.Name, dbo.[User].Email, dbo.DistributionListUser.PointOfContact
FROM         dbo.DistributionList WITH (NOLOCK) INNER JOIN
                      dbo.DistributionListUser WITH (NOLOCK) ON dbo.DistributionList.Id = dbo.DistributionListUser.DistributionListId INNER JOIN
                      dbo.[User] WITH (NOLOCK) ON dbo.DistributionListUser.UserId = dbo.[User].ID

GO

/****** Object:  View [dbo].[v_DistributionListEmail]    Script Date: 09/12/2011 15:35:22 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[v_DistributionListEmail]
AS
SELECT     *
FROM         dbo.v_DistributionListRoleEmail with (NOLOCK)
UNION
SELECT     *
FROM         dbo.v_DistributionListUserEmail with (NOLOCK)

GO



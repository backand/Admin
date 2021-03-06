/****** Object:  StoredProcedure [dbo].[durados_GetExternalAvailableInstance]    Script Date: 12/14/2014 20:14:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[durados_GetExternalAvailableInstance]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[durados_GetExternalAvailableInstance]
@productId nvarchar(150) = 3 --''MySql''
--,@provider nvarchar(150)=''RDS''
AS 
BEGIN

SELECT top(1) SqlConnectionId  FROM durados_ExternaInstance WITH(NOLOCK) INNER JOIN durados_SqlConnection WITH(NOLOCK) on durados_SqlConnection.Id = durados_ExternaInstance.SqlConnectionId
WHERE durados_SqlConnection.SqlProductId =@productId
 --AND Provider=@provider
 AND IsActive=1


END' 
END
GO
/****** Object:  Table [dbo].[durados_ExternaInstance]    Script Date: 12/14/2014 20:14:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[durados_ExternaInstance]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[durados_ExternaInstance](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InstanceName] [nvarchar](500) NULL,
	[DbName] [nvarchar](250) NULL,
	[IsActive] [bit] NULL,
	[Provider] [nvarchar](250) NULL,
	[Endpoint] [nvarchar](500) NULL,
	[SequrityGroup] [nvarchar](250) NULL,
	[Region] [nvarchar](250) NULL,
	[Version] [nvarchar](250) NULL,
	[Capacity] [int] NULL,
	[SqlConnectionId] [int] NULL,
 CONSTRAINT [PK__durados___3213E83FFD4E23C1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET IDENTITY_INSERT [dbo].[durados_ExternaInstance] ON 

--INSERT [dbo].[durados_ExternaInstance] ([Id], [InstanceName], [DbName], [IsActive], [Provider], [Endpoint], [SequrityGroup], [Region], [Version], [Capacity], [SqlConnectionId]) VALUES (1, N'yrv-dev', N'backandRDS_dev', 1, N'RDS', N'yrv-dev.czvbzzd4kpof.eu-central-1.rds.amazonaws.com', NULL, NULL, NULL, NULL, 5113)
SET IDENTITY_INSERT [dbo].[durados_ExternaInstance] OFF
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_durados_ExternaInstance_durados_SqlConnection]') AND parent_object_id = OBJECT_ID(N'[dbo].[durados_ExternaInstance]'))
ALTER TABLE [dbo].[durados_ExternaInstance]  WITH CHECK ADD  CONSTRAINT [FK_durados_ExternaInstance_durados_SqlConnection] FOREIGN KEY([SqlConnectionId])
REFERENCES [dbo].[durados_SqlConnection] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_durados_ExternaInstance_durados_SqlConnection]') AND parent_object_id = OBJECT_ID(N'[dbo].[durados_ExternaInstance]'))
ALTER TABLE [dbo].[durados_ExternaInstance] CHECK CONSTRAINT [FK_durados_ExternaInstance_durados_SqlConnection]
GO

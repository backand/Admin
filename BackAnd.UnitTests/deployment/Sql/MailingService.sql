USE [__DB_NAME__]


/****** Object:  Table [dbo].[durados_MailingServiceSubscribers]    Script Date: 08/18/2013 16:14:29 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[durados_MailingServiceSubscribers]') AND type in (N'U'))
DROP TABLE [dbo].[durados_MailingServiceSubscribers]
GO


/****** Object:  Table [dbo].[durados_MailingServiceSubscribers]    Script Date: 08/18/2013 16:14:29 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[durados_MailingServiceSubscribers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PK] [int] NULL,
	[DataAction] [nvarchar](250) NULL,
	[BookmarkId] [int] NULL,
	[Date] [datetime] NULL,
	[UserId] [int] NULL,
	[IsSubscribed] [bit] NULL,
	[Errors] [nvarchar](250) NULL,
	[Email] [nvarchar](150) NULL,
 CONSTRAINT [PK_durados_MailingServiceSubscribers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


/****** Object:  UserDefinedTableType [dbo].[durados_MailingServiceSubscribersType]    Script Date: 08/18/2013 16:37:39 ******/
IF  EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N'durados_MailingServiceSubscribersType' AND ss.name = N'dbo')
DROP TYPE [dbo].[durados_MailingServiceSubscribersType]
GO

/****** Object:  UserDefinedTableType [dbo].[durados_MailingServiceSubscribersType]    Script Date: 08/18/2013 16:37:39 ******/
CREATE TYPE [dbo].[durados_MailingServiceSubscribersType] AS TABLE(
	[PK] [int] NULL,
	[DataAction] [nvarchar](250) NULL,
	[BookmarkId] [int] NULL,
	[Date] [datetime] NULL,
	[UserId] [int] NULL,
	[IsSubscribed] [bit] NULL,
	[Errors] [nvarchar](250) NULL,
	[Email] [nvarchar](150) NULL
)
GO




/****** Object:  Index [IX_durados_MailingServiceSubscribers]    Script Date: 08/18/2013 16:16:44 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[durados_MailingServiceSubscribers]') AND name = N'IX_durados_MailingServiceSubscribers')
DROP INDEX [IX_durados_MailingServiceSubscribers] ON [dbo].[durados_MailingServiceSubscribers] WITH ( ONLINE = OFF )
GO

/****** Object:  Index [IX_durados_MailingServiceSubscribers]    Script Date: 08/18/2013 16:16:45 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_durados_MailingServiceSubscribers] ON [dbo].[durados_MailingServiceSubscribers] 
(
	[DataAction] ASC,
	[Email] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

/****** Object:  StoredProcedure [dbo].[durados_UpdateMailingServiceSubscribers]    Script Date: 08/18/2013 16:36:53 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[durados_UpdateMailingServiceSubscribers]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[durados_UpdateMailingServiceSubscribers]
GO

/****** Object:  StoredProcedure [dbo].[durados_UpdateMailingServiceSubscribers]    Script Date: 08/18/2013 16:36:53 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[durados_UpdateMailingServiceSubscribers]
      -- Add the parameters for the stored procedure here
      @durados_MailingServiceSubscribersType As [dbo].[durados_MailingServiceSubscribersType] Readonly
AS
BEGIN
      -- SET NOCOUNT ON added to prevent extra result sets from
      -- interfering with SELECT statements.
      SET NOCOUNT ON;
     
      UPDATE ms
      SET ms.IsSubscribed = t.IsSubscribed,ms.Errors=t.Errors
      FROM durados_MailingServiceSubscribers ms INNER JOIN @durados_MailingServiceSubscribersType t ON (t.DataAction = ms.DataAction and t.Email = ms.Email)
 
    -- Insert statements for procedure here
      Insert Into durados_MailingServiceSubscribers(PK, DataAction, BookmarkId, [Date], UserId, IsSubscribed, Errors, Email)
      Select t.PK, t.DataAction, t.BookmarkId, t.[Date], t.UserId, t.IsSubscribed, t.Errors, t.Email From @durados_MailingServiceSubscribersType t left join durados_MailingServiceSubscribers ms on (t.DataAction = ms.DataAction and t.Email = ms.Email) where ms.Id is null
END
 
GO



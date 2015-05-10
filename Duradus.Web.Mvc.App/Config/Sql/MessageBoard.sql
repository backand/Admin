USE [__DB_NAME__]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON


IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_durados_MessageBoard_CreatedDate]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[durados_MessageBoard] DROP CONSTRAINT [DF_durados_MessageBoard_CreatedDate]
END


/****** Object:  Table [dbo].[durados_MessageBoard]    Script Date: 11/06/2011 13:53:01 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[durados_MessageBoard]') AND type in (N'U'))
DROP TABLE [dbo].[durados_MessageBoard]

/****** Object:  Table [dbo].[durados_MessageBoard]    Script Date: 11/06/2011 13:53:03 ******/

CREATE TABLE [dbo].[durados_MessageBoard](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Subject] [nvarchar](250) NULL,
	[Message] [nvarchar](max) NULL,
	[OriginatedUserId] [int] NULL,
	[ViewName] [nvarchar](250) NULL,
	[ViewDisplayName] [nvarchar](250) NULL,
	[PK] [nvarchar](250) NULL,
	[RowDisplayName] [nvarchar](250) NULL,
	[CreatedDate] [datetime] NULL,
	[RowLink] [nvarchar](350) NULL,
	[ViewLink] [nvarchar](350) NULL,
 CONSTRAINT [PK_durados_MessageBoard] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]


ALTER TABLE [dbo].[durados_MessageBoard] ADD  CONSTRAINT [DF_durados_MessageBoard_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_durados_MessageStatus_durados_MessageBoard]') AND parent_object_id = OBJECT_ID(N'[dbo].[durados_MessageStatus]'))
ALTER TABLE [dbo].[durados_MessageStatus] DROP CONSTRAINT [FK_durados_MessageStatus_durados_MessageBoard]

/****** Object:  Table [dbo].[durados_MessageStatus]    Script Date: 11/06/2011 13:54:09 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[durados_MessageStatus]') AND type in (N'U'))
DROP TABLE [dbo].[durados_MessageStatus]

/****** Object:  Table [dbo].[durados_MessageStatus]    Script Date: 11/06/2011 13:54:11 ******/

CREATE TABLE [dbo].[durados_MessageStatus](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MessageId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[Deleted] [bit] NULL,
	[Reviewed] [bit] NULL,
	[Important] [bit] NULL,
	[ActionRequired] [bit] NULL,
 CONSTRAINT [PK_durados_MessageStatus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]


ALTER TABLE [dbo].[durados_MessageStatus]  WITH CHECK ADD  CONSTRAINT [FK_durados_MessageStatus_durados_MessageBoard] FOREIGN KEY([MessageId])
REFERENCES [dbo].[durados_MessageBoard] ([Id])

ALTER TABLE [dbo].[durados_MessageStatus] CHECK CONSTRAINT [FK_durados_MessageStatus_durados_MessageBoard]


GO


/****** Object:  StoredProcedure [dbo].[Durados_MessageBoard_Action]    Script Date: 11/06/2011 15:58:05 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Durados_MessageBoard_Action]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Durados_MessageBoard_Action]
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Durados_MessageBoard_Action] 
	-- Add the parameters for the stored procedure here
@MessageId int,
@UserId int,
@ActionID int,
@ActionValue bit


AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	declare @StatusId int 
	set @StatusId = null
	
	select @StatusId = Id from dbo.durados_MessageStatus
	where MessageId = @MessageId and UserId = @UserId

	if @StatusId is null
	begin
		if (@ActionId = 1) -- delete
			insert into dbo.durados_MessageStatus (MessageId, UserId, Deleted)
			values (@MessageId, @UserId, @ActionValue)
		else if (@ActionId = 2) -- reviewed
			insert into dbo.durados_MessageStatus (MessageId, UserId, Reviewed)
			values (@MessageId, @UserId, @ActionValue)
		else if (@ActionId = 3) -- important
			insert into dbo.durados_MessageStatus (MessageId, UserId, Important)
			values (@MessageId, @UserId, @ActionValue)
		else if (@ActionId = 4) -- required
			insert into dbo.durados_MessageStatus (MessageId, UserId, ActionRequired)
			values (@MessageId, @UserId, @ActionValue)
	end
	else
	begin
		if (@ActionId = 1) -- delete
			update dbo.durados_MessageStatus
			set Deleted = @ActionValue
			where Id = @StatusId
		else if (@ActionId = 2) -- reviewed
			update dbo.durados_MessageStatus
			set Reviewed = @ActionValue
			where Id = @StatusId
		else if (@ActionId = 3) -- important
			update dbo.durados_MessageStatus
			set Important = @ActionValue
			where Id = @StatusId
		else if (@ActionId = 4) -- required
			update dbo.durados_MessageStatus
			set ActionRequired = @ActionValue
			where Id = @StatusId
	
	end
END

GO




/****** Object:  View [dbo].[durados_v_MessageBoard]    Script Date: 11/02/2011 16:04:15 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[durados_v_MessageBoard]'))
DROP VIEW [dbo].[durados_v_MessageBoard]

GO



CREATE VIEW [dbo].[durados_v_MessageBoard]
AS
SELECT dbo.durados_MessageStatus.Id, dbo.durados_MessageBoard.Subject, dbo.durados_MessageBoard.Message, dbo.durados_MessageBoard.OriginatedUserId, 
                  dbo.durados_MessageBoard.ViewName, dbo.durados_MessageBoard.ViewDisplayName, dbo.durados_MessageBoard.PK, dbo.durados_MessageBoard.RowDisplayName, 
                  dbo.durados_MessageBoard.CreatedDate, dbo.durados_MessageBoard.RowLink, dbo.durados_MessageBoard.ViewLink, dbo.durados_MessageStatus.UserId, 
                  ISNULL(dbo.durados_MessageStatus.Deleted, 0) AS Deleted, ISNULL(dbo.durados_MessageStatus.Reviewed, 0) AS Reviewed, 
                  ISNULL(dbo.durados_MessageStatus.Important, 0) AS Important, ISNULL(dbo.durados_MessageStatus.ActionRequired, 0) AS ActionRequired, 
                  CASE WHEN dbo.durados_MessageStatus.Reviewed = 1 THEN '' ELSE 'Bold' END AS Css
FROM     dbo.durados_MessageBoard INNER JOIN
                  dbo.durados_MessageStatus ON dbo.durados_MessageBoard.Id = dbo.durados_MessageStatus.MessageId
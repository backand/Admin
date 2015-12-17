USE [__DB_NAME__]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[durados_CustomViews](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,	
	[ViewName] [nvarchar](300) NOT NULL,
	[CustomView] [nvarchar](MAX) NOT NULL,
 CONSTRAINT [PK_durados_CustomViews] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_durados_CustomViews] ON [dbo].[durados_CustomViews] 
(
	[UserId] ASC, [ViewName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

GO
CREATE PROCEDURE [dbo].[durados_UpdateUserCustomView] 
@UserId int,
@ViewName nvarchar(300),
@CustomView nvarchar(MAX)

AS
SET transaction isolation level SERIALIZABLE
BEGIN TRANSACTION Upsert
DECLARE @err int, @rowcount int

UPDATE [dbo].[durados_CustomViews] SET [CustomView] = @CustomView WHERE UserId = @UserId AND ViewName = @ViewName

set @rowcount = @@ROWCOUNT
set @err = @@ERROR

IF @err = 0 AND @rowcount = 0
BEGIN
INSERT INTO [dbo].[durados_CustomViews] ([UserId], [ViewName], [CustomView]) VALUES (@UserId, @ViewName, @CustomView)
END
COMMIT TRANSACTION Upsert
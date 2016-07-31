USE [__DB_NAME__]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[durados_WF_Info](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ViewName] [nvarchar](150) NOT NULL,
	[GraphState] [nvarchar](MAX) NOT NULL,
 CONSTRAINT [PK_durados_WF_Info] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_durados_WF_Info] ON [dbo].[durados_WF_Info] 
(
	[ViewName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

GO
CREATE PROCEDURE [dbo].[durados_UpdateWorkflowGraphState] 
@ViewName nvarchar(300),
@State nvarchar(MAX)

AS
SET transaction isolation level SERIALIZABLE
BEGIN TRANSACTION Upsert
DECLARE @err int, @rowcount int

UPDATE [dbo].[durados_WF_Info] SET [GraphState] = @State WHERE ViewName = @ViewName

set @rowcount = @@ROWCOUNT
set @err = @@ERROR

IF @err = 0 AND @rowcount = 0
BEGIN
INSERT INTO [dbo].[durados_WF_Info] ([ViewName], [GraphState]) VALUES (@ViewName,@State)
END
COMMIT TRANSACTION Upsert
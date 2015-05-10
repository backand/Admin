USE [Sandisk_Allegro]
GO
/****** Object:  Trigger [dbo].[UpdateTestFlow]    Script Date: 01/18/2011 10:39:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER TRIGGER [dbo].[UpdateTestFlow]
ON [dbo].[Test]
FOR UPDATE
AS
	IF TRIGGER_NESTLEVEL() > 1
     RETURN
	
	declare @Id bigint = null
	declare @TestFlowId int = null
	declare @PrevTestFlowId int = null
	
	select @Id = Id, @TestFlowId = TestFlowId from inserted
	select @PrevTestFlowId = TestFlowId from deleted
	
	
	if isnull(@TestFlowId, -1) <> isnull(@PrevTestFlowId, -1)
	begin
		
		delete from TestTestTypeCommitDate
		where TestId = @Id
		
		/* create TestTestTypeCommitDate rows from TestFlow template */
		insert into TestTestTypeCommitDate (TestId, TestFlowTypeId)
		SELECT @Id, TestType
		FROM     TestFlowType
		WHERE  TestFlow = @TestFlowId
			
		
	end
	

	
	
	
	

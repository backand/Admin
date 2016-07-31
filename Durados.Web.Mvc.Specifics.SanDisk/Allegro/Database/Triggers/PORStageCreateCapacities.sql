USE [Sandisk_Allegro]
GO
/****** Object:  Trigger [dbo].[PORStageCreateCapacities]    Script Date: 01/18/2011 10:38:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER TRIGGER [dbo].[PORStageCreateCapacities]
ON [dbo].[POR]
FOR UPDATE
AS
	IF TRIGGER_NESTLEVEL() > 1
     RETURN
	
	declare @Id bigint = null
	declare @StageId int = null
	declare @PrevStageId int = null
	
	select @Id = Id, @StageId = StageId from inserted
	select @PrevStageId = StageId from deleted
	
	
	if isnull(@StageId, -1) <> isnull(@PrevStageId, -1) and @StageId = 2 /* 2 : Scoping */
	begin
		
		/* create PORCapacity rows that are not already exists */
		insert into PORCapacity (PORId, CapacityID)
		SELECT @Id, PORProductClassCapacity.ProductCapacityId
		FROM     PORProductClassCapacity LEFT OUTER JOIN
						  PORCapacity ON PORProductClassCapacity.PORId = PORCapacity.PORId AND PORProductClassCapacity.ProductCapacityId = PORCapacity.CapacityId
		WHERE  (PORCapacity.CapacityId IS NULL) AND (PORProductClassCapacity.PORId = @Id)
		/*
		SELECT @Id, ProductCapacity.Id
		FROM     ProductCapacity LEFT OUTER JOIN
							  (SELECT Id, PORId, CapacityId
							   FROM      PORCapacity
							   WHERE   (PORId = @Id)) AS p ON ProductCapacity.Id = p.CapacityId
		WHERE  (ProductCapacity.ProductTechnologyId = @ProductTechnologyId) AND (p.Id IS NULL)
		 */
		 
		insert into PMM (PORCapacityId)
		select Id from PORCapacity
		where PORId = @Id
		
		insert into PLM (PORCapacityId)
		select Id from PORCapacity
		where PORId = @Id
		
		insert into Test (PORCapacity)
		select Id from PORCapacity
		where PORId = @Id
		
	end

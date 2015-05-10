USE [Sandisk_Allegro]
GO
/****** Object:  Trigger [dbo].[ChangeTTPStatus]    Script Date: 01/18/2011 10:33:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER TRIGGER [dbo].[ChangeTTPStatus]
ON [dbo].[DateChange]
FOR INSERT
AS
	IF TRIGGER_NESTLEVEL() > 1
     RETURN
	
	declare @DateTypeId int
	declare @PlmId int
	declare @TtpId int
	declare @RequestedDate datetime
	declare @CommittedDate datetime

	select @DateTypeId = [DateTypeId], @PlmId = [DateTypeObjId], @CommittedDate = [Date] from inserted
	
	/* 10 - change made in the PLM committed date */
	if (@DateTypeId = 10)
	begin
		/* get the last requested date for this ttp object */
		SELECT TOP (1) @RequestedDate = v_RequestedDateTTPFromPLM.Date, @TtpId = TTP.Id
		FROM     TTP INNER JOIN
						  PLM ON TTP.PLMId = PLM.Id INNER JOIN
						  v_RequestedDateTTPFromPLM ON TTP.Id = v_RequestedDateTTPFromPLM.DateTypeObjId
		WHERE  (PLM.Id = @PlmId)
		ORDER BY v_RequestedDateTTPFromPLM.DateCreated DESC
		
		
		/* if committed date is later than requested date then change TTP State to "De-commit POR"  */
		if (@RequestedDate is not null and @CommittedDate is not null and @CommittedDate > @RequestedDate)
		begin
			
			update TTP
			set StateID = 2
			where Id = @TtpId
			
		end
		
	end
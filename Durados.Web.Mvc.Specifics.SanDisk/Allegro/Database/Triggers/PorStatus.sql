ALTER FUNCTION dbo.PorStatus
	(
	-- Add the parameters for the function here
	@create date,
	@required date,
	@committed date,
	@forecast date
	)
RETURNS nvarchar(1)
AS
	BEGIN
	DECLARE @ResultVar nvarchar(1)
	DECLARE @HalfYearInDays int = 180
	DECLARE @MonthInDays int = 30
	DECLARE @HalfMonthInDays int = 14
	DECLARE @WeekInDays int = 7
	
	declare @R nvarchar(1) = 'R';
	declare @Y nvarchar(1) = 'Y';
	declare @G nvarchar(1) = 'G';
	
	select @ResultVar = @G
	
	if @required is not null
	begin
		if datediff(day, getdate(), @required) < @HalfYearInDays
		begin
			if (@committed is not null)
			begin 
				if datediff(day, @required, @committed) > @HalfMonthInDays
				begin
                    select @ResultVar = @R
                    return @ResultVar
                end
                            
			end
			else if @create is not null and datediff(day, @create, getdate()) > @HalfMonthInDays
			begin
                select @ResultVar = @R
                return @ResultVar
			end
			
			if (@forecast is not null)
			begin 
				if datediff(day, @required, @forecast) > @HalfMonthInDays
				begin
                    select @ResultVar = @R
                    return @ResultVar
                end
                            
			end
			else if @create is not null and datediff(day, @create, getdate()) > @HalfMonthInDays
			begin
                select @ResultVar = @R
                return @ResultVar
			end
			
			
			
			if (@committed is not null)
			begin 
				if datediff(day, @required, @committed) > @WeekInDays
				begin
                    select @ResultVar = @Y
                    return @ResultVar
                end
                            
			end
			else if @create is not null and datediff(day, @create, getdate()) > @WeekInDays
			begin
                select @ResultVar = @Y
                return @ResultVar
			end
			
			if (@forecast is not null)
			begin 
				if datediff(day, @required, @forecast) > @WeekInDays
				begin
                    select @ResultVar = @Y
                    return @ResultVar
                end
                            
			end
			else if @create is not null and datediff(day, @create, getdate()) > @WeekInDays
			begin
                select @ResultVar = @Y
                return @ResultVar
			end
		end
		
		else
		begin
			if (@committed is not null)
			begin 
				if datediff(day, @required, @committed) > @MonthInDays
				begin
                    select @ResultVar = @R
                    return @ResultVar
                end
                            
			end
			else if @create is not null and datediff(day, @create, getdate()) > @MonthInDays
			begin
                select @ResultVar = @R
                return @ResultVar
			end
			
			if (@forecast is not null)
			begin 
				if datediff(day, @required, @forecast) > @MonthInDays
				begin
                    select @ResultVar = @R
                    return @ResultVar
                end
                            
			end
			else if @create is not null and datediff(day, @create, getdate()) > @MonthInDays
			begin
                select @ResultVar = @R
                return @ResultVar
			end
			
			
			
			if (@committed is not null)
			begin 
				if datediff(day, @required, @committed) > @HalfMonthInDays
				begin
                    select @ResultVar = @Y
                    return @ResultVar
                end
                            
			end
			else if @create is not null and datediff(day, @create, getdate()) > @HalfMonthInDays
			begin
                select @ResultVar = @Y
                return @ResultVar
			end
			
			if (@forecast is not null)
			begin 
				if datediff(day, @required, @forecast) > @HalfMonthInDays
				begin
                    select @ResultVar = @Y
                    return @ResultVar
                end
                            
			end
			else if @create is not null and datediff(day, @create, getdate()) > @HalfMonthInDays
			begin
                select @ResultVar = @Y
                return @ResultVar
			end
		end
		
	end
	
	RETURN @ResultVar
	END

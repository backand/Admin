USE [__DB_NAME__]

/****** Object:  Table [dbo].[durados_session]    Script Date: 12/01/2011 14:58:12 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[durados_session]') AND type in (N'U'))
DROP TABLE [dbo].[durados_session]

/****** Object:  Table [dbo].[durados_session]    Script Date: 12/01/2011 14:58:19 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[durados_session](
	[SessionID] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[Scalar] [nvarchar](max) NULL,
	[SerializedObject] [ntext] NULL,
	[TypeCode] [nvarchar](50) NULL,
	[ObjectType] [nvarchar](500) NULL,
 CONSTRAINT [PK_Durados_Session] PRIMARY KEY CLUSTERED 
(
	[SessionID] ASC,
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

/****** Object:  StoredProcedure [dbo].[durados_setsession]    Script Date: 12/01/2011 14:58:48 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[durados_setsession]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[durados_setsession]
GO

/****** Object:  StoredProcedure [dbo].[durados_setsession]    Script Date: 12/01/2011 14:58:48 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[durados_setsession]
	
	(
	@Name nvarchar(250) = null,
	@SessionID nvarchar(50) = null,
	@Scalar nvarchar(max) = null,
	@TypeCode nvarchar(50) = null
	)
	
AS
	DECLARE @isExist nvarchar(250)
	
	select @isExist = [Name] from durados_session with(nolock) where SessionID=@SessionID  and [Name]=@Name
	
	IF @isExist IS NOT NULL 
	BEGIN
        update durados_session set Scalar=@Scalar, TypeCode=@TypeCode, SerializedObject=null, ObjectType=null where SessionID=@SessionID and [Name]=@Name
	end
	else
	begin
		insert into durados_session(SessionID, [Name], Scalar, TypeCode) values (@SessionID, @Name, @Scalar, @TypeCode)

	end
GO



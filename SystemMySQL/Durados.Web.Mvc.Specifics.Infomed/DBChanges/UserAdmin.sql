USE [Webteb]
GO

/****** Object:  View [dbo].[tbl_AdminUsers_New]    Script Date: 10/27/2011 12:09:53 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[tbl_AdminUsers_New]'))
DROP VIEW [dbo].[tbl_AdminUsers_New]
GO

create view [dbo].[tbl_AdminUsers_New]
as
select id as UserID,FullName as Name, [Username], [Password], 100 as PermissionLevel, 'Admin' as AdminType
from dbo.v_User
GO

set identity_insert [dbo].[User] on
go

INSERT INTO [dbo].[User]
           ([ID]
           ,[Username]
           ,[FirstName]
           ,[LastName]
           ,[Email]
           ,[Password]
           ,[Role]
           ,[IsApproved]
           ,[NewUser])
     select UserID, Username, Name, '', 'durados@durados.com', [Password], 'Admin', 0, 1 from tbl_AdminUsers
go
     
set identity_insert [dbo].[User] off

GO

exec sp_rename 'tbl_AdminUsers', 'tbl_AdminUsers_old'
exec sp_rename 'tbl_AdminUsers_New', 'tbl_AdminUsers'

go

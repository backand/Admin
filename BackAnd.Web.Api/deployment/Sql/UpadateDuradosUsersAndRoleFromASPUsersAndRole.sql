declare @MSDBName as varchar(50) = 'glowner'
declare @DuradosDBName as varchar(50) = 'littlebrown'
declare @DuradosUser as varchar(50) = 'durados_User'
declare @DuradosRole as varchar(50) = 'UserRole'

declare @query as varchar(Max)
begin tran A
set @query=
'INSERT INTO '+@DuradosDBName+'.dbo.'+@DuradosRole+'(Name,[Description])
SELECT roleName,'''' FROM '+@MSDBName+'.dbo.aspnet_Roles WHERE roleName NOT IN(SELECT name FROM '+@DuradosDBName+'.dbo.'+@DuradosRole+')'
--print @query
EXEC (@query)

set @query=
'INSERT INTO '+@DuradosDBName+'.dbo.'+@DuradosUser+'([Username],[FirstName],[LastName],[Email],[Role],[Guid],[IsApproved],[NewUser])
SELECT USERNAME,USERNAME ,'''',EMAIL,isnull((SELECT TOP(1) Rolename from '+@MSDBName+'.dbo.aspnet_Roles INNER JOIN  '+@MSDBName+'.dbo.aspnet_UsersInRoles  on '+@MSDBName+'.dbo.aspnet_Roles.RoleId='+@MSDBName+'.dbo.aspnet_UsersInRoles.RoleId
 where  '+@MSDBName+'.dbo.aspnet_Users.UserId='+@MSDBName+'.dbo.aspnet_UsersInRoles.UserId 
  ),(select top(1) rolename from  '+@MSDBName+'.dbo.aspnet_Roles order by rolename desc)) as [role],NEWID() ,IsApproved,0
FROM '+@MSDBName+'.dbo.aspnet_Membership INNER JOIN '+@MSDBName+'.dbo.aspnet_Users ON '+@MSDBName+'.dbo.aspnet_Membership.UserId = '+@MSDBName+'.dbo.aspnet_Users.UserId 
where  USERNAME  not in(select [Username] from '+@DuradosDBName+'.dbo.'+@DuradosUser+')'
 EXEC (@query) 
--print  @query
 
commit tran A
--rollback tran A

select 'INSERT INTO [dbo].[durados_User] ([Username],[FirstName],[LastName],[Email],[Role],[IsApproved]) VALUES ('''+Username+''','''+FirstName+''','''+LastName+''','''+Email+''','''+[Role]+''',1)' as t from [dbo].[durados_User]           
GO

Select 'INSERT INTO [dbo].[UserRole] ([Name],[Description])  VALUES ('''+[Name]+''','''+[Description]+''')' as t from [dbo].[UserRole]
GO
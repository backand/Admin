use BackAnd_dev




CREATE TABLE [dbo].[durados_Theme] (
    [Id]           INT            NOT NULL,
    [Name]         NVARCHAR (50)  NOT NULL,
    [RelativePath] NVARCHAR (250) NOT NULL,
    CONSTRAINT [PK_durados_Theme] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

ALTER TABLE [dbo].[durados_App] ADD
	ThemeId int NULL
GO

ALTER TABLE [dbo].[durados_App] ADD
	CustomThemePath nvarchar(500) NULL
GO

ALTER TABLE [dbo].[durados_App] WITH NOCHECK
    ADD CONSTRAINT [FK_durados_App_durados_Theme] FOREIGN KEY ([ThemeId]) REFERENCES [dbo].[durados_Theme] ([Id]);


GO

INSERT INTO [dbo].[durados_Theme]
           ([ID],
		   [Name]
           ,[RelativePath])
     VALUES
           (1,'Custom'
           ,'app/index.html')
GO

INSERT INTO [dbo].[durados_Theme]
           ([ID],
		   [Name]
           ,[RelativePath])
     VALUES
           (2,'AdminLTE'
           ,'app/index-lte.html')
GO
INSERT INTO [dbo].[durados_Theme]
           ([ID],
		   [Name]
           ,[RelativePath])
     VALUES
           (3,'DashGum'
           ,'app/index-dashgum.html')
GO
INSERT INTO [dbo].[durados_Theme]
           ([ID],
		   [Name]
           ,[RelativePath])
     VALUES
           (4,'DevOOPS'
           ,'app/index-devoops.html')
GO
/****** Script for SelectTopNRows command from SSMS  ******/

insert  into [dbo].[LabStatus]([Name],[Ordinal]) values ('Request Pending',-10)
insert  into [dbo].[QualStatus]([Name],[Ordinal]) values ('ePlan',15)


SET IDENTITY_INSERT [dbo].[PLMExecutionQUState] ON
INSERT [dbo].[PLMExecutionQUState] ([id], [Name], [Ordinal]) VALUES (1, N'No Request', 10)
INSERT [dbo].[PLMExecutionQUState] ([id], [Name], [Ordinal]) VALUES (2, N'Request', 20)
INSERT [dbo].[PLMExecutionQUState] ([id], [Name], [Ordinal]) VALUES (3, N'Commit', 30)
INSERT [dbo].[PLMExecutionQUState] ([id], [Name], [Ordinal]) VALUES (4, N'Arrived', 40)
SET IDENTITY_INSERT [dbo].[PLMExecutionQUState] OFF
/****** Object:  Table [dbo].[PLMExecutionFWState]    Script Date: 02/06/2012 21:16:48 ******/
SET IDENTITY_INSERT [dbo].[PLMExecutionFWState] ON
INSERT [dbo].[PLMExecutionFWState] ([id], [Name], [Ordinal]) VALUES (1, N'No Request', 10)
INSERT [dbo].[PLMExecutionFWState] ([id], [Name], [Ordinal]) VALUES (2, N'Request', 20)
INSERT [dbo].[PLMExecutionFWState] ([id], [Name], [Ordinal]) VALUES (3, N'Commit', 30)
INSERT [dbo].[PLMExecutionFWState] ([id], [Name], [Ordinal]) VALUES (3, N'Released', 40)

SET IDENTITY_INSERT [dbo].[PLMExecutionFWState] OFF
--DO NOT Run ON Sandisk Production or QA
SET IDENTITY_INSERT [dbo].[LabStatus] ON
INSERT [dbo].[LabStatus] ([Id], [Name], [Ordinal]) VALUES (1, N'No Request', 10)
INSERT [dbo].[LabStatus] ([Id], [Name], [Ordinal]) VALUES (2, N'Request Pending', 20)
INSERT [dbo].[LabStatus] ([Id], [Name], [Ordinal]) VALUES (3, N'Awaiting Cards', 30)
INSERT [dbo].[LabStatus] ([Id], [Name], [Ordinal]) VALUES (4, N'Running', 40)
INSERT [dbo].[LabStatus] ([Id], [Name], [Ordinal]) VALUES (5, N'Request Stop', 50)
INSERT [dbo].[LabStatus] ([Id], [Name], [Ordinal]) VALUES (6, N'Stop', 60)
INSERT [dbo].[LabStatus] ([Id], [Name], [Ordinal]) VALUES (7, N'Reporting', 70)
INSERT [dbo].[LabStatus] ([Id], [Name], [Ordinal]) VALUES (8, N'Done', 80)
SET IDENTITY_INSERT [dbo].[LabStatus] OFF


update dbo.PLMExecution set ConfigurationCardsNumber=750 where ConfigurationCardsNumber is null 
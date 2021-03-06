USE [Logs]

/****** Object:  StoredProcedure [dbo].[LogClear]    Script Date: 06/25/2010 17:54:27 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[Durados_Log](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ApplicationName] [nvarchar](250) NULL,
	[Username] [nvarchar](250) NULL,
	[MachineName] [nvarchar](250) NULL,
	[Time] [datetime] NULL,
	[Controller] [nvarchar](250) NULL,
	[Action] [nvarchar](250) NULL,
	[MethodName] [nvarchar](250) NULL,
	[LogType] [int] NULL,
	[ExceptionMessage] [nvarchar](max) NULL,
	[Trace] [nvarchar](max) NULL,
	[FreeText] [nvarchar](max) NULL,
	[Guid] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Durados_Log] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]


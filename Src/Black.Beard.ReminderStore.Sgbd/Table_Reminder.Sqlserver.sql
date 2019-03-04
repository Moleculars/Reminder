USE [Reminder]
GO

/****** Object:  Table [dbo].[Reminders]    Script Date: 04/03/2019 19:16:44 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Reminders](
	[_id] [bigint] IDENTITY(1,1) NOT NULL,
	[Uuid] [uniqueidentifier] NOT NULL,
	[Binding] [varchar](50) NOT NULL,
	[Address] [varchar](2048) NOT NULL,
	[Message] [varchar](max) NOT NULL,
	[Expire] [datetimeoffset](7) NOT NULL,
	[Resolved] [bit] NOT NULL,
 CONSTRAINT [PK_Reminders] PRIMARY KEY CLUSTERED 
(
	[_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Index [UuidNoUniqueIndex]    Script Date: 04/03/2019 19:19:46 ******/
CREATE NONCLUSTERED INDEX [UuidNoUniqueIndex] ON [dbo].[Reminders]
(
	[Uuid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

/****** Object:  Index [ExpiredItemsIndex]    Script Date: 04/03/2019 19:20:18 ******/
CREATE NONCLUSTERED INDEX [ExpiredItemsIndex] ON [dbo].[Reminders]
(
	[Expire] ASC,
	[Resolved] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO



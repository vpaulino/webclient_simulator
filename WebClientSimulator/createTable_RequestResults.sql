USE [Samples]
GO

/****** Object:  Table [dbo].[RequestResults]    Script Date: 11/11/2012 21:04:59 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[RequestResults](
	[RequestId] [int] NULL,
	[ClientId] [int] NULL,
	[RequestTime] [timestamp] NULL,
	[HttpStatusCode] [varchar](50) NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO



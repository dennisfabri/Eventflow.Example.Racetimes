USE [TimesEF]
GO

CREATE TABLE [dbo].[EventFlowSnapshots] (
	[AggregateName] [nvarchar](255) NOT NULL,
	[AggregateId] [nvarchar](255) NOT NULL,
	[AggregateSequenceNumber] [int] NOT NULL,
	[Data] [nvarchar](max) NOT NULL,
	[Metadata] [nvarchar](max) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];
GO



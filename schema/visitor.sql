SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[visitor](
  [UserId] [int] NULL,
	[PhoneNumber] [bigint] NOT NULL,
	[FirstName] [varchar](50) NOT NULL,
	[LastName] [varchar](50) NOT NULL,
	[BirthDate] [date] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[visitor] ADD PRIMARY KEY CLUSTERED 
(
	[PhoneNumber] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[visitor]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[zoo_user] ([UserId])
GO

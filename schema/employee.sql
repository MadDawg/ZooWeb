SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[employee](
	[EmployeeId] [int] IDENTITY(1,1) NOT NULL,
	[Phone_num] [varchar](10) NOT NULL,
	[Dno] [smallint] NULL,
	[Super_Eid] [int] NULL,
	[Email] [varchar](50) NOT NULL,
	[FName] [varchar](30) NOT NULL,
	[Lname] [varchar](30) NOT NULL,
	[Salary] [int] NOT NULL,
  [IsEmployed] [bit] NOT NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[employee] ADD PRIMARY KEY CLUSTERED 
(
	[EmployeeId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[employee] ADD  DEFAULT ((1)) FOR [IsEmployed]
GO
SET ANSI_PADDING ON
GO
ALTER TABLE [dbo].[employee] ADD UNIQUE NONCLUSTERED 
(
	[EmployeeId] ASC,
	[Phone_num] ASC,
	[Email] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[employee]  WITH CHECK ADD FOREIGN KEY([Dno])
REFERENCES [dbo].[department] ([Dnumber])
GO

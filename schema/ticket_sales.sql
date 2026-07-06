SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ticket_sales](
	[Ticket_Id] [int] IDENTITY(1,1) NOT NULL,
	[Pass_type] [varchar](10) NOT NULL,
	[Eid] [int] NOT NULL,
	[Visitor_pn] [bigint] NOT NULL,
	[R_date] [date] NOT NULL,
	[R_total] [money] NOT NULL,
  [IsValid] [bit] NOT NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ticket_sales] ADD PRIMARY KEY CLUSTERED 
(
	[Ticket_Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ticket_sales] ADD  DEFAULT (getdate()) FOR [R_date]
GO
ALTER TABLE [dbo].[ticket_sales] ADD  DEFAULT ((1)) FOR [IsValid]
GO
ALTER TABLE [dbo].[ticket_sales]  WITH CHECK ADD FOREIGN KEY([Visitor_pn])
REFERENCES [dbo].[visitor] ([PhoneNumber])
GO
ALTER TABLE [dbo].[ticket_sales]  WITH CHECK ADD FOREIGN KEY([Eid])
REFERENCES [dbo].[employee] ([EmployeeId])
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [dbo].[trgAfterTicketSales]
	ON [dbo].[ticket_sales]
	FOR DELETE, INSERT, UPDATE
	AS
	BEGIN
		SET NOCOUNT ON;

		-- Update for UPDATE actions
		IF EXISTS (SELECT 1 FROM inserted)
		BEGIN
			-- Update Revenue table for inserted/updated records
			UPDATE r
			SET r.Total = i.R_total,
				r.ReceiptSource = 'Ticket Sales',
				r.ReceiptNum = CONCAT('TCKTSALE', FORMAT(i.Ticket_Id, 'd12')),
				r.RevenueDate = i.R_date,
				r.EmployeeId = i.Eid
			FROM dbo.Revenue r
			INNER JOIN inserted i ON CONCAT('TCKTSALE', FORMAT(i.Ticket_Id, 'd12')) = r.ReceiptNum;
		END

		-- Insert for INSERT actions
		IF EXISTS (SELECT 1 FROM inserted) AND NOT EXISTS (SELECT 1 FROM deleted)
		BEGIN
			-- Insert new records into Revenue table for inserted records
			INSERT INTO dbo.Revenue (Total, ReceiptSource, ReceiptNum, RevenueDate, EmployeeId)
			SELECT i.R_total, 'Ticket Sales', CONCAT('TCKTSALE', FORMAT(i.Ticket_Id, 'd12')), i.R_date, i.Eid
			FROM inserted i
			WHERE NOT EXISTS (
				SELECT 1
				FROM dbo.Revenue r
				WHERE CONCAT('TCKTSALE', FORMAT(i.Ticket_Id, 'd12')) = r.ReceiptNum
			);
		END

		-- Delete for DELETE actions
		IF EXISTS (SELECT 1 FROM deleted)
		BEGIN
			-- Remove corresponding records from Revenue table for deleted records
			DELETE FROM r
			FROM dbo.Revenue r
			INNER JOIN deleted d ON CONCAT('TCKTSALE', FORMAT(d.Ticket_Id, 'd12')) = r.ReceiptNum;
		END
	END;
GO
ALTER TABLE [dbo].[ticket_sales] ENABLE TRIGGER [trgAfterTicketSales]
GO

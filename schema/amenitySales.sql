SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[amenitySales](
	[Eid] [int] NULL,
	[LocationID] [int] NOT NULL,
	[SaleType] [varchar](255) NULL,
	[SaleDate] [datetime] NULL,
	[SaleTotal] [money] NULL,
	[SaleId] [bigint] IDENTITY(1,1) NOT NULL,
  [IsValid] [bit] NOT NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[amenitySales] ADD PRIMARY KEY CLUSTERED 
(
	[SaleId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[amenitySales] ADD UNIQUE NONCLUSTERED 
(
	[SaleId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[amenitySales] ADD  DEFAULT (getdate()) FOR [SaleDate]
GO
ALTER TABLE [dbo].[amenitySales] ADD  DEFAULT ((1)) FOR [IsValid]
GO
ALTER TABLE [dbo].[amenitySales]  WITH CHECK ADD FOREIGN KEY([Eid])
REFERENCES [dbo].[employee] ([EmployeeId])
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [dbo].[trgAfterAmenitySales]
	ON [dbo].[amenitySales]
	FOR DELETE, INSERT, UPDATE
	AS
	BEGIN
		SET NOCOUNT ON;

		-- Update for UPDATE actions
		IF EXISTS (SELECT 1 FROM inserted)
		BEGIN
			-- Update Revenue table for inserted/updated records
			UPDATE r
			SET r.Total = i.SaleTotal,
				r.ReceiptSource = 'Amenity Sales',
				r.ReceiptNum = CONCAT('AMENSALE', FORMAT(i.SaleId, 'd12')),
				r.RevenueDate = i.SaleDate
			FROM dbo.Revenue r
			INNER JOIN inserted i ON CONCAT('AMENSALE', FORMAT(i.SaleId, 'd12')) = r.ReceiptNum;
		END

		-- Insert for INSERT actions
		IF EXISTS (SELECT 1 FROM inserted) AND NOT EXISTS (SELECT 1 FROM deleted)
		BEGIN
			-- Insert new records into Revenue table for inserted records
			INSERT INTO dbo.Revenue (Total, ReceiptSource, ReceiptNum, RevenueDate, EmployeeId)
			SELECT i.SaleTotal, 'Amenity Sales', CONCAT('AMENSALE', FORMAT(i.SaleId, 'd12')), i.SaleDate, i.Eid
			FROM inserted i
			WHERE NOT EXISTS (
				SELECT 1
				FROM dbo.Revenue r
				WHERE CONCAT('AMENSALE', FORMAT(i.SaleId, 'd12')) = r.ReceiptNum
			);
		END

		-- Delete for DELETE actions
		IF EXISTS (SELECT 1 FROM deleted)
		BEGIN
			-- Remove corresponding records from Revenue table for deleted records
			DELETE FROM r
			FROM dbo.Revenue r
			INNER JOIN deleted d ON CONCAT('AMENSALE', FORMAT(d.SaleId, 'd12')) = r.ReceiptNum;
		END
	END;
GO
ALTER TABLE [dbo].[amenitySales] ENABLE TRIGGER [trgAfterAmenitySales]
GO

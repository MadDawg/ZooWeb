SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[animal](
	[Animal_ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](32) NOT NULL,
	[Scientific_name] [varchar](64) NOT NULL,
	[Common_name] [varchar](64) NOT NULL,
	[Sex] [bit] NOT NULL,
	[Birth_date] [date] NOT NULL,
	[Status] [nvarchar](4000) NULL,
	[Location_ID] [bigint] NULL,
  [IsDeleted] [bit] NOT NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[animal] ADD PRIMARY KEY CLUSTERED 
(
	[Animal_ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[animal]  WITH CHECK ADD FOREIGN KEY([Location_ID])
REFERENCES [dbo].[enclosure] ([LocationID])
GO
ALTER TABLE [dbo].[animal] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [dbo].[trgUpdateEnclosureOccupantNum]
	ON [dbo].[animal]
	FOR DELETE, INSERT, UPDATE
	AS
	BEGIN
		SET NOCOUNT ON;

		IF EXISTS (SELECT * FROM deleted)
		BEGIN
			-- Update when animal is deleted
			UPDATE enc
			SET enc.Occupant_Num = (
				SELECT COUNT(animal.Animal_ID)
				FROM dbo.animal animal
				WHERE animal.Location_ID = enc.LocationID
			)
			FROM dbo.enclosure enc
			INNER JOIN deleted del ON enc.LocationID = del.Location_ID;
		END

		IF EXISTS (SELECT * FROM inserted)
		BEGIN
			-- Update when animal is inserted or updated
			UPDATE enc
			SET enc.Occupant_Num = (
				SELECT COUNT(animal.Animal_ID)
				FROM dbo.animal animal
				WHERE animal.Location_ID = enc.LocationID
			)
			FROM dbo.enclosure enc
			INNER JOIN inserted ins ON enc.LocationID = ins.Location_ID;
		END
	END
GO
ALTER TABLE [dbo].[animal] ENABLE TRIGGER [trgUpdateEnclosureOccupantNum]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--Semantic constraint: every time an animal's status is updated to transferred, all zookeepers must be notified
CREATE TRIGGER [dbo].[UpdateLocationOnTransfer]
ON [dbo].[animal]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @AnimalID INT;
    DECLARE @NewStatus NVARCHAR(50);
    DECLARE @NotificationPriority SMALLINT = 1;

    SELECT @AnimalID = Animal_ID, @NewStatus = Status
    FROM INSERTED;

    IF UPDATE(Status) AND @NewStatus = 'Transferred'
    BEGIN
        -- Update Location_ID to a default value of 1
        UPDATE a
        SET Location_ID = 1 
        FROM animal a
        INNER JOIN INSERTED i ON a.Animal_ID = i.Animal_ID
        WHERE i.Status = 'Transferred';

        -- Notify all users with the zookeeper role and include timestamp
        INSERT INTO [dbo].[notification] ([Title], [Message], [Recipient], [Timestamp])
        SELECT 'Animal Transfer Notification', 
               'An animal has been transferred. Animal ID: ' + CAST(@AnimalID AS NVARCHAR(50)) + 
               '. Animal is being transferred outside of the facility.', 
               UserId,
               GETDATE()
        FROM [dbo].[zoo_user] 
        WHERE UserRole = 'zookeeper';
    END
END;
GO
ALTER TABLE [dbo].[animal] ENABLE TRIGGER [UpdateLocationOnTransfer]
GO

-----------------------------------------------------------kholood
---------------------------------ShowSubscribers
CREATE PROCEDURE ShowSubscribers
AS
BEGIN
    SELECT *
    FROM [dbo].[subscribers]
END
-----------------------------------AddSubscriber
CREATE PROCEDURE AddSubscriber
    @identification bigint,
    @sub_name NVARCHAR(50),
    @sub_phone NVARCHAR(9)
AS
BEGIN
    DECLARE @Exists BIT;
	SET @Exists=dbo.CheckSubscriberExists(@identification);
	IF(@Exists=0)
	BEGIN
	INSERT INTO [dbo].[subscribers] ([identification_number], 
	[subscriber_name], [subscriber_phone])
    VALUES (@identification,@sub_name,@sub_phone)
	END
END
------------------------------------ChecSubscriberkExists
CREATE FUNCTION CheckSubscriberExists
    (@identification bigint)
RETURNS BIT
AS
BEGIN
    DECLARE @Exists BIT;
    SELECT @Exists = CASE WHEN EXISTS (SELECT 1 FROM [dbo].[subscribers]
	WHERE [identification_number] = @identification) THEN 1 ELSE 0 END;
    RETURN @Exists;
END
------------------------------------SearchSubscriber
CREATE PROCEDURE SearchSubscriber
    @SearchString NVARCHAR(100)
AS
BEGIN
    SELECT *
    FROM [dbo].[subscribers]
    WHERE [subscriber_name] LIKE '%' + @SearchString + '%'
	or [identification_number] LIKE '%' + @SearchString + '%'
END
--------------------------------------
CREATE VIEW SubscriberCountersView AS
SELECT
    s.subscriber_id,
    s.identification_number,
    s.subscriber_name,
    s.subscriber_phone,
    c.counter_number,
	c.counter_type_id,
	c.counter_location,
	c.previse_balance,
    ct.counter_type_name
FROM
    [dbo].[subscribers] s
JOIN
    [dbo].[counters] c ON s.[subscriber_id] = c.subscriber_id
JOIN 
    [dbo].[counter_type] ct ON c.counter_type_id=ct.counter_type_id;
----------------------------------------------------------
CREATE PROCEDURE GetSubscriberDetails
    @SubscriberId INT
AS
BEGIN
    SELECT *
    FROM SubscriberCountersView 
    WHERE subscriber_id = @SubscriberId;
END

---------------------------------------------------------------------------reem
--------------------------------------ÚÑÖ ßá ÇáÝæÇÊíÑ
create PROCEDURE [dbo].[GetInvoicesByCounter]
    @CounterId INT
AS
BEGIN
SET NOCOUNT ON;
    
    SELECT * FROM Invoices WHERE  [counter_id]= @CounterId;
END


---------------------------------------------------------
CREATE VIEW InvoiceCounterView AS
SELECT
ct.[counter_type_name],
ct.[local_comcils],
ct.[meter_rang],
c.[counter_number],
c.counter_id,
c.[counter_location],
nv.[invoice_id],
nv.[from_date],
nv.[to_data],
nv.[balance],
nv.[is_paied],
nv.[WaterValue],
nv.[SanitationFees],
s.[subscriber_name]

FROM
    [dbo].[invoices] nv
JOIN
    [dbo].[counters] c ON nv.[counter_id] = c.[counter_id]
JOIN 
    [dbo].[counter_type] ct ON c.counter_type_id=ct.counter_type_id
JOIN 
    [dbo].[subscribers] s ON c.[subscriber_id]=s.[subscriber_id];

--------------------------------------ÚÑÖ   ÊÝÇÕíá ÝæÇÊíÑ
CREATE PROCEDURE GetInvicDetails
    @InvoicId INT
AS
BEGIN
SET NOCOUNT ON;
    SELECT *
    FROM InvoiceCounterView 
    WHERE  [invoice_id]= @InvoicId;
END

EXEC GetInvicDetails @InvoicId = 1;


---------------------------------------------------
--CREATE PROCEDURE PayInvoice
--    @InvoiceId INT,
--    @InvoiceBalance DECIMAL(18,2),
--    @CounterId INT
--AS
--BEGIN
--    SET NOCOUNT ON;
--    BEGIN TRANSACTION;

--    -- ÊÍÏíË ÇáÝÇÊæÑÉ áÌÚáåÇ ãÏÝæÚÉ
--    UPDATE [dbo].[invoices]
--    SET is_paied = 1
--    WHERE invoice_id = @InvoiceId;

--    -- ÊÍÏíË ÍÓÇÈ ÇáÚÏÇÏ: íÝÊÑÖ Ãä ÇáÚãæÏ ÇáÐí íãËá ÇáÍÓÇÈ ÇáÓÇÈÞ ÇÓãå previous_balance
--    UPDATE [dbo].[counters]
--    SET [previse_balance]= [previse_balance] - @InvoiceBalance
--    WHERE counter_id = @CounterId;

--    COMMIT TRANSACTION;
--END


---------------------------------------------------------------
CREATE FUNCTION fn_IsInvoicePaid
(
    @InvoiceId INT
)
RETURNS BIT
AS
BEGIN
    DECLARE @result BIT;
    
    SELECT @result = is_paied
    FROM dbo.invoices
    WHERE invoice_id = @InvoiceId;
    
    -- ÅÐÇ ßÇäÊ ÇáÞíãÉ NULL (Ãí áã íÊã ÇáÚËæÑ Úáì ÇáÝÇÊæÑÉ) íÊã ÅÑÌÇÚ 0
    RETURN ISNULL(@result, 0);
END
-----------------------------------------------------------------
CREATE PROCEDURE PayInvoice
    @InvoiceId INT,
    @InvoiceBalance DECIMAL(18,2),
    @CounterId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- ÇÓÊÏÚÇÁ ÇáÏÇáÉ fn_IsInvoicePaid ááÊÍÞÞ ããÇ ÅÐÇ ßÇäÊ ÇáÝÇÊæÑÉ ãÏÝæÚÉ
    DECLARE @isPaid BIT;
    SET @isPaid = dbo.fn_IsInvoicePaid(@InvoiceId);

    IF (@isPaid = 1)
    BEGIN
        RAISERROR('ÇáÝÇÊæÑÉ ãÏÝæÚÉ ÈÇáÝÚá.', 16, 1);
        RETURN;
    END

    BEGIN TRANSACTION;

    -- ÊÍÏíË ÇáÝÇÊæÑÉ áÌÚáåÇ ãÏÝæÚÉ
    UPDATE [dbo].[invoices]
    SET is_paied = 1
    WHERE invoice_id = @InvoiceId;

    -- ÊÍÏíË ÍÓÇÈ ÇáÚÏÇÏ: ØÑÍ ÞíãÉ ÇáÝÇÊæÑÉ ãä ÇáÑÕíÏ ÇáÓÇÈÞ
    UPDATE [dbo].[counters]
    SET [previse_balance] = [previse_balance] - @InvoiceBalance
    WHERE counter_id = @CounterId;

    COMMIT TRANSACTION;
END
GO

-------------------------------------------------------------------------------
CREATE FUNCTION dbo.fn_MonthsPassed
(
    @CounterId INT
)
RETURNS INT
AS
BEGIN
    DECLARE @lastReadDate DATE;
    DECLARE @monthsPassed INT;
    DECLARE @now DATETIME = GETDATE();
    DECLARE @dayes INT;

    -- ÇÓÊÑÌÇÚ ÊÇÑíÎ ÂÎÑ ÞÑÇÁÉ ãä ÌÏæá ÇáÚÏÇÏÇÊ
    SELECT @lastReadDate = [date_last_reading]
    FROM dbo.counters
    WHERE counter_id = @CounterId;

    -- ÅÐÇ áã íßä ÊÇÑíÎ ÇáÞÑÇÁÉ ãæÌæÏÇð¡ ÝíõÑÌÚ 0
    IF @lastReadDate IS NULL
        RETURN 0;

    -- ÍÓÇÈ ÇáÝÑÞ ÈÇáÃíÇã ÈÇÓÊÎÏÇã ÇáÎæÇÑÒãíÉ ÇáÊÝÕíáíÉ:
    SET @dayes = CASE 
        WHEN MONTH(@lastReadDate) = MONTH(@now)
            THEN (DAY(@now) - DAY(@lastReadDate)) + 30 * (YEAR(@now) - YEAR(@lastReadDate))
        WHEN MONTH(@lastReadDate) < MONTH(@now)
            THEN 30 * ((MONTH(@now) - MONTH(@lastReadDate)) - 1)
                 + (30 - DAY(@lastReadDate))
                 + DAY(@now)
                 + 30 * (YEAR(@now) - YEAR(@lastReadDate))
        ELSE -- ÍÇáÉ ÚäÏãÇ íßæä ÔåÑ ÊÇÑíÎ ÇáÞÑÇÁÉ ÃßÈÑ ãä ÔåÑ Çáíæã (Ãí ÇäÊÞáäÇ Åáì ÓäÉ ÌÏíÏÉ)
            30 * (((12 - MONTH(@lastReadDate)) + MONTH(@now)) - 1)
            + (30 - DAY(@lastReadDate))
            + DAY(@now)
            + 30 * ((YEAR(@now) - YEAR(@lastReadDate)) - 1)
        END;

    -- ÍÓÇÈ ÚÏÏ ÇáÃÔåÑ ÇáãäÞÖíÉ (ÈÇÝÊÑÇÖ Ãä ßá ÔåÑ íÍÊæí Úáì 30 íæãðÇ)
    SET @monthsPassed = @dayes / 30;

    RETURN @monthsPassed;
END

----------------------------------------------------------------
CREATE FUNCTION dbo.fn_CommercialWaterValue
(
    @PrevReading INT,
    @CurrReading INT
)
RETURNS INT
AS
BEGIN
    DECLARE @Consumption INT = @CurrReading - @PrevReading;
    DECLARE @WaterValue INT = 500 * @Consumption;
    RETURN @WaterValue;
END
GO
----------------------------------------------------------------------
CREATE FUNCTION dbo.fn_CommercialSanitationFees
(
    @PrevReading INT,
    @CurrReading INT
)
RETURNS INT
AS
BEGIN
    DECLARE @WaterValue INT = dbo.fn_CommercialWaterValue(@PrevReading, @CurrReading);
    -- äÍÓÈ äÓÈÉ 70% ãä WaterValueº ãÚ ÇáÚáã Ãäå Ýí ÍÇáÉ ÇáÍÓÇÈÇÊ ÇáÕÍíÍÉ íõÝÖøá ÇÓÊÎÏÇã ÃÑÞÇã ÚÔÑíÉ
    DECLARE @SanitationFees INT = (@WaterValue * 70)/ 100;
    RETURN @SanitationFees;
END
GO
-------------------------------------------------------------------------
CREATE FUNCTION dbo.fn_HomeWaterValue
(
    @PrevReading INT,
    @CurrReading INT
)
RETURNS INT
AS
BEGIN
    DECLARE @MonthlyConsumption INT = @CurrReading - @PrevReading;
    DECLARE @WaterValue INT = 0;
    DECLARE @num INT = 1;
    DECLARE @s INT;

    WHILE (@MonthlyConsumption > 0)
    BEGIN
        IF (@MonthlyConsumption >= 10)
        BEGIN
            SET @WaterValue = @WaterValue + 
                CASE @num
                    WHEN 1 THEN 10 * 100
                    WHEN 2 THEN 10 * 120
                    WHEN 3 THEN 10 * 140
                    WHEN 4 THEN 10 * 160
                    ELSE 0
                END;
            SET @MonthlyConsumption = @MonthlyConsumption - 10;
        END
        ELSE  -- ÈÇÞí ÇÓÊåáÇß ÃÞá ãä 10
        BEGIN
            SET @s = @MonthlyConsumption;
            SET @WaterValue = @WaterValue + 
                CASE @num
                    WHEN 1 THEN @s * 100
                    WHEN 2 THEN @s * 120
                    WHEN 3 THEN @s * 140
                    WHEN 4 THEN @s * 160
                    ELSE 0
                END;
            SET @MonthlyConsumption = 0;
        END
        SET @num = @num + 1;
    END

    RETURN @WaterValue;
END
GO
----------------------------------------------------------
CREATE FUNCTION dbo.fn_HomeSanitationFees
(
    @PrevReading INT,
    @CurrReading INT
)
RETURNS INT
AS
BEGIN
    DECLARE @MonthlyConsumption INT = @CurrReading - @PrevReading;
    DECLARE @SanitationFees INT = 0;
    DECLARE @num INT = 1;
    DECLARE @s INT;

    WHILE (@MonthlyConsumption > 0)
    BEGIN
        IF (@MonthlyConsumption >= 10)
        BEGIN
            SET @SanitationFees = @SanitationFees + 
                CASE @num
                    WHEN 1 THEN 10 * 70
                    WHEN 2 THEN 10 * 84
                    WHEN 3 THEN 10 * 98
                    WHEN 4 THEN 10 * 112
                    ELSE 0
                END;
            SET @MonthlyConsumption = @MonthlyConsumption - 10;
        END
        ELSE  -- ÈÇÞí ÇáÇÓÊåáÇß ÃÞá ãä 10 æáßä ÑÓæã ÇáÕÑÝ ÇáÕÍí ÊõÍÓÈ ßÇãáÉ ßÏÝÚÉ ãä 10
        BEGIN
            -- äÓÊÎÏã ÞíãÉ 10 ááæÍÏÉ ÇáËÇÈÊÉ áÑÓæã ÇáÕÑÝ ÇáÕÍí
            SET @SanitationFees = @SanitationFees + 
                CASE @num
                    WHEN 1 THEN 10 * 70
                    WHEN 2 THEN 10 * 84
                    WHEN 3 THEN 10 * 98
                    WHEN 4 THEN 10 * 112
                    ELSE 0
                END;
            SET @MonthlyConsumption = 0;
        END
        SET @num = @num + 1;
    END

    RETURN @SanitationFees;
END
GO

-----------------------------------------------------------------------------
CREATE PROCEDURE sp_AddInvoice
    @CounterId INT,
    --@SubscriberId INT,
    @PrevReading INT,
    @CurrReading INT
    --@IsCommercial BIT  -- 1 ááÚóÏøÇÏ ÇáÊÌÇÑí¡ 0 ááÚóÏøÇÏ ÇáÓßäí
AS
BEGIN
    SET NOCOUNT ON;

      DECLARE @InvoiceFrom DATE;
    DECLARE @InvoiceTo DATE;
    DECLARE @WaterValue INT;
    DECLARE @localcomcils INT;
    DECLARE @meter_rang INT;
    DECLARE @SanitationFees INT;
    DECLARE @balance float;

    DECLARE @lastReadDate DATE;
    DECLARE @monthsPassed INT;
    DECLARE @counterTypeId INT;

    -- ÇáÍÕæá Úáì ÊÇÑíÎ ÇáÞÑÇÁÉ ÇáÃÎíÑ ááÚÏÇÏ
    SELECT @lastReadDate =[date_last_reading],@counterTypeId=[counter_type_id]  FROM [dbo].[counters] WHERE counter_id = @CounterId;

    -- ÇÓÊÏÚÇÁ ÇáÏÇáÉ fn_MonthsPassed áÍÓÇÈ ÚÏÏ ÇáÃÔåÑ ÇáãäÞÖíÉ
    SET @monthsPassed = dbo.fn_MonthsPassed(@CounterId);

    -- ÇáÊÃßÏ ãä ãÑæÑ ÔåÑ ßÇãá (Ãí @monthsPassed íÌÈ Ãä íßæä ÃßÈÑ Ãæ íÓÇæí 1)
    IF (@monthsPassed < 1)
    BEGIN
        RAISERROR('áÇ íãßä ÅÖÇÝÉ ÝÇÊæÑÉ áÃä ÇáÔåÑ áã íäÊå ÈÚÏ.', 16, 1);
        RETURN;
    END

    -- ÅÚÏÇÏ ÊæÇÑíÎ ÇáÝÇÊæÑÉ ÇáÌÏíÏÉ: ãä ÊÇÑíÎ ÇáÞÑÇÁÉ ÇáÍÇáí Åáì ÈÚÏ ÔåÑ
    SET @InvoiceFrom = @lastReadDate;
    SET @InvoiceTo = DATEADD(MONTH, 1, @lastReadDate);

    -- ÍÓÇÈ Þíã WaterValue æ SanitationFees ÈäÇÁð Úáì äæÚ ÇáÚÏÇÏ
	
    IF (@counterTypeId = 1)
    BEGIN
        SET @WaterValue = dbo.fn_CommercialWaterValue(@PrevReading, @CurrReading);
        SET @SanitationFees = dbo.fn_CommercialSanitationFees(@PrevReading, @CurrReading);
    END
    ELSE
    BEGIN
        SET @WaterValue = dbo.fn_HomeWaterValue(@PrevReading, @CurrReading);
        SET @SanitationFees = dbo.fn_HomeSanitationFees(@PrevReading, @CurrReading);
    END

     set @balance= dbo.fn_balane(@WaterValue,@SanitationFees,@counterTypeId);

    BEGIN TRANSACTION;

    -- ÅÏÑÇÌ ÝÇÊæÑÉ ÌÏíÏÉ (íãßäß ÊÚÏíá ÇáÃÚãÏÉ ÍÓÈ ÈäíÉ ÌÏæá invoices)
    INSERT INTO  [dbo].[invoices]
        (counter_id, from_date, [to_data], balance, is_paied,[WaterValue] , [SanitationFees])
    VALUES 
        (@CounterId,  @InvoiceFrom, @InvoiceTo, @balance, 0, @WaterValue, @SanitationFees);

    -- ÊÍÏíË ÊÇÑíÎ ÇáÞÑÇÁÉ ááÚÏÇÏ ááÝÊÑÉ ÇáÊÇáíÉ
    UPDATE [dbo].[counters]
    SET  [date_last_reading]= @InvoiceTo,[previse_balance]+=@balance
    WHERE counter_id = @CounterId;

    COMMIT TRANSACTION;
END
GO

------------------------------------------------------ÝäßÔä ÍÓÇÈ ÇáãÌãæÚ
CREATE FUNCTION fn_balane
(
    @WaterValue int,
	@SanitationFees int,
	@counterTypeId int
)
RETURNS float
AS
BEGIN
    DECLARE @result float;
    DECLARE @localcomcils int;
    DECLARE @meterRang int;
    
   
    SELECT @localcomcils = [local_comcils],@meterRang=[meter_rang]
    FROM [dbo].[counter_type]
    WHERE [counter_type_id] = @counterTypeId;

    set @result=@SanitationFees+@WaterValue+@localcomcils+@meterRang;
	return @result
END

----------------------------------------------------------------------------------
CREATE FUNCTION GetCounterDates (@CounterId INT)
RETURNS TABLE
AS
RETURN
(
    SELECT 
        [date_last_reading] AS FromDate,
        DATEADD(MONTH, 1, [date_last_reading]) AS ToDate
    FROM [dbo].[counters]
    WHERE [counter_id] = @CounterId
);

-----------------------------------------------------------------
CREATE TRIGGER trg_UpdateLastReadDate
ON [dbo].[invoices]
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE M
    SET  [date_last_reading]= DATEADD(MONTH, 1, M.date_last_reading),[previse_balance]+=I.[balance]
    FROM [dbo].[counters] M
    INNER JOIN inserted I ON M.[counter_id] = I.[counter_id];
END

---------------------------------------------------------------------
create view [GetCounterDatesFirstStage] as 
 SELECT 
        [date_last_reading] AS FromDate,
        DATEADD(MONTH, 1, [date_last_reading]) AS ToDate
		,[counter_id]
    FROM [dbo].[counters]



---------------------------------------------------------------------------
---Counter
------------------------------------GetAllCounters
CREATE PROCEDURE GetAllCounters
AS
BEGIN
    SELECT 
        c.counter_id,
        c.counter_number,
        c.counter_location,
        c.date_last_reading,
        c.previse_balance,
        c.subscriber_id,
        c.counter_type_id,
        ct.counter_type_name
    FROM [dbo].[counters] c
    JOIN [dbo].[counter_type] ct ON c.counter_type_id = ct.counter_type_id
END
GO

------------------------------------AddCounter
CREATE PROCEDURE AddCounter
    @counter_number BIGINT,
    @counter_location NVARCHAR(50),
    @previse_balance FLOAT,
    @subscriber_id INT,
    @counter_type_id INT
AS
BEGIN
    BEGIN TRY
        -- فتح المعاملة
        BEGIN TRANSACTION
        
        -- التحقق من وجود المشترك
        IF NOT EXISTS (SELECT 1 FROM subscribers WHERE subscriber_id = @subscriber_id)
        BEGIN
            RAISERROR('Subscriber does not exist', 16, 1)
            RETURN
        END

        -- التحقق من نوع العداد
        IF NOT EXISTS (SELECT 1 FROM counter_type WHERE counter_type_id = @counter_type_id)
        BEGIN
            RAISERROR('Counter type does not exist', 16, 1)
            RETURN
        END

        -- إضافة العداد مع تعيين تاريخ آخر قراءة إلى اليوم
        INSERT INTO [dbo].[counters] (
            counter_number,
            counter_location,
            date_last_reading,
            previse_balance,
            subscriber_id,
            counter_type_id
        )
        VALUES (
            @counter_number,
            @counter_location,
            GETDATE(),  -- يأخذ تاريخ اليوم تلقائيًا
            @previse_balance,
            @subscriber_id,
            @counter_type_id
        )

        -- إتمام المعاملة
        COMMIT TRANSACTION
    END TRY
    BEGIN CATCH
        -- إذا حدث خطأ، إرجاع المعاملة
        IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRANSACTION
        END

        -- رفع الاستثناء
        -- رفع خطأ عام إذا لم يكن هناك خطأ مخصص
        IF ERROR_NUMBER() <> 0
        BEGIN
            THROW
        END
    END CATCH
END

GO

EXEC AddCounter 
    @counter_number = 123456, 
    @counter_location = 'TestLoc', 
    @date_last_reading = '2024-03-21', 
    @previse_balance = 0, 
    @subscriber_id = 1, 
    @counter_type_id = 2;


------------------------------------GetMonthsSinceLastRead (New Function)
CREATE FUNCTION GetMonthsSinceLastRead
(
    @date_last_reading DATE
)
RETURNS NVARCHAR(50)
AS
BEGIN
    DECLARE @current_date DATE = GETDATE();
    DECLARE @months INT = DATEDIFF(MONTH, @date_last_reading, @current_date);
    DECLARE @days INT = DATEDIFF(DAY, 
                               DATEADD(MONTH, @months, @date_last_reading), 
                               @current_date);

    RETURN CAST(@months AS NVARCHAR) + ' months, ' + CAST(@days AS NVARCHAR) + ' days';
END
GO

------------------------------------Updated GetCounterDetails
CREATE PROCEDURE GetCounterDetails
    @CounterId INT
AS
BEGIN
    SELECT 
        c.counter_id,
        c.counter_number,
        c.counter_location,
        c.date_last_reading,
        dbo.GetMonthsSinceLastRead(c.date_last_reading) AS last_read_duration,
        c.previse_balance,
        s.subscriber_name,
        s.identification_number,
        ct.counter_type_name,
        ct.meter_rang,
        ct.local_comcils
    FROM [dbo].[counters] c
    JOIN [dbo].[subscribers] s ON c.subscriber_id = s.subscriber_id
    JOIN [dbo].[counter_type] ct ON c.counter_type_id = ct.counter_type_id
    WHERE c.counter_id = @CounterId
END

EXEC GetCounterDetails @CounterId = 4;

GO

------------------------------------CheckCounterExists (Function)
CREATE FUNCTION CheckCounterExists
    (@CounterNumber BIGINT)
RETURNS BIT
AS
BEGIN
    DECLARE @Exists BIT = 0
    IF EXISTS (SELECT 1 FROM counters WHERE counter_number = @CounterNumber)
        SET @Exists = 1
    RETURN @Exists
END
GO

------------------------------------UpdateCounter (Recommended)
CREATE PROCEDURE UpdateCounter
    @CounterId INT,
    @counter_location NVARCHAR(50),
    @date_last_reading DATE,
    @previse_balance FLOAT,
    @counter_type_id INT
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION

        UPDATE [dbo].[counters]
        SET 
            counter_location = @counter_location,
            date_last_reading = @date_last_reading,
            previse_balance = @previse_balance,
            counter_type_id = @counter_type_id
        WHERE counter_id = @CounterId

        IF @@ROWCOUNT = 0
        BEGIN
            RAISERROR('Counter not found', 16, 1)
        END

        COMMIT TRANSACTION
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION
        THROW
    END CATCH
END
GO


------------------------------------CounterDetailsView (Recommended)
CREATE VIEW CounterDetailsView AS
SELECT
    c.counter_id,
    c.counter_number,
    c.counter_location,
    c.date_last_reading,
    c.previse_balance,
    s.subscriber_name,
    ct.counter_type_name,
    ct.meter_rang,
    ct.local_comcils
FROM [dbo].[counters] c
JOIN [dbo].[subscribers] s ON c.subscriber_id = s.subscriber_id
JOIN [dbo].[counter_type] ct ON c.counter_type_id = ct.counter_type_id
GO


-------------------------------SearchCounter
CREATE PROCEDURE SearchCounter
    @SearchString NVARCHAR(100)
AS
BEGIN
    SELECT 
        c.counter_id,
        c.counter_number,
        c.counter_location,
        c.date_last_reading,
        c.previse_balance,
        c.subscriber_id,
        c.counter_type_id,
        ct.counter_type_name  -- إضافة العمود من جدول counter_type
    FROM counters c
    JOIN counter_type ct ON c.counter_type_id = ct.counter_type_id  -- الانضمام إلى جدول counter_type
    WHERE c.counter_number LIKE '%' + @SearchString + '%'
       OR c.counter_location LIKE '%' + @SearchString + '%'
       OR CAST(c.counter_id AS NVARCHAR) LIKE '%' + @SearchString + '%'
       OR ct.counter_type_name LIKE '%' + @SearchString + '%'  -- البحث أيضًا في اسم نوع العداد
END

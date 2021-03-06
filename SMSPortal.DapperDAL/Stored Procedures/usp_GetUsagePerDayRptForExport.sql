USE [CassiaBeta]
GO
/****** Object:  StoredProcedure [dbo].[usp_GetUsagePerDayRptForExport]    Script Date: 12/12/2013 PM 5:41:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Rekha
-- Create date: 5th December 2013
-- Description:	Gets Usage Per Day Details for reports
-- =============================================
ALTER PROCEDURE [dbo].[usp_GetUsagePerDayRptForExport]
(	
	@StartDate			nvarchar(30),
	@EndDate			nvarchar(30)	
)
AS
	DECLARE @query nvarchar(4000)	

	SET @query = 'SELECT 
						CAST(mp.DateTime AS DATE) AS Date, 
						Count(*) AS TotalMessagesSentPerDay,
						Sum(NetAmount) AS TotalNetAmountPerDay
					FROM 
						tblMicroPayment mp
					WHERE
						mp.DateTime BETWEEN '''+ @StartDate + ''' 
									AND DATEADD(DAY, 1, '''+ @EndDate + ''')
					GROUP BY CAST(mp.DateTime AS DATE)'

	
	print @query
	-- Execute the SQL query
	EXEC sp_executesql @query
	
	-----For Developers Use----(Executing the query in SQL)
		
	--EXEC [dbo].[usp_GetUsagePerDayRptForExport]
	
	--@StartDate		= '2010-10-01',
	--@EndDate			= '2014-02-18'
	
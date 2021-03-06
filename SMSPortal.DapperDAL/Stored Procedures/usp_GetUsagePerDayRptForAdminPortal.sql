USE [CassiaBeta]
GO
/****** Object:  StoredProcedure [dbo].[usp_GetUsagePerDayRptForAdminPortal]    Script Date: 11/12/2013 18:21:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Rekha
-- Create date: 2nd December 2013
-- Description:	Gets Usage Details for reports based on Page Number and Sorted Order
-- =============================================
ALTER PROCEDURE [dbo].[usp_GetUsagePerDayRptForAdminPortal]
(
	@sortColumn		    nvarchar(50),
	@sortOrder			nvarchar(50),
	@pageNumber			int,
	@pageSize			int,
	
	@StartDate			nvarchar(30),
	@EndDate			nvarchar(30)	
)
AS
	IF LEN(@sortColumn) = 0
		SET @sortColumn = 'CAST(mp.DateTime AS DATE)'

	IF LEN(@sortOrder) = 0
		SET @sortOrder = 'ASC'
	
	-- Issue query	
	DECLARE @query nvarchar(4000)	

	SET @query = 'SELECT 
						CAST(mp.DateTime AS DATE) as [Date], 
						Count(*) AS TotalMessagesSentPerDay,
						Sum(NetAmount) AS TotalNetAmountPerDay
					FROM 
						tblMicroPayment mp
					WHERE
						mp.DateTime BETWEEN '''+ @StartDate + ''' 
									AND DATEADD(DAY, 1, '''+ @EndDate + ''')
					GROUP BY CAST(mp.DateTime AS DATE) '+
					' ORDER BY ' + @sortColumn + ' ' + @sortOrder +
					' OFFSET (' + CONVERT(nvarchar(10), @pageNumber) + '-1) * ' + CONVERT(nvarchar(10), @pageSize) + ' ROWS'  + 
					' FETCH NEXT ' + CONVERT(nvarchar(10), @pageSize) + ' ROWS ONLY'

	
	print @query
	-- Execute the SQL query
	EXEC sp_executesql @query
	
	-----For Developers Use----(Executing the query in SQL)
		
	--EXEC [dbo].[usp_GetUsagePerDayRptForAdminPortal]
		 
	--@sortColumn='UsageDatetime',
	--@sortOrder='DESC',
	--@pageNumber = 1, 
	--@pageSize=50, 

	--@StartDate		= '2010-10-01',
	--@EndDate	    = '2013-02-18'
	
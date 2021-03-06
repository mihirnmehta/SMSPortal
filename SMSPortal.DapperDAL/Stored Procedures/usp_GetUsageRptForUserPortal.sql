USE [CassiaBeta]
GO
/****** Object:  StoredProcedure [dbo].[usp_GetUsageRptForUserPortal]    Script Date: 12/12/2013 14:30:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Rekha
-- Create date: 2nd December 2013
-- Modified date: 12th December 2013, Mihir Mehta
-- Description:	Gets Usage Details for reports based on Page Number and Sorted Order
-- =============================================
ALTER PROCEDURE [dbo].[usp_GetUsageRptForUserPortal]
(
	@sortColumn		    nvarchar(50),
	@sortOrder			nvarchar(50),
	@pageNumber			int,
	@pageSize			int,
	
	@OrganisatonID		int,
	@MPAccountCode		int,
	@StartDate			nvarchar(30), --accepts date in yyyy-MM-dd
	@EndDate			nvarchar(30)  --accepts date in yyyy-MM-dd	
)
AS
	IF LEN(@sortColumn) = 0
		SET @sortColumn = 'Description'

	IF LEN(@sortOrder) = 0
		SET @sortOrder = 'ASC'

	DECLARE @searchCriteria nvarchar(300)
	
	SET @searchCriteria=''
								
	IF @MPAccountCode != 0
		SET @searchCriteria = ' AND mp.MPAccountCode = ' + CONVERT(nvarchar(10), @MPAccountCode)
	
	DECLARE @query nvarchar(4000)	

	SET @query = 'SELECT 
						mpacc.Description AS MPAccountName,
						mp.DateTime AS [Date], 
						mp.NetAmount, 
						mp.StatementDescription 
					FROM 
						tblMicroPayment mp JOIN tblMPAccount mpacc
					ON
						mp.MPAccountCode = mpacc.MPAccountCode
					WHERE  mpacc.OrganisationID = ' + CONVERT(nvarchar(10), @OrganisatonID) +
						  ' AND 
								mp.[DateTime] BETWEEN ''' + @StartDate + '''
							AND
								DATEADD(DAY, 1,''' + @EndDate + ''')'+@searchCriteria+
					' ORDER BY ' + @sortColumn + ' ' + @sortOrder +
					' OFFSET (' + CONVERT(nvarchar(10), @pageNumber) + '-1) * ' + CONVERT(nvarchar(10), @pageSize) + ' ROWS'  + 
					' FETCH NEXT ' + CONVERT(nvarchar(10), @pageSize) + ' ROWS ONLY'

		
	print @query
	-- Execute the SQL query
	EXEC sp_executesql @query
	
	-----For Developers Use----(Executing the query in SQL)
		
	--EXEC usp_GetUsageRptForUserPortal
	 
	--	@sortColumn='',
	--	@sortOrder='',
	--	@pageNumber = 1, 
	--	@pageSize=50, 
		
	--	@OrganisatonID  = 30,
	--  @MPAccountCode  = 0,
	--  @StartDate		= '2012/10/18',
	--  @EndDate	    = '2013/09/25'
	
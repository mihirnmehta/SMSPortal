USE [CassiaBeta]
GO
/****** Object:  StoredProcedure [dbo].[usp_GetUsageRptForExport]    Script Date: 12/12/2013 PM 5:12:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Rekha
-- Create date: 5th December 2013
-- Description:	Gets Usage Details for reports
-- =============================================
ALTER PROCEDURE [dbo].[usp_GetUsageRptForExport]
(	
	@OrganisatonID		int,
	@MPAccountCode		int,
	@StartDate			nvarchar(30), --accepts date in yyyy-MM-dd
	@EndDate			nvarchar(30)  --accepts date in yyyy-MM-dd	
)
AS
	DECLARE @searchCriteria nvarchar(300)
	
	SET @searchCriteria=''
								
	IF @MPAccountCode != 0
		SET @searchCriteria = ' AND mp.MPAccountCode = ' + CONVERT(nvarchar(10), @MPAccountCode)
	
	DECLARE @query nvarchar(4000)	

	SET @query = 'SELECT 
						mpacc.Description AS MPAccountName,
						mp.DateTime AS [Date],
						mp.NetAmount, 
						mp.StatementDescription AS StatementDescription 
					FROM 
						tblMicroPayment mp JOIN tblMPAccount mpacc
					ON
						mp.MPAccountCode = mpacc.MPAccountCode
					WHERE  mpacc.OrganisationID = ' + CONVERT(nvarchar(10), @OrganisatonID) +
						  ' AND 
								mp.[DateTime] BETWEEN ''' + @StartDate + '''
							AND
								DATEADD(DAY, 1,''' + @EndDate + ''')'+@searchCriteria
				
		
	print @query
	-- Execute the SQL query
	EXEC sp_executesql @query
	
	-----For Developers Use----(Executing the query in SQL)
		
	--EXEC usp_GetUsageRptForExport
	 		
	--@OrganisatonID  = 5319,
	--@MPAccountCode  = 0,
	--@StartDate	  = '2012-10-01',
	--@EndDate	      = '2013-12-31'
	
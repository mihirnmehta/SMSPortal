
/****** Object:  StoredProcedure [dbo].[usp_GetFinanceRptForAdminPortal]    Script Date: 12/12/2013 14:14:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Rekha
-- Create date: 3rd December 2013
-- Modified Date: 12th December 2013, Mihir Mehta
-- Description:	Gets finance Details regarding Topup for reports based on Page Number and Sorted Order
-- =============================================
ALTER PROCEDURE [dbo].[usp_GetFinanceRptForAdminPortal]
(
	@sortColumn		    nvarchar(50),
	@sortOrder			nvarchar(50),
	@pageNumber			int,
	@pageSize			int,
	
	@BillingMethodID	int,
	@StartDate			nvarchar(30),
	@EndDate			nvarchar(30)	
)
AS
	IF LEN(@sortColumn) = 0
		SET @sortColumn = 'Description'

	IF LEN(@sortOrder) = 0
		SET @sortOrder = 'ASC'

	DECLARE @searchCriteria nvarchar(300)
	SET @searchCriteria=''
	
	IF @BillingMethodID != 0
		SET @searchCriteria = ' AND mpTrans.PaymentMethodID = ' + CONVERT(nvarchar(10), @BillingMethodID)

	-- Issue query	
	DECLARE @query nvarchar(4000)	

	SET @query = 'SELECT
						org.OrganisationName AS CustomerName,
						mpAcc.[Description] AS MPAccountName,
						bm.BillingMethodName AS BillingMethod,
						mpTrans.TransactionDate,
						mpTrans.Amount,
						mpTrans.InvoiceStatusDate AS OAExportDate
					FROM 
						tblMicropaymentTransactions mpTrans 
					JOIN tblMPAccount mpAcc 
					ON 
						mpTrans.MicropaymentAccountID = mpAcc.MPAccountCode
					JOIN
						tblBillingMethod bm
					ON
						mpTrans.PaymentMethodID = bm.BillingMethodID
					JOIN
						tblOrganisation org
					ON
						mpAcc.OrganisationID = org.OrganisationID
					WHERE
						mpTrans.TransactionDate BETWEEN ''' + @StartDate + '''
					AND
						DATEADD(DAY, 1,''' + @EndDate  + ''')'+@searchCriteria+
					' ORDER BY ' + @sortColumn + ' ' + @sortOrder +
					' OFFSET (' + CONVERT(nvarchar(10), @pageNumber) + '-1) * ' + CONVERT(nvarchar(10), @pageSize) + ' ROWS'  + 
					' FETCH NEXT ' + CONVERT(nvarchar(10), @pageSize) + ' ROWS ONLY'

	
	print @query
	-- Execute the SQL query
	EXEC sp_executesql @query
	
	-----For Developers Use----(Executing the query in SQL)
	--EXEC usp_GetFinanceRptForAdminPortal
	 
	--	@sortColumn		= '',
	--	@sortOrder		= '',
	--	@pageNumber		= 1, 
	--	@pageSize		= 10, 
	--	@StartDate		= '10/01/2013',					
	--	@EndDate		= '12/01/2013',
	--	@BillingMethodID= 2
		
	
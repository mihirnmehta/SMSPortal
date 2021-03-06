
/****** Object:  StoredProcedure [dbo].[usp_GetFinanceRptForExport]    Script Date: 12/12/2013 PM 5:52:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Rekha
-- Create date: 5th December 2013
-- Description:	Gets finance Details regarding Topup for reports for exporting in CSV file
-- =============================================
ALTER PROCEDURE [dbo].[usp_GetFinanceRptForExport]
(
	@BillingMethodID	int,
	@StartDate			nvarchar(30),
	@EndDate			nvarchar(30)	
)
AS

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
						mpTrans.TransactionDate AS TransactionDate,
						mpTrans.Amount AS Amount,
						mpTrans.InvoiceStatusDate AS OAExportDate
					FROM 
						tblMicropaymentTransactions mpTrans 
					JOIN
						tblMPAccount mpAcc
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
						DATEADD(DAY, 1,''' + @EndDate  + ''')'+@searchCriteria

	
	print @query
	-- Execute the SQL query
	EXEC sp_executesql @query
	
	-----For Developers Use----(Executing the query in SQL)
	--EXEC usp_GetFinanceRptForExport

	--@StartDate		= '2010-01-01',					
	--@EndDate		    = '2012-12-31',
	--@BillingMethodID  = 0
		
	
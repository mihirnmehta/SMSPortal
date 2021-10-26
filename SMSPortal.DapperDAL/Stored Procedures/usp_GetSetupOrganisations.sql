USE [SMS Portal]
GO
/****** Object:  StoredProcedure [dbo].[usp_GetSetupOrganisations]    Script Date: 15/12/2013 10:24:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Mihir Mehta
-- Create date: 30th November 2013
-- Description:	Gets Organisations Already setup based on Search Field, Page Number and Sorted Order
-- =============================================
ALTER PROCEDURE [dbo].[usp_GetSetupOrganisations]
(
	@sortColumn		    nvarchar(50),
	@sortOrder			nvarchar(50),
	@searchColumn       nvarchar(50) = NULL,
	@searchText			nvarchar(255) = NULL,
	@searchOperator     nvarchar(10) = NULL,
	@CompanyID			int,
	@pageNumber			int,
	@pageSize			int
)
AS
	DECLARE @searchCriteria nvarchar(300)

	IF LEN(@sortColumn) = 0
		SET @sortColumn = 'OrganisationName'

	IF LEN(@sortOrder) = 0
		SET @sortOrder = 'ASC'

	IF(@searchColumn IS NULL OR @searchColumn = '')
		SET @searchCriteria = ''
	ELSE 
		BEGIN
			IF @searchOperator = 'cn'
				SET @searchCriteria = ' AND org.' + @searchColumn  + ' LIKE ''%' + @searchText + '%'''
			ELSE
				SET @searchCriteria = ' AND org.' + @searchColumn + ' = ''' + @searchText + ''''
		END

	print @searchCriteria	

	-- Issue query	
	DECLARE @query nvarchar(4000)	

	SET @query = 'SELECT org.OrganisationID, 
		                org.OrganisationName, 
		                org.OrgOpenAccountID,
						org.CompanyID, 						
		                ISNULL(t.MPAccounts, 0) as MPActCount, 
		                CASE ISNULL(p.PLCount,0)
                                WHEN 0 then 0
                                ELSE 1
                            end as CustomPLExist,
                        CASE WHEN( (select count(*) from tblOrgBillingMethod obm 
                                    where obm.OrganisationID = org.OrganisationID and obm.BillingMethodID = 1)) = 0 
                                THEN CAST(0 AS BIT) 
                                ELSE CAST(1 AS BIT) 
                                END as PayPal,
		                CASE WHEN( (select count(*) from tblOrgBillingMethod obm 
                                    where obm.OrganisationID = org.OrganisationID and obm.BillingMethodID = 2)) = 0 
								THEN CAST(0 AS BIT) 
								ELSE CAST(1 AS BIT) 
								END as Invoice						
                    FROM tblOrganisation org
                    LEFT JOIN 	
	                    (select OrganisationID, count(*) as MPAccounts 
		                    from tblMPAccount group by OrganisationID) t
                    ON org.OrganisationID = t.OrganisationID
                    LEFT JOIN
	                    (select OrganisationID, count(*) as PLCount
		                    from tblOrgPriceList group by OrganisationID) p
                    ON org.OrganisationID = p.OrganisationID
                    WHERE org.CompanyID = ' +  CONVERT(nvarchar(10), @CompanyID) + 'and org.IsSetup = 1 and org.IsDeletedFromOA = 0 ' +  @searchCriteria +
					' ORDER BY ' + @sortColumn + ' ' + @sortOrder +
					' OFFSET (' + CONVERT(nvarchar(10), @pageNumber) + '-1) * ' + CONVERT(nvarchar(10), @pageSize) + ' ROWS'  + 
					' FETCH NEXT ' + CONVERT(nvarchar(10), @pageSize) + ' ROWS ONLY'

	
	print @query
	-- Execute the SQL query
	 EXEC sp_executesql @query



--EXEC [usp_GetSetupOrganisations]
	 
--@sortColumn		= '',
--@sortOrder		= '',
--@pageNumber		= 1, 
--@pageSize		= 10,
--@CompanyID		= 350
	
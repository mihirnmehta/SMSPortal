USE [master]
GO
/****** Object:  Database [SMS Portal]    Script Date: 3/7/2015 1:02:45 PM ******/
CREATE DATABASE [SMS Portal]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'SMS Portal', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\SMS Portal.mdf' , SIZE = 5120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'SMS Portal_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\SMS Portal_log.ldf' , SIZE = 20096KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [SMS Portal] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [SMS Portal].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [SMS Portal] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [SMS Portal] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [SMS Portal] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [SMS Portal] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [SMS Portal] SET ARITHABORT OFF 
GO
ALTER DATABASE [SMS Portal] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [SMS Portal] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [SMS Portal] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [SMS Portal] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [SMS Portal] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [SMS Portal] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [SMS Portal] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [SMS Portal] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [SMS Portal] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [SMS Portal] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [SMS Portal] SET  DISABLE_BROKER 
GO
ALTER DATABASE [SMS Portal] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [SMS Portal] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [SMS Portal] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [SMS Portal] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [SMS Portal] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [SMS Portal] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [SMS Portal] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [SMS Portal] SET RECOVERY FULL 
GO
ALTER DATABASE [SMS Portal] SET  MULTI_USER 
GO
ALTER DATABASE [SMS Portal] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [SMS Portal] SET DB_CHAINING OFF 
GO
ALTER DATABASE [SMS Portal] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [SMS Portal] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
EXEC sys.sp_db_vardecimal_storage_format N'SMS Portal', N'ON'
GO
USE [SMS Portal]
GO
/****** Object:  User [smsportal]    Script Date: 3/7/2015 1:02:46 PM ******/
CREATE USER [smsportal] FOR LOGIN [smsportal] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [smsportal]
GO
/****** Object:  StoredProcedure [dbo].[usp_GetFinanceRptForAdminPortal]    Script Date: 3/7/2015 1:02:47 PM ******/
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
CREATE PROCEDURE [dbo].[usp_GetFinanceRptForAdminPortal]
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
						mpTrans.Amount
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
		
	
GO
/****** Object:  StoredProcedure [dbo].[usp_GetFinanceRptForExport]    Script Date: 3/7/2015 1:02:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Rekha
-- Create date: 5th December 2013
-- Description:	Gets finance Details regarding Topup for reports for exporting in CSV file
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetFinanceRptForExport]
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
						mpTrans.Amount AS Amount
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
		
	
GO
/****** Object:  StoredProcedure [dbo].[usp_GetSetupOrganisations]    Script Date: 3/7/2015 1:02:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Mihir Mehta
-- Create date: 30th November 2013
-- Description:	Gets Organisations Already setup based on Search Field, Page Number and Sorted Order
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetSetupOrganisations]
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
	
GO
/****** Object:  StoredProcedure [dbo].[usp_GetUsagePerDayRptForAdminPortal]    Script Date: 3/7/2015 1:02:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Rekha
-- Create date: 2nd December 2013
-- Description:	Gets Usage Details for reports based on Page Number and Sorted Order
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetUsagePerDayRptForAdminPortal]
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
		SET @sortColumn = 'UsageDateTime'

	IF LEN(@sortOrder) = 0
		SET @sortOrder = 'ASC'
	
	-- Issue query	
	DECLARE @query nvarchar(4000)	

	SET @query = 'SELECT 
						mp.DateTime as Date, 
						Count(*) AS TotalMessagesSentPerDay,
						Sum(NetAmount) AS TotalNetAmountPerDay
					FROM 
						tblMicroPayment mp
					WHERE
						mp.DateTime BETWEEN '''+ @StartDate + ''' 
									AND DATEADD(DAY, 1, '''+ @EndDate + ''')
					GROUP BY CONVERT( nvarchar(30), mp.DateTime,106)'+
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
	
	--GROUP BY CONVERT( nvarchar(30), mp.DateTime,106)'+
GO
/****** Object:  StoredProcedure [dbo].[usp_GetUsagePerDayRptForExport]    Script Date: 3/7/2015 1:02:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Rekha
-- Create date: 5th December 2013
-- Description:	Gets Usage Per Day Details for reports
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetUsagePerDayRptForExport]
(	
	@StartDate			nvarchar(30),
	@EndDate			nvarchar(30)	
)
AS
	DECLARE @query nvarchar(4000)	

	SET @query = 'SELECT 
						CAST(mp.DateTime AS DATE), 
						Count(*) AS TotalMessageSentPerDay,
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
	
GO
/****** Object:  StoredProcedure [dbo].[usp_GetUsageRptForExport]    Script Date: 3/7/2015 1:02:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Rekha
-- Create date: 5th December 2013
-- Description:	Gets Usage Details for reports
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetUsageRptForExport]
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
	
GO
/****** Object:  StoredProcedure [dbo].[usp_GetUsageRptForUserPortal]    Script Date: 3/7/2015 1:02:47 PM ******/
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
CREATE PROCEDURE [dbo].[usp_GetUsageRptForUserPortal]
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
	
GO
/****** Object:  Table [dbo].[OAActivityCode]    Script Date: 3/7/2015 1:02:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[OAActivityCode](
	[CompanyID] [int] NOT NULL,
	[BaseCompanyID] [varchar](10) NOT NULL,
	[ActivityCode] [nvarchar](10) NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[OAAnalysis]    Script Date: 3/7/2015 1:02:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OAAnalysis](
	[BaseCompanyID] [nvarchar](50) NULL,
	[BaseCompanyName] [nvarchar](100) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[OABaseCompany]    Script Date: 3/7/2015 1:02:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[OABaseCompany](
	[BaseCompanyID] [varchar](10) NOT NULL,
	[CostCode] [nvarchar](10) NOT NULL,
	[ExpenseCode] [nvarchar](10) NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[OACompany]    Script Date: 3/7/2015 1:02:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OACompany](
	[ID] [int] NOT NULL,
	[CompanyName] [nvarchar](60) NOT NULL,
	[ProjectCode] [nvarchar](10) NOT NULL,
 CONSTRAINT [PK_OACompany] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[OACurrency]    Script Date: 3/7/2015 1:02:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OACurrency](
	[CompanyID] [int] NOT NULL,
	[Currency] [nvarchar](10) NOT NULL,
	[EffectiveDate] [date] NOT NULL,
	[ConversionRate] [float] NOT NULL,
	[IsDeletedFromOA] [bit] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[OAPeriod]    Script Date: 3/7/2015 1:02:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OAPeriod](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CompanyID] [int] NOT NULL,
	[IsDeletedFromOA] [bit] NOT NULL,
	[NotesID] [nvarchar](24) NULL,
	[PYear] [numeric](4, 0) NOT NULL,
	[PDates] [nvarchar](max) NOT NULL,
	[YearDescription] [nvarchar](50) NULL,
	[EndDate] [datetime] NULL,
	[NoOfWeeks] [nvarchar](150) NOT NULL,
	[NoOfDays] [nvarchar](150) NOT NULL,
	[MaxPeriod] [int] NOT NULL,
	[PeriodName] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_OAPeriod] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tblAccountServicePerms]    Script Date: 3/7/2015 1:02:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblAccountServicePerms](
	[MPAccountCode] [int] NOT NULL,
	[ServiceCode] [int] NULL,
	[WorkDayAllowedFrom] [datetime] NULL,
	[WorkDayAllowedTo] [datetime] NULL,
	[NonWorkDayAllowedFrom] [datetime] NULL,
	[NonWorkDayAllowedTo] [datetime] NULL,
 CONSTRAINT [PK_tblAccountServicePerms_1] PRIMARY KEY CLUSTERED 
(
	[MPAccountCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tblAdminPortalAccessLevel]    Script Date: 3/7/2015 1:02:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblAdminPortalAccessLevel](
	[AccessLevelID] [int] IDENTITY(1,1) NOT NULL,
	[AccessLevel] [varchar](20) NOT NULL,
 CONSTRAINT [PK_tblAdminPortalAccessLevels] PRIMARY KEY CLUSTERED 
(
	[AccessLevelID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblBillingMethod]    Script Date: 3/7/2015 1:02:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblBillingMethod](
	[BillingMethodID] [int] IDENTITY(1,1) NOT NULL,
	[BillingMethodName] [varchar](20) NOT NULL,
 CONSTRAINT [PK_tblBillingMethod] PRIMARY KEY CLUSTERED 
(
	[BillingMethodID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblGlobalPriceList]    Script Date: 3/7/2015 1:02:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblGlobalPriceList](
	[TierID] [int] IDENTITY(1,1) NOT NULL,
	[PricePerSMS] [numeric](18, 4) NOT NULL,
	[Banding] [int] NOT NULL,
 CONSTRAINT [PK_tblGlobalPriceList] PRIMARY KEY CLUSTERED 
(
	[TierID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tblManagementUser]    Script Date: 3/7/2015 1:02:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblManagementUser](
	[ManagementUserID] [int] IDENTITY(1,1) NOT NULL,
	[Forename] [varchar](255) NULL,
	[Surname] [varchar](255) NULL,
	[Email] [varchar](255) NULL,
	[PhoneNumber] [varchar](15) NULL,
	[Password] [varchar](80) NULL,
	[AccessLevelID] [int] NULL,
	[UpdatedBy] [varchar](255) NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_tblManagementUser] PRIMARY KEY CLUSTERED 
(
	[ManagementUserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblMicropaymentTransactions]    Script Date: 3/7/2015 1:02:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblMicropaymentTransactions](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[MicropaymentAccountID] [int] NOT NULL,
	[PaymentMethodID] [int] NOT NULL,
	[Amount] [numeric](18, 4) NOT NULL,
	[LoggedInUserEmail] [varchar](255) NOT NULL,
	[FromAdminPortal] [bit] NOT NULL,
	[PayPalTransactionID] [varchar](50) NULL,
	[PayerEmail] [varchar](255) NULL,
	[Currency] [varchar](10) NOT NULL,
	[TransactionDate] [datetime] NOT NULL,
	[InvoiceStatus] [varchar](25) NULL,
	[InvoiceStatusDate] [datetime] NULL,
	[BatchNumber] [varchar](15) NULL,
 CONSTRAINT [PK_MicropaymentTransactions] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblMPAccount]    Script Date: 3/7/2015 1:02:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblMPAccount](
	[MPAccountCode] [int] IDENTITY(1,1) NOT NULL,
	[AccountLogin] [varchar](20) NOT NULL,
	[Description] [varchar](50) NULL,
	[Balance] [numeric](18, 4) NOT NULL,
	[UseSecureMode] [bit] NOT NULL,
	[EncryptedPassword] [varchar](80) NOT NULL,
	[OrganisationID] [int] NOT NULL,
	[SendLowBalanceWarnings] [bit] NOT NULL,
	[BalanceWarningLimit] [float] NULL,
	[BalanceWarningContact] [int] NULL,
	[BalanceOverdraftLimit] [numeric](18, 4) NULL,
	[BalanceWarningEmail] [varchar](255) NULL,
	[IsEnabled] [bit] NOT NULL,
	[CreatedBy] [varchar](255) NULL,
	[CreateDate] [datetime] NULL,
	[UpdatedBy] [varchar](255) NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_TBLMPACCOUNT] PRIMARY KEY CLUSTERED 
(
	[MPAccountCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblMPAccountBillingMethod]    Script Date: 3/7/2015 1:02:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblMPAccountBillingMethod](
	[MPAccountBillingMethodID] [int] IDENTITY(1,1) NOT NULL,
	[MPAccountCode] [int] NOT NULL,
	[BillingMethodID] [int] NOT NULL,
 CONSTRAINT [PK_tblMPAccountBillingMethod] PRIMARY KEY CLUSTERED 
(
	[MPAccountBillingMethodID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tblOrganisation]    Script Date: 3/7/2015 1:02:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblOrganisation](
	[OrganisationID] [int] IDENTITY(1,1) NOT NULL,
	[OrganisationName] [varchar](255) NOT NULL,
	[OrgOpenAccountID] [int] NOT NULL,
	[CompanyID] [int] NOT NULL,
	[BaseCompanyID] [nvarchar](50) NULL,
	[Address] [varchar](255) NOT NULL,
	[InvoiceCurrency] [varchar](10) NULL,
	[VATCode] [nvarchar](50) NULL,
	[UpdatedBy] [varchar](255) NULL,
	[UpdatedDate] [datetime] NULL,
	[IsSetup] [bit] NOT NULL,
	[IsDeletedFromOA] [bit] NOT NULL,
 CONSTRAINT [PK_tblOrganisation] PRIMARY KEY CLUSTERED 
(
	[OrganisationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblOrganisationUser]    Script Date: 3/7/2015 1:02:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblOrganisationUser](
	[OrganisationUserID] [int] IDENTITY(1,1) NOT NULL,
	[OrganisationID] [int] NULL,
	[Forename] [varchar](255) NULL,
	[Surname] [varchar](255) NULL,
	[Email] [varchar](255) NULL,
	[Password] [varchar](80) NULL,
	[AccessLevelID] [int] NOT NULL,
	[UpdatedBy] [varchar](255) NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_tblOrganisationUsers] PRIMARY KEY CLUSTERED 
(
	[OrganisationUserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblOrgBillingMethod]    Script Date: 3/7/2015 1:02:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblOrgBillingMethod](
	[OrgBillingMethodID] [int] IDENTITY(1,1) NOT NULL,
	[OrganisationID] [int] NOT NULL,
	[BillingMethodID] [int] NOT NULL,
 CONSTRAINT [PK_tblOrgBillingMethod] PRIMARY KEY CLUSTERED 
(
	[OrgBillingMethodID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tblOrgContact]    Script Date: 3/7/2015 1:02:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblOrgContact](
	[OrganisationID] [int] NOT NULL,
	[ContactName] [varchar](255) NOT NULL,
	[ContactEmail] [varchar](255) NULL,
	[ContactPhone] [varchar](15) NULL,
 CONSTRAINT [PK_tblContact_1] PRIMARY KEY CLUSTERED 
(
	[OrganisationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblOrgPriceList]    Script Date: 3/7/2015 1:02:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblOrgPriceList](
	[TierID] [int] IDENTITY(1,1) NOT NULL,
	[OrganisationID] [int] NOT NULL,
	[PricePerSMS] [numeric](18, 4) NOT NULL,
	[Banding] [int] NOT NULL,
 CONSTRAINT [PK_tblOrgPriceList] PRIMARY KEY CLUSTERED 
(
	[TierID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tblUserPortalAccessLevel]    Script Date: 3/7/2015 1:02:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblUserPortalAccessLevel](
	[AccessLevelID] [int] IDENTITY(1,1) NOT NULL,
	[AccessLevel] [varchar](20) NOT NULL,
 CONSTRAINT [PK_tblUserPortalAccessLevel] PRIMARY KEY CLUSTERED 
(
	[AccessLevelID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Index [UQ_tblOrganisation_OrgOpenActID_CompanyID]    Script Date: 3/7/2015 1:02:47 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UQ_tblOrganisation_OrgOpenActID_CompanyID] ON [dbo].[tblOrganisation]
(
	[OrgOpenAccountID] ASC,
	[CompanyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[tblManagementUser] ADD  CONSTRAINT [DF_tblManagementUser_UpdatedDate]  DEFAULT (getdate()) FOR [UpdatedDate]
GO
ALTER TABLE [dbo].[tblMicropaymentTransactions] ADD  CONSTRAINT [DF_tblMicropaymentTransactions_TransactionDate]  DEFAULT (getdate()) FOR [TransactionDate]
GO
ALTER TABLE [dbo].[tblMPAccount] ADD  CONSTRAINT [DF_tblMPAccount_Balance]  DEFAULT ((0)) FOR [Balance]
GO
ALTER TABLE [dbo].[tblMPAccount] ADD  CONSTRAINT [DF_tblMPAccount_UseSecureMode]  DEFAULT ((1)) FOR [UseSecureMode]
GO
ALTER TABLE [dbo].[tblMPAccount] ADD  CONSTRAINT [DF_tblMPAccount_SendLowBalanceWarnings]  DEFAULT ((0)) FOR [SendLowBalanceWarnings]
GO
ALTER TABLE [dbo].[tblMPAccount] ADD  CONSTRAINT [DF_tblMPAccount_CreateDate]  DEFAULT (getdate()) FOR [CreateDate]
GO
ALTER TABLE [dbo].[tblMPAccount] ADD  CONSTRAINT [DF_tblMPAccount_UpdatedDate]  DEFAULT (getdate()) FOR [UpdatedDate]
GO
ALTER TABLE [dbo].[tblOrganisation] ADD  CONSTRAINT [DF_tblOrganisation_UpdatedDate]  DEFAULT (getdate()) FOR [UpdatedDate]
GO
ALTER TABLE [dbo].[tblOrganisation] ADD  CONSTRAINT [DF_tblOrganisation_IsSetup]  DEFAULT ((0)) FOR [IsSetup]
GO
ALTER TABLE [dbo].[tblOrganisationUser] ADD  CONSTRAINT [DF_tblOrganisationUser_UpdatedDate]  DEFAULT (getdate()) FOR [UpdatedDate]
GO
ALTER TABLE [dbo].[OAPeriod]  WITH CHECK ADD  CONSTRAINT [FK_OAPeriod_OACompany] FOREIGN KEY([CompanyID])
REFERENCES [dbo].[OACompany] ([ID])
GO
ALTER TABLE [dbo].[OAPeriod] CHECK CONSTRAINT [FK_OAPeriod_OACompany]
GO
ALTER TABLE [dbo].[tblAccountServicePerms]  WITH CHECK ADD  CONSTRAINT [FK_TBLACCTSRVPER_MPACCTCODE] FOREIGN KEY([MPAccountCode])
REFERENCES [dbo].[tblMPAccount] ([MPAccountCode])
GO
ALTER TABLE [dbo].[tblAccountServicePerms] CHECK CONSTRAINT [FK_TBLACCTSRVPER_MPACCTCODE]
GO
ALTER TABLE [dbo].[tblManagementUser]  WITH CHECK ADD  CONSTRAINT [FK_tblManagementUser_tblAdminPortalAccessLevel] FOREIGN KEY([AccessLevelID])
REFERENCES [dbo].[tblAdminPortalAccessLevel] ([AccessLevelID])
GO
ALTER TABLE [dbo].[tblManagementUser] CHECK CONSTRAINT [FK_tblManagementUser_tblAdminPortalAccessLevel]
GO
ALTER TABLE [dbo].[tblMPAccountBillingMethod]  WITH CHECK ADD  CONSTRAINT [FK_tblMPAccountBillingMethod_tblBillingMethod] FOREIGN KEY([BillingMethodID])
REFERENCES [dbo].[tblBillingMethod] ([BillingMethodID])
GO
ALTER TABLE [dbo].[tblMPAccountBillingMethod] CHECK CONSTRAINT [FK_tblMPAccountBillingMethod_tblBillingMethod]
GO
ALTER TABLE [dbo].[tblMPAccountBillingMethod]  WITH CHECK ADD  CONSTRAINT [FK_tblMPAccountBillingMethod_tblMPAccount] FOREIGN KEY([MPAccountCode])
REFERENCES [dbo].[tblMPAccount] ([MPAccountCode])
GO
ALTER TABLE [dbo].[tblMPAccountBillingMethod] CHECK CONSTRAINT [FK_tblMPAccountBillingMethod_tblMPAccount]
GO
ALTER TABLE [dbo].[tblOrganisationUser]  WITH CHECK ADD  CONSTRAINT [FK_tblOrganisationUser_tblUserPortalAccessLevel] FOREIGN KEY([AccessLevelID])
REFERENCES [dbo].[tblUserPortalAccessLevel] ([AccessLevelID])
GO
ALTER TABLE [dbo].[tblOrganisationUser] CHECK CONSTRAINT [FK_tblOrganisationUser_tblUserPortalAccessLevel]
GO
ALTER TABLE [dbo].[tblOrganisationUser]  WITH CHECK ADD  CONSTRAINT [FK_tblOrganisationUsers_tblUserPortalAccessLevel] FOREIGN KEY([OrganisationID])
REFERENCES [dbo].[tblOrganisation] ([OrganisationID])
GO
ALTER TABLE [dbo].[tblOrganisationUser] CHECK CONSTRAINT [FK_tblOrganisationUsers_tblUserPortalAccessLevel]
GO
ALTER TABLE [dbo].[tblOrgBillingMethod]  WITH CHECK ADD  CONSTRAINT [FK_tblOrgBillingMethod_tblOrganisation] FOREIGN KEY([BillingMethodID])
REFERENCES [dbo].[tblBillingMethod] ([BillingMethodID])
GO
ALTER TABLE [dbo].[tblOrgBillingMethod] CHECK CONSTRAINT [FK_tblOrgBillingMethod_tblOrganisation]
GO
ALTER TABLE [dbo].[tblOrgBillingMethod]  WITH CHECK ADD  CONSTRAINT [FK_tblOrgBillingMethod_tblOrganisation1] FOREIGN KEY([OrganisationID])
REFERENCES [dbo].[tblOrganisation] ([OrganisationID])
GO
ALTER TABLE [dbo].[tblOrgBillingMethod] CHECK CONSTRAINT [FK_tblOrgBillingMethod_tblOrganisation1]
GO
ALTER TABLE [dbo].[tblOrgContact]  WITH CHECK ADD  CONSTRAINT [FK_tblContact_tblOrganisation] FOREIGN KEY([OrganisationID])
REFERENCES [dbo].[tblOrganisation] ([OrganisationID])
GO
ALTER TABLE [dbo].[tblOrgContact] CHECK CONSTRAINT [FK_tblContact_tblOrganisation]
GO
ALTER TABLE [dbo].[tblOrgPriceList]  WITH CHECK ADD  CONSTRAINT [FK_tblOrgPriceList_tblOrganisation] FOREIGN KEY([OrganisationID])
REFERENCES [dbo].[tblOrganisation] ([OrganisationID])
GO
ALTER TABLE [dbo].[tblOrgPriceList] CHECK CONSTRAINT [FK_tblOrgPriceList_tblOrganisation]
GO
USE [master]
GO
ALTER DATABASE [SMS Portal] SET  READ_WRITE 
GO

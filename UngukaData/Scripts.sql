

select *,CASE WHEN TRY_CONVERT(datetime,insuranceexpirydate,105) is null then null else CONVERT(datetime,insuranceexpirydate,105) end  
from TempInsuranceData where [LOANACCOUNTNUMBER]  in (select [ACCT_NO] from TempBankLoans) 
and DATEDIFF(year,DISBURSEMENTDATE,MATURITY_DT)>1

--and CONVERT(datetime,insuranceexpirydate,105)>=MATURITY_DT

select * from TempInsuranceData where CONVERT(datetime,insuranceexpirydate,105) >=getdate()


select * from TempCollateralData where COLLATERAL_TY_DESC not in ('DEBENTURE-CASH HELD BY THE BANK','DEBENTURE-DEBTORS','DEBENTURE-CASH HELD BY OTHER BANKS OR FINANCIAL INSTITUTIONS')
and acct_no in (select acct_no from TempBankLoans) and COLLATERAL_MARKET_VALUE>10000000 and COLLATERAL_DESC like '%house%'

select * from TempBankLoans where DATEDIFF(year,DISBURSMENT_DT,MATURITY_DT)>1 and status not in ('WRITE OFF','SUBMITTED','CANCELLED','INACTIVE')
AND MATURITY_DT>=getdate()

select count(*),BRANCH from TempBankLoans where DATEDIFF(year,DISBURSMENT_DT,MATURITY_DT)>1 and status not in ('WRITE OFF','SUBMITTED','CANCELLED','INACTIVE')
--AND MATURITY_DT>=getdate()
group by branch



using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eBroker
{
    public class BrokerDataContext : DbContext
    {
        //Connection String
        public BrokerDataContext(string dbConnString) : base(dbConnString) { }
        public DbSet<eUser> eUser { get; set; }
        public DbSet<Vw_Policy_Report> Vw_Policy_Report { get; set; }
        public DbSet<Vw_Brokerage_Summary> Vw_Brokerage_Summary { get; set; }
        public DbSet<Menu> Menu { get; set; }
        public DbSet<Client> Client { get; set; }
        public DbSet<Partner> Partner { get; set; }
        public DbSet<Appointment> Appointment { get; set; }
        public DbSet<eUserCategory> eUserCategory { get; set; }
        public DbSet<InsurancePolicy> InsurancePolicy { get; set; }
        public DbSet<AppointmentDetail> AppointmentDetails { get; set; }
        public DbSet<AppointmentEventType> AppointmentEventType { get; set; }
        public DbSet<AppointmentStatu> AppointmentStatus { get; set; }
        public DbSet<AppointmentType> AppointmentType { get; set; }
        public DbSet<Bank> Bank { get; set; }
        public DbSet<BankCollateral> BankCollateral { get; set; }
        public DbSet<BankLoan> BankLoans { get; set; }
        public DbSet<Brokerage_Company> Brokerage_Company { get; set; }
        public DbSet<claim> claim { get; set; }
        public DbSet<Commission_Tariff> Commission_Tariff { get; set; }
        public DbSet<District> District { get; set; }
        //public DbSet<eCategoryMenu> eCategoryMenus { get; set; }
        public DbSet<eMenu> eMenus { get; set; }
        public DbSet<Fire_allied_peril> Fire_allied_peril { get; set; }
        public DbSet<Insurance_Product> Insurance_Product { get; set; }
        public DbSet<Insurance_Product_Category> Insurance_Product_Category { get; set; }
        public DbSet<Invoice> Invoice { get; set; }
        public DbSet<Invoice_Detail> Invoice_Detail { get; set; }
        public DbSet<LoanType> LoanType { get; set; }
        public DbSet<Personal_Accident> Personal_Accident { get; set; }
        public DbSet<Policy_Loan_Account> Policy_Loan_Account { get; set; }
        public DbSet<Policy_Property> Policy_Property { get; set; }
        public DbSet<Policy_Vehicle> Policy_Vehicle { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Property> Property { get; set; }
        public DbSet<Property_peril> Property_peril { get; set; }
        public DbSet<Property_Roof_Material> Property_Roof_Material { get; set; }
        public DbSet<Property_Use> Property_Use { get; set; }
        public DbSet<Property_Wall_Material> Property_Wall_Material { get; set; }
        public DbSet<Property_Window_Material> Property_Window_Material { get; set; }
        public DbSet<Proposer> Proposer { get; set; }
        public DbSet<Province> Province { get; set; }
        public DbSet<Quotation> Quotation { get; set; }
        public DbSet<Right> Rights { get; set; }
        public DbSet<Sector> Sector { get; set; }
        public DbSet<SMS_Log> SMS_Log { get; set; }
        public DbSet<Vehicle> Vehicle { get; set; }
        public DbSet<Vehicle_Age_Range> Vehicle_Age_Range { get; set; }
        public DbSet<Vehicle_Clas> Vehicle_Class { get; set; }
        public DbSet<Vehicle_Guarantee> Vehicle_Guarantee { get; set; }
        public DbSet<Vehicle_Insurance_Duration> Vehicle_Insurance_Duration { get; set; }
        public DbSet<Vehicle_Occupant> Vehicle_Occupant { get; set; }
        public DbSet<Vehicle_Tariff_Base> Vehicle_Tariff_Base { get; set; }
        public DbSet<Vehicle_Territorial_Limit> Vehicle_Territorial_Limit { get; set; }
        public DbSet<Vehicle_Usage> Vehicle_Usage { get; set; }
        public DbSet<SystemReports> SystemReports { get; set; }
        public DbSet<eCategoryMenu> eCategoryMenu { get; set; }
        public DbSet<Temp_Client> Temp_Client { get; set; }
        public DbSet<Vw_Bank_Collateral> Vw_Bank_Collateral { get; set; }
        public DbSet<BankReports> BankReports { get; set; }
        public DbSet<LoanApplication> LoanApplication { get; set; }
        public DbSet<LoanApplication_Property> LoanApplication_Property { get; set; }
        public DbSet<LoanApplication_Vehicle> LoanApplication_Vehicle { get; set; }

    }
}
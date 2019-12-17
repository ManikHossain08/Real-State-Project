using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace InitVent.Common.Util
{
    /// <summary>
    /// To insert log in user_activity_log table 
    /// </summary>
    public class ActionLogger
    {
        /// <summary>
        /// This will log the information according to the action of the user under specific session on an item
        /// </summary>
        /// <param name="message">message object</param>
        public void Log(Guid userId, string userCode, string userName, string sessionId, Enum item, ActionItem action, string message, string elementRef)
        {
            //var connectionString = ConfigurationManager.ConnectionStrings["iMFASDataServices"].ConnectionString;
            //using (var Conn = new SqlConnection(connectionString))
            //{
            //    var itemId = GetEnumDescription(item);
            //    var itemName = item.ToString().Replace('_', ' ');
            //    Conn.Open();
            //    var comText = " INSERT INTO user_activity_log (id, user_id, userId, userName, session_id, item_id, itemName, action, comment, working_time, element_ref) VALUES('" + Guid.NewGuid() + "','" + userId + "','" + userCode + "','" + userName + "','" + sessionId + "','" + itemId + "','" + itemName + "','" + action + "','" + message + "','" + DateTime.Now + "','" + elementRef + "')";
            //    using (SqlCommand Com = new SqlCommand(comText, Conn))
            //    {
            //        Com.ExecuteNonQuery();
            //    }
            //    Conn.Close();
            //}
        }

        /// <summary>
        /// This will log the information according to the action of the user under specific session on an item
        /// </summary>
        /// <param name="message">message object</param>
        public void Log(Guid userId, string userCode, string userName, string sessionId, Enum item, ActionItem action, string message)
        {
            Log(userId, userCode, userName, sessionId, item, action, message, null);
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
    }

    public enum ActionItem
    {
        Read,
        Create,
        Update,
        Delete,
        Approve,
        Export,
        Import
    }


    public enum ActionLogItem
    {
        [Description("3B316ED5-6F0A-46F4-BF04-05BC1A50A274")]
        Loan_Sector,
        [Description("E7256F05-E76D-4065-8D48-0A299CEF7DB2")]
        Receipts_and_Payments,
        [Description("8EC91C63-2344-4236-85F1-0B589CA84DE6")]
        Import_Offline_Data,
        [Description("26EAE4F3-883A-4701-9F6C-0D0C93991077")]
        Year_End_Process,
        [Description("7A911C16-AFB4-482F-885E-0D896A6FC87C")]
        Salary,
        [Description("DD9034CB-8360-4A60-921B-1342398032A1")]
        Statistics,
        [Description("FB18EFD6-0EF7-458C-B010-143EBAE559C8")]
        Union,
        [Description("F31B0ADC-7DCA-4CA0-9471-17585940FBCD")]
        Thana,
        [Description("94D46FD1-2D47-4132-A455-17D93964E21E")]
        Voucher_Correction,
        [Description("C5181021-062A-42BE-AAAA-1B1FDBA69ABB")]
        Statistics_Opening,
        [Description("94757D4C-235A-4887-8564-1E6883C8C68F")]
        Branch_Savings_Portfolio_Opening,
        [Description("007D9B90-FAC5-4FB3-AC2F-1F55D1501E89")]
        Member,
        [Description("C4E7EEC6-40AA-4332-B295-20DD17775F07")]
        Designation,
        [Description("9F87AB23-F659-4736-9BC0-262AAC328841")]
        Loan_Sub_Sector,
        [Description("50B0E2AB-E501-4DB5-8065-29C060B1667E")]
        Loan_Product,
        [Description("DCF6A87E-21B6-44A5-920A-37BB5B2851DC")]
        Center_Samity_Closing,
        [Description("60352C4A-3EE8-4EDD-8ADE-38A3DC4CD65F")]
        Query_Executor,
        [Description("FDE0D033-08CC-4413-96F4-38CBD06A706A")]
        Loan_Portfolio,
        [Description("8A603FB1-E143-4D9B-B1E3-3F8B3D8FA9B0")]
        Monitoring,
        [Description("5FE6258D-3F15-4599-A5A7-3F977DEF1644")]
        Customer_Type,
        [Description("72F960C9-1AF9-4F63-8325-3FBD80D384A5")]
        Loan_Portfolio_Opening,
        [Description("F7BB4522-5862-4E4E-A458-41B98D16A02D")]
        Organization_Calendar,
        [Description("66A608E7-EF86-43AC-B947-41EA0EDB5D86")]
        Savings_Portfolio,
        [Description("E8BCBC23-0633-49C2-91AB-43405A599CC9")]
        Program,
        [Description("533457A9-B116-42FF-AECC-473A5CA8D760")]
        Service_Charge,
        [Description("29FFF1AE-1E48-4001-BFD9-479FE0287DED")]
        Loan_Accounting_Opening,
        [Description("B8604DDC-06D3-4D67-B2D5-4CB3854D77AF")]
        Loan_Disburse_Realization_Outstanding,
        [Description("3DBECC48-8F23-4DA7-B350-500F15CB44B0")]
        Employees_Comission_Defintion,
        [Description("F90E6FB8-9D54-41C7-BE61-52392C8A7BA1")]
        Loan_Limit_Table,
        [Description("0CE313C7-5C93-4356-BAD7-61DB2FA92F44")]
        Village,
        [Description("13CCF15D-BBF2-4FFB-B233-67EDE7490776")]
        Savings,
        [Description("90FBEF5E-3FCA-4753-8FD9-68E35744598F")]
        Branch,
        [Description("905C690F-4EBF-49F2-979C-6BF489FEEA21")]
        Pass_Book_Sale,
        [Description("4427C3E6-7360-4821-A092-6C19C8A0119F")]
        Pay_Slip,
        [Description("03053833-5449-448C-B7DF-6DCEB565098B")]
        Branch_Accounting_Opening,
        [Description("87ECB3C4-6212-49A7-9205-6E42F735312B")]
        Chart_of_Accounts,
        [Description("6246A536-BD1E-4987-99FF-70DE7A17E218")]
        Generate_Offline_Opening,
        [Description("92276BD5-E5F5-4658-A419-7228AECCCC24")]
        Advance_Sector,
        [Description("B2F4A02F-256D-426C-9C38-7EDD4465AA31")]
        Savings_Accounting_Opening,
        [Description("E08692CA-699D-439E-822B-81B3FF9F4C60")]
        Loan,
        [Description("5C32DFB7-6158-4A1F-90FD-82F3D4EC6420")]
        Accounts_Transaction_Correction,
        [Description("A3F6173D-BF50-4F1B-A7E0-8465474A4F8B")]
        Action_Log_Audit,
        [Description("5935073D-2740-42D3-AE0A-88E879A9F0B8")]
        Savings_Product,
        [Description("1E3A9054-8D9F-452B-82CD-8E12065DA4A9")]
        Accounting_Opening,
        [Description("AA3B1D2F-DD38-4E5C-AAB0-8F610872A401")]
        Migrate_Offline_Branch_to_Online,
        [Description("D08F24EB-AC33-4872-B74E-8F6A1D414C55")]
        Holiday_Payment_Rescheduling,
        [Description("F37EC61D-265B-4721-AF58-8FAC2A17909A")]
        Interest_Generation,
        [Description("7867680C-9DA5-4438-A7A0-903D5D71A126")]
        Collection_Sheet,
        [Description("47D95DA0-3DE9-4337-914E-90E2CAF80A14")]
        CSF_Register,
        [Description("F380FB68-3D7C-4783-840D-93FF2FAB40CB")]
        Accounts,
        [Description("CE2A6082-993B-4D83-AFC3-9ADD4CF1918D")]
        Edit_Lock,
        [Description("6B152E3F-0B4E-4613-8CE1-9E9AA84F59D8")]
        Finance_Accounts_Mapping,
        [Description("829A83F0-3EBA-4402-A472-A40871FFCE85")]
        HO_Interest_Auto_Voucher_Generation,
        [Description("39569810-5C5F-4E0D-9B14-A5CA213CE164")]
        Sectoral_Breakdown_of_Loans,
        [Description("6DA9510B-CFA1-4786-98B4-A6A2A1F82D40")]
        Dashboard_Statistics,
        [Description("95DDE790-8438-44F9-A590-A9F3C195A24D")]
        Fine_Table,
        [Description("79B82498-F374-4DBB-946A-AB814107C455")]
        Auto_Voucher_Generation,
        [Description("BF31C843-0716-41FC-8088-AD22A9531070")]
        Area,
        [Description("2AAA4EC7-70E0-466B-BDD3-B03108B97EC3")]
        Voucher,
        [Description("82CFD726-1D91-4D0F-B5F0-B04AA9F5D835")]
        Period_End_Process,
        [Description("4401BE64-E7F2-4E76-B0E5-B1F30D0DCC78")]
        Interest,
        [Description("BA5B31F6-4E6B-4202-9D9A-B8E0C9E08DD5")]
        User_Info,
        [Description("BC2FF9A0-AC94-4831-AD72-B9C8F6BD30A8")]
        Role_Permission,
        [Description("85202BDD-F3F4-4444-98F3-BA5589D922A1")]
        Loan_Account,
        [Description("97BF0F4A-D8B7-459F-9371-BB490A246342")]
        Loan_Amount_Table,
        [Description("47DA8BA9-0555-44F7-B834-BF13F2B80486")]
        Product_Availing_Scope,
        [Description("43324B67-BBE5-4706-BA83-C0393754BA69")]
        Savings_Portfolio_Opening,
        [Description("CA501E94-45A2-491A-8B89-C2492B2E3757")]
        Loan_Loss_Provision,
        [Description("55DC3505-E5FF-4B31-80E3-C56F4148836A")]
        Advance,
        [Description("8D696253-D9BF-4C9D-A910-C6183261F192")]
        General_Configuration,
        [Description("18FE32B8-FE3F-420A-A75E-C7460251A067")]
        Collection_Sheet_Correction,
        [Description("E879A7A8-3280-453F-B48F-CAD67194BA68")]
        District,
        [Description("8A1C0ACA-9DCA-451C-841D-D16EE3791BD7")]
        Payroll,
        [Description("21028878-69F6-4EF2-ACC1-D2144BF32791")]
        Center,
        [Description("EAF9E9C5-5A6B-4838-8430-D32A04676252")]
        Staff_Loan,
        [Description("AC2ACEF8-7A53-4556-A517-D74BC2ED9661")]
        Field_Collection,
        [Description("7D10C480-AAE5-4A76-80A2-D99D04A51689")]
        Account_Transaction,
        [Description("758A73F9-6256-42F3-8FBF-DA433E3C2670")]
        Insurance,
        [Description("3FFF0509-84C0-4946-89CF-DC9132BEBC3C")]
        Aging_of_Loan,
        [Description("A207B1C0-E04D-4C63-9958-DE5F1E4067F5")]
        Employee,
        [Description("C45B93D2-5E31-48A6-BAF4-E050009C79C0")]
        Zone,
        [Description("F60A9D4C-4A39-4D4F-8E0D-E2E493D579CC")]
        Day_End_Process,
        [Description("B52B4520-49EE-4BC9-B1FC-E5DEF14BB8F2")]
        Customer_Transfer,
        [Description("9D5BA2E3-2348-49DD-825C-E63F5094E653")]
        Savings_Account,
        [Description("EB8867F3-0A41-41D7-B8CC-E64F20228994")]
        Loan_Disburse_Realization_Opening,
        [Description("40A3700A-0F88-4F38-A98B-EAF22F97B9E3")]
        Samity,
        [Description("841C382C-CB21-4CE3-9F39-F1CECC5A825A")]
        Disburse_Opening,
        [Description("E32D5A03-06A4-420E-8FFE-F5E207C69ECB")]
        Accounts_Rescheduling,
        [Description("1DE9C34A-9A66-4A82-BFF4-F65BD372463F")]
        Over_Due_Savings_Account_Halting,
        [Description("D13A3C48-F64E-4254-A5EA-F962EC880833")]
        Program_Organizers_Opening_Generate,
        [Description("B7E7FEB6-6EE0-4706-AA9B-FC47CC456DB1")]
        Working_Day_Change,
        [Description("0AD432C9-67AA-4990-A27C-FCEBE6FA7D96")]
        Project
    }
}
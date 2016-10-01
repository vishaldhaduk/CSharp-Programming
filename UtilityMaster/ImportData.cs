using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityMaster
{
    public class ImportData
    {
        public static List<MemberMaster> listMember = new List<MemberMaster>();
        public static List<MemberDetailMaster> listMemberDetail = new List<MemberDetailMaster>();
        public static List<MemberAccountMaster> listMemberAccount = new List<MemberAccountMaster>();

        public static string connString = "Data Source=DEV-Vishal\\SQLEXPRESS;Initial Catalog=AGSDB;Integrated Security=SSPI;";
        public static string connTestString = "Data Source=DEV-Vishal\\SQLEXPRESS;Initial Catalog=Test;Integrated Security=SSPI;";

        //public static string connString = "Data Source=VISHAL-PC;Initial Catalog=AGSDB;Integrated Security=SSPI;";
        //public static string connTestString = "Data Source=VISHAL-PC;Initial Catalog=Test;Integrated Security=SSPI;";

        #region InsertUsingSP
        internal static void ConnectSP()
        {
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = connTestString;
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SelectAllMemberFromExcel";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        MemberMaster m = new MemberMaster();
                        m.ID = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("ID")));
                        m.BarcodeId = string.Concat(Convert.ToInt32(reader.GetValue(reader.GetOrdinal("ID"))),
                                                    "+",
                                                    Convert.ToInt32(reader.GetValue(reader.GetOrdinal("FamilyId"))));
                        m.FamilyId = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("FamilyId")));
                        m.Title = reader.GetValue(reader.GetOrdinal("Title")).ToString();
                        m.LName = reader.GetValue(reader.GetOrdinal("LName")).ToString();
                        m.FName = reader.GetValue(reader.GetOrdinal("FName")).ToString();
                        m.IsPrimary = false;
                        listMember.Add(m);


                        MemberDetailMaster md = new MemberDetailMaster();
                        md.ID = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("ID")));
                        md.MemberID = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("ID")));

                        string add = reader.GetValue(reader.GetOrdinal("Address")).ToString();
                        string city = reader.GetValue(reader.GetOrdinal("City")).ToString();
                        string province = reader.GetValue(reader.GetOrdinal("Province")).ToString();
                        string country = reader.GetValue(reader.GetOrdinal("Country")).ToString();
                        string postal = reader.GetValue(reader.GetOrdinal("Postal")).ToString();

                        string fAddress = string.Concat(ModifyAddress(add), ",", city, ",", province, ",", country, ",", postal);

                        md.Address = fAddress;
                        md.DOB = (reader.GetValue(reader.GetOrdinal("DOB"))).ToString();
                        //md.Sex = reader.GetValue(reader.GetOrdinal("Sex")).ToString();
                        md.Email = reader.GetValue(reader.GetOrdinal("Email")).ToString();
                        md.THome = reader.GetValue(reader.GetOrdinal("THome")).ToString();
                        md.TBusiness = reader.GetValue(reader.GetOrdinal("TBusiness")).ToString();
                        md.TFax = reader.GetValue(reader.GetOrdinal("TFax")).ToString();
                        md.NewsLetter = true;
                        md.MemberType = reader.GetValue(reader.GetOrdinal("MemberType")).ToString();
                        listMemberDetail.Add(md);

                        MemberAccountMaster mam = new MemberAccountMaster();
                        mam.ID = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("ID")));
                        mam.MemberID = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("ID")));
                        //mam.Mail = reader.GetValue(reader.GetOrdinal("Mail")).ToString();
                        mam.Amount = reader.GetValue(reader.GetOrdinal("PAID")).ToString();
                        mam.PaymentType = reader.GetValue(reader.GetOrdinal("PayType")).ToString();
                        mam.DepositDate = reader.GetValue(reader.GetOrdinal("DepositDate")).ToString();

                        listMemberAccount.Add(mam);
                    }
                }
                InserMemberMaster(listMember, conn, cmd);
                InsertMemberDetailMaster(listMemberDetail, conn, cmd);
                InsertMemberAccountMaster(listMemberAccount, conn, cmd);
            }
        }

        private static void InsertMemberAccountMaster(List<MemberAccountMaster> lma, SqlConnection conn, SqlCommand cmd)
        {
            for (int i = 0; i <= lma.Count(); i++)
            {
                try
                {
                    //SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "INSERT INTO [AGSDB].[dbo].[MemberAccountMaster]  "+
                            " ([MemberID], [Paid], [Amount], [DepositDate], [PaymentType] ,[Comment]) Values  "+
                            "(" + lma[i].ID + "," + 0 + ",'" + Convert.ToString(lma[i].Amount) + "','" + Convert.ToString(lma[i].DepositDate) + "','" + Convert.ToString(lma[i].PaymentType) + "','" + Convert.ToString(lma[i].Comment) + "')";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conn;
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                }
            }
        }

        public static string ModifyAddress(string str)
        {
            int index = str.IndexOf("'");

            if (index != -1)
            {
                str = str.Insert(index, "'");
            }

            return str;
        }

        private static void InsertMemberDetailMaster(List<MemberDetailMaster> lmd, SqlConnection conn, SqlCommand cmd)
        {
            for (int i = 0; i <= lmd.Count(); i++)
            {
                try
                {
                    cmd.CommandText = "INSERT INTO [AGSDB].[dbo].[MemberDetailMaster] ([MemberID], [Address], " +
                    "[DOB], [Sex], [Email] ,[Newsletter], [MemberType], " +
                        "[THome] ,[TBusiness] ,[TFax])" +
                        " Values (" + lmd[i].ID + ", ' " +
                        "" + lmd[i].Address.ToString() + "', " +
                        "'" + lmd[i].DOB.ToString() + "', " +
                        "'', " +
                        "'" + lmd[i].Email.ToString() + "', " +
                        "" + 1 + ", " +
                        "'" + lmd[i].MemberType.ToString() + "', " +
                        "'" + lmd[i].THome.ToString() + "', " +
                        "'" + lmd[i].TBusiness.ToString() + "', " +
                        "'" + lmd[i].TFax.ToString() + "')";


                    //"'" + listMember[i].NewsLetter + "' "+ 

                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conn;

                    cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                }
            }
        }

        private static void InserMemberMaster(List<MemberMaster> list, SqlConnection conn, SqlCommand cmd)
        {
            for (int i = 0; i <= list.Count(); i++)
            {
                try
                {
                    cmd.CommandText = "INSERT INTO [AGSDB].[dbo].[MemberMaster] ([ID] ,[BarcodeId] , " +
                        "[LName] ,[FName] ,[IsPrimary] ,[FamilyId] ,[Title])" +
                        " Values (" + list[i].ID + ", '" + list[i].BarcodeId.ToString() + "', " +
                        "'" + list[i].LName.ToString() + "' " +
                        ", '" + list[i].FName.ToString() + "', " + 0 + ", " +
                        "" + list[i].FamilyId + ", '" + list[i].Title + "' )";

                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conn;

                    cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                }
            }
        }
        #endregion

        #region SimpleInsert
        public static void ConnectDb()
        {
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = connString;

                SqlCommand cmd = new SqlCommand();

                cmd.CommandText = "SELECT " +
                                "MemberNo AS ID, FamilyNo AS FamilyId, " +
                                "Surname AS LName, Name AS FName, [Title] AS Title" +
                                "FROM [Test].[dbo].[Membership$]";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;

                conn.Open();

                GetExcelData(cmd);

                InserMemberMaster(listMember, conn, cmd);
            }
        }

        private static void GetExcelData(SqlCommand cmd)
        {
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    MemberMaster m = new MemberMaster();
                    m.ID = Convert.ToInt32(reader.GetValue(0));
                    m.BarcodeId = string.Concat(Convert.ToInt32(reader.GetValue(0)),
                                                "+",
                                                Convert.ToInt32(reader.GetValue(1)));
                    m.FamilyId = Convert.ToInt32(reader.GetValue(1));
                    m.Title = reader.GetValue(4).ToString();
                    m.LName = reader.GetValue(2).ToString();
                    m.FName = reader.GetValue(3).ToString();
                    m.IsPrimary = false;
                    listMember.Add(m);
                }
            }
        }
        #endregion
    }
}
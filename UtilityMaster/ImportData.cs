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
        public static void ConnectDb()
        {
            Dictionary<string, string> users = new Dictionary<string, string>();
            List<MemberMaster> list = new List<MemberMaster>();
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "Data Source=DEV-Vishal\\SQLEXPRESS;Initial Catalog=AGSDB;Integrated Security=SSPI;";
                // using the code here...

                SqlCommand cmd = new SqlCommand();
                //SqlDataReader reader;

                cmd.CommandText = "SELECT " +
                                "MemberNo AS ID, FamilyNo AS FamilyId, " +
                                "Surname AS LName, Name AS FName, Title as Title" +
                                "FROM [Test].[dbo].[Membership$]";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;

                conn.Open();
                int fid;
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

                        list.Add(m);
                    }
                }

                for (int i = 0; i <= list.Count(); i++)
                {
                    try
                    {
                        cmd.CommandText = "INSERT INTO [AGSDB].[dbo].[MemberMaster] ([ID] ,[BarcodeId] , " +
                            "[LName] ,[FName] ,[IsPrimary] ,[FamilyId] ,[Title])" +
                            " Values (" + list[i].ID + ", '" + list[i].BarcodeId.ToString() + "', "+
                            "'" + list[i].LName.ToString() + "' " +
                            ", '" + list[i].FName.ToString() + "', " + 0 + ", "+
                            "" + list[i].FamilyId + ", '" + list[i].Title + "' )";

                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;

                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                    }
                }
                //SqlCommand command = new SqlCommand("SELECT " +
                //                "MemberNo AS ID, FamilyNo AS FamilyId, " +
                //                "Surname AS LName, Name AS FName " +
                //                "FROM [Test].[dbo].[Membership$]", conn);

                //SqlCommand command = new SqlCommand("INSERT " +
                //                    "INTO [AGSDB].[dbo].[MemberMaster] " +
                //                    "(ID,FName,LName,FamilyId)" +
                //                    "SELECT " +
                //                    "MemberNo AS ID, FamilyNo AS FamilyId, " +
                //                    "Surname AS LName, Name AS FName " +
                //                    "FROM [Test].[dbo].[Membership$]", conn);
                //command.ExecuteNonQuery();
            }
        }
    }
}

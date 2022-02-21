using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using FineArtsSite.Models;

namespace FineArtsSite.HelperMethods
{
    public class Database
    {
        private const string SelectInventory = @"USE FineArts
            SELECT DISTINCT recID, ArtistName, Title, ItemNum, Cost, Sold, InvoiceNum

            FROM FineArts.dbo.Inventory

            -- WHERE CLAUSE -- Place logic for search filter here
            [WHERE]

            ORDER BY ArtistName, recID";

        private const string SelectInvoice = @"USE FineArts
            SELECT DISTINCT RecID, PiecesSold, TotalCost, PaymentType, SaleTime

            FROM FineArts.dbo.Invoices

            -- WHERE CLAUSE -- Place logic for search filter here
            [WHERE]

            ORDER BY recID";

        private const string SelectInventoryUniqArtist = @"USE FineArts
            SELECT DISTINCT ArtistName

            FROM FineArts.dbo.Inventory

           -- WHERE CLAUSE -- Place logic for search filter here
            [WHERE]
            
            ORDER BY ArtistName ASC";

        private const string inputInventory = @"USE FineArts
            INSERT INTO dbo.Inventory
            VALUES ([VALUESIN])";

        private const string inputInvoice = @"USE FineArts
            INSERT INTO dbo.Invoices
            VALUES ([VALUESIN])
            SELECT SCOPE_IDENTITY()";

        private const string updateInventory = @"USE FineArts
            UPDATE dbo.Inventory
            SET [SET]
            WHERE [WHERE]";

        private const string updateInvoice = @"USE FineArts
            UPDATE dbo.Invoices
            SET [SET]
            WHERE [WHERE]";

        private const string deleteInventory = @"USE FineArts
            DELETE FROM dbo.Inventory
            WHERE [WHERE]";
        public static IConfigurationRoot GetConfiguration(string path)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            builder = builder.AddEnvironmentVariables();

            return builder.Build();
        }

        public static string GetConnection()
        {

            string conn = "";
            var configuration = GetConfiguration(Directory.GetCurrentDirectory());
            conn = configuration.GetConnectionString("Inventory");
            return conn;
        }

        public static DataTable PullInventory(string conn)
        {
            SqlConnection sqlconn = new SqlConnection(conn);
            // Get SQL results
            DataTable dt = new DataTable();
            string whereQuery = "";
            string SelectInventoryFinal = SelectInventory.Replace("[WHERE]", whereQuery);
            SqlCommand sc = new SqlCommand(SelectInventoryFinal, sqlconn);
            SqlDataAdapter sda = new SqlDataAdapter(sc);
            sda.Fill(dt);
            return dt;
        }

        public static DataTable PullInventoryUniqArtist(string conn)
        {
            SqlConnection sqlconn = new SqlConnection(conn);
            // Get SQL results
            DataTable dt = new DataTable();
            string whereQuery = "";
            string SelectInventoryFinal = SelectInventoryUniqArtist.Replace("[WHERE]", whereQuery);
            SqlCommand sc = new SqlCommand(SelectInventoryFinal, sqlconn);
            SqlDataAdapter sda = new SqlDataAdapter(sc);
            sda.Fill(dt);
            return dt;
        }

        public static Inventory PullInventorybyID(string conn, int id)
        {
            float cost;
            string sold;
            Inventory model = new Inventory();
            SqlConnection sqlconn = new SqlConnection(conn);
            // Get SQL results
            DataTable dt = new DataTable();
            string whereQuery = "WHERE RecID = '" + id + "'";
            string SelectInventoryFinal = SelectInventory.Replace("[WHERE]", whereQuery);
            SqlCommand sc = new SqlCommand(SelectInventoryFinal, sqlconn);
            SqlDataAdapter sda = new SqlDataAdapter(sc);
            sda.Fill(dt);
            DataRow row = dt.Rows[0];
            int recID = 0;
            int.TryParse(row.ItemArray[0].ToString(), out recID);
            model.recID = recID;
            model.ArtistName = row.ItemArray[1].ToString();
            model.Title = row.ItemArray[2].ToString();
            model.ItemNum = row.ItemArray[3].ToString();
            float.TryParse(row.ItemArray[4].ToString(), out cost);
            model.Cost = cost;
            sold = row.ItemArray[5].ToString();
            if (sold == "Y") model.Sold = true;
            else model.Sold = false;

            return model;
        }

        public static DataTable PullInventorybyArtist(string conn, string artist)
        {
            SqlConnection sqlconn = new SqlConnection(conn);
            // Get SQL results
            DataTable dt = new DataTable();
            string whereQuery = "WHERE ArtistName = '" + artist + "' AND Sold = 'N'";
            string SelectInventoryFinal = SelectInventory.Replace("[WHERE]", whereQuery);
            SqlCommand sc = new SqlCommand(SelectInventoryFinal, sqlconn);
            SqlDataAdapter sda = new SqlDataAdapter(sc);
            sda.Fill(dt);

            return dt;
        }

        public static DataTable PullInventorybyArtistInv(string conn, string artist)
        {
            SqlConnection sqlconn = new SqlConnection(conn);
            // Get SQL results
            DataTable dt = new DataTable();
            string whereQuery = "WHERE ArtistName = '" + artist + "'";
            string SelectInventoryFinal = SelectInventory.Replace("[WHERE]", whereQuery);
            SqlCommand sc = new SqlCommand(SelectInventoryFinal, sqlconn);
            SqlDataAdapter sda = new SqlDataAdapter(sc);
            sda.Fill(dt);

            return dt;
        }

        public static bool InInventory(DataRow row, string conn)
        {
            SqlConnection sqlconn = new SqlConnection(conn);
            string valuesIn = "";
            row[1] = row[1].ToString().Replace(",", "").Replace("'", "");
            row[3] = row[3].ToString().Replace(",", "").Replace("'", "");
            valuesIn += "'" + row[1] + "','" + row[3] + "','" + row[2] + "'," + row[4] + ",'N',''";
            DataTable dt = new DataTable();
            string SelectInventoryFinal = inputInventory.Replace("[VALUESIN]", valuesIn);
            SqlCommand sc = new SqlCommand(SelectInventoryFinal, sqlconn);
            SqlDataAdapter sda = new SqlDataAdapter(sc);
            sda.Fill(dt);
            return true;
        }

        public static void updateDB(Inventory inv, int id, string conn)
        {
            DataTable dt = new DataTable();
            SqlConnection sqlconn = new SqlConnection(conn);
            string sold = "";
            string whereQuery = "RecID = '" + id + "'";
            if (inv.Sold == true) sold = "Y";
            else sold = "N";
            string setQuery = "ArtistName = '" + inv.ArtistName + "', Title = '" + inv.Title + "', ItemNum = '" + inv.ItemNum +
                "', Cost = '" + inv.Cost + "', Sold = '" + sold + "'";
            string UpdateInventoryFinal = updateInventory.Replace("[WHERE]", whereQuery);
            UpdateInventoryFinal = UpdateInventoryFinal.Replace("[SET]", setQuery);
            SqlCommand sc = new SqlCommand(UpdateInventoryFinal, sqlconn);
            SqlDataAdapter sda = new SqlDataAdapter(sc);
            sda.Fill(dt);
        }

        public static void addtoDB(Inventory inv, string conn)
        {
            string sold = "Y";
            if (inv.Sold == false) sold = "N";
            string valuesIn = "";
            valuesIn += "'" + inv.ArtistName + "','" + inv.Title + "','" + inv.ItemNum + "'," + inv.Cost + ",'" + sold + "',''";
            SqlConnection sqlconn = new SqlConnection(conn);
            DataTable dt = new DataTable();
            string SelectInventoryFinal = inputInventory.Replace("[VALUESIN]", valuesIn);
            SqlCommand sc = new SqlCommand(SelectInventoryFinal, sqlconn);
            SqlDataAdapter sda = new SqlDataAdapter(sc);
            sda.Fill(dt);
        }

        public static void deleteDB(int id, string conn)
        {
            string valuesIn = "recID = " + id;
            SqlConnection sqlconn = new SqlConnection(conn);
            DataTable dt = new DataTable();
            string DeleteInventoryFinal = deleteInventory.Replace("[WHERE]", valuesIn);
            SqlCommand sc = new SqlCommand(DeleteInventoryFinal, sqlconn);
            SqlDataAdapter sda = new SqlDataAdapter(sc);
            sda.Fill(dt);
        }

        public static List<SelectListItem> getArtists(DataTable dt, string name)
        {
            List<string> lsArtists = new List<string>(); // using a list to set the datasource for the drop down
            for (int i = 0; i < dt.Rows.Count; i++) // loop through and add from the results to the list
            {
                lsArtists.Add(dt.Rows[i].ItemArray[0].ToString());
            }
            List<SelectListItem> artistDDL = new List<SelectListItem>();
            foreach (var c in lsArtists)
            {
                artistDDL.Add(new SelectListItem
                {
                    Text = c,
                    Value = c
                });
            }
            foreach (var item in artistDDL)
            {
                if (item.Value == name) item.Selected = true;
            }
            return artistDDL;
        }

        public static void addInvoice(string iNums, string total, string pType, string conn)
        {
            string recVals = "";
            string[] iNumsList = iNums.Split(" ");
            string UpdateInventoryFinal = "";
            string valuesIn = "";
            SqlConnection sqlconn = new SqlConnection(conn);
            DataTable dt = new DataTable();


            foreach (string num in iNumsList)
            {
                string whereQuery = "WHERE RecID = '" + num + "'";
                string SelectInventoryFinal = SelectInventory.Replace("[WHERE]", whereQuery);
                SqlCommand sc2 = new SqlCommand(SelectInventoryFinal, sqlconn);
                SqlDataAdapter sda = new SqlDataAdapter(sc2);
                sda.Fill(dt);

                foreach(DataRow row in dt.Rows)
                {
                    recVals += row.ItemArray[0] + " " + row.ItemArray[1] + " " + row.ItemArray[2] + " $" + row.ItemArray[4] + ", ";
                }
                dt.Clear();
            }

            valuesIn += "'" + recVals.Substring(0,recVals.Length - 2) + "','" + total + "','" + pType + "'," + "CURRENT_TIMESTAMP";

            string InputInvoiceFinal = inputInvoice.Replace("[VALUESIN]", valuesIn);
            sqlconn.Open();
            SqlCommand sc = new SqlCommand(InputInvoiceFinal, sqlconn);
            string insertedID = Convert.ToString(sc.ExecuteScalar());

            foreach(string num in iNumsList)
            {

                string whereQuery = "RecID = '" + num + "'";
                string setQuery = "Sold = 'Y', InvoiceNum = '" + insertedID + "'";
                UpdateInventoryFinal = updateInventory.Replace("[WHERE]", whereQuery);
                UpdateInventoryFinal = UpdateInventoryFinal.Replace("[SET]", setQuery);
                sc = new SqlCommand(UpdateInventoryFinal, sqlconn);
                sc.ExecuteScalar();
            }
            sqlconn.Close();
        }

        public static DataTable pullInvoices(string conn)
        {
            SqlConnection sqlconn = new SqlConnection(conn);
            DataTable dt = new DataTable();
            string whereQuery = "";
            string SelectInvoiceFinal = SelectInvoice.Replace("[WHERE]", whereQuery);
            SqlCommand sc = new SqlCommand(SelectInvoiceFinal, sqlconn);
            SqlDataAdapter sda = new SqlDataAdapter(sc);
            sda.Fill(dt);
            return dt;
        }

        public static Invoice pullInvoicesByID(string conn, int id)
        {
            SqlConnection sqlconn = new SqlConnection(conn);
            // Get SQL results
            DataTable dt = new DataTable();
            string whereQuery = "WHERE RecID = '" + id + "'";
            string SelectInventoryFinal = SelectInvoice.Replace("[WHERE]", whereQuery);
            SqlCommand sc = new SqlCommand(SelectInventoryFinal, sqlconn);
            SqlDataAdapter sda = new SqlDataAdapter(sc);
            sda.Fill(dt);
            Invoice model = new Invoice();
            int recID = 0;
            DataRow row = dt.Rows[0];
            int.TryParse(row.ItemArray[0].ToString(), out recID);
            model.recID = recID;
            model.PiecesSold = row.ItemArray[1].ToString();
            model.TotalCost = row.ItemArray[2].ToString();
            model.PaymentType = row.ItemArray[3].ToString();
            return model;
        }

        public static void updateInvoiceDB(Invoice inv, int id, string conn)
        {
            SqlConnection sqlconn = new SqlConnection(conn);
            string whereQuery = "RecID = '" + id + "'";
            string setQuery = "PaymentType = '" + inv.PaymentType + "'";
            string UpdateInvoiceFinal = updateInvoice.Replace("[WHERE]", whereQuery);
            UpdateInvoiceFinal = UpdateInvoiceFinal.Replace("[SET]", setQuery);
            sqlconn.Open();
            SqlCommand sc = new SqlCommand(UpdateInvoiceFinal, sqlconn);
            
            sc.ExecuteScalar();

            sqlconn.Close();
        }

        public static DataTable GetArtistsSoldWork(string conn, string name)
        {
            SqlConnection sqlconn = new SqlConnection(conn);
            // Get SQL results
            DataTable dt = new DataTable();
            string whereQuery = "WHERE ArtistName = '" + name + "' AND Sold = 'Y'";
            string SelectInventoryFinal = SelectInventory.Replace("[WHERE]", whereQuery);
            SqlCommand sc = new SqlCommand(SelectInventoryFinal, sqlconn);
            SqlDataAdapter sda = new SqlDataAdapter(sc);
            sda.Fill(dt);

            return dt;
        }
    }
}

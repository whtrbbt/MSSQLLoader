using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace MSSQLLoader
{
    class Program
    {
        static void Main(string[] args)
        {
            string dirpath = @ConfigurationManager.AppSettings.Get("dirpath");
            string MSSQLtableName = @ConfigurationManager.AppSettings.Get("MSSQLtableName");


            SqlConnectionStringBuilder csbuilder =
                new SqlConnectionStringBuilder("");

            csbuilder["Server"] = @ConfigurationManager.AppSettings.Get("MSSQL_Server");
            csbuilder["UID"] = @ConfigurationManager.AppSettings.Get("UID");
            csbuilder["Password"] = @ConfigurationManager.AppSettings.Get("Password");
            csbuilder["Connect Timeout"] = 6000;
            csbuilder["integrated Security"] = true; //для коннекта с локальным экземпляром
            //csbuilder["Multisubnetfailover"] = "True";
            //csbuilder["Trusted_Connection"] = true;

            Console.WriteLine(csbuilder.ConnectionString);

            if(@ConfigurationManager.AppSettings.Get("CLEAR_TABLE") == "1")
                CSVUtility.CSVUtility.ClearTargetTable(csbuilder.ConnectionString, MSSQLtableName);


            var dir = new DirectoryInfo(dirpath); // папка с файлами 

            foreach(FileInfo file in dir.GetFiles())
            {
                Console.WriteLine(Path.GetFileName(file.FullName));
            }

            Console.WriteLine("Загружаем данные на сервер MS SQL.");

            DataTable CSVtable = new DataTable();

            foreach(FileInfo file in dir.GetFiles())
            {

                Console.WriteLine(Path.GetFileName(file.FullName));
                CSVtable = CSVUtility.CSVUtility.GetDataTabletFromCSVFile(file.FullName);
                CSVUtility.CSVUtility.InsertDataIntoSQLServerUsingSQLBulkCopy(CSVtable, MSSQLtableName, csbuilder.ConnectionString);
                CSVtable = null;
                //GC.Collect(1, GCCollectionMode.Forced);
            }


            //ShowTable(CSVtable);



            Console.WriteLine("Готово!");
            Console.ReadKey();
            //CSVUtility.InsertDataIntoSQLServerUsingSQLBulkCopy(CSVtable, MSSQLtableName, csbuilder.ConnectionString);
        }
    }
}
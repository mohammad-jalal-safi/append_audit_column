
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Services
{
    class Migration
    {
        public void AddColumn()
        {
            string[,,] List = { {{ "created_by", "int8", "1" }},
                                {{ "created_on", "timestamp", "'1900-01-01 12:00:00.848564'" }},
                                {{ "modified_by", "int8", "1" }},
                                {{ "modified_on", "timestamp", "'1900-01-01 12:00:00.848564'" }},
                             };

            List<model> list = new List<model>();

            for (int i =0; i<= 3; i++)
            {
                model model = new model()
                {
                    ColumnName = List[i, 0, 0],
                    ColumnDataType = List[i, 0, 1],
                    ColumnValue = List[i, 0, 2]
                };
                list.Add(model);
            }

            using (Npgsql.NpgsqlConnection con = new Npgsql.NpgsqlConnection("Host=192.168.5.22:5432;Database=APMIS;Username=postgres;Password=nSia@123#$"))
            {
                con.Open();

                //List<string> schemaList = con.GetSchema("Tables").AsEnumerable().Select(s => s[1].ToString()).Distinct().ToList().Where(e=> e != "prc" && e != "sec" && e != "public" && e != "look").ToList();
                List<string> schemaList = con.GetSchema("Tables").AsEnumerable().Select(s => s[1].ToString()).Distinct().ToList().Where(e => e == "look").ToList();

                var notAdded = new List<string>() { "code_sequence", "modules",
                    "pages", "document_type", "section"
                };

                var lookEntity = new List<string>() { "organization", "unit" };

                schemaList.ForEach(schema =>
                {
                    List<string> tableList = con.GetSchema("Tables").AsEnumerable().Where(e => e[1].ToString() == schema).Select(s => s[2].ToString()).ToList().Where(e => lookEntity.Contains(e)).ToList();

                    tableList.ForEach(table =>
                    {
                        list.ForEach(column =>
                        {
                            var GetColumn = "SELECT * FROM information_schema.columns WHERE column_name= '" + column.ColumnName + "' AND table_schema = '" + schema + "' AND table_name = '" + table + "'; ";

                            using var check = new NpgsqlCommand(GetColumn, con);
                            NpgsqlDataReader dr = check.ExecuteReader();
                            var x = dr.Read() ? dr["column_name"].ToString() : "";

                            con.Close();
                            con.Open();

                            if (x == "")
                            {
                                var com = "ALTER TABLE " + schema + "." + table + "  ADD COLUMN  \"" + column.ColumnName + "\"  " + column.ColumnDataType + " ;" + " Update  " + schema + "." + table + " set  \"" + column.ColumnName + "\"  = " + column.ColumnValue + ";" + " ALTER TABLE " + schema + "." + table + " ALTER COLUMN   \"" + column.ColumnName + "\"   SET NOT NULL; ";

                                using var cmd = new NpgsqlCommand(com, con);

                                cmd.ExecuteScalar();
                                Console.WriteLine(schema + "." + table + "." + column.ColumnName + "  :  Is Succesfully Added!");
                            }
                            else
                            {
                                Console.WriteLine(schema + "." + table + "." + column.ColumnName +  "  : Already Exist");
                            }
                        });
                    });
                });
            }
        }
    }
    class model
    {
        public string ColumnName { get; set; }
        public string ColumnDataType { get; set; }
        public string ColumnValue { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PigLatin
{
    class Database
    {
        OdbcCommand cmd;
        OdbcConnection conn;
        OdbcDataReader reader;

        public void ConnectDB()
        {
            try
            {
                conn = new OdbcConnection("Dsn=SimpleDSN");
                conn.Open();
                Console.WriteLine("Connected to SimpleDB");
            }
            catch (OdbcException oe)
            {
                Console.WriteLine(oe.Message, "Database Connection Error");
            }
        }

        public void DisconnectDB()
        {
            conn.Close();
        }

        private bool ExecuteCommand(string query)
        {
            try
            {
                ConnectDB();
                cmd = conn.CreateCommand();
                cmd.CommandText = query;
                cmd.ExecuteReader();
                return true;
            }
            catch (OdbcException ex)
            {
                Console.WriteLine(ex.Message, "Database Execute Command Error");
                return false;
            }
            finally
            {
                DisconnectDB();
            }
        }

        public OdbcDataReader ExecuteQuery(String query)
        {
            try
            {
                ConnectDB();
                cmd = conn.CreateCommand();
                cmd.CommandText = query;
                return cmd.ExecuteReader();
            }
            catch (OdbcException ex)
            {
                Console.WriteLine(ex.Message, "Database Execute Query Error");
                return null;
            }
        }

        //Add a new user to the database
        public bool AddTelUser(string chatID, string name, string pass)
        {
            bool ans = false;
            reader = ExecuteQuery("SELECT [chatID] FROM [telUser] WHERE [chatID] = '" + chatID + "'");

            if (!(reader.HasRows))
            {
                reader.Close();
                ans = ExecuteCommand("INSERT INTO [telUser] VALUES ('" + chatID + "', '" + name + "', '"+ pass +"')");
                return ans;
            }
            else
                return ans;
        }

        //Add user to task
        public bool AddToTask(string taskID, string chatID)
        {
            bool ans = false;
            reader = ExecuteQuery("SELECT [chatID] FROM [crTable] WHERE [taskID] = '" + taskID + "' AND [chatID] = '" + chatID + "';");

            if (!(reader.HasRows))
            {
                reader.Close();
                ans = ExecuteCommand("INSERT INTO [crTable] VALUES('" + taskID + "', '" + chatID + "')");
                
                return ans;
            }
            else
                return ans;
        }

        //Get task members
        public List<string> GetMemberIDs(string taskID)
        {
            List<string> ans = new List<string>(); 
            reader = ExecuteQuery("SELECT [chatID] FROM [crTable] WHERE [taskID] = '" + taskID + "'");

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    ans.Add(reader.GetString(0));
                }
                return ans;
            }
            else
                return null;
        }

        //Check Password
        public bool CheckPass(string chatID, string pass)
        {
            bool ans = false;
            reader = ExecuteQuery("SELECT [chatID] FROM [telUser] WHERE [chatID] = '" + chatID + "' AND [pass]= '"+ pass +"'");
            if (reader.HasRows)
            {
                ans = true;
            }
            return ans;
        }
        
        //Get taskID's
        public List<string> GetTaskIDs()
        {
            List<string> ans = new List<string>();
            reader = ExecuteQuery("SELECT [taskID] FROM [task]");

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    ans.Add(reader.GetString(0));
                }
                return ans;
            }
            else
                return null;
        }

        //Get the task description
        public OdbcDataReader GetTask(string taskID)
        {
            reader = ExecuteQuery("SELECT [tDesc] FROM [task] WHERE [taskID] = '" + taskID + "'");

            if (reader.HasRows)
                return reader;
            else
                return null;
        }
    }
}

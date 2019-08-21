using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Data.Common;

namespace QDataManager
{
    public abstract class QDatabase
    {
        //Abstract definitions of DB objects
        public virtual DbConnection Connection { get; set; }
        public virtual DbCommand Command { get; set; }
        public abstract DbCommand GetCommand(string sql);
        public abstract DataTable Execute(string sql);
        public abstract int ExecuteNonQuery(DbCommand commandDb);
        public abstract object ExecuteScalar(DbCommand commandDb);
    }
    class SqlQDBManager : QDatabase
    {
        // This project is designed to use MSSQL server. If we want to use other DB sources we will rewrite this class. 
        private DbConnection connection = null;
        private DbCommand command = null;
        public override DbConnection Connection
        {
            get
            {
                if (connection == null )
                {
                    connection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServerConnection"].ConnectionString);
                }
                return connection;
            }
            set
            {
                connection = value;
            }
        }

        public override DbCommand Command
        {
            get
            {
                if (command == null)
                {
                    command = new SqlCommand();
                    command.Connection = Connection;
                }
                return command;
            }
            set
            {
                command = value;
            }
        }

        public override DbCommand GetCommand(string sql)
        {
            SqlCommand sqlCmd = (SqlCommand)Command;
            sqlCmd.CommandText = sql;
            return sqlCmd;
        }
        public override DataTable Execute(string sql)
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = (SqlCommand)Command;
            cmd.CommandText = sql;
            cmd.Connection.Open();
            dt.Load(cmd.ExecuteReader());
            cmd.Connection.Close();
            return dt;
        }

        public override int ExecuteNonQuery(DbCommand commandDb)
        {
            SqlCommand cmd = (SqlCommand)commandDb;
            cmd.Connection.Open();
            int result = cmd.ExecuteNonQuery();
            cmd.Connection.Close();
            return result;
        }

        public override object ExecuteScalar(DbCommand commandDb)
        {
            SqlCommand cmd = (SqlCommand)commandDb;
            cmd.Connection.Open();
            object result = cmd.ExecuteScalar();
            cmd.Connection.Close();
            return result;
        }

    }
    class DBManager
    {
        // We can choose the DB source from config file
        public QDatabase GetDatabaseObject()
        {
            QDatabase q = null;
            if (ConfigurationManager.AppSettings["DBType"] == "SqlServerDatabase")
                q = new SqlQDBManager();
            return q;
        }
    }
    public class QDataManager
    {
        //DB layer for CRUD operations
        public DataTable GetQuestionData()
        {
            //Gets the columns of Question table 
            DBManager mg = new DBManager();
            QDatabase qdb = mg.GetDatabaseObject();
            
            string sql = "SELECT QID,QIDENTIFIER,DESCRIPTION,CHOICETYPE FROM QUESTION(NOLOCK)";
            DataTable dtQ = qdb.Execute(sql);
            dtQ.TableName = "QUESTION";
            
            return dtQ;
        }
        public DataTable GetChoiceData()
        {
            //Gets the columns of Choice table 
            DBManager mg = new DBManager();
            QDatabase qdb = mg.GetDatabaseObject();
            
            string sql = "SELECT CID,QID,CIDENTIFIER,DESCRIPTION FROM CHOICE";
            DataTable dtQ = qdb.Execute(sql);
            dtQ.TableName = "CHOICE";
            
            return dtQ;
        }
        public DataTable GetAnswerData()
        {
            //get choice list and if exists get the answer count of each respond 
            DBManager mg = new DBManager();
            QDatabase qdb = mg.GetDatabaseObject();

            string sql = @"SELECT C.CID,C.QID, CIDENTIFIER, DESCRIPTION, COUNT(RESPONDID) ANSWER FROM CHOICE C(nolock)
                            LEFT JOIN ANSWERS A(NOLOCK) ON A.CID= C.CID
                            GROUP BY C.CID,C.QID, CIDENTIFIER, DESCRIPTION";
            DataTable dtQ = qdb.Execute(sql);
            dtQ.TableName = "CHOICE";

            return dtQ;
        }
        public int GetNumOfRespondent()
        {
            //Gets the number of responds to questionaire 
            DBManager mg = new DBManager();
            QDatabase qdb = mg.GetDatabaseObject();
            string sql = "SELECT COUNT(*) FROM (select DISTINCT RESPONDID FROM ANSWERS(nolock))K";
            qdb.Command.CommandText = sql;
            int noOfRespondent = Convert.ToInt32(qdb.ExecuteScalar(qdb.Command));

            return noOfRespondent;
        }
        private bool CheckCID(int cId)
        {
            //Checks the choice id existance linked to the question. Written as a precaution against data integrity corruption
            bool result = false;
            DBManager mg = new DBManager();
            QDatabase qdb = mg.GetDatabaseObject();

            string sql = "SELECT CID FROM CHOICE C(NOLOCK) INNER JOIN QUESTION Q(NOLOCK) ON Q.QID=C.QID WHERE CID=@CID";
            qdb.Command.CommandText = sql;
            DbParameter param = qdb.Command.CreateParameter();
            param.ParameterName = "@CID";
            param.Value = cId;
            qdb.Command.Parameters.Add(param);
            object resultValue= qdb.ExecuteScalar(qdb.Command);
            if (resultValue != null) result = true;
            return result;
        }
        public int InsertAnswer(int cId, string responseId)
        {
            DBManager mg = new DBManager();
            QDatabase qdb = mg.GetDatabaseObject();

            if (CheckCID(cId))
            {
                //Inserts each answer selected by the respondent
                string sql = "INSERT INTO ANSWERS (CID,RESPONDID,DATETIME) VALUES (@CID,@RESPONDID,@DATETIME)";
                qdb.Command.CommandText = sql;
                DbParameter param = qdb.Command.CreateParameter();
                param.ParameterName = "@CID";
                param.Value = cId;
                qdb.Command.Parameters.Add(param);
                param = qdb.Command.CreateParameter();
                param.ParameterName = "@RESPONDID";
                param.Value = responseId;
                qdb.Command.Parameters.Add(param);
                param = qdb.Command.CreateParameter();
                param.ParameterName = "@DATETIME";
                param.Value = DateTime.Now;
                qdb.Command.Parameters.Add(param);

                return qdb.ExecuteNonQuery(qdb.Command);
            }
            else
            {
                // no insertions done
                return 0;
            }
        }

    }
}

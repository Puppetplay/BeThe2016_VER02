//
// 데이터베이스 연결 매니저
//

using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Linq;
using BeThe.Item;
using System.Net;

namespace BeThe.Util
{
    public enum DatabaseInfo
    {
        MyHouse,
        Company,
        SkyCenter,
        Server
    }

    public class DatabaseManager
    {
        #region ♥ Property & Values

        private static String DATA_SOURCE;
        private static String USER_ID;
        private static String PASSWORD;
        private static String CATALOG;

        public DataContext DataContext { get; set; }

        #endregion

        #region ♥ Constructor

        public DatabaseManager()
        {
            //  데이터 베이스 컨텍스트를 생성자에서 하나 생성한다.
            SetDatabaseInfo(GetDatabaseInfo());
            var connection = new SqlConnection(GetConnString());
            DataContext = new DataContext(connection);

        }

        #endregion

        #region ♥ Public Functions

        // 조회하기
        public IQueryable<T> SelectAll<T>() where T : DbItemBase
        {
            var table = DataContext.GetTable<T>();
            var query = from item in table
                        select item;
            return query;
        }

        // 데이터 저장
        public void Save<T>(IEnumerable<T> items) where T : DbItemBase
        {
            if (items.Count() > 0)
            {
                Type type = items.First().GetType();
                var table = DataContext.GetTable(type);
                Int32 count = 0;
                foreach (DbItemBase item in items)
                {
                    count++;
                    table.InsertOnSubmit(item);
                    if (count > 200)
                    {
                        count = 0;
                        DataContext.SubmitChanges();
                    }
                }
                DataContext.SubmitChanges();
            }
        }

        #endregion

        #region ♥ Private Functions

        private String GetConnString()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = DATA_SOURCE;
            builder.InitialCatalog = CATALOG;
            builder.UserID = USER_ID;
            builder.Password = PASSWORD;
            return builder.ConnectionString;
        }

        private void SetDatabaseInfo(DatabaseInfo databaseInfo)
        {
            if (databaseInfo == DatabaseInfo.MyHouse)
            {
                DATA_SOURCE = "MIN-PC";
                USER_ID = "sa";
                PASSWORD = "1";
                CATALOG = "BETHE2016_VER02";
            }
            else if(databaseInfo == DatabaseInfo.Server)
            {
                DATA_SOURCE = "61.73.185.162,3915";
                USER_ID = "sa";
                PASSWORD = "@1014vkfl";
                CATALOG = "BETHE2016_VER02";
            }
        }

        private DatabaseInfo GetDatabaseInfo()
        {
            String strHostName = string.Empty;
            strHostName = Dns.GetHostName();
            Console.WriteLine("Local Machine's Host Name: " + strHostName);
            // Then using host name, get the IP address list..
            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
            IPAddress[] addrs = ipEntry.AddressList;
            foreach(var addr in addrs)
            {
                if(addr.ToString() == "192.168.1.100")
                {
                    return DatabaseInfo.MyHouse;
                }
                if (addr.ToString() == "61.73.185.162")
                {
                    return DatabaseInfo.Server;
                }
            }
            throw new Exception("아이피 매칭실패");
        }
        #endregion
    }
}

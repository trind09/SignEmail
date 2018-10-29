using MimeKit.Cryptography;
using System.Data.SQLite;
using System.IO;

namespace WebApplication2.MineMail
{
    public class MySecureMimeContext : DefaultSecureMimeContext
    {
        public MySecureMimeContext()
            : base(OpenDatabase(System.Web.Hosting.HostingEnvironment.MapPath("~/TempFiles/certdb.sqlite")))
        {
        }

        static IX509CertificateDatabase OpenDatabase(string fileName)
        {
            var builder = new SQLiteConnectionStringBuilder();
            builder.DateTimeFormat = SQLiteDateFormats.Ticks;
            builder.DataSource = fileName;

            if (!File.Exists(fileName))
                SQLiteConnection.CreateFile(fileName);

            var sqlite = new SQLiteConnection(builder.ConnectionString);
            sqlite.Open();

            return new SqliteCertificateDatabase(sqlite, "password");
        }
    }
}
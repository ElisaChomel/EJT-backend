namespace judo_backend.Services
{
    public class BaseService
    {
        public string ConnectionString;

        public BaseService(IConfiguration configuration)
        {
            this.ConnectionString = configuration.GetSection("ConnectionString").Value;
        }

        public String FormatValue(string value)
        {
            return value.Replace("'", "'").Replace("\"", "\"");
        }
    }
}

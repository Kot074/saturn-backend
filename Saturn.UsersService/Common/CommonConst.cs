namespace Saturn.UsersService.Common
{
    public class CommonConst
    {
        /// <summary>
        /// Название переменной окружения, содержащей строку подключения к БД.
        /// </summary>
        public const string ConnectionStringVariable = "SATURN_DB_CONNECTION_STRING";

        /// <summary>
        /// Название поля в конфигурационном файле, содержащего строку подключения к БД.
        /// </summary>
        public const string ConfigurationConnectionStringField = "DbConnectionString";
    }
}

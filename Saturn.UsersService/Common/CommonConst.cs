namespace Saturn.UsersService.Common
{
    public class CommonConst
    {
        /// <summary>
        /// Название переменной окружения, содержащей строку подключения к БД.
        /// </summary>
        public const string ConnectionStringVariable = "SATURN_DB_CONNECTION_STRING";

        /// <summary>
        /// Название переменной окружения, содержащей адрес электронной почты владельца продукта.
        /// </summary>
        public const string OwnerEmailVariable = "SATURN_OWNER_EMAIL";

        /// <summary>
        /// Название переменной окружения, содержащей пароль владельца продукта.
        /// </summary>
        public const string OwnerPasswordVariable = "SATURN_OWNER_PASSWORD";

        /// <summary>
        /// Название поля в конфигурационном файле, содержащего строку подключения к БД.
        /// </summary>
        public const string ConfigurationConnectionStringField = "DbConnectionString";

        /// <summary>
        /// Название поля в конфигурационном файле, содержащего адрес электронной почты владельца продукта.
        /// </summary>
        public const string ConfigurationOwnerEmailField = "OwnerEmail";

        /// <summary>
        /// Название поля в конфигурационном файле, содержащего пароль владельца продукта.
        /// </summary>
        public const string ConfigurationOwnerPasswordField = "OwnerPassword";
    }
}

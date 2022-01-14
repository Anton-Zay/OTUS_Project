namespace SecretSantaBot
{
    public static class Config
    {
        public static string Token { get; set; } = "5095584182:AAGrGSLjY0lAKfRhKaD8YqYK_ALVC7tDe60";
        public static string BotName { get; set; } = "secret.santaBot";
        public static string BotUserName { get; set; } = "secret_joulupukki_Bot";
        public static string SqlConnectionStringToOtus_SecretSantaBase = "User ID=postgres; Password = 'avz';Host=localhost;Port=5432;Database=otus_SecretSantaBase";
    }
}
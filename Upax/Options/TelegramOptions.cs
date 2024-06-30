namespace Upax.Options;

public class TelegramOptions
{
    public const string Telegram = nameof(Telegram);
    public string Token { get; set; } = string.Empty;
    public string PumpsUrl { get; set; } = string.Empty;
}
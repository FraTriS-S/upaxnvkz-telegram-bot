namespace Upax.Options;

public class TelegramOptions
{
    public const string Telegram = nameof(Telegram);
    public string Token { get; set; } = string.Empty;
    public string UpaxOtherGroupUrl { get; set; } = string.Empty;
    public string UpaxShirtPackagesGroupUrl { get; set; } = string.Empty;
    public string UpaxContainersGroupUrl { get; set; } = string.Empty;
    public string UpaxGlassesGroupUrl { get; set; } = string.Empty;
    public string UpaxDisposableTablewareGroupUrl { get; set; } = string.Empty;
    public string UpaxConsumablesGroupUrl { get; set; } = string.Empty;
    public string UpaxPackingPackegesGroupUrl { get; set; } = string.Empty;
}
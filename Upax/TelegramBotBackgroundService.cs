using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Upax.Options;

namespace Upax;

public class TelegramBotBackgroundService : BackgroundService
{
    private readonly ILogger<TelegramBotBackgroundService> _logger;
    private readonly TelegramOptions _telegramOptions;

    public TelegramBotBackgroundService(ILogger<TelegramBotBackgroundService> logger,
        IOptions<TelegramOptions> telegramOptions)
    {
        _logger = logger;
        _telegramOptions = telegramOptions.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var botClient = new TelegramBotClient(_telegramOptions.Token);

        ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates = []
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            await botClient.ReceiveAsync(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandleErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: stoppingToken);
        }
    }

    async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        // Only process Message updates: https://core.telegram.org/bots/api#message
        if (update.Message is not { } message)
            return;
        // Only process text messages
        if (message.Text is not { } messageText)
            return;

        InlineKeyboardMarkup inlineKeyboard = new([
            [
                InlineKeyboardButton.WithUrl("Пакеты фасовочные", _telegramOptions.UpaxPackingPackegesGroupUrl),
                InlineKeyboardButton.WithUrl("Пакеты-майки", _telegramOptions.UpaxShirtPackagesGroupUrl),
            ],
            [
                InlineKeyboardButton.WithUrl("Контейнеры", _telegramOptions.UpaxContainersGroupUrl),
                InlineKeyboardButton.WithUrl("Стаканы", _telegramOptions.UpaxGlassesGroupUrl),
            ],
            
            [
                InlineKeyboardButton.WithUrl("Одноразовая посуда", _telegramOptions.UpaxDisposableTablewareGroupUrl),
            ],
            [
                InlineKeyboardButton.WithUrl("Расходные материалы", _telegramOptions.UpaxConsumablesGroupUrl),
            ],
            [
                InlineKeyboardButton.WithUrl("Другое", _telegramOptions.UpaxOtherGroupUrl)
            ],
        ]);


        var chatId = message.Chat.Id;

        _logger.LogDebug($"Received a '{messageText}' message in chat {chatId}.");

        // Echo received message text
        Message sentMessage = await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: "Выберите категорию товара",
            replyMarkup: inlineKeyboard
        );
    }

    Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }
}
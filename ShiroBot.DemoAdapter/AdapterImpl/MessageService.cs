using Newtonsoft.Json;
using ShiroBot.SDK.Adapter;

namespace ShiroBot.DemoAdapter.AdapterImpl;

public class MessageService : IMessageService
{
    public async Task<SendPrivateMessageResponse> SendPrivateMessageAsync(SendPrivateMessageRequest request)
    {
        var json = JsonConvert.SerializeObject(request);
        Console.WriteLine(json);
        Console.WriteLine("test from adapter.");
        await Task.CompletedTask;
        return new SendPrivateMessageResponse(123, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
    }

    public Task<SendGroupMessageResponse> SendGroupMessageAsync(SendGroupMessageRequest request)
    {
        throw new NotImplementedException();
    }

    public Task RecallPrivateMessageAsync(RecallPrivateMessageRequest request)
    {
        throw new NotImplementedException();
    }

    public Task RecallGroupMessageAsync(RecallGroupMessageRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<GetMessageResponse> GetMessageAsync(GetMessageRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<GetHistoryMessagesResponse> GetHistoryMessagesAsync(GetHistoryMessagesRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<GetResourceTempUrlResponse> GetResourceTempUrlAsync(GetResourceTempUrlRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<GetForwardedMessagesResponse> GetForwardedMessagesAsync(GetForwardedMessagesRequest request)
    {
        throw new NotImplementedException();
    }

    public Task MarkMessageAsReadAsync(MarkMessageAsReadRequest request)
    {
        throw new NotImplementedException();
    }
}

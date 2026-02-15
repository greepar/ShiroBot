using System.Text;
using EmbedIO;
using Milky.Net.Model;
using PuppeteerSharp;
using QBotSharp.SDK;
using QBotSharp.SDK.Plugin;

namespace QBotSharp.PluginDemo;

public class DemoPlugin : IBotPlugin
{
    private long[] AllowGroups { get; } = [622603336, 742274811];
    public string Name { get; } = "DemoPlugin";
    
    
    public async Task OnLoad(IBotContext context)
    {
        // Logger.NoLogging();


        var server = new WebServer(o => o
                .WithUrlPrefix("http://localhost:8080")
                .WithMode(HttpListenerMode.EmbedIO))

            // GET 接口
            .WithAction("/", HttpVerbs.Get, ctx =>
                ctx.SendStringAsync("Hello World!", "text/plain", Encoding.UTF8));


        _ = server.RunAsync();

        Console.WriteLine($"已加载插件:{Name}");
        var segments = new OutgoingSegment[]
        {
            new TextOutgoingSegment($"hello from plugin:{Name}")
        };
        var request = new SendPrivateMessageRequest(1034028486, segments);
        //测试发送私聊消息
        Console.WriteLine("开始获取群文件...");
        await context.Message.SendPrivateMessageAsync(request);
        var groupFiles = await context.File.GetGroupFilesAsync(new GetGroupFilesRequest(887098745));
        var groupFileList = groupFiles.Files;
        var groupFileNames = groupFileList.Select(f => f.FileName).ToList();
        Console.WriteLine($"群文件列表: {string.Join(", ", groupFileNames)}");

        context.Event.GroupMessageReceived += async message =>
        {
            if (!AllowGroups.Contains(message.Group.GroupId))
            {
                Console.WriteLine("不是指定群，忽略处理");
                return;
            }

            var messageSegments = message.Segments;
            var text = messageSegments.OfType<TextIncomingSegment>().ToArray();
            //拼接所有text段的文本内容
            var textContent = string.Join("", text.Select(t => t.Text));
            Console.WriteLine($"收到群消息: {textContent}");
            if (textContent.StartsWith("#status"))
            {
                var browser = await BrowserManager.GetBrowserAsync();
                await using var page = await browser.NewPageAsync();
                await page.SetExtraHttpHeadersAsync(new Dictionary<string, string>
                {
                    { "Accept-Language", "zh-CN,zh;q=0.9" }
                });

                await page.EvaluateFunctionOnNewDocumentAsync("""
                                                              () => {
                                                                  Object.defineProperty(navigator, 'language', {
                                                                      get: function() { return 'zh-CN'; }
                                                                  });
                                                                  Object.defineProperty(navigator, 'languages', {
                                                                      get: function() { return ['zh-CN', 'zh']; }
                                                                  });
                                                              }
                                                              """);

                var hour = DateTime.Now.Hour;
                var theme = hour is >= 18 or < 6 ? "dark" : "light";

                await page.EmulateMediaFeaturesAsync([
                    new MediaFeatureValue { Value = theme, MediaFeature = MediaFeature.PrefersColorScheme }
                ]);


                await page.GoToAsync("https://monitor.oeo.one/instance/52d7c676-a120-4de3-980a-85a82a2f2ceb");
                await page.WaitForSelectorAsync(
                    "#root > div > div > main > div > div.rt-Flex.rt-r-fd-column.rt-r-ai-center.rt-r-gap-4.w-full.max-w-screen > div.w-full.overflow-x-auto.px-2 > div > div > button:nth-child(4) > span.rt-SegmentedControlItemLabel");
                await page.ClickAsync(
                    "#root > div > div > main > div > div.rt-SegmentedControlRoot.rt-r-size-2.rt-variant-surface > button:nth-child(2) > span.rt-SegmentedControlItemLabel > span.rt-SegmentedControlItemLabelInactive");
                await page.WaitForSelectorAsync(
                    "#root > div > div > main > div > div.rt-Flex.rt-r-fd-column.rt-r-ai-center.rt-r-gap-4.w-full.max-w-screen > div.w-full.overflow-x-auto.px-2 > div > div > button:nth-child(4) > span.rt-SegmentedControlItemLabel");
                await page.ClickAsync(
                    "#root > div > div > main > div > div.rt-Flex.rt-r-fd-column.rt-r-ai-center.rt-r-gap-4.w-full.max-w-screen > div.w-full.overflow-x-auto.px-2 > div > div > button:nth-child(4) > span.rt-SegmentedControlItemLabel");


                await page.SetViewportAsync(new ViewPortOptions
                {
                    Width = 1000
                });

                await page.WaitForNetworkIdleAsync();
                await Task.Delay(2500);


                await page.ClickAsync("#cut-peak > span");
                var pic = await page.ScreenshotBase64Async(new ScreenshotOptions { FullPage = true });

                Console.WriteLine("截图已生成，准备发送...");

                await page.CloseAsync();
                var finSegments = new OutgoingSegment[]
                {
                    new ImageOutgoingSegment(new MilkyUri($"base64://{pic}"), "[图片]")
                };
                var finRequest = new SendGroupMessageRequest(message.Group.GroupId, finSegments);
                await context.Message.SendGroupMessageAsync(finRequest);
            }
        };
    }

    
    
    public async Task OnUnload()
    {
        await Task.CompletedTask;
        throw new NotImplementedException();
    }
}
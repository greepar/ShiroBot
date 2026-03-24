using ShiroBot.Model.Common;

namespace ShiroBot.SDK.Plugin;

public static class IncomingMessageExtensions
{
    public static string GetPlainText(this GroupIncomingMessage message) =>
        message.Segments.GetPlainText();

    public static string GetPlainText(this FriendIncomingMessage message) =>
        message.Segments.GetPlainText();

    public static string GetPlainText(this IReadOnlyList<IncomingSegment> segments) =>
        string.Concat(segments.OfType<TextIncomingSegment>().Select(segment => segment.Text));
}

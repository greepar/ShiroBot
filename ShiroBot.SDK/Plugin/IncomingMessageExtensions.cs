using ShiroBot.Model.Common;

namespace ShiroBot.SDK.Plugin;

public static class IncomingMessageExtensions
{
    public static string GetPlainText(this GroupIncomingMessage message) =>
        message.Segments.GetPlainText();

    public static string GetPlainText(this FriendIncomingMessage message) =>
        message.Segments.GetPlainText();

    public static bool HasMention(this GroupIncomingMessage message) =>
        message.Segments.HasMention();

    public static bool HasMention(this FriendIncomingMessage message) =>
        message.Segments.HasMention();

    public static bool HasMention(this GroupIncomingMessage message, long userId) =>
        message.Segments.HasMention(userId);

    public static bool HasMention(this FriendIncomingMessage message, long userId) =>
        message.Segments.HasMention(userId);

    public static bool HasMentionAll(this GroupIncomingMessage message) =>
        message.Segments.HasMentionAll();

    public static ReplyIncomingSegment? GetReply(this GroupIncomingMessage message) =>
        message.Segments.GetReply();

    public static ReplyIncomingSegment? GetReply(this FriendIncomingMessage message) =>
        message.Segments.GetReply();

    private static string GetPlainText(this IReadOnlyList<IncomingSegment> segments) =>
        string.Concat(segments.OfType<TextIncomingSegment>().Select(segment => segment.Text));

    private static bool HasMention(this IReadOnlyList<IncomingSegment> segments) =>
        segments.OfType<MentionIncomingSegment>().Any();

    private static bool HasMention(this IReadOnlyList<IncomingSegment> segments, long userId) =>
        segments.OfType<MentionIncomingSegment>().Any(segment => segment.UserId == userId);

    private static bool HasMentionAll(this IReadOnlyList<IncomingSegment> segments) =>
        segments.OfType<MentionAllIncomingSegment>().Any();

    private static ReplyIncomingSegment? GetReply(this IReadOnlyList<IncomingSegment> segments) =>
        segments.OfType<ReplyIncomingSegment>().FirstOrDefault();
}

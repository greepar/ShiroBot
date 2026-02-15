namespace QBotSharp.SDK.Models;

public enum QqProtocolType
{
    Windows,
    Linux,
    Macos,
    AndroidPad,
    AndroidPhone,
    Ipad,
    Iphone,
    Harmony,
    Watch
}

public enum Sex
{
    Male,
    Female,
    Unknown
}

public record LoginInfo(long Uin, string Nickname);

public record ImplInfo(
    string ImplName,
    string ImplVersion,
    string QqProtocolVersion,
    QqProtocolType QqProtocolType,
    string MilkyVersion = "1.1"
);

public record UserProfile(
    string Nickname,
    string Qid,
    int Age,
    Sex Sex,
    string Remark,
    string Bio,
    int Level,
    string Country,
    string City,
    string School
);

public record FriendEntity(long UserId, string Nickname, string Remark);

public record GroupEntity(long GroupId, string GroupName, int MemberCount, int MaxMemberCount);

public record GroupMemberEntity(
    long GroupId,
    long UserId,
    string Nickname,
    string Card,
    Sex Sex,
    string Role,
    DateTime JoinTime,
    DateTime LastSentTime,
    string Level
);

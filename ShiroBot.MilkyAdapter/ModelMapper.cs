using System.Collections;
using System.Reflection;

namespace ShiroBot.MilkyAdapter;

internal static class ModelMapper
{
    public static TTarget Convert<TTarget>(object? source) =>
        (TTarget)Convert(source, typeof(TTarget))!;

    private static object? Convert(object? source, Type targetType)
    {
        if (source is null)
        {
            return IsNullable(targetType) ? null : Activator.CreateInstance(targetType);
        }

        var effectiveTargetType = Nullable.GetUnderlyingType(targetType) ?? targetType;
        var sourceType = source.GetType();

        if (effectiveTargetType.IsAssignableFrom(sourceType))
        {
            return source;
        }

        if (effectiveTargetType.IsEnum)
        {
            return sourceType.IsEnum
                ? Enum.Parse(effectiveTargetType, source.ToString()!, true)
                : Enum.Parse(effectiveTargetType, System.Convert.ToString(source)!, true);
        }

        if (effectiveTargetType == typeof(string))
        {
            return System.Convert.ToString(source);
        }

        if (TryConvertStringConstructor(source, effectiveTargetType, out var stringCtorResult))
        {
            return stringCtorResult;
        }

        if (TryConvertDateTimeOffset(source, effectiveTargetType, out var dateTimeOffsetResult))
        {
            return dateTimeOffsetResult;
        }

        if (effectiveTargetType == typeof(byte[]) && source is byte[] bytes)
        {
            return bytes.ToArray();
        }

        if (effectiveTargetType.IsPrimitive || effectiveTargetType == typeof(decimal) || effectiveTargetType == typeof(DateTimeOffset))
        {
            return System.Convert.ChangeType(source, effectiveTargetType);
        }

        if (TryConvertCollection(source, effectiveTargetType, out var collectionResult))
        {
            return collectionResult;
        }

        if (effectiveTargetType.IsInterface || effectiveTargetType.IsAbstract)
        {
            var concreteTargetType = ResolveConcreteTargetType(sourceType, effectiveTargetType);
            if (concreteTargetType is not null)
            {
                return Convert(source, concreteTargetType);
            }
        }

        return ConvertObject(source, effectiveTargetType);
    }

    private static bool TryConvertCollection(object source, Type targetType, out object? result)
    {
        result = null;
        if (source is not IEnumerable sourceEnumerable || source is string)
        {
            return false;
        }

        if (targetType.IsArray)
        {
            var elementType = targetType.GetElementType()!;
            var items = sourceEnumerable.Cast<object?>()
                .Select(item => Convert(item, elementType))
                .ToArray();

            var array = Array.CreateInstance(elementType, items.Length);
            for (var i = 0; i < items.Length; i++)
            {
                array.SetValue(items[i], i);
            }

            result = array;
            return true;
        }

        var genericArgs = targetType.GetGenericArguments();
        if (genericArgs.Length != 1)
        {
            return false;
        }

        var elementTypeGeneric = genericArgs[0];
        var listType = typeof(List<>).MakeGenericType(elementTypeGeneric);
        var list = (IList)Activator.CreateInstance(listType)!;
        foreach (var item in sourceEnumerable)
        {
            list.Add(Convert(item, elementTypeGeneric));
        }

        result = list;
        return true;
    }

    private static bool TryConvertDateTimeOffset(object source, Type targetType, out object? result)
    {
        result = null;

        switch (source)
        {
            case DateTimeOffset dto when targetType == typeof(long):
                result = dto.ToUnixTimeSeconds();
                return true;
            case DateTimeOffset dto when targetType == typeof(DateTime):
                result = dto.UtcDateTime;
                return true;
            case DateTime dt when targetType == typeof(DateTimeOffset):
                result = new DateTimeOffset(dt);
                return true;
            case DateTime dt when targetType == typeof(long):
                result = new DateTimeOffset(dt).ToUnixTimeSeconds();
                return true;
            case long unixSeconds when targetType == typeof(DateTimeOffset):
                result = DateTimeOffset.FromUnixTimeSeconds(unixSeconds);
                return true;
            case int unixSecondsInt when targetType == typeof(DateTimeOffset):
                result = DateTimeOffset.FromUnixTimeSeconds(unixSecondsInt);
                return true;
            default:
                return false;
        }
    }

    private static bool TryConvertStringConstructor(object source, Type targetType, out object? result)
    {
        result = null;

        if (source is not string stringValue)
        {
            return false;
        }

        var stringCtor = targetType.GetConstructor([typeof(string)]);
        if (stringCtor is null)
        {
            return false;
        }

        result = stringCtor.Invoke([stringValue]);
        return true;
    }

    private static object ConvertObject(object source, Type targetType)
    {
        var sourceProperties = source.GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase);

        var constructor = targetType
            .GetConstructors(BindingFlags.Instance | BindingFlags.Public)
            .OrderByDescending(ctor => ctor.GetParameters().Length)
            .FirstOrDefault()
            ?? throw new InvalidOperationException($"No public constructor found for {targetType.FullName}.");

        var parameters = constructor.GetParameters();
        var arguments = new object?[parameters.Length];
        for (var i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];
            if (!sourceProperties.TryGetValue(parameter.Name!, out var sourceProperty))
            {
                throw new InvalidOperationException(
                    $"Unable to map property '{parameter.Name}' from {source.GetType().FullName} to {targetType.FullName}.");
            }

            arguments[i] = Convert(sourceProperty.GetValue(source), parameter.ParameterType);
        }

        return constructor.Invoke(arguments);
    }

    private static Type? ResolveConcreteTargetType(Type sourceType, Type targetType) =>
        targetType.Assembly
            .GetTypes()
            .FirstOrDefault(type =>
                type.Name == sourceType.Name &&
                !type.IsAbstract &&
                !type.IsInterface &&
                targetType.IsAssignableFrom(type));

    private static bool IsNullable(Type type) =>
        !type.IsValueType || Nullable.GetUnderlyingType(type) is not null;
}

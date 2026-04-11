using System.Reflection;

namespace AlRunner.Runtime;

/// <summary>
/// Runtime index of <c>[NavEventSubscriber(ObjectType.CodeUnit, N, "EventName", …)]</c>
/// methods found in the loaded assembly. Populated lazily per assembly
/// and consulted by <see cref="AlCompat.FireEvent"/> when a publisher's
/// rewritten event-method body fires.
/// </summary>
public static class EventSubscriberRegistry
{
    private static readonly Dictionary<Assembly, Dictionary<(int PublisherId, string EventName), List<(Type OwnerType, MethodInfo Method)>>> _byAssembly = new();

    public static IReadOnlyList<(Type OwnerType, MethodInfo Method)> GetSubscribers(
        Assembly assembly, int publisherCodeunitId, string eventName)
    {
        if (!_byAssembly.TryGetValue(assembly, out var map))
        {
            map = Build(assembly);
            _byAssembly[assembly] = map;
        }

        return map.TryGetValue((publisherCodeunitId, eventName), out var list)
            ? list
            : Array.Empty<(Type, MethodInfo)>();
    }

    public static void Clear()
    {
        _byAssembly.Clear();
    }

    private static Dictionary<(int, string), List<(Type, MethodInfo)>> Build(Assembly assembly)
    {
        var result = new Dictionary<(int, string), List<(Type, MethodInfo)>>();

        Type[] types;
        try { types = assembly.GetTypes(); }
        catch (ReflectionTypeLoadException ex) { types = ex.Types.Where(t => t != null).ToArray()!; }

        foreach (var type in types)
        {
            if (type == null) continue;
            foreach (var method in type.GetMethods(
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
            {
                // Inspect the attribute via CustomAttributeData so we can read
                // the raw positional constructor arguments without caring
                // about the runtime wrapper types (ApplicationObjectId etc.).
                foreach (var data in method.GetCustomAttributesData())
                {
                    if (data.AttributeType.Name != "NavEventSubscriberAttribute") continue;

                    // Expected positional order:
                    //   (ObjectType targetObjectType, int targetObjectNo,
                    //    string targetMethodName, [int memberId,]
                    //    string fieldName, EventSubscriberCallOptions callOptions)
                    // The first int-typed arg is the ObjectType enum; we want
                    // the *second* — targetObjectNo. The first string is the
                    // target method name.
                    int? objId = null;
                    string? evName = null;
                    int intSeen = 0;
                    foreach (var arg in data.ConstructorArguments)
                    {
                        if (arg.Value is int i)
                        {
                            intSeen++;
                            if (intSeen == 2 && objId is null) objId = i;
                            continue;
                        }
                        if (arg.Value is string s && evName is null) evName = s;
                    }

                    if (objId == null || evName == null) continue;

                    var key = (objId.Value, evName);
                    if (!result.TryGetValue(key, out var list))
                        result[key] = list = new List<(Type, MethodInfo)>();
                    list.Add((type, method));
                }
            }
        }

        return result;
    }
}

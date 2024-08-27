using ProtoBuf;

namespace DriverBusPrototype.DriverCommands;

internal static class ProtoConverter
{
    public static byte[] ProtoSerialize<TInput>(TInput obj)
    {
        using (var buffer = new MemoryStream())
        {
            Serializer.Serialize(buffer, obj);
            return buffer.ToArray();
        }
    }

    public static TOutput ProtoDeserialize<TOutput>(byte[] bytes)
    {
        return Serializer.Deserialize<TOutput>(bytes.AsSpan());
    }

    public static (IntPtr, int) ObjectToProtoPtr<TInput>(TInput obj)
    {
        var protoBytes = ProtoSerialize(obj);
        var commandPtr = ArrayToPtrHelper.ByteArrayToPtr(protoBytes);

        return (commandPtr, protoBytes.Length);
    }
    
    public static TOutput ObjectFromProtoPtr<TOutput>(IntPtr protoPtr, int size)
    {
        var bytes = ArrayToPtrHelper.ByteArrayFromPtr(protoPtr, size);
        var obj = ProtoDeserialize<TOutput>(bytes);

        return obj;
    }
}
using System;

public static class EnumExtensions
{
    public static bool Has<T>(this Enum type, T value) where T : Enum
    {
        try {
            return (((int)(object)type & (int)(object)value) == (int)(object)value);
        }
        catch {
            return false;
        }
    }
}

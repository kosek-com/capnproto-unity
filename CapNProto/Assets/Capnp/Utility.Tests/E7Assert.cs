using NUnit.Framework;
using System;
using System.Globalization;
using System.Threading.Tasks;


// https://forum.unity.com/threads/can-i-replace-upgrade-unitys-nunit.488580/#post-3187140
// https://forum.unity.com/threads/can-i-replace-upgrade-unitys-nunit.488580/#post-6543523
public static class E7Assert
{
    public static async Task<T> ThrowsExceptionAsync<T>(Func<Task> action)
            where T : Exception
    {
        return await ThrowsExceptionAsync<T>(action, string.Empty, null).ConfigureAwait(false);
    }

    public static async Task<T> ThrowsExceptionAsync<T>(Func<Task> action, string message)
            where T : Exception
    {
        return await ThrowsExceptionAsync<T>(action, message, null).ConfigureAwait(false);
    }

    public static async Task<T> ThrowsExceptionAsync<T>(Func<Task> action, string message, params object[] parameters)
            where T : Exception
    {
        var finalMessage = string.Empty;

        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        if (message == null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        try
        {
            await action().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            if (!typeof(T).Equals(ex.GetType()))
            {
                Assert.Fail($"Wrong exception was thrown, expected {typeof(T).Name}, {ex.GetType().Name} given");
            }

            return (T)ex;
        }

        Assert.Fail($"No exception was thrown, expected {typeof(T).Name}");

        // This will not hit, but need it for compiler.
        return null;
    }
}

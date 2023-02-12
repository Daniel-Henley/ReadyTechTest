namespace ReadyTechTest.API.Shared;
/// <summary>
/// Used for getting DateTime.Now(), time is changeable for unit testing
/// 
/// Personally in a larger system I would create an interface rather than this but this is okay for the purpose
/// Unashamedly stolen from https://stackoverflow.com/questions/2425721/unit-testing-datetime-now
/// </summary>
public static class SystemTime
{
    /// <summary> Normally this is a pass-through to DateTime.Now, but it can be overridden with SetDateTime( .. ) for testing or debugging.
    /// </summary>
    public static Func<DateTime> Now = () => DateTime.Now;

    /// <summary> Set time to return when SystemTime.Now() is called.
    /// </summary>
    public static void SetDateTime(DateTime dateTimeNow)
    {
        Now = () => dateTimeNow;
    }

    /// <summary> Resets SystemTime.Now() to return DateTime.Now.
    /// </summary>
    public static void ResetDateTime()
    {
        Now = () => DateTime.Now;
    }
}
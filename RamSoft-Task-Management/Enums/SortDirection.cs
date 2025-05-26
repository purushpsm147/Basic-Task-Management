namespace RamSoft_Task_Management.Enums;

public enum SortDirection
{
    Asc,
    Desc
}

public static class SortDirectionExtensions
{
    public static string ToFriendlyString(this SortDirection direction)
    {
        return direction switch
        {
            SortDirection.Asc => "Ascending",
            SortDirection.Desc => "Descending",
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }
}

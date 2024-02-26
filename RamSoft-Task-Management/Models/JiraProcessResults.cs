namespace RamSoft_Task_Management.Models;

//Better way to do it by using FluentResults
public class JiraProcessResults
{
    private JiraProcessResults(bool isSuccess,JiraProcessError error)
    {
        if(isSuccess && error != JiraProcessError.None
            || !isSuccess && error == JiraProcessError.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }
        IsSuccess = isSuccess;
        Error = error;
    }
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public JiraProcessError Error { get; }
    public static JiraProcessResults Success() => new(true, JiraProcessError.None);
    public static JiraProcessResults Failure(JiraProcessError error) => new(false, error);
}

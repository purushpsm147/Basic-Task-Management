namespace RamSoft_Task_Management.Models;

public sealed record JiraProcessError(string Code, string? Description = null)
{
    public static readonly JiraProcessError None = new(string.Empty, string.Empty);
    public static readonly JiraProcessError NotFound = new("NotFound", "The requested resource was not found.");
    public static readonly JiraProcessError BadRequest = new("BadRequest", "The request was invalid.");
    public static readonly JiraProcessError InternalServerError = new("InternalServerError", "An error occurred while processing the request.");
    public static readonly JiraProcessError Unauthorized = new("Unauthorized", "The request was not authorized.");
    public static readonly JiraProcessError Forbidden = new("Forbidden", "The request was not allowed.");
    public static readonly JiraProcessError Conflict = new("Conflict", "The request could not be completed due to a conflict with the current state of the resource.");
    public static readonly JiraProcessError NotImplemented = new("NotImplemented", "The request was not implemented.");

}

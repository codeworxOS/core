namespace Codeworx.AspNetCore.Mvc.Filters
{
    public class UnhandledErrorResponse
    {
        public string? DetailMessage { get; init; }

        public required string TraceIdentifier { get; init; }
    }
}
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

public class ErrorModel : PageModel
{
    public string RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    public int StatusCode { get; set; }

    private readonly ILogger<ErrorModel> _logger;

    public ErrorModel(ILogger<ErrorModel> logger)
    {
        _logger = logger;
    }

    public void OnGet(int? id)
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        StatusCode = id.HasValue ? id.Value : StatusCodes.Status500InternalServerError;
    }
}

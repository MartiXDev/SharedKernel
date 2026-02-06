using System.Diagnostics;
using MartiX.GuardClauses;
using Mediator;
using Microsoft.Extensions.Logging;

namespace MartiX.SharedKernel;

/// <summary>
/// Adds logging for all requests in Mediator pipeline.
/// Configure by adding the service with a scoped lifetime
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
/// <param name="logger">The logger instance.</param>
public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
  : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
  private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger = logger;

  /// <summary>
  /// Logs the request and response around the next handler in the pipeline.
  /// </summary>
  /// <param name="request">The incoming request.</param>
  /// <param name="next">The next delegate in the pipeline.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The response from the next handler.</returns>
  public async ValueTask<TResponse> Handle(
      TRequest request,
      MessageHandlerDelegate<TRequest, TResponse> next,
      CancellationToken cancellationToken)
  {
    Guard.Against.Null(request);
    
    if (_logger.IsEnabled(LogLevel.Information))
    {
      _logger.LogInformation("Handling {RequestName} with {@Request}", typeof(TRequest).Name, request);
    }

    var sw = Stopwatch.StartNew();

    var response = await next(request, cancellationToken);

    sw.Stop();
    
    if (_logger.IsEnabled(LogLevel.Information))
    {
      _logger.LogInformation("Handled {RequestName} with {Response} in {ElapsedMilliseconds} ms", 
        typeof(TRequest).Name, response, sw.ElapsedMilliseconds);
    }
    
    return response;
  }
}

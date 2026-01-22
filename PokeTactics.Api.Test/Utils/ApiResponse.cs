using System.Net;

namespace PokeTactics.Api.Test.Utils;

public record ApiResponse<T>(T? Body, HttpStatusCode StatusCode);

using System;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.DynamoDBv2;
using System.Net;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace NotesBackendLambda
{

    public class Function
    {
        private readonly IRouteHandler _routeHandler;

        public Function(IRouteHandler routeHandler)
        {
            this._routeHandler = routeHandler;
        }

        public Function() : this(RouteHandlerBuilder.GetRouteHandler()) {

        }

        public APIGatewayProxyResponse FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return new APIGatewayProxyResponse() {
                Body = "Not implemented",
                StatusCode = (int) HttpStatusCode.InternalServerError
            };
        }
    }
}

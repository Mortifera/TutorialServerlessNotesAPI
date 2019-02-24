
using Amazon.Lambda.APIGatewayEvents;

namespace NotesBackendLambda.Routing
{
    public interface IRouteHandler {
        APIGatewayProxyResponse Handle(APIGatewayProxyRequest request);
    }
}

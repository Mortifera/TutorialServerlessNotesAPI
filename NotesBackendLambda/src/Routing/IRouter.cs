
using Amazon.Lambda.APIGatewayEvents;

namespace NotesBackendLambda
{
    public interface IRouteHandler {
        APIGatewayProxyResponse Handle(APIGatewayProxyRequest request);
    }
}

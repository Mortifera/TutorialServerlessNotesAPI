using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Amazon.Lambda.APIGatewayEvents;

namespace NotesBackendLambda
{
    public class RequestRouter : IRouteHandler {
        private readonly Dictionary<Route, IRouteHandler> _routesHandlers;

        public RequestRouter(Dictionary<Route, IRouteHandler> routesHandlers) {
            this._routesHandlers = routesHandlers;
        }

        public APIGatewayProxyResponse Handle(APIGatewayProxyRequest request)
        {
            if (request.PathParameters == null) {
                return new APIGatewayProxyResponse() {
                    Body = "Given no path parameters",
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }
            string pathToMatch = request.PathParameters.Aggregate(request.Path, 
                            (current, keyValuePair) => current.Replace(keyValuePair.Value, "{" + keyValuePair.Key + "}"));

            if (Enum.TryParse(request.HttpMethod, out HttpMethod httpMethod)) {
                return new APIGatewayProxyResponse() {
                    Body = "Invalid HTTP Method",
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }
            
            var chosenRoute = _routesHandlers.Keys.Where(route => route.Path.Equals(pathToMatch) && route.Method.Equals(httpMethod));
            
            if (!chosenRoute.Any()) {
                return new APIGatewayProxyResponse() {
                    Body = $"Unable to handle route for path '{pathToMatch}', and http method '{httpMethod}'",
                    StatusCode = (int) HttpStatusCode.NotFound
                };
            }
            
            return _routesHandlers[chosenRoute.First()].Handle(request);
        }
    }
}

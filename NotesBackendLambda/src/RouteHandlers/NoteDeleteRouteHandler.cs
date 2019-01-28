using System.Linq;
using System.Net;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.APIGatewayEvents;
using NotesBackendLambda.Model;

namespace NotesBackendLambda.RouteHandlers
{
    public class NoteDeleteRouteHandler : IRouteHandler
    {
        private readonly Table _dynamoDbTable;
        private readonly NoteDynamoDocumentMapper _noteDynamoDocumentMapper;

        public NoteDeleteRouteHandler(Table dynamoDbTable, NoteDynamoDocumentMapper noteDynamoDocumentMapper) {
            _dynamoDbTable = dynamoDbTable;
            _noteDynamoDocumentMapper = noteDynamoDocumentMapper;
        }

        public APIGatewayProxyResponse Handle(APIGatewayProxyRequest request)
        {
            if (!ExtractRequiredData(request, out string noteId)) {
                return new APIGatewayProxyResponse() {
                    Body = "Unable to extract noteId from query string!",
                    StatusCode = (int) HttpStatusCode.BadRequest
                };
            }

            var searchTask = _dynamoDbTable.Query(new QueryFilter("NoteId", QueryOperator.Equal, noteId)).GetRemainingAsync();
            searchTask.Wait();

            if (!searchTask.IsCompletedSuccessfully) {
                return new APIGatewayProxyResponse() {
                    Body = "Unable to read item from database",
                    StatusCode = (int) HttpStatusCode.InternalServerError
                };
            }

            if(!searchTask.Result.Any()) {
                return new APIGatewayProxyResponse() {
                    Body = $"No '{noteId}' in database",
                    StatusCode = (int) HttpStatusCode.NotFound
                };
            }

            var deleteTask = _dynamoDbTable.DeleteItemAsync(searchTask.Result.First());
            deleteTask.Wait();

            if (!deleteTask.IsCompletedSuccessfully) {
                return new APIGatewayProxyResponse() {
                    Body = "Unable to delete item from database",
                    StatusCode = (int) HttpStatusCode.InternalServerError
                };
            }

            return new APIGatewayProxyResponse() {
                StatusCode = (int) HttpStatusCode.OK
            };
        }

        private bool ExtractRequiredData(APIGatewayProxyRequest request, out string noteId) {
            if (request.PathParameters == null  || !request.PathParameters.ContainsKey("note_id")) {
                noteId = null;
                return false;
            }

            noteId = request.PathParameters["note_id"];
            return true;
        }
    }
}

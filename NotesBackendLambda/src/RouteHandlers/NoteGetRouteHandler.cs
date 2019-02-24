using System;
using System.Linq;
using System.Net;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using NotesBackendLambda.Model;
using NotesBackendLambda.Routing;

namespace NotesBackendLambda.RouteHandlers
{
    public class NoteGetRouteHandler : IRouteHandler
    {
        private readonly Table _dynamoDbTable;
        private readonly NoteDynamoDocumentMapper _noteDynamoDocumentMapper;

        public NoteGetRouteHandler(Table dynamoDbTable, NoteDynamoDocumentMapper noteDynamoDocumentMapper) {
            _dynamoDbTable = dynamoDbTable;
            _noteDynamoDocumentMapper = noteDynamoDocumentMapper;
        }

        public APIGatewayProxyResponse Handle(APIGatewayProxyRequest request)
        {
            if (!ExtractRequiredData(request, out string noteId)) {
                return new APIGatewayProxyResponse() {
                    Body = "Unable to extract noteId from params!",
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

            var note = _noteDynamoDocumentMapper.GetNoteFromDocument(searchTask.Result.First());

            try {
                return new APIGatewayProxyResponse() {
                    Body = JsonConvert.SerializeObject(new NoteGetRouteHandlerResponse(note)),
                    StatusCode = (int) HttpStatusCode.OK
                };
            } catch (Exception) {
                return new APIGatewayProxyResponse() {
                    Body = $"Unable to serialize note {noteId}",
                    StatusCode = (int) HttpStatusCode.InternalServerError
                };
            }
        }

        private bool ExtractRequiredData(APIGatewayProxyRequest request, out string noteId) {
            if (request.PathParameters == null || !request.PathParameters.ContainsKey("note_id")) {
                noteId = null;
                return false;
            }

            noteId = request.PathParameters["note_id"];
            return true;
        }

        private struct NoteGetRouteHandlerResponse {
            public string Title { get; set; }
            public string Content { get; set; }
            public long ModifiedTime { get; set; }

            public NoteGetRouteHandlerResponse(Note note) {
                Title = note.Title;
                Content = note.Content;
                ModifiedTime = note.ModifiedTime.ToUnixTimeSeconds();
            }
        }
    }
}

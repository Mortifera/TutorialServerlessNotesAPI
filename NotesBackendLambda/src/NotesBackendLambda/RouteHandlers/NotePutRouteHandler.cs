using System;
using System.Net;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NotesBackendLambda.Model;
using NotesBackendLambda.Routing;

namespace NotesBackendLambda.RouteHandlers
{
    internal class NotePutRouteHandler : IRouteHandler
    {
        private Table _dynamoDbTable;
        private NoteDynamoDocumentMapper _noteModelMapper;

        public NotePutRouteHandler(Table dynamoDbTable, NoteDynamoDocumentMapper noteModelMapper)
        {
            this._dynamoDbTable = dynamoDbTable;
            this._noteModelMapper = noteModelMapper;
        }

        public APIGatewayProxyResponse Handle(APIGatewayProxyRequest request)
        {
            if (!ExtractRequiredData(request, out string noteId, out Note note)) {
                return new APIGatewayProxyResponse() {
                    Body = "Unable to extract note. Check the format is correct!",
                    StatusCode = (int) HttpStatusCode.BadRequest
                };
            }

            var noteWithId = _noteModelMapper.CreateNoteWithId(note, noteId);
            var noteDocument = _noteModelMapper.CreateDocumentFromNote(noteWithId);

            var numberOfNoteIds = _dynamoDbTable.Query(new QueryFilter("NoteId", QueryOperator.Equal, noteId)).Count;

            if (numberOfNoteIds < 1) {
                return new APIGatewayProxyResponse() {
                    Body = $"Unable to PUT resourse with noteId '{noteId}' as it does not already exist. Please create note using POST",
                    StatusCode = (int) HttpStatusCode.Forbidden
                };
            }

            var putTask = _dynamoDbTable.PutItemAsync(noteDocument);
            putTask.Wait();

            if (!putTask.IsCompletedSuccessfully) {
                return new APIGatewayProxyResponse() {
                    Body = "Unable to write item to database",
                    StatusCode = (int) HttpStatusCode.InternalServerError
                };
            }

            return new APIGatewayProxyResponse() {
                StatusCode = (int) HttpStatusCode.OK
            };
        }

        private bool ExtractRequiredData(APIGatewayProxyRequest request, out string noteId, out Note note) {
            if (request.Body == null || request.PathParameters == null || !request.PathParameters.ContainsKey("note_id")) {
                note = null;
                noteId = null;
                return false;
            }

            noteId = request.PathParameters["note_id"];

            try {
                var jobj = JObject.Parse(request.Body);
                note = new Note() {
                    Title = jobj["Title"].ToObject<string>(),
                    Content = jobj["Content"].ToObject<string>(),
                    ModifiedTime = DateTimeOffset.FromUnixTimeSeconds(jobj["ModifiedTime"].ToObject<long>())
                };
                return true;
            } catch(Exception) {
                note = null;
                return false;
            }
        }
    }
}
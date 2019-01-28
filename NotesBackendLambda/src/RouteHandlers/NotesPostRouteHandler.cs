using System;
using System.Net;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NotesBackendLambda.Model;

namespace NotesBackendLambda.RouteHandlers
{
    public class NotesPostRouteHandler : IRouteHandler
    {
        private readonly Table _dynamoDbTable;
        private readonly NoteDynamoDocumentMapper _noteDynamoDocumentMapper;

        public NotesPostRouteHandler(Table dynamoDbTable, NoteDynamoDocumentMapper noteDynamoDocumentMapper)
        {
            _dynamoDbTable = dynamoDbTable;
            _noteDynamoDocumentMapper = noteDynamoDocumentMapper;
        }

        public APIGatewayProxyResponse Handle(APIGatewayProxyRequest request)
        {
            if (!ExtractRequiredData(request, out Note note)) {
                return new APIGatewayProxyResponse() {
                    Body = "Unable to extract note. Check the format is correct!",
                    StatusCode = (int) HttpStatusCode.BadRequest
                };
            }

            string noteId = Guid.NewGuid().ToString();

            var noteWithId = _noteDynamoDocumentMapper.CreateNoteWithId(note, noteId);
            var noteDocument = _noteDynamoDocumentMapper.GetDocumentFromNote(noteWithId);

            var putTask = _dynamoDbTable.PutItemAsync(noteDocument);
            putTask.Wait();

            if (!putTask.IsCompletedSuccessfully) {
                return new APIGatewayProxyResponse() {
                    Body = "Unable to write item to database",
                    StatusCode = (int) HttpStatusCode.InternalServerError
                };
            }

            return new APIGatewayProxyResponse() {
                Body = JsonConvert.SerializeObject(new NotesPostResponse(noteId)),
                StatusCode = (int) HttpStatusCode.OK
            };
        }

        private bool ExtractRequiredData(APIGatewayProxyRequest request, out Note note) {
            if (request.Body == null) {
                note = null;
                return false;
            }

            try {
                var jobj = JObject.Parse(request.Body);
                note = new Note() {
                    Title = jobj["Title"].ToObject<string>(),
                    Content = jobj["Content"].ToObject<string>(),
                    ModifiedTime = DateTimeOffset.FromUnixTimeSeconds(jobj["ModifiedTime"].ToObject<long>())
                };
                return true;
            } catch(Exception ex) {
                note = null;
                return false;
            }
        }

        private struct NotesPostResponse {
            public string NoteId { get; }

            public NotesPostResponse(string noteId) {
                this.NoteId = noteId;
            }
        }

    }
}

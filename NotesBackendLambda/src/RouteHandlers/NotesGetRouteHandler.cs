using System;
using System.Linq;
using System.Net;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NotesBackendLambda.Model;

namespace NotesBackendLambda.RouteHandlers
{
    public class NotesGetRouteHandler : IRouteHandler
    {
        private readonly Table _dynamoDbTable;
        private readonly NoteDynamoDocumentMapper _noteDynamoDocumentMapper;

        public NotesGetRouteHandler(Table dynamoDbTable, NoteDynamoDocumentMapper noteDynamoDocumentMapper) {
            _dynamoDbTable = dynamoDbTable;
            _noteDynamoDocumentMapper = noteDynamoDocumentMapper;
        }

        public APIGatewayProxyResponse Handle(APIGatewayProxyRequest request)
        {
            var searchTask = _dynamoDbTable.Scan(new ScanFilter()).GetRemainingAsync();
            searchTask.Wait();

            if (!searchTask.IsCompletedSuccessfully) {
                return new APIGatewayProxyResponse() {
                    Body = "Unable to read item from database",
                    StatusCode = (int) HttpStatusCode.InternalServerError
                };
            }

            var notes = searchTask.Result
                                    .Select(_noteDynamoDocumentMapper.GetNoteWithIdFromDocument)
                                    .Select(note => new NoteInfo () {
                                        NoteId = note.NoteId,
                                        Title = note.Title
                                    })
                                    .ToArray();

            try {
                return new APIGatewayProxyResponse() {
                    Body = JsonConvert.SerializeObject(new NotesGetRouteHandlerResponse() { Notes = notes }),
                    StatusCode = (int) HttpStatusCode.OK
                };
            } catch (Exception ex) {
                return new APIGatewayProxyResponse() {
                    Body = $"Unable to serialize notes",
                    StatusCode = (int) HttpStatusCode.InternalServerError
                };
            }
        }

        private struct NotesGetRouteHandlerResponse {
            public NoteInfo[] Notes { get; set; }
        }

        private struct NoteInfo {
            public string NoteId { get; set; }
            public string Title { get; set; }
        }
    }
}

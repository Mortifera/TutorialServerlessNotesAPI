using System.Collections.Generic;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using NotesBackendLambda.Model;
using NotesBackendLambda.RouteHandlers;
using NotesBackendLambda.Routing;

namespace NotesBackendLambda
{
    public class RouteHandlerBuilder {
        public IRouteHandler GetRouteHandler() {

            var notesGetRoute = new Route() {
                Path = "/v1/notes",
                Method = HttpMethod.GET
            };

            var notesPostRoute = new Route() {
                Path = "/v1/notes",
                Method = HttpMethod.POST
            };

            var noteGetRoute = new Route() {
                Path = "/v1/notes/{note_id}",
                Method = HttpMethod.GET
            };

            var notePutRoute = new Route() {
                Path = "/v1/notes/{note_id}",
                Method = HttpMethod.PUT
            };

            var noteDeleteRoute = new Route() {
                Path = "/v1/notes/{note_id}",
                Method = HttpMethod.DELETE
            };

            var dynamoDbClient = new AmazonDynamoDBClient();
            var notesTableName = System.Environment.GetEnvironmentVariable("NOTES_TABLE_NAME"); 
            var notesDynamoDbTable = Table.LoadTable(dynamoDbClient, notesTableName);

            var noteModelMapper = new NoteDynamoDocumentMapper();

            var routeHandlerDictionary = new Dictionary<Route, IRouteHandler>() {
                [notesPostRoute] = new NotesPostRouteHandler(notesDynamoDbTable, noteModelMapper),
                [notesGetRoute] = new NotesGetRouteHandler(notesDynamoDbTable, noteModelMapper),
                [noteGetRoute] = new NoteGetRouteHandler(notesDynamoDbTable, noteModelMapper),
                [notePutRoute] = new NotePutRouteHandler(notesDynamoDbTable, noteModelMapper),
                [noteDeleteRoute] = new NoteDeleteRouteHandler(notesDynamoDbTable, noteModelMapper)
            };

            return new RequestRouter(routeHandlerDictionary);
        } 
    }
}

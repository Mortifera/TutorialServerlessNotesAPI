using System.Collections.Generic;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using NotesBackendLambda.Model;
using NotesBackendLambda.RouteHandlers;

namespace NotesBackendLambda
{

    public class FunctionLazyConfig {

        private IAmazonDynamoDB _dynamoDbClient = null;

        public IAmazonDynamoDB DynamoDbClient => _dynamoDbClient == null ? (_dynamoDbClient = new AmazonDynamoDBClient()) : _dynamoDbClient;

        public string DynamoTableName => System.Environment.GetEnvironmentVariable("NOTES_TABLE_NAME"); 
        
        private Table _dynamoDbTable = null;
        public Table DynamoDbTable => _dynamoDbTable == null ? (_dynamoDbTable = Table.LoadTable(DynamoDbClient, DynamoTableName)) : _dynamoDbTable;
    }

    public class RouteHandlerBuilder {
        private static FunctionLazyConfig Config = new FunctionLazyConfig();

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

            var noteModelMapper = new NoteDynamoDocumentMapper();
            var routeHandlerDictionary = new Dictionary<Route, IRouteHandler>() {
                [notesPostRoute] = new NotesPostRouteHandler(Config.DynamoDbTable, noteModelMapper),
                [notesGetRoute] = new NotesGetRouteHandler(Config.DynamoDbTable, noteModelMapper),
                [noteGetRoute] = new NoteGetRouteHandler(Config.DynamoDbTable, noteModelMapper),
                [notePutRoute] = new NotePutRouteHandler(Config.DynamoDbTable, noteModelMapper),
                [noteDeleteRoute] = new NoteDeleteRouteHandler(Config.DynamoDbTable, noteModelMapper)
            };

            return new RequestRouter(routeHandlerDictionary);
        } 
    }
}

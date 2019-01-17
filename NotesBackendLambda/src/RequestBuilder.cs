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
                Path = "api/v1/notes",
                Method = HttpMethod.GET
            };

            var notesPostRoute = new Route() {
                Path = "api/v1/notes",
                Method = HttpMethod.POST
            };
            
            var noteGetRoute = new Route() {
                Path = "api/v1/notes/{note_id}",
                Method = HttpMethod.GET
            };

            var noteDeleteRoute = new Route() {
                Path = "v1/ntoes/{note_id}",
                Method = HttpMethod.DELETE
            };

            var noteModelMapper = new NoteDynamoDocumentMapper();
            var routeHandlerDictionary = new Dictionary<Route, IRouteHandler>() {
                [notesPostRoute] = new NotesPostRouteHandler(Config.DynamoDbTable, noteModelMapper),
                [notesGetRoute] = new NotesGetRouteHandler(),
                [noteGetRoute] = new NoteGetRouteHandler(),
                [noteDeleteRoute] = new NoteDeleteRouteHandler()
            };

            return new RequestRouter(routeHandlerDictionary);
        } 
    }
}

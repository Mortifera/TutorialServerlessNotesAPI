using System.Collections.Generic;

namespace NotesBackendLambda
{

    public static class RouteHandlerBuilder {
        public static IRouteHandler GetRouteHandler() {
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

            var notePutRoute = new Route() {
                Path = "api/v1/notes/{note_id}",
                Method = HttpMethod.PUT
            };

            var noteDeleteRoute = new Route() {
                Path = "v1/ntoes/{note_id}",
                Method = HttpMethod.DELETE
            };

            var routeHandlerDictionary = new Dictionary<Route, IRouteHandler>() {
                [notesPostRoute] = new NotesPostRouteHandler(),
                [notesGetRoute] = new NotesGetRouteHandler(),
                [noteGetRoute] = new NoteGetRouteHandler(),
                [notePutRoute] = new NotePutRouteHandler(),
                [noteDeleteRoute] = new NoteDeleteRouteHandler()
            };

            return new RequestRouter(routeHandlerDictionary);
        } 
    }
}

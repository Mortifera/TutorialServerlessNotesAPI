using System;

namespace NotesBackendLambda.Model
{

    public class Note {

        public string Title { get; set; }
        public string Content { get; set; }
        public DateTimeOffset ModifiedTime { get; set; } 
    }

    public class NoteWithId : Note {
        public string NoteId { get; set; }
    }
}

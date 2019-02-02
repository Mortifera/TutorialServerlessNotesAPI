using System;
using Amazon.DynamoDBv2.DocumentModel;

namespace NotesBackendLambda.Model
{
    public class NoteDynamoDocumentMapper {

        public Note GetNoteFromDocument(Document noteDocument) {
            return new Note() {
                Title = noteDocument["Title"].AsString(),
                Content = noteDocument["Content"].AsString(),
                ModifiedTime = DateTimeOffset.FromUnixTimeSeconds(noteDocument["ModifiedTime"].AsLong())
            };
        }

        public NoteWithId GetNoteWithIdFromDocument(Document noteDocument) {
            return new NoteWithId() {
                NoteId = noteDocument["NoteId"].AsString(),
                Title = noteDocument["Title"].AsString(),
                Content = noteDocument["Content"].AsString(),
                ModifiedTime = DateTimeOffset.FromUnixTimeSeconds(noteDocument["ModifiedTime"].AsLong())
            };
        }
/* 
        public Document GetDocumentFromNote(Note note) {
            throw new NotImplementedException();
        }*/

        public NoteWithId CreateNoteWithId(Note note, string noteId) {
            return new NoteWithId() {
                NoteId = noteId,
                Title = note.Title,
                Content = note.Content,
                ModifiedTime = note.ModifiedTime
            };
        }

        public Document CreateDocumentFromNote(NoteWithId note) {
            return new Document() {
                ["NoteId"] = note.NoteId,
                ["Title"] = note.Title,
                ["Content"] = note.Content,
                ["ModifiedTime"] = note.ModifiedTime.ToUnixTimeSeconds()
            };
        }
    }
}

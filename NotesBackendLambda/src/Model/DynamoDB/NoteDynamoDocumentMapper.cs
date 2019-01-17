using System;
using Amazon.DynamoDBv2.DocumentModel;

namespace NotesBackendLambda.Model
{
    public class NoteDynamoDocumentMapper {
        /* 
        public NoteWithId GetNoteFromDocument(Document noteDocument) {
            throw new NotImplementedException();
        }
        public Note GetNoteWithIdFromDocument(Document noteDocument) {
            throw new NotImplementedException();
        }

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

        public Document GetDocumentFromNote(NoteWithId note) {
            return new Document() {
                ["NoteId"] = note.NoteId,
                ["Title"] = note.Title,
                ["Content"] = note.Content,
                ["ModifiedTime"] = note.ModifiedTime.ToUnixTimeSeconds()
            };
        }
    }
}

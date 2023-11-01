using Microsoft.AspNetCore.Mvc;
using NotesAPI.Data;
using Microsoft.EntityFrameworkCore;
using NotesAPI.Models.Entities;

namespace NotesAPI.Controllers
{
    [ApiController] // Esto indica que es un controlador de API, no de MVC
    [Route("api/[controller]")] // Esto indica que la ruta de este controlador será api/notes
    public class NotesController : Controller
    {
        private readonly NotesDBContext notesDBContext;

        // Inyectar el contexto de la base de datos
        public NotesController(NotesDBContext notesDBContext)
        {
           this.notesDBContext = notesDBContext;
        }

        [HttpGet] // Esto indica que este método responderá a peticiones GET
        public async Task<IActionResult> GetNotes()
        {
            // Obtener todas las notas de la base de datos, usando el contexto NotesDBContext

            // Como es una operación asíncrona, se debe usar await, el Ok indica que la operación fue exitosa (Status 200)
            // El método ToListAsync() convierte el resultado de la consulta en una lista de objetos
            return Ok(await notesDBContext.Notes.ToListAsync());
        }

        [HttpGet] // Esto indica que este método responderá a peticiones GET con un parámetro en la ruta
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetNoteById(Guid id)
        {
            // Obtener la nota con el id especificado, usando el contexto NotesDBContext

            // Como es una operación asíncrona, se debe usar await, el Ok indica que la operación fue exitosa (Status 200)
            // El método FirstOrDefaultAsync() obtiene el primer elemento de la consulta, o null si no hay elementos

            // Primer método
            // return Ok(await notesDBContext.Notes.FirstOrDefaultAsync(note => note.Id == id));

            // o, se puede usar el metodo FindAsync() del contexto, que busca por llave primaria
            var note = await notesDBContext.Notes.FindAsync(id);

            if (note == null)
            {
                // Si no se encontró la nota, se responde con un status 404 (NotFound)
                return NotFound();
            }

            return Ok(note);
        }

        [HttpPost] // Esto indica que este método responderá a peticiones POST
        [Route("create")]
        [ActionName("CreateNote")]
        public async Task<IActionResult> CreateNote(Note note)
        {
            note.Id = Guid.NewGuid(); // Generar un nuevo id para la nota
            note.CreatedAt = DateTime.Now; // Establecer la fecha de creación de la nota

            // Agregar la nota al contexto
            // Como es una operación asíncrona, se debe usar await, el Ok indica que la operación fue exitosa (Status 200)
            await notesDBContext.Notes.AddAsync(note);
            await notesDBContext.SaveChangesAsync(); // Guardar los cambios en la base de datos

            // Responder con un status 201 (Created) y la nota creada
            return CreatedAtAction(nameof (GetNoteById), new { id = note.Id}, note);
        }

        [HttpPut] // Esto indica que este método responderá a peticiones PUT
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdateNote([FromRoute] Guid id, [FromBody] Note note)
        {
            // Obtener la nota con el id especificado, usando el contexto NotesDBContext
            var noteToUpdate = await notesDBContext.Notes.FindAsync(id);

            if (noteToUpdate == null)
            {
                // Si no se encontró la nota, se responde con un status 404 (NotFound)
                return NotFound();
            }

            // Actualizar los campos de la nota
            noteToUpdate.Title = note.Title;
            noteToUpdate.Content = note.Content;
            noteToUpdate.isVisible = note.isVisible;

            // Guardar los cambios en la base de datos
            await notesDBContext.SaveChangesAsync();

            // Responder con un status 200 (OK) y la nota actualizada
            return Ok(noteToUpdate);
        }

        [HttpDelete] // Esto indica que este método responderá a peticiones DELETE
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteNoteById(Guid id)
        {
            // Obtener la nota con el id especificado, usando el contexto NotesDBContext
            var noteToDelete = await notesDBContext.Notes.FindAsync(id);

            if (noteToDelete == null)
            {
                // Si no se encontró la nota, se responde con un status 404 (NotFound)
                return NotFound();
            }

            // Eliminar la nota del contexto
            notesDBContext.Notes.Remove(noteToDelete);

            // Guardar los cambios en la base de datos
            await notesDBContext.SaveChangesAsync();

            // Responder con un status 200 (OK) y la nota eliminada
            return Ok(noteToDelete);
        }
    }
}

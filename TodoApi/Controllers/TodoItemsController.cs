using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoApi.Models;
using TodoApi.Models;

namespace TodoApi.Controllers
{
  [Route("api/TodoItems")]
  [ApiController]
  public class TodoItemsController : ControllerBase
  {
    private readonly TodoContext _context;

    public TodoItemsController(TodoContext context)
    {
      _context = context;
    }

    // GET: api/TodoItems
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoItemDto>>> GetTodoItems()
    {
      return await _context.TodoItems.Select(x => ItemToDto(x)).ToListAsync();
    }

    // GET: api/TodoItems/5
    [HttpGet("{id}")]
    public async Task<ActionResult<TodoItemDto>> GetTodoItem(long id)
    {
      var todoItemDTO = await _context.TodoItems
        .Where(x => x.Id == id)
        .Select(x => ItemToDto(x))
        .SingleAsync();

      if (todoItemDTO == null)
      {
        return NotFound();
      }

      return todoItemDTO;
    }

    // PUT: api/TodoItems/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for
    // more details see https://aka.ms/RazorPagesCRUD.
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTodoItem(long id, TodoItemDto todoItemDTO)
    {
      if (id != todoItemDTO.Id)
      {
        return BadRequest();
      }

      var todoItem = await _context.TodoItems.FindAsync(id);
      if (todoItem == null)
      {
        return NotFound();
      }

      todoItem.Name = todoItemDTO.Name;
      todoItem.IsComplete = todoItemDTO.IsComplete;

      try
      {
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException) when (!TodoItemExists(id))
      {
          return NotFound();
      }

      return NoContent();
    }

    // POST: api/TodoItems
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for
    // more details see https://aka.ms/RazorPagesCRUD.
    [HttpPost]
    public async Task<ActionResult<TodoItem>> CreateTodoItem(TodoItemDto todoItemDTO)
    {
      var todoItem = new TodoItem
      {
        Name = todoItemDTO.Name,
        IsComplete = todoItemDTO.IsComplete
      };

      _context.TodoItems.Add(todoItem);
      await _context.SaveChangesAsync();

      // Use "nameof" to avoid hard coding values. The less hard coded values the better.
      return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, ItemToDto(todoItem));
    }

    // DELETE: api/TodoItems/5
    [HttpDelete("{id}")]
    public async Task<ActionResult<TodoItem>> DeleteTodoItem(long id)
    {
      var todoItem = await _context.TodoItems.FindAsync(id);

      if (todoItem == null)
      {
        return NotFound();
      }

      _context.TodoItems.Remove(todoItem);
      await _context.SaveChangesAsync();

      return NoContent();
    }

    #region Private Functions
    private bool TodoItemExists(long id) => _context.TodoItems.Any(e => e.Id == id);
    private static TodoItemDto ItemToDto(TodoItem todoItem) =>
      new TodoItemDto
      {
        Id = todoItem.Id,
        Name = todoItem.Name,
        IsComplete = todoItem.IsComplete
      };
    #endregion Private Functions
  }

}

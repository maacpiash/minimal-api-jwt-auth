/*
 * MIT License
 *
 * Copyright (c) 2022 Mohammad Abdul Ahad Chowdhury
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
using Microsoft.EntityFrameworkCore;
using MiniValidation;
using static Microsoft.AspNetCore.Http.Results;

public static class Todos
{
	internal static async Task<IResult> ReadAllAsync(AppDbContext db)
	{
		var todos = await db.Todos.ToListAsync();
		return Ok(todos);
	}

	internal static async Task<IResult> ReadOneAsync(AppDbContext db, Guid id)
	{
		var todo = await db.Todos.FindAsync(id);
		if (todo is null) return NotFound();
		return Ok(todo);
	}

	internal static async Task<IResult> CreateAsync(TodoDTO todo, AppDbContext db)
	{
		if (todo is null) return BadRequest("Must include a Todo.");
		if (!MiniValidator.TryValidate(todo, out var errors)) return BadRequest(errors);

		var (id, title, isDone, assignedToId) = todo;

		if (assignedToId == Guid.Empty) return BadRequest("Invalid `assignedToId`.");
		if (string.IsNullOrWhiteSpace(title)) return BadRequest("Invalid `title`.");
		if (id == Guid.Empty) id = Guid.NewGuid();

		var user = await db.Users.FindAsync(assignedToId);
		if (user is null) return NotFound($"User with Id {assignedToId} not found.");

		var todoFromDb = await db.Todos.FindAsync(id);
		if (todoFromDb is not null) return Conflict("A todo with this ID already exists.");

		var td = new Todo(id, title, isDone, assignedToId, user);

		await db.Todos.AddAsync(td);
		await db.SaveChangesAsync();

		return Results.Created($"/todos/{id}", td);
	}

	internal static async Task<IResult> UpdateAsync(Todo todo, AppDbContext db)
	{
		if (todo is null) return BadRequest("Must include a Todo.");
		if (!MiniValidator.TryValidate(todo, out var errors)) return BadRequest(errors);

		var todoFromDb = await db.Todos.FindAsync(todo.Id);
		if (todoFromDb is null) return NotFound();

		var modifiedTodo = db.Todos.Update(todo);
		await db.SaveChangesAsync();
		return Ok(modifiedTodo);
	}

	internal static async Task<IResult> DeleteAsync(Guid id, AppDbContext db)
	{
		var todo = await db.Todos.FindAsync(id);
		if (todo is null) return NotFound();

		db.Todos.Remove(todo);
		await db.SaveChangesAsync();
		return NoContent();
	}
}

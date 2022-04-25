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
using Microsoft.AspNetCore.Identity;
using MiniValidation;
using static Microsoft.AspNetCore.Http.Results;

public static class Users
{
	internal static async Task<IResult> SignUpAsync(AppDbContext db, UserManager<User> users, UserCreateDTO user)
	{
		if (user is null) return BadRequest();
		if (!MiniValidator.TryValidate(user, out var errors)) return BadRequest(errors);

		if (users.Users.Any(u => u.Email == user.Email))
			return Conflict("Invalid `email`: A user with this email address already exists.");

		if (string.IsNullOrWhiteSpace(user.Username))
		{
			user.Username = string.Join('_', user.FullName.Split(' ')).ToLower();
			if (users.Users.Any(u => u.UserName == user.Username))
				user.Username = user.Username + '_' + Guid.NewGuid().ToString("N").Substring(0, 4);
		}
		else if (users.Users.Any(u => u.UserName == user.Username))
			return Conflict("Invalid `username`: A user with this username already exists.");

		var newser = new User(user);
		var result = await users.CreateAsync(newser, user.Password);
		if (!result.Succeeded) return BadRequest(result.Errors);

		return Created($"/users/{newser.Id}", newser);
	}
}

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
using System.ComponentModel.DataAnnotations;

public class UserDTO
{
	public string Id { get; set; }

	[Required]
	public string UserName { get; set; }

	[Required]
	public string FullName { get; set; }

	[Required]
	public string Email { get; set; }

	public int Age { get; set; }

	[Required]
	public string Role { get; set; }

	public string Address { get; set; }

	public User ToEntity()
	{
		return new User
		{
			Id = Guid.TryParse(Id, out Guid UserId) ? UserId : Guid.NewGuid(),
			UserName = UserName,
			FullName = FullName,
			Email = Email,
			Age = Age,
			Role = Role,
			Address = Address
		};
	}
}

public class UserCreateDTO
{
	[Required]
	[EmailAddress]
	public string Email { get; set; }

	[Required]
	[DataType(DataType.Password), MinLength(8)]
	public string Password { get; set; }

	[Required]
	[Compare("Password")]
	public string ConfirmPassword { get; set; }

	[Required]
	public string FullName { get; set; }

	public string Username { get; set; }
	public int Age { get; set; }

	[Required]
	public string Role { get; set; }

	public string Address { get; set; }
}

public class UserLoginDTO
{
	[Required]
	public string Login { get; set; }

	[Required]
	public string Password { get; set; }
}

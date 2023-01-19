/*
 * MIT License
 *
 * Copyright (c) 2022—2023 Mohammad Abdul Ahad Chowdhury
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
using System.Text.Json;
using System.Text.Json.Serialization;

public class UserConverter : JsonConverter<User>
{
	public override User Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var user = new User();
		var json = JsonDocument.Parse(reader.GetString()).RootElement.EnumerateObject();
		foreach (var property in json)
		{
			switch (property.Name)
			{
				case "id":
					user.Id = property.Value.GetGuid();
					break;
				case "fullName":
					user.FullName = property.Value.GetString();
					break;
				case "username":
					user.UserName = property.Value.GetString();
					break;
				case "email":
					user.Email = property.Value.GetString();
					break;
				case "age":
					user.Age = property.Value.GetInt32();
					break;
				case "role":
					user.Role = property.Value.GetString();
					break;
				case "address":
					user.Address = property.Value.GetString();
					break;
				default:
					break;
			}
		}
		return user;
	}

	public override void Write(Utf8JsonWriter writer, User user, JsonSerializerOptions options)
	{
		writer.WriteStartObject();
		writer.WriteString("id", user.Id.ToString());
		writer.WriteString("fullName", user.FullName);
		writer.WriteString("username", user.UserName);
		writer.WriteString("email", user.Email);
		writer.WriteNumber("age", user.Age);
		writer.WriteString("role", user.Role);
		writer.WriteString("address", user.Address);
		writer.WriteEndObject();
	}
}

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
using System.Security.Claims;

public static class UserClaims
{
	public static bool TryValidate(ClaimsPrincipal claims, out User user, out string ErrorMessage)
	{
		if (!Guid.TryParse(claims.FindFirstValue("jti"), out var _))
			ErrorMessage = "Invalid `tokenId`: Not a valid GUID.";

		Guid UserId = Guid.Empty;
		string Username = "";
		user = null;
		ErrorMessage = "";

		var userIds = claims.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).ToArray();
		if (userIds.Length == 2) // only userid and username
		{
			if (Guid.TryParse(userIds[0].Value, out Guid userId0))
			{
				UserId = userId0;
				Username = userIds[1].Value;
			}
			else if (Guid.TryParse(userIds[1].Value, out Guid userId1))
			{
				UserId = userId1;
				Username = userIds[0].Value;
			}
			else
			{
				ErrorMessage = "Invalid userid";
				return false;
			}
		}
		else if (userIds.Length == 1) // only userid or only username
		{
			if (Guid.TryParse(userIds[0].Value, out Guid userId)) UserId = userId;
			else Username = userIds[0].Value;
		}
		else
		{
			ErrorMessage = "Invalid claims of type 'nameidentifier'";
			return false;
		}

		string FullName = claims.FindFirstValue(ClaimTypes.Name);
		string Email = claims.FindFirstValue(ClaimTypes.Email);
		string Role = claims.FindFirstValue(ClaimTypes.Role);

		user = new User
		{
			Id = UserId,
			UserName = Username,
			FullName = FullName,
			Email = Email,
			Role = Role
		};

		return true;
	}
}

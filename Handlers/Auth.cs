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
using static Microsoft.AspNetCore.Http.Results;

public static class Auth
{
	internal static async Task<IResult> RefreshTokenAsync
	(
		HttpRequest request,
		HttpResponse response,
		TokenRepository repository,
		TokenValidator validator,
		TokenGenerator tokens,
		AppDbContext db
	)
	{
		var refreshToken = request.Cookies["refresh_token"];

		if (string.IsNullOrWhiteSpace(refreshToken))
			return BadRequest("Please include a refresh token in the request.");

		var tokenIsValid = validator.TryValidate(refreshToken, out var tokenId);
		if (!tokenIsValid) return BadRequest("Invalid refresh token.");

		var token = await repository.Tokens.Where(token => token.Id == tokenId).FirstOrDefaultAsync();
		if (token is null) return BadRequest("Refresh token not found.");

		var user = await db.Users.Where(u => u.Id == token.UserId).FirstOrDefaultAsync();
		if (user is null) return BadRequest("User not found.");

		var accessToken = tokens.GenerateAccessToken(user);
		var (newRefreshTokenId, newRefreshToken) = tokens.GenerateRefreshToken();

		repository.Tokens.Remove(token);
		await repository.Tokens.AddAsync(new Token { Id = newRefreshTokenId, UserId = user.Id });
		await repository.SaveChangesAsync();

		response.Cookies.Append("refresh_token", newRefreshToken, new CookieOptions
		{
			Expires = DateTime.Now.AddDays(1),
			HttpOnly = true,
			IsEssential = true,
			MaxAge = new TimeSpan(1, 0, 0, 0),
			Secure = true,
			SameSite = SameSiteMode.Strict
		});

		return Ok(accessToken);
	}

	internal static async Task<IResult> SignOutAsync
	(
		HttpRequest request,
		HttpResponse response,
		TokenRepository repository,
		TokenValidator validator
	)
	{
		var refreshToken = request.Cookies["refresh_token"];

		if (string.IsNullOrWhiteSpace(refreshToken))
			return BadRequest("Please include a refresh token in the request.");

		var tokenIsValid = validator.TryValidate(refreshToken, out var tokenId);
		if (!tokenIsValid) return BadRequest("Invalid refresh token.");

		var token = await repository.Tokens.Where(token => token.Id == tokenId).FirstOrDefaultAsync();
		if (token is null) return BadRequest("Refresh token not found.");

		repository.Tokens.Remove(token);
		await repository.SaveChangesAsync();

		response.Cookies.Delete("refresh_token");
		return NoContent();
	}
}

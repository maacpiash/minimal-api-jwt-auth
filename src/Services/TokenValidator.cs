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
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

public class TokenValidator
{
	private readonly byte[] _refreshTokenSecret;
	private readonly string _issuer;
	private readonly string _audience;

	public TokenValidator(IConfiguration config)
	{
		_refreshTokenSecret = Encoding.ASCII.GetBytes(config.GetValue<string>("Jwt:RefreshTokenSecret"));
		_issuer = config.GetValue<string>("Jwt:Issuer");
		_audience = config.GetValue<string>("Jwt:Audience");
	}

	public bool TryValidate(string refreshToken, out Guid tokenId)
	{
		var tokenHandler = new JwtSecurityTokenHandler();
		var tokenValidationParams = new TokenValidationParameters
		{
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(_refreshTokenSecret),
			ValidateIssuer = true,
			ValidateAudience = true,
			ClockSkew = TimeSpan.Zero,
			ValidAudience = _audience,
			ValidIssuer = _issuer,
		};

		try
		{
			tokenHandler.ValidateToken(refreshToken, tokenValidationParams, out SecurityToken token);
			var jwt = (JwtSecurityToken)token;
			var valid = Guid.TryParse(jwt.Id, out var id);
			tokenId = id;
			return valid;
		}
		catch (Exception)
		{
			tokenId = default;
			return false;
		}
	}
}

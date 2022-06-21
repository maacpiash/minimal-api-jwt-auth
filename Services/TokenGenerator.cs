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
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

public class TokenGenerator
{
	private readonly byte[] _accessTokenSecret;
	private readonly byte[] _refreshTokenSecret;
	private readonly string _issuer;
	private readonly string _audience;

	public TokenGenerator(IConfiguration config)
	{
		_accessTokenSecret = Encoding.ASCII.GetBytes(config.GetValue<string>("Jwt:AccessTokenSecret"));
		_refreshTokenSecret = Encoding.ASCII.GetBytes(config.GetValue<string>("Jwt:RefreshTokenSecret"));
		_issuer = config.GetValue<string>("Jwt:Issuer");
		_audience = config.GetValue<string>("Jwt:Audience");
	}

	public string GenerateAccessToken(User user)
	{
		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(
				new[]
				{
					new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
					new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
					new Claim(ClaimTypes.NameIdentifier, user.UserName),
					new Claim(ClaimTypes.Name, user.FullName),
					new Claim(ClaimTypes.Email, user.Email),
					new Claim(ClaimTypes.Role, user.Role),
				}
			),
			Expires = DateTime.UtcNow.AddMinutes(15),
			SigningCredentials = new SigningCredentials(
				new SymmetricSecurityKey(_accessTokenSecret),
				SecurityAlgorithms.HmacSha256Signature
			),
			Issuer = _issuer,
			Audience = _audience,
		};

		var tokenHandler = new JwtSecurityTokenHandler();
		var token = tokenHandler.CreateToken(tokenDescriptor);
		return tokenHandler.WriteToken(token);
	}

	public (Guid, string) GenerateRefreshToken()
	{
		var tokenId = Guid.NewGuid();
		var tokenHandler = new JwtSecurityTokenHandler();
		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(new[] { new Claim(JwtRegisteredClaimNames.Jti, tokenId.ToString()), }),
			Expires = DateTime.UtcNow.AddDays(1),
			SigningCredentials = new SigningCredentials(
				new SymmetricSecurityKey(_refreshTokenSecret),
				SecurityAlgorithms.HmacSha256Signature
			),
			Issuer = _issuer,
			Audience = _audience,
		};

		var token = tokenHandler.CreateToken(tokenDescriptor);
		return (tokenId, tokenHandler.WriteToken(token));
	}
}

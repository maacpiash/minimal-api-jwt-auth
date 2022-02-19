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
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using static Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId;

var builder = WebApplication.CreateBuilder(args);

var accessTokenSecret = builder.Configuration["Jwt:AccessTokenSecret"];
var isProduction = builder.Environment.IsProduction();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(optionsBuilder =>
	optionsBuilder.UseSqlite("Data Source=app.db")
		.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
		.EnableDetailedErrors()
		.ConfigureWarnings(b => b.Log(ConnectionOpened, CommandExecuted, ConnectionClosed)));

builder.Services.AddDbContext<TokenRepository>(optionsBuilder => optionsBuilder.UseInMemoryDatabase("Tokens"));

builder.Services.AddSingleton<TokenGenerator>();
builder.Services.AddSingleton<TokenValidator>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<User>(options =>
{
	options.User.RequireUniqueEmail = true;
	options.Password.RequireDigit = isProduction;
	options.Password.RequireLowercase = isProduction;
	options.Password.RequireNonAlphanumeric = isProduction;
	options.Password.RequireUppercase = isProduction;
	if (isProduction)
	{
		options.Password.RequiredLength = 8;
		options.Password.RequiredUniqueChars = 3;
	}
})
.AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(accessTokenSecret)),
			ValidIssuer = builder.Configuration["Jwt:Issuer"],
			ValidAudience = builder.Configuration["Jwt:Audience"],
			ValidateIssuerSigningKey = true,
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ClockSkew = TimeSpan.Zero
		};
		options.SaveToken = true;
	});

builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("admin", policy => policy.RequireAuthenticatedUser().RequireClaim("role", "admin"));
	options.AddPolicy("user", policy => policy.RequireAuthenticatedUser().RequireClaim("role", "user"));
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", (TokenGenerator tokenGen, UserDTO user) => tokenGen.GenerateAccessToken(user.ToEntity()));

app.MapGet("/validate", (string jwt, TokenValidator refreshToken) =>
{
	var tokenIsValid = refreshToken.TryValidate(jwt, out var id);
	return tokenIsValid ? Results.Ok(id) : Results.BadRequest();
});

app.Run();

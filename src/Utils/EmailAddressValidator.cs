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
// source: https://docs.microsoft.com/en-au/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format

using System.Globalization;
using System.Text.RegularExpressions;

public static class EmailAddressValidator
{
	public static bool TryValidate(string email, out string errorMessage)
	{
		if (string.IsNullOrWhiteSpace(email))
		{
			errorMessage = "Null, empty, or whitespace.";
			return false;
		}

		var timeSpan = TimeSpan.FromMilliseconds(250);

		try
		{
			// Normalize the domain
			email = Regex.Replace(email, @"(@)(.+)$", DomainMapper, RegexOptions.None, timeSpan);

			// Examines the domain part of the email and normalizes it.
			string DomainMapper(Match match)
			{
				// Use IdnMapping class to convert Unicode domain names.
				var idn = new IdnMapping();

				// Pull out and process domain name (throws ArgumentException on invalid)
				string domainName = idn.GetAscii(match.Groups[2].Value);

				return match.Groups[1].Value + domainName;
			}
		}
		catch (RegexMatchTimeoutException e)
		{
			errorMessage = e.Message;
			return false;
		}
		catch (ArgumentException e)
		{
			errorMessage = e.Message;
			return false;
		}

		try
		{
			var match = Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase, timeSpan);
			errorMessage = match ? string.Empty : "The pattern did not match.";
			return match;
		}
		catch (RegexMatchTimeoutException e)
		{
			errorMessage = e.Message;
			return false;
		}
	}
}

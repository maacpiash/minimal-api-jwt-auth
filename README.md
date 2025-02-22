![Status: in progress](https://img.shields.io/badge/status-maintained-green?logo=git&style=flat-square)
![.NET Version](https://img.shields.io/badge/targets-8.0,%209.0-%23512BD4?logo=.net&style=flat-square)

Demonstration of ASP.NET (versions 8 and 9) minimal web API with JWT authentication and authorization.

## Workflow

- User signs up with username, email address, and password. These are saved in the database.
- When user signs in, the following is returned:
  - a short-lived access token (JWT) is sent in the response body. This JWT contains the user's details and role.
  - a longer-lived refresh token (also JWT) is sent as a strict same-site, secure, HTTP-only cookie. This is also saved in an in-memory storage as `jti:userID` key-value pair.
- The user can access their `todo`s with the access token in the header in `Authorization: Bearer {access_token}` format
- When the access token expires, the user can obtain a new access token by calling the `refresh` endpoint, with the refresh token in the cookie.
  - This call also replaces the refresh token with a new one. Meaning, the refresh token in the cookie is removed from the in-memory storage and a new refresh token is saved, which is also sent in as a response cookie.

## Roadmap

- [x] JWT generation with symmetric signing
- [x] Implementation of refresh token generation with repository (in-memory storage and retrieval)
- [ ] Asymmetric signing with JWKS

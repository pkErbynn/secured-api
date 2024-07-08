Swagger usage
- Swagger is configured to serve as the Client to be adding the generated token (from `/login`) to every request made for a resource.
- 

`[Authorize]`
- Needs authentication

`[Authorize(Roles = "User")]`
- Should match the `claim` that was used in the token generation
- Valid token returns status of `403 Forbidden` on unmatched role
- Invalid token returns status of `401 Unauthorized` on matched/unmatched role

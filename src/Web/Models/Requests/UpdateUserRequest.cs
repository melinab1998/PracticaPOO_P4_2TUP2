namespace Web.Models.Requests;

public record UpdateUserRequest(int Id, string UserName, string FirstName, string LastName, string Email, string Phone);

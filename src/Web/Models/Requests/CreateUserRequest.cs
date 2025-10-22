
namespace Web.Models.Requests;

public record CreateUserRequest(string UserName, string FirstName, string LastName, string Email, string Phone);

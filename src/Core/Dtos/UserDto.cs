using System;
using Core.Entities;

namespace Core.Dtos;

public record UserDto(Guid ExternalId, string UserName, string FirstName, string LastName, string Email, string Phone)
{
    public static UserDto Create(User user)
    {
        return new UserDto(
            user.ExternalId ?? Guid.Empty,
            user.FirstName,
            user.LastName,
            user.Email,
            user.Phone,
            user.UserName
           );

    }

    public static List<UserDto> Create(IEnumerable<User> users)
    {
        return users.Select(user => Create(user)).ToList();
    }
}



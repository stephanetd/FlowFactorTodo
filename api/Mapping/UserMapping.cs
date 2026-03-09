using System;
using api.DTOs;
using api.Entities;

namespace api.Mapping;

public static class UserMapping
{
    public static UserDTO ToDto(this User user)
    {
        return new UserDTO
        (
            user.Id,
            user.FirstName,
            user.LastName
        );
    }
}

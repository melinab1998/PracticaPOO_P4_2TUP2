using Microsoft.AspNetCore.Mvc;
using Web.Models.Requests;
using Core.Dtos;
using Core.Services;
using Microsoft.AspNetCore.Authorization;


namespace Web.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]




public class UserController : ControllerBase

{

    private readonly UserServices _userService;
    public UserController(UserServices userService)
    {
        _userService = userService;
    }


    [HttpGet()]
    public ActionResult<List<UserDto>> GetAllUsersInfo()
    {
        var list = _userService.GetAllUsersInfo();
        return Ok(list);
    }

    [HttpGet("{id}")]
    public ActionResult<UserDto> GetUserInfo([FromRoute] int id)
    {
        var user = _userService.GetUserInfo(id);

        return Ok(user);
    }

    [HttpPost()]
    public ActionResult<UserDto> CreateUser([FromBody] CreateUserRequest createUserRequest)
    {
        var user = _userService.CreateUser(createUserRequest.UserName,
            createUserRequest.FirstName,
            createUserRequest.LastName,
            createUserRequest.Email,
            createUserRequest.Phone);
        return Ok(user);
    }


    [HttpPut("{id}")]
    public ActionResult<UserDto> UpdateUser([FromRoute] int id, [FromBody] UpdateUserRequest updateUserRequest)
    {
        var user = _userService.UpdateUser(id,
            updateUserRequest.Id,
   
            updateUserRequest.UserName,
            updateUserRequest.FirstName,
            updateUserRequest.LastName,
            updateUserRequest.Email,
            updateUserRequest.Phone);
        return Ok(user);
    }


    [HttpDelete("{id}")]
    public ActionResult<UserDto> DeleteUser([FromRoute] int id)
    {
        _userService.DeleteUser(id);
        return NoContent();
    }
};



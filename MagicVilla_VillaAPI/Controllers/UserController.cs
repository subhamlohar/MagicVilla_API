using System.Net;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using MagicVilla_Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
	[Route("api/v{version:apiVersion}/UsersAuth")]
	[ApiController]
	[ApiVersionNeutral]
	public class UserController : Controller
	{
		private readonly IUserRepository _userRepo;
		protected APIResponse _response;
		public UserController(IUserRepository userRepo)
		{
			_userRepo = userRepo;
			this._response = new();
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
		{
			var loginResponse = await _userRepo.Login(model);
			if(loginResponse.User==null || string.IsNullOrEmpty(loginResponse.Token))
			{
				_response.StatusCode=HttpStatusCode.BadRequest;
				_response.IsSuccess=false;
				_response.ErrorMessages.Add("Username or password is incorrect");
				return BadRequest(_response);
			}
			_response.StatusCode = HttpStatusCode.OK;
			_response.IsSuccess = true;
			_response.Result=loginResponse;
			return Ok(_response);
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO model)
		{
			bool ifUserNameUnique = _userRepo.IsUniqueUser(model.UserName);
			if(!ifUserNameUnique)
			{
				_response.StatusCode = HttpStatusCode.BadRequest;
				_response.IsSuccess = false;
				_response.ErrorMessages.Add("Username already exists");
				return BadRequest(_response);
			}
			var user = await _userRepo.Register(model);
			if(user==null)
			{
				_response.StatusCode = HttpStatusCode.BadRequest;
				_response.IsSuccess = false;
				_response.ErrorMessages.Add("Error while registration");
				return BadRequest(_response);
			}
			_response.StatusCode = HttpStatusCode.OK;
			_response.IsSuccess = true;
			return Ok(_response);
		}
	}
}

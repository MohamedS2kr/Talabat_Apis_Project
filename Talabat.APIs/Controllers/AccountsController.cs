using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.APIs.Extensions;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services.Interfaces;

namespace Talabat.APIs.Controllers
{
    public class AccountsController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountsController(
                                  UserManager<AppUser> userManager,
                                  SignInManager<AppUser> signInManager,
                                  ITokenService tokenService,
                                  IMapper mapper) 
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }
        //Login
        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null) return Unauthorized(new ApiResponse(401));
            var Result = await _signInManager.CheckPasswordSignInAsync(user, model.Password,false);
            if (!Result.Succeeded) return Unauthorized(new ApiResponse(401));
            return Ok(new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = model.Email,
                Token = await _tokenService.CreateTokenAsync(user, _userManager)
            }) ;
            
        }
        //Register
        [HttpPost("Register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto model)
        {

            if (CheckEmailExists(model.Email).Result.Value)
            {
                return BadRequest(new ApiResponse(400, "email is already exists"));
            }

            var user = new AppUser()
            {
                DisplayName = model.DisplayName,
                Email = model.Email,
                UserName = model.Email.Split("@")[0],
                PhoneNumber = model.PhoneNumber,

            };
            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded) return BadRequest(new ApiResponse(400));
            var returnUser = new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await _tokenService.CreateTokenAsync(user, _userManager)

            };
            return Ok(returnUser); 
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("GetCurrentUser")]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var useremail = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(useremail);
            return Ok(new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await _tokenService.CreateTokenAsync(user, _userManager)
            });
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet ("CurrentUserAddress")]
        public async Task<ActionResult<AddressDto>> GetCurrentUserAddress()
        {
            //var userEmail = User.FindFirstValue(ClaimTypes.Email);

            var user =await _userManager.FindUserWithAddressAsync(User);
            var result = _mapper.Map<Address, AddressDto>(user.address);
            return Ok(result); 

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPut("address")]
        public async Task<ActionResult<AddressDto>> UpdateCurrentUserAddress(AddressDto model)
        {
            var user = await _userManager.FindUserWithAddressAsync(User);
            
            var address = _mapper.Map<AddressDto, Address>(model);
            
            user.address = address;
            
            var Result = await _userManager.UpdateAsync(user);
            if(!Result.Succeeded) return BadRequest(new ApiResponse(400));
            
            return Ok(model);
        }

        [HttpGet("emailExists")]
        public async Task<ActionResult<bool>> CheckEmailExists(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null) return false;
            return true;
        }
    }
}

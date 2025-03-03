﻿using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TestingSystem.Service.DTOs.Users;
using TestingSystem.Service.Interfaces.Users;

namespace TestingSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;
        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        /// <summary>
        /// Authorization
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async ValueTask<IActionResult> Login(UserForLoginDTO dto)
        {
            var token = await authService.GenerateToken(dto.Login, dto.Password);
            return Ok(new
            {
                token
            });
        }
    }
}

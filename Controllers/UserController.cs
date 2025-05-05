//using Login.Models;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Authentication.Google;
//using Microsoft.AspNetCore.Mvc;
//using System.Security.Claims;
//using Microsoft.AspNetCore.Authorization;
//// Required for ToListAsync


//namespace Login.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class UserController : ControllerBase
//    {
//        private readonly UserContext _context;
//        private readonly JwtService _jwtService;

//        public UserController(UserContext context)
//        {
//            _context = context;
//            _jwtService = jwtService;

//        }

//        // GET: api/user/{id}
//        [HttpGet("{id}")]
//        public async Task<IActionResult> GetUser(Guid id)
//        {
//            var user = await _context.users.FindAsync(id);
//            if (user == null)
//            {
//                return NotFound();
//            }
//            return Ok(user);
//        }

//        // GET: api/user
//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<Users>>> GetAllUsers()
//        {
//            var users = await _context.users.ToListAsync();
//            return Ok(users);
//        }
//        [AllowAnonymous]
//        [HttpPost]
//        public async Task<IActionResult> RegisterUser([FromBody] Users userModel)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);  // Return validation errors
//            }

//            // Check if the username or email already exists
//            var existingUser = await _context.users
//                .AnyAsync(u => u.Username == userModel.Username || u.Email == userModel.Email);

//            if (existingUser)
//            {
//                return Conflict("Username or Email already exists."); // Conflict if user exists
//            }

//            // Add the new user to the context
//            _context.users.Add(userModel);
//            await _context.SaveChangesAsync(); // Save changes to the database

//            return CreatedAtAction(nameof(GetUser), new { id = userModel.UserID }, userModel); // Return the created user
//        }

//        [AllowAnonymous]
//        [HttpPost("login")]
//        public async Task<IActionResult> Login([FromBody] Logins loginModel)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState); // Return validation errors
//            }

//            // Check if the user exists with the given email and password
//            var user = await _context.users
//                .FirstOrDefaultAsync(u => u.Email == loginModel.Email && u.Password == loginModel.Password); // Use hashed passwords in production

//            if (user == null)
//            {
//                return Unauthorized("Invalid email or password."); // Return 401 if credentials are invalid
//            }

//            // Here, you might want to generate a JWT token or return user details
//            var token = _jwtService.GenerateToken(user.UserID, user.FirstName, user.Username, user.Email);

//            return Ok(new
//            {
//                Message = "Login successful",
//                UserId = user.UserID,
//                UserName = user.Username,
//                Token = token // Return the generated token
//            });
//            //return Ok(new { Message = "Login successful", UserId = user.UserID, UserName=user.Username});
//        }

//        [HttpGet("google-login")]
//        public IActionResult GoogleLogin(string returnUrl = "/")
//        {
//            var redirectUrl = Url.Action("GoogleResponse", "Auth", new { ReturnUrl = returnUrl });
//            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
//            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
//        }

//        [HttpGet("google-response")]
//        public async Task<IActionResult> GoogleResponse(string returnUrl = "/")
//        {
//            var info = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
//            if (info.Principal == null)
//            {
//                return BadRequest("Authentication failed.");
//            }

//            // Extract user information from the info.Principal
//            var email = info.Principal.FindFirst(ClaimTypes.Email)?.Value;
//            var name = info.Principal.FindFirst(ClaimTypes.Name)?.Value;

//            // Check if the user exists in the database, or create a new user
//            var user = await _context.users.FirstOrDefaultAsync(u => u.Email == email);
//            if (user == null)
//            {
//                user = new Users
//                {
//                    Email = email,
//                    Username = name,
//                    // Add other user properties
//                };
//                _context.users.Add(user);
//                await _context.SaveChangesAsync();
//            }

//            // Generate JWT token here if required

//            return Ok(new { Message = "Login successful", UserId = user.UserID, UserName = user.Username });
//        }

//    }
//}
using Login.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json.Linq;

namespace Login.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserContext _context;
       // private readonly JwtService _jwtService;

        //public UserController(UserContext context, JwtService jwtService)
        //{
        //    _context = context;
        //    _jwtService = jwtService;
        //}

        public UserController(UserContext context )
        {
            _context = context;
           
        }

        // GET: api/user/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var user = await _context.users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        // GET: api/user
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Users>>> GetAllUsers()
        {
            var users = await _context.users.ToListAsync();
            return Ok(users);
        }


        //[HttpPost("register")]
        //public async Task<IActionResult> RegisterUser([FromBody] Users userModel)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    // Check if the username or email already exists
        //    var existingUser = await _context.users
        //        .AnyAsync(u => u.Username == userModel.Username || u.Email == userModel.Email);

        //    if (existingUser)
        //    {
        //        return Conflict("Username or Email already exists.");
        //    }

        //    // Hash the password (you should implement this securely)
        //    userModel.Password = HashPassword(userModel.Password);

        //    // Add the new user to the context
        //    _context.users.Add(userModel);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction(nameof(GetUser), new { id = userModel.UserID }, userModel);
        //}

        [AllowAnonymous]
        [HttpPost("register")] // Ensure the route matches the curl request
        public async Task<IActionResult> RegisterUser([FromBody] Users userModel)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            // Check if the username or email already exists
            var existingUser = await _context.users
                .AnyAsync(u => u.Username == userModel.Username || u.Email == userModel.Email);

            if (existingUser)
            {
                return Conflict("Username or Email already exists.");
            }

            // Hash the password (implement this securely)
            userModel.Password = HashPassword(userModel.Password);

            // Add the new user to the context
            _context.users.Add(userModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = userModel.UserID }, userModel);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Logins loginModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if the user exists with the given email
            var user = await _context.users.FirstOrDefaultAsync(u => u.Email == loginModel.Email);

            if (user == null || !VerifyPassword(user.Password, loginModel.Password))
            {
                return Unauthorized("Invalid email or password.");
            }

            //// Generate JWT token
            //var token = _jwtService.GenerateToken(
            //    user.UserID,
            //    user.FirstName,
            //    user.Username,
            //    user.Email
            //);

            //return Ok(new
            //{
            //    Message = "Login successful",
            //    UserId = user.UserID,
            //    UserName = user.Username,
            //    Token = token
            //});

            return Ok(new
            {
                Message = "Login successful",
                UserId = user.UserID,
                UserName = user.Username,
               // Token = token
            });
        }

        [HttpGet("google-login")]
        public IActionResult GoogleLogin(string returnUrl = "/")
        {
            var redirectUrl = Url.Action("GoogleResponse", "User", new { ReturnUrl = returnUrl });
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        // Example password hashing method
        private string HashPassword(string password)
        {
            // Implement your password hashing logic (e.g., using ASP.NET Core Identity)
            return password; // Placeholder, replace with actual hash
        }

        // Example password verification method
        private bool VerifyPassword(string hashedPassword, string password)
        {
            // Implement your password verification logic (e.g., using ASP.NET Core Identity)
            return hashedPassword == password; // Placeholder, replace with actual verification
        }
    }

}

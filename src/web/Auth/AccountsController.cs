namespace Hypomos.Web.Auth
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    using AutoMapper;

    using FluentValidation;

    using Hypomos.Web.Areas.Identity;
    using Hypomos.Web.Areas.Identity.Entities;
    using Hypomos.Web.Data;
    using Hypomos.Web.Helpers;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;

    using Newtonsoft.Json;
    //using FluentValidation.Attributes;

    public class CredentialsViewModelValidator : AbstractValidator<CredentialsViewModel>
    {
        public CredentialsViewModelValidator()
        {
            this.RuleFor(vm => vm.UserName).NotEmpty().WithMessage("Username cannot be empty");
            this.RuleFor(vm => vm.Password).NotEmpty().WithMessage("Password cannot be empty");
            this.RuleFor(vm => vm.Password).Length(6, 12).WithMessage("Password must be between 6 and 12 characters");
        }
    }

    //[Validator(typeof(CredentialsViewModelValidator))]
    public class CredentialsViewModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IJwtFactory _jwtFactory;
        private readonly JwtIssuerOptions _jwtOptions;

        public AuthController(UserManager<AppUser> userManager, IJwtFactory jwtFactory, IOptions<JwtIssuerOptions> jwtOptions)
        {
            this._userManager = userManager;
            this._jwtFactory = jwtFactory;
            this._jwtOptions = jwtOptions.Value;
        }

        // POST api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Post([FromBody]CredentialsViewModel credentials)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var identity = await this.GetClaimsIdentity(credentials.UserName, credentials.Password);
            if (identity == null)
            {
                return this.BadRequest(Errors.AddErrorToModelState("login_failure", "Invalid username or password.", this.ModelState));
            }

            var jwt = await Tokens.GenerateJwt(identity, this._jwtFactory, credentials.UserName, this._jwtOptions, new JsonSerializerSettings { Formatting = Formatting.Indented });
            return new OkObjectResult(jwt);
        }

        private async Task<ClaimsIdentity> GetClaimsIdentity(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                return await Task.FromResult<ClaimsIdentity>(null);

            // get the user to verifty
            var userToVerify = await this._userManager.FindByNameAsync(userName);

            if (userToVerify == null) return await Task.FromResult<ClaimsIdentity>(null);

            // check the credentials
            if (await this._userManager.CheckPasswordAsync(userToVerify, password))
            {
                return await Task.FromResult(this._jwtFactory.GenerateClaimsIdentity(userName, userToVerify.Id));
            }

            // Credentials are invalid, or account doesn't exist
            return await Task.FromResult<ClaimsIdentity>(null);
        }
    }

    [Route("api/[controller]")]
    public class AccountsController : Controller
    {
        private readonly ApplicationDbContext appDbContext;

        private readonly IMapper mapper;

        private readonly UserManager<AppUser> userManager;

        public AccountsController(UserManager<AppUser> userManager, IMapper mapper, ApplicationDbContext appDbContext)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.appDbContext = appDbContext;
        }

        // POST api/accounts
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RegistrationViewModel model)
        {
            if (!this.ModelState.IsValid) return this.BadRequest(this.ModelState);

            var userIdentity = this.mapper.Map<AppUser>(model);

            var result = await this.userManager.CreateAsync(userIdentity, model.Password);

            if (!result.Succeeded)
            {
                this.ModelState.TryAddModelError("error", "could not create user identity entry");
                return new BadRequestObjectResult(this.ModelState);
            }

            await this.appDbContext.Customers.AddAsync(new Customer
                                                            {
                                                                IdentityId = userIdentity.Id,
                                                                Location = model.Location
                                                            });

            await this.appDbContext.SaveChangesAsync();

            return new OkObjectResult("Account created");
        }
    }
}
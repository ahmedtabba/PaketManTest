using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PaketMan.Context;
using PaketMan.Contracts;
using PaketMan.Models;
using PaketMan.Repository;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<IRestaurantRepository, RestaurantRepository>();
builder.Services.AddScoped<IMealRepository, MealRepository>();
builder.Services.AddScoped<IIdentityService, IdentityService>();


builder.Services.AddTransient<IRoleStore<ApplicationRole>, RoleStore>();
builder.Services.AddTransient<IUserStore<ApplicationUser>, UserStore>();

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddDefaultTokenProviders();

builder.Services.AddTransient<RoleManager<ApplicationRole>>();
builder.Services.AddTransient<UserManager<ApplicationUser>>();






//services.AddDefaultIdentity<PortalUser>()
//    .AddRoles<IdentityRole>()


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(name: "v1", new OpenApiInfo() { Title = "API", Version = "V1" });


    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
"JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });

    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
});


var key = Encoding.UTF8.GetBytes("AAAidSecurityCode20_WithHappy");

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
           .AddJwtBearer(x =>
           {
               x.Events = new JwtBearerEvents
               {
                   OnTokenValidated = context =>
                   {
                       var userService = context.HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
                       var userId = context.Principal.Identity.Name;
                       var user = userService.FindByIdAsync(userId);
                       if (user == null)
                       {
                            // return unauthorized if user no longer exists
                           context.Fail("Unauthorized");
                       }
                       return Task.CompletedTask;
                   }
               };
               x.RequireHttpsMetadata = false;
               x.SaveToken = true;
               x.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuerSigningKey = true,
                   IssuerSigningKey = new SymmetricSecurityKey(key),
                   ValidateIssuer = false,
                   ValidateAudience = false,
                   RequireExpirationTime = false,
                   ValidateLifetime = true
               };
           });



var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();



}



app.UseAuthorization();

app.MapControllers();

app.Run();

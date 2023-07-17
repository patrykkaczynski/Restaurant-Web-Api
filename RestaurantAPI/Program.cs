using FluentValidation;
using FluentValidation.AspNetCore;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog.Web;
using RestaurantAPI;
using RestaurantAPI.Authorization;
using RestaurantAPI.Entities;
using RestaurantAPI.Middleware;
using RestaurantAPI.Models;
using RestaurantAPI.Services;
using RestaurantAPI.Validators;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
#region Lifetime of the object/Registration of dependencies
// AddSingleton - dana zale�no�� zostanie utworzona tylko raz podczas ca�ego trwania aplikacji czyli od jej uruchomienia do zamkni�cia
// AddScoped - ka�dy obiekt zostanie utworzony na nowo przy ka�dym zapytaniu wys�anym przez klienta (na jedno zapytanie, b�dzie tylko jedna istancja danego serwisu)
// AddTransient  - rejestruj�c serwisy t� metod�, obiekty b�d� utworzone za ka�dym razem kiedy odwo�uje si� do nich przez konstruktor. 
// Ka�da z tych trzech metod przyjmuje dwa typy generyczne. Pierwszym z nich jest typ abstrakcji na kt�r� trzeba zarejestrowa� konkretn� implementacj�
// Przez tak� rejestracj� wbudowany kontener zale�no�ci na podstawie wstrzykni�tego typu abstrakcji b�dzie w stanie automatycznie utworzy� nowy obiekt konkretnej klasy kt�ra zawiera dan� implementacj�
// Dzi�ki DI pozbywamy si� silnej zale�no�ci pomi�dzy klasami, dzi�ki czemu pisz�c testy dla danej klasy bez problemu mo�na zamokowa� dany interfejs po to aby zwr�ci� konkretne wyniki

//builder.Services.AddTransient<IWeatherForecastService, WeatherForecastService>();

#endregion

//Authentication Settings
var authenticationSettings = new AuthenticationSettings();

builder.Configuration.GetSection("Authentication").Bind(authenticationSettings);
builder.Services.AddSingleton(authenticationSettings);
builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = "Bearer";
    option.DefaultScheme = "Bearer";
    option.DefaultChallengeScheme = "Bearer";
}).AddJwtBearer(cfg =>
{
    cfg.RequireHttpsMetadata = false; // nie wymuszamy od klienta zapyta� przez protok� https
    cfg.SaveToken = true; // dany token powinien by� zapisany po stronie serwera do cel�w uwierzytelnienia
    cfg.TokenValidationParameters = new TokenValidationParameters // parametry walidacji po to aby sprawdzi� czy dany token wys�any przez klienta jest zgodny z tym co wie serwer
    {
        ValidIssuer = authenticationSettings.JwtIssuer, // wydawca danego tokenu
        ValidAudience = authenticationSettings.JwtIssuer, // jakie podmioty mog� u�ywa� tego tokenu (w tym wypadku jest to ta sama warto�� poniewa� token b�dzie generowany w obr�bie tej aplikacji i tylko tacy klienci b�d� dopuszczeni do uwierzytelnienia)
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey)), // klucz prywatny wygenerowany na podstawie warto�ci JwtKey kt�ra zapisana jest w pliku appsettings.json 
    };

});

// Authorization Settings
builder.Services.AddAuthorization(options =>
{
    #region Policy which requires to add nationality during user registration
    //options.AddPolicy("HasNationality", builder => builder.RequireClaim("Nationality"));
    #endregion

    // Policy which requires to add nationality during user registration with specified nationality name
    options.AddPolicy("HasNationality", builder => builder.RequireClaim("Nationality", "German", "Polish", "English"));

    // Custom Policy for Age
    options.AddPolicy("Atleast20", builder => builder.AddRequirements(new MinimumAgeRequirement(20)));

    // Custom Policy for users who created at least 2 restaurants
    options.AddPolicy("CreatedAtleast2Restaurants", builder => builder.AddRequirements(new CreatedMultipleRestaurantsRequirement(2)));
});


// NLog: Setup NLog for Dependency injection
builder.Logging.ClearProviders();
builder.Host.UseNLog();

builder.Services.AddScoped<IAuthorizationHandler, CreatedMultipleRestaurantsRequirementHandler>();
builder.Services.AddScoped<IAuthorizationHandler, MinimumAgeRequirementHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementHandler>();
builder.Services.AddControllers();
builder.Services.AddDbContext<RestaurantDbContext>();
builder.Services.AddScoped<RestaurantSeeder>();
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddScoped<IRestaurantService, RestaurantService>();
builder.Services.AddScoped<IDishService, DishService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
builder.Services.AddScoped<IValidator<RestaurantQuery>, RestaurantQueryValidator>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddScoped<ErrorHandlingMiddleware>();
builder.Services.AddScoped<RequestTimeMiddleware>();
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddHttpContextAccessor(); // dzi�ki temu mo�liwe jest wstrzykni�cie IHttpContextAccessor do klasy UserContextService

// Swagger Configuration
builder.Services.AddSwaggerGen(options =>
{
    // Adding of authorization feature. This feature consists of an "Autorize" button at the top of the page
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Description = "Enter the Bearer Authorization string as following: `Bearer Generated-JWT-Token`",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    // AddSecurityRequirement extension method will add an authorization header to each endpoint when the request is sent
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Name = "Bearer", 
                In = ParameterLocation.Header,
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()

        }
    });
    
});

builder.Services.AddFluentValidationRulesToSwagger();

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontEndClient", policyBuilder =>

        policyBuilder.AllowAnyMethod()
        .AllowAnyHeader()
        .WithOrigins(builder.Configuration["AllowedOrigins"])
    );
});

var app = builder.Build();


// Configure the HTTP request pipeline.
#region Notes about Middleware
// Wszystkie niezb�dne metody przep�ywu, przez kt�re musi przej�� zapytanie do API przed zwr�ceniem odpowiedzi
// Ka�da metoda przep�ywu wywo�ywana na Application Builderze jest nazywana middleware
// Middleware to kawa�ek kodu, kt�ry ma dost�p do dw�ch rzeczy
// Pierwsz� z nich jest to kontekst zapytania czyli informacje o czasowniku HTTP, nag��wkach i adresie
// Drug� z nich jest dost�p do kolejnego middleware 
// Mo�liwe jest dodawanie w�asnych middleware lub korzystanie z istniej�cych
// Wazna jest kolejno�� wywo�ywania tych metod
#endregion
using var scope = app.Services.CreateScope();
var seeder = scope.ServiceProvider.GetRequiredService<RestaurantSeeder>();

seeder.Seed();

//var dbContext = scope.ServiceProvider.GetRequiredService<RestaurantDbContext>();
//DataGenerator.Seed(dbContext);

app.UseResponseCaching();
app.UseStaticFiles();
app.UseCors("FrontEndClient");
app.UseDeveloperExceptionPage();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestTimeMiddleware>();
app.UseAuthentication(); // ka�dy request wys�any przez klienta API b�dzie podlega� uwierzytelnieniu


app.UseHttpsRedirection(); // je�li klient wy�le zapytanie bez protoko�u https to jego zapytanie zostanie automatycznie przekierowane na adres z protoko�em https

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant API");
    
});
app.UseRouting();
app.UseAuthorization();

app.MapControllers(); // zapytanie, kt�re zostanie wys�ane przez przegl�dark� na podany adres zostanie odpowiednio zmapowane do wywo�ania akcji w danym kontrolerze poprzez atrybut Route

app.Run();



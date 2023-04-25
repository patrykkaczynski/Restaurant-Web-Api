using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using NLog.Web;
using RestaurantAPI;
using RestaurantAPI.Entities;
using RestaurantAPI.Middleware;
using RestaurantAPI.Models;
using RestaurantAPI.Services;
using RestaurantAPI.Validators;
using System.Reflection;
using System.Runtime;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
#region Lifetime of the object/Registration of dependencies
// AddSingleton - dana zale¿noœæ zostanie utworzona tylko raz podczas ca³ego trwania aplikacji czyli od jej uruchomienia do zamkniêcia
// AddScoped - ka¿dy obiekt zostanie utworzony na nowo przy ka¿dym zapytaniu wys³anym przez klienta (na jedno zapytanie, bêdzie tylko jedna istancja danego serwisu)
// AddTransient  - rejestruj¹c serwisy tê metod¹, obiekty bêd¹ utworzone za ka¿dym razem kiedy odwo³uje siê do nich przez konstruktor. 
// Ka¿da z tych trzech metod przyjmuje dwa typy generyczne. Pierwszym z nich jest typ abstrakcji na któr¹ trzeba zarejestrowaæ konkretn¹ implementacjê
// Przez tak¹ rejestracjê wbudowany kontener zale¿noœci na podstawie wstrzykniêtego typu abstrakcji bêdzie w stanie automatycznie utworzyæ nowy obiekt konkretnej klasy która zawiera dan¹ implementacjê
// Dziêki DI pozbywamy siê silnej zale¿noœci pomiêdzy klasami, dziêki czemu pisz¹c testy dla danej klasy bez problemu mo¿na zamokowaæ dany interfejs po to aby zwróciæ konkretne wyniki

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
    cfg.RequireHttpsMetadata = false; // nie wymuszamy od klienta zapytañ przez protokó³ https
    cfg.SaveToken = true; // dany token powinien byæ zapisany po stronie serwera do celów uwierzytelnienia
    cfg.TokenValidationParameters = new TokenValidationParameters // parametry walidacji po to aby sprawdziæ czy dany token wys³any przez klienta jest zgodny z tym co wie serwer
    {
        ValidIssuer = authenticationSettings.JwtIssuer, // wydawca danego tokenu
        ValidAudience = authenticationSettings.JwtIssuer, // jakie podmioty mog¹ u¿ywaæ tego tokenu (w tym wypadku jest to ta sama wartoœæ poniewa¿ token bêdzie generowany w obrêbie tej aplikacji i tylko tacy klienci bêd¹ dopuszczeni do uwierzytelnienia)
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey)), // klucz prywatny wygenerowany na podstawie wartoœci JwtKey która zapisana jest w pliku appsettings.json 
    };

});


// NLog: Setup NLog for Dependency injection
builder.Logging.ClearProviders();
builder.Host.UseNLog();

builder.Services.AddControllers();
builder.Services.AddDbContext<RestaurantDbContext>();
builder.Services.AddScoped<RestaurantSeeder>();
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddScoped<IRestaurantService, RestaurantService>();
builder.Services.AddScoped<IDishService, DishService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddScoped<ErrorHandlingMiddleware>();
builder.Services.AddScoped<RequestTimeMiddleware>();
builder.Services.AddSwaggerGen();

var app = builder.Build();


// Configure the HTTP request pipeline.
#region Notes about Middleware
// Wszystkie niezbêdne metody przep³ywu, przez które musi przejœæ zapytanie do API przed zwróceniem odpowiedzi
// Ka¿da metoda przep³ywu wywo³ywana na Application Builderze jest nazywana middleware
// Middleware to kawa³ek kodu, który ma dostêp do dwóch rzeczy
// Pierwsz¹ z nich jest to kontekst zapytania czyli informacje o czasowniku HTTP, nag³ówkach i adresie
// Drug¹ z nich jest dostêp do kolejnego middleware 
// Mo¿liwe jest dodawanie w³asnych middleware lub korzystanie z istniej¹cych
// Wazna jest kolejnoœæ wywo³ywania tych metod
#endregion
using var scope = app.Services.CreateScope();
var seeder = scope.ServiceProvider.GetRequiredService<RestaurantSeeder>();

seeder.Seed();

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestTimeMiddleware>();
app.UseAuthentication(); // ka¿dy request wys³any przez klienta API bêdzie podlega³ uwierzytelnieniu
app.UseAuthorization();

app.UseHttpsRedirection(); // jeœli klient wyœle zapytanie bez protoko³u https to jego zapytanie zostanie automatycznie przekierowane na adres z protoko³em https

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant API");
});

app.MapControllers(); // zapytanie, które zostanie wys³ane przez przegl¹darkê na podany adres zostanie odpowiednio zmapowane do wywo³ania akcji w danym kontrolerze poprzez atrybut Route

app.Run();



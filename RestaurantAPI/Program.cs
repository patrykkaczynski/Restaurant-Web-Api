using RestaurantAPI;
using RestaurantAPI.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
#region Lifetime of the object/Registration of dependencies
// AddSingleton - dana zale�no�� zostanie utworzona tylko raz podczas ca�ego trwania aplikacji czyli od jej uruchomienia do zamkni�cia
// AddScoped - ka�dy obiekt zostanie utworzony na nowo przy ka�dym zapytaniu wys�anym przez klienta (na jedno zapytanie, b�dzie tylko jedna istancja danego serwisu)
// AddTransient  - rejestruj�c serwisy t� metod�, obiekty b�d� utworzone za ka�dym razem kiedy odwo�uje si� do nich przez konstruktor. 
// Ka�da z tych trzech metod przyjmuje dwa typy generyczne. Pierwszym z nich jest typ abstrakcji na kt�r� trzeba zarejestrowa� konkretn� implementacj�
// Przez tak� rejestracj� wbudowany kontener zale�no�ci na podstawie wstrzykni�tego typu abstrakcji b�dzie w stanie automatycznie utworzy� nowy obiekt konkretnej klasy kt�ra zawiera dan� implementacj�
// Dzi�ki DI pozbywamy si� silnej zale�no�ci pomi�dzy klasami, dzi�ki czemu pisz�c testy dla danej klasy bez problemu mo�na zamokowa� dany interfejs po to aby zwr�ci� konkretne wyniki
#endregion
builder.Services.AddTransient<IWeatherForecastService, WeatherForecastService>();
builder.Services.AddControllers();
builder.Services.AddDbContext<RestaurantDbContext>();
builder.Services.AddScoped<RestaurantSeeder>();

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

app.UseHttpsRedirection(); // je�li klient wy�le zapytanie bez protoko�u https to jego zapytanie zostanie automatycznie przekierowane na adres z protoko�em https

app.MapControllers(); // zapytanie, kt�re zostanie wys�ane przez przegl�dark� na podany adres zostanie odpowiednio zmapowane do wywo�ania akcji w danym kontrolerze poprzez atrybut Route

app.Run();



using RestaurantAPI;
using RestaurantAPI.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
#region Lifetime of the object/Registration of dependencies
// AddSingleton - dana zale¿noœæ zostanie utworzona tylko raz podczas ca³ego trwania aplikacji czyli od jej uruchomienia do zamkniêcia
// AddScoped - ka¿dy obiekt zostanie utworzony na nowo przy ka¿dym zapytaniu wys³anym przez klienta (na jedno zapytanie, bêdzie tylko jedna istancja danego serwisu)
// AddTransient  - rejestruj¹c serwisy tê metod¹, obiekty bêd¹ utworzone za ka¿dym razem kiedy odwo³uje siê do nich przez konstruktor. 
// Ka¿da z tych trzech metod przyjmuje dwa typy generyczne. Pierwszym z nich jest typ abstrakcji na któr¹ trzeba zarejestrowaæ konkretn¹ implementacjê
// Przez tak¹ rejestracjê wbudowany kontener zale¿noœci na podstawie wstrzykniêtego typu abstrakcji bêdzie w stanie automatycznie utworzyæ nowy obiekt konkretnej klasy która zawiera dan¹ implementacjê
// Dziêki DI pozbywamy siê silnej zale¿noœci pomiêdzy klasami, dziêki czemu pisz¹c testy dla danej klasy bez problemu mo¿na zamokowaæ dany interfejs po to aby zwróciæ konkretne wyniki
#endregion
builder.Services.AddTransient<IWeatherForecastService, WeatherForecastService>();
builder.Services.AddControllers();
builder.Services.AddDbContext<RestaurantDbContext>();
builder.Services.AddScoped<RestaurantSeeder>();

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

app.UseHttpsRedirection(); // jeœli klient wyœle zapytanie bez protoko³u https to jego zapytanie zostanie automatycznie przekierowane na adres z protoko³em https

app.MapControllers(); // zapytanie, które zostanie wys³ane przez przegl¹darkê na podany adres zostanie odpowiednio zmapowane do wywo³ania akcji w danym kontrolerze poprzez atrybut Route

app.Run();



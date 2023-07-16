<br />
<h1>
<p align="center">
  <br>Restaurant WebApi 
</p>
<p align="center">
    <img src="https://raw.githubusercontent.com/gildean/foodicon/HEAD/favicons/Hamburger.ico" alt="Logo" width="30" height="30">
    <img src="https://raw.githubusercontent.com/gildean/foodicon/HEAD/favicons/French_Fries.ico" alt="Logo" width="30" height="30">
    <img src="https://raw.githubusercontent.com/gildean/foodicon/HEAD/favicons/Green_Salad.ico" alt="Logo" width="30" height="30">
    <img src="https://raw.githubusercontent.com/gildean/foodicon/HEAD/favicons/Slice_Of_Pizza.ico" alt="Logo" width="30" height="30">
    <img src="https://raw.githubusercontent.com/gildean/foodicon/HEAD/favicons/Taco.ico" alt="Logo" width="30" height="30">
</p>  
</h1>
</p>
<div align="center">

**[GENERAL INFO](#General-Info) • 
[TECHNOLOGIES USED](#Technologies-Used) • 
[PACKAGES USED](#Packages-Used) • 
[FEATURES](#Features) • 
[SCREENSHOTS](#Screenshots) • 
[SETUP](#Setup)**
</div>
<br />

# :information_source: General Info
The purpose of the project was to create a web api which enables CRUD (create, read, update, delete) operations for restaurants and their lists of dishes. The program includes user registration and log-in, handling an authentication process by using JWT token (JSON Web Token) in this way. Every user is authorized based on its role or custom policy in order to check its privileges to specific endpoints in the application. Additionally, as a part of the training, downloading a file from server and sending a file to server have been implemented in the program.

The project has been created based on practical course on Udemy platform in order to understand the principle of operation of every web application and what elements it should consist of. The important aspect of learning was integration of the program with database by using Entity Framework Core.

# :hammer_and_wrench: Technologies Used
- C#
- [ASP.NET Core 7.0](https://github.com/dotnet/aspnetcore)
- [Entity Framework Core 7.0](https://github.com/dotnet/efcore)
- MS SQL Server

# Packages Used
- [AutoMapper (v12.0.1)](https://github.com/AutoMapper/AutoMapper) -  mapping DTOs (data transfer objects) into domain objects and domain objects into DTOs
- [FluentValidation (v11.3.0)](https://github.com/FluentValidation/FluentValidation) - validating of input during user registration
- [NLog (v5.2.3)](https://github.com/NLog/NLog) - logging of messages to files to have a possibility to debug the application in the production environment
- [Swashbuckle.AspNetCore (v6.5.0)](https://github.com/domaindrivendev/Swashbuckle.AspNetCore/tree/master) - using of swagger tool in order to document the application which is built on ASP.NET Core
- [MicroElements.Swashbuckle.FluentValidation (v5.7.0)](https://github.com/micro-elements/MicroElements.Swashbuckle.FluentValidation) - using of FluentValidation rules instead of ComponentModel attributes to define swagger schema
- [Bogus (v34.0.2)](https://github.com/bchavez/Bogus) - generating of fake data for restaurants and dishes


# ⚙️ Features

# Screenshots

# Setup

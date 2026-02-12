using Oauth_1a_Demo;
using Oauth_1a_Demo.Middleware;
using Oauth_1a_Demo.Model;
using Oauth_1a_Demo.Services;

var builder = WebApplication.CreateBuilder(args);


//Temporary code to get encrypted Keys
var demoKey = builder.Configuration["OAuth:DecryptionKey"];
string[] allSecrets = new[] { "tokenSecret1" , "tokenSecret2" , "consumerSecret1" , "tokenSecret3" , "tokenSecret4" , "consumerSecret2" };
foreach (var secret in allSecrets)
{
    string encryptedText = EncryptionHelper.Encrypt(secret, demoKey);
    Console.WriteLine("Name : "+ secret +" Encrypted: " + encryptedText);
}


// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<OAuthValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseMiddleware<OAuthMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.Run();

using Backend.Converters;
using Python.Runtime;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new IPersonConverter());
        options.JsonSerializerOptions.Converters.Add(new IPersonDTOConverter());
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Specify the path to the Python DLL
Runtime.PythonDLL = @"C:\Users\Dotev\AppData\Local\Programs\Python\Python312\python312.dll";
PythonEngine.Initialize();
PythonEngine.BeginAllowThreads();


// Register the shutdown event before running the app
AppDomain.CurrentDomain.ProcessExit += (s, e) =>
{
    PythonEngine.Shutdown();
    Console.WriteLine("Python Engine Shut Down");
};

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
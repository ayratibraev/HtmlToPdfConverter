var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors();

// app.UseStaticFiles(new StaticFileOptions()
// {
//     OnPrepareResponse = ctx => {
//         ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
//         ctx.Context.Response.Headers.Append("Access-Control-Allow-Headers", 
//             "Origin, X-Requested-With, Content-Type, Accept");
//     },
//
// });

app.Run();
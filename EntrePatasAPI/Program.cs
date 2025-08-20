using EntrePatasAPI.Data;
using EntrePatasAPI.Data.Contrato;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//DEPENDENCIAS
builder.Services.AddScoped<IProducto, ProductoRepositorio>();
builder.Services.AddScoped<IUsuario, UsuarioRepositorio>();
builder.Services.AddScoped<ISolicitud,  SolicitudRepositorio>();
builder.Services.AddScoped<IAnimal, AnimalRepositorio>();
builder.Services.AddScoped<IVacuna, VacunaRepositorio>();
builder.Services.AddScoped<ISolicitudVacuna, SolicitudVacunaRepositorio>();
builder.Services.AddScoped<IPedido, PedidoRepositorio>();
builder.Services.AddScoped<IDetallePedido, DetallePedidoRepositorio>();
builder.Services.AddScoped<IPago, PagoRepositorio>();   
builder.Services.AddScoped<IEnvio, EnvioRepositorio>();







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

app.Run();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin() // �����κ���Դ
                   .AllowAnyMethod() // �����κ�HTTP����
                   .AllowAnyHeader(); // �����κ�ͷ����Ϣ
        });
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddServer(new Microsoft.OpenApi.Models.OpenApiServer()
    {
        Url = "https://sbox.e-desk.cloud"
    });
    var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);
    if (string.IsNullOrWhiteSpace(basePath))
        return;
    var xmlPath = Path.Combine(basePath, "SBox.xml");
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();
app.UseCors("AllowAllOrigins");

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(x => { x.SwaggerEndpoint("/swagger/v1/swagger.yaml", "API"); });

app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();

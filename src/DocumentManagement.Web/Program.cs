var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.ApplicationService(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors();
app.UseRouting();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Elsa"));

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpActivities(); // Install middleware for triggering HTTP Endpoint activities. 

app.MapRazorPages();
app.MapControllers(); // Elsa API Endpoints are implemented as ASP.NET API controllers.

app.Run();

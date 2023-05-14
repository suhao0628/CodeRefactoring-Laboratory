using Microsoft.OpenApi.Models;

namespace Delivery.Api.Extensions
{
    public static class SwaggerExtension
    {
        public static void AddSwaggerExt(this WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                //Version control
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Delivery.Api",
                    Version = "v1",
                    Description = ""
                });
                //Add comment
                var file = Path.Combine(AppContext.BaseDirectory, "Delivery.Api.xml");
                options.IncludeXmlComments(file, true);

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description =
                        "JWT Authorization header using the Bearer scheme. \r\n\r\n " +
                        "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
                        "Example: \"Bearer 12345abcdef\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Scheme = "Bearer"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });
        }

        public static void UseSwaggerExt(this WebApplication app)
        {
            //Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
        }

    }
}

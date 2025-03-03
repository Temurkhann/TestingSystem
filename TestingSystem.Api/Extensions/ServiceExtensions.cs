﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StarBucks.Service.Services;
using System.Linq;
using System.Net.Mail;
using System.Text;
using TestingSystem.Data.IRepositories;
using TestingSystem.Data.Repositories;
using TestingSystem.Domain.Entities.Courses;
using TestingSystem.Domain.Entities.Quizes;
using TestingSystem.Domain.Entities.Users;
using TestingSystem.Domain.Entities.Attachments;
using TestingSystem.Service.Interfaces.Courses;
using TestingSystem.Service.Interfaces.Questiones;
using TestingSystem.Service.Interfaces.Quizes;
using TestingSystem.Service.Interfaces.Users;
using TestingSystem.Service.Mappers;
using TestingSystem.Service.Services;
using TestingSystem.Service.Services.CourseServices;
using TestingSystem.Service.Services.Quizes;
using Attachment = TestingSystem.Domain.Entities.Attachments.Attachment;

namespace StarBucks.Api.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddCustomServices(this IServiceCollection services)
        {
            // mapper
            
            services.AddAutoMapper(typeof(MappingProfile));

            // repos
            services.AddScoped<IGenericRepository<User>, GenericRepository<User>>();
            services.AddScoped<IGenericRepository<Course>, GenericRepository<Course>>();
            services.AddScoped<IGenericRepository<Quiz>, GenericRepository<Quiz>>();
            services.AddScoped<IGenericRepository<Question>, GenericRepository<Question>>();
            services.AddScoped<IGenericRepository<Answer>, GenericRepository<Answer>>();
            services.AddScoped<IGenericRepository<QuizResult>, GenericRepository<QuizResult>>();
            services.AddScoped<IGenericRepository<Attachment>, GenericRepository<Attachment>>();
            // services

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<IQuizService, QuizService>();
            services.AddScoped<IQuestionService, QuestionService>();
            services.AddScoped<IAnswerService, AnswerService>();
            services.AddScoped<IQuizResultService, QuizResultService>();
            services.AddScoped<IAttachmentService, AttachmentService>();

        }

        public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("Jwt");

            string key = jwtSettings.GetSection("Key").Value;

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.GetSection("Issuer").Value,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))

                };
            });
        }

        public static void AddSwaggerService(this IServiceCollection services)
        {
            services.AddSwaggerGen(p =>
            {
                p.ResolveConflictingActions(ad => ad.FirstOrDefault());
                p.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                });

                p.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme()
                        {
                            Reference = new OpenApiReference()
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });
        }
    }
}

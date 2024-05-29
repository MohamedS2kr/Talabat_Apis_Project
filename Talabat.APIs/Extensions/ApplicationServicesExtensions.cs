using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core;
using Talabat.Core.Interfaces;
using Talabat.Core.Services.Interfaces;
using Talabat.Repository;
using Talabat.Repository.Repositories;
using Talabat.Services;

namespace Talabat.APIs.Extensions
{
    public static class ApplicationServicesExtensions 
    {
        public static IServiceCollection AddApplicationServices( this IServiceCollection services)
        {
            services.AddScoped<IPaymentServices, PaymentService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            //builder.Services.AddAutoMapper( M => M.AddProfile(new MappingProfiles())); //(: عايزين حاجه اسهل من الشكل ده   
            services.AddAutoMapper(typeof(MappingProfiles)); // الشكل ده احسن واسهل بكتييييييير 
            services.Configure<ApiBehaviorOptions>(option =>
            {
                option.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var errors = actionContext.ModelState.Where(P => P.Value.Errors.Count() > 0)
                                            .SelectMany(P => P.Value.Errors)
                                            .Select(E => E.ErrorMessage)
                                            .ToArray();

                    var validationErrorResponse = new ValidationErrorResponse()
                    {
                        Errors = errors
                    };

                    return new BadRequestObjectResult(validationErrorResponse);
                };
            });

            return services;
        }
    }
}

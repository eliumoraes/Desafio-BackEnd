using Domain.Common.Interfaces;
using Domain.User;
using MediatR;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;


namespace Application.Commands.Users.RegisterDeliveryDriver;

public class RegisterDeliveryDriverRequest : IRequest<IResult<RegisterDeliveryDriverResponse>>
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string BusinessIdentificationNumber { get; set; }
    public string DriverLicenseNumber { get; set; }
    public List<string> DriverLicenseTypes { get; set; }
    public string Name { get; set; }
    public string BirthDate { get; set; }
    [JsonIgnore]
    public IFormFile? DriverLicenseImage { get; set; }
    
}
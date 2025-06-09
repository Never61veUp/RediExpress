using System.ComponentModel.DataAnnotations;
using RediExpress.Core.Model.ValueObjects;
using Swashbuckle.AspNetCore.Annotations;

namespace RediExpress.Host.Contracts;

public record SignUpRequest(
    [SwaggerSchema("Имя")]
    [Required(ErrorMessage = "Введите имя")]
    string firstName, 
    [SwaggerSchema("Фамилия")]
    [Required(ErrorMessage = "Введите фамилию")]
    string lastName, 
    [SwaggerSchema("Отчество")]
    string? middleName, 
    [SwaggerSchema("Почтовый адрес")]
    [Required(ErrorMessage = "Введите почтовый адрес")]
    string email, 
    [SwaggerSchema("Номер телефона")]
    [Required(ErrorMessage = "Введите номер телефона")]
    string phoneNumber, 
    [SwaggerSchema("Пароль")]
    [Required(ErrorMessage = "Введите пароль")]
    string password);
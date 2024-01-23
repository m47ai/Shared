namespace M47.Shared.Extensions;

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

public static class OptionsBuilderFluentValidationExtensions
{
    public static OptionsBuilder<TOptions> ValidateFluently<TOptions>(this OptionsBuilder<TOptions> optionsBuilder) where TOptions : class
    {
        optionsBuilder.Services.AddSingleton<IValidateOptions<TOptions>>(
            s => new FluentValidationOptions<TOptions>(optionsBuilder.Name, s.GetRequiredService<IValidator<TOptions>>()));
        return optionsBuilder;
    }
}

public class FluentValidationOptions<TOptions> : IValidateOptions<TOptions> where TOptions : class
{
    public string? Name { get; }
    private readonly IValidator<TOptions> _validator;

    public FluentValidationOptions(string? name, IValidator<TOptions> validator)
    {
        Name = name;
        _validator = validator;
    }

    public ValidateOptionsResult Validate(string? name, TOptions options)
    {
        // Null name is used to configure all named options.
        if (Name is not null && Name != name)
        {
            // Ignored if not validating this instance.
            return ValidateOptionsResult.Skip;
        }

        ArgumentNullException.ThrowIfNull(options);

        var validatorResult = _validator.Validate(options);

        if (validatorResult.IsValid)
        {
            return ValidateOptionsResult.Success;
        }

        var errors = validatorResult.Errors.Select(x =>
            $"Options validation failed for '{x.PropertyName}' with error: '{x.ErrorMessage}'.");

        return ValidateOptionsResult.Fail(errors);
    }
}
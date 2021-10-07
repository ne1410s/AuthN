﻿using FluentValidation;

namespace AuthN.Domain.Services.Validation
{
    /// <summary>
    /// Extensions for <see cref="FluentValidation"/>.
    /// </summary>
    public static class FluentValidatorExtensions
    {
        /// <summary>
        /// Validates that a password meets complexity requirements.
        /// </summary>
        /// <typeparam name="T">The model type.</typeparam>
        /// <param name="ruleBuilder">The model rule builder.</param>
        /// <returns>Rule builder options.</returns>
        public static IRuleBuilderOptions<T, string> IsSufficientlyComplex<T>(
            this IRuleBuilder<T, string> ruleBuilder)
        {
            const string containsText = "Password must contain at least one";
            return ruleBuilder.ChildRules(a =>
            {
                a.RuleFor(x => x)
                    .Matches("[A-Z]")
                    .WithMessage($"{containsText} capital letter");
                a.RuleFor(x => x)
                    .Matches("[a-z]")
                    .WithMessage($"{containsText} lower case letter");
                a.RuleFor(x => x)
                    .Matches("\\d")
                    .WithMessage($"{containsText} number");
                a.RuleFor(x => x)
                    .Matches("\\W")
                    .WithMessage($"{containsText} special character");
            });
        }
    }
}

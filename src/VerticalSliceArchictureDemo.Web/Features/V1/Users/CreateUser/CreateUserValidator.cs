using FluentValidation;

namespace VerticalSliceArchictureDemo.Web.Features.V1.Users.CreateUser
{
    public class CreateUserValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserValidator()
            => RuleFor(x => x.Username)
                .NotEmpty()
                .MaximumLength(255);
    }
}
using FluentValidation;

namespace VerticalSliceArchictureDemo.Web.Features.V1.Users.UpdateUser
{
    public class UpdateUserValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserValidator()
            => RuleFor(x => x.Username)
                .NotEmpty()
                .MaximumLength(255);
    }
}

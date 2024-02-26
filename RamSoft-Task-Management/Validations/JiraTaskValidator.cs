using FluentValidation;
using RamSoft_Task_Management.Models;

namespace RamSoft_Task_Management.Validations
{
    public class JiraTaskValidator : AbstractValidator<JiraTask>
    {
        public JiraTaskValidator()
        {

            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required").MaximumLength(100).WithMessage("Max 100 Characters Allowed");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required");
            RuleFor(x => x.Deadline).NotEmpty().WithMessage("Deadline is required").GreaterThan(DateTime.Now);
            RuleFor(x => x.Status).IsInEnum().WithMessage("Invalid status");
        }
    }
}

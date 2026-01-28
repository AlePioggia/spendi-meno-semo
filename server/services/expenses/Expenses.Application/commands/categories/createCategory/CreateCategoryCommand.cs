using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Application.commands.categories.createCategory
{
    public sealed record CreateCategoryCommand(
        string name, 
        string description,
        DateTime createdAt
    ): IRequest;
}

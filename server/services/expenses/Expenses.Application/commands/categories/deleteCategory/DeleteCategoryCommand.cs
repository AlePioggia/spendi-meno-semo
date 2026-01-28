using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Application.commands.categories.deleteCategory
{
    public sealed record DeleteCategoryCommand(
        long id
    ): IRequest;
}

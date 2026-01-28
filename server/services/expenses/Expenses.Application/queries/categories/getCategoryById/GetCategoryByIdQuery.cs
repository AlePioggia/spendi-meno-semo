using Expenses.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Application.queries.categories.getCategoryById
{
    public sealed record GetCategoryByIdQuery(
        long CategoryId
    ) : IRequest<Category?>;
}
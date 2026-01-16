using Expenses.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Application.queries.categories.getCategories
{
    public sealed record GetCategoriesQuery(long TenantId, long UserId) : IRequest<List<Category>>;
}

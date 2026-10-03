using Expenses.Application.repositories;
using Expenses.Application.services;
using Expenses.Domain.Entities;
using MediatR;

namespace Expenses.Application.queries.transactions
{
    public sealed record GetTransactionsReportQuery() : IRequest<byte[]>;

    public class GetTransactionsReportHandler : IRequestHandler<GetTransactionsReportQuery, byte[]>
    {
        private readonly IRepository<Transaction, long> _transactionRepository;
        private readonly IRepository<Category, long> _categoryRepository;
        private readonly ITransactionsReportService _transactionsReportService;

        public GetTransactionsReportHandler(
            IRepository<Transaction, long> transactionRepository,
            IRepository<Category, long> categoryRepository,
            ITransactionsReportService transactionsReportService)
        {
            _transactionRepository = transactionRepository;
            _categoryRepository = categoryRepository;
            _transactionsReportService = transactionsReportService;
        }

        public async Task<byte[]> Handle(GetTransactionsReportQuery request, CancellationToken cancellationToken)
        {
            var transactions = await _transactionRepository.GetAllAsync();
            var categories = await _categoryRepository.GetAllAsync();

            return await _transactionsReportService.CreateReportAsync(transactions, categories, cancellationToken);
        }
    }
}

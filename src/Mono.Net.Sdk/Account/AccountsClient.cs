﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Mono.Net.Sdk.Config;
using Mono.Net.Sdk.Interfaces;
using Mono.Net.Sdk.Models;
using Mono.Net.Sdk.Models.Account;
using System.Globalization;


namespace Mono.Net.Sdk.Account
{
    public class AccountsClient : IAccountsClient
    {
        private readonly IBaseApiClient _apiClient;
        private readonly IApiAuthHeader _apiAuthHeader;

        public AccountsClient(IBaseApiClient apiClient, ApiConfig config)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            if (config == null) throw new ArgumentNullException(nameof(config));
            _apiAuthHeader = new SecretKeyAuthHeader(config);
        }
        
        public async Task<ApiResponse<InformationResponse>> GetInformation(string accountId,  CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(accountId)) throw new ArgumentNullException(nameof(accountId));
            var response = await _apiClient.GetHttpAsync<InformationResponse>($"accounts/{accountId}", cancellationToken);
            return response.ToApiResponse();
        }
        
        public async Task<ApiResponse<StatementResponse>> GetStatementsInJson(string accountId, int period = 1, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(accountId)) throw new ArgumentNullException(nameof(accountId)); 
            var periodText = $"last{period}months";
            var statementRequest = new StatementRequest {Output = OutputType.Json.ToString(), Period = periodText};
            var queryString = statementRequest.PathWithQuery("statement");
            var response = await _apiClient.GetHttpAsync<StatementResponse>($"accounts/{accountId}/{queryString}", cancellationToken);
            return response.ToApiResponse();
        }
        
        public async Task<ApiResponse<StatementPdfResponse>> GetStatementsPdf(string accountId, int period = 1,  CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(accountId)) throw new ArgumentNullException(nameof(accountId)); 
            var periodText = $"last{period}months";
            var statementRequest = new StatementRequest {Output = OutputType.Pdf.ToString(), Period = periodText};
            var queryString = statementRequest.PathWithQuery("statement");
            var response = await _apiClient.GetHttpAsync<StatementPdfResponse>($"accounts/{accountId}/{queryString}", cancellationToken);
            return response.ToApiResponse();
        }
        
        public async Task<ApiResponse<StatementPdfResponse>> GetPollPdfAccountStatementStatus(string accountId, string jobId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(accountId)) throw new ArgumentNullException(nameof(accountId)); 
            if (string.IsNullOrWhiteSpace(jobId)) throw new ArgumentNullException(nameof(jobId));  
            var response = await _apiClient.GetHttpAsync<StatementPdfResponse>($"accounts/{accountId}/statement/jobs/{jobId}", cancellationToken);
            return response.ToApiResponse();
        }

        public async Task<ApiResponse<TransactionsResponse>> GetTransactions(string accountId,
            string start = null,
            string end = null, 
            string narration = null, 
            int limit = 0, 
            string type = TransactionType.Credit,
            bool paginate = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            CultureInfo currentCulture = CultureInfo.CurrentCulture;

            if (string.IsNullOrWhiteSpace(accountId)) throw new ArgumentNullException(nameof(accountId));

            
            if (!string.IsNullOrWhiteSpace(start) && !DateTime.TryParseExact(start, "dd-MM-yyyy", currentCulture, DateTimeStyles.None, out _))
                throw new ArgumentException("Invalid date format; please use dd-mm-yyy ie 05-01-2020");
             
            if (!string.IsNullOrWhiteSpace(end) &&  !DateTime.TryParseExact(end, "dd-MM-yyyy", currentCulture, DateTimeStyles.None, out _)) 
                throw new ArgumentException("Invalid date format; please use dd-mm-yyy ie 05-01-2020");
            
            if(!string.IsNullOrWhiteSpace(type) && ((type != TransactionType.Credit) && (type != TransactionType.Debit)))
                throw new ArgumentException("Invalid transaction filtering type: please use credit or debit to filter transaction");
                
            var accountTransactionsOptionsRequest = new AccountTransactionsOptionsRequest
            {
                Start = start,
                End = end,
                Narration = narration, 
                Type = type,
                Paginate = paginate,
                Limit = limit
            };
            var queryString = accountTransactionsOptionsRequest.PathWithQuery("transactions");
            var response = await _apiClient.GetHttpAsync<TransactionsResponse>($"accounts/{accountId}/{queryString}",
                    cancellationToken);
            return response.ToApiResponse();
        }

        public async Task<ApiResponse<IncomeResponse>> GetIncome(string accountId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(accountId)) throw new ArgumentNullException(nameof(accountId));
            var response = await _apiClient.GetHttpAsync<IncomeResponse>($"accounts/{accountId}/income", cancellationToken);
            return response.ToApiResponse();
        }

        public async Task<ApiResponse<IdentityResponse>> GetUserIdentity(string accountId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(accountId)) throw new ArgumentNullException(nameof(accountId));
            var response = await _apiClient.GetHttpAsync<IdentityResponse>($"accounts/{accountId}/identity", cancellationToken);
            return response.ToApiResponse();
        }
    }
}

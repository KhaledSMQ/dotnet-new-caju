﻿namespace Full_Solution.Application.UseCases.Withdraw
{
    using System.Threading.Tasks;
    using Full_Solution.Application.Outputs;
    using Full_Solution.Domain.ValueObjects;
    using Full_Solution.Application.Repositories;
    using Full_Solution.Domain.Accounts;

    public class WithdrawInteractor : IInputBoundary<WithdrawInput>
    {
        private readonly IAccountReadOnlyRepository accountReadOnlyRepository;
        private readonly IAccountWriteOnlyRepository accountWriteOnlyRepository;
        private readonly IOutputBoundary<WithdrawOutput> outputBoundary;
        private readonly IOutputConverter outputConverter;
        
        public WithdrawInteractor(
            IAccountReadOnlyRepository accountReadOnlyRepository,
            IAccountWriteOnlyRepository accountWriteOnlyRepository,
            IOutputBoundary<WithdrawOutput> outputBoundary,
            IOutputConverter outputConverter)
        {
            this.accountReadOnlyRepository = accountReadOnlyRepository;
            this.accountWriteOnlyRepository = accountWriteOnlyRepository;
            this.outputBoundary = outputBoundary;
            this.outputConverter = outputConverter;
        }

        public async Task Process(WithdrawInput input)
        {
            Account account = await accountReadOnlyRepository.Get(input.AccountId);
            if (account == null)
                throw new AccountNotFoundException($"The account {input.AccountId} does not exists or is already closed.");

            Debit debit = new Debit(new Amount(input.Amount));
            account.Withdraw(debit);

            await accountWriteOnlyRepository.Update(account);

            TransactionOutput transactionOutput = outputConverter.Map<TransactionOutput>(debit);
            WithdrawOutput output = new WithdrawOutput(
                transactionOutput,
                account.GetCurrentBalance().Value
            );

            outputBoundary.Populate(output);
        }
    }
}
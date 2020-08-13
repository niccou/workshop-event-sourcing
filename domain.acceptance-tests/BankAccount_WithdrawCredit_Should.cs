﻿using System;
using domain.account;
using NFluent;
using static domain.account.BankAccount;
using Xunit;

namespace domain.acceptance_tests
{
    public class BankAccount_WithdrawCredit_Should : AbstractBankAccountTesting
    {
        [Fact]
        public void fail_withdrawing_more_credit_than_provisioned() {
            // Given
            BankAccount bankAccount = RegisterBankAccount("bankAccountId", eventStore);

            // When
            Check.ThatCode(() => bankAccount.WithdrawCredit(1)).Throws<Exception>();
            
            var events = eventStore.Load("bankAccountId");
            Check.That(events).ContainsExactly(new BankAccountRegistered("bankAccountId"));
        }

        [Fact]
        public void succeed_withdrawing_less_credit_than_provisioned() {
            // Given
            BankAccount bankAccount = RegisterBankAccount("bankAccountId", eventStore);
            bankAccount.ProvisionCredit(1);

            // When
            bankAccount.WithdrawCredit(1);

            // Then
            Check.That(eventStore.Load("bankAccountId")).ContainsExactly(new BankAccountRegistered("bankAccountId"),
                new CreditProvisioned("bankAccountId", 1, 1),
                new CreditWithdrawn("bankAccountId", 1, 0));

            Check.That(bankAccount).IsEqualTo(new BankAccount("bankAccountId", eventStore, 0, 3));
        }
    }
}
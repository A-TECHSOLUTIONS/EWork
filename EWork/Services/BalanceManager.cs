﻿using System;
using System.Threading.Tasks;
using EWork.Config;
using EWork.Data.Interfaces;
using EWork.Exceptions;
using EWork.Models;
using EWork.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace EWork.Services
{
    public class BalanceManager : IBalanceManager
    {
        private readonly IBalanceRepository _repository;
        private readonly IOptions<FreelancingPlatformConfig> _freelancingPlatformOptions;
        private readonly UserManager<User> _userManager;
        private static Balance _freelancingPlatformBalance;

        public BalanceManager(IBalanceRepository repository, 
            IOptions<FreelancingPlatformConfig> freelancingPlatformOptions,
            UserManager<User> userManager)
        {
            _repository = repository;
            _freelancingPlatformOptions = freelancingPlatformOptions;
            _userManager = userManager;
        }

        public Task<Balance> FindAsync(Predicate<Balance> predicate) => _repository.FindAsync(predicate);

        public async Task<Balance> GetFreelancingPlatformBalanceAsync()
        {
            var balanceOwner = 
                await _userManager.FindByNameAsync(_freelancingPlatformOptions.Value.BalanceOwner);

            return _freelancingPlatformBalance ?? (_freelancingPlatformBalance =
                       await FindAsync(b => b.UserId == balanceOwner.Id));
        }

        public async Task<bool> ReplenishAsync(Balance balance, decimal amount)
        {
            if (amount < 0)
                throw new ArgumentException("The amount of replenishment must be greater than 0", nameof(amount));

            if (balance is null)
                throw new ArgumentNullException(nameof(balance));

            balance.Money += amount;
            await _repository.UpdateAsync(balance);
            return true;
        }

        public async Task<bool> TransferMoneyAsync(Balance senderBalance, Balance recipientBalance, decimal amount)
        {
            if (senderBalance is null)
                throw new ArgumentNullException(nameof(senderBalance));

            if (recipientBalance is null)
                throw new ArgumentNullException(nameof(recipientBalance));

            if (amount <= 0)
                throw new ArgumentException("The amount must be greater than 0", nameof(amount));
            if (senderBalance.Money < amount)
                throw new NotEnoughMoneyException($"The user {senderBalance.User?.Name ?? "freelancing platform"} hasn't got enough money for this operation");

            senderBalance.Money -= amount;
            recipientBalance.Money += amount;
            await _repository.UpdateRangeAsync(new[] { senderBalance, recipientBalance });

            return true;
        }
    }
}

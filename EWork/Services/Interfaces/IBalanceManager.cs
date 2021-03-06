﻿using System;
using System.Threading.Tasks;
using EWork.Models;

namespace EWork.Services.Interfaces
{
    public interface IBalanceManager
    {
        Task<Balance> FindAsync(Predicate<Balance> predicate);

        Task<Balance> GetFreelancingPlatformBalanceAsync();

        Task<bool> ReplenishAsync(Balance balance, decimal amount);

        Task<bool> TransferMoneyAsync(Balance senderBalance, Balance recipientBalance, decimal amount);
    }
}

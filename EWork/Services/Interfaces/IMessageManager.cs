﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EWork.Data.Interfaces;
using EWork.Models;

namespace EWork.Services.Interfaces
{
    public interface IMessageManager : IRepository<Message>
    {
        IQueryable<Message> GetChatHistory(string username1, string username2);
    }
}
﻿using Sakan.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Domain.Interfaces
{
    public interface IMessage
    {
        Task<bool> ChatExistsAsync(int chatId);
        Task AddMessageAsync(Message message);
        Task<IEnumerable<Message>> GetMessagesByChatIdAsync(int chatId);
        Task SaveChangesAsync();
        Task<IEnumerable<UserChatSummary>> GetUserChatsAsync(string userId);
        Task<Chat> CreateChatIfNotExistsAsync(string senderId, string receiverId, int listingId);
        Task<Chat?> GetChatWitIdAsync(int chatId);//
        Task<BookingRequest?> GetLatestActiveBookingAsync(int listingId);//
        Task<BookingRequest?> GetLatestActiveBookingAsync(int listingId, string guestId);//
        Task<string?> GetGuestIdByChatId(int chatId);
        Task<BookingRequest?> GetBookingByIdAsync(int bookingId);//
        Task<string?> GetGuestIdFromChat(int chatId);
        Task<Listing?> GetListingByIdAsync(int listingId);//

    }
}

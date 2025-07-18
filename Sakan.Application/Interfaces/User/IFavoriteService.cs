﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sakan.Application.DTOs.User;

namespace Sakan.Application.Interfaces.User
{
    public interface IFavoriteService
    {
        // دالة واحدة تقوم بالحفظ والإزالة (Toggle)
        Task<bool> ToggleFavoriteAsync(string userId, int listingId);

        // جلب قائمة المفضلات للمستخدم
        Task<List<ListingSummaryDto>> GetUserFavoritesAsync(string userId);
    }
}

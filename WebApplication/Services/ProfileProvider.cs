﻿using System;
using System.Threading.Tasks;

namespace WebApplication.Services
{
    public class ProfileProvider
    {
        private readonly Lazy<Task<dynamic>> _lazyProfile;
        public string Token { private get; set; }

        public ProfileProvider(IForgeOSS forgeOss)
        {
            _lazyProfile = new Lazy<Task<dynamic>>(async () => await forgeOss.GetProfileAsync(Token));
        }

        public bool IsAuthenticated => !string.IsNullOrEmpty(Token);

        public Task<dynamic> GetProfileAsync() => _lazyProfile.Value;
    }
}

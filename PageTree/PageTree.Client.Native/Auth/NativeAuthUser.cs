﻿using Common.Infrastructure.MauiMsalAuth;
using PageTree.Client.Shared.Services;

namespace PageTree.Client.Native.Auth
{
    internal class NativeAuthUser : IAuthUser
    {
        private readonly ISignInManager _signInManager;

        public bool IsSignedIn => _signInManager.IsSignedIn;

        public string Name => string.Empty;

        public event Action<bool> OnAuthenticatedStateChanged {
            add {
                _signInManager.OnAuthenticatedStateChanged += value;
            }
            remove {
                _signInManager.OnAuthenticatedStateChanged -= value;
            }
        }

        public NativeAuthUser(ISignInManager signInManager)
        {
            _signInManager = signInManager;
        }

        public Task SignIn() => _signInManager.SignIn();
        public Task SignOut() => _signInManager.SignOut();
    }
}
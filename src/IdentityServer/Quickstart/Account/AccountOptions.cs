﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Hypomos.IdentityServer.Quickstart.Account
{
    using System;

    using Microsoft.AspNetCore.Server.IISIntegration;

    public class AccountOptions
    {
        // specify the Windows authentication scheme being used
        public static readonly string WindowsAuthenticationSchemeName = IISDefaults.AuthenticationScheme;

        public static bool AllowLocalLogin = true;

        public static bool AllowRememberLogin = true;

        public static bool AutomaticRedirectAfterSignOut = false;

        // if user uses windows auth, should we load the groups from windows
        public static bool IncludeWindowsGroups = false;

        public static string InvalidCredentialsErrorMessage = "Invalid username or password";

        public static TimeSpan RememberMeLoginDuration = TimeSpan.FromDays(30);

        public static bool ShowLogoutPrompt = true;
    }
}
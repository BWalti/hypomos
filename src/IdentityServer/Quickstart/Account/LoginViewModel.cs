// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Hypomos.IdentityServer.Quickstart.Account
{
    using System.Collections.Generic;
    using System.Linq;

    public class LoginViewModel : LoginInputModel
    {
        public bool AllowRememberLogin { get; set; }

        public bool EnableLocalLogin { get; set; }

        public string ExternalLoginScheme =>
            this.IsExternalLoginOnly ? this.ExternalProviders?.SingleOrDefault()?.AuthenticationScheme : null;

        public IEnumerable<ExternalProvider> ExternalProviders { get; set; }

        public bool IsExternalLoginOnly => this.EnableLocalLogin == false && this.ExternalProviders?.Count() == 1;

        public IEnumerable<ExternalProvider> VisibleExternalProviders =>
            this.ExternalProviders.Where(x => !string.IsNullOrWhiteSpace(x.DisplayName));
    }
}
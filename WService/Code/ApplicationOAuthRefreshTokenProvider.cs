using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WService.Code
{
    internal class ApplicationOAuthRefreshTokenProvider : AuthenticationTokenProvider
    {
        public override void Create(AuthenticationTokenCreateContext context)
        {
            var form = context.Request.ReadFormAsync().Result;
            var grantType = form.GetValues("grant_type");

            if (grantType[0] != "refresh_token")
            {
                // 35 days.
                int expire = 35 * 24 * 60 * 60;
                context.Ticket.Properties.ExpiresUtc = new DateTimeOffset(DateTime.Now.AddSeconds(expire));
                context.SetToken(context.SerializeTicket());
            }
            base.Create(context);
        }

        public override void Receive(AuthenticationTokenReceiveContext context)
        {
            context.DeserializeTicket(context.Token);
            base.Receive(context);
        }

    }

}
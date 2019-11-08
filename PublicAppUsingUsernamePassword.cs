
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace up_console
{ 
    public class PublicAppUsingUsernamePassword
    { 
        public PublicAppUsingUsernamePassword(IPublicClientApplication app)
        {
            App = app;
        }
        protected IPublicClientApplication App { get; private set; }
 
        public async Task<AuthenticationResult> AcquireATokenFromCacheOrUsernamePasswordAsync(IEnumerable<String> scopes, string username, SecureString password)
        {
            Console.WriteLine(username + " AcquireATokenFromCacheOrUsernamePasswordAsync");
            AuthenticationResult result = null;
            var accounts = await App.GetAccountsAsync(); 

            if (accounts.Any())
            { 
                try
                { 
                    result = await (App as PublicClientApplication).AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                        .ExecuteAsync();
                }
                catch (MsalUiRequiredException ex)
                {
                    Console.WriteLine(ex); 
                }
            } 
 
            if (result == null)
            { 
                result = await GetTokenForWebApiUsingUsernamePasswordAsync(scopes, username, password);
            }
 
            return result;
        } 
        private async Task<AuthenticationResult> GetTokenForWebApiUsingUsernamePasswordAsync(IEnumerable<string> scopes, string username, SecureString password)
        {
            AuthenticationResult result = null;
            try
            { 
                result = await App.AcquireTokenByUsernamePassword(scopes, username, password)
                    .ExecuteAsync(); 
            Console.WriteLine(result.AccessToken);
            }
            catch (MsalUiRequiredException ex)
            {
                Console.WriteLine(ex.Message); 
            }
            catch (MsalServiceException ex) when (ex.ErrorCode == "invalid_request")
            {
                Console.WriteLine(ex.Message); 
            }
            catch (MsalServiceException ex) when (ex.ErrorCode == "unauthorized_client")
            {
                Console.WriteLine(ex.Message); 
            }
            catch (MsalServiceException ex) when (ex.ErrorCode == "invalid_client")
            {
                Console.WriteLine(ex.Message); 
            }  
            catch (MsalClientException ex) when (ex.ErrorCode == "unknown_user_type")
            {
                Console.WriteLine(ex.Message); 
                throw new ArgumentException("U/P: Wrong username", ex);
            }
            catch (MsalClientException ex) when (ex.ErrorCode == "user_realm_discovery_failed")
            {
                Console.WriteLine(ex.Message); 
                throw new ArgumentException("U/P: Wrong username", ex);
            }
            catch (MsalClientException ex) when (ex.ErrorCode == "unknown_user")
            {
                Console.WriteLine(ex.Message); 
                throw new ArgumentException("U/P: Wrong username", ex);
            }
            catch (MsalClientException ex) when (ex.ErrorCode == "parsing_wstrust_response_failed")
            {
                Console.WriteLine(ex.Message); 
            }
            return result;
        }
    }
}

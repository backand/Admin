using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.UI.Helpers
{
    public class OnlinePayments
    {
       

        public class PayPalConfig
        {
            public string clientId { get; set; }
            public string clientSecret { get; set; }
            public string cancel_url { get; set; }
            public string return_url { get; set; }

            
        }

        public class PayPal
        {
            protected Map map = null;
            public Map Map
            {
                get
                {
                    if (map == null)
                        map = Maps.Instance.GetMap();
                    map.OpenSshSession();
                    return map;
                }
            }

            private PayPalConfig payPalConfig = null;

            private PayPalConfig GetPayPalConfig()
            {
                if (IsDebug)
                {
                    return new PayPalConfig() { clientId = "AQkquBDf1zctJOWGKWUEtKXm6qVhueUEMvXO_-MCI4DQQ4-LWvkDLIN2fGsd", clientSecret = "EL1tVxAjhT7cJimnz5-Nsx9k2reTKSVfErNQF-CmrwJgxRtylkGTKlU4RvrX", cancel_url = "https://devtools-paypal.com/guide/pay_paypal/dotnet?cancel=true", return_url = "https://devtools-paypal.com/guide/pay_paypal/dotnet?success=true" };
                }
                return null;

            }
            
            private PayPalConfig PayPalConfig
            {
                get
                {
                    if (payPalConfig == null)
                        payPalConfig = GetPayPalConfig();

                    return payPalConfig;
                }
            }


            private bool IsDebug { get { return Maps.Debug; } }
            private Dictionary<string, string> GetSdkConfig()
            {
                return GetSdkConfig(IsDebug);
            }

            private Dictionary<string, string> GetSdkConfig(bool debug)
            {
                if (debug)
                {
                    Dictionary<string, string> sdkConfig = new Dictionary<string, string>();
                    sdkConfig.Add("mode", "sandbox");
                    return sdkConfig;
                }
                return null;
            }

            
            private static Dictionary<string, string> tokens = new Dictionary<string,string>();

            private string GetToken()
            {
                try
                {
                    return new OAuthTokenCredential(PayPalConfig.clientId, PayPalConfig.clientSecret, GetSdkConfig()).GetAccessToken();
                    
                }
                catch (Exception exception)
                {
                    throw new DuradosException("Failed to get access token to paypal", exception);
                }
            }

            

            public void Create(string currency, string total)
            {
                try
                {
                    string token = GetToken();
                    string accessToken = "Bearer " + token;
                    APIContext apiContext = new APIContext(accessToken);
                    apiContext.Config = GetSdkConfig();

                    Amount amnt = new Amount();
                    amnt.currency = currency;
                    amnt.total = total;

                    List<Transaction> transactionList = new List<Transaction>();
                    Transaction tran = new Transaction();
                    tran.description = "creating a payment";
                    tran.amount = amnt;
                    transactionList.Add(tran);

                    Payer payr = new Payer();
                    payr.payment_method = "paypal";

                    RedirectUrls redirUrls = new RedirectUrls();
                    redirUrls.cancel_url = PayPalConfig.cancel_url;
                    redirUrls.return_url = PayPalConfig.return_url;

                    Payment pymnt = new Payment();
                    pymnt.intent = "sale";
                    pymnt.payer = payr;
                    pymnt.transactions = transactionList;
                    pymnt.redirect_urls = redirUrls;

                    Payment createdPayment = pymnt.Create(apiContext);
                    tokens.Add(createdPayment.id, token);
                    InsertIntoPaymentLog(createdPayment);
                }
                catch (Exception exception)
                {
                    throw new DuradosException("Failed to create a paypal payment", exception);
                }
            }

            private void InsertIntoPaymentLog(Payment payment)
            {
                try
                {
                    
                }
                catch (Exception exception)
                {
                    throw new DuradosException("Failed to write to log a paypal payment", exception);
                }
            }

            private void UpdatePaymentLog(Payment payment)
            {
                try
                {

                }
                catch (Exception exception)
                {
                    throw new DuradosException("Failed to write to log a paypal payment", exception);
                }
            }

            public void Execute(string paymentId, string payerId)
            {
                try
                {
                    if (!tokens.ContainsKey(paymentId))
                    {
                        throw new DuradosException("There is no token in cache");
                    }
                    string token = tokens[paymentId];
                    string accessToken = "Bearer " + token;
                    APIContext apiContext = new APIContext(accessToken);
                    apiContext.Config = GetSdkConfig();

                    Payment payment = new Payment();
                    payment.id = paymentId;

                    PaymentExecution paymentExecution = new PaymentExecution();
                    paymentExecution.payer_id = payerId;
                    Payment executedPayment = payment.Execute(apiContext, paymentExecution);
                    tokens.Remove(paymentId);

                    UpdatePaymentLog(executedPayment);
                }
                catch (Exception exception)
                {
                    throw new DuradosException("Failed to execute a paypal payment", exception);
                }
            }
        }
    }
}

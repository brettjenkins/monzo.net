﻿namespace Monzo.Tests.MonzoClientTests
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Monzo.Tests.Fakes;
    using NUnit.Framework;

    [TestFixture]
    public sealed class Accounts
    {
        [Test]
        public async Task CanGetAccounts()
        {
            var (httpClient, fakeMessageHandler) = FakeHttpClientFactory.Create(HttpStatusCode.OK,
                @"{
                    'accounts': [
                        {
                            'id': 'acc_00009237aqC8c5umZmrRdh',
                            'description': 'Peter Pan\'s Account',
                            'created': '2015-11-13T12:17:42Z',
                            'type': 'uk_retail',
                            'currency': 'GBP',
                            'country_code': 'GB',
                            'sort_code': '040004',
                            'account_number': '12345678',
                            'closed': false,
                            'owners': [
                                {
                                    'user_id': 'user_00009awdawdawdawdg',
                                    'preferred_name': 'Peter Pan',
                                    'preferred_first_name': 'Peter'
                                }
                            ]
                        },
                        {
                            'id': 'acc_00009238aqC8c5umZmrRdh',
                            'description': 'Joint account between user_00009awdawdawdawdr and user_00009awdawdawdawdg',
                            'created': '2019-10-27T19:50:42Z',
                            'type': 'uk_retail_joint',
                            'currency': 'GBP',
                            'country_code': 'GB',
                            'sort_code': '040004',
                            'account_number': '567891234',
                            'closed': true,
                            'owners': [
                                {
                                    'user_id': 'user_00009awdawdawdawdr',
                                    'preferred_name': 'Captain Hook',
                                    'preferred_first_name': 'Hook'
                                },
                                {
                                    'user_id': 'user_00009awdawdawdawdg',
                                    'preferred_name': 'Peter Pan',
                                    'preferred_first_name': 'Peter'
                                }
                            ]
                        },
						{
                            'id': 'acc_00009abcdefghijklmnopq',
                            'closed': false,
                            'created': '2019-10-27T19:50:42Z',
                            'description': 'loan_00009abcdefghijklmnopq',
                            'type': 'uk_loan',
                            'currency': 'GBP',
                            'country_code': 'GB',
                            'owners': [
                                {
                                    'user_id': 'user_00009awdawdawdawdg',
                                    'preferred_name': 'Peter Pan',
                                    'preferred_first_name': 'Peter'
                                }
                            ]
                        }
                    ]
                }");

            using (var client = new MonzoClient(httpClient, "testAccessToken"))
            {
                var accounts = await client.GetAccountsAsync();

                Assert.AreEqual(3, accounts.Count);
                var singleAccount = accounts[0];
                #region Single Account
                Assert.AreEqual("acc_00009237aqC8c5umZmrRdh", singleAccount.Id);
                Assert.AreEqual("Peter Pan's Account", singleAccount.Description);
                Assert.AreEqual(AccountType.uk_retail, singleAccount.Type);
                Assert.AreEqual("040004", singleAccount.SortCode);
                Assert.AreEqual("12345678", singleAccount.AccountNumber);
                Assert.AreEqual(false, singleAccount.Closed);
                Assert.AreEqual(1, singleAccount.Owners.Length);

                Assert.AreEqual("user_00009awdawdawdawdg", singleAccount.Owners[0].Id);
                Assert.AreEqual("Peter Pan", singleAccount.Owners[0].PreferredName);
                Assert.AreEqual("Peter", singleAccount.Owners[0].PreferredFirstName);

                Assert.AreEqual(new DateTime(2015, 11, 13, 12, 17, 42, DateTimeKind.Utc), singleAccount.Created);
                #endregion

                var jointAccount = accounts[1];
                #region Joint Account
                Assert.AreEqual("acc_00009238aqC8c5umZmrRdh", jointAccount.Id);
                Assert.AreEqual("Joint account between user_00009awdawdawdawdr and user_00009awdawdawdawdg", jointAccount.Description);
                Assert.AreEqual(AccountType.uk_retail_joint, jointAccount.Type);
                Assert.AreEqual("040004", jointAccount.SortCode);
                Assert.AreEqual("567891234", jointAccount.AccountNumber);
                Assert.AreEqual(true, jointAccount.Closed);
                Assert.AreEqual(2, jointAccount.Owners.Length);
                Assert.AreEqual("user_00009awdawdawdawdr", jointAccount.Owners[0].Id);
                Assert.AreEqual("Captain Hook", jointAccount.Owners[0].PreferredName);
                Assert.AreEqual("Hook", jointAccount.Owners[0].PreferredFirstName);
                Assert.AreEqual("user_00009awdawdawdawdg", jointAccount.Owners[1].Id);
                Assert.AreEqual("Peter Pan", jointAccount.Owners[1].PreferredName);
                Assert.AreEqual("Peter", jointAccount.Owners[1].PreferredFirstName);

                Assert.AreEqual(new DateTime(2019, 10, 27, 19, 50, 42, DateTimeKind.Utc), jointAccount.Created);
                #endregion

                var loan = accounts[2];

                #region Loan
                Assert.AreEqual("acc_00009abcdefghijklmnopq", loan.Id);
                Assert.AreEqual(false, loan.Closed);
                Assert.AreEqual("loan_00009abcdefghijklmnopq", loan.Description);
                Assert.AreEqual(AccountType.uk_loan, loan.Type);
                Assert.AreEqual("GBP", loan.Currency);
                Assert.AreEqual("GB", loan.CountryCode);
                Assert.AreEqual("user_00009awdawdawdawdg", loan.Owners[0].Id);
                Assert.AreEqual("Peter Pan", loan.Owners[0].PreferredName);
                Assert.AreEqual("Peter", loan.Owners[0].PreferredFirstName);

                Assert.AreEqual(new DateTime(2019, 10, 27, 19, 50, 42, DateTimeKind.Utc), loan.Created);
                #endregion
            }

            Assert.AreEqual("/accounts", fakeMessageHandler.Request.RequestUri.PathAndQuery);

            Assert.AreEqual("Bearer testAccessToken", fakeMessageHandler.Request.Headers.Authorization.ToString());
        }
    }
}

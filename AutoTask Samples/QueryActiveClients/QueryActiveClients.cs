//The MIT License (MIT)

//Copyright (c) <2013> <Jared L Jennings jared@jaredjennings.org>

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTask_Samples.StubAutoTask;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace AutoTask_Samples
{
        /// <summary>
        /// Demonstrates how to retrieve all active clients
        /// </summary>
        class QueryActiveClients
        {
                // the userID and password used to authenticate to AutoTask
                private static string auth_user_id = ""; // user@domain.com
                private static string auth_user_password = "";
                private static ATWSZoneInfo zoneInfo = null;

                public static void Main ()
                {

                        // demonstrates how to intellegently get the zone information.
                        // autotask will tell you which zone URL to use based on the userID
                        var client = new ATWSSoapClient ();
                        zoneInfo = client.getZoneInfo (auth_user_id);
                        Console.WriteLine ("ATWS Zone Info: \n\n"
                                + "URL = " + zoneInfo.URL);

                        // Create the binding.
                        // must use BasicHttpBinding instead of WSHttpBinding
                        // otherwise a "SOAP header Action was not understood." is thrown.
                        BasicHttpBinding myBinding = new BasicHttpBinding ();
                        myBinding.Security.Mode = BasicHttpSecurityMode.Transport;
                        myBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

                        // Must set the size otherwise
                        //The maximum message size quota for incoming messages (65536) has been exceeded. To increase the quota, use the MaxReceivedMessageSize property on the appropriate binding element.
                        myBinding.MaxReceivedMessageSize = 2147483647;

                        // Create the endpoint address.
                        EndpointAddress ea = new EndpointAddress (zoneInfo.URL);

                        client = new ATWSSoapClient (myBinding, ea);
                        client.ClientCredentials.UserName.UserName = auth_user_id;
                        client.ClientCredentials.UserName.Password = auth_user_password;
                        
                        // query for any account. This should return all accounts since we are retreiving anything greater than 0.
                        StringBuilder sb = new StringBuilder ();
                        sb.Append ("<queryxml><entity>Account</entity>").Append (System.Environment.NewLine);
                        sb.Append ("<query><field>id<expression op=\"greaterthan\">0</expression></field></query>").Append (System.Environment.NewLine);
                        sb.Append ("</queryxml>").Append (System.Environment.NewLine);

                        // have no clue what this does.
                        AutotaskIntegrations at_integrations = new AutotaskIntegrations ();

                        // this example will not handle the 500 results limitation.
                        // Autotask only returns up to 500 results in a response. if there are more you must query again for the next 500.
                        var r = client.query (at_integrations, sb.ToString ());
                        Console.WriteLine ("response ReturnCode = " + r.ReturnCode);
                        if (r.ReturnCode == 1) {
                                if (r.EntityResults.Length > 0) {
                                        foreach (var item in r.EntityResults) {
                                                Account acct = (Account)item;
                                                Console.WriteLine ("Account Name = " + acct.AccountName);
                                                Console.WriteLine ("Account number = " + acct.AccountNumber);
                                        }
                                }
                        }
                        Console.ReadLine ();
                }

        }
}

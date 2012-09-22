﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace NKCraddock.AmazonItemLookup.Client
{
    public class AwsProductApiClient
    {
        const string AWS_SERVICE = "AWSECommerceService";
        const string AWS_VERSION = "2009-03-31";
        const string AWS_DESTINATION = "ecs.amazonaws.com";

        ProductApiConnectionInfo connectionInfo;

        public AwsProductApiClient(ProductApiConnectionInfo connectionInfo)
        {
            this.connectionInfo = connectionInfo;
        }

        public AwsItem LookupByAsin(string asin)
        {
            var args = GetRequestArguments(AwsProductApiOperation.ItemLookup);
            args["ItemId"] = asin;

            string requestUrl = SignRequest(args);

            string responseText = GetResponse(requestUrl);

            var itemLookupResponse = new AwsItemLookupResponse(responseText);

            return itemLookupResponse.ToAwsItem();
        }

        private Dictionary<string, string> GetRequestArguments(string operation)
        {
            var requestArgs = new Dictionary<string, String>();
            requestArgs["Service"] = AWS_SERVICE;
            requestArgs["Version"] = AWS_VERSION;
            requestArgs["Operation"] = operation;
            requestArgs["ResponseGroup"] = "Large";
            requestArgs["AssociateTag"] = connectionInfo.AWSAssociateTag;
            return requestArgs;
        }

        private string SignRequest(Dictionary<string, String> requestArgs)
        {
            var signer = new SignedRequestHelper(connectionInfo.AWSAccessKey, connectionInfo.AWSSecretKey, AWS_DESTINATION);
            return signer.Sign(requestArgs);
        }

        private string GetResponse(string requestUrl)
        {
            var request = WebRequest.Create(requestUrl);
            var response = (HttpWebResponse)request.GetResponse();
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                return sr.ReadToEnd();
            }
        }
    }
}
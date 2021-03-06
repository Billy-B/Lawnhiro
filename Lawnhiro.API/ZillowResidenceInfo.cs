﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using System.Xml;

namespace Lawnhiro.API
{
    public class ZillowResidenceInfo
    {
        public int ZillowId { get; private set; }
        public decimal? LotSizeSqFt { get; private set; }
        public decimal? FinishedSqFt { get; private set; }
        public decimal? NumberOfFloors { get; private set; }

        const string ZWS_ID = "X1-ZWz1fafu2i84y3_1hdll";

        public static ZillowResidenceInfo GetResidenceInfo(string address, string city, string state, string zip)
        {
            XmlDocument searchResults = new XmlDocument();
            string addressParam = HttpUtility.UrlEncode(address);
            string cityStateZipParam = HttpUtility.UrlEncode($"{city}, {state} {zip}");
            searchResults.Load($"http://www.zillow.com/webservice/GetDeepSearchResults.htm?zws-id={ZWS_ID}&address={addressParam}&citystatezip={cityStateZipParam}");
            XmlNode resultsNode = searchResults.DocumentElement.SelectSingleNode("response/results/result");
            if (resultsNode == null) // no zillow info found
            {
                return null;
            }
            else
            {
                XmlElement lotSqFtNode = resultsNode["lotSizeSqFt"];
                decimal? lotSqFt, finishedSqFt;
                if (lotSqFtNode == null)
                {
                    lotSqFt = null;
                }
                else
                {
                    lotSqFt = decimal.Parse(lotSqFtNode.InnerText);
                }
                XmlElement finishedSqFtNode = resultsNode["finishedSqFt"];
                if (finishedSqFtNode == null)
                {
                    finishedSqFt = null;
                }
                else
                {
                    finishedSqFt = decimal.Parse(finishedSqFtNode.InnerText);
                }
                int zpid = int.Parse(resultsNode["zpid"].InnerText);
                XmlDocument updatedPropertyDetails = new XmlDocument();
                updatedPropertyDetails.Load($"http://www.zillow.com/webservice/GetUpdatedPropertyDetails.htm?zws-id={ZWS_ID}&zpid={zpid}");
                XmlNode numFloorsNode = updatedPropertyDetails.DocumentElement.SelectSingleNode("numFloors");
                decimal? numFloors;
                if (numFloorsNode == null)
                {
                    numFloors = null;
                }
                else
                {
                    numFloors = decimal.Parse(numFloorsNode.InnerText);
                }
                return new ZillowResidenceInfo
                {
                    ZillowId = zpid,
                    LotSizeSqFt = lotSqFt,
                    FinishedSqFt = finishedSqFt,
                    NumberOfFloors = numFloors
                };
            }
        }
    }
}

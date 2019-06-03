using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class STHRootobject
{
    public Query query { get; set; }
}

public class Query
{
    public STHUser[] sthuser  { get; set; }
}

public class STHUser
{
    public int Login_ID { get; set; }
    public string Name { get; set; }
}
public class EventInfo
{
    public string EventName { get; set; }
    public string EventDate { get; set; }
    public string DUE { get; set; }
    public string VenueName { get; set; }
    public string VenueCity { get; set; }
    public string Low { get; set; }
    public string High { get; set; }
    public string Description { get; set; }
    public string Wind { get; set; }
    public string Rain { get; set; }
    public string Chance { get; set; }
    public string Icons { get; set; }
    public string Rankings { get; set; }
    public Boolean Taxable { get; set; }
}
namespace EventSearch
{
    public class Venue
    {
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string webURI { get; set; }
        public string seoURI { get; set; }
        public string venueUrl { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string timezone { get; set; }
        public string jdkTimezone { get; set; }
        public string address1 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string postalCode { get; set; }
        public string country { get; set; }
        public int venueConfigId { get; set; }
    }
    public class VenueConfiguration
    {
        public int id { get; set; }
        public string name { get; set; }
    }
    public class Event
    {
        public int id { get; set; }
        public string locale { get; set; }
        public string name { get; set; }
        public string originalName { get; set; }
        public DateTime eventDateLocal { get; set; }
        public Venue venue { get; set; }
        public VenueConfiguration venueConfiguration { get; set; }
        public string sourceId { get; set; }
        public string defaultLocale { get; set; }
    }

    public class RootObject
    {
        public int numFound { get; set; }
        public List<Event> events { get; set; }
    }
}

namespace Axis.Models
{
    public class BrokerList
    {
        public int brokerId { get; set; }
        public string brokerName { get; set; }
    }

    public class ShoppingCart
    {
        public class RootObject
        {
            public int Listing_ID { get; set; }
            public int Event_ID { get; set; }
            public string Section { get; set; }
            public string Row { get; set; }
            public double Price { get; set; }
            public int Qty { get; set; }
            public bool Taxable { get; set; }
        }
    }
    public class Stubhub
    {
        public class CurrentPrice
        {
            public double amount { get; set; }
            public string currency { get; set; }
        }

        public class ListingPrice
        {
            public double amount { get; set; }
            public string currency { get; set; }
        }

        public class FaceValue
        {
            public double amount { get; set; }
            public string currency { get; set; }
        }

        public class Listing
        {
            public int listingId { get; set; }
            public CurrentPrice currentPrice { get; set; }
            public ListingPrice listingPrice { get; set; }
            public int sectionId { get; set; }
            public string row { get; set; }
            public int quantity { get; set; }
            public string sellerSectionName { get; set; }
            public string sectionName { get; set; }
            public string seatNumbers { get; set; }
            public int zoneId { get; set; }
            public string zoneName { get; set; }
            public List<int> listingAttributeList { get; set; }
            public List<int> deliveryTypeList { get; set; }
            public List<int> deliveryMethodList { get; set; }
            public bool dirtyTicketInd { get; set; }
            public string splitOption { get; set; }
            public string ticketSplit { get; set; }
            public List<int> splitVector { get; set; }
            public int sellerOwnInd { get; set; }
            public FaceValue faceValue { get; set; }
            public List<int?> listingAttributeCategoryList { get; set; }
            public bool Local { get; set; }
            public string SH_Section { get; set; }
            public int Listing_ID { get; set; }
        }

        public class Percentile
        {
            public double name { get; set; }
            public double value { get; set; }
        }

        public class PricingSummary
        {
            public string name { get; set; }
            public double minTicketPrice { get; set; }
            public double averageTicketPrice { get; set; }
            public double maxTicketPrice { get; set; }
            public int totalListings { get; set; }
            public List<Percentile> percentiles { get; set; }
        }

        public class RootObject
        {
            public int eventId { get; set; }
            public int totalListings { get; set; }
            public int totalTickets { get; set; }
            public int minQuantity { get; set; }
            public int maxQuantity { get; set; }
            public List<Listing> listing { get; set; }
            public PricingSummary pricingSummary { get; set; }
            public List<object> listingAttributeCategorySummary { get; set; }
            public List<object> deliveryTypeSummary { get; set; }
            public int start { get; set; }
            public int rows { get; set; }
        }
    }
}
namespace FullcalendarMvcApp
{
    public class EventViewModel
    {
        public Int64 id { get; set; }
        public String title { get; set; }
        public String start { get; set; }
        public String end { get; set; }
        public bool allDay { get; set; }
        public String textColor { get; set; }
        public String borderColor { get; set; }
        public String backgroundColor { get; set; }
        public String url { get; set; }
    }
}
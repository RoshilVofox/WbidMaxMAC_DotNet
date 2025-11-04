using ADT.Common.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADT.Common.Models
{
    public  class SWATrip
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class BidPeriod
        {
            [JsonProperty("name")]
            public string name { get; set; }

            [JsonProperty("department")]
            public string department { get; set; }

            [JsonProperty("start")]
            public string start { get; set; }

            [JsonProperty("end")]
            public string end { get; set; }
        }

        public class BidRoundId
        {
            [JsonProperty("year")]
            public string year { get; set; }

            [JsonProperty("schedulePeriod")]
            public string schedulePeriod { get; set; }

            [JsonProperty("roundType")]
            public string roundType { get; set; }

            [JsonProperty("value")]
            public string value { get; set; }
        }

        public class Duty
        {
            [JsonProperty("dutyPeriodNumber")]
            public int dutyPeriodNumber { get; set; }

            [JsonProperty("shortBack")]
            public bool shortBack { get; set; }

            [JsonProperty("dutyMinutes")]
            public int dutyMinutes { get; set; }

            [JsonProperty("dutyStartIATAStationCode")]
            public string dutyStartIATAStationCode { get; set; }

            [JsonProperty("dutyEndIATAStationCode")]
            public string dutyEndIATAStationCode { get; set; }

            [JsonProperty("dutyRestIATAStationCode")]
            public string dutyRestIATAStationCode { get; set; }

            [JsonProperty("reportDateTimeUTC")]
            public DateTime reportDateTimeUTC { get; set; }

            [JsonProperty("releaseDateTimeUTC")]
            public DateTime releaseDateTimeUTC { get; set; }

            [JsonProperty("redeye")]
            public bool redeye { get; set; }

            [JsonProperty("language")]
            public string language { get; set; }

            [JsonProperty("missionType")]
            public string missionType { get; set; }

            [JsonProperty("international")]
            public bool international { get; set; }

            [JsonProperty("restMinutes")]
            public int restMinutes { get; set; }

            [JsonProperty("dutyBlockLimit")]
            public int dutyBlockLimit { get; set; }

            [JsonProperty("flightDutyPeriod")]
            public int flightDutyPeriod { get; set; }

            [JsonProperty("flightDutyPeriodLimit")]
            public int flightDutyPeriodLimit { get; set; }

            [JsonProperty("totalDeadheads")]
            public int totalDeadheads { get; set; }

            [JsonProperty("daytimeCategory")]
            public string daytimeCategory { get; set; }

            [JsonProperty("dutyPay")]
            public DutyPay dutyPay { get; set; }

            [JsonProperty("totalBlockMinutes")]
            public int totalBlockMinutes { get; set; }

            [JsonProperty("totalGroundMinutes")]
            public int totalGroundMinutes { get; set; }

            [JsonProperty("totalAircraftChanges")]
            public int totalAircraftChanges { get; set; }

            [JsonProperty("flightLegs")]
            public List<FlightLeg> flightLegs { get; set; }
        }

        public class DutyPay
        {
            [JsonProperty("totalTripForPay")]
            public double totalTripForPay { get; set; }

            [JsonProperty("dutyRig")]
            public double dutyRig { get; set; }

            [JsonProperty("tripForPayType")]
            public string tripForPayType { get; set; }

            [JsonProperty("dutyCredit")]
            public double dutyCredit { get; set; }

            [JsonProperty("deadheadRig")]
            public double deadheadRig { get; set; }
        }

        public class Embedded
        {
            [JsonProperty("LinesPairings")]
            public List<LinesPairing> LinesPairings { get; set; }
        }

        public class Equipment
        {
            [JsonProperty("scheduledAircraftType")]
            public string scheduledAircraftType { get; set; }

            [JsonProperty("equipmentLegCode")]
            public string equipmentLegCode { get; set; }
        }

        public class First
        {
            [JsonProperty("href")]
            public string href { get; set; }
        }

        public class FlightLeg
        {
            [JsonProperty("flightLegKey")]
            public FlightLegKey flightLegKey { get; set; }

            [JsonProperty("legNumber")]
            public int legNumber { get; set; }

            [JsonProperty("arrivalDateTimeUTC")]
            public DateTime arrivalDateTimeUTC { get; set; }

            [JsonProperty("blockMinutes")]
            public int blockMinutes { get; set; }

            [JsonProperty("departureDateTimeUTC")]
            public DateTime departureDateTimeUTC { get; set; }

            [JsonProperty("equipment")]
            public Equipment equipment { get; set; }

            [JsonProperty("flightType")]
            public string flightType { get; set; }

            [JsonProperty("routeNumber")]
            public int routeNumber { get; set; }

            [JsonProperty("redeye")]
            public bool redeye { get; set; }

            [JsonProperty("language")]
            public string language { get; set; }

            [JsonProperty("missionType")]
            public string missionType { get; set; }

            [JsonProperty("international")]
            public bool international { get; set; }

            [JsonProperty("departureInternational")]
            public bool departureInternational { get; set; }

            [JsonProperty("arrivalInternational")]
            public bool arrivalInternational { get; set; }

            [JsonProperty("conus")]
            public bool conus { get; set; }

            [JsonProperty("departureConus")]
            public bool departureConus { get; set; }

            [JsonProperty("arrivalConus")]
            public bool arrivalConus { get; set; }

            [JsonProperty("deadhead")]
            public bool deadhead { get; set; }

            [JsonProperty("originator")]
            public bool originator { get; set; }

            [JsonProperty("terminator")]
            public bool terminator { get; set; }

            [JsonProperty("groundMinutes")]
            public int groundMinutes { get; set; }

            [JsonProperty("aircraftChange")]
            public bool aircraftChange { get; set; }

            [JsonProperty("tripForPay")]
            public double tripForPay { get; set; }

            [JsonProperty("tripForPayType")]
            public string tripForPayType { get; set; }

            [JsonProperty("snackType")]
            public string snackType { get; set; }

            [JsonProperty("hotel")]
            public bool hotel { get; set; }
        }

        public class FlightLegKey
        {
            [JsonProperty("operationalCarrier")]
            public string operationalCarrier { get; set; }

            [JsonProperty("flightNumber")]
            public string flightNumber { get; set; }

            [JsonProperty("originDate")]
            public DateTime originDate { get; set; }

            [JsonProperty("departureAirportIATACode")]
            public string departureAirportIATACode { get; set; }

            [JsonProperty("arrivalAirportIATACode")]
            public string arrivalAirportIATACode { get; set; }
        }

        public class Last
        {
            [JsonProperty("href")]
            public string href { get; set; }
        }

        public class LinesPairing
        {
            [JsonProperty("bidRoundId")]
            public BidRoundId bidRoundId { get; set; }

            [JsonProperty("publishedVersion")]
            public int publishedVersion { get; set; }

            [JsonProperty("pairingKey")]
            public PairingKey pairingKey { get; set; }

            [JsonProperty("bidRoundType")]
            public string bidRoundType { get; set; }

            [JsonProperty("crewComplement")]
            public string crewComplement { get; set; }

            [JsonProperty("seatPositions")]
            public List<string> seatPositions { get; set; }

            [JsonProperty("bidPeriod")]
            public BidPeriod bidPeriod { get; set; }

            [JsonProperty("pairingBaseIATAStationCode")]
            public string pairingBaseIATAStationCode { get; set; }

            [JsonProperty("fleet")]
            public string fleet { get; set; }

            [JsonProperty("pairingType")]
            public string pairingType { get; set; }

            [JsonProperty("pairingCredit")]
            public double pairingCredit { get; set; }

            [JsonProperty("pairingLength")]
            public int pairingLength { get; set; }

            [JsonProperty("reportDateTimeUTC")]
            public DateTime reportDateTimeUTC { get; set; }

            [JsonProperty("releaseDateTimeUTC")]
            public DateTime releaseDateTimeUTC { get; set; }

            [JsonProperty("daytimeCategory")]
            public string daytimeCategory { get; set; }

            [JsonProperty("redeye")]
            public bool redeye { get; set; }

            [JsonProperty("language")]
            public string language { get; set; }

            [JsonProperty("missionType")]
            public string missionType { get; set; }

            [JsonProperty("international")]
            public bool international { get; set; }

            [JsonProperty("reserveInfo")]
            public object reserveInfo { get; set; }

            [JsonProperty("operationalInfo")]
            public OperationalInfo operationalInfo { get; set; }

            [JsonProperty("tripPay")]
            public TripPay tripPay { get; set; }

            [JsonProperty("duties")]
            public List<Duty> duties { get; set; }

            [JsonProperty("_links")]
            public Links _links { get; set; }
        }

        public class Links
        {
            [JsonProperty("self")]
            public Self self { get; set; }

            [JsonProperty("up")]
            public Up up { get; set; }

            [JsonProperty("first")]
            public First first { get; set; }

            [JsonProperty("next")]
            public Next next { get; set; }

            [JsonProperty("last")]
            public Last last { get; set; }

            [JsonProperty("search")]
            public Search search { get; set; }
        }

        public class Next
        {
            [JsonProperty("href")]
            public string href { get; set; }
        }

        public class OperationalInfo
        {
            [JsonProperty("timeAwayFromBase")]
            public int timeAwayFromBase { get; set; }

            [JsonProperty("totalDutyMinutes")]
            public int totalDutyMinutes { get; set; }

            [JsonProperty("numberOfDuties")]
            public int numberOfDuties { get; set; }

            [JsonProperty("totalDeadheads")]
            public int totalDeadheads { get; set; }

            [JsonProperty("restIATAStationCode")]
            public List<string> restIATAStationCode { get; set; }

            [JsonProperty("totalBlockMinutes")]
            public int totalBlockMinutes { get; set; }

            [JsonProperty("totalGroundMinutes")]
            public int totalGroundMinutes { get; set; }

            [JsonProperty("totalAircraftChanges")]
            public int totalAircraftChanges { get; set; }

            [JsonProperty("totalFlightDutyPeriod")]
            public int totalFlightDutyPeriod { get; set; }
        }

        public class Page
        {
            [JsonProperty("size")]
            public int size { get; set; }

            [JsonProperty("totalElements")]
            public int totalElements { get; set; }

            [JsonProperty("totalPages")]
            public int totalPages { get; set; }

            [JsonProperty("number")]
            public int number { get; set; }
        }

        public class PairingKey
        {
            [JsonProperty("pairingNumber")]
            public string pairingNumber { get; set; }

            [JsonProperty("department")]
            public string department { get; set; }

            [JsonProperty("pairingDate")]
            public string pairingDate { get; set; }
        }

        public class Root:IPaginatedResponse
        {
            [JsonProperty("_embedded")]
            public Embedded _embedded { get; set; }

            [JsonProperty("_links")]
            public Links _links { get; set; }

            [JsonProperty("page")]
            public Page page { get; set; }

            public bool HasNextPage => _links?.next != null;

            public object Page => page;
        }

        public class Search
        {
            [JsonProperty("href")]
            public string href { get; set; }

            [JsonProperty("templated")]
            public bool templated { get; set; }
        }

        public class Self
        {
            [JsonProperty("href")]
            public string href { get; set; }

            [JsonProperty("name")]
            public string name { get; set; }
        }

        public class TripPay
        {
            [JsonProperty("totalRig")]
            public double totalRig { get; set; }

            [JsonProperty("pairingRig")]
            public double pairingRig { get; set; }

            [JsonProperty("tripForPayType")]
            public string tripForPayType { get; set; }

            [JsonProperty("totalDutyRig")]
            public double totalDutyRig { get; set; }

            [JsonProperty("totalTripForPay")]
            public double totalTripForPay { get; set; }

            [JsonProperty("totalDutyHourRatioRig")]
            public double totalDutyHourRatioRig { get; set; }

            [JsonProperty("totalDutyPeriodMinimumRig")]
            public double totalDutyPeriodMinimumRig { get; set; }
        }

        public class Up
        {
            [JsonProperty("href")]
            public string href { get; set; }

            [JsonProperty("name")]
            public string name { get; set; }
        }


    }
}

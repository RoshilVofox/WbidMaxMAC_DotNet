using ADT.Common.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Core.Enum;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary.Utility;

namespace ADT.Engine.Mapper
{
    public class SwaApiMapper
    {
        public SwaApiMapper() { }

        public Dictionary<string, Trip> MapSWaTriptoWBidTrip(List<SWATrip.LinesPairing> swatripData,Dictionary<string,Line> lines)
        {
            try
            {
                List<Duplicate> lstdup = new List<Duplicate>();
                Dictionary<string, Trip> tripList = new Dictionary<string, Trip>();
                Dictionary<string, Trip> duptripList = new Dictionary<string, Trip>();
                foreach (var swatrip in swatripData)
                {
                    try
                    {
                        Trip Wbtrip = new Trip();

                        var splitdate = swatrip.pairingKey.pairingDate.Split('-');
                        string tripdate = splitdate.LastOrDefault();
                        DateTime startDate = new DateTime(Convert.ToInt16(splitdate[0]), Convert.ToInt16(splitdate[1]), Convert.ToInt16(splitdate[2]));
                        Wbtrip.TripNum = swatrip.pairingKey.pairingNumber + tripdate;
                        Wbtrip.PairLength = swatrip.pairingLength;
                        //DateTime tripShowTime= Helper.ConvertToHerbFromUTC(swatrip.reportDateTimeUTC);
                        //DateTime tripReleaseTime = Helper.ConvertToHerbFromUTC(swatrip.releaseDateTimeUTC);
                        Wbtrip.Tafb = swatrip.operationalInfo.timeAwayFromBase;
                        var exist = tripList.Any(x => x.Key == Wbtrip.TripNum);

                        if (swatrip.pairingType != "RESERVE")
                        {

                            foreach (var swadp in swatrip.duties)
                            {
                                DutyPeriod dutyPeriod = new DutyPeriod();
                                dutyPeriod.DutPerSeqNum = swadp.dutyPeriodNumber;
                                dutyPeriod.Tfp = Convert.ToDecimal(swadp.dutyPay.dutyCredit);
                                dutyPeriod.TripNum = swatrip.pairingKey.pairingNumber;
                                DateTime showTime = Helper.ConvertToHerbFromUTC(swadp.reportDateTimeUTC);
                                dutyPeriod.ShowTime = ((showTime - startDate).Days * 1440) + showTime.Hour * 60 + showTime.Minute;
                                DateTime releaseTime = Helper.ConvertToHerbFromUTC(swadp.releaseDateTimeUTC);
                                dutyPeriod.ReleaseTime = ((releaseTime - startDate).Days * 1440) + releaseTime.Hour * 60 + releaseTime.Minute;
                                //int firstFlightDay = 0;
                                //bool firstFlight = true;
                                foreach (var swaflt in swadp.flightLegs)
                                {
                                    Flight flt = new Flight();
                                    flt.FltNum = Convert.ToInt32(swaflt.flightLegKey.flightNumber);
                                    flt.DepSta = swaflt.flightLegKey.departureAirportIATACode;
                                    flt.ArrSta = swaflt.flightLegKey.arrivalAirportIATACode;
                                    //flt.ArrTime = ((dutyPeriod.DutPerSeqNum - 1) * 1440) + (swaflt.arrivalDateTimeUTC.Hour * 60) + swaflt.arrivalDateTimeUTC.Minute;
                                    // flt.DepTime = ((dutyPeriod.DutPerSeqNum - 1) * 1440) + (swaflt.departureDateTimeUTC.Hour * 60) + swaflt.departureDateTimeUTC.Minute;
                                    DateTime arrivalTimeInHerb = Helper.ConvertToHerbFromUTC(swaflt.arrivalDateTimeUTC);
                                    DateTime departureTimeInHerb = Helper.ConvertToHerbFromUTC(swaflt.departureDateTimeUTC);
                                    flt.ArrTime = ((arrivalTimeInHerb - startDate).Days * 1440) + (arrivalTimeInHerb.Hour * 60) + arrivalTimeInHerb.Minute;
                                    flt.DepTime = ((departureTimeInHerb - startDate).Days * 1440) + (departureTimeInHerb.Hour * 60) + departureTimeInHerb.Minute;
                                    if (flt.DepTime > flt.ArrTime)
                                    {
                                        flt.ArrTime += 1440;
                                    }
                                    //if (firstFlight)
                                    //{
                                    //    firstFlightDay = swaflt.departureDateTimeUTC.Day;
                                    //    firstFlight = false;
                                    //}
                                    //if (firstFlightDay != swaflt.departureDateTimeUTC.Day)
                                    //{
                                    //    flt.ArrTime += 1440;
                                    //    flt.DepTime += 1440;
                                    //}

                                    flt.AcftChange = swaflt.aircraftChange;
                                    flt.Tfp = Convert.ToDecimal(swaflt.tripForPay);
                                    flt.DeadHead = swaflt.deadhead;
                                    flt.RedEye = swaflt.redeye;
                                    flt.FlightSeqNum = swaflt.legNumber;
                                    flt.ETOPS = (swaflt.missionType == "ET") ? true : false;
                                    flt.Equip = swaflt.equipment.equipmentLegCode;
                                    dutyPeriod.Flights.Add(flt);
                                }
                                dutyPeriod.ArrStaLastLeg = dutyPeriod.Flights[dutyPeriod.Flights.Count - 1].ArrSta;
                                dutyPeriod.TotFlights = swadp.flightLegs.Count();
                                Wbtrip.DutyPeriods.Add(dutyPeriod);
                            }
                            Wbtrip.BriefTime = Convert.ToInt32((Helper.ConvertToHerbFromUTC(swatrip.duties.FirstOrDefault().flightLegs.FirstOrDefault().departureDateTimeUTC) - Helper.ConvertToHerbFromUTC(swatrip.reportDateTimeUTC)).TotalMinutes);
                            Wbtrip.DebriefTime = Convert.ToInt32((Helper.ConvertToHerbFromUTC(swatrip.releaseDateTimeUTC) - Helper.ConvertToHerbFromUTC(swatrip.duties.LastOrDefault().flightLegs.LastOrDefault().arrivalDateTimeUTC)).TotalMinutes);

                        }
                        else
                        {
                            Wbtrip.ReserveTrip = true;
                            if (swatrip.duties.Count == 0)
                            {
                                var dpDate = Convert.ToDateTime(swatrip.pairingKey.pairingDate);
                                DutyPeriod dutyPeriod = new DutyPeriod();
                                Wbtrip.DutyPeriods.Add(dutyPeriod);

                                var reservelines = lines.Where(x => x.Value.Pairings.Any(y => y == Wbtrip.TripNum));
                                int reserveMode = 0;
                                foreach (var item in reservelines)
                                {
                                    reserveMode = (int)item.Value.ReserveMode;
                                }

                                int showtime = 0;
                                int releaseTime = 0;

                                var offsets = new Dictionary<ReserveType, (int Show, int Release)>
    {
        { ReserveType.SeniorAMReserve,   (180, 660) },
        { ReserveType.SeniorPMReserve,   (600, 1080) },
        { ReserveType.JuniorAMReserve,   (180, 900) },
        { ReserveType.JuniorPMReserve,   (600, 1320) },
        { ReserveType.JuniorLateReserve, (900, 1619) }
    };

                                if (offsets.TryGetValue((ReserveType)reserveMode, out var times))
                                {
                                    showtime = Helper.ConvertToHerbFromDomTime(GlobalSettings.CurrentBidDetails.Domicile, dpDate, times.Show);
                                    releaseTime = Helper.ConvertToHerbFromDomTime(GlobalSettings.CurrentBidDetails.Domicile, dpDate, times.Release);
                                }
                                else
                                {
                                    DateTime dpshowTime = Helper.ConvertToHerbFromUTC(swatrip.reportDateTimeUTC);
                                    showtime = dpshowTime.Hour * 60 + dpshowTime.Minute;

                                    DateTime dpreleaseTime = Helper.ConvertToHerbFromUTC(swatrip.releaseDateTimeUTC);
                                    releaseTime = dpreleaseTime.Hour * 60 + dpreleaseTime.Minute;
                                }

                                dutyPeriod.ShowTime = showtime;
                                dutyPeriod.ReleaseTime = releaseTime;


                                //DateTime showTime = WBidHelper.ConvertToHerbFromUTC(swatrip.reportDateTimeUTC);
                                //dutyPeriod.ShowTime = showTime.Hour * 60 + showTime.Minute;
                                //DateTime releaseTime = WBidHelper.ConvertToHerbFromUTC(swatrip.releaseDateTimeUTC);
                                //dutyPeriod.ReleaseTime = releaseTime.Hour * 60 + releaseTime.Minute;

                                if (dutyPeriod.ShowTime > dutyPeriod.ReleaseTime)
                                    dutyPeriod.ReleaseTime = dutyPeriod.ReleaseTime + 1440;
                                // dutyPeriod.ShowTime = _dutyPeriodDate.Subtract(currentFAReserveTrip.TripStartDates[0]).Days * 1440 + (ConvertToHerbFromDomTime(_domicileName, currentFAReserveTrip.TripStartDates[0], showtime));
                                // dutyPeriod.ReleaseTime = _dutyPeriodDate.Subtract(currentFAReserveTrip.TripStartDates[0]).Days * 1440 + (ConvertToHerbFromDomTime(_domicileName, currentFAReserveTrip.TripStartDates[0], deptime));

                                dutyPeriod.Tfp = GlobalSettings.FAReserveDayPay;
                                dutyPeriod.Block = 0;


                                dutyPeriod.Flights = new List<Flight>();

                                dutyPeriod.Flights.Add(new Flight()
                                {
                                    DepSta = swatrip.pairingBaseIATAStationCode,
                                    ArrSta = swatrip.pairingBaseIATAStationCode,
                                    DepTime = dutyPeriod.ShowTime,
                                    ArrTime = dutyPeriod.ReleaseTime,
                                    Tfp = GlobalSettings.FAReserveDayPay,
                                    Block = 0,
                                    FlightSeqNum = 1


                                });

                            }


                        }
                        //Wbtrip.BriefTime = Wbtrip.DutyPeriods[0].Flights[0].DepTime - swatrip.reportDateTimeUTC.Minute;
                        // Wbtrip.DebriefTime = swatrip.reportDateTimeUTC.Minute - Wbtrip.DutyPeriods[Wbtrip.DutyPeriods.Count - 1].Flights[Wbtrip.DutyPeriods[Wbtrip.DutyPeriods.Count - 1].Flights.Count - 1].ArrTime;
                        if (!exist)
                        {
                            tripList.Add(Wbtrip.TripNum, Wbtrip);
                        }
                        else
                        {
                            var dup = lstdup.FirstOrDefault(x => x.TripName == Wbtrip.TripNum);
                            if ((dup != null))
                            {
                                dup.Count++;
                            }
                            else
                                lstdup.Add(new Duplicate { Count = 1, TripName = Wbtrip.TripNum });
                            var exist1 = duptripList.Any(x => x.Key == Wbtrip.TripNum);

                            var data = duptripList.FirstOrDefault(x => x.Key == Wbtrip.TripNum);

                            if (!exist1)
                                duptripList.Add(Wbtrip.TripNum, Wbtrip);
                            else
                            {
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                var lstordered = lstdup.OrderByDescending(x => x.Count).ToList();

                return tripList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }



        public Dictionary<string, Line> MapSWaLinetoWBidLine(List<SWALine.LinesLine> swaLineData)
        {
            Dictionary<string, Line> lineList = new Dictionary<string, Line>();
            foreach (var swLine in swaLineData)
            {
               
                Line wbline = new Line();
                wbline.LineNum = swLine.lineKey.lineNumber;
                wbline.LineType = GetLineTypeValue(swLine.lineType);
                wbline.LineDisplay = wbline.LineNum.ToString();
                if (swLine.language == "ES")
                {
                    wbline.LineType = (int)LineType.LODO;   
                }
                wbline.Tfp = Convert.ToDecimal(swLine.totalLineCredit);
                wbline.TripTfp = Convert.ToDecimal(swLine.totalLineCredit);
                wbline.Block = Helper.ConvertMinutesToHhhmmInt(Convert.ToInt32(swLine.totalBlockTime));
                //wbline.Block = Convert.ToInt32(swLine.totalBlockTime);
                List<string> fapositionList = new List<string>();
                //fapositionList.AddRange(swLine.lineKey.seatPosition.Select(c => c.ToString()));
                if (swLine.lineKey.seatPositions != null)
                    fapositionList.AddRange(swLine.lineKey.seatPositions);
                //var myList = new List<string> { swLine.lineKey.seatPosition };
                if (swLine.bidRoundId.roundType == "SECONDARY")
                {
                    switch (wbline.LineType)
                    {
                        case (int)LineType.RESERVE:
                            wbline.LineDisplay = wbline.LineDisplay + "R";
                            wbline.ReserveMode = (int)ReserveType.Reserve;
                            wbline.ReserveLine = true;
                            break;
                        case (int)LineType.AM_RESERVE:
                            wbline.LineDisplay = wbline.LineDisplay + "R";
                            wbline.ReserveMode = (int)ReserveType.AM_Reserve;
                            wbline.ReserveLine = true;
                            break;
                        case (int)LineType.PM_RESERVE:
                            wbline.LineDisplay = wbline.LineDisplay + "R";
                            wbline.ReserveMode = (int)ReserveType.PM_Reserve;
                            wbline.ReserveLine = true;
                            break;
                        case (int)LineType.READY_RESERVE:
                            wbline.LineDisplay = wbline.LineDisplay + "RR";
                            wbline.ReserveMode = (int)ReserveType.Ready_Reserve;
                            wbline.ReserveLine = true;
                            break;
                        case (int)LineType.SENIOR_AM_RESERVE:
                            wbline.LineDisplay = wbline.LineDisplay + "sa";
                            wbline.ReserveMode = (int)ReserveType.SeniorAMReserve;
                            wbline.ReserveLine = true;
                            break;
                        case (int)LineType.SENIOR_PM_RESERVE:
                            wbline.LineDisplay = wbline.LineDisplay + "sp";
                            wbline.ReserveMode = (int)ReserveType.SeniorPMReserve;
                            wbline.ReserveLine = true;
                            break;
                        case (int)LineType.JUNIOR_AM_RESERVE:
                            wbline.LineDisplay = wbline.LineDisplay + "ja";
                            wbline.ReserveMode = (int)ReserveType.JuniorAMReserve;
                            wbline.ReserveLine = true;
                            break;
                        case (int)LineType.JUNIOR_PM_RESERVE:
                            wbline.LineDisplay = wbline.LineDisplay + "jp";
                            wbline.ReserveMode = (int)ReserveType.JuniorPMReserve;
                            wbline.ReserveLine = true;
                            break;
                        case (int)LineType.JUNIOR_LATE_RESERVE:
                            wbline.LineDisplay = wbline.LineDisplay + "jl";
                            wbline.ReserveMode = (int)ReserveType.JuniorLateReserve;
                            wbline.ReserveLine = true;
                            break;

                    }
                }
                else
                {
                    if (wbline.ReserveLine)
                    {
                        wbline.LineDisplay = wbline.LineNum.ToString() + "-R";
                    }
                    else if (wbline.BlankLine)
                    {
                        wbline.LineDisplay = wbline.LineNum.ToString() + "-B";
                    }
                    else if (wbline.LineType == (int)LineType.LODO)
                    {
                        wbline.LineDisplay = wbline.LineNum.ToString() + "-L";
                    }
                }

                wbline.FAPositions = fapositionList;
                wbline.Pairings = new List<string>();
                foreach (var item in swLine.linePairings)
                {
                    wbline.Pairings.Add(item.pairingKey.pairingNumber + Convert.ToDateTime(item.pairingKey.pairingDate).Day.ToString().PadLeft(2, '0'));
                }
                lineList.Add(wbline.LineNum.ToString(), wbline);
            }
            return lineList;
        }

        public List<SeniorityListMember> MapSeniorityData(List<SWASeniority.IFLineBaseAuctionSeniority> lstSeniority)
        {
            SeniorityListMember seniorityListMember = new SeniorityListMember();
            List<SeniorityListMember> lstSeniorityListMember = new List<SeniorityListMember>();


            foreach (var item in lstSeniority)
            {
                seniorityListMember = new SeniorityListMember();
                seniorityListMember.EmpNum = item.employeeId.Replace("(", "").Replace(")", "").PadLeft(6, '0');
                seniorityListMember.DomicileSeniority = item.baseSeniority;
                seniorityListMember.Position = item.department;
                if (item.Vacation != null)
                {
                    seniorityListMember.Absences = new List<Absense>();
                    foreach (var vacation in item.Vacation)
                    {
                        Absense absense = new Absense()
                        {
                            AbsenceType = "VA",
                            StartAbsenceDate = new DateTime(int.Parse(vacation.From.Substring(0, 4)), int.Parse(vacation.From.Substring(5, 2)), int.Parse(vacation.From.Substring(8, 2))),
                            EndAbsenceDate = new DateTime(int.Parse(vacation.To.Substring(0, 4)), int.Parse(vacation.To.Substring(5, 2)), int.Parse(vacation.To.Substring(8, 2)))

                        };
                        seniorityListMember.Absences.Add(absense);

                    }



                }
                lstSeniorityListMember.Add(seniorityListMember);
            }
            return lstSeniorityListMember;
        }






        #region Private methods
        private int GetLineTypeValue(string linetype)
        {
            int type = 1;
            switch (linetype)
            {
                case "HARD":
                    type = (int)LineType.HARD;
                    break;
                case "RELIEF":
                    type = (int)LineType.RELIEF;
                    break;
                case "RESERVE":
                    type = (int)LineType.RESERVE;
                    break;
                case "MIXED":
                    type = (int)LineType.MIXED;
                    break;
                case "ETOPS":
                    type = (int)LineType.ETOPS;
                    break;
                case "LODO":
                    type = (int)LineType.LODO;
                    break;
                case "AM_RESERVE":
                    type = (int)LineType.AM_RESERVE;
                    break;
                case "PM_RESERVE":
                    type = (int)LineType.PM_RESERVE;
                    break;
                case "READY_RESERVE":
                    type = (int)LineType.READY_RESERVE;
                    break;
                case "SENIOR_AM_RESERVE":
                    type = (int)LineType.SENIOR_AM_RESERVE;
                    break;
                case "SENIOR_PM_RESERVE":
                    type = (int)LineType.SENIOR_PM_RESERVE;
                    break;
                case "JUNIOR_AM_RESERVE":
                    type = (int)LineType.JUNIOR_AM_RESERVE;
                    break;
                case "JUNIOR_PM_RESERVE":
                    type = (int)LineType.JUNIOR_PM_RESERVE;
                    break;
                case "JUNIOR_LATE_RESERVE":
                    type = (int)LineType.JUNIOR_LATE_RESERVE;
                    break;
                default:
                    type = 1;
                    break;

            }
            return type;
        }        #endregion
    }
}

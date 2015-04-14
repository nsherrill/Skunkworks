using GP.Accessors.RemoteAccessor;
using GP.Shared.Common;
using GP.Shared.Common.DataContracts;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Accessors.RemoteLeagueAccessor
{
    public interface IRemoteFanDuelAcc
    {
        FantasyLeagueEntry[] GetAllLeagues(GPChromeDriver chromeDriver);
        FantasyPlayer[] GetAllPlayers(GPChromeDriver driver, string foreignLeagueId, string url);
        FantasyRosterDefinition GetRoster(GPChromeDriver driver, string foreignLeagueId, string url);

        void NavigateToLeague(GPChromeDriver cachedDriver, FantasyLeagueEntry interestedLeague);

        void AddPlayerToRoster(GPChromeDriver cachedDriver, string foreignLeagueId, FantasyPlayerRanking fantasyPlayerRanking);

        void ConfirmEntry(GPChromeDriver cachedDriver, string foreignLeagueId);
    }

    public class RemoteFanDuelAcc : IRemoteFanDuelAcc
    {
        private const string HOMEPAGE = @"https://www.fanduel.com/p/Home";
        private const string POSTPAGE = @"https://www.fanduel.com/c/CCAuth";
        private const string FOREIGNSITE_START = @"https://www.fanduel.com";

        public FantasyLeagueEntry[] GetAllLeagues(GPChromeDriver driver)
        {
            List<FantasyLeagueEntry> result = new List<FantasyLeagueEntry>();

            driver.Url = HOMEPAGE;
            driver.Navigate();

            System.Threading.Thread.Sleep(5000);
            try
            {
                var closeModalButton = driver.FindElementByXPath("//*[@id=\"body\"]/div/div[2]/a");
                if (closeModalButton != null)
                    closeModalButton.Click();
            }
            catch
            {
            }

            System.Threading.Thread.Sleep(1000);
            try
            {
                var mlbFilterButton = driver.FindElementByXPath("//*[@id=\"body\"]/section/div[2]/div[2]/div[3]/div[1]/ul/label[3]/li");
                driver.ExecuteScript("arguments[0].scrollIntoView(true);", mlbFilterButton);
                mlbFilterButton.Click();

                var leaguesFilterButton = driver.FindElementByXPath("//*[@id=\"body\"]/section/div[2]/div[2]/div[3]/div[3]/ul/label[3]/li");
                driver.ExecuteScript("arguments[0].scrollIntoView(true);", leaguesFilterButton);
                leaguesFilterButton.Click();
            }
            catch
            {
                var searchTextBox = driver.FindElementByXPath("//*[@id=\"foo\"]/input[1]");
                searchTextBox.SendKeys("\b\b\b\b\b\b\b\b\b\b\b\b\b");
                searchTextBox.SendKeys("MLB salary");
            }

            System.Threading.Thread.Sleep(2000);
            var allTxt = driver.PageSource;
            allTxt = allTxt.Substring(allTxt.IndexOf("LobbyConnection.initialData =") + "LobbyConnection.initialData =".Length);
            allTxt = allTxt.Substring(0, allTxt.IndexOf("LobbyConnection.lastUpdate =")).Trim();
            if (allTxt.EndsWith(";"))
                allTxt = allTxt.Substring(0, allTxt.Length - 1).Trim();

            var data = JsonConvert.DeserializeObject<FantasyLeagueJSONObject>(allTxt);
            foreach (var league in data.additions)
            {
                bool includeLeague = true;
                FantasyLeagueEntry newEntry = null;

                string sport = league.sport;

                if (sport == "mlb")
                {
                    var entryFeeStr = league.entryFee;
                    var entryFee = double.Parse(entryFeeStr);
                    var foreignId = league.uniqueId;
                    var title = league.title;
                    var leagueSizeStr = league.size;
                    var leagueSize = int.Parse(leagueSizeStr);
                    var dateStartStr = league.startString.Replace("&amp;nbsp;", " ");
                    dateStartStr = dateStartStr.ToLower().Replace("sun", "").Replace("mon", "").Replace("tue", "")
                        .Replace("wed", "").Replace("thu", "").Replace("fri", "").Replace("sat", "");
                    var dateStart = DateTime.Parse(DateTime.Now.Date.ToShortDateString() + " " + dateStartStr);
                    if (DateTime.Now > dateStart)
                        dateStart = DateTime.Parse(DateTime.Now.AddDays(1).Date.ToShortDateString() + " " + dateStartStr);
                    var leagueUrl = (FOREIGNSITE_START.TrimEnd('/') + '/' + league.entryURL.TrimStart('/')).Replace("&amp;", "&");

                    var salaryCapStr = league.cap;
                    var salaryCap = double.Parse(salaryCapStr.Replace("$", "").Replace(",", ""));

                    var leagueType = LeagueType.Auction;
                    if (title.ToLower().Contains("head-to-head"))
                    {
                        leagueType = LeagueType.HeadToHead;
                        includeLeague = false;
                    }

                    newEntry = new FantasyLeagueEntry()
                    {
                        BuyIn = entryFee,
                        ForeignId = foreignId,
                        ForeignSite = FOREIGNSITE_START,
                        ForeignTitle = title,
                        IsActive = true,
                        LeagueType = leagueType,
                        ParticipantCount = leagueSize,
                        Sport = SportType.Baseball,
                        StartDate = dateStart,
                        Url = leagueUrl,
                        SalaryCap = salaryCap,
                    };
                }
                else includeLeague = false; //only include baseball

                if (includeLeague)
                    result.Add(newEntry);
            }

            return result.ToArray();
        }

        public FantasyPlayer[] GetAllPlayers(GPChromeDriver driver, string foreignLeagueId, string url)
        {
            driver.Url = url;
            driver.Navigate();

            var allTxt = driver.PageSource;
            allTxt = allTxt.Substring(allTxt.IndexOf("FD.playerpicker.allPlayersFullData = ") + "FD.playerpicker.allPlayersFullData = ".Length);
            allTxt = allTxt.Substring(0, allTxt.IndexOf("FD.playerpicker.teamIdToFixtureCompactString =")).Trim();
            if (allTxt.EndsWith(";"))
                allTxt = allTxt.Substring(0, allTxt.Length - 1).Trim();
            var data = JsonConvert.DeserializeObject<Dictionary<int, object[]>>(allTxt);


            allTxt = driver.PageSource;
            allTxt = allTxt.Substring(allTxt.IndexOf("FD.playerpicker.teamIdToFixtureCompactString = ") + "FD.playerpicker.teamIdToFixtureCompactString = ".Length);
            allTxt = allTxt.Substring(0, allTxt.IndexOf("FD.playerpicker.positions")).Trim();
            if (allTxt.EndsWith(";"))
                allTxt = allTxt.Substring(0, allTxt.Length - 1).Trim();
            var gameData = JsonConvert.DeserializeObject<Dictionary<int, string>>(allTxt);

            //{"597":"KAN@&lt;b&gt;CLE&lt;/b&gt;","599":"&lt;b&gt;KAN&lt;/b&gt;@CLE","609":"NYM@&lt;b&gt;WAS&lt;/b&gt;","607":"&lt;b&gt;NYM&lt;/b&gt;@WAS","595":"SEA@&lt;b&gt;TOR&lt;/b&gt;","603":"&lt;b&gt;SEA&lt;/b&gt;@TOR","605":"PIT@&lt;b&gt;ATL&lt;/b&gt;","614":"&lt;b&gt;PIT&lt;/b&gt;@ATL","611":"MIL@&lt;b&gt;CIN&lt;/b&gt;","613":"&lt;b&gt;MIL&lt;/b&gt;@CIN","592":"TAM@&lt;b&gt;BOS&lt;/b&gt;","594":"&lt;b&gt;TAM&lt;/b&gt;@BOS","606":"PHI@&lt;b&gt;MIA&lt;/b&gt;","608":"&lt;b&gt;PHI&lt;/b&gt;@MIA","604":"HOU@&lt;b&gt;TEX&lt;/b&gt;","612":"&lt;b&gt;HOU&lt;/b&gt;@TEX","610":"STL@&lt;b&gt;CHC&lt;/b&gt;","615":"&lt;b&gt;STL&lt;/b&gt;@CHC","619":"COL@&lt;b&gt;SDP&lt;/b&gt;","617":"&lt;b&gt;COL&lt;/b&gt;@SDP","618":"SFG@&lt;b&gt;LOS&lt;/b&gt;","620":"&lt;b&gt;SFG&lt;/b&gt;@LOS"};

            List<FantasyPlayer> result = new List<FantasyPlayer>();
            foreach (var foreignPlayerId in data.Keys)
            {
                try
                {
                    //"5427":["P","Clayton Kershaw","84737","618","30","12600",17.7,26,false,5,"","recent",""]
                    int foreignId = foreignPlayerId;
                    var availabilityStr = data[foreignId][12].ToString();
                    PlayerAvailability availability;
                    if (string.IsNullOrEmpty(availabilityStr))
                        availability = PlayerAvailability.a_Available;
                    else
                    {
                        if (!Enum.TryParse<PlayerAvailability>("a_" + availabilityStr.Replace("-", "").Replace(" ", ""), true, out availability))
                        {

                        }
                    }

                    if (availability == PlayerAvailability.a_Available)
                    {
                        string positionTxt = data[foreignId][0].ToString();
                        BaseballPosition position = (BaseballPosition)Enum.Parse(typeof(BaseballPosition), "pos_" + positionTxt);

                        string nameTxt = data[foreignId][1].ToString();

                        string teamAbrIdTxt = data[foreignId][3].ToString();
                        int teamAbrId = int.Parse(teamAbrIdTxt);
                        string teamAbrTxtSource = gameData[teamAbrId].Replace("&gt;", ">").Replace("&lt;", "<");
                        string teamAbrTxt = RegexHelper.GetRegex(@".*?<b>(.+?)</b>.*?", teamAbrTxtSource, 1);

                        string ppgTxt = data[foreignId][6].ToString();
                        double ppg = double.Parse(ppgTxt);

                        string valueTxt = data[foreignId][5].ToString();
                        double value = double.Parse(valueTxt);

                        string gamesPlayedTxt = data[foreignId][7].ToString();
                        int gamesPlayed = int.Parse(gamesPlayedTxt);

                        var newPlayer = new FantasyPlayer()
                        {
                            ForeignId = foreignId.ToString(),
                            GamesPlayed = gamesPlayed,
                            Name = nameTxt,
                            Position = position,
                            PPG = ppg,
                            TeamAbr = teamAbrTxt,
                            Value = value,
                            ForeignLeagueId = foreignLeagueId,
                        };
                        result.Add(newPlayer);
                    }
                }
                catch (Exception e)
                {

                }
            }

            return result.ToArray();
        }

        public FantasyRosterDefinition GetRoster(GPChromeDriver driver, string foreignLeagueId, string url)
        {
            driver.Url = url;
            driver.Navigate();
            try
            {
                FantasyRosterDefinition result = new FantasyRosterDefinition();

                var allTxt = driver.PageSource;
                //    FD.playerpicker.teamPlayersData = [["P",0],["C",0],["1B",0],["2B",0],["3B",0],["SS",0],["OF",0],["OF",0],["OF",0]];
                //FD.playerpicker.allPlayersFullData
                allTxt = allTxt.Substring(allTxt.IndexOf("FD.playerpicker.teamPlayersData = ") + "FD.playerpicker.teamPlayersData = ".Length);
                allTxt = allTxt.Substring(0, allTxt.IndexOf("FD.playerpicker.allPlayersFullData")).Trim();
                if (allTxt.EndsWith(";"))
                    allTxt = allTxt.Substring(0, allTxt.Length - 1).Trim();

                var strings = allTxt.Split('"');
                result.Starting1B = strings.Where(s => s == "1B").Count();
                result.Starting2B = strings.Where(s => s == "2B").Count();
                result.Starting3B = strings.Where(s => s == "3B").Count();
                result.StartingC = strings.Where(s => s == "C").Count();
                result.StartingP = strings.Where(s => s == "P").Count();
                result.StartingOF = strings.Where(s => s == "OF").Count();
                result.StartingSS = strings.Where(s => s == "SS").Count();

                allTxt = driver.PageSource;
                //FD.playerpicker.salaryCap = 35000;
                allTxt = allTxt.Substring(allTxt.IndexOf("FD.playerpicker.salaryCap = ") + "FD.playerpicker.salaryCap = ".Length);
                allTxt = allTxt.Substring(0, allTxt.IndexOf(";")).Trim();
                result.SalaryCap = double.Parse(allTxt);

                result.ForeignId = foreignLeagueId;
                return result;
            }
            catch
            {
            }

            return null;
        }

        public void NavigateToLeague(GPChromeDriver cachedDriver, FantasyLeagueEntry interestedLeague)
        {
            //cachedDriver.Url = string.Format(FOREIGNSITE_LEAGUE, interestedLeague.ForeignId);
            cachedDriver.Url = interestedLeague.Url;
            cachedDriver.Navigate();

            /*<input type="checkbox" id="probablePlayersOnlyCheckbox" class="probablePlayersOnlyCheckbox" checked="">
             */
            var probableCheck = cachedDriver.FindElementById("probablePlayersOnlyCheckbox");
            probableCheck.Click();
        }

        public void AddPlayerToRoster(GPChromeDriver cachedDriver, string foreignLeagueId, FantasyPlayerRanking fantasyPlayerRanking)
        {
            //ensure it's within view
            //<input type="search" results="10" autosave="fd-player-search" incremental="" placeholder="Find a player..." id="player-search">

            var textboxElement = cachedDriver.FindElementById("player-search");
            textboxElement.SendKeys(fantasyPlayerRanking.Name);

            System.Threading.Thread.Sleep(2000);
            /*<tr id="playerListPlayerId_6234" data-role="player" data-position="P" data-fixture="95394" data-probable="" class="pR  fixtureId_95394 teamId_609 probablePitcher  news-old">
									<td class="player-position">P</td>
									<td class="player-name"><div onclick="sSts('6234_12026')">Jordan Zimmermann<span class="player-badge player-badge-probable-pitcher">P</span></div></td>
									<td class="player-fppg">11.8</td>
									<td class="player-played">32</td>
									<td class="player-fixture">NYM@<b>WAS</b></td>
									<td class="player-salary">$9,900</td>
									<td class="player-add">
										<a data-role="add" id="add-button" data-player-id="6234" class="button tiny text player-add-button"><i class="icon"></i></a>
										<a data-role="remove" id="remove-button" data-player-id="6234" class="button tiny text player-remove-button"><i class="icon">␡</i></a>
									</td>
								</tr>             
             */

            string tableId = string.Format("playerListPlayerId_{0}", fantasyPlayerRanking.ForeignId);
            var tableElement = cachedDriver.FindElementsById(tableId);
            var addButton = tableElement.First().FindElement(By.Id("add-button"));

            addButton.Click();

            /*
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < fantasyPlayerRanking.Name.Length + 10; i++)
                sb.Append("\b");
            textboxElement.SendKeys(sb.ToString());
            */
        }

        public void ConfirmEntry(GPChromeDriver cachedDriver, string foreignLeagueId)
        {
            /*<input id="enterButton" type="submit" class="button jumbo primary" value="Enter" data-nav-warning="off">
             */
            var enterButton = cachedDriver.FindElementById("enterButton");
            enterButton.Click();
        }



        #region testing
        public void TEST_DESERIALIZE()
        {
            //var data = JsonConvert.DeserializeObject<FantasyLeagueJSONObject>(SAMPLE_JSON_RESULT);

            var tempdata = System.IO.File.ReadAllText(@"C:\Users\Nick\Documents\testdata.html");
            var data = JsonConvert.DeserializeObject<FantasyLeagueJSONObject>(tempdata);
        }

        private const string SAMPLE_JSON_RESULT = @"{""lastUpdate"":1409534417485,""additions"":[{""uniqueId"":""5228281"",""gameId"":10464,""dateCreated"":1409513177138,""entryHTML"":null,""tab"":""contest"",""entryURL"":""\/e\/Game\/10464?tableId=5228281&fromLobby=true"",""sport"":""mlb"",""cap"":35000,""startTime"":1409601900,""startString"":""Mon 4:05&nbsp;pm"",""title"":""Head to Head Matrix"",""tableSpecId"":31804,""entryFee"":270,""entryFeeFormatted"":""$270"",""prizes"":2750,""prizeBreakdown"":true,""prizeSummary"":null,""size"":""11"",""maxEntriesPerUser"":1,""flags"":{""standard"":1},""seatCode"":"""",""entriesData"":""0"",""dateUpdated"":1409513177139},{""uniqueId"":""5228073"",""gameId"":10463,""dateCreated"":1409513167520,""entryHTML"":null,""tab"":""contest"",""entryURL"":""\/e\/Game\/10463?tableId=5228073&fromLobby=true"",""sport"":""mlb"",""cap"":35000,""startTime"":1409591400,""startString"":""Mon 1:10&nbsp;pm"",""title"":""MLB 50\/50 League (Early Only)"",""tableSpecId"":33626,""entryFee"":1,""entryFeeFormatted"":""$1"",""prizes"":72,""prizeBreakdown"":true,""prizeSummary"":null,""size"":""80"",""maxEntriesPerUser"":1,""flags"":{""standard"":1,""50_50"":1},""seatCode"":"""",""entriesData"":""17"",""dateUpdated"":1409533634055},{""uniqueId"":""5228029"",""gameId"":10463,""dateCreated"":1409513165434,""entryHTML"":null,""tab"":""contest"",""entryURL"":""\/e\/Game\/10463?tableId=5228029&fromLobby=true"",""sport"":""mlb"",""cap"":35000,""startTime"":1409591400,""startString"":""Mon 1:10&nbsp;pm"",""title"":""MLB Salary Cap 35k Mon Sep 1st (Early Only)"",""tableSpecId"":1029,""entryFee"":0,""entryFeeFormatted"":""$0"",""prizes"":0,""prizeBreakdown"":false,""prizeSummary"":null,""size"":""10"",""maxEntriesPerUser"":1,""flags"":{""standard"":1},""seatCode"":"""",""entriesData"":""5"",""dateUpdated"":1409529209299},{""uniqueId"":""4981219"",""gameId"":10290,""dateCreated"":1409229551399,""entryHTML"":null,""tab"":""contest"",""entryURL"":""\/e\/Game\/10290?tableId=4981219&fromLobby=true"",""sport"":""nfl"",""cap"":60000,""startTime"":1410109200,""startString"":""Sun 1:00&nbsp;pm"",""title"":""NFL Salary Cap 60k Sun Sep 7th"",""tableSpecId"":35733,""entryFee"":2660,""entryFeeFormatted"":""$2,660"",""prizes"":5000,""prizeBreakdown"":false,""prizeSummary"":null,""size"":""2"",""maxEntriesPerUser"":1,""flags"":{""standard"":1},""seatCode"":"""",""entriesData"":""1"",""dateUpdated"":1409504400002},{""uniqueId"":""5227311"",""gameId"":10462,""dateCreated"":1409508907939,""entryHTML"":null,""tab"":""contest"",""entryURL"":""\/e\/Game\/10462?tableId=5227311&fromLobby=true"",""sport"":""mlb"",""cap"":35000,""startTime"":1409591400,""startString"":""Mon 1:10&nbsp;pm"",""title"":""MLB 50\/50 League"",""tableSpecId"":33626,""entryFee"":1,""entryFeeFormatted"":""$1"",""prizes"":72,""prizeBreakdown"":true,""prizeSummary"":null,""size"":""80"",""maxEntriesPerUser"":1,""flags"":{""standard"":1,""50_50"":1},""seatCode"":"""",""entriesData"":""13"",""dateUpdated"":1409532142543},{""uniqueId"":""5228103"",""gameId"":10463,""dateCreated"":1409513168927,""entryHTML"":null,""tab"":""contest"",""entryURL"":""\/e\/Game\/10463?tableId=5228103&fromLobby=true"",""sport"":""mlb"",""cap"":35000,""startTime"":1409591400,""startString"":""Mon 1:10&nbsp;pm"",""title"":""Head to Head Matrix"",""tableSpecId"":33630,""entryFee"":5,""entryFeeFormatted"":""$5"",""prizes"":94.5,""prizeBreakdown"":true,""prizeSummary"":null,""size"":""21"",""maxEntriesPerUser"":1,""flags"":{""standard"":1},""seatCode"":"""",""entriesData"":""2"",""dateUpdated"":1409529034261},{""uniqueId"":""5229467"",""gameId"":10290,""dateCreated"":1409521802404,""entryHTML"":null,""tab"":""contest"",""entryURL"":""\/e\/Game\/10290?tableId=5229467&fromLobby=true"",""sport"":""nfl"",""cap"":60000,""startTime"":1410109200,""startString"":""Sun 1:00&nbsp;pm"",""title"":""NFL 50\/50 League"",""tableSpecId"":35527,""entryFee"":10,""entryFeeFormatted"":""$10"",""prizes"":450,""prizeBreakdown"":true,""prizeSummary"":null,""size"":""50"",""maxEntriesPerUser"":1,""flags"":{""standard"":1,""50_50"":1},""seatCode"":"""",""entriesData"":""3"",""dateUpdated"":1409525413453}],""modifications"":[],""remaining"":[5228281,5228073,5228029,4981219,5227311,5228103,5229467,""874056-5553-10463"",5184789,5223464,5228221,""69831-1079-10463"",5184911,5077015,5228098,5227297,5227091,5227811,5228022,5076849,5182139,""748773-370-10290"",5192909,5220842,""63154-823-10395"",5230506,5228215,""874056-3-10463"",5221761,5046189,5131980,5223709,5046348,5225595,4940177,5223272,5227989,5176713,5230332,5227276,5227133,5208072,5227258,5200376,5175999,""94104-370-10290"",5046066,""726336-4-10462"",5218794,4981222,5230353,5229583,5046370,5226814,5227961,5229442,5045217,5163812,4933663,5211172,""69831-5553-10462"",5227227,""318499-1079-10395"",5045304,5229561,5117973,5227349,5045545,5076785,5228173,5229035,5228115,5227308,5211777,5223348,5227332,5224618,5216411,5228244,5102904,5077060,5227344,5200813,5176070,5228261,5076781,5222650,""69142-5-10462"",5050753,5225871,5171113,5227934,5224269,5227071,5227516,5218009,""874056-2-10290"",5217868,5228235,""63154-5-10395"",5228231,5228127,4933547,""41619-5553-10290"",5228229,5176489,5181559,5228192,5200168,5222791,5203376,5227337,5058709,5228130,5076904,5228271,5227998,5228075,5227166,4933620,5228088,5227352,5227255,5224889,5227914,""60705-823-10395"",5227424,5215454,5224590,""125323-4-10395"",5228089,5215063,5045412,5165955,5076782,""60705-979-10395"",5228274,5228148,5191479,5176324,5227044,5186937,5045305,5224513,5228210,5176847,5198990,5223667,5226294,5176011,5228152,5227158,5176867,5228216,5147409,5229199,5228246,5227309,5166396,5228213,5046505,5228111,5228049,5227310,""874056-4-10462"",5221291,5215007,5230137,5227423,5176297,5045523,5230136,""118627-4-10464"",5046336,""290021-1-10462"",5046338,5230459,5229036,5227302,5229024,5186297,5228035,""799856-2-10290"",5228064,5202491,5227284,5227940,5227322,5228249,""118627-370-10462"",5230170,4981224,5228491,5227291,5215958,""748773-3-10395"",5223760,5175556,5227191,5045210,5227966,5227184,5209631,5202697,""874056-5-10395"",5045546,""60705-823-10290"",5228083,5220985,5227294,""63154-370-10395"",5208429,5046359,5227303,""874056-2-10462"",5217530,5230431,5228044,5076847,5190665,5225643,""284099-5553-10464"",5224638,5216324,5228160,5218726,5227830,""372694-3-10290"",5046358,4933660,5046064,5227684,5208991,5182146,""27411-2-10290"",5176663,4940180,5227224,4981221,5045306,5209839,5077063,5230077,4933503,5230238,5198497,5088799,5227796,""54203-979-10395"",5092395,5230197,""1059037-1079-10395"",5228268,5209260,5228165,5227999,5198524,5209511,5227261,5227146,5077024,5229016,5228252,4933450,5227059,""125323-370-10395"",5174076,5164048,5229072,5227256,5175960,5223474,5228050,5208381,""726336-5-10464"",5176385,5230356,5176714,5228090,5199171,4981216,5230335,5140312,5046371,5068397,5230383,5227319,""75657-3-10290"",5203148,4981213,5176764,5086471,5216004,4933614,5227301,5228104,5229317,4983535,5045220,""874056-1079-10464"",5202669,5077018,5190936,5227257,5224435,5223579,5227931,5225964,5045311,5228076,5176872,5200550,5176865,5227967,5228054,5218132,5228189,""54203-3-10290"",5227240,5221319,5227219,""154019-370-10290"",5221157,""799856-3-10395"",""118627-2-10464"",5230376,""754355-2-10290"",5199129,5228256,5227183,""548636-2-10462"",5227046,5221005,5228186,5045221,5194227,5228232,5204373,5185785,5228114,5227328,5227304,""874056-3-10395"",""69142-2-10462"",5076945,""125323-370-10290"",5076783,5228058,5228280,5217206,5045211,5227783,5176499,5228079,5217560,5227313,5227151,""118627-5-10395"",5230340,5185209,4933652,5221272,5176215,5045547,5218949,5227388,""118627-2-10395"",5227096,5200938,5077059,""874056-1079-10463"",5227116,5219090,5227293,5209199,""748773-823-10290"",5045413,5230399,5077052,""748773-979-10290"",4981223,5176801,5228061,5228187,5226250,5227039,4996040,5228091,5222483,5228211,5228255,5221999,5228236,""54203-370-10395"",5176229,""154019-4-10290"",5230139,5229451,5198845,5227145,""548636-2-10463"",5228003,5180960,5229204,5077903,5222234,5203277,5230483,5175930,""118627-4-10462"",5184449,5221654,5229140,""372694-5-10395"",""53023-1-10462"",5177659,5228043,5227253,""75657-4-10290"",""118627-5-10290"",5230348,5224535,5227150,5227960,5077896,5198840,4981217,4933626,5202719,5113245,""290021-1-10464"",""528959-1-10462"",5221226,""290021-1-10463"",5228131,5227318,5203898,4933451,""726336-4-10463"",5225464,5029988,5180804,5225416,5217146,5089377,""724101-4-10462"",5045526,5228276,4983563,5077025,5226834,5176848,5227245,5046547,5228053,5228096,5227072,5222390,5227288,5230500,5076778,4950370,5230248,5227927,5228390,5224297,5172833,5045302,5227460,5185019,5228283,5223335,5176716,5224055,5227342,""754355-5553-10395"",""75657-2-10290"",5208313,""27298-4-10290"",5168993,5227097,5221895,""75657-5-10290"",""72290-3-10395"",5162126,5227292,5224974,5128604,5168577,5223743,5165900,""600169-2-10395"",""54203-370-10290"",5222026,5227079,5230334,""118627-3-10290"",5077017,5046331,5222273,""739747-2-10395"",""874056-4-10464"",5147480,4981228,5227312,5201397,5043081,""900582-5553-10462"",5228102,5228077,5225063,""118627-4-10463"",5217207,""75657-823-10290"",5228070,5045212,""36419-1079-10290"",""41619-3-10290"",5228034,""63154-4-10290"",5227056,""69831-1079-10464"",""888093-1-10464"",5159823,5203033,5215880,5228228,5221488,4936960,5229318,5215891,5228185,5146348,5228002,5194646,""60705-3-10395"",5213197,5224362,4981212,5045548,5176090,5209103,""98760-5-10290"",5228009,""728827-1079-10395"",5129015,5210650,5229476,""27411-3-10290"",5227865,""874056-4-10463"",5210942,5076846,5194826,5077007,5227270,5077061,5227229,5077023,5228082,5176755,5223481,5216092,5042910,5228238,5213006,5210882,5227959,5176193,5221876,5228062,5228038,""284099-1079-10464"",""184726-5553-10395"",5227340,5228057,5046249,5227211,5221387,""748773-2-10395"",5194832,4933659,5076895,5222733,5210896,5046065,5225757,5201126,5042887,5168643,5176715,""72290-2-10395"",5227985,5176870,5230207,5123707,5228655,5209338,5076946,5228194,4933452,5085910,""724101-370-10462"",5045219,5221401,""726336-370-10462"",5228067,4940171,5216380,5077020,5230401,5227281,5171215,5203303,4981211,5182150,5228273,5046350,""726336-5-10462"",5227296,""874056-3-10462"",""63154-4-10395"",5228195,5229478,5220992,5199586,5045415,5227207,5230268,5227230,5228170,5227345,5229218,""726336-3-10462"",5221304,""118627-3-10395"",4933506,5203373,5076779,5227315,4939963,5229707,5228144,5023333,5228220,5182160,5176320,5046337,5226802,5039886,5175121,5194687,5227181,5203363,5203369,5227695,5114492,5230374,5181640,4933449,5228251,5227979,5227414,5226252,5227324,5221872,5228219,5230138,4939701,5229033,5227330,4954360,5203313,5200752,""748773-370-10395"",4933617,5222391,5228214,5136893,5229293,5146693,5046349,5088790,""49756-2-10462"",5076903,5176309,5221565,""724101-3-10462"",5045525,5205620,""874056-1079-10462"",5227317,4933627,5228169,5227239,5228100,5221388,5216200,5045213,5228600,5147258,5227287,5170489,5203060,5229631,5228223,5227263,5194173,5224972,5228248,5042872,5228205,5229445,5204649,5230465,5173707,5230347,5173865,5203406,""118627-4-10290"",5227238,5181990,5076844,5230246,5228209,""748773-5-10290"",5077026,5227926,5227259,""724101-5-10462"",""754355-5553-10290"",5202870,5200166,""69142-823-10462"",""874056-370-10462"",5216123,5076850,5228092,4933618,5221643,5182022,5208416,5227911,5227182,5042504,5046346,5227008,5228233,5227279,5173889,5227055,5168652,5215123,5227932,5050991,""372694-979-10395"",5210934,""397079-3-10290"",5045303,5227228,5228174,5228166,""200344-5553-10395"",5166399,5225313,""41619-1079-10290"",4947218,""36419-5553-10290"",5227348,""74727-1-10462"",5230342,5227223,5076788,5201582,""613333-1079-10395"",5200143,5176756,5228270,5085004,5165970,5212747,5160828,5228015,5228042,5212946,5179680,5202653,5077880,5182017,5221381,5076845,5228026,5166104,5222384,4981215,5227197,5228392,5228282,5192402,""54203-2-10395"",5228222,5181122,5192198,""94104-5-10290"",""118627-3-10464"",5202514,5039770,""525156-3-10290"",5212112,5176709,""54203-3-10395"",4981214,5045310,5229401,5045222,5230295,""637607-1079-10395"",5227321,5229532,5217141,5046361,5198906,4933625,5184591,5162929,5045414,5211726,5227925,5164747,5216340,5230179,5228234,4933453,5227208,5208300,""72290-5553-10395"",5228039,5228085,5228056,5227994,5222665,5228178,5176321,""728827-1079-10290"",5176129,5227286,""748773-823-10395"",5178157,5221210,""69142-370-10462"",5191308,""54203-4-10395"",5228084,5224053,5228007,5228217,5227333,5227316,5228012,5216371,5212107,5194507,5203824,4933454,5228258,5225042,5227050,5228254,5227939,5228237,5046526,5227180,""54203-5-10290"",5227155,5131760,""726336-370-10463"",5206455,""118627-2-10463"",5227978,5108600,5176855,5175901,""372694-823-10290"",5184460,5039967,5228078,5181235,""900582-1079-10462"",5203345,""372694-370-10395"",5227919,5221448,5116147,5030426,5227266,5043088,""372694-823-10395"",5227426,5209319,5208591,5221103,5202210,5229579,5227254,""63154-979-10395"",5227060,5130152,5192482,5204446,5228230,5228093,5227329,""125323-5-10290"",5223555,5198659,5166537,5077019,5227179,5063542,5227992,5227077,5228011,5222155,""888093-1-10462"",4940181,5013299,5227962,5045528,5227143,5217773,4939875,5205224,5160575,""726336-3-10464"",5227973,5230235,5228014,5230346,5209785,5227102,5228055,5076789,5218552,5130204,5228040,""482156-1079-10395"",5228068,5216240,5112915,5176396,5076851,5228265,""754355-3-10290"",5227225,5228242,5022939,5227314,5039784,5228175,5228250,5227343,5045308,5213007,5227220,5209456,5230400,5046369,5228224,4981225,""726336-2-10462"",5228107,5209571,5227043,5227737,""118627-5-10463"",5046344,5227339,5230395,5225841,5230329,5227326,5228126,5223798,5221995,5176858,5172634,5076898,5222219,5224271,5061304,5077057,5203677,5176856,""748773-979-10395"",5227283,5221656,5228208,5077051,""772420-1-10464"",""152801-370-10395"",5228275,5229177,""874056-4-10290"",""118627-370-10290"",5076793,5230482,5226043,5184462,""874056-5553-10290"",5227076,5222333,5228257,4981218,5228353,5164777,5223067,5228179,5213806,5227988,5228059,5175849,5199612,5227169,5198849,5227280,5230366,5228060,5203350,""372694-2-10395"",5221572,5230257,""125323-3-10395"",5227048,5182152,5227305,""118627-3-10463"",5229544,5124013,5228072,5215937,""118627-2-10290"",5218680,5228239,5228028,5046301,5199712,5228279,""482156-2-10395"",5228225,5227267,5210465,5045527,5028476,5229575,5228203,5175902,5229576,5228253,5229791,5176371,5216307,5230497,5181642,5228245,""152801-823-10395"",""63154-370-10290"",4933607,5228343,5224610,5076786,5165772,5228212,5181933,""874056-3-10290"",5229060,""36419-2-10290"",5227164,5076900,""799856-4-10290"",4988649,5176802,5223503,5228069,5204961,5221507,""372694-4-10290"",5202496,5211257,5077021,5192239,5227295,""69142-4-10462"",5168692,""98760-4-10290"",5228087,4940178,5230504,5227501,5221228,""200344-5553-10464"",5227794,5222487,5227996,""726336-3-10463"",5221832,5230449,5227081,5230106,5129876,5159588,""372694-979-10290"",5227272,5227178,5227053,""27411-4-10290"",5227209,5228036,5226815,5181749,5208698,5228037,5045223,5076905,4933622,5221482,5210408,5227232,5228263,5176322,4933648,5230088,""372694-4-10395"",5198513,5183856,""874056-979-10462"",5224804,5228010,5046360,5076852,5228262,5046339,5176769,5221864,5222319,5216382,""754355-2-10395"",5120762,5228047,5046469,""739747-5553-10290"",5230339,5210374,5189900,5026158,5211366,5215274,5227140,5182149,5199077,""63154-3-10395"",4933504,5227429,5228080,5226862,5159832,5176706,5215180,5046327,5076843,""874056-4-10395"",5117640,5229277,""63154-823-10290"",5198909,5228094,5227278,5227407,5077013,""60705-979-10290"",5050987,5046186,5225810,5045309,5224576,5116365,4981220,5228065,""874056-2-10395"",""728827-5553-10395"",""888093-1-10463"",5176859,""874056-5553-10462"",5208385,4940179,5228154,5208673,""129443-2-10290"",5227282,5175903,""355974-3-10290"",5041357,5124019,5050782,5222436,""69142-3-10462"",""726336-5-10463"",""125323-5553-10395"",5230409,5230241,5209188,5131157,5228013,5228071,5168650,5103070,""118627-370-10395"",5077058,5203726,5205290,5227937,5223409,5076842,5228129,5225406,5227057,5221740,5227277,""874056-2-10464"",5228008,5228200,5229027,5222223,5199587,5199639,5228051,5182705,""118627-3-10462"",5046368,5045543,5229646,5228158,5228108,5227323,5227528,""54203-823-10395"",""748773-3-10290"",5227231,5211492,5222851,5201164,5046345,""60705-4-10290"",5230402,5045550,5228376,5199010,5228066,5227944,5230492,5204303,5230158,5046530,5168727,5076964,5230345,5227213,5078678,5175900,5046329,""874056-823-10462"",""726336-370-10464"",5205416,5221345,5227813,5191044,5222518,5227331,5134173,5228202,5227647,5138971,5209393,5076838,5176772,5227075,5200957,5176319,5217947,5216610,4933623,5230105,5046335,5228177,5227041,""724101-2-10462"",5227351,5228005,5227335,""884870-5553-10462"",5216031,""726336-4-10464"",""60705-370-10290"",5227285,5176241,5229581,5172636,5045513,""482156-5553-10395"",""60705-370-10395"",5229704,5203569,5208656,5227964,5209058,5228240,5228095,5183157,5217158,5211330,5211359,""874056-3-10464"",5045416,5076790,5216659,5227274,5160137,4933505,""54203-5-10395"",5227250,5198825,5185056,4933664,5227299,5043803,5176711,""799856-2-10395"",5225922,5227149,5176323,5192174,5176160,5192197,5120745,5229834,4940174,""347005-2-10462"",5184723,5228046,""60705-5-10395"",5229151,5220917,""63154-3-10290"",5227320,5224338,5210791,5222325,5229674,5223116,4976557,""60705-2-10290"",""372694-370-10290"",5227025,""118627-370-10464"",5228033,5226907,""154019-823-10290"",5191229,5227327,5227222,5227325,""874056-5553-10464"",5217159,5076853,5230324,5077012,5228081,""397079-2-10290"",5203314,4933545,""54203-979-10290"",5076787,5208603,5176850,5227733,""125323-2-10395"",""69831-2-10464"",""726336-2-10463"",5200671,5226818,5227251,""63154-5-10290"",5146399,""27298-3-10290"",5229452,5227778,5208102,5227853,5228019,5042861,""118627-5-10464"",5228030,5208332,5211725,5228181,5227990,5225842,5223089,5192789,5228847,5228086,5227307,5210200,5229506,5228243,5229978,5208422,5227040,5227098,5229167,5218017,""152801-4-10395"",5076848,5202637,5153082,5227168,5230505,""482156-3-10395"",5227275,""54203-823-10290"",5076784,5077050,5228045,5227350,""63154-979-10290"",5190113,5228006,""200344-1079-10395"",5117731,""200344-1079-10463"",5230311,5176803,5210201,5175360,5227981,5021504,5046188,5203250,5051897,5229982,5228106,5228112,5228136,5229735,5200383,4933925,5230469,5227298,5227421,5176448,5222714,5200624,5198850,5146421,""27298-2-10290"",5187262,5228227,5222388,5176432,5229019,5076839,5076943,5227249,""54203-4-10290"",5172631,""152801-979-10395"",5058691,5221476,5046330,5203431,5169105,5228156,5209386,4933851,5076776,5203260,5077016,5228113,5228023,4933661,5227380,5185573,5226798,5176754,5175776,5046328,""125323-5-10395"",5227300,5221467,5221288,5228105,""75657-370-10290"",5226353,5227216,5230337,5228247,5230259,""874056-2-10463"",5227341,5192545,""118627-4-10395"",5046355,5227148,5229050,5193976,5097290,4933546,5230368,5045524,""726336-2-10464"",5227938,5228241,""355974-4-10290"",5227234,5230495,5076901,4940175,5228048,4933621,5216156,5228074,5229499,""118627-5-10462"",""118627-370-10463"",5225867,5215147,5209169,""372694-5-10290"",5228021,""75657-979-10290"",5227273,""372694-3-10395"",5121097,""98760-370-10290"",5217212,""60705-5-10290"",""60705-3-10290"",4933662,5181949,""160733-1-10464"",5176712,5221512,5228272,5227217,5227187,5209072,5115374,5222507,5228167,5230267,5182144,5077008,5227855,5177310,5228063,""41619-4-10290"",5173517,5222214,5043451,5228259,5177145,""118627-2-10462"",5230385,5228201,5228133,5176310,5228140,""874056-370-10395"",5227425,""69831-5553-10463"",5218406,5227422,5228162,5215900,5227921,5228052,5146419,5228132],""lastRemoval"":1409534409341}";
        #endregion
    }

    class FantasyLeagueJSONObject
    {
        public string lastUpdate;
        public FantasyLeagueJSONObject_additions[] additions;
        public string[] modifications;
        public string[] remaining;
    }

    class FantasyLeagueJSONObject_additions
    {
        public string uniqueId;
        public string gameId;
        public string dateCreated;
        public string entryHTML;
        public string tab;
        public string entryURL;
        public string sport;
        public string cap;
        public string startTime;
        public string startString;
        public string title;
        public string tableSpecId;
        public string entryFee;
        public string entryFeeFormatted;
        public string prizes;
        public string prizeBreakdown;
        public string prizeSummary;
        public string size;
        public string maxEntriesPerUser;
        public FantasyLeagueJSONObject_flags flags;
        public string seatCode;
        public object entriesData;
        public string dateUpdated;
    }

    class FantasyLeagueJSONObject_flags
    {
        public int standard;
    }
}

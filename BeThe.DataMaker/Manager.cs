using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeThe.Item;
using HtmlAgilityPack;

namespace BeThe.DataMaker
{
    public class Manager
    {
        private String[] separator = new String[] { "=" };
        private String[] separatorComma = new String[] { "," };
        private String[] separatorSlush = new String[] { "/" };
        private String[] separatorBlank = new String[] { " " };

        #region Public Functions

        public Match MakeMatch(Situation_W situation, BoxScore_W boxScore)
        {
            // MatchInfo 만들기
            var matchInfo = MakeMatchInfo(boxScore);

            // Make Match
            Match match = new Match();
            match.GameId = situation.GameId;

            // MakeThs
            match.Ths = MakeThs(match, situation, matchInfo);

            return match;
        }

        #endregion

        #region Private Functions

        private List<Th> MakeThs(Match match, Situation_W situation, MatchInfo matchInfo)
        {
            String[] separator = new String[] { "회" };
            List<Th> ths = new List<Th>();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(situation.Content);

            for (Int32 i = 1; i <= 12; ++i)
            {
                String id = String.Format("sms{0:D2}", i);
                var allNode = doc.DocumentNode.SelectSingleNode(String.Format("//*[@id='{0}']", id));

                if (allNode != null)
                {
                    var tThs = MakeTh(match, allNode.OuterHtml, i, matchInfo);
                    ths = ths.Concat(tThs).ToList();
                }
            }
            return ths;
        }

        private List<Th> MakeTh(Match match, String content, Int32 number, MatchInfo matchInfo)
        {
            List<Th> ths = new List<Th>();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(content);
            var nodes = doc.DocumentNode.SelectNodes("//*[@class='tEx Ex2']");

            if (nodes == null)
            {
                return ths;
            }
            if (nodes.Count > 0)
            {
                // 초공격
                var bats = MakeBatts(match, nodes[0].OuterHtml, number, matchInfo, ThType.초);
                if (bats != null)
                {
                    Th th = new Th();
                    th.ThType = ThType.초;
                    th.Number = number;
                    th.Bats = bats;
                    ths.Add(th);
                }
            }

            if (nodes.Count > 1)
            {
                var bats = MakeBatts(match, nodes[1].OuterHtml, number, matchInfo, ThType.말);
                if (bats != null)
                {
                    Th th = new Th();
                    th.ThType = ThType.말;
                    th.Number = number;
                    th.Bats = bats;
                    ths.Add(th);
                }
            }

            return ths;
        }

        private List<Bat> MakeBatts(Match match, String content, Int32 number, MatchInfo matchInfo, ThType type)
        {
            List<Bat> bats = new List<Bat>();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(content);
            var nodes = doc.DocumentNode.SelectNodes("/table[@class='tEx Ex2']/tbody[1]/tr/td");

            if (nodes == null)
            {
                return null;
            }

            foreach (var node in nodes)
            {
                var bat = MakeBat(node.OuterHtml, number, matchInfo, type);
                if (bat != null)
                {
                    bats.Add(bat);
                }
            }

            return bats;
        }

        private Bat MakeBat(String content, Int32 number, MatchInfo matchInfo, ThType type)
        {
            try
            {
                Bat bat = new Bat();
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(content);

                // Hitter Id 
                var node = doc.DocumentNode.SelectSingleNode("//*[@class='btn_detail detail_minus']");
                String item = node.GetAttributeValue("onclick", "");
                String[] items = item.Split(separatorComma, StringSplitOptions.RemoveEmptyEntries);
                bat.HitterId = Convert.ToInt32(items[1]);

                Int32 count = 0;
                List<PitcherInfo> pitcherInfos = null;
                List<HitterInfo> hitterInfos = null;
                if (type == ThType.초)
                {
                    pitcherInfos = matchInfo.HomeTeamPitcherInfos;
                    hitterInfos = matchInfo.AwayTeamHitterInfos;
                    count = matchInfo.AwayHitterCount++;
                }
                else
                {
                    pitcherInfos = matchInfo.AwayTeamPitcherInfos;
                    hitterInfos = matchInfo.HomeTeamHitterInfos;
                    count = matchInfo.HomeHitterCount++;
                }

                // Pitcher Id
                Int32 sum = 0;
                foreach (var pitcherInfo in pitcherInfos)
                {
                    sum += pitcherInfo.BatterCount;
                    if (count < sum)
                    {
                        bat.PitcherId = pitcherInfo.PlayerId;
                        break;
                    }
                }

                // 세부 결과
                node = doc.DocumentNode.SelectSingleNode("//td");
                var delNode = node.SelectSingleNode("//table");
                node.RemoveChild(delNode);
                delNode = node.SelectSingleNode("//span");
                node.RemoveChild(delNode);

                bat.DetailResult = node.InnerText.Trim();

                // 결과가 나지 않은 타석 제거
                String[] datas = bat.DetailResult.Split(separatorBlank, StringSplitOptions.RemoveEmptyEntries);
                if (datas.Count() == 4)
                {
                    if (datas[0] == datas[2] && datas[1] == datas[3])
                    {
                        if (type == ThType.초)
                        {
                            matchInfo.AwayHitterCount--;
                        }
                        else
                        {

                            matchInfo.HomeHitterCount--;
                        }
                        return null;
                    }
                }

                // 결과
                var hitterResults = (from p in hitterInfos
                                     where p.PlayerId == bat.HitterId
                                     select p.HitterResults).First();

                var hitterResult = (from r in hitterResults
                                    where r.Number == number
                                    select r).First();

                bat.Result = hitterResult.Result;

                //1 or 2 or 3 or 안 or 홈 으로끈나는것들
                //Pass -> '희' 포함 or 4구, 타방, 고4, 4구
                if (bat.Result.EndsWith("1") || bat.Result.EndsWith("2") || bat.Result.EndsWith("3")
                    || bat.Result.EndsWith("안") || bat.Result.EndsWith("홈"))
                {
                    bat.PResult = PResultType.Hit;
                }
                else if (bat.Result.EndsWith("희") || bat.Result.EndsWith("4구") || bat.Result.EndsWith("타방")
                    || bat.Result.EndsWith("고4"))
                {
                    bat.PResult = PResultType.Pass;
                }
                else
                {
                    bat.PResult = PResultType.Out;
                }

                hitterResults.Remove(hitterResult);

                // Balls 
                bat.Balls = MakeBalls(content);
                return bat;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private List<Ball> MakeBalls(String content)
        {
            List<Ball> balls = new List<Ball>();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(content);
            var nodes = doc.DocumentNode.SelectNodes("//tbody//tr");
            foreach (var node in nodes)
            {
                var ball = MakeBall(node.OuterHtml);
                if (ball != null)
                {
                    balls.Add(ball);
                }
            }
            return balls;
        }

        private Ball MakeBall(String content)
        {
            Ball ball = new Ball();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(content);
            var nodes = doc.DocumentNode.SelectNodes("//td");

            ball.Number = ConvertToInt32(nodes[0].InnerHtml.Replace("구", ""));
            ball.Speed = ConvertToInt32(nodes[1].InnerHtml.Replace("km", ""));
            ball.BallType = nodes[2].InnerHtml.Trim();
            ball.Result = nodes[3].InnerHtml.Trim();
            ball.BallCount = nodes[4].InnerHtml.Trim();

            return ball;
        }

        private Int32? ConvertToInt32(String str)
        {
            try
            {
                return Convert.ToInt32(str);
            }
            catch
            {
                return null;
            }
        }
        #region Match Info

        private MatchInfo MakeMatchInfo(BoxScore_W boxScore)
        {
            MatchInfo matchInfo = new MatchInfo();
            matchInfo.HomeTeamPitcherInfos = GetPitcherInfos(boxScore.HomePitcher);
            matchInfo.AwayTeamPitcherInfos = GetPitcherInfos(boxScore.AwayPitcher);
            matchInfo.HomeTeamHitterInfos = GetBatterInfos(boxScore.HomeHitter);
            matchInfo.AwayTeamHitterInfos = GetBatterInfos(boxScore.AwayHitter);
            return matchInfo;
        }

        // 투수정보 얻어오기
        private List<PitcherInfo> GetPitcherInfos(String content)
        {
            List<PitcherInfo> pitcherInfos = new List<PitcherInfo>();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(content);
            var nodes = doc.DocumentNode.SelectSingleNode("//tbody").SelectNodes("tr");

            foreach (var node in nodes)
            {
                HtmlDocument pitcherDoc = new HtmlDocument();
                pitcherDoc.LoadHtml(node.OuterHtml);

                // Player ID 얻어오기
                String href = pitcherDoc.DocumentNode.SelectSingleNode("//a").GetAttributeValue("href", "");
                String[] items = href.Split(separator, StringSplitOptions.RemoveEmptyEntries);

                PitcherInfo pitcherInfo = new PitcherInfo();
                pitcherInfo.PlayerId = Convert.ToInt32(items[1]);

                // 타자수 얻어오기
                pitcherInfo.BatterCount = Convert.ToInt32(pitcherDoc.DocumentNode.SelectNodes("//td")[6].InnerText);

                pitcherInfos.Add(pitcherInfo);
            }
            return pitcherInfos;
        }

        // 타자정보 얻어오기
        private List<HitterInfo> GetBatterInfos(String content)
        {
            List<HitterInfo> HitterInfos = new List<HitterInfo>();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(content);
            var nodes = doc.DocumentNode.SelectSingleNode("//tbody").SelectNodes("tr");

            foreach (var node in nodes)
            {
                HtmlDocument hitterDoc = new HtmlDocument();
                hitterDoc.LoadHtml(node.OuterHtml);

                // Player ID 얻어오기
                String href = hitterDoc.DocumentNode.SelectSingleNode("//a").GetAttributeValue("href", "");
                String[] items = href.Split(separator, StringSplitOptions.RemoveEmptyEntries);

                HitterInfo htterInfo = new HitterInfo();
                htterInfo.HitterResults = new List<HitterResult>();
                htterInfo.PlayerId = Convert.ToInt32(items[1]);

                // 결과 얻어오기
                var resultNodes = hitterDoc.DocumentNode.SelectNodes("//td");
                for (Int32 i = 1; i <= 12; ++i)
                {
                    String result = resultNodes[i].InnerHtml;
                    String[] results = result.Split(separatorSlush, StringSplitOptions.RemoveEmptyEntries);

                    foreach (String r in results)
                    {
                        if (String.IsNullOrEmpty(result) == false)
                        {
                            HitterResult hitterResult = new HitterResult();
                            hitterResult.Number = i;
                            hitterResult.Result = r.Trim();
                            htterInfo.HitterResults.Add(hitterResult);
                        }
                    }
                }
                HitterInfos.Add(htterInfo);
            }

            return HitterInfos;
        }

        #endregion

        #endregion

    }
}

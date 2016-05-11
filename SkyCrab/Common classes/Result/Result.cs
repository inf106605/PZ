using System;

namespace SkyCrab.Common_classes.Result
{
    public class Result
    {

        private uint gameId;
        private string roomName;
        private DateTime beginTime;
        private DateTime endTime;
        private Score[] scores;


        public uint GameId
        {
            get { return gameId; }
        }

        public string RoomName
        {
            get { return roomName; }
        }

        public DateTime BeginTime
        {
            get { return beginTime; }
        }

        public DateTime EndTime
        {
            get { return endTime; }
        }

        public Score[] Scores
        {
            get { return scores; }
        }


        public Result(uint gameId, string roomName, DateTime beginTime, DateTime endTime, Score[] scores)
        {
            this.gameId = gameId;
            this.roomName = roomName;
            this.beginTime = beginTime;
            this.endTime = endTime;
            this.scores = scores;
        }

    }
}

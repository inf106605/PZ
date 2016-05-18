using SkyCrab.Common_classes.Games.Boards;
using SkyCrab.Common_classes.Games.Players;
using SkyCrab.Common_classes.Rooms;
using SkyCrab.Common_classes.Rooms.Players;
using System;

namespace SkyCrab.Common_classes.Games
{
    public sealed class FirstPlayerNotSelectedException : SkyCrabException
    {
    }

    public sealed class NoSuchPlayerInGameException : SkyCrabException
    {
        public NoSuchPlayerInGameException(UInt32 playerId) :
            base("There is not player with ID " + playerId + " in this game!")
        {
        }
    }

    public sealed class NoThatManyPlayersInGameException : SkyCrabException
    {
        public NoThatManyPlayersInGameException(uint playerNumber) :
            base("There is less than " + playerNumber + " players in this game!")
        {
        }
    }

    public class ThereIsNoAnyActivePlayerException : SkyCrabException
    {
    }

    public class PlayerDidWalkoverException : SkyCrabException
    {
    }

    class Game
    {

        private UInt32 id;
        private Room room;
        private Board board;
        private PlayerInGame[] players;
        private uint firstPlayerNumber = uint.MaxValue;
        private uint currentPlayerNumber = uint.MaxValue;
        private Pouch.Pouch[] pouches;
        private bool isDummy;
        private bool isFinished = false;
        private uint turnNumber = 0;
        private uint fullRoundNumber = 0;


        public UInt32 Id
        {
            get { return id; }
        }

        public Room Room
        {
            get { return room; }
        }

        public Board Board
        {
            get { return board; }
        }

        public PlayerInGame[] Players
        {
            get { return players; }
        }

        public uint FirstPlayerNumber
        {
            get { return firstPlayerNumber; }
        }

        public uint CurrentPlayerNumber
        {
            get { return currentPlayerNumber; }
            set {
                if (value > players.Length)
                    throw new NoThatManyPlayersInGameException(value);
                if (players[value].Walkover)
                    throw new PlayerDidWalkoverException();
                if (currentPlayerNumber == uint.MaxValue)
                    firstPlayerNumber = value;
                currentPlayerNumber = value;
            }
        }

        public uint ActivePlayersNumber
        {
            get
            {
                uint result = 0;
                foreach (PlayerInGame playerInGame in players)
                    if (!playerInGame.Walkover)
                        ++result;
                return result;
            }
        }

        public PlayerInGame FirstPlayer
        {
            get
            {
                if (firstPlayerNumber == uint.MaxValue)
                    return null;
                else
                    return players[firstPlayerNumber];
            }
        }

        public PlayerInGame CurrentPlayer
        {
            get {
                if (currentPlayerNumber == uint.MaxValue)
                    return null;
                else
                    return players[currentPlayerNumber];
            }
        }

        public UInt32 CurrentPlayerId
        {
            get
            {
                if (currentPlayerNumber == uint.MaxValue)
                    throw new FirstPlayerNotSelectedException();
                return CurrentPlayer.Player.Id;
            }
            set
            {
                for (uint i = 0; i != players.Length; ++i)
                    if (players[i].Player.Id == value)
                    {
                        CurrentPlayerNumber = i;
                        return;
                    }
                throw new NoSuchPlayerInGameException(value);
            }
        }

        public Pouch.Pouch[] Puoches
        {
            get { return pouches; }
        }

        public bool IsDummy
        {
            get { return isDummy; }
        }

        public bool IsFinished
        {
            get { return isFinished; }
        }

        public uint TurnNumber
        {
            get { return turnNumber; }
        }

        public uint FullRoundNumber
        {
            get { return fullRoundNumber; }
        }


        public Game(UInt32 id, Room room, bool isDummy)
        {
            this.id = id;
            this.room = room;
            this.board = room.Rules.CreateBoard();
            this.players = new PlayerInGame[room.Players.Count];
            uint i = 0;
            foreach (PlayerInRoom playerInRoom in room.Players)
                this.players[i++] = new PlayerInGame(playerInRoom.Player);
            this.pouches = room.Rules.CreatePouches(isDummy);
            this.isDummy = isDummy;
        }

        public PlayerInGame GetPlayer(UInt32 playerId)
        {
            foreach (PlayerInGame playerInGame in players)
                if (playerInGame.Player.Id == playerId)
                    return playerInGame;
            throw new NoSuchPlayerInGameException(playerId);
        }

        public void SwitchToNextPlayer(bool pass)
        {
            if (currentPlayerNumber == uint.MaxValue)
                throw new FirstPlayerNotSelectedException();
            if (pass)
                CurrentPlayer.IncrementPassCount();
            else
                CurrentPlayer.ResetPassCount();
            ++turnNumber;
            for (uint i = currentPlayerNumber + 1; i != players.Length; ++i)
            {
                if (i == firstPlayerNumber)
                    ++fullRoundNumber;
                if (!players[i].Walkover)
                {
                    currentPlayerNumber = i;
                    return;
                }
            }
            for (uint i = 0; i != currentPlayerNumber + 1; ++i)
            {
                if (i == firstPlayerNumber)
                    ++fullRoundNumber;
                if (!players[i].Walkover)
                {
                    currentPlayerNumber = i;
                    return;
                }
            }
            throw new ThereIsNoAnyActivePlayerException();
        }

        public void FinishGame()
        {
            isFinished = true;
        }
        
    }
}

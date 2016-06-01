using SkyCrab.Connection.PresentationLayer.Messages;
using SkyCrab.Connection.PresentationLayer.Messages.Common.Errors;
using SkyCrab.Connection.PresentationLayer.Messages.Menu.Games;
using SkyCrabServer.Databases;
using System;

namespace SkyCrabServer.ServerLogics
{
    class ServerGameResult
    {
        private readonly ServerPlayer serverPlayer;


        public ServerGameResult(ServerPlayer serverPlayer)
        {
            this.serverPlayer = serverPlayer;
        }

        public void GetGameLog(Int16 id, UInt32 gameId)
        {
            if (!GameTable.IdExists(gameId))
                ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.NO_SUCH_GAME);
            else if (!GameTable.IsEnded(gameId))
                ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.GAME_NOT_ENDED);
            else
                GameLogMsg.AsyncPost(id, serverPlayer.connection, GameTable.GetLogById(gameId));
        }
    }
}

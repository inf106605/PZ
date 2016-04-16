using SkyCrab.Common_classes.Players;
using System;

namespace SkyCrab.Connection.PresentationLayer.Messages.Menu
{
    public sealed class LoginOk : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.LOGIN_OK; }
        }

        internal override bool Answer
        {
            get { return true; }
        }

        internal override object Read(MessageConnection connection)
        {
            uint id = connection.SyncReadData(MessageConnection.uint32Transcoder);
            PlayerProfile playerProfile = ReadPlayerProfile(connection);
            Player player = new Player(id, false, playerProfile.nick);
            player.Profile = playerProfile;
            return player;
        }

        private static PlayerProfile ReadPlayerProfile(MessageConnection connection)
        {
            string login = connection.SyncReadData(MessageConnection.stringTranscoder);
            string nick = connection.SyncReadData(MessageConnection.stringTranscoder);
            string eMail = connection.SyncReadData(MessageConnection.stringTranscoder);
            DateTime registration = connection.SyncReadData(MessageConnection.dateTimeTranscoder);
            DateTime lastActivity = connection.SyncReadData(MessageConnection.dateTimeTranscoder);
            PlayerProfile playerProfile = new PlayerProfile();
            playerProfile.login = login;
            playerProfile.nick = nick;
            playerProfile.eMail = eMail;
            playerProfile.registration = registration;
            playerProfile.lastActivity = lastActivity;
            return playerProfile;
        }

        public static void AsyncPostLoginOk(MessageConnection connection, Player player)
        {
            MessageConnection.MessageProcedure messageProcedure = (writingBlock) =>
            {
                connection.AsyncWriteData(MessageConnection.uint32Transcoder, writingBlock, player.Id);
                PostPlayerProfile(connection, writingBlock, player.Profile);
            };
            connection.PostMessage(MessageId.LOGIN_OK, messageProcedure);
        }

        private static void PostPlayerProfile(MessageConnection connection, object writingBlock, PlayerProfile playerProfile)
        {
            connection.AsyncWriteData(MessageConnection.stringTranscoder, writingBlock, playerProfile.login);
            connection.AsyncWriteData(MessageConnection.stringTranscoder, writingBlock, playerProfile.nick);
            connection.AsyncWriteData(MessageConnection.stringTranscoder, writingBlock, playerProfile.eMail);
            connection.AsyncWriteData(MessageConnection.dateTimeTranscoder, writingBlock, playerProfile.registration);
            connection.AsyncWriteData(MessageConnection.dateTimeTranscoder, writingBlock, playerProfile.lastActivity);
        }

    }
}

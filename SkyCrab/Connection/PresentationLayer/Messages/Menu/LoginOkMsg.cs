using SkyCrab.Common_classes.Players;
using System;

namespace SkyCrab.Connection.PresentationLayer.Messages.Menu
{
    /// <summary>
    /// <para>Sender: Server</para>
    /// <para>ID: <see cref="MessageId.LOGIN_OK"/></para>
    /// <para>Data type: <see cref="Player"/> (without password)</para>
    /// <para>Passible answers: [none]</para>
    /// </summary>
    public sealed class LoginOkMsg : AbstractMessage
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
            Player player = new Player(id, playerProfile);
            return player;
        }

        private static PlayerProfile ReadPlayerProfile(MessageConnection connection)
        {
            string login = connection.SyncReadData(MessageConnection.loginTranscoder);
            string nick = connection.SyncReadData(MessageConnection.nickTranscoder);
            string eMail = connection.SyncReadData(MessageConnection.eMailTranscoder);
            DateTime registration = connection.SyncReadData(MessageConnection.dateTimeTranscoder);
            DateTime lastActivity = connection.SyncReadData(MessageConnection.dateTimeTranscoder);
            PlayerProfile playerProfile = new PlayerProfile();
            playerProfile.Login = login;
            playerProfile.Nick = nick;
            playerProfile.EMail = eMail;
            playerProfile.Registration = registration;
            playerProfile.LastActivity = lastActivity;
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
            connection.AsyncWriteData(MessageConnection.loginTranscoder, writingBlock, playerProfile.Login);
            connection.AsyncWriteData(MessageConnection.nickTranscoder, writingBlock, playerProfile.Nick);
            connection.AsyncWriteData(MessageConnection.eMailTranscoder, writingBlock, playerProfile.EMail);
            connection.AsyncWriteData(MessageConnection.dateTimeTranscoder, writingBlock, playerProfile.Registration);
            connection.AsyncWriteData(MessageConnection.dateTimeTranscoder, writingBlock, playerProfile.LastActivity);
        }

    }
}

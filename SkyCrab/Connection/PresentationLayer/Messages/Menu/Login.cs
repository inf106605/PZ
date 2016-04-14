using SkyCrab.Common_classes.Players;

namespace SkyCrab.Connection.PresentationLayer.Messages.Menu
{
    public sealed class Login : AbstractMessage
    {

        public override MessageId Id
        {
            get
            {
                return MessageId.LOGIN;
            }
        }

        internal override bool Answer
        {
            get
            {
                return false;
            }
        }


        internal override object Read(MessageConnection connection)
        {
            string login = connection.SyncReadData(MessageConnection.stringTranscoder);
            string password = connection.SyncReadData(MessageConnection.stringTranscoder);
            PlayerProfile playerProfile = new PlayerProfile();
            playerProfile.login = login;
            playerProfile.password = password;
            return playerProfile;
        }

        public static void PostLogin(MessageConnection connection, PlayerProfile playerProfile, MessageConnection.AnswerCallback callback, object state = null)
        {
            MessageConnection.MessageProcedure messageProcedure = (object writingBlock) =>
            {
                connection.AsyncWriteData(MessageConnection.stringTranscoder, writingBlock, playerProfile.login);
                connection.AsyncWriteData(MessageConnection.stringTranscoder, writingBlock, playerProfile.password);
                connection.SetAnswerCallback(writingBlock, callback, state);
            };
            connection.PostMessage(MessageId.LOGIN, messageProcedure);
        }

    }
}

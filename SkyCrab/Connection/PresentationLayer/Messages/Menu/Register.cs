using SkyCrab.Common_classes.Players;

namespace SkyCrab.Connection.PresentationLayer.Messages.Menu
{
    public sealed class Register : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.REGISTER; }
        }

        internal override bool Answer
        {
            get { return false; }
        }

        internal override object Read(MessageConnection connection)
        {
            string login = connection.SyncReadData(MessageConnection.stringTranscoder);
            string password = connection.SyncReadData(MessageConnection.stringTranscoder);
            string eMail = connection.SyncReadData(MessageConnection.stringTranscoder);
            PlayerProfile playerProfile = new PlayerProfile();
            playerProfile.login = login;
            playerProfile.password = password;
            playerProfile.eMail = eMail;
            return playerProfile;
        }

        public static void PostRegister(MessageConnection connection, PlayerProfile playerProfile, MessageConnection.AnswerCallback callback, object state = null)
        {
            MessageConnection.MessageProcedure messageProc = (object writingBlock) =>
            {
                connection.AsyncWriteData(MessageConnection.stringTranscoder, writingBlock, playerProfile.login);
                connection.AsyncWriteData(MessageConnection.stringTranscoder, writingBlock, playerProfile.password);
                connection.AsyncWriteData(MessageConnection.stringTranscoder, writingBlock, playerProfile.eMail);
                connection.SetAnswerCallback(writingBlock, callback, state);
            };
            connection.PostMessage(MessageId.REGISTER, messageProc);
        }

    }
}

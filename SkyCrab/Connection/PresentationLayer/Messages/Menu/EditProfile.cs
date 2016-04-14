using SkyCrab.Common_classes.Players;

namespace SkyCrab.Connection.PresentationLayer.Messages.Menu
{
    public sealed class EditProfile : AbstractMessage
    {

        public override MessageId Id
        {
            get
            {
                return MessageId.EDIT_PROFILE;
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
            string password = connection.SyncReadData(MessageConnection.stringTranscoder);
            string nick = connection.SyncReadData(MessageConnection.stringTranscoder);
            string eMail = connection.SyncReadData(MessageConnection.stringTranscoder);
            PlayerProfile playerProfile = new PlayerProfile();
            playerProfile.password = password;
            playerProfile.nick = nick;
            playerProfile.eMail = eMail;
            return playerProfile;
        }

        public static void PostEditProfile(MessageConnection connection, PlayerProfile playerProfile, MessageConnection.AnswerCallback callback, object state = null)
        {
            MessageConnection.MessageProcedure messageProc = (object writingBlock) =>
            {
                connection.AsyncWriteData(MessageConnection.stringTranscoder, writingBlock, playerProfile.password);
                connection.AsyncWriteData(MessageConnection.stringTranscoder, writingBlock, playerProfile.nick);
                connection.AsyncWriteData(MessageConnection.stringTranscoder, writingBlock, playerProfile.eMail);
                connection.SetAnswerCallback(writingBlock, callback, state);
            };
            connection.PostMessage(MessageId.EDIT_PROFILE, messageProc);
        }

    }
}

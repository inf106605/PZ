using SkyCrab.Common_classes.Players;

namespace SkyCrab.Connection.PresentationLayer.Messages.Menu
{
    /// <summary>
    /// <para>Sender: Client</para>
    /// <para>ID: <see cref="MessageId.REGISTER"/></para>
    /// <para>Data type: <see cref="PlayerProfile"/> (without nick, registration and lastActivity)</para>
    /// <para>Passible answers: <see cref="LoginOk"/>, <see cref="Error"/></para>
    /// <para>Error codes: <see cref="ErrorCode.LOGIN_OCCUPIED"/>, <see cref="ErrorCode.PASSWORD_TOO_SHORT"/>, <see cref="ErrorCode.EMAIL_OCCUPIED"/></para>
    /// </summary>
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
        
        public static MessageConnection.MessageInfo? SyncPostRegister(MessageConnection connection, PlayerProfile playerProfile, int timeout)
        {
            return SyncPost((callback, state) => AsyncPostRegister(connection, playerProfile, callback, state), timeout);
        }

        public static void AsyncPostRegister(MessageConnection connection, PlayerProfile playerProfile, MessageConnection.AnswerCallback callback, object state = null)
        {
            MessageConnection.MessageProcedure messageProc = (writingBlock) =>
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

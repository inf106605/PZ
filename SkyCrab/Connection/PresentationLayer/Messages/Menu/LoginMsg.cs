using SkyCrab.Common_classes.Players;

namespace SkyCrab.Connection.PresentationLayer.Messages.Menu
{
    /// <summary>
    /// <para>Sender: Client</para>
    /// <para>ID: <see cref="MessageId.LOGIN"/></para>
    /// <para>Data type: <see cref="PlayerProfile"/> (login and password only)</para>
    /// <para>Passible answers: <see cref="LoginOkMsg"/>, <see cref="ErrorMsg"/></para>
    /// <para>Error codes: <see cref="ErrorCode.WRONG_LOGIN_OR_PASSWORD"/>, <see cref="ErrorCode.USER_ALREADY_LOGGED", <see cref="ErrorCode.SESSION_ALREADY_LOGGED"/></para>
    /// </summary>
    public sealed class LoginMsg : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.LOGIN; }
        }

        internal override bool Answer
        {
            get { return false; }
        }


        internal override object Read(MessageConnection connection)
        {
            string login = connection.SyncReadData(MessageConnection.stringTranscoder);
            string password = connection.SyncReadData(MessageConnection.stringTranscoder);
            PlayerProfile playerProfile = new PlayerProfile();
            playerProfile.Login = login;
            playerProfile.Password = password;
            return playerProfile;
        }

        public static MessageConnection.MessageInfo? SyncPostLogin(MessageConnection connection, PlayerProfile playerProfile, int timeout)
        {
            return SyncPost((callback, state) => AsyncPostLogin(connection, playerProfile, callback, state), timeout);
        }

        public static void AsyncPostLogin(MessageConnection connection, PlayerProfile playerProfile, MessageConnection.AnswerCallback callback, object state = null)
        {
            MessageConnection.MessageProcedure messageProcedure = (writingBlock) =>
            {
                connection.AsyncWriteData(MessageConnection.stringTranscoder, writingBlock, playerProfile.Login);
                connection.AsyncWriteData(MessageConnection.stringTranscoder, writingBlock, playerProfile.Password);
                connection.SetAnswerCallback(writingBlock, callback, state);
            };
            connection.PostMessage(MessageId.LOGIN, messageProcedure);
        }

    }
}

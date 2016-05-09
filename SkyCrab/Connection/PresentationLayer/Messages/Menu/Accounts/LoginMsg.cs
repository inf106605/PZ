using SkyCrab.Common_classes.Players;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes.Players;
using SkyCrab.Connection.PresentationLayer.MessageConnections;

namespace SkyCrab.Connection.PresentationLayer.Messages.Menu.Accounts
{
    /// <summary>
    /// <para>Sender: Client</para>
    /// <para>ID: <see cref="MessageId.LOGIN"/></para>
    /// <para>Data type: <see cref="PlayerProfile"/> (login and password only)</para>
    /// <para>Passible answers: <see cref="LoginOkMsg"/>, <see cref="ErrorMsg"/></para>
    /// <para>Error codes: <see cref="ErrorCode.SESSION_ALREADY_LOGGED"/>, <see cref="ErrorCode.WRONG_LOGIN_OR_PASSWORD"/>, <see cref="ErrorCode.USER_ALREADY_LOGGED"/></para>
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
            PlayerProfile playerProfile = PlayerProfileTranscoder.Get.Read(connection);
            return playerProfile;
        }

        public static MessageInfo? SyncPost(MessageConnection connection, PlayerProfile playerProfile, int timeout)
        {
            return AsyncPostToSyncPost((callback, state) => AsyncPost(connection, playerProfile, callback, state), timeout);
        }

        public static void AsyncPost(MessageConnection connection, PlayerProfile playerProfile, AnswerCallback callback, object state = null)
        {
            MessageConnection.MessageProcedure messageProcedure = (writingBlock) =>
            {
                PlayerProfileTranscoder.Get.Write(connection, writingBlock, playerProfile);
            };
            connection.PostNewMessage(MessageId.LOGIN, messageProcedure, callback, state);
        }

    }
}
